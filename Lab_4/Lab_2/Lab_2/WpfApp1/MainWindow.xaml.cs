 using DigitRecognition;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Net.Http;
using System.Windows;
using System.Windows.Forms;
using System.Windows.Threading;

namespace WpfApp1
{
    public partial class MainWindow : Window
    {
        public DataGenerator dataGenerator = new DataGenerator();
        List<int> stats  = new List<int>();

        public MainWindow()
        {
            InitializeComponent();

            DataContext = dataGenerator;

            listView.Items.Clear();
            listView.ItemsSource = dataGenerator.Result[0];

            dataGenerator.directory = @"C:\Users\Пользователь\Desktop\SmallTestImages";
        }

        private void buttonBrowseClick(object sender, RoutedEventArgs e)
        {
            FolderBrowserDialog browser = new FolderBrowserDialog();
            
            var res = browser.ShowDialog();

            if (res == System.Windows.Forms.DialogResult.OK)
            {
                dataGenerator.directory = browser.SelectedPath;
            }
        }

        private void buttonRunClick(object sender, RoutedEventArgs e)
        {
            for (int i =0; i<10; ++i)
            {
                dataGenerator.resultList[i].Clear();
            }
            dataGenerator.startWork();
        }

        private void buttonStopClick(object sender, RoutedEventArgs e)
        {
            //dataGenerator.recognizer.workHandler.Set();            
        }

        private void buttonShowClick(object sender, RoutedEventArgs e)
        {
            if (listBox.SelectedIndex != -1)
            {
                listView.ItemsSource = dataGenerator.Result[listBox.SelectedIndex];
            }            
        }

        private void buttonClearClick(object sender, RoutedEventArgs e)
        {
            var client = new HttpClient();
            client.DeleteAsync("http://localhost:5000/api/database/");  
        }

        private async System.Threading.Tasks.Task buttonGetStatsClickAsync(object sender, RoutedEventArgs e, Dispatcher dispatcher)
        {
            var client = new HttpClient();
            var response = await client.GetStringAsync("http://localhost:5000/api/database");

            List<int> stats = JsonConvert.DeserializeObject<List<int>>(response);

            dispatcher.BeginInvoke(() => listBox_Copy.ItemsSource = stats);            
        }

        private void buttonGetStatsClick(object sender, RoutedEventArgs e)
        {
            buttonGetStatsClickAsync(sender, e, Dispatcher.CurrentDispatcher);
        }

    }
}
