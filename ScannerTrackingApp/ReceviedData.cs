using System;
using System.Net;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Http;
using Newtonsoft.Json;
using System.IO;
using NAudio.Wave;
using System.Media;
//using Newtonsoft.Json;

namespace ScannerTrackingApp
{
    class ReceviedData
    {
          public class ScannerVM
         {
            public string SerialNumber { get; set; }
             public string BarCode { get; set; }
         }
        public static async Task SendData(string SerialName, string BarCode)
         {
             var myData = new ScannerVM
             {
                 SerialNumber = SerialName,
                 BarCode = BarCode,
             };

             try
             {
                 await SendDataAsync(myData);
             }
             catch (Exception ex)
             {
                 Console.WriteLine($"Error: {ex.Message}");
            }
        }


        public static async Task SendDataAsync(object data)
        {
            string url = "http://192.168.2.45:8081/ScannerBarCodes/AddScannedBarCode";
            using (var client = new HttpClient())
            {
                // Serialize the object to JSON
                var jsonData = JsonConvert.SerializeObject(data);

                // Create HTTP content
                var content = new StringContent(jsonData, Encoding.UTF8, "application/json");

                // Send POST request
                var response = await client.PostAsync(url, content);

                // Ensure response is successful
                response.EnsureSuccessStatusCode();


                string responseBody = await response.Content.ReadAsStringAsync();
                Console.WriteLine($"Response: {responseBody}");
                if (responseBody.Trim() == "OK")
                {
                    PlaySound("correct-answer.wav");
                    Console.WriteLine(responseBody);
                    Program.MainFormInstance.scannedDataForm.UpdateResponseBody(responseBody);
                }
                else
                {
                    PlaySound("error-answer.wav");
                    Console.WriteLine(responseBody);
                    Program.MainFormInstance.scannedDataForm.UpdateResponseBody(responseBody);
                }

            }
        }
        
        private static void PlaySound(string soundFilePath)
        {
            try
            {
                string fullPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, soundFilePath);
                if (File.Exists(fullPath))
                {
                    using (SoundPlayer player = new SoundPlayer(fullPath))
                    {
                        player.PlaySync(); // Play sound synchronously
                    }
                }
                else
                {
                    Console.WriteLine($"Sound file not found: {fullPath}");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error playing sound: {ex.Message}");
            }
        }
    }
}
