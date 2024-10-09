using System;
using ChatClient.MVVM.Core;
using ChatClient.MVVM.Model;
using ChatClient.Net;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;

namespace ChatClient.MVVM.ViewModel;

internal class MainViewModel : ObservableObject, IDisposable
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

    public string IpAddress { get; set; }

    private readonly Server _server;

    public MainViewModel()
    {
        Users = new ObservableCollection<UserModel>();
        Messages = new ObservableCollection<string>();
        _server = new Server();

        SubscribeToServer();
        CreateServerCommands();
    }

    public void Dispose()
    {
        UnsubscribeFromServer();
    }

    private void SubscribeToServer()
    {
        _server.ConnectedEvent += OnUserConnected;
        _server.MessageReceivedEvent += OnMessageReceived;
        _server.UserDisconnectEvent += OnUserDisconnect;
    }

    private void UnsubscribeFromServer()
    {
        _server.ConnectedEvent -= OnUserConnected;
        _server.MessageReceivedEvent -= OnMessageReceived;
        _server.UserDisconnectEvent -= OnUserDisconnect;
    }

    private void CreateServerCommands()
    {
        ConnectToServerCommand = new RelayCommand(_ => ConnectToServer(), _ => CanConnectToServer());
        SendMessageCommand = new RelayCommand(_ => SendMessageToServer(), _ => CanSendMessage());
    }

    private void ConnectToServer() =>
        _server.ConnectToServer(IpAddress, Username);

    private void SendMessageToServer() =>
        _server.SendMessageToServer(Message);

    private bool CanConnectToServer() =>
        !string.IsNullOrEmpty(IpAddress) && !string.IsNullOrEmpty(Username);

    private bool CanSendMessage() =>
        !string.IsNullOrEmpty(Message);

    private void OnUserConnected()
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

    private void OnUserDisconnect()
    {
        var uid = _server.PacketReader.ReadMessage();
        var user = Users.FirstOrDefault(x => x.Uid == uid);
        Application.Current.Dispatcher.Invoke(() => Users.Remove(user));
    }

    private void OnMessageReceived()
    {
        var msg = _server.PacketReader.ReadMessage();

        Application.Current.Dispatcher.Invoke(() =>
        {
            Messages.Add(msg);
        });
    }
}