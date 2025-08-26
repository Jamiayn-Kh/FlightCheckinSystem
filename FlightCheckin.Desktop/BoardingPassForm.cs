using System;
using System.Drawing;
using System.Drawing.Printing;
using System.Windows.Forms;
using FlightCheckin.Models;

namespace FlightCheckin.Desktop
{
    public partial class BoardingPassForm : Form
    {
        private readonly BoardingPass _boardingPass;
        private readonly string _flightNumber;
        private readonly string _seatCode;

        public BoardingPassForm(BoardingPass boardingPass, string flightNumber, string seatCode)
        {
            _boardingPass = boardingPass;
            _flightNumber = flightNumber;
            _seatCode = seatCode;
            InitializeComponent();
            DisplayBoardingPass();
        }

        private void InitializeComponent()
        {
            this.Text = "🎫 Boarding Pass - Flight Check-in System";
            this.Size = new Size(1000, 750);
            this.StartPosition = FormStartPosition.CenterParent;
            this.BackColor = Color.FromArgb(248, 250, 252);
            this.Font = new Font("Segoe UI", 9F, FontStyle.Regular);
            this.AutoScaleMode = AutoScaleMode.Dpi;
            this.AutoScaleDimensions = new SizeF(96F, 96F);
            this.FormBorderStyle = FormBorderStyle.Sizable;
            this.MaximizeBox = true;
            this.MinimizeBox = true;
            this.ShowInTaskbar = false;
            this.MinimumSize = new Size(900, 650);

            // Scrollable container for main content (prevents clipping on smaller screens)
            var mainContainer = new Panel
            {
                Dock = DockStyle.Fill,
                AutoScroll = true,
                BackColor = this.BackColor,
            };

            // Main boarding pass panel with proper double buffering - make it responsive
            var boardingPassPanel = new Panel
            {
                Dock = DockStyle.Top,
                Height = 500,
                BackColor = Color.White,
                BorderStyle = BorderStyle.None
            };
            
            // Enable double buffering to prevent flickering
            typeof(Panel).InvokeMember("DoubleBuffered",
                System.Reflection.BindingFlags.SetProperty | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic,
                null, boardingPassPanel, new object[] { true });

            // Simplified shadow effect - removed overlapping drawing
            boardingPassPanel.Paint += (s, e) =>
            {
                e.Graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
                
                var rect = new Rectangle(0, 0, boardingPassPanel.Width, boardingPassPanel.Height);
                
                // Draw shadow first
                using (var shadowBrush = new SolidBrush(Color.FromArgb(15, 0, 0, 0)))
                {
                    e.Graphics.FillRectangle(shadowBrush, rect.X + 3, rect.Y + 3, rect.Width, rect.Height);
                }
                
                // Draw main background
                using (var backgroundBrush = new SolidBrush(Color.White))
                {
                    e.Graphics.FillRectangle(backgroundBrush, rect);
                }
                
                // Draw border
                using (var borderPen = new Pen(Color.FromArgb(229, 231, 235), 1))
                {
                    e.Graphics.DrawRectangle(borderPen, rect.X, rect.Y, rect.Width - 1, rect.Height - 1);
                }
            };

            // Header with airline info
            var headerPanel = new Panel
            {
                Dock = DockStyle.Top,
                Size = new Size(0, 100),
                BackColor = Color.FromArgb(37, 99, 235)
            };

            var airlineLabel = new Label
            {
                Text = "✈️ SKYLINE AIRWAYS",
                Location = new Point(30, 20),
                Size = new Size(400, 35),
                Font = new Font("Segoe UI", 18F, FontStyle.Bold),
                ForeColor = Color.White,
                BackColor = Color.Transparent
            };

            var boardingPassTitle = new Label
            {
                Text = "BOARDING PASS",
                Location = new Point(30, 60),
                Size = new Size(300, 25),
                Font = new Font("Segoe UI", 14F, FontStyle.Regular),
                ForeColor = Color.FromArgb(219, 234, 254),
                BackColor = Color.Transparent
            };

            var dateLabel = new Label
            {
                Text = $"Issued: {_boardingPass.IssuedAt:dd MMM yyyy HH:mm}",
                Dock = DockStyle.Right,
                Width = 220,
                Font = new Font("Segoe UI", 12F, FontStyle.Regular),
                ForeColor = Color.FromArgb(219, 234, 254),
                BackColor = Color.Transparent,
                TextAlign = ContentAlignment.MiddleRight
            };

            headerPanel.Controls.AddRange(new Control[] { airlineLabel, boardingPassTitle, dateLabel });

            // Main content panel with double buffering
            var contentPanel = new Panel
            {
                Name = "contentPanel",
                Dock = DockStyle.Fill,
                BackColor = Color.White
            };
            
            // Enable double buffering for content panel
            typeof(Panel).InvokeMember("DoubleBuffered",
                System.Reflection.BindingFlags.SetProperty | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic,
                null, contentPanel, new object[] { true });

            boardingPassPanel.Controls.AddRange(new Control[] { headerPanel, contentPanel });

            // Action buttons
            var buttonPanel = new Panel
            {
                Dock = DockStyle.Bottom,
                Height = 70,
                BackColor = Color.Transparent
            };

            var printBtn = new Button
            {
                Text = "🖨️ Print Boarding Pass",
                Location = new Point(0, 15),
                Size = new Size(220, 45),
                Font = new Font("Segoe UI", 12F, FontStyle.Bold),
                BackColor = Color.FromArgb(16, 185, 129),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                FlatAppearance = { BorderSize = 0 },
                Cursor = Cursors.Hand
            };
            printBtn.Click += PrintBtn_Click;

            var saveBtn = new Button
            {
                Text = "💾 Save as PDF",
                Location = new Point(240, 15),
                Size = new Size(180, 45),
                Font = new Font("Segoe UI", 12F, FontStyle.Bold),
                BackColor = Color.FromArgb(147, 51, 234),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                FlatAppearance = { BorderSize = 0 },
                Cursor = Cursors.Hand
            };
            saveBtn.Click += SaveBtn_Click;

            var closeBtn = new Button
            {
                Text = "✖️ Close",
                Dock = DockStyle.Right,
                Size = new Size(180, 45),
                Font = new Font("Segoe UI", 12F, FontStyle.Bold),
                BackColor = Color.FromArgb(107, 114, 128),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                FlatAppearance = { BorderSize = 0 },
                Cursor = Cursors.Hand
            };
            closeBtn.Click += (s, e) => this.Close();

            buttonPanel.Controls.AddRange(new Control[] { printBtn, saveBtn, closeBtn });

            mainContainer.Controls.Add(boardingPassPanel);
            this.Controls.AddRange(new Control[] { mainContainer, buttonPanel });

            // Ensure text renders without clipping on high-DPI and with emoji
            ApplyTextTuning(this);
        }

        private void DisplayBoardingPass()
        {
            var contentPanel = (Panel)Controls.Find("contentPanel", true)[0];
            contentPanel.Controls.Clear();

            // Use SuspendLayout/ResumeLayout to prevent flickering during control addition
            contentPanel.SuspendLayout();

            try
            {
                // Passenger information
                var passengerLabel = new Label
                {
                    Text = "PASSENGER",
                    Location = new Point(40, 30),
                    Size = new Size(250, 25),
                    Font = new Font("Segoe UI", 12F, FontStyle.Bold),
                    ForeColor = Color.FromArgb(107, 114, 128),
                    BackColor = Color.Transparent
                };

                var passengerName = new Label
                {
                    Text = _boardingPass.PassengerName.ToUpper(),
                    Location = new Point(40, 60),
                    Size = new Size(700, 60),
                    Font = new Font("Segoe UI", 24F, FontStyle.Bold),
                    ForeColor = Color.FromArgb(17, 24, 39),
                    BackColor = Color.Transparent,
                    TextAlign = ContentAlignment.MiddleLeft,
                    AutoSize = false
                };

                // Flight info
                var flightLabel = new Label
                {
                    Text = "FLIGHT",
                    Location = new Point(40, 140),
                    Size = new Size(150, 25),
                    Font = new Font("Segoe UI", 12F, FontStyle.Bold),
                    ForeColor = Color.FromArgb(107, 114, 128),
                    BackColor = Color.Transparent
                };

                var flightNumber = new Label
                {
                    Text = _flightNumber,
                    Location = new Point(40, 170),
                    Size = new Size(200, 55),
                    Font = new Font("Segoe UI", 24F, FontStyle.Bold),
                    ForeColor = Color.FromArgb(17, 24, 39),
                    BackColor = Color.Transparent,
                    TextAlign = ContentAlignment.MiddleLeft,
                    AutoSize = false
                };

                // Seat info
                var seatLabel = new Label
                {
                    Text = "SEAT",
                    Location = new Point(300, 140),
                    Size = new Size(150, 25),
                    Font = new Font("Segoe UI", 12F, FontStyle.Bold),
                    ForeColor = Color.FromArgb(107, 114, 128),
                    BackColor = Color.Transparent
                };

                var seatNumber = new Label
                {
                    Text = _seatCode,
                    Location = new Point(300, 170),
                    Size = new Size(150, 55),
                    Font = new Font("Segoe UI", 24F, FontStyle.Bold),
                    ForeColor = Color.FromArgb(17, 24, 39),
                    BackColor = Color.Transparent,
                    TextAlign = ContentAlignment.MiddleLeft,
                    AutoSize = false
                };

                // Passport info
                var passportLabel = new Label
                {
                    Text = "PASSPORT",
                    Location = new Point(520, 140),
                    Size = new Size(150, 25),
                    Font = new Font("Segoe UI", 12F, FontStyle.Bold),
                    ForeColor = Color.FromArgb(107, 114, 128),
                    BackColor = Color.Transparent
                };

                var passportNumber = new Label
                {
                    Text = _boardingPass.PassportNumber,
                    Location = new Point(520, 170),
                    Size = new Size(250, 55),
                    Font = new Font("Segoe UI", 20F, FontStyle.Bold),
                    ForeColor = Color.FromArgb(17, 24, 39),
                    BackColor = Color.Transparent,
                    TextAlign = ContentAlignment.MiddleLeft,
                    AutoSize = false
                };

                // Boarding pass ID
                var idLabel = new Label
                {
                    Text = "BOARDING PASS ID",
                    Location = new Point(40, 250),
                    Size = new Size(250, 25),
                    Font = new Font("Segoe UI", 12F, FontStyle.Bold),
                    ForeColor = Color.FromArgb(107, 114, 128),
                    BackColor = Color.Transparent
                };

                var idNumber = new Label
                {
                    Text = $"BP{_boardingPass.Id:D6}",
                    Location = new Point(40, 280),
                    Size = new Size(250, 45),
                    Font = new Font("Segoe UI", 16F, FontStyle.Bold),
                    ForeColor = Color.FromArgb(17, 24, 39),
                    BackColor = Color.Transparent,
                    TextAlign = ContentAlignment.MiddleLeft,
                    AutoSize = false
                };

                // Gate
                var gateLabel = new Label
                {
                    Text = "GATE",
                    Location = new Point(220, 180),
                    Size = new Size(100, 20),
                    Font = new Font("Segoe UI", 9F, FontStyle.Bold),
                    ForeColor = Color.FromArgb(107, 114, 128),
                    BackColor = Color.Transparent
                };

                var gateNumber = new Label
                {
                    Text = "A12",
                    Location = new Point(220, 205),
                    Size = new Size(100, 30),
                    Font = new Font("Segoe UI", 12F, FontStyle.Bold),
                    ForeColor = Color.FromArgb(17, 24, 39),
                    BackColor = Color.Transparent,
                    TextAlign = ContentAlignment.MiddleLeft
                };

                // Boarding time
                var boardingTimeLabel = new Label
                {
                    Text = "BOARDING TIME",
                    Location = new Point(520, 250),
                    Size = new Size(200, 25),
                    Font = new Font("Segoe UI", 12F, FontStyle.Bold),
                    ForeColor = Color.FromArgb(107, 114, 128),
                    BackColor = Color.Transparent
                };

                var boardingTime = new Label
                {
                    Text = DateTime.Now.AddMinutes(30).ToString("HH:mm"),
                    Location = new Point(520, 280),
                    Size = new Size(150, 45),
                    Font = new Font("Segoe UI", 16F, FontStyle.Bold),
                    ForeColor = Color.FromArgb(17, 24, 39),
                    BackColor = Color.Transparent,
                    TextAlign = ContentAlignment.MiddleLeft,
                    AutoSize = false
                };

                // Success message with border
                var successPanel = new Panel
                {
                    Anchor = AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Bottom,
                    Location = new Point(40, 340),
                    Size = new Size(720, 60),
                    BackColor = Color.FromArgb(220, 252, 231)
                };
                
                // Add border to success panel
                successPanel.Paint += (s, e) =>
                {
                    using (var borderPen = new Pen(Color.FromArgb(34, 197, 94), 1))
                    {
                        e.Graphics.DrawRectangle(borderPen, 0, 0, successPanel.Width - 1, successPanel.Height - 1);
                    }
                };

                var successIcon = new Label
                {
                    Text = "✅",
                    Location = new Point(20, 18),
                    Size = new Size(40, 25),
                    Font = new Font("Segoe UI", 18F),
                    BackColor = Color.Transparent
                };

                var successMessage = new Label
                {
                    Text = "CHECK-IN SUCCESSFUL! Please proceed to security and gate.",
                    Location = new Point(70, 15),
                    Size = new Size(640, 30),
                    Font = new Font("Segoe UI", 14F, FontStyle.Bold),
                    ForeColor = Color.FromArgb(21, 128, 61),
                    BackColor = Color.Transparent,
                    TextAlign = ContentAlignment.MiddleLeft,
                    AutoSize = false
                };

                successPanel.Controls.AddRange(new Control[] { successIcon, successMessage });

                contentPanel.Controls.AddRange(new Control[] {
                    passengerLabel, passengerName,
                    flightLabel, flightNumber,
                    seatLabel, seatNumber,
                    passportLabel, passportNumber,
                    idLabel, idNumber,
                    gateLabel, gateNumber,
                    boardingTimeLabel, boardingTime,
                    successPanel
                });
            }
            finally
            {
                contentPanel.ResumeLayout();
            }
        }

        private void PrintBtn_Click(object? sender, EventArgs e)
        {
            try
            {
                var printDialog = new PrintDialog();
                var printDocument = new PrintDocument();
                printDocument.PrintPage += PrintDocument_PrintPage;

                if (printDialog.ShowDialog() == DialogResult.OK)
                {
                    printDocument.PrinterSettings = printDialog.PrinterSettings;
                    printDocument.Print();
                    MessageBox.Show("Boarding pass printed successfully!", "Print Success", 
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Printing error: {ex.Message}", "Print Error", 
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void PrintDocument_PrintPage(object? sender, PrintPageEventArgs e)
        {
            if (e.Graphics == null) return;

            var font = new Font("Segoe UI", 10F);
            var titleFont = new Font("Segoe UI", 16F, FontStyle.Bold);
            var headerFont = new Font("Segoe UI", 12F, FontStyle.Bold);
            var brush = new SolidBrush(Color.Black);

            float y = 100;
            var leftMargin = 100f;

            // Title
            e.Graphics.DrawString("✈️ SKYLINE AIRWAYS - BOARDING PASS", titleFont, brush, leftMargin, y);
            y += 40;

            // Passenger info
            e.Graphics.DrawString($"Passenger: {_boardingPass.PassengerName}", headerFont, brush, leftMargin, y);
            y += 25;
            e.Graphics.DrawString($"Passport: {_boardingPass.PassportNumber}", font, brush, leftMargin, y);
            y += 30;

            // Flight info
            e.Graphics.DrawString($"Flight: {_flightNumber}", headerFont, brush, leftMargin, y);
            y += 25;
            e.Graphics.DrawString($"Seat: {_seatCode}", font, brush, leftMargin, y);
            y += 25;
            e.Graphics.DrawString($"Boarding Pass ID: BP{_boardingPass.Id:D6}", font, brush, leftMargin, y);
            y += 25;
            e.Graphics.DrawString($"Issued: {_boardingPass.IssuedAt:dd MMM yyyy HH:mm}", font, brush, leftMargin, y);
            y += 40;

            e.Graphics.DrawString("✅ CHECK-IN SUCCESSFUL", headerFont, brush, leftMargin, y);
            y += 25;
            e.Graphics.DrawString("Please proceed to security and gate.", font, brush, leftMargin, y);

            font.Dispose();
            titleFont.Dispose();
            headerFont.Dispose();
            brush.Dispose();
        }

        private void SaveBtn_Click(object? sender, EventArgs e)
        {
            try
            {
                var saveDialog = new SaveFileDialog
                {
                    Filter = "Text files (*.txt)|*.txt|All files (*.*)|*.*",
                    DefaultExt = "txt",
                    FileName = $"BoardingPass_{_flightNumber}_{_boardingPass.PassportNumber}_{DateTime.Now:yyyyMMdd_HHmmss}.txt"
                };

                if (saveDialog.ShowDialog() == DialogResult.OK)
                {
                    var content = $@"
✈️ SKYLINE AIRWAYS - BOARDING PASS
=====================================

Passenger: {_boardingPass.PassengerName}
Passport: {_boardingPass.PassportNumber}
Flight: {_flightNumber}
Seat: {_seatCode}
Boarding Pass ID: BP{_boardingPass.Id:D6}
Issued: {_boardingPass.IssuedAt:dd MMM yyyy HH:mm}

✅ CHECK-IN SUCCESSFUL
Please proceed to security and gate.

Thank you for flying with Skyline Airways!
";

                    System.IO.File.WriteAllText(saveDialog.FileName, content);
                    MessageBox.Show("Boarding pass saved successfully!", "Save Success", 
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Save error: {ex.Message}", "Save Error", 
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void ApplyTextTuning(Control root)
        {
            foreach (Control child in root.Controls)
            {
                if (child is Label lbl)
                {
                    lbl.UseCompatibleTextRendering = true;
                    if (lbl.Padding.Top < 2)
                    {
                        lbl.Padding = new Padding(lbl.Padding.Left, 2, lbl.Padding.Right, lbl.Padding.Bottom);
                    }
                }
                else if (child is Button btn)
                {
                    btn.UseCompatibleTextRendering = true;
                }

                if (child.HasChildren)
                {
                    ApplyTextTuning(child);
                }
            }
        }
    }
}