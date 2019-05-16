using System;
using System.Collections.Generic;
using System.IO;
using CeboMsrNet;
using System.Data;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using System.Runtime.InteropServices;

public static class QueryPerformanceMethd
{
    [DllImport("kernel32.dll")]
    public extern static short QueryPerformanceCounter(ref long x);


    [DllImport("kernel32.dll")]
    public extern static short QueryPerformanceFrequency(ref long x);
}

static void Wang()
{
    long stop_Value = 0;
    long start_Value = 0;
    long freq = 0;

    QueryPerformanceMethd.QueryPerformanceFrequency(ref freq);
    QueryPerformanceMethd.QueryPerformanceCounter(ref start_Value);
    //Fun()    需要计时方法          
    QueryPerformanceMethd.QueryPerformanceCounter(ref stop_Value);
    double time = (double)(stop_Value - start_Value) / (double)(freq);

    Console.WriteLine(time);//单位S
    Console.ReadLine();
}
