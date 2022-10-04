using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.IO.Ports;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;

namespace COM_Ports_Packages.Core
{
    internal class COM
    {
        private SerialPort _serialPort1;
        private SerialPort _serialPort2;

        private string _packageExample;
        public string PackageExample
        {
            get { return _packageExample; ; }
            private set { _packageExample = value; }
        }

        private string _packageFlag;
        public string PackageFlag
        {
            get { return _packageFlag; ; }
            private set { _packageFlag = value; }
        }

        private string _packageLength;
        public string PackageLength
        {
            get { return _packageLength; ; }
            private set { _packageLength = value; }
        }

        private string _stuffedData;
        public string StuffedData
        {
            get { return _stuffedData; }
            private set { _stuffedData = value; }
        }

        private string _receivedData;
        public string ReceivedData
        {
            get { return _receivedData; }
            private set { _receivedData = value; }
        }

        private void PackageReceivedEventHandler(object sender, SerialDataReceivedEventArgs e)
        {
            ReceivedData = StuffedData = _serialPort2.ReadExisting();
            ReceivedData = Stuffing.BitDestuffing(ReceivedData);

            var hash = SHA256.Create().ComputeHash(Encoding.UTF8.GetBytes(ReceivedData.Substring(0, 12)));
            if (BitConverter.ToString(hash).ToLower().Replace("-", String.Empty) != ReceivedData.Substring(12))
                throw new Exception("The hash didn't match.");
        }

        private void CheckPackageCorrectness(string package)
        {
            if (String.IsNullOrEmpty(package))
                throw new Exception("The package is empty.");

            if (!Regex.IsMatch(package, @"^[a-f0-9]+$"))
                throw new Exception("The package must contain only lowercase hex numbers.");

            if (package.Length % 4 != 0)
                throw new Exception("The package structure is incorrect.");

            if (package.Length == 8)
                throw new Exception("The package hasn't payload.");

            if (package.Substring(0, 2) != PackageFlag)
                throw new Exception("The package flag is invalid. Should be <" + PackageFlag + ">.");

            if (package.Substring(2, 2) != PackageLength)
                throw new Exception("The package length is invalid. Should be <" + PackageLength + ">.");
        }


        public COM(string firstPortName, string secondPortName)
        {
            PackageFlag = "0b";
            PackageLength = "02";
            PackageExample = "eb0a020d00ff";

            _serialPort1 = new SerialPort(firstPortName, 9600, Parity.None, 8, StopBits.One);
            _serialPort2 = new SerialPort(secondPortName, 9600, Parity.None, 8, StopBits.One);

            _serialPort1.WriteTimeout = 10;
            _serialPort2.ReadTimeout = 10;

            _serialPort1.Encoding = Encoding.Unicode;
            _serialPort2.Encoding = Encoding.Unicode;

            _serialPort2.DataReceived += new SerialDataReceivedEventHandler(PackageReceivedEventHandler);
            ReceivedData = StuffedData = String.Empty;
        }

        public void OpenPorts()
        {
            try
            {
                _serialPort1.Open();
                _serialPort2.Open();
            }
            catch (IOException ex)
            {
                throw new Exception("The ports do not exist.");
            }
        }
        public void ClosePorts()
        {
            if (_serialPort1.IsOpen == true && _serialPort2.IsOpen == true)
            {
                _serialPort1.Close();
                _serialPort2.Close();
            }
            else
            {
                throw new Exception("The ports are not open yet.");
            }
        }

        public void SendPackage(string package)
        {
            if (_serialPort1.IsOpen == false || _serialPort2.IsOpen == false)
                throw new Exception("The ports are not open yet.");

            try
            {
                CheckPackageCorrectness(package);
            } 
            catch (Exception ex)
            {
                throw new Exception(ex.Message + "\nInput example: " + PackageExample);
            }

            var packageHash = SHA256.Create().ComputeHash(Encoding.UTF8.GetBytes(package));
            package += BitConverter.ToString(packageHash).ToLower().Replace("-", String.Empty);
            package = Stuffing.BitStuffing(package);

            _serialPort1.Write(package);
            Thread.Sleep(50);
        }
    }
}
