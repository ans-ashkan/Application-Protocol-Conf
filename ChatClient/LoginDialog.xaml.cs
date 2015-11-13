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
using System.Windows.Shapes;
using Common;

namespace ChatClient
{
    /// <summary>
    /// Interaction logic for LoginDialog.xaml
    /// </summary>
    public partial class LoginDialog : Window
    {
        public bool LoggedIn { get; set; }
        public long SessionId { get; set; }

        public LoginDialog()
        {
            InitializeComponent();
        }

        private void ButtonLogin_Click(object sender, RoutedEventArgs e)
        {
            MainWindow.client = new CClient();
            MainWindow.client.Connected += () => Dispatcher.Invoke(() =>
            {
                ButtonLogin.IsEnabled = false;
                ButtonCancel.IsEnabled = false;
                MainWindow.client.DataMessageArrived += ClientOnDataMessageArrived;

                MainWindow.client.SendMessage(new RequestSession()
                {
                    Username = TextBoxUsername.Text,
                    Password = PasswordBox.Password
                });
            });
        }

        private void ClientOnDataMessageArrived(IMessage msg)
        {
            switch (msg.MessageType)
            {
                case MessageType.RejectSession:
                    MessageBox.Show(((RejectSession)msg).Reason);
                    MainWindow.client.DataMessageArrived -= ClientOnDataMessageArrived;
                    MainWindow.client.Dispose();
                    break;
                case MessageType.AcceptSession:
                    var acceptmsg = (AcceptSession)msg;
                    MessageBox.Show(acceptmsg.WellcomeMessage);
                    MainWindow.client.DataMessageArrived -= ClientOnDataMessageArrived;
                    LoggedIn = true;
                    SessionId = acceptmsg.SessionId;
                    Dispatcher.Invoke(this.Close);
                    break;
                case MessageType.Error:
                    MessageBox.Show(((Error)msg).ErrorMessage);
                    MainWindow.client.DataMessageArrived -= ClientOnDataMessageArrived;
                    MainWindow.client.Dispose();
                    break;
            }
            Dispatcher.Invoke(() =>
            {
                ButtonLogin.IsEnabled = true;
                ButtonCancel.IsEnabled = true;
            });
        }

        private void ButtonCancel_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
