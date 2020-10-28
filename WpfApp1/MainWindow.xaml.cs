using DigitRecognition;
using System.Diagnostics;
using System.Windows;
using System.Windows.Forms;

namespace WpfApp1
{
    public partial class MainWindow : Window
    {
        public DataGenerator dataGenerator = new DataGenerator();
        //public delegate void NextPrimeDelegate();

        public MainWindow()
        {
            InitializeComponent();

            DataContext = dataGenerator;

            //listBox.ItemsSource = dataGenerator.DigitCount;

            listView.Items.Clear();
            listView.ItemsSource = dataGenerator.Result[0];

            dataGenerator.directory = @"C:\Users\Пользователь\Desktop\TestImages";
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
            //dataGenerator.resultList[0].Add(new UserInterfaceStruct());
        }

        private void buttonShowClick(object sender, RoutedEventArgs e)
        {
            if (listBox.SelectedIndex != -1)
            {
                //listView.Items.Clear();
                listView.ItemsSource = dataGenerator.Result[listBox.SelectedIndex];
            }            
        }
    }
}
