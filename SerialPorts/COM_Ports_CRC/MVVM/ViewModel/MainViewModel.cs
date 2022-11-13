using COM_Ports_CRC.Core;
using COM_Ports_CRC.Helpers;
using System;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;

namespace COM_Ports_CRC.MVVM.ViewModel
{
    internal class MainViewModel : ObservableObject
    {
        private COM _serialPorts;

        private string _hexSendMessage;

        public string HexSendMessage
        {
            get { return _hexSendMessage; }
            set
            {
                _hexSendMessage = value;
                OnPropertyChanged();
            }
        }

        private string _binSendMessage;

        public string BinSendMessage
        {
            get { return _binSendMessage; }
            set
            {
                _binSendMessage = value;
                OnPropertyChanged();
            }
        }

        private string _errorSendMessage;

        public string ErrorSendMessage
        {
            get { return _errorSendMessage; }
            set
            {
                _errorSendMessage = value;
                OnPropertyChanged();
            }
        }

        private string _CRC;

        public string CRC
        {
            get { return _CRC; }
            set
            {
                _CRC = value;
                OnPropertyChanged();
            }
        }

        private int _errorsNum;

        public int ErrorsNum
        {
            get { return _errorsNum; }
            set
            {
                _errorsNum = value;
                OnPropertyChanged();
            }
        }

        private string _hexReceivedMessage;

        public string HexReceivedMessage
        {
            get { return _hexReceivedMessage; }
            set
            {
                _hexReceivedMessage = value;
                OnPropertyChanged();
            }
        }

        private string _binReceivedMessage;

        public string BinReceivedMessage
        {
            get { return _binReceivedMessage; }
            set
            {
                _binReceivedMessage = value;
                OnPropertyChanged();
            }
        }

        private StringBuilder _logs;
        public StringBuilder Logs 
        {
            get { return _logs; }
            set
            {
                _logs = value;
                OnPropertyChanged();
            }
        }

        public RelayCommand HexToBinCommand
        {
            get
            {
                return new RelayCommand(lostFocus =>
                {
                    if (!String.IsNullOrEmpty(HexSendMessage) && Regex.IsMatch(HexSendMessage, @"^[a-f0-9]+$"))
                        BinSendMessage = HexSendMessage.HexToBin().GetHumanReadableBin();
                });
            }
        }

        public RelayCommand GenerateErrorCommand
        {
            get
            {
                return new RelayCommand(click =>
                {
                    try
                    {
                        if (String.IsNullOrEmpty(HexSendMessage))
                            throw new Exception("The package has not been sent yet.");

                        if (ErrorsNum <= Convert.ToInt32(_serialPorts.PackageLength, 16) * 8)
                        {
                            Random rd = new Random();
                            string binSendMessage = HexSendMessage.HexToBin();

                            for (int i = 0; i < ErrorsNum; i++)
                            {
                                int errorPos = rd.Next(32, 47);
                                binSendMessage = new string (binSendMessage.Select((c, j) => j == errorPos ? (c == '0' ? '1' : '0') : c).ToArray());
                                ErrorSendMessage = binSendMessage.BinToHex();
                            }    
                        } 
                        else
                            throw new Exception("Too many errors. Should be no more than 16.");

                        Logs = Logs.AppendLine(ErrorsNum + " errors were generated.");
                    }
                    catch (Exception ex)
                    {
                        Logs = Logs.AppendLine(ex.Message);
                    }
                });
            }
        }

        public RelayCommand SendButtonCommand
        {
            get
            {
                return new RelayCommand(click =>
                {
                    try
                    {

                        try
                        {
                            _serialPorts.CheckPackageCorrectness(HexSendMessage);
                        }
                        catch (Exception ex)
                        {
                            throw new Exception(ex.Message + "\nInput example: " + _serialPorts.PackageExample);
                        }

                        CRC = CheckSum.CRC16(HexSendMessage);

                        if (String.IsNullOrEmpty(ErrorSendMessage))
                            _serialPorts.SendPackage(HexSendMessage, CRC);
                        else
                            _serialPorts.SendPackage(ErrorSendMessage, CRC);
                        
                        HexReceivedMessage = _serialPorts.ReceivedData;
                        BinReceivedMessage = HexReceivedMessage.HexToBin().GetHumanReadableBin();

                        string receivedCRC = CheckSum.CRC16(HexReceivedMessage.Substring(0, HexSendMessage.Length));
                        
                        if (receivedCRC != CRC)
                            throw new Exception("The hash didn't match.");
                    }
                    catch (Exception ex)
                    {
                        Logs = Logs.AppendLine(ex.Message);
                    }
                });
            }
        }

        public RelayCommand OpenButtonCommand
        {
            get
            {
                return new RelayCommand(click =>
                {
                    try
                    {
                        _serialPorts.OpenPorts();
                        Logs = Logs.AppendLine("The ports were successfully opened.");
                    }
                    catch (Exception ex)
                    {
                        Logs = Logs.AppendLine(ex.Message);
                    }
                });
            }
        }

        public RelayCommand CloseButtonCommand
        {
            get
            {
                return new RelayCommand(click =>
                {
                    try
                    {
                        _serialPorts.ClosePorts();
                        Logs = Logs.AppendLine("The ports were successfully closed.");
                    }
                    catch (Exception ex)
                    {
                        Logs = Logs.AppendLine(ex.Message);
                    }
                });
            }
        }

        public RelayCommand ClearButtonCommand
        {
            get
            {
                return new RelayCommand(click =>
                {
                    HexSendMessage = BinSendMessage = HexReceivedMessage = BinReceivedMessage = CRC = String.Empty;
                    ErrorsNum = 0;
                    Logs = Logs.AppendLine("Windows were cleared.");
                });
            }
        }


        public MainViewModel()
        {
            _logs = new StringBuilder();
            _serialPorts = new COM("COM1", "COM2");
        }
    }
}
