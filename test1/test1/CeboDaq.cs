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
 * 
 * Data in the .csv File export.
 */
namespace CeboDaq
{
    class CeboDaq
    {
        public int digitalPort0_Value;
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
                device.DigitalPorts[0],
            };
            // Prepare device with this collection ...
            device.SetupInputFrame(inputs);
            Queue<double> voltageData = new Queue<double>();
            double periodDuration = 9.9;
            double startTime = (DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1))).TotalMilliseconds;
            while (true)
            {
                if (digitalPort0_Value == 0)
                {
                    if(((DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1))).TotalMilliseconds-startTime)>periodDuration)
                    {
                        startTime = (DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1))).TotalMilliseconds;
                        InputFrame inputFrame = device.ReadFrame();
                        double readFramePeriod = (DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1))).TotalMilliseconds-startTime;
                        voltageData.Enqueue(startTime);
                        voltageData.Enqueue(readFramePeriod);
                        voltageData.Enqueue(inputFrame.GetSingleEnded(0));
                        voltageData.Enqueue(inputFrame.GetSingleEnded(1));
                        digitalPort0_Value = inputFrame.GetDigitalPort(0);

                        if (voltageData.Count>=1000)
                        {                
                            MeasureToCsv.DataToCsv(voltageData);
                            voltageData = new Queue<double>();                            
                        }                              
                    }                    
                }
                else
                {
                    Console.WriteLine("Dataaqu is stopped");
                    Thread.Sleep(1000);
                    DigitalPort dp0 = device.DigitalPorts[0];
                    digitalPort0_Value = dp0.Value;
                    
                }
            }
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
                    //StartAndStopControl(device);
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
        /// </summary>
        /// <param name="Queue<double> qu"></param>
        /// <returns></returns>
        public async static Task DataToCsv(Queue<double> qu)
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
                        string ListName = "startTime" + ","+"readFramePeriod"+"," + "SingleEnded0" + "," + "SingleEnded1";
                        sw.WriteLine(ListName);
                        a++;
                    }
                    else
                        a++;
                    int rows= qu.Count/4;
                    for (int i = 1; i <= rows; i++)
                    {
                        string data = string.Empty;
                        data = qu.Dequeue() + "," + qu.Dequeue() + ","+ qu.Dequeue() + "," + qu.Dequeue();
                        sw.WriteLine(data);
                    }
                    if (a >= 5)
                    {
                        if (b >= 10)
                        {
                            b = 0;
                        }
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


