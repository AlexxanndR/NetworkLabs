using System;
using System.IO;
using System.IO.Ports;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;

namespace COM_Ports_Collisions.Core
{
    internal class COM
    {
        private SerialPort _serialPort1;
        private SerialPort _serialPort2;

        private const int _collisionWindow = 100;
        private const int _normalAttemptsNum = 10;
        public string PackageExample { get; private set; }
        public string PackageFlag { get; private set; }
        public string PackageLength { get; private set; }
        public string ReceivedData { get; private set; }

        private void PackageReceivedEventHandler(object sender, SerialDataReceivedEventArgs e)
        {
            ReceivedData = _serialPort2.ReadExisting();
        }

        private void CheckPackageCorrectness(string package)
        {
            if (String.IsNullOrEmpty(package))
                throw new Exception("The package is empty.");

            if (!Regex.IsMatch(package, @"^[a-f0-9]+$"))
                throw new Exception("The package must contain only lowercase hex numbers.");

            if (package.Length != 12)
                throw new Exception("The package structure is incorrect.");

            if (package.Substring(0, 2) != PackageFlag)
                throw new Exception("The package flag is invalid. Should be <" + PackageFlag + ">.");

            if (package.Substring(2, 2) != PackageLength)
                throw new Exception("The package length is invalid. Should be <" + PackageLength + ">.");
        }

        private bool CheckBusy(uint busyProbability)
        {
            Random gen = new Random();
            return gen.Next(100) <= busyProbability ? true : false;
        }

        private bool CheckCollision(uint collisionProbability)
        {
            Random gen = new Random();
            return gen.Next(100) <= collisionProbability ? true : false;
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
            ReceivedData = String.Empty;
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

        public string SendPackage(string package, uint collisionProb, uint busyProb)
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

            uint attemptNum = 0;
            StringBuilder report = new StringBuilder("Report:\n");

            while (true)
            {
                if (CheckBusy(busyProb))
                    report.AppendLine("The monochannel is busy.");

                _serialPort1.Write(package);

                Thread.Sleep(_collisionWindow);

                if (CheckCollision(collisionProb))
                {
                    attemptNum++;
                    report.AppendLine("Collision " + attemptNum + " was detected.");
                    if (attemptNum > _normalAttemptsNum)
                    {
                        report.AppendLine("Too many attempts to send a package.");
                        throw new Exception(report.ToString());
                    }
                }
                else
                    break;

                Random latency = new Random();
                Thread.Sleep(latency.Next(100));
            }

            return report.ToString();
        }
    }
}
