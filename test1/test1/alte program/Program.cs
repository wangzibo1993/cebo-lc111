using System;
using System.Collections.Generic;
using System.IO;
using CeboMsrNet;
using System.Data;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using System.Runtime.InteropServices;


namespace Datensammeln
{
    class Datensammeln

    {

        private void SaveCsv(Device device)
        {
            for (int j = 0; j < 5; ++j)
            {
                FileStream fs = null;
                StreamWriter sw = null;
                string filePath = @"C:\cebo-lc\test1\test1\Daten\";
                try
                {
                    DateTime d = DateTime.Now;
                    string name = d.ToString("yyyy_MM_dd_HH_mm_ss_fffff");
                    IInput[] inputs = new IInput[]
                    {
                        device.SingleEndedInputs[0],
                        device.SingleEndedInputs[1],

                    };
                    device.SetupInputFrame(inputs);

                    double zeit1 = (DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1))).TotalMilliseconds;
                    fs = new FileStream(filePath + "_" + name + ".csv", FileMode.Create, FileAccess.Write);
                    sw = new StreamWriter(fs, Encoding.Default);
                    double zeit2 = (DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1))).TotalMilliseconds;
                    Console.WriteLine(zeit2 - zeit1);

                    //写出列名称

                    string ListName = "Time" + "," + "SingleEnded0" + "," + "SingleEnded1";
                    sw.WriteLine(ListName);

                    for (int i = 0; i < 1000; ++i)
                    {
                        InputFrame inputFrame = device.ReadFrame();
                        double zeit = (DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1))).TotalMilliseconds;
                        string data = string.Empty;
                        data = zeit + "," + inputFrame.GetSingleEnded(0) + "," + inputFrame.GetSingleEnded(1);
                        sw.WriteLine(data);
                    };
                }
                catch (IOException ex)
                {
                    throw new IOException(ex.Message, ex);
                }
                finally
                {
                    if (sw != null) sw.Close();
                    if (fs != null) fs.Close();
                }
            }
        }


        private void Runexample()
        {
             try
            {
                IList<Device> devices = LibraryInterface.Enumerate(DeviceType.CeboLC);
                if (devices.Count > 0)
                {
                    Device device = devices[0];
                    device.Open();

                    //Datensammeln.Timer();
                    SaveCsv(device);

                    device.Close();

                }
            }
            catch (IOException ex)
            {
                Console.WriteLine(ex.ToString());
            }
        }


        static void Main(string[] args)
        {
            new Datensammeln().Runexample();
        }
    }
}

