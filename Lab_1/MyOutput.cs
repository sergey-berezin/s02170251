using DigitRecognition;
using System;
using System.Collections.Generic;
using System.Text;

namespace Lab_1
{
    class MyOutput : IResultOutput
    {
        public void SendResult(ResultStruct res)
        {
            Console.WriteLine(res.filename + ": ");
            for (int i = 0; i< 10; i++)
            {
                Console.WriteLine( i.ToString() + " with confidence " +res.confidence[i].ToString());
            }
            Console.WriteLine("");
        }
    }
}
