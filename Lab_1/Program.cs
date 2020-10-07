using System;
using SixLabors.ImageSharp; // Из одноимённого пакета NuGet
using SixLabors.ImageSharp.PixelFormats;
using System.Linq;
using SixLabors.ImageSharp.Processing;
using Microsoft.ML.OnnxRuntime;
using System.Collections.Generic;
using System.Numerics.Tensors;
using System.IO;
using System.Threading;

using DigitRecognition;
using Lab_1;

namespace OnnxSample
{
    class Program
    {
        static ManualResetEvent workHandler = new ManualResetEvent(false);

        static MyOutput outp = new MyOutput();

        public static Recognizer recognizer = new Recognizer(workHandler, outp);

        static void Main(string[] args)
        {
            string dirName = args.FirstOrDefault() ?? @"C:\Users\Пользователь\Desktop\TestImages";

            Console.CancelKeyPress += new ConsoleCancelEventHandler(myHandler);
            
            recognizer.GetResults(dirName);
        }

        protected static void myHandler(object sender, ConsoleCancelEventArgs args)
        {
            Console.WriteLine("ctrl + c signal recieved!");
            recognizer.workHandler.Set();            
            args.Cancel = true;
        }
    }

    
}