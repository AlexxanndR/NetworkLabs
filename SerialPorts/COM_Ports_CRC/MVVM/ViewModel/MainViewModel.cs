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
                return new RelayCommand(click =>
                {
                    if (Regex.IsMatch(HexSendMessage, @"^[a-f0-9]+$"))
                        BinSendMessage = HexSendMessage.HexToBin();
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
                        if (ErrorsNum <= Convert.ToInt32(_serialPorts.PackageLength) * 8)
                        {
                            Random rd = new Random();

                            for (int i = 0; i < ErrorsNum; i++)
                            {
                                int errorPos = rd.Next(0, ErrorsNum - 1);
                                var dataField = BinSendMessage.Replace(" ", String.Empty).Substring(4 * 8, 16);
                                ErrorSendMessage = new string (dataField.Select((x, j) => j != errorPos ? x : (x == '0' ? '1' : '0')).ToArray());
                            }
                        }
                        else
                            throw new Exception("Too many errors. Should be no more than 16.");
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
                        if (ErrorsNum == 0)
                            _serialPorts.SendPackage(HexSendMessage);
                        else {
                            _serialPorts.SendPackage(ErrorSendMessage);
                            ErrorsNum = 0;
                        }                      
                        HexReceivedMessage = _serialPorts.ReceivedData;
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
                    HexSendMessage = BinSendMessage = HexReceivedMessage = BinReceivedMessage = String.Empty;
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
