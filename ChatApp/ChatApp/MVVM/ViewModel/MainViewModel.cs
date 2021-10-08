using ChatClient.MVVM.Core;
using ChatClient.Net;

namespace ChatClient.MVVM.ViewModel
{
    class MainViewModel
    {
        public RelayCommand ConnectToServerCommand { get; set; }

        private Server _server;
        
        public MainViewModel()
        {
            _server = new Server();
            ConnectToServerCommand = new RelayCommand(o => _server.ConnectToServer());
        }
    }
}
