using System;
using System.ComponentModel;
using System.IO.Ports;
using System.Linq;
using System.Windows;
using System.Windows.Threading;

namespace ScanManager
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window, INotifyPropertyChanged
    {
        readonly DispatcherTimer m_Timer = new DispatcherTimer();

        public MainWindow()
        {
            InitializeComponent();

            m_Timer.Interval = TimeSpan.FromSeconds(0.2);
            m_Timer.Tick += Timer_Tick;
        }

        private void Window_Closed(object sender, EventArgs e)
        {
            Properties.Settings.Default.Save();
        }

        public string[] AllSerialPorts => SerialPort.GetPortNames().OrderBy(portName => portName).ToArray();

        bool m_IsConnected;

        public bool IsConnected
        {
            get => m_IsConnected;
            set
            {
                if (m_IsConnected != value)
                {
                    m_IsConnected = value;
                    PropertyChanged.Invoke(this, new PropertyChangedEventArgs(nameof(IsConnected)));
                    PropertyChanged.Invoke(this, new PropertyChangedEventArgs(nameof(IsDisconnected)));
                }
            }
        }

        public bool IsDisconnected => !IsConnected;

        int? m_IsReady;

        public int? IsReady
        {
            get => m_IsReady;
            set
            {
                if (m_IsReady != value)
                {
                    m_IsReady = value;
                    PropertyChanged.Invoke(this, new PropertyChangedEventArgs(nameof(IsReady)));
                }
            }
        }

        int? m_IsScanning;

        public int? IsScanning
        {
            get => m_IsScanning;
            set
            {
                if (m_IsScanning != value)
                {
                    m_IsScanning = value;
                    PropertyChanged.Invoke(this, new PropertyChangedEventArgs(nameof(IsScanning)));
                }
            }
        }

        int? m_IsEjecting;

        public int? IsEjecting
        {
            get => m_IsEjecting;
            set
            {
                if (m_IsEjecting != value)
                {
                    m_IsEjecting = value;
                    PropertyChanged.Invoke(this, new PropertyChangedEventArgs(nameof(IsEjecting)));
                }
            }
        }

        ushort? m_Resolution;

        public ushort? Resolution
        {
            get => m_Resolution;
            set
            {
                if (m_Resolution != value)
                {
                    m_Resolution = value;
                    PropertyChanged.Invoke(this, new PropertyChangedEventArgs(nameof(Resolution)));
                }
            }
        }

        readonly SerialPort m_Port = new SerialPort()
        {
            ReadTimeout = 1000,
            WriteTimeout = 1000
        };

        private void Connect(object sender, RoutedEventArgs e)
        {
            try
            {
                m_Port.PortName = Properties.Settings.Default.SerialPort;
                m_Port.Open();
                m_Timer.Start();
                IsConnected = true;
                Timer_Tick(m_Timer, EventArgs.Empty);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, Title, MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void Disconnect(object sender, RoutedEventArgs e)
        {
            m_Port.Close();
            m_Timer.Stop();
            IsConnected = false;
            IsReady = null;
            IsScanning = null;
            IsEjecting = null;
            Resolution = null;
        }

        private void Scan(object sender, RoutedEventArgs e)
        {
            try
            {
                m_Port.WriteValue("SC", "1");
            }
            catch
            {
            }
        }

        private void Abort(object sender, RoutedEventArgs e)
        {
            try
            {
                m_Port.WriteValue("SC", "0");
            }
            catch
            {
            }
        }

        private void Eject(object sender, RoutedEventArgs e)
        {
            try
            {
                m_Port.WriteValue("EJ", "1");
            }
            catch
            {
            }
        }

        private void Restart(object sender, RoutedEventArgs e)
        {
            try
            {
                m_Port.WriteCommand("RST");
            }
            catch
            {
            }
        }

        private void SetResolution(object sender, RoutedEventArgs e)
        {
            try
            {
                if (ushort.TryParse(ResolutionText.Text, out var value))
                {
                    m_Port.WriteValue("RS", value.ToString());
                }
            }
            catch
            {
            }
        }

        private void Timer_Tick(object sender, EventArgs e)
        {
            try
            {
                IsReady = int.Parse(m_Port.ReadValue("RD"));
                IsScanning = int.Parse(m_Port.ReadValue("SC"));
                IsEjecting = int.Parse(m_Port.ReadValue("EJ"));
                Resolution = ushort.Parse(m_Port.ReadValue("RS"));
            }
            catch
            {
                Disconnect(this, new RoutedEventArgs());
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
    }
}
