using COM_Ports_Packages.Core;
using System;
using System.Linq;
using System.Text;

namespace COM_Ports_Packages.MVVM.ViewModel
{
    internal class MainViewModel : ObservableObject
    {
        private COM _serialPorts;

        private string _sendMessage;

        public string SendMessage
        {
            get { return _sendMessage; }
            set
            {
                _sendMessage = value;
                OnPropertyChanged();
            }
        }

        private string _stuffedMessage;

        public string StuffedMessage
        {
            get { return _stuffedMessage; }
            set
            {
                _stuffedMessage = value;
                OnPropertyChanged();
            }
        }

        private string _receivedMessage;

        public string ReceivedMessage
        {
            get { return _receivedMessage; }
            set
            {
                _receivedMessage = value;
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


        public RelayCommand SendButtonCommand
        {
            get
            {
                return new RelayCommand(click =>
                {
                    try
                    {
                        _serialPorts.SendPackage(SendMessage);
                        ReceivedMessage = _serialPorts.ReceivedData;
                        var receivedBytes = Enumerable.Range(0, _serialPorts.StuffedData.Length / 8)
                                                      .Select(i => _serialPorts.StuffedData.Substring(i * 8, 8))
                                                      .ToArray();
                        StuffedMessage = String.Join(" ", receivedBytes);
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
                    SendMessage = ReceivedMessage = StuffedMessage = String.Empty;
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
