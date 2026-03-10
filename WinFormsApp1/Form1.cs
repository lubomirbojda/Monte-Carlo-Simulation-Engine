using DS_SEM_1.MonteCarlo.TransporterMonteCarlo;
using ScottPlot.WinForms;

namespace WinFormsApp1;

public partial class Form1 : Form
{
    private TabControl tabControl;
    private TabPage tabUloha1;
    private TabPage tabUloha2;

    private NumericUpDown nudStartH, nudStartM, nudStartS;
    private NumericUpDown nudKStartH, nudKStartM, nudKStartS;
    private NumericUpDown nudReplications, nudSeed;
    private CheckBox cbUseSeed;

    private TransporterAllRoutesMC _task;

    private FormsPlot[] plots = new FormsPlot[6];
    private Label[] plotResults = new Label[6];

    private NumericUpDown nudReplU2;
    private FormsPlot plotU2;
    private Label lblResultU2;

    public Form1()
    {
        Text = "MonteCarlo";
        Size = new Size(1300, 900);
        MinimumSize = new Size(1100, 750);

        InitializeUI();
    }

    private void InitializeUI()
    {
        tabControl = new TabControl { Dock = DockStyle.Fill };
        tabUloha1 = new TabPage("Úloha 1");
        tabUloha2 = new TabPage("Úloha 2");

        tabControl.TabPages.Add(tabUloha1);
        tabControl.TabPages.Add(tabUloha2);
        Controls.Add(tabControl);

        SetupUloha1();
        SetupUloha2();
    }

    private void SetupUloha1()
    {
        TableLayoutPanel mainLayout = new TableLayoutPanel
        {
            Dock = DockStyle.Fill,
            RowCount = 2,
            ColumnCount = 1
        };
        mainLayout.RowStyles.Add(new RowStyle(SizeType.Absolute, 130));
        mainLayout.RowStyles.Add(new RowStyle(SizeType.Percent, 100));
        tabUloha1.Controls.Add(mainLayout);

        FlowLayoutPanel inputPanel = new FlowLayoutPanel { Dock = DockStyle.Fill, FlowDirection = FlowDirection.LeftToRight, Padding = new Padding(10) };
        mainLayout.Controls.Add(inputPanel, 0, 0);

        var startGroup = CreateTimeGroup("Čas začiatku", out nudStartH, out nudStartM, out nudStartS);
        inputPanel.Controls.Add(startGroup);

        var endGroup = CreateTimeGroup("Čas začiatku spomalenia", out nudKStartH, out nudKStartM, out nudKStartS);
        inputPanel.Controls.Add(endGroup);

        GroupBox simGroup = new GroupBox { Text = "Nastavenia simulácie", Size = new Size(450, 100) };

        Label lblRepl = new Label { Text = "Replikácie:", Location = new Point(10, 30), AutoSize = true };
        nudReplications = new NumericUpDown
        {
            Location = new Point(85, 28),
            Width = 100,
            Maximum = decimal.MaxValue,
            Value = 1000
        };

        Label lblSeed = new Label { Text = "Seed:", Location = new Point(10, 60), AutoSize = true };
        nudSeed = new NumericUpDown
        {
            Location = new Point(85, 58),
            Width = 100,
            Maximum = decimal.MaxValue,
            Value = 12345,
            Enabled = false
        };

        cbUseSeed = new CheckBox
        {
            Text = "Použiť vlastný Seed",
            Location = new Point(190, 58),
            AutoSize = true
        };

        cbUseSeed.CheckedChanged += (s, e) => { nudSeed.Enabled = cbUseSeed.Checked; };

        Button btnStart = new Button
        {
            Text = "SPUSTIŤ",
            Location = new Point(230, 20),
            Size = new Size(95, 30),
            BackColor = Color.LightGreen,
            Font = new Font(DefaultFont, FontStyle.Bold)
        };
        btnStart.Click += BtnStart_Click;

        Button btnStop = new Button
        {
            Text = "ZASTAVIŤ",
            Location = new Point(335, 20),
            Size = new Size(95, 30),
            BackColor = Color.LightCoral,
            Font = new Font(DefaultFont, FontStyle.Bold)
        };
        btnStop.Click += BtnStop_Click;

        simGroup.Controls.AddRange(new Control[] { lblRepl, nudReplications, lblSeed, nudSeed, cbUseSeed, btnStart, btnStop });
        inputPanel.Controls.Add(simGroup);

        // --- GRAFY ---
        TableLayoutPanel chartGrid = new TableLayoutPanel { Dock = DockStyle.Fill, RowCount = 2, ColumnCount = 3 };
        for (int i = 0; i < 3; i++) chartGrid.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 33.33f));
        for (int i = 0; i < 2; i++) chartGrid.RowStyles.Add(new RowStyle(SizeType.Percent, 50.00f));

        for (int i = 0; i < 6; i++)
        {
            Panel pnlChart = new Panel { Dock = DockStyle.Fill, Padding = new Padding(3) };

            plotResults[i] = new Label
            {
                Text = $"Priemerný čas: ",
                Dock = DockStyle.Bottom,
                Height = 25,
                TextAlign = ContentAlignment.MiddleLeft,
                Font = new Font("Segoe UI", 9, FontStyle.Bold),
                ForeColor = Color.DarkBlue
            };

            plots[i] = new FormsPlot { Dock = DockStyle.Fill };

            pnlChart.Controls.Add(plots[i]);
            pnlChart.Controls.Add(plotResults[i]);
            chartGrid.Controls.Add(pnlChart, i % 3, i / 3);
        }
        mainLayout.Controls.Add(chartGrid, 0, 1);
    }

    private GroupBox CreateTimeGroup(string title, out NumericUpDown h, out NumericUpDown m, out NumericUpDown s)
    {
        GroupBox gb = new GroupBox { Text = title, Size = new Size(220, 100) };

        h = new NumericUpDown { Location = new Point(10, 45), Width = 50, Maximum = 23 };
        m = new NumericUpDown { Location = new Point(75, 45), Width = 50, Maximum = 59 };
        s = new NumericUpDown { Location = new Point(140, 45), Width = 50, Maximum = 59 };

        gb.Controls.Add(new Label { Text = "Hod:", Location = new Point(10, 25), AutoSize = true });
        gb.Controls.Add(new Label { Text = "Min:", Location = new Point(75, 25), AutoSize = true });
        gb.Controls.Add(new Label { Text = "Sek:", Location = new Point(140, 25), AutoSize = true });
        gb.Controls.Add(h); gb.Controls.Add(m); gb.Controls.Add(s);

        return gb;
    }

    private async void BtnStart_Click(object sender, EventArgs e)
    {
        int startH = (int)nudStartH.Value;
        int startM = (int)nudStartM.Value;
        int startS = (int)nudStartS.Value;

        int kStartH = (int)nudKStartH.Value;
        int kStartM = (int)nudKStartM.Value;
        int kStartS = (int)nudKStartS.Value;

        TimeSpan start = new(startH, startM, startS);
        TimeSpan kStart = new(kStartH, kStartM, kStartS);

        int replikacie = (int)nudReplications.Value;     

        foreach (var plot in plots)
        {
            plot.Plot.Clear();
            plot.Refresh();
        }

        if (cbUseSeed.Checked)
        {
            int seed = (int)nudSeed.Value;
            _task = new TransporterAllRoutesMC(replikacie, plots, plotResults, start, kStart, seed);
        }
        else
        {
            _task = new TransporterAllRoutesMC(replikacie, plots, plotResults, start, kStart);
        }
        

        await Task.Run(() => _task.RunTask1());
    }

    private void BtnStop_Click(object sender, EventArgs e)
    {
        _task?.StopSimulation();
    }

    private void SetupUloha2()
    {
        TableLayoutPanel mainLayout = new TableLayoutPanel
        {
            Dock = DockStyle.Fill,
            RowCount = 2,
            ColumnCount = 1
        };
        mainLayout.RowStyles.Add(new RowStyle(SizeType.Absolute, 100)); // Panel pre inputy
        mainLayout.RowStyles.Add(new RowStyle(SizeType.Percent, 100));  // Graf
        tabUloha2.Controls.Add(mainLayout);

        FlowLayoutPanel inputPanel = new FlowLayoutPanel
        {
            Dock = DockStyle.Fill,
            FlowDirection = FlowDirection.LeftToRight,
            Padding = new Padding(10)
        };
        mainLayout.Controls.Add(inputPanel, 0, 0);

        GroupBox simGroup = new GroupBox { Text = "Nastavenia simulácie (Úloha 2)", Size = new Size(450, 70) };

        Label lblRepl = new Label { Text = "Replikácie:", Location = new Point(10, 30), AutoSize = true };
        nudReplU2 = new NumericUpDown
        {
            Location = new Point(85, 28),
            Width = 100,
            Maximum = decimal.MaxValue,
            Value = 1000
        };

        Button btnStart = new Button
        {
            Text = "SPUSTIŤ",
            Location = new Point(210, 22),
            Size = new Size(100, 35),
            BackColor = Color.LightGreen,
            Font = new Font(DefaultFont, FontStyle.Bold)
        };
        btnStart.Click += BtnStartU2_Click;

        Button btnStop = new Button
        {
            Text = "ZASTAVIŤ",
            Location = new Point(320, 22),
            Size = new Size(100, 35),
            BackColor = Color.LightCoral,
            Font = new Font(DefaultFont, FontStyle.Bold)
        };
        btnStop.Click += BtnStopU2_Click;

        simGroup.Controls.AddRange(new Control[] { lblRepl, nudReplU2, btnStart, btnStop });
        inputPanel.Controls.Add(simGroup);

        Panel pnlChart = new Panel { Dock = DockStyle.Fill, Padding = new Padding(10) };

        lblResultU2 = new Label
        {
            Text = "Výsledok: ",
            Dock = DockStyle.Bottom,
            Height = 30,
            TextAlign = ContentAlignment.MiddleLeft,
            Font = new Font("Segoe UI", 10, FontStyle.Bold),
            ForeColor = Color.DarkBlue
        };

        plotU2 = new FormsPlot { Dock = DockStyle.Fill };

        pnlChart.Controls.Add(plotU2);
        pnlChart.Controls.Add(lblResultU2);
        mainLayout.Controls.Add(pnlChart, 0, 1);
    }

    private async void BtnStartU2_Click(object sender, EventArgs e)
    {
        int replikacie = (int)nudReplU2.Value;

        plotU2.Plot.Clear();
        plotU2.Refresh();


        _task = new TransporterAllRoutesMC(replikacie, plots, plotResults, new(7, 0, 0), new(6, 30, 0));
        await Task.Run(() => _task.RunTask2(plotU2, lblResultU2));
    }

    private async void BtnStopU2_Click(object sender, EventArgs e)
    {

    }
}