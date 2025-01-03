using System;
using System.Windows.Forms;

namespace ScannerTrackingApp
{
    public partial class ScannedDataForm : Form
    {
        public ScannedDataForm()
        {
            InitializeComponent();
        }
        public void UpdateResponseBody(string responseBody)
        {
            txtResponseBody.AppendText(responseBody + Environment.NewLine);
            Console.WriteLine($"Response updated in form: {responseBody}");
        }
        // Method to add scanned data to the ListBox
        public void AddData(string scannerName , string barcode)
        {
            string data = $"Serial: {scannerName},  Barcode: {barcode}";
            lstDataDisplay.Items.Add(data);
            Console.WriteLine($"Data added to form: {data}");
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            this.Hide(); // Hide the form instead of closing it
        }
    }
}
