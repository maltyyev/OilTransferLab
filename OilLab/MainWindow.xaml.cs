using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace OilLab
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        #region глобальные переменные

        public static double Pressure;
        public static double Temperature;

        public static bool StationWorks = false;
        public static bool TransferStatus = true;
        public static bool AdditionalPipeUsed = false;
        public static bool AssOnFire = false;

        public static int OilTransferred = 0;
        public static int Time = 50;

        public static string StationStatusString = "в норме";
        public static string AdditionalPipe = "запасной трубопровод не задействован";

        public delegate void LabelContentSetter();

        public delegate void ProgressBarSetter();

        #endregion

        private LabelContentSetter _labelContentSetter;

        private ProgressBarSetter _oilTransferSetter;
        private ProgressBarSetter _fireSetter;

        public MainWindow()
        {
            InitializeComponent();

            _labelContentSetter = new LabelContentSetter(UpdateLabels);

            _oilTransferSetter = new ProgressBarSetter(UpdateTransferBar);
            _fireSetter = new ProgressBarSetter(UpdateFireBar);

            OilAmount.Content = String.Format("Перекачано нефти: {0} баррелей", OilTransferred);
            StationStatus.Content = String.Format("Состояние станции: {0}, {1}", StationStatusString, AdditionalPipe);

            System.Threading.Thread timerThread = new System.Threading.Thread(Timer);
            timerThread.IsBackground = true;
            timerThread.Start();
        }

        public void UpdateTransferBar()
        {
            if (TransferProgressBar.Value == 100)
            {
                TransferProgressBar.Value = 0;
                OilTransferred++;
            }
            else if (TransferProgressBar.Value < 99)
                TransferProgressBar.Value += 1;
            else if (TransferProgressBar.Value >= 99)
                TransferProgressBar.Value = 100;
        }

        public void Transfer()
        {
            if (TransferStatus)
                Dispatcher.Invoke(_oilTransferSetter);
        }

        public void UpdateFireBar()
        {
            if (FireBar.Value == 100)
            {
                FireBar.Value = 0;
                AssOnFire = false;
                TransferStatus = true;
                FireStatusLabel.Content = "Возгораний не замечено";
            }
            else if (FireBar.Value < 99)
                FireBar.Value += 1;
            else if (FireBar.Value >= 99)
                FireBar.Value = 100;
        }

        public void Fire()
        {
            if (AssOnFire)
            {
                TransferStatus = false;
                Dispatcher.Invoke(_fireSetter);
            }
        }

        public void UpdateLabels()
        {
            OilAmount.Content = String.Format("Перекачано нефти: {0} баррелей", OilTransferred);
            StationStatus.Content = String.Format("Состояние станции: {0}, {1}", StationStatusString, AdditionalPipe);
        }

        public void Timer()
        {
            while (true)
                if (StationWorks)
                {
                    System.Threading.Thread.Sleep(Time);
                    Transfer();
                    Fire();

                    Dispatcher.Invoke(_labelContentSetter);
                }
        }

        private void OnStart_Click(object sender, RoutedEventArgs e)
        {
            if (StationWorks)
            {
                StationWorks = false;
                OnStart.Content = "Старт";
            }
            else if (!StationWorks)
            {
                StationWorks = true;
                OnStart.Content = "Стоп";
            }
        }

        private void OilTransferButton_Click(object sender, RoutedEventArgs e)
        {
            if (TransferStatus)
            {
                TransferStatus = false;
                TransferStatusLabel.Content = "Перекачка не идёт";
            }
            else if (!TransferStatus)
            {
                TransferStatus = true;
                TransferStatusLabel.Content = "Идёт перекачка нефти";
            }
        }

        private void MakeFireButton_Click(object sender, RoutedEventArgs e)
        {
            AssOnFire = true;
            FireStatusLabel.Content = "ПОЖАР!";
        }
    }
}
