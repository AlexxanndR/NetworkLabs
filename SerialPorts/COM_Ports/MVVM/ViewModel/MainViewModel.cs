using COM_Ports.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace COM_Ports.MVVM.ViewModel
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

        public StringBuilder logs 
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
                        logs = logs.AppendLine("Ports have been successfully opened.");
                    }
                    catch (Exception ex)
                    {
                        logs = logs.AppendLine(ex.Message);
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
                        logs = logs.AppendLine("Ports have been successfully closed.");
                    }
                    catch (Exception ex)
                    {
                        logs = logs.AppendLine(ex.Message);
                    }
                });
            }
        }
        public RelayCommand SendDataCommand
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
                        logs = logs.AppendLine(ex.Message);
                    }
                });
            }
        }

        public MainViewModel()
        {
            _serialPorts = new COM("COM1", "COM2");
        }
    }
}
