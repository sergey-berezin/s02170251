using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Threading;
using DigitRecognition;

namespace WpfApp1
{
    public class MyOutput : IResultOutput
    {
        ObservableCollection<UserInterfaceStruct>[] lst;

        Dispatcher dispatcher;

        public MyOutput(ObservableCollection<UserInterfaceStruct>[] l, Dispatcher d)
        {
            lst = l;
            dispatcher = d;
        }

        public void SendResult(ResultStruct res)
        {
            dispatcher.BeginInvoke( () => lst[res.res_digit].Add(new UserInterfaceStruct(res)));

            using (AppContext context = new AppContext())
            {
                var pic = new Picture() { Filename = res.filename, Label = res.res_digit };
                var blob = new BlobPicture() { Pixels = File.ReadAllBytes(res.filename) };

                pic.Blob = blob;
                blob.Picture = pic;

                context.Pictures.Add(pic);
                context.BlobPictures.Add(blob);
                context.SaveChanges();
            }
        }
    }
}
