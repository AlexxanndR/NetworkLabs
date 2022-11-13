using COM_Ports_Collisions.Core;
using System;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;

namespace COM_Ports_Collisions.MVVM.ViewModel
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

        private uint _busyProbability;

        public uint BusyProbability
        {
            get { return _busyProbability; }
            set
            {
                _busyProbability = value;
                OnPropertyChanged();
            }
        }

        private uint _collisionProbability;

        public uint CollisionProbability
        {
            get { return _collisionProbability; }
            set
            {
                _collisionProbability = value;
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
                        if (BusyProbability > 100)
                            throw new Exception("Incorrect busy probability [>100].");
                        
                        if (CollisionProbability > 100)
                            throw new Exception("Incorrect collision probability [>100].");

                        var result = _serialPorts.SendPackage(SendMessage, CollisionProbability, BusyProbability);

                        ReceivedMessage = _serialPorts.ReceivedData;

                        Logs = Logs.AppendLine(result + "The package was successfully received.");
                    }
                    catch (Exception ex)
                    {
                        ReceivedMessage = String.Empty;
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
                    SendMessage = ReceivedMessage = String.Empty;
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
