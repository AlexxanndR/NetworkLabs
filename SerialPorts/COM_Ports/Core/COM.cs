using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace COM_Ports.Core
{
    internal class COM
    {
        private SerialPort _serialPort1;
        private SerialPort _serialPort2;

        private string _receivedData;

        public string ReceivedData
        {
            get { return _receivedData; }
            private set { _receivedData = value; }
        }

        public COM(string firstPortName, string secondPortName)
        {
            _serialPort1 = new SerialPort(firstPortName, 9600, Parity.None, 8, StopBits.One);
            _serialPort2 = new SerialPort(secondPortName, 9600, Parity.None, 8, StopBits.One);
            _serialPort1.Encoding = Encoding.Unicode;
            _serialPort2.Encoding = Encoding.Unicode;
            _serialPort2.DataReceived += new SerialDataReceivedEventHandler(DataReceivedEventHandler);
            _receivedData = String.Empty;
        }

        public void OpenPorts()
        {
            try
            {
                _serialPort1.Open();
                _serialPort2.Open();
            }
            catch (Exception ex)
            {
                throw new Exception("The ports do not exist.");
            }
        }

        public void SendData(string data)
        {
            _serialPort1.Write(data);
            Thread.Sleep(50);
        }

        private void DataReceivedEventHandler(object sender, SerialDataReceivedEventArgs e)
        {            
            ReceivedData = _serialPort2.ReadExisting();
        }

        public void ClosePorts()
        {
            if (_serialPort1.IsOpen == true && _serialPort2.IsOpen == true)
            {
                _serialPort1.Close();
                _serialPort2.Close();
            } else
            {
                throw new Exception("Ports are not open yet.");
            }
        }
    }
}
