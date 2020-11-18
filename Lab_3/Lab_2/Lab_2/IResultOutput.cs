using System;
using System.Collections.Generic;
using System.Text;

namespace DigitRecognition
{
    public interface IResultOutput
    {
        void SendResult(ResultStruct res);
    }
}
