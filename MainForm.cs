// GradeWiz
// A simple app that calculates a final module mark based on component weightings and component marks.
// Created (c) by Ricki Angel, 2024
using System;
using System.Drawing;
using System.Windows.Forms;

namespace GradeWiz
{
    public partial class MainForm : Form
    {
        private const int MaxComponents = 5;
        private int _numComponents;
        private double[] _weightings;
        private double[] _scores;

        private MenuStrip _menuStrip;  // Add this line

        // Layout Constants
        private const int LabelX = 10;
        private const int TextBoxWidth = 50;
        private const int ComponentSpacing = 40;
        private const int ButtonWidth = 80;
        private const int ButtonHeight = 30;
        private const int StartY = 40;
        private const int LabelSpacing = 40;
        private const int FieldHeight = 30;
        private const int MenuStripHeight = 24;

        public MainForm()
        {
            InitializeComponent();
            this.BackColor = Color.LightGray;
            this.ClientSize = new Size(400, 350);  // Increase form height to account for MenuStrip
            CreateMenuBar();
            ShowNumComponentsScreen();
            CenterTitle();
        }

        private void CenterTitle()
        {
            var title = "GradeWiz ✔";
            Text = title;
            var titleSize = TextRenderer.MeasureText(title, Font);
            var formWidth = ClientSize.Width;
            var titleX = (formWidth - titleSize.Width) / 2;
            Text = title;
            // This part sets the form's title bar text alignment
            Text = title;
            this.TextChanged += (sender, e) => Text = title;
            this.Resize += (sender, e) =>
            {
                var newTitleSize = TextRenderer.MeasureText(title, Font);
                this.Text = title; // Ensure title remains the same
            };
        }

        private void CreateMenuBar()
        {
            _menuStrip = new MenuStrip();  // Initialize the MenuStrip
            var fileMenu = new ToolStripMenuItem("File");

            // About Menu Item
            var aboutItem = new ToolStripMenuItem("About");
            aboutItem.Click += (sender, e) => ShowAboutDialog();
            fileMenu.DropDownItems.Add(aboutItem);

            // Restart Menu Item
            var restartItem = new ToolStripMenuItem("Restart");
            restartItem.Click += (sender, e) => ShowNumComponentsScreen();
            fileMenu.DropDownItems.Add(restartItem);

            // Quit Menu Item
            var quitItem = new ToolStripMenuItem("Quit");
            quitItem.Click += (sender, e) => Application.Exit();
            fileMenu.DropDownItems.Add(quitItem);

            _menuStrip.Items.Add(fileMenu);
            MainMenuStrip = _menuStrip;
            Controls.Add(_menuStrip);  // Add the MenuStrip to the form
        }

       private void ShowAboutDialog()
{
    MessageBox.Show(
        "GradeWiz ✔\n\nA simple app that calculates a final module mark based on component weightings and component marks.\n\n© 2024 Ricki Angel\nhttps://github.com/TechAngelX\n\n" +
        "Licensed under the GNU General Public License v3.0",
        "About GradeWiz",
        MessageBoxButtons.OK,
        MessageBoxIcon.Information
    );
}


        private void ShowNumComponentsScreen()
        {
            var panel = new Panel { Dock = DockStyle.Fill };
            var label = new Label { Text = "Number of components (1-5):", Location = new Point(LabelX, StartY), AutoSize = true };
            var numComponentsField = new TextBox { Location = new Point(LabelX + 190, StartY), Width = TextBoxWidth };
            var nextButton = new Button { Text = "Next", Location = new Point(LabelX, StartY + ButtonHeight + 10), Size = new Size(ButtonWidth, ButtonHeight) };

            nextButton.Click += (sender, e) =>
            {
                if (int.TryParse(numComponentsField.Text, out _numComponents) && _numComponents > 0 && _numComponents <= MaxComponents)
                {
                    _weightings = new double[_numComponents];
                    _scores = new double[_numComponents];
                    ShowWeightingScreen();
                }
                else
                {
                    MessageBox.Show($"Please enter a number of components between 1 and {MaxComponents}.");
                }
            };

            panel.Controls.Add(label);
            panel.Controls.Add(numComponentsField);
            panel.Controls.Add(nextButton);
            Controls.Clear();
            Controls.Add(_menuStrip);  // Ensure MenuStrip is re-added
            Controls.Add(panel);
        }

        private void ShowWeightingScreen()
        {
            var panel = new Panel { Dock = DockStyle.Fill };
            var label = new Label { Text = "Enter % weightings for each component:", Location = new Point(LabelX, StartY), AutoSize = true };
            panel.Controls.Add(label);

            var weightingFields = new TextBox[_numComponents];

            for (int i = 0; i < _numComponents; i++)
            {
                var componentLabel = new Label { Text = $"Component {i + 1}:", Location = new Point(LabelX, StartY + LabelSpacing + i * ComponentSpacing), AutoSize = true };
                var weightingField = new TextBox { Location = new Point(LabelX + 100, StartY + LabelSpacing + i * ComponentSpacing), Width = TextBoxWidth };
                weightingField.Tag = i;
                weightingFields[i] = weightingField;

                panel.Controls.Add(componentLabel);
                panel.Controls.Add(weightingField);

                // Label for percentage sign
                var percentLabel = new Label
                {
                    Text = "%",
                    Location = new Point(LabelX + 153, StartY + LabelSpacing + i * ComponentSpacing + 5),  // Positioned at the end of the TextBox
                    AutoSize = true
                };
                panel.Controls.Add(percentLabel);
            }

            var backButton = new Button { Text = "Back", Location = new Point(LabelX, StartY + LabelSpacing + _numComponents * ComponentSpacing + 10), Size = new Size(ButtonWidth, ButtonHeight) };
            backButton.Click += (sender, e) => ShowNumComponentsScreen();
            var nextButton = new Button { Text = "Next", Location = new Point(LabelX + ButtonWidth + 10, StartY + LabelSpacing + _numComponents * ComponentSpacing + 10), Size = new Size(ButtonWidth, ButtonHeight) };

            nextButton.Click += (sender, e) =>
            {
                try
                {
                    double totalWeight = 0;
                    for (int i = 0; i < _numComponents; i++)
                    {
                        _weightings[i] = double.Parse(weightingFields[i].Text);
                        if (_weightings[i] < 0) throw new FormatException();
                        totalWeight += _weightings[i];
                    }

                    if (Math.Abs(totalWeight - 100) < 1e-5)
                    {
                        ShowComponentMarkScreen();
                    }
                    else
                    {
                        throw new FormatException();
                    }
                }
                catch (FormatException)
                {
                    MessageBox.Show("Please enter valid weighting numbers that sum up to 100%.");
                }
            };

            panel.Controls.Add(backButton);
            panel.Controls.Add(nextButton);
            Controls.Clear();
            Controls.Add(_menuStrip);  // Ensure MenuStrip is re-added
            Controls.Add(panel);
        }

        private void ShowComponentMarkScreen()
        {
            var panel = new Panel { Dock = DockStyle.Fill };
            var label = new Label
            {
                Text = "Enter marks for each component:",
                Location = new Point(LabelX, StartY),
                AutoSize = true
            };
            panel.Controls.Add(label);

            var markFields = new TextBox[_numComponents];

            for (int i = 0; i < _numComponents; i++)
            {
                // Label for component mark
                var componentLabel = new Label
                {
                    Text = $"Component {i + 1} mark:",
                    Location = new Point(LabelX, StartY + LabelSpacing + i * ComponentSpacing),
                    AutoSize = true
                };
                panel.Controls.Add(componentLabel);

                // TextBox for entering mark
                var markField = new TextBox
                {
                    Location = new Point(LabelX + 117, StartY + LabelSpacing + i * ComponentSpacing),
                    Width = TextBoxWidth
                };
                markFields[i] = markField;
                panel.Controls.Add(markField);

                // Label for percentage sign
                var percentLabel = new Label
                {
                    Text = $"{_weightings[i]}%",
                    Location = new Point(LabelX + 165, StartY + LabelSpacing + i * ComponentSpacing + 5),
                    AutoSize = true
                };
                panel.Controls.Add(percentLabel);
            }

            // Back button
            var backButton = new Button
            {
                Text = "Back",
                Location = new Point(LabelX, StartY + LabelSpacing + _numComponents * ComponentSpacing + 10),
                Size = new Size(ButtonWidth, ButtonHeight)
            };
            backButton.Click += (sender, e) => ShowWeightingScreen();
            panel.Controls.Add(backButton);

            // Calculate button
            var calculateButton = new Button
            {
                Text = "Calculate",
                Location = new Point(LabelX + ButtonWidth + 10, StartY + LabelSpacing + _numComponents * ComponentSpacing + 10),
                Size = new Size(ButtonWidth, ButtonHeight)
            };
            calculateButton.Click += (sender, e) =>
            {
                try
                {
                    for (int i = 0; i < _numComponents; i++)
                    {
                        _scores[i] = double.Parse(markFields[i].Text);
                    }
                    ShowResultScreen();
                }
                catch (FormatException)
                {
                    MessageBox.Show("Please enter valid numbers for the scores.");
                }
            };
            panel.Controls.Add(calculateButton);

           

            Controls.Clear();
            Controls.Add(_menuStrip);  // Ensure MenuStrip is re-added
            Controls.Add(panel);
        }

        private void ShowResultScreen()
        {
            var panel = new Panel { Dock = DockStyle.Fill };
            var resultLabel = new Label
            {
                Text = $"Total module mark: {CalculateTotalMark():F2}",
                Font = new Font("Arial", 16, FontStyle.Bold),
                ForeColor = Color.Black,
                Location = new Point(LabelX, StartY),
                AutoSize = true
            };
            panel.Controls.Add(resultLabel);

            for (int i = 0; i < _numComponents; i++)
            {
                var componentLabel = new Label
                {
                    Text = $"Component {i + 1} mark:",
                    Location = new Point(LabelX, StartY + LabelSpacing + i * 30),
                    AutoSize = true
                };
                var markLabel = new Label
                {
                    Text = $"{_scores[i] * (_weightings[i] / 100):F2}",
                    Location = new Point(LabelX + 140, StartY + LabelSpacing + i * 30),
                    AutoSize = true
                };

                panel.Controls.Add(componentLabel);
                panel.Controls.Add(markLabel);
            }

            var backButton = new Button { Text = "Back", Location = new Point(LabelX, StartY + LabelSpacing + _numComponents * 30 + 10), Size = new Size(ButtonWidth, ButtonHeight) };
            backButton.Click += (sender, e) => ShowComponentMarkScreen();

            var restartButton = new Button { Text = "Restart", Location = new Point(LabelX + ButtonWidth + 10, StartY + LabelSpacing + _numComponents * 30 + 10), Size = new Size(ButtonWidth, ButtonHeight) };
            restartButton.Click += (sender, e) => ShowNumComponentsScreen();

            panel.Controls.Add(backButton);
            panel.Controls.Add(restartButton);
            Controls.Clear();
            Controls.Add(_menuStrip);  // Ensure MenuStrip is re-added
            Controls.Add(panel);
        }

        private double CalculateTotalMark()
        {
            double totalMark = 0;
            for (int i = 0; i < _numComponents; i++)
            {
                totalMark += _scores[i] * (_weightings[i] / 100);
            }
            return totalMark;
        }
    }
}
