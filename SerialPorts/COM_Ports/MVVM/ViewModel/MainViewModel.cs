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
        private string _sendMessage;

        public string sendMessage
        {
            get { return _sendMessage; }
            set
            {
                _sendMessage = value;
                OnPropertyChanged();
            }
        }

        private string _receivedMessage;

        public String receivedMessage
        {
            get { return _receivedMessage; }
            set
            {
                _receivedMessage = value;
                OnPropertyChanged();
            }
        }

        private StringBuilder _logs = new StringBuilder();

        public StringBuilder logs {
            get { return _logs; }
            set
            {
                _logs = value;
                OnPropertyChanged();
            }
        }

/*        public RelayCommand SendMessageReadyCommand
        {
            get
            {
                return new RelayCommand(changed => receivedMessage = "s");
            }
        }*/

        public MainViewModel()
        {

        }


    }
}
