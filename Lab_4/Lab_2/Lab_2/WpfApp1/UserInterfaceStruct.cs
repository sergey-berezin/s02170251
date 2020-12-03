using DigitRecognition;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace WpfApp1
{
    public class UserInterfaceStruct
    {
        public string filename { get; set; }
        public BitmapSource img { get; set; }
        public int res_digit { get; set; }
        //public byte[] Bytes { get; set; }

        public UserInterfaceStruct (ResultStruct rs)
        {
            filename = Path.GetFileName(rs.filename);
            res_digit = rs.res_digit;

            img = new BitmapImage(new Uri(rs.filename, UriKind.Absolute));
            //Bytes = File.ReadAllBytes(filename);
            
            
            //PixelFormat pf = PixelFormats.Rgb24;
            //int width = rs.img.Width;
            //int height = rs.img.Height;
            //int rawStride = width * 3;

            //byte[] rawImage = new byte[rawStride * height];
            //for (int i =0; i<height; ++i)
            //{
            //    Span<Rgb24>  pixelSpan = rs.img.GetPixelRowSpan(i);
            //    for (int j = 0; j < width; j++)
            //    {
            //        rawImage[i * rawStride + j*3] = pixelSpan[j].R ;  
            //        rawImage[i * rawStride + j*3 + 1] = pixelSpan[j].G;
            //        rawImage[i * rawStride + j*3 + 2] = pixelSpan[j].B;
            //    }                   
            //}

            //img = BitmapSource.Create(
            //    width,
            //    height,
            //    96,
            //    96,
            //    PixelFormats.Rgb24,
            //    null,
            //    rawImage, rawStride);          
        }

        public UserInterfaceStruct(string s, int i)
        {
            filename = Path.GetFileName(s);
            res_digit = i;
            img = new BitmapImage(new Uri(s, UriKind.Absolute));
        }
    }
}
