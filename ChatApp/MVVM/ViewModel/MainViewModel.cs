using ChatApp.MVVM.Core;
using ChatApp.MVVM.Model;
using ChatApp.Net;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Net.Http;
using System.Security.RightsManagement;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace ChatApp.MVVM.ViewModel
{
    internal class MainViewModel
    {

        public ObservableCollection<UserModel> Users { get; set; }
        public ObservableCollection<string> Messages { get; set; }
        public RelayCommand ConnectToServerCommand { get; set; }
        public RelayCommand SendMessageCommand { get; set; }
        public RelayCommand CloseApplicationCommand { get; set; }

        private Server _server;
        public string Username { get; set; }
        public string Message { get; set; }
        public MainViewModel()
        {
            Users=new ObservableCollection<UserModel>();
            Messages = new ObservableCollection<string>();
            _server =new Server();
            _server.connectedEvent += UserConnected;
            _server.msgRecievedEvent += MessageRecieved;
            _server.userDissconnectedEvent += RemoveUser;

            ConnectToServerCommand = new RelayCommand(o=>_server.ConnectToServer(Username),o=>!string.IsNullOrEmpty(Username));
            SendMessageCommand = new RelayCommand(o => _server.SendMessageToServer(Message), o => !string.IsNullOrEmpty(Message));
            CloseApplicationCommand = new RelayCommand(o=>CloseApplication());

        }
        private void UserConnected()
        {
            var User=new UserModel { Username=_server.PacketReader.ReadMessage(),UID=_server.PacketReader.ReadMessage()};

            if(!Users.Any(x=>x.UID==User.UID))
            {
                Application.Current.Dispatcher.Invoke(() => Users.Add(User));
            }
        }
        private void MessageRecieved()
        {
            var msg=_server.PacketReader.ReadMessage();
            Application.Current.Dispatcher.Invoke(() => Messages.Add(msg));
        }
        private void RemoveUser()
        {
            var uid=_server.PacketReader.ReadMessage();
            var user=Users.Where(x=>x.UID==uid).FirstOrDefault();
            Application.Current.Dispatcher.Invoke(()=>Users.Remove(user));
        }
        private void CloseApplication()
        {
            
            Application.Current.Shutdown();
        }


    }
}
