using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;
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
        public List<string> unrecognized_pics;
       

        public event PropertyChangedEventHandler PropertyChanged;


        public DataGenerator(string s = @"C:\Users\Пользователь\Desktop\SmallTestImages")
        {
            outp = new MyOutput(resultList, Dispatcher.CurrentDispatcher);
            recognizer = new Recognizer(outp);
            unrecognized_pics = new List<string>();

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
            if (unrecognized_pics.Count >0)
                recognizer.GetResults(unrecognized_pics);
        }
 
        public void startWork()
        {
            DirectoryInfo directoryInfo = new DirectoryInfo(directory);
            unrecognized_pics = new List<string>();
            foreach (var file in directoryInfo.GetFiles()) //проходим по файлам
            {
                using (AppContext context = new AppContext())
                {
                    bool found = false;
                    foreach (var p in context.Pictures)
                    {
                        if (p.Filename == file.FullName)
                        {
                            var new_blob = File.ReadAllBytes(file.FullName);
                            if (  ((IStructuralEquatable)new_blob).Equals((IStructuralEquatable)p.Blob.Pixels) )
                            {
                                resultList[p.Label].Add(new UserInterfaceStruct(p.Filename, p.Label));
                                found = true;
                                break;
                            }
                        }                                              
                    }
                    if (!found)
                        unrecognized_pics.Add(file.FullName);
                }
            }

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
