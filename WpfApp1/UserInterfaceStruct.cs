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

        public UserInterfaceStruct (ResultStruct rs)
        {
            filename = Path.GetFileName(rs.filename);
            res_digit = rs.res_digit;

            img = new BitmapImage(new Uri(rs.filename, UriKind.Absolute));
            /*
            PixelFormat pf = PixelFormats.Rgb24;
            int width = rs.img.Width;
            int height = rs.img.Height;
            int rawStride = (width * pf.BitsPerPixel + 7) / 8;

            byte[] rawImage = new byte[rawStride * height];
            for (int i =0; i<height; ++i)
            {
                Span<Rgb24>  pixelSpan = rs.img.GetPixelRowSpan(i);
                for (int j = 0; j < width; ++j)
                {
                    rawImage[i*rawStride + j] = (byte)(pixelSpan[j].R + pixelSpan[j].G + pixelSpan[j].B); 
                }                   
            }

            img = BitmapSource.Create(
                width,
                height,
                rs.img.Metadata.HorizontalResolution,
                rs.img.Metadata.VerticalResolution,
                PixelFormats.Rgb24,
                null,
                rawImage, rawStride);
          */
        }
    }
}
