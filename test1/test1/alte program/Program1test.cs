using System;
using System.Collections.Generic;
using System.IO;
using CeboMsrNet;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

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
                    // Start sampling ...3000frames are read every time(10000Hz)
                    double TotalMilliseconds1 = (DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1))).TotalMilliseconds;
                    device.StartBufferedDataAcquisition(10000, 3000, false);
                    double TotalMilliseconds2 = (DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1))).TotalMilliseconds;
                    IList<InputFrame> frames = device.ReadBlocking(3000);
                    double TotalMilliseconds3 = (DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1))).TotalMilliseconds;
                    device.StopDataAcquisition();
                    double TotalMilliseconds4 = (DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1))).TotalMilliseconds;

                    //Measure Data in the.csv File export.
                    //MeasureToCsv.DataToCsv(frames3);
                    Console.WriteLine("start_device_timeperiod"+","+(TotalMilliseconds2 - TotalMilliseconds1));
                    Console.WriteLine("3000_Dataaqu_timeperiod" + "," + (TotalMilliseconds3 - TotalMilliseconds2));
                    Console.WriteLine("stop_device_timeperiod" + "," + (TotalMilliseconds4 - TotalMilliseconds3));



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
        static DateTime d = DateTime.Now;
        static string name = d.ToString("yyyy_MM_dd_HH_mm_ss_fff");
        /// <summary>
        /// Data receive and als.csv  file export
        /// 20 times Data(each one 3000 frames) in one .csv file
        /// </summary>
        /// <param name="frames"></param>
        /// <returns></returns>
        public async static Task DataToCsv(IList<InputFrame> frames)
        {
            FileStream fs = null;
            StreamWriter sw = null;
            await Task.Run(() =>
            {
                try
                {
                    fs = new FileStream(filePath + "_" + name + "_" + ".csv", FileMode.Append, FileAccess.Write);
                    sw = new StreamWriter(fs, Encoding.Default);
                    if (a == 0)
                    {
                        string ListName = "Time" + "," + "SingleEnded0" + "," + "SingleEnded1";
                        sw.WriteLine(ListName);
                        a++;
                    }
                    else
                        a++;
                    for (int k = 0; k < 3000; ++k)
                    {
                        InputFrame inFrame = frames[k];
                        string data = string.Empty;
                        double zeit = (DateTime.Now.ToUniversalTime().Ticks - 621355968000000000) / 100;
                        data = zeit + "," + inFrame.GetSingleEnded(0) + "," + inFrame.GetSingleEnded(0) + a;
                        sw.WriteLine(data);
                    }
                    if (a >= 5)
                    {
                        Console.WriteLine(".csv File is saved");
                        a = 0;
                        d = DateTime.Now;
                        name = d.ToString("yyyy_MM_dd_HH_mm_ss_fff");
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


