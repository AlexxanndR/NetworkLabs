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

        public String ReceivedMessage
        {
            get { return _receivedMessage; }
            set
            {
                _receivedMessage = value;
                OnPropertyChanged();
            }
        }

        private StringBuilder _logs = new StringBuilder();

        public StringBuilder Logs 
        {
            get { return _logs; }
            set
            {
                _logs = value;
                OnPropertyChanged();
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
                        Logs = Logs.AppendLine("Ports have been successfully opened.");
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
                        Logs = Logs.AppendLine("Ports have been successfully closed.");
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
                    try
                    {
                        _sendMessage = _receivedMessage = _stuffedMessage = String.Empty;
                        Logs = Logs.AppendLine("Windows were cleared.");
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
                return new RelayCommand(changed =>
                {
                    try 
                    {
                        _serialPorts.SendData(SendMessage);
                        ReceivedMessage = _serialPorts.ReceivedData;
                    } 
                    catch (Exception ex)
                    {
                        Logs = Logs.AppendLine(ex.Message);
                    }
                });
            }
        }

        public MainViewModel()
        {  
            _serialPorts = new COM("COM5", "COM6");
        }
    }
}
