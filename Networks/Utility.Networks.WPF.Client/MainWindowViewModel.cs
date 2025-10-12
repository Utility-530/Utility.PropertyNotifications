using Simple.Models;
using System;
using System.Collections.ObjectModel;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using Utility.Commands;
using Utility.Networks.Infrastructure;
using Utility.Networks.Packets;
using Utility.Networks.WPF.Server.Services;
using Utility.PropertyNotifications;

namespace Utility.Networks.WPF.Client
{
    public record User(Guid Guid, string Name, string Color);
    //public record UserDetails(string Name, string Color);
    public record Message(string Name, string Color, string Content);

    public class MainWindowViewModel : NotifyPropertyClass
    {
        private string _username = "User" + new Random().Next(1000, 9999);
        private string _address = "127.0.0.1";
        private string _port = "8000";
        private SynchronizationContext context;
        private string _message;

        private string _colorCode;
        private bool isFlashing;
        private bool isConnected;

        public string Username
        {
            get { return _username; }
            set { RaisePropertyChanged(ref _username, value); }
        }

        public string Address
        {
            get { return _address; }
            set { RaisePropertyChanged(ref _address, value); }
        }

        public string Port
        {
            get { return _port; }
            set { RaisePropertyChanged(ref _port, value); }
        }

        public string Message
        {
            get { return _message; }
            set { RaisePropertyChanged(ref _message, value); }
        }

        public string ColorCode
        {
            get { return _colorCode; }
            set { RaisePropertyChanged(ref _colorCode, value); }
        }

        public bool IsConnected
        {
            get { return isConnected; }
            set { RaisePropertyChanged(ref isConnected, value); }
        }

        public ICommand ConnectCommand { get; set; }
        public ICommand DisconnectCommand { get; set; }
        public ICommand SendCommand { get; set; }

        public ObservableCollection<Message> Messages { get; } = new();
        public ObservableCollection<User> Users { get; } = new();
        public ObservableCollection<Exception> Exceptions { get; } = new();

        public MainWindowViewModel()
        {
            context = SynchronizationContext.Current;
            ConnectCommand = new Command(Connect, canConnect);
            DisconnectCommand = new Command(Disconnect, canDisconnect);
            SendCommand = new Command(Send, canSend);
            Model.Instance.Changes.CollectionChanged += (s, e) =>
            {
                foreach (IChange item in e.NewItems)
                {
                    if(item is ExceptionChange { Value: { } exception })
                    {
                        Exceptions.Add(exception);
                    }
                }
            };
        }

        private async Task Connect()
        {
            Clear();

            if (!validate(out int socketPort))
            {
                return;
            }


            Model.Instance.AddChange(new ConnectChange(new ConnectParameters(Address, socketPort, packet =>
            {
                switch (packet)
                {
                    case PongPacket:
                        {
                            IsConnected = true;
                            return;
                        }
                    case MessagePacket { Value: string value } chatP:
                        {
                            var user = Users.Single(a => a.Guid == chatP.Source);
                            Messages.Add(new Client.Message(user.Name, user.Color, value));
                            break;
                        }
                    case ConnectionPacket { Source: { } source, Value: UserDetails { Name: { } name, ColorCode: { } color } }:
                        {
                            if (Users.Any(u => u.Guid == source) == false)
                            {
                                Users.Add(new User(source, name, color));
                                Messages.Add(new Message(name, color, $"{name} has connected to the server"));
                            }
                            break;
                        }
                    case DisConnectionPacket { Source: { } _source }:
                        {
                            var user = Users.Single(a => a.Guid == _source);
                            Users.Remove(user);
                            Messages.Add(new Message(user.Name, user.Color, $"{user.Name} has disconnected from the server"));
                            break;
                        }
                }


            }, a =>
            {
                Messages.Add(new Message(Username, ColorCode, "You have connected to the server"));
                return new UserDetails(Username, ColorCode);

            }, a =>
            {
                IsConnected = false;
                Messages.Add(new Message(Username, ColorCode, "You have disconnected from the server"));
            })));
        }

        public void Clear()
        {
            Messages.Clear();
            Users.Clear();
        }

        private async Task Disconnect()
        {
            if (IsConnected == false)
                displayError("You are not connected to a server.");

            //await Client.DisconnectAsync();
            Model.Instance.AddChange(new DisconnectChange());

        }

        private async Task Send()
        {
            if (IsConnected == false)
                displayError("You are not connected to a server.");

            string message = Message;
            context.Post(_ =>
            {
                Messages.Add(new Client.Message(Username, ColorCode, message));
            }, null);

            Model.Instance.AddChange(new SendChange(message));
            Message = string.Empty;
        }

        private bool canConnect() => IsConnected == false;
        private bool canDisconnect() => IsConnected;
        private bool canSend() => !string.IsNullOrWhiteSpace(Message) && IsConnected;

        private void displayError(string message) =>
            MessageBox.Show(message, "Woah there!", MessageBoxButton.OK, MessageBoxImage.Error);

        bool validate(out int socketPort)
        {
            if (!int.TryParse(Port, out socketPort))
            {
                displayError($"Please provide a valid port ({socketPort}).");
                return false;
            }

            if (string.IsNullOrWhiteSpace(Address))
            {
                displayError($"Please provide a valid address {Address}.");
                return false;
            }

            if (string.IsNullOrWhiteSpace(Username))
            {
                displayError($"Please provide a username {Username}.");
                return false;
            }

            return true;
        }
    }
}
