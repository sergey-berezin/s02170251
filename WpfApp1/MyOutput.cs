using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
        }
    }
}
