using System;
using System.IO.Ports;

namespace ScanManager
{
    static class SerialPortExtension
    {

        public static void WriteValue(this SerialPort port, string name, string value)
        {
            port.WriteLine($"{name}={value}");
            var response = port.ReadLine();
            if (response != "OK")
            {
                throw new InvalidOperationException("Invalid response");
            }
        }

        public static string ReadValue(this SerialPort port, string name)
        {
            port.WriteLine($"{name}?");
            var response = port.ReadLine();
            if (!response.StartsWith($"{name}="))
            {
                throw new InvalidOperationException("Invalid response");
            }
            return response.Substring(name.Length + 1);
        }
    }
}
