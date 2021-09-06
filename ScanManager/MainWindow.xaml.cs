using FluentModbus;
using System;
using System.ComponentModel;
using System.Net;
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

        bool m_IsReady;

        public bool IsReady
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

        bool m_IsScanning;

        public bool IsScanning
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

        bool m_IsEjecting;

        public bool IsEjecting
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

        readonly ModbusTcpClient m_ModbusClient = new ModbusTcpClient();

        private void Connect(object sender, RoutedEventArgs e)
        {
            try
            {
                m_ModbusClient.Connect(IPAddress.Parse(Properties.Settings.Default.Host));
                m_Timer.Start();
                Timer_Tick(m_Timer, EventArgs.Empty);
                IsConnected = true;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, Title, MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void Disconnect(object sender, RoutedEventArgs e)
        {
            m_ModbusClient.Disconnect();
            m_Timer.Stop();
            IsConnected = false;
            IsReady = false;
            IsScanning = false;
            IsEjecting = false;
            Resolution = null;
        }

        private void Scan(object sender, RoutedEventArgs e)
        {
            try
            {
                m_ModbusClient.WriteSingleCoil(0, 0, true);
            }
            catch
            {
            }
        }

        private void Abort(object sender, RoutedEventArgs e)
        {
            try
            {
                m_ModbusClient.WriteSingleCoil(0, 0, false);
            }
            catch
            {
            }
        }

        private void Eject(object sender, RoutedEventArgs e)
        {
            try
            {
                m_ModbusClient.WriteSingleCoil(0, 1, true);
            }
            catch
            {
            }
        }

        private void SetResolution(object sender, RoutedEventArgs e)
        {
            try
            {
                if (ushort.TryParse(ResolutionText.Text, out ushort value))
                {
                    m_ModbusClient.WriteSingleRegister(0, 0, value);
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
                var discreteInputs = m_ModbusClient.ReadDiscreteInputs(0, 0, 1);
                IsReady = discreteInputs.Get(0);
                var coils = m_ModbusClient.ReadCoils(0, 0, 2);
                IsScanning = coils.Get(0);
                IsEjecting = coils.Get(1);
                var registers = m_ModbusClient.ReadHoldingRegisters<ushort>(0, 0, 1);
                Resolution = registers[0];
            }
            catch
            {
                Disconnect(this, new RoutedEventArgs());
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
    }
}
