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

    private TransporterAllRoutesMC _task1;

    private FormsPlot[] plots = new FormsPlot[6];
    private Label[] plotResults = new Label[6];

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

        GroupBox simGroup = new GroupBox { Text = "Nastavenia simulácie", Size = new Size(420, 100) };

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
            Value = 12345
        };

        Button btnStart = new Button
        {
            Text = "SPUSTIŤ",
            Location = new Point(210, 25),
            Size = new Size(90, 55),
            BackColor = Color.LightGreen,
            Font = new Font(DefaultFont, FontStyle.Bold)
        };
        btnStart.Click += BtnStart_Click;

        Button btnStop = new Button
        {
            Text = "ZASTAVIŤ",
            Location = new Point(310, 25),
            Size = new Size(90, 55),
            BackColor = Color.LightCoral,
            Font = new Font(DefaultFont, FontStyle.Bold)
        };
        btnStop.Click += BtnStop_Click;

        simGroup.Controls.AddRange(new Control[] { lblRepl, nudReplications, lblSeed, nudSeed, btnStart, btnStop });
        inputPanel.Controls.Add(simGroup);

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
        int seed = (int)nudSeed.Value;

        foreach (var plot in plots)
        {
            plot.Plot.Clear();
            plot.Refresh();
        }


        _task1 = new TransporterAllRoutesMC(replikacie, plots, plotResults, start, kStart);

        await Task.Run(() => _task1.RunTask1());
    }

    private void BtnStop_Click(object sender, EventArgs e)
    {
        _task1.StopSimulation();
    }
}