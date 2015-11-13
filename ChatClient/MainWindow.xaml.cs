using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Common;

namespace ChatClient
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public static CClient client;

        public MainWindow()
        {
            InitializeComponent();

        }

        private void ClientOnDataMessageArrived(IMessage message)
        {
            switch (message.MessageType)
            {
                case MessageType.Broadcast:
                    var aMsg = (Broadcast)message;
                    Dispatcher.Invoke(() => AddLog(aMsg.Message));
                    break;
                case MessageType.Error:
                    var eMsg = (Error)message;
                    Dispatcher.Invoke(() => MessageBox.Show(eMsg.ErrorMessage));
                    break;
            }
        }

        private void AddLog(string msg)
        {
            TextBoxLog.Text += msg + Environment.NewLine;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            client.SendMessage(new Broadcast { SessionId = SessionId, Message = TextBoxMessage.Text });

//            AddLog(TextBoxMessage.Text);
            TextBoxMessage.Text = string.Empty;
        }

        public long SessionId { get; set; }
        private void MainWindow_OnLoaded(object sender, RoutedEventArgs e)
        {
            var loginWindow = new LoginDialog();
            loginWindow.Owner = this;
            loginWindow.WindowStartupLocation = WindowStartupLocation.CenterOwner;
            loginWindow.ShowDialog();
            if (loginWindow.LoggedIn)
            {
                ButtonSend.IsEnabled = true;
                client.DataMessageArrived += ClientOnDataMessageArrived;
                SessionId = loginWindow.SessionId;
            }
        }
    }
}