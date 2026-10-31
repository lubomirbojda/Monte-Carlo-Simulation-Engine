# Courier Delivery Route Optimization (Monte Carlo Simulation Engine)

[![C#](https://img.shields.io/badge/Language-C%23%2014-239120.svg)](https://learn.microsoft.com/en-us/dotnet/csharp/)
[![.NET 10](https://img.shields.io/badge/Framework-.NET%2010.0-512BD4.svg)](https://dotnet.microsoft.com/en-us/download/dotnet/10.0)
[![UI](https://img.shields.io/badge/UI-WinForms%20(Windows)-0078D6.svg)](https://learn.microsoft.com/en-us/dotnet/desktop/winforms/)
[![Plotting](https://img.shields.io/badge/Plotting-ScottPlot%205.1-6B46C1.svg)](https://scottplot.net/)
[![IDE](https://img.shields.io/badge/IDE-Visual%20Studio-purple.svg)](https://visualstudio.microsoft.com/)
[![Course](https://img.shields.io/badge/Course-Discrete%20Simulation-orange.svg)](https://fri.uniza.sk)

High-performance C# / Windows Forms desktop application and custom **static Monte Carlo simulation engine** with **uniform and empirical random variate generators**, designed to model stochastic road networks, traffic congestion, and optimal courier delivery routing in the Žilina region.

This project was developed as a university semester project (**Semester Project S1**) for the **Discrete Simulation (Diskrétna simulácia)** course at the Faculty of Management Science and Informatics, University of Žilina (**FRI UNIZA**).

---

## Table of Contents

- [Overview](#overview)
- [Key Features](#key-features)
- [Simulation Architecture & Random Generators](#simulation-architecture--random-generators)
- [Stochastic Road Network & Traffic Model](#stochastic-road-network--traffic-model)
- [Implemented Simulation Tasks & Experimental Results](#implemented-simulation-tasks--experimental-results)
- [Installation & How to Run](#installation--how-to-run)
- [GUI Controls & Usage](#gui-controls--usage)
- [Documentation](#documentation)
- [Screenshots](#screenshots)

---

## Overview

A food wholesale warehouse located in **Žilina (ZA)** dispatches a courier vehicle every morning to supply stores in three surrounding municipalities: **Divinka (D)**, **Strečno (S)**, and **Rajecké Teplice (RT)** before returning to Žilina. Historical GPS telemetry from the delivery vehicle defines specific probability distributions for average travel speeds across four distinct categories of road segments (Red, Green, Black, and Blue), as well as time-dependent morning traffic congestion at central intersection **K**.

The simulation engine is implemented from scratch using only standard `.NET` libraries (`System.Random`) for base pseudo-random number generation, featuring:
1. **Generic Random Variate Generators (`BaseUniform<T>` & `BaseEmpiric<T, TInterval>`):** Custom implementations of discrete uniform, continuous uniform, discrete empirical, and continuous empirical distributions utilizing C# generic math (`INumber<T>`) and hierarchical seed propagation.
2. **Reusable Monte Carlo Framework (`BaseMonteCarlo`):** A domain-independent template-method simulation core orchestrating replication lifecycles (`BeforeSimulation`, `BeforeReplication`, `DoReplication`, `AfterReplication`, `AfterSimulation`) with asynchronous execution and instant early stopping.

---

## Key Features

- **Hierarchical Seed Management & Custom PRNG Suite:**
  - A single master `Random` seed generator deterministically initializes dedicated `Random` instances for every distribution and every empirical sub-interval, guaranteeing full reproducibility when a custom seed is provided.
  - Built-in statistical verification suite (`GeneratorsTester`) exporting `1,000,000` samples per distribution for histogram validation.
- **Flexible Multi-Interval Empirical Distributions (`BaseEmpiric<T, TInterval>`):**
  - Supports arbitrary numbers of disjoint or contiguous sub-intervals `([min_i, max_i), p_i)` with automatic validation that the sum of probabilities equals `1.0`.
  - Cumulative probability lookup selects the target interval via `r ∈ [0, 1)`, followed by uniform sampling within the selected interval's bounds.
- **Dynamic Shortest-Time Sub-Route Selection (`Map`):**
  - Whenever multiple alternative paths connect two municipalities, the courier dynamically evaluates the generated traversal times for the current replication and selects the fastest option (`GetBestOption`).
- **Time-Dependent Traffic Congestion at Node K:**
  - Models morning traffic jams forming at intersection **K**: if the courier departs node **K** after the configured congestion threshold time, the generated speed on the outgoing road segment is reduced by a random percentage drawn from a continuous uniform distribution `U ∈ [10%, 25%)`.
- **Real-Time Convergence Visualization (`ScottPlot.WinForms`):**
  - Live plotting of the cumulative average delivery completion time across all 6 route permutations simultaneously.
  - Automatically filters out the initial **20% warm-up replications** (`_warmUp = 0.2 * _replicationsCount`) to eliminate initial variance spikes and refreshes the signal chart every `1,000` replications with dynamic Y-axis scaling.
- **Strict Core / GUI Separation:**
  - Complete decoupling between the simulation library (`DS_SEM_1`) and the multi-tab Windows Forms presentation layer (`GUI`), with background worker threads (`Task.Run`) ensuring responsive UI controls and non-blocking simulation cancellation.

---

## Simulation Architecture & Random Generators

### Class Hierarchy

```
BaseUniform<T> (where T : INumber<T>)
 ├── DiscreteUniform          (int: [min, max] inclusive)
 └── ContinuousUniform        (double: [min, max) half-open)

BaseInterval<T> (where T : INumber<T>)
 ├── IntervalDiscrete         (int sub-interval generator)
 └── IntervalContinuous       (double sub-interval generator)

BaseEmpiric<T, TInterval>
 ├── DiscreteEmpiric          (manages List<IntervalDiscrete>)
 └── ContinuousEmpiric        (manages List<IntervalContinuous>)

BaseMonteCarlo
 └── TransporterMC            (Task 1: single route convergence & mean return time)
      ├── TransporteMCTask2          (Task 2: 80th percentile duration & latest departure time)
      └── TransporteMCTask2Percentil (Task 2 Verification: % of trips arriving by 07:35)

TransporterAllRoutesMC        (Orchestrates all 6 route permutations & Task 2 experiments)
 └── Map                      (Road network topology, speed generators & node K congestion logic)
```

### Implemented Probability Distributions

| Generator Class | Distribution Type | Sampling Formula | Road Segment Usage |
| :--- | :--- | :--- | :--- |
| **`DiscreteUniform`** | Discrete Uniform | `X = ⌊r · (U_max - U_min + 1)⌋ + U_min`, where `X ∈ {U_min, ..., U_max}` | **Red** segments (`U_min = 55`, `U_max = 75 km/h`) |
| **`ContinuousUniform`** | Continuous Uniform | `X = r · (U_max - U_min) + U_min`, where `X ∈ [U_min, U_max)` | **Green** segments (`[50, 80) km/h`) & **Node K slowdown** (`[10, 25)%`) |
| **`ContinuousEmpiric`** | Continuous Empirical | Interval `i` chosen if `C_{i-1} ≤ r₁ < C_i`; value `X = r₂ · (max_i - min_i) + min_i` | **Black** segments (`5` sub-intervals) |
| **`DiscreteEmpiric`** | Discrete Empirical | Interval `i` chosen if `C_{i-1} ≤ r₁ < C_i`; value `X = ⌊r₂ · (max_i - min_i)⌋ + min_i` | **Blue** segments (`3` sub-intervals) |

---

## Stochastic Road Network & Traffic Model

### 1. Segment Speed Distributions

Based on historical GPS records from the courier vehicle, average speeds `U` (`km/h`) on individual road segments follow these probability distributions:

| Segment Color | Distribution Type | Speed Parameters / Sub-Intervals (`km/h`) | Probability (`p_i`) | Cumulative (`C_i`) |
| :--- | :--- | :--- | :---: | :---: |
| **Red** (*Červený*) | Discrete Uniform | `U_min = 55`, `U_max = 75` | `1.00` | `1.00` |
| **Green** (*Zelený*) | Continuous Uniform | `U ∈ [50, 80)` | `1.00` | `1.00` |
| **Black** (*Čierny*) | Continuous Empirical | `U₁ ∈ [10, 20)`<br>`U₂ ∈ [20, 32)`<br>`U₃ ∈ [32, 45)`<br>`U₄ ∈ [45, 75)`<br>`U₅ ∈ [75, 85)` | `0.10`<br>`0.50`<br>`0.20`<br>`0.15`<br>`0.05` | `0.10`<br>`0.60`<br>`0.80`<br>`0.95`<br>`1.00` |
| **Blue** (*Modrý*) | Discrete Empirical | `U₁ ∈ [15, 29)`<br>`U₂ ∈ [29, 45)`<br>`U₃ ∈ [45, 65)` | `0.20`<br>`0.40`<br>`0.40` | `0.20`<br>`0.60`<br>`1.00` |

### 2. Traffic Congestion Model at Intersection K

When the courier leaves node **K** after the congestion threshold time (`t_K`), the generated speed `v` on the immediately outgoing segment from **K** of length `d` is penalized by a slowdown percentage `s ∈ [10, 25)`:

- **Before or at threshold (`t_departure ≤ t_K`):**
  - `v_effective = v`
- **After threshold (`t_departure > t_K`):**
  - `v_effective = v · ((100 - s) / 100)`
- **Segment Travel Time (in hours):**
  - `Δt = d / v_effective`

### 3. Inter-Municipality Connections (`Map.cs`)

Between any two municipalities, the courier evaluates all valid direct road sequences (without passing through another store's municipality) and picks the minimum travel time:

| Origin ↔ Destination | Available Sub-Routes (Color & Distance in `km`) |
| :--- | :--- |
| **Žilina ↔ Divinka** | **1.** Green (`4 km`) &nbsp;\|&nbsp; **2.** Red (`4 km`) &nbsp;\|&nbsp; **3.** Black (`2 km`) → **K** → Red (`2 km`) |
| **Žilina ↔ Strečno** | **1.** Red (`3 km`) + Red (`4 km`) &nbsp;\|&nbsp; **2.** Green (`4 km`) + Black (`3 km`) &nbsp;\|&nbsp; **3.** Black (`2 km`) → **K** → Blue (`4 km`) |
| **Žilina ↔ Rajecké Teplice** | **1.** Black (`2 km`) → **K** → Green (`2 km`) |
| **Divinka ↔ Rajecké Teplice** | **1.** Red (`2 km`) → **K** → Green (`2 km`) &nbsp;\|&nbsp; **2.** Black (`1 km`) + Red (`3 km`) + Blue (`1 km`) &nbsp;\|&nbsp; **3.** Black (`1 km`) + Blue (`1 km`) + Red (`2 km`) + Blue (`1 km`) |
| **Divinka ↔ Strečno** | **1.** Red (`2 km`) → **K** → Blue (`4 km`) |
| **Strečno ↔ Rajecké Teplice** | **1.** Blue (`4 km`) → **K** → Green (`2 km`) &nbsp;\|&nbsp; **2.** Blue (`5 km`) + Blue (`8 km`) &nbsp;\|&nbsp; **3.** Black (`5 km`) + Blue (`8 km`) |

---

## Implemented Simulation Tasks & Experimental Results

All experiments were conducted with **`10,000,000` Monte Carlo replications** per route.

### Task 1: Optimal Delivery Sequence (Departure at 06:00)

**Goal:** Determine the visitation order of municipalities (`Divinka`, `Strečno`, `Rajecké Teplice`) starting and ending in `Žilina` (departure at `06:00:00`) that minimizes the average total delivery time.

| # | Route Permutation | Mean Duration (`min`) | Mean Return Time to Žilina | Rank |
| :-: | :--- | :---: | :---: | :-: |
| **1** | `Žilina → Divinka → Rajecké Teplice → Strečno → Žilina` | `≈ 22.2537` | `06:22:15.2240339` | 2nd |
| **2** | `Žilina → Divinka → Strečno → Rajecké Teplice → Žilina` | `≈ 26.8099` | `06:26:48.5926192` | 6th |
| **3** | `Žilina → Rajecké Teplice → Divinka → Strečno → Žilina` | `≈ 24.8775` | `06:24:52.6467990` | 3rd |
| **4** | `Žilina → Rajecké Teplice → Strečno → Divinka → Žilina` | `≈ 26.7896` | `06:26:47.3748144` | 5th |
| **5** | **`Žilina → Strečno → Rajecké Teplice → Divinka → Žilina`** | **`≈ 22.2531`** | **`06:22:15.1835231`** | **1st (Optimal)** |
| **6** | `Žilina → Strečno → Divinka → Rajecké Teplice → Žilina` | `≈ 24.8775` | `06:24:52.6471542` | 4th |

> **Conclusion (Task 1):** The courier achieves the shortest average delivery time by serving the municipalities in the order **Žilina → Strečno → Rajecké Teplice → Divinka → Žilina**, returning to the Žilina warehouse on average at **06:22:15.1835231**.

---

### Task 2: Latest Departure Time for 80% Reliability (Return by 07:35)

**Goal:** Determine the latest time the courier can depart from Žilina along the optimal route (`ZA → S → RT → D → ZA`) so that they return to Žilina by **07:35:00** with a probability of **at least 80%**.

1. **80th Percentile Calculation (`TransporteMCTask2`):**
   - `10,000,000` replications were executed on the optimal route under active node **K** congestion.
   - All simulated trip durations were recorded and sorted in ascending order to extract the **80th percentile duration (`T_80`)**, representing the maximum trip duration for the fastest `80%` of runs.
   - Subtracting `T_80` from the target deadline (`07:35:00`) yields the latest safe departure time:
     ```text
     Latest Departure Time = 07:35:00 - T_80 = 07:09:59.2447731
     ```

2. **Independent Verification Run (`TransporteMCTask2Percentil`):**
   - To verify tightness of the bound, an independent `10,000,000`-replication verification experiment was run with the departure time shifted by less than one second later to **`07:10:00`**.
   - At `07:10:00`, the empirical probability of returning by `07:35:00` dropped to **79.94217%** (just below the `80%` threshold), confirming **`07:09:59.2447731`** as the exact boundary.

---

## Installation & How to Run

### Prerequisites

- **Operating System:** Windows 10 / 11
- **SDK:** [.NET 10.0 SDK](https://dotnet.microsoft.com/en-us/download/dotnet/10.0) and [.NET 8.0 SDK](https://dotnet.microsoft.com/en-us/download/dotnet/8.0) (`net10.0-windows` / `net8.0-windows`)
- **IDE:** [Visual Studio 2022 / 2026](https://visualstudio.microsoft.com/) (with *.NET desktop development* workload)

### Steps

1. **Clone the repository:**
   ```bash
   git clone https://github.com/lubomirbojda/Monte-Carlo-Simulation-Engine
   cd Monte-Carlo-Simulation-Engine
   ```

2. **Open the solution:**
   - Open `DS_SEM_1.slnx` in Visual Studio.

3. **Choose the Startup Project:**
   - **Monte Carlo Simulation GUI:** Right-click `GUI` in Solution Explorer → **Set as Startup Project**.
   - **Generator Statistical Tester (Console):** Right-click `DS_SEM_1` in Solution Explorer → **Set as Startup Project** (uncomment the desired test methods in `DS_SEM_1/Program.cs`).

4. **Build & Run:**
   - Select `Release` or `Debug` configuration and press `F5` or `Ctrl + F5`.
   - Or run directly from the CLI:
     ```bash
     # Launch the main Windows Forms Monte Carlo simulation application
     dotnet run --project GUI -c Release

     # Run the console random generator tester
     dotnet run --project DS_SEM_1 -c Release
     ```

---

## GUI Controls & Usage

### 1. Tab `Úloha 1` (All 6 Route Permutations Comparison)
- **Time Configuration (`Čas začiatku` & `Čas začiatku spomalenia`):**
  - Configure the courier's departure time from Žilina (`Hod`, `Min`, `Sek` — default `06:00:00`) and the congestion threshold time at intersection **K** (default `06:30:00`).
- **Simulation Settings (`Nastavenia simulácie`):**
  - Set the number of Monte Carlo replications (`Replikácie` — default `100,000`).
  - Optionally enable **`Použiť vlastný Seed`** to run deterministic experiments with a fixed integer seed.
  - Click **`SPUSTIŤ`** to start asynchronous evaluation of all 6 route permutations or **`ZASTAVIŤ`** to halt the simulation early and display the results achieved so far.
- **6-Panel Live Convergence Grid:**
  - Displays 6 interactive `ScottPlot` charts (`2 × 3` grid) showing the stabilization of average trip duration (in minutes) after the 20% warm-up phase, along with the exact formatted `Priemerný čas` of arrival back in Žilina below each chart.

### 2. Tab `Úloha 2` (80th Percentile Departure Time & Reliability Verification)
- **80th Percentile Finder (`Nastavenia simulácie (Úloha 2)`):**
  - Specify the replication count and click **`SPUSTIŤ`** to simulate the optimal route (`ZA → S → RT → D → ZA`), plot its convergence curve, and compute the latest departure time from Žilina to guarantee an 80% on-time arrival rate by `07:35:00`.
- **On-Time Arrival Verification (`Simulácia pre % jázd do 7:35`):**
  - Enter any custom departure time (`Čas: Hod / Min / Sek`) and replication count (`Repl.`), then click **`SPUSTIŤ`** to measure the exact percentage of simulated trips that return to Žilina at or before `07:35:00`.

---

## Documentation

Complete technical documentation (in Slovak) is included in the repository root:

- **[Project Documentation (PDF)](https://github.com/lubomirbojda/Monte-Carlo-Simulation-Engine/blob/main/Documentation.pdf)**
  - Architecture and mathematical design of uniform and multi-interval empirical random number generators
  - Statistical validation histograms across `1,000,000` generated samples for all four generator types
  - Universal Monte Carlo core design (`BaseMonteCarlo`) and domain model (`Map`, `TransporterMC`)
  - Experimental results, convergence plots (`10,000,000` replications), and verification analysis for Task 1 and Task 2
  - Complete UML Class Diagrams of the generator hierarchy and Monte Carlo simulation classes

---

## Screenshots

<p align="center">
  <img width="1283" height="889" alt="image" src="https://github.com/user-attachments/assets/645d0c26-6165-4757-9e06-241debd019f5" />
  <img width="1281" height="887" alt="image" src="https://github.com/user-attachments/assets/adcbb464-400d-4dfe-8c51-90090bd9d264" />
</p>
