using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows.Forms;

namespace ScannerTrackingApp
{
    public partial class MainForm : Form
    {
        private const int WM_INPUT = 0x00FF;

        // Known scanners with their DeviceIDs
        private static readonly List<DeviceInfo> KnownScanners = new List<DeviceInfo>
        {
            new DeviceInfo { Name = "2434581193", DeviceID = "\\\\?\\acpi#dllk086f#4&2f1b6b8e&0#{884b96c3-56ef-11d1-bc8c-00a0c91405dd}" },
            new DeviceInfo { Name = "2434581959", DeviceID = "\\\\?\\hid#vid_34eb&pid_1502#7&27d4eacd&0&0000#{884b96c3-56ef-11d1-bc8c-00a0c91405dd}" }
        };

        internal ScannedDataForm scannedDataForm;
        private TextBox barcodeInputTextBox; // To capture the raw barcode input

        public MainForm()
        {

            InitializeComponent();

            // Initialize the scanned data form and the barcode input TextBox
            scannedDataForm = new ScannedDataForm();
            barcodeInputTextBox = new TextBox
            {
                Dock = DockStyle.Top,
                Font = new System.Drawing.Font("Arial", 16),
                //PlaceholderText = "Scan barcode here...",
                ReadOnly = false // Allow scanner to write input directly
            };

            Controls.Add(barcodeInputTextBox);
        }

        private void MainForm_Load(object sender, EventArgs e)
        {
            scannedDataForm.Show();
            RegisterRawInput(); // Register raw input devices
            barcodeInputTextBox.Focus(); // Ensure the TextBox is focused for scanner input
        }

        protected override void WndProc(ref Message m)
        {
            if (m.Msg == WM_INPUT)
            {
                ProcessRawInput(m.LParam);
            }

            base.WndProc(ref m);
        }

        private void ProcessRawInput(IntPtr lParam)
        
        {
            uint dwSize = 0;
            GetRawInputData(lParam, RID_INPUT, IntPtr.Zero, ref dwSize, (uint)Marshal.SizeOf(typeof(RAWINPUTHEADER)));

            if (dwSize > 0)
            {
                IntPtr rawInputBuffer = Marshal.AllocHGlobal((int)dwSize);

                try
                {
                    if (GetRawInputData(lParam, RID_INPUT, rawInputBuffer, ref dwSize, (uint)Marshal.SizeOf(typeof(RAWINPUTHEADER))) > 0)
                    {
                        RAWINPUT rawInput = (RAWINPUT)Marshal.PtrToStructure(rawInputBuffer, typeof(RAWINPUT));

                        if (rawInput.header.dwType == RIM_TYPEKEYBOARD)
                        {


                            string deviceName = GetDeviceName(rawInput.header.hDevice).ToLowerInvariant();
                            //string deviceName = "\\\\?\\hid#vid_34eb&pid_1502#7&2e569e72&0&0000#{884b96c3-56ef-11d1-bc8c-00a0c91405dd}";

                            
                                Console.WriteLine($"Normalized Device Name: {deviceName}");

                            var activeScanner = FindActiveScanner(deviceName);

                            if (activeScanner != null)
                            {
                                Console.WriteLine($"Active Scanner Identified: {activeScanner.Name}, DeviceID: {activeScanner.DeviceID}");
                            }
                            else
                            {
                                Console.WriteLine("Active Scanner: Unknown (Device not in KnownScanners list)");
                            }

                            if (rawInput.keyboard.VKey == (ushort)Keys.Enter) // Barcode completion
                            {
                                string barcode = barcodeInputTextBox.Text.Trim(); // Read the barcode as-is
                                barcodeInputTextBox.Clear();

                                if (!string.IsNullOrEmpty(barcode))
                                {
                                    LogScannedData(activeScanner, barcode);
                                }
                                else
                                {
                                    Console.WriteLine("Ignored empty barcode.");
                                }
                            }
                        }
                    }
                }
                finally
                {
                    Marshal.FreeHGlobal(rawInputBuffer);
                }
            }
        }

        private DeviceInfo FindActiveScanner(string deviceName)
        
        {
                if (string.IsNullOrEmpty(deviceName))
            {
                return null;
            }

            foreach (var scanner in KnownScanners)
            {
                string normalizedDeviceID = scanner.DeviceID.ToLowerInvariant();

                if (deviceName.Contains(normalizedDeviceID) || normalizedDeviceID.Contains(deviceName))
                {
                    return scanner;
                }
            }

            return null;
        }

        private string GetDeviceName(IntPtr deviceHandle)
        {
            uint size = 0;
            GetRawInputDeviceInfo(deviceHandle, RIDI_DEVICENAME, IntPtr.Zero, ref size);

            if (size > 0)
            {
                var buffer = new char[size];
                GetRawInputDeviceInfo(deviceHandle, RIDI_DEVICENAME, buffer, ref size);
                return new string(buffer).TrimEnd('\0');
            }

            return null;
        }

        private void LogScannedData(DeviceInfo scanner, string barcode)
        {
            Invoke((MethodInvoker)delegate
            {
                string scannerName = scanner?.Name ?? "Unknown";
                string scannerSerial = scanner?.DeviceID ?? "N/A";

                // Add the scanned data to the form
               
               var responce =ReceviedData.SendData(scannerName, barcode);
                scannedDataForm.AddData(scannerName, barcode);
                // Log the scanned data to the output window
                Console.WriteLine($"Data added to form: Scanner: {scannerName}, Serial: {scannerSerial}, Barcode: {barcode}");
            });
        }

        private void RegisterRawInput()
        {
            var rid = new RAWINPUTDEVICE[1];

            rid[0].UsagePage = 0x01; // Generic desktop controls
            rid[0].Usage = 0x06; // Keyboard
            rid[0].Flags = 0; // Default
            rid[0].Target = Handle; // Associate with this window

            if (!RegisterRawInputDevices(rid, (uint)rid.Length, (uint)Marshal.SizeOf(typeof(RAWINPUTDEVICE))))
            {
                throw new ApplicationException("Failed to register raw input devices.");
            }
        }

        // Windows API structures and constants
        private const uint RID_INPUT = 0x10000003;
        private const uint RIDI_DEVICENAME = 0x20000007;
        private const uint RIM_TYPEKEYBOARD = 1;

        [StructLayout(LayoutKind.Sequential)]
        private struct RAWINPUTDEVICE
        {
            public ushort UsagePage;
            public ushort Usage;
            public uint Flags;
            public IntPtr Target;
        }

        [StructLayout(LayoutKind.Sequential)]
        private struct RAWINPUTHEADER
        {
            public uint dwType;
            public uint dwSize;
            public IntPtr hDevice;
            public IntPtr wParam;
        }

        [StructLayout(LayoutKind.Sequential)]
        private struct RAWINPUT
        {
            public RAWINPUTHEADER header;
            public RAWKEYBOARD keyboard;
        }

        [StructLayout(LayoutKind.Sequential)]
        private struct RAWKEYBOARD
        {
            public ushort MakeCode;
            public ushort Flags;
            public ushort Reserved;
            public ushort VKey;
            public uint Message;
            public uint ExtraInformation;
        }

        [DllImport("user32.dll", SetLastError = true)]
        private static extern uint GetRawInputData(IntPtr hRawInput, uint uiCommand, IntPtr pData, ref uint pcbSize, uint cbSizeHeader);

        [DllImport("user32.dll", SetLastError = true, CharSet = CharSet.Unicode)]
        private static extern uint GetRawInputDeviceInfo(IntPtr hDevice, uint uiCommand, char[] pData, ref uint pcbSize);

        [DllImport("user32.dll", SetLastError = true, CharSet = CharSet.Unicode)]
        private static extern uint GetRawInputDeviceInfo(IntPtr hDevice, uint uiCommand, IntPtr pData, ref uint pcbSize);

        [DllImport("user32.dll", SetLastError = true)]
        private static extern bool RegisterRawInputDevices(RAWINPUTDEVICE[] pRawInputDevices, uint uiNumDevices, uint cbSize);

      
    }

    public class DeviceInfo
    {
        public string Name { get; set; }
        public string DeviceID { get; set; }
    }
}
