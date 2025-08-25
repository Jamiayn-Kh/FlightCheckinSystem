using System;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.Windows.Forms;
using FlightCheckin.Models;
using System.Collections.Generic; // Added missing import
using System.Drawing; // Added missing import

namespace FlightCheckin.Desktop
{
    public partial class ConcurrentTestForm : Form
    {
        private readonly HttpClient _httpClient;
        private readonly TextBox _logTextBox;

        public ConcurrentTestForm()
        {
            _httpClient = new HttpClient { BaseAddress = new Uri("http://localhost:5000/") };
            
            this.Text = "Concurrent Booking Test";
            this.Size = new Size(800, 600);
            this.StartPosition = FormStartPosition.CenterScreen;

            var panel = new Panel
            {
                Dock = DockStyle.Fill,
                Padding = new Padding(10)
            };

            var testButton = new Button
            {
                Text = "Start Concurrent Test",
                Size = new Size(200, 40),
                Location = new Point(10, 10)
            };
            testButton.Click += StartConcurrentTest_Click;

            _logTextBox = new TextBox
            {
                Multiline = true,
                ScrollBars = ScrollBars.Vertical,
                ReadOnly = true,
                Font = new Font("Consolas", 9),
                Location = new Point(10, 60),
                Size = new Size(760, 480)
            };

            panel.Controls.Add(testButton);
            panel.Controls.Add(_logTextBox);
            this.Controls.Add(panel);
        }

        private async void StartConcurrentTest_Click(object? sender, EventArgs e)
        {
            _logTextBox.Clear();
            Log("Starting concurrent booking test...");
            Log("This will simulate 5 passengers trying to book the same seat simultaneously.");
            Log("");

            var tasks = new List<Task>();
            var seatRow = 1;
            var seatColumn = "A";

            for (int i = 1; i <= 5; i++)
            {
                var passengerNumber = i;
                var task = Task.Run(async () =>
                {
                    await SimulateBooking(passengerNumber, seatRow, seatColumn);
                });
                tasks.Add(task);
            }

            await Task.WhenAll(tasks);
            Log("");
            Log("Test completed!");
        }

        private async Task SimulateBooking(int passengerNumber, int seatRow, string seatColumn)
        {
            var request = new CheckinRequest
            {
                FlightNumber = "MGL101",
                PassportNumber = $"TEST{passengerNumber:D3}",
                PassengerName = $"Test Passenger {passengerNumber}",
                SeatRow = seatRow,
                SeatColumn = seatColumn
            };

            try
            {
                Log($"Passenger {passengerNumber}: Attempting to book seat {seatRow}{seatColumn}...");
                
                var json = JsonSerializer.Serialize(request);
                var content = new StringContent(json, Encoding.UTF8, "application/json");
                var response = await _httpClient.PostAsync("api/checkin", content);

                if (response.IsSuccessStatusCode)
                {
                    var responseJson = await response.Content.ReadAsStringAsync();
                    var checkinResponse = JsonSerializer.Deserialize<CheckinResponse>(responseJson, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
                    
                    if (checkinResponse?.Success == true)
                    {
                        Log($"Passenger {passengerNumber}: SUCCESS - Booked seat {checkinResponse.SeatCode}");
                    }
                    else
                    {
                        Log($"Passenger {passengerNumber}: FAILED - {checkinResponse?.Message}");
                    }
                }
                else
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    Log($"Passenger {passengerNumber}: HTTP ERROR {response.StatusCode} - {errorContent}");
                }
            }
            catch (Exception ex)
            {
                Log($"Passenger {passengerNumber}: EXCEPTION - {ex.Message}");
            }
        }

        private void Log(string message)
        {
            if (_logTextBox.InvokeRequired)
            {
                _logTextBox.Invoke(new Action(() => Log(message)));
            }
            else
            {
                _logTextBox.AppendText($"[{DateTime.Now:HH:mm:ss.fff}] {message}{Environment.NewLine}");
                _logTextBox.SelectionStart = _logTextBox.Text.Length;
                _logTextBox.ScrollToCaret();
            }
        }

        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            _httpClient?.Dispose();
            base.OnFormClosing(e);
        }
    }
}



