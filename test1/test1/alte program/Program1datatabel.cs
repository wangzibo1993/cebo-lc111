using System;
using System.Collections.Generic;
using System.IO;
using CeboMsrNet;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Data;

/**
 * CeboMsr - C# CeboLc DataAcquisition.
 *
 * Multiple inputs are read multiple times at 10000 Hz
 * and Data in the .csv File export.
 */
namespace CeboDaq
{
    class CeboDaq
    {
        static int value;
        /// <summary>
        /// Read blocks of data from device.
        /// </summary>
        /// <param name="device">Device to use for measure</param>
        private void DataAcquisition(Device device)
        {
            // Construct selection that contains inputs to read from.
            IInput[] inputs = new IInput[]
            {
                device.SingleEndedInputs[0],
                device.SingleEndedInputs[1],
            };
            // Prepare device with this collection ...
            device.SetupInputFrame(inputs);

            while (true)
            {
                if (value == 0)
                {
                    DataTable dt = new DataTable("Data");
                    dt.Columns.Add("Time", typeof(double));
                    dt.Columns.Add("SingleEnded0", typeof(float));
                    dt.Columns.Add("SingleEnded1", typeof(float));
                    for (int i = 0; i < 100; ++i)
                    {
                        InputFrame inputFrame = device.ReadFrame();                        
                        DataRow dr = dt.NewRow();
                        double zeit = (DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1))).TotalMilliseconds;
                        dr[0] = zeit;
                        dr[1] = inputFrame.GetSingleEnded(0);
                        dr[2] = inputFrame.GetSingleEnded(1);
                        dt.Rows.Add(dr);
                    }
                    MeasureToCsv.DataToCsv(dt);
                }
                else
                {
                    Console.WriteLine("DataAcquisition is stopped");
                    Thread.Sleep(3000);
                }
            }
        }

        /// <summary>
        /// /use digitalport[0] to control the DataAcquisition process
        /// </summary>
        /// <param name="device"></param>
        /// <returns></returns>
        private async Task StartAndStopControl(Device device)
        {
            await Task.Run(()=>
                {
                    while(true)
                    {

                        DigitalPort dp0 = device.DigitalPorts[0];
                        Thread.Sleep(5000);
                        value = dp0.Value;
                        Thread.Sleep(5000);
                        value = 1;
                    }
                });
        }

        private void DeviceConnect()
        {
            try
            {
                IList<Device> devices = LibraryInterface.Enumerate(DeviceType.CeboLC);
                if (devices.Count > 0)
                {
                    Device device = devices[0];
                    device.Open();
                    StartAndStopControl(device);
                    DataAcquisition(device);
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
            new CeboDaq().DeviceConnect();
        }
    }

    public static class MeasureToCsv
    {
        static string filePath = @"C:\cebo-lc\test1\test1\Daten\";
        static int a=0;
        static int b=1;
        static double TotalSecond = (DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1))).TotalSeconds;
        static string name = (long)TotalSecond + " _ForceSensor9031A_" + b;
        /// <summary>
        /// Data receive and als.csv  file export
        /// 20 times Data(each one 3000 frames) in one .csv file
        /// </summary>
        /// <param name="frames"></param>
        /// <returns></returns>
        public async static Task DataToCsv(DataTable dt)
        {

            await Task.Run(() =>
            {
                
                FileStream fs = null;
                StreamWriter sw = null;
                try
                {
                    fs = new FileStream(filePath + name  + ".csv", FileMode.Append, FileAccess.Write);
                    sw = new StreamWriter(fs, Encoding.Default);
                    if (a == 0)
                    {
                        string ListName = "Time" + "," + "SingleEnded0" + "," + "SingleEnded1";
                        sw.WriteLine(ListName);
                        a++;
                    }
                    else
                        a++;
                    for (int i = 0; i < dt.Rows.Count; i++)
                    {
                        string data = string.Empty;
                        for (var j = 0; j < dt.Columns.Count; j++)
                        {
                            data += dt.Rows[i][j].ToString();
                            if (j < dt.Columns.Count - 1)
                            {
                                data += ",";
                            }
                        }
                        sw.WriteLine(data);
                    }
                        if (a >= 5)
                    {
                        a = 0;
                        b++;
                        TotalSecond = (DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1))).TotalSeconds;
                        name = (long)TotalSecond + " _ForceSensor9031A_" + b;

                    }
                }
                catch (IOException ex)
                {
                    throw new IOException(ex.Message, ex);
                }
                finally
                {
                    if (sw != null)
                        sw.Close();
                    if (fs != null)
                        fs.Close();
                }
            });
        }
    }
}


