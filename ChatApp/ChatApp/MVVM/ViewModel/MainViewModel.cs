using ChatClient.MVVM.Core;
using ChatClient.MVVM.Model;
using ChatClient.Net;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;

namespace ChatClient.MVVM.ViewModel
{
    internal class MainViewModel : ObservableObject
    {
        public ObservableCollection<UserModel> Users { get; set; }
        public ObservableCollection<string> Messages { get; set; }
        public RelayCommand ConnectToServerCommand { get; set; }
        public RelayCommand SendMessageCommand { get; set; }

        public string Username { get; set; }

        private string _message;
        
        public string Message
        {
            get => _message;
            set
            {
                _message = value; 
                OnPropertyChanged();
            }
        }

        private readonly Server _server;

        public MainViewModel()
        {
            Users = new ObservableCollection<UserModel>();
            Messages = new ObservableCollection<string>();
            _server = new Server();
            _server.ConnectedEvent += UserConnected;
            _server.MessageReceivedEvent += MessageReceived;
            _server.UserDisconnectEvent += UserDisconnect;

            ConnectToServerCommand = new RelayCommand(o => _server.ConnectToServer(Username), 
                o => !string.IsNullOrEmpty(Username));

            SendMessageCommand = new RelayCommand(o => {
                _server.SendMessageToServer(Message);
            }, o => !string.IsNullOrEmpty(Message));
        }

        private void UserConnected()
        {
            var user = new UserModel
            {
                Username = _server.PacketReader.ReadMessage(),
                Uid = _server.PacketReader.ReadMessage(),
            };

            if(Users.All(x => x.Uid != user.Uid))
            {
                Application.Current.Dispatcher.Invoke(() => Users.Add(user));
            }
        }

        private void UserDisconnect()
        {
            var uid = _server.PacketReader.ReadMessage();
            var user = Users.FirstOrDefault(x => x.Uid == uid);
            Application.Current.Dispatcher.Invoke(() => Users.Remove(user));
        }

        private void MessageReceived()
        {
            var msg = _server.PacketReader.ReadMessage();
            Application.Current.Dispatcher.Invoke(() =>
            {
                Messages.Add(msg);
            });
            Message = "";
        }

    }
}
