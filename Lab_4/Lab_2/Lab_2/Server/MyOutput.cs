using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using DigitRecognition;

namespace Server
{
    public class MyOutput : IResultOutput
    {
        public void SendResult(ResultStruct res)
        {
            using (AppContext context = new AppContext())
            {
                var pic = new Picture() { Filename = res.filename, Label = res.res_digit };
                var blob = new BlobPicture() { Pixels = res.picture };

                pic.Blob = blob;
                blob.Picture = pic;
                
                context.Pictures.Add(pic);
                context.BlobPictures.Add(blob);
                context.SaveChanges();
            }
        }
    }
}
