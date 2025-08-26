using System;
using System.Net.Sockets;
using System.Text;
using System.Text.Json;
using System.Windows.Forms;
using FlightCheckin.Models;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Net.Http;
using System.Drawing;

namespace FlightCheckin.Desktop
{
    public partial class CheckinForm : Form
    {
        private TcpClient? _client;
        private NetworkStream? _stream;
        private readonly HttpClient _httpClient;
        private System.Windows.Forms.Timer _statusTimer;

        public CheckinForm()
        {
            InitializeComponent();
            _httpClient = new HttpClient { BaseAddress = new Uri("http://localhost:5051/") };
            _statusTimer = new System.Windows.Forms.Timer { Interval = 5000 }; // 5 seconds
            _statusTimer.Tick += StatusTimer_Tick;
            _statusTimer.Start();
            
            _ = LoadFlights();
        }

        private void InitializeComponent()
        {
            this.Text = "✈️ Flight Check-in System";
            this.Size = new Size(820, 720); // Increased size for better spacing
            this.StartPosition = FormStartPosition.CenterScreen;
            this.BackColor = Color.FromArgb(241, 245, 249); // Modern slate background
            this.Font = new Font("Segoe UI", 9F, FontStyle.Regular);
            this.FormBorderStyle = FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.Padding = new Padding(10); // Add form padding

            // Flight selection group
            var flightGroup = new GroupBox
            {
                Text = "✈️ Flight Selection",
                Location = new Point(25, 25),
                Size = new Size(760, 120), // Increased height for better spacing
                Font = new Font("Segoe UI", 10F, FontStyle.Bold),
                ForeColor = Color.FromArgb(30, 41, 59),
                BackColor = Color.FromArgb(255, 255, 255),
                FlatStyle = FlatStyle.Flat,
                Padding = new Padding(15, 20, 15, 15) // Add internal padding
            };

            var flightLabel = new Label 
            { 
                Text = "Flight:", 
                Location = new Point(25, 35), 
                Font = new Font("Segoe UI", 9F, FontStyle.Regular),
                ForeColor = Color.FromArgb(51, 65, 85),
                Size = new Size(65, 22)
            };
            var flightCombo = new ComboBox
            {
                Name = "flightCombo",
                Location = new Point(95, 33),
                Size = new Size(240, 28), // Slightly larger
                DropDownStyle = ComboBoxStyle.DropDownList,
                Font = new Font("Segoe UI", 9F),
                BackColor = Color.White,
                ForeColor = Color.FromArgb(51, 65, 85),
                FlatStyle = FlatStyle.Flat
            };

            var refreshBtn = new Button
            {
                Text = "🔄 Refresh",
                Location = new Point(350, 32),
                Size = new Size(120, 30), // Slightly larger
                Font = new Font("Segoe UI", 9F, FontStyle.Bold),
                BackColor = Color.FromArgb(37, 99, 235),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                FlatAppearance = { BorderSize = 0, BorderColor = Color.FromArgb(29, 78, 216) },
                Cursor = Cursors.Hand
            };
            refreshBtn.Click += RefreshFlights_Click;

            var statusLabel = new Label 
            { 
                Text = "Status:", 
                Location = new Point(25, 75), 
                Font = new Font("Segoe UI", 9F, FontStyle.Regular),
                ForeColor = Color.FromArgb(51, 65, 85),
                Size = new Size(65, 22)
            };
            var statusCombo = new ComboBox
            {
                Name = "statusCombo",
                Location = new Point(95, 73),
                Size = new Size(240, 28),
                DropDownStyle = ComboBoxStyle.DropDownList,
                Font = new Font("Segoe UI", 9F),
                BackColor = Color.White,
                ForeColor = Color.FromArgb(51, 65, 85),
                FlatStyle = FlatStyle.Flat
            };
            statusCombo.Items.AddRange(Enum.GetNames<FlightStatus>());

            var changeStatusBtn = new Button
            {
                Text = "✈️ Change Status",
                Location = new Point(485, 72),
                Size = new Size(140, 30),
                Font = new Font("Segoe UI", 9F, FontStyle.Bold),
                BackColor = Color.FromArgb(16, 185, 129),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                FlatAppearance = { BorderSize = 0, BorderColor = Color.FromArgb(5, 150, 105) },
                Cursor = Cursors.Hand
            };
            changeStatusBtn.Click += ChangeStatus_Click;

            flightGroup.Controls.AddRange(new Control[] { flightLabel, flightCombo, refreshBtn, statusLabel, statusCombo, changeStatusBtn });

            // Passenger info group
            var passengerGroup = new GroupBox
            {
                Text = "👤 Passenger Information",
                Location = new Point(25, 160), // More space from previous group
                Size = new Size(760, 140), // Increased height
                Font = new Font("Segoe UI", 10F, FontStyle.Bold),
                ForeColor = Color.FromArgb(30, 41, 59),
                BackColor = Color.FromArgb(255, 255, 255),
                FlatStyle = FlatStyle.Flat,
                Padding = new Padding(15, 20, 15, 15)
            };

            var passportLabel = new Label 
            { 
                Text = "Passport:", 
                Location = new Point(25, 35), 
                Font = new Font("Segoe UI", 9F, FontStyle.Regular),
                ForeColor = Color.FromArgb(51, 65, 85),
                Size = new Size(75, 22)
            };
            var passportText = new TextBox
            {
                Name = "passportText",
                Location = new Point(105, 33),
                Size = new Size(240, 28),
                Font = new Font("Segoe UI", 9F),
                BackColor = Color.FromArgb(249, 250, 251),
                ForeColor = Color.FromArgb(17, 24, 39),
                BorderStyle = BorderStyle.FixedSingle
            };

            var nameLabel = new Label 
            { 
                Text = "Name:", 
                Location = new Point(25, 70), 
                Font = new Font("Segoe UI", 9F, FontStyle.Regular),
                ForeColor = Color.FromArgb(51, 65, 85),
                Size = new Size(75, 22)
            };
            var nameText = new TextBox
            {
                Name = "nameText",
                Location = new Point(105, 68),
                Size = new Size(240, 28),
                Font = new Font("Segoe UI", 9F),
                BackColor = Color.FromArgb(249, 250, 251),
                ForeColor = Color.FromArgb(17, 24, 39),
                BorderStyle = BorderStyle.FixedSingle
            };

            var seatLabel = new Label 
            { 
                Text = "Seat:", 
                Location = new Point(25, 105), 
                Font = new Font("Segoe UI", 9F, FontStyle.Regular),
                ForeColor = Color.FromArgb(51, 65, 85),
                Size = new Size(75, 22)
            };
            var seatRowText = new TextBox
            {
                Name = "seatRowText",
                Location = new Point(105, 103),
                Size = new Size(65, 28),
                Text = "1",
                Font = new Font("Segoe UI", 9F, FontStyle.Bold),
                BackColor = Color.FromArgb(249, 250, 251),
                ForeColor = Color.FromArgb(17, 24, 39),
                BorderStyle = BorderStyle.FixedSingle,
                TextAlign = HorizontalAlignment.Center
            };

            var seatColText = new TextBox
            {
                Name = "seatColText",
                Location = new Point(180, 103),
                Size = new Size(65, 28),
                Text = "A",
                Font = new Font("Segoe UI", 9F, FontStyle.Bold),
                BackColor = Color.FromArgb(249, 250, 251),
                ForeColor = Color.FromArgb(17, 24, 39),
                BorderStyle = BorderStyle.FixedSingle,
                TextAlign = HorizontalAlignment.Center
            };

            var checkinBtn = new Button
            {
                Text = "🎫 Check-in",
                Location = new Point(260, 102),
                Size = new Size(130, 30),
                Font = new Font("Segoe UI", 9F, FontStyle.Bold),
                BackColor = Color.FromArgb(147, 51, 234),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                FlatAppearance = { BorderSize = 0, BorderColor = Color.FromArgb(124, 58, 237) },
                Cursor = Cursors.Hand
            };
            checkinBtn.Click += Checkin_Click;

            passengerGroup.Controls.AddRange(new Control[] { passportLabel, passportText, nameLabel, nameText, seatLabel, seatRowText, seatColText, checkinBtn });

            // Seats display group
            var seatsGroup = new GroupBox
            {
                Text = "💺 Seat Map",
                Location = new Point(25, 315), // More space from previous group
                Size = new Size(760, 320), // Increased height
                Font = new Font("Segoe UI", 10F, FontStyle.Bold),
                ForeColor = Color.FromArgb(30, 41, 59),
                BackColor = Color.FromArgb(255, 255, 255),
                FlatStyle = FlatStyle.Flat,
                Padding = new Padding(15, 20, 15, 15)
            };

            var seatsList = new ListView
            {
                Name = "seatsList",
                Location = new Point(20, 30),
                Size = new Size(720, 275), // Better size
                View = View.Details,
                FullRowSelect = true,
                GridLines = true,
                Font = new Font("Segoe UI", 9F),
                BackColor = Color.FromArgb(248, 250, 252),
                ForeColor = Color.FromArgb(17, 24, 39),
                BorderStyle = BorderStyle.None,
                HeaderStyle = ColumnHeaderStyle.Nonclickable
            };
            seatsList.Columns.Add("🔢 Row", 90);
            seatsList.Columns.Add("🔤 Column", 90);
            seatsList.Columns.Add("📊 Status", 140);
            seatsList.Columns.Add("👤 Passenger", 350);

            seatsGroup.Controls.Add(seatsList);

            // Visual separator line
            var separatorLine = new Label
            {
                Location = new Point(25, 640),
                Size = new Size(760, 2),
                BackColor = Color.FromArgb(209, 213, 219),
                BorderStyle = BorderStyle.None
            };

            // Status display
            var statusDisplay = new Label
            {
                Name = "statusDisplay",
                Text = "🟢 Ready",
                Location = new Point(25, 650), // Adjusted for new form size
                Size = new Size(760, 30), // Slightly larger
                Font = new Font("Segoe UI", 9F, FontStyle.Regular),
                ForeColor = Color.FromArgb(55, 65, 81),
                BackColor = Color.FromArgb(243, 244, 246),
                TextAlign = ContentAlignment.MiddleLeft,
                Padding = new Padding(15, 0, 0, 0), // More padding
                BorderStyle = BorderStyle.None
            };

            this.Controls.AddRange(new Control[] { flightGroup, passengerGroup, seatsGroup, separatorLine, statusDisplay });
        }

        private async Task LoadFlights()
        {
            try
            {
                UpdateStatus("Loading flights...");
                var response = await _httpClient.GetAsync("api/flight");
                UpdateStatus($"Response status: {response.StatusCode}");
                
                if (response.IsSuccessStatusCode)
                {
                    var json = await response.Content.ReadAsStringAsync();
                    UpdateStatus($"Received JSON: {json}");
                    
                    var flights = JsonSerializer.Deserialize<List<FlightDto>>(json, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
                    
                    var flightCombo = (ComboBox)Controls.Find("flightCombo", true)[0];
                    flightCombo.Items.Clear();
                    if (flights != null && flights.Count > 0)
                    {
                        foreach (var flight in flights)
                        {
                            flightCombo.Items.Add(flight.FlightNumber);
                        }
                        if (flightCombo.Items.Count > 0)
                        {
                            flightCombo.SelectedIndex = 0;
                            UpdateStatus($"Loaded {flights.Count} flights. Selected: {flightCombo.SelectedItem}");
                            await LoadSeats();
                        }
                    }
                    else
                    {
                        UpdateStatus("No flights found in response");
                    }
                }
                else
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    UpdateStatus($"Error response: {response.StatusCode} - {errorContent}");
                }
            }
            catch (Exception ex)
            {
                UpdateStatus($"Error loading flights: {ex.Message}");
                MessageBox.Show($"Error loading flights: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private async void RefreshFlights_Click(object? sender, EventArgs e)
        {
            await LoadFlights();
        }

        private async void ChangeStatus_Click(object? sender, EventArgs e)
        {
            var flightCombo = (ComboBox)Controls.Find("flightCombo", true)[0];
            var statusCombo = (ComboBox)Controls.Find("statusCombo", true)[0];

            if (flightCombo.SelectedItem == null || statusCombo.SelectedItem == null)
            {
                MessageBox.Show("Please select both flight and status");
                return;
            }

            try
            {
                var request = new StatusChangeRequest
                {
                    FlightNumber = flightCombo.SelectedItem.ToString()!,
                    Status = Enum.Parse<FlightStatus>(statusCombo.SelectedItem.ToString()!)
                };

                var json = JsonSerializer.Serialize(request);
                var content = new StringContent(json, Encoding.UTF8, "application/json");
                var response = await _httpClient.PutAsync("api/flight/status", content);

                if (response.IsSuccessStatusCode)
                {
                    UpdateStatus("Flight status updated successfully");
                    await LoadSeats();
                }
                else
                {
                    UpdateStatus($"Error updating status: {response.StatusCode}");
                }
            }
            catch (Exception ex)
            {
                UpdateStatus($"Error: {ex.Message}");
            }
        }

        private async void Checkin_Click(object? sender, EventArgs e)
        {
            var flightCombo = (ComboBox)Controls.Find("flightCombo", true)[0];
            var passportText = (TextBox)Controls.Find("passportText", true)[0];
            var nameText = (TextBox)Controls.Find("nameText", true)[0];
            var seatRowText = (TextBox)Controls.Find("seatRowText", true)[0];
            var seatColText = (TextBox)Controls.Find("seatColText", true)[0];

            if (flightCombo.SelectedItem == null || string.IsNullOrWhiteSpace(passportText.Text))
            {
                MessageBox.Show("Please select flight and enter passport number");
                return;
            }

            try
            {
                var request = new CheckinRequest
                {
                    FlightNumber = flightCombo.SelectedItem.ToString()!,
                    PassportNumber = passportText.Text.Trim(),
                    PassengerName = nameText.Text.Trim()
                };

                if (!string.IsNullOrWhiteSpace(seatRowText.Text) && !string.IsNullOrWhiteSpace(seatColText.Text))
                {
                    if (int.TryParse(seatRowText.Text, out var row))
                    {
                        request.SeatRow = row;
                        request.SeatColumn = seatColText.Text.Trim().ToUpper();
                    }
                }

                // Try HTTP first, then fallback to Socket
                var success = await TryHttpCheckin(request);
                if (!success)
                {
                    success = await TrySocketCheckin(request);
                }

                if (success)
                {
                    UpdateStatus("Check-in successful");
                    await LoadSeats();
                    ClearPassengerFields();
                }
            }
            catch (Exception ex)
            {
                UpdateStatus($"Check-in error: {ex.Message}");
            }
        }

        private async Task<bool> TryHttpCheckin(CheckinRequest request)
        {
            try
            {
                var json = JsonSerializer.Serialize(request);
                var content = new StringContent(json, Encoding.UTF8, "application/json");
                var response = await _httpClient.PostAsync("api/checkin", content);

                if (response.IsSuccessStatusCode)
                {
                    var responseJson = await response.Content.ReadAsStringAsync();
                    var checkinResponse = JsonSerializer.Deserialize<CheckinResponse>(responseJson, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
                    
                    if (checkinResponse?.Success == true)
                    {
                        // Show boarding pass immediately if available
                        if (checkinResponse.BoardingPass != null)
                        {
                            ShowBoardingPass(checkinResponse.BoardingPass, request.FlightNumber, checkinResponse.SeatCode);
                        }
                        else
                        {
                            MessageBox.Show($"Check-in successful! Seat: {checkinResponse.SeatCode}", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        }
                        return true;
                    }
                }
                return false;
            }
            catch
            {
                return false;
            }
        }

        private async Task<bool> TrySocketCheckin(CheckinRequest request)
        {
            try
            {
                _client = new TcpClient();
                await _client.ConnectAsync("localhost", 8888);
                _stream = _client.GetStream();

                var json = JsonSerializer.Serialize(request);
                var data = Encoding.UTF8.GetBytes(json + "\n");
                await _stream.WriteAsync(data);

                var buffer = new byte[1024];
                var bytesRead = await _stream.ReadAsync(buffer);
                var response = Encoding.UTF8.GetString(buffer, 0, bytesRead);

                var checkinResponse = JsonSerializer.Deserialize<CheckinResponse>(response, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
                
                if (checkinResponse?.Success == true)
                {
                    // Show boarding pass immediately if available
                    if (checkinResponse.BoardingPass != null)
                    {
                        ShowBoardingPass(checkinResponse.BoardingPass, request.FlightNumber, checkinResponse.SeatCode);
                    }
                    else
                    {
                        MessageBox.Show($"Check-in successful! Seat: {checkinResponse.SeatCode}", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                    return true;
                }
                else
                {
                    MessageBox.Show($"Check-in failed: {checkinResponse?.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return false;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Socket error: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }
            finally
            {
                _stream?.Close();
                _client?.Close();
            }
        }

        private async Task LoadSeats()
        {
            var flightCombo = (ComboBox)Controls.Find("flightCombo", true)[0];
            if (flightCombo.SelectedItem == null) return;

            try
            {
                var flightNumber = flightCombo.SelectedItem.ToString()!;
                var response = await _httpClient.GetAsync($"api/checkin/seats/{flightNumber}");
                
                if (response.IsSuccessStatusCode)
                {
                    var json = await response.Content.ReadAsStringAsync();
                    var seats = JsonSerializer.Deserialize<List<SeatDto>>(json, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
                    
                    var seatsList = (ListView)Controls.Find("seatsList", true)[0];
                    seatsList.Items.Clear();
                    
                    if (seats != null)
                    {
                        foreach (var seat in seats)
                        {
                            var item = new ListViewItem(seat.Row.ToString());
                            item.SubItems.Add(seat.Column);
                            item.SubItems.Add(seat.IsTaken ? "✅ Taken" : "🟢 Available");
                            item.SubItems.Add(seat.AssignedPassengerName ?? "");
                            
                            // Enhanced color coding for seat status
                            if (seat.IsTaken)
                            {
                                item.BackColor = Color.FromArgb(254, 242, 242); // Modern light red
                                item.ForeColor = Color.FromArgb(185, 28, 28); // Modern red
                            }
                            else
                            {
                                item.BackColor = Color.FromArgb(240, 253, 244); // Modern light green
                                item.ForeColor = Color.FromArgb(22, 163, 74); // Modern green
                            }
                            
                            seatsList.Items.Add(item);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                UpdateStatus($"Error loading seats: {ex.Message}");
            }
        }

        private void StatusTimer_Tick(object? sender, EventArgs e)
        {
            _ = LoadSeats();
        }

        private void UpdateStatus(string message)
        {
            var statusDisplay = (Label)Controls.Find("statusDisplay", true)[0];
            statusDisplay.Text = $"🕒 {DateTime.Now:HH:mm:ss} - {message}";
        }

        private void ClearPassengerFields()
        {
            var passportText = (TextBox)Controls.Find("passportText", true)[0];
            var nameText = (TextBox)Controls.Find("nameText", true)[0];
            var seatRowText = (TextBox)Controls.Find("seatRowText", true)[0];
            var seatColText = (TextBox)Controls.Find("seatColText", true)[0];

            passportText.Clear();
            nameText.Clear();
            seatRowText.Text = "1";
            seatColText.Text = "A";
        }

        private void ShowBoardingPass(BoardingPass boardingPass, string flightNumber, string seatCode)
        {
            try
            {
                var boardingPassForm = new BoardingPassForm(boardingPass, flightNumber, seatCode);
                boardingPassForm.ShowDialog(this);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error displaying boarding pass: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                // Fallback to simple message
                MessageBox.Show($"Check-in successful! Seat: {seatCode}\nBoarding Pass ID: BP{boardingPass.Id:D6}", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            _statusTimer?.Stop();
            _statusTimer?.Dispose();
            _httpClient?.Dispose();
            _stream?.Close();
            _client?.Close();
            base.OnFormClosing(e);
        }
    }
}

