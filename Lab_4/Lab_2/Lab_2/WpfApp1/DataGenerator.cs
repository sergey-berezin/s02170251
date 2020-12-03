using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;
using System.Windows.Threading;
using DigitRecognition;
using Newtonsoft.Json;

namespace WpfApp1
{
    public class DataGenerator : INotifyPropertyChanged
    {
        public ObservableCollection<UserInterfaceStruct>[] resultList = new ObservableCollection<UserInterfaceStruct>[10];       
        public string directory { get; set; }
        public List<Tuple<byte[], string>> unrecognized_pics;
       

        public event PropertyChangedEventHandler PropertyChanged;


        public DataGenerator(string s = @"C:\Users\Пользователь\Desktop\SmallTestImages")
        {
            unrecognized_pics = new List<Tuple<byte[], string>>();

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

        private async Task MakeRecognitionAsync(Tuple<byte[], string> tpl, Dispatcher dispatcher)
        {
            var client = new HttpClient();

            RequestStruct request_body = new RequestStruct(tpl.Item2);
            var request = new StringContent(JsonConvert.SerializeObject(request_body));
            request.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("application/json");
            string addres = "http://localhost:5000/api/recognition/";

            //Debug.WriteLine(request_body.filename);
            var response = await client.PutAsync(addres, request);            
            string body = await response.Content.ReadAsStringAsync();
            //Debug.WriteLine("BODY: " +body);

            ImageInfo info = JsonConvert.DeserializeObject<ImageInfo>(body);
            //UserInterfaceStruct result = new UserInterfaceStruct(tpl.Item2, info.res_digit);
            dispatcher.BeginInvoke( () => {
                UserInterfaceStruct result = new UserInterfaceStruct(tpl.Item2, info.res_digit);
                resultList[result.res_digit].Add(result); 
            });            
        }

        void thread_func(object obj)
        {
            Dispatcher dispatcher = (Dispatcher)obj;
            foreach (var tpl in unrecognized_pics )
            {
                MakeRecognitionAsync(tpl, dispatcher);
            }
        }

        public void startWork()
        {
            DirectoryInfo directoryInfo = new DirectoryInfo(directory);
            unrecognized_pics = new List<Tuple<byte[], string>>();
            foreach (var file in directoryInfo.GetFiles()) //проходим по файлам
            {
                unrecognized_pics.Add(new Tuple<byte[], string>(File.ReadAllBytes(file.FullName), file.FullName));
            }

            Thread asynch_thread = new Thread(new ParameterizedThreadStart(thread_func));
            asynch_thread.Start(Dispatcher.CurrentDispatcher);
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
