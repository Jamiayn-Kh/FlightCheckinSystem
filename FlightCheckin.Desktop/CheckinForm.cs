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
            this.Text = "Flight Check-in System";
            this.Size = new Size(800, 600);
            this.StartPosition = FormStartPosition.CenterScreen;

            // Flight selection group
            var flightGroup = new GroupBox
            {
                Text = "Flight Selection",
                Location = new Point(10, 10),
                Size = new Size(760, 100)
            };

            var flightLabel = new Label { Text = "Flight:", Location = new Point(10, 25) };
            var flightCombo = new ComboBox
            {
                Name = "flightCombo",
                Location = new Point(80, 22),
                Size = new Size(200, 23),
                DropDownStyle = ComboBoxStyle.DropDownList
            };

            var refreshBtn = new Button
            {
                Text = "Refresh",
                Location = new Point(290, 22),
                Size = new Size(80, 23)
            };
            refreshBtn.Click += RefreshFlights_Click;

            var statusLabel = new Label { Text = "Status:", Location = new Point(10, 55) };
            var statusCombo = new ComboBox
            {
                Name = "statusCombo",
                Location = new Point(80, 52),
                Size = new Size(200, 23),
                DropDownStyle = ComboBoxStyle.DropDownList
            };
            statusCombo.Items.AddRange(Enum.GetNames<FlightStatus>());

            var changeStatusBtn = new Button
            {
                Text = "Change Status",
                Location = new Point(290, 52),
                Size = new Size(100, 23)
            };
            changeStatusBtn.Click += ChangeStatus_Click;

            flightGroup.Controls.AddRange(new Control[] { flightLabel, flightCombo, refreshBtn, statusLabel, statusCombo, changeStatusBtn });

            // Passenger info group
            var passengerGroup = new GroupBox
            {
                Text = "Passenger Information",
                Location = new Point(10, 120),
                Size = new Size(760, 120)
            };

            var passportLabel = new Label { Text = "Passport:", Location = new Point(10, 25) };
            var passportText = new TextBox
            {
                Name = "passportText",
                Location = new Point(80, 22),
                Size = new Size(200, 23)
            };

            var nameLabel = new Label { Text = "Name:", Location = new Point(10, 55) };
            var nameText = new TextBox
            {
                Name = "nameText",
                Location = new Point(80, 52),
                Size = new Size(200, 23)
            };

            var seatLabel = new Label { Text = "Seat:", Location = new Point(10, 85) };
            var seatRowText = new TextBox
            {
                Name = "seatRowText",
                Location = new Point(80, 82),
                Size = new Size(50, 23),
                Text = "1"
            };

            var seatColText = new TextBox
            {
                Name = "seatColText",
                Location = new Point(140, 82),
                Size = new Size(50, 23),
                Text = "A"
            };

            var checkinBtn = new Button
            {
                Text = "Check-in",
                Location = new Point(200, 82),
                Size = new Size(80, 23)
            };
            checkinBtn.Click += Checkin_Click;

            passengerGroup.Controls.AddRange(new Control[] { passportLabel, passportText, nameLabel, nameText, seatLabel, seatRowText, seatColText, checkinBtn });

            // Seats display group
            var seatsGroup = new GroupBox
            {
                Text = "Seats",
                Location = new Point(10, 250),
                Size = new Size(760, 300)
            };

            var seatsList = new ListView
            {
                Name = "seatsList",
                Location = new Point(10, 20),
                Size = new Size(740, 270),
                View = View.Details,
                FullRowSelect = true,
                GridLines = true
            };
            seatsList.Columns.Add("Row", 50);
            seatsList.Columns.Add("Column", 50);
            seatsList.Columns.Add("Status", 100);
            seatsList.Columns.Add("Passenger", 200);

            seatsGroup.Controls.Add(seatsList);

            // Status display
            var statusDisplay = new Label
            {
                Name = "statusDisplay",
                Text = "Ready",
                Location = new Point(10, 560),
                Size = new Size(760, 20),
                Font = new Font(FontFamily.GenericSansSerif, 10, FontStyle.Bold)
            };

            this.Controls.AddRange(new Control[] { flightGroup, passengerGroup, seatsGroup, statusDisplay });
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
                        MessageBox.Show($"Check-in successful! Seat: {checkinResponse.SeatCode}", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
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
                    MessageBox.Show($"Check-in successful! Seat: {checkinResponse.SeatCode}", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
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
                            item.SubItems.Add(seat.IsTaken ? "Taken" : "Available");
                            item.SubItems.Add(seat.AssignedPassengerName ?? "");
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
            statusDisplay.Text = $"{DateTime.Now:HH:mm:ss} - {message}";
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

