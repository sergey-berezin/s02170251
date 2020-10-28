using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Threading;
using DigitRecognition;

namespace WpfApp1
{
    public class DataGenerator : INotifyPropertyChanged
    {
        public ObservableCollection<UserInterfaceStruct>[] resultList = new ObservableCollection<UserInterfaceStruct>[10];       
        public MyOutput outp;
        public Recognizer recognizer;
        public string directory { get; set; }


        public event PropertyChangedEventHandler PropertyChanged;


        public DataGenerator(string s ="")
        {
            outp = new MyOutput(resultList, Dispatcher.CurrentDispatcher);
            recognizer = new Recognizer(outp);

            directory = s;

            for (int i =0; i<10; ++i)
            {
                resultList[i] = new ObservableCollection<UserInterfaceStruct>();
                resultList[i].CollectionChanged += myCollectionChanged;
            }            
        }

        public ObservableCollection<UserInterfaceStruct>[]  Result
        {
            get
            {
                return resultList;
            }
        }

        public List<Tuple<string, int>> DigitCount
        {
            get
            {
                var res = new List<Tuple<string, int>>();

                for(int i = 0; i<10; ++i)
                {
                    res.Add(new Tuple<string, int>(i.ToString()+" ", resultList[i].Count));
                }

                return res;
            }
        }

        
        void thread_func()
        {
            recognizer.GetResults(directory);
        }
 
        public void startWork()
        {
            Thread asynch_thread = new Thread(thread_func);
            asynch_thread.Start();
        }
        

        private void NotifyPropertyChanged([CallerMemberName] string propertyName = "")
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        //Каждый раз когда изменяется коллекция
        private void myCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            NotifyPropertyChanged("Result");
            NotifyPropertyChanged("DigitCount");
        }
    }
}
