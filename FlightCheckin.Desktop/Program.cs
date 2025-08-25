using System;
using System.Windows.Forms;

namespace FlightCheckin.Desktop
{
    internal static class Program
    {
        [STAThread]
        static void Main()
        {
            ApplicationConfiguration.Initialize();
            
            // Show form selection dialog
            var result = MessageBox.Show(
                "Choose application mode:\n\n" +
                "Yes = Main Check-in Form\n" +
                "No = Concurrent Test Form",
                "Flight Check-in System",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question);

            if (result == DialogResult.Yes)
            {
                Application.Run(new CheckinForm());
            }
            else
            {
                Application.Run(new ConcurrentTestForm());
            }
        }
    }
}
