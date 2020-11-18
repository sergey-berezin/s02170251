using DigitRecognition;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Windows;
using System.Windows.Forms;

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
            dataGenerator.recognizer.workHandler.Set();            
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
            using (AppContext context = new AppContext())
            {
                context.Pictures.RemoveRange(context.Pictures);
                context.BlobPictures.RemoveRange(context.BlobPictures);
                context.SaveChanges();
            }
        }

        private void buttonGetStatsClick(object sender, RoutedEventArgs e)
        {
            List<int> stats = new List<int>();
            using (AppContext context = new AppContext())
            {                
                for (int i = 0; i < 10; ++i)
                    stats.Add(0);

                foreach (var p in context.Pictures)
                {
                    stats[p.Label] += 1;
                }
            }
            listBox_Copy.ItemsSource = stats;
        }
    }
}
