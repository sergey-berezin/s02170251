using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using System;
using System.Collections.Generic;
using System.Text;

namespace DigitRecognition
{
    public class ResultStruct
    {
        public string filename { get; set; }    
        public int res_digit { get; set; }
        public byte[] picture { get; set; }
    }
}
