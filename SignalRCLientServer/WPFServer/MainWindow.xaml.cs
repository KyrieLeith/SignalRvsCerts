using Microsoft.AspNet.SignalR;
using Microsoft.Owin.Cors;
using Microsoft.Owin.Hosting;
using Owin;
using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Reflection;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace WPFServer
{
    /// <summary>
    /// WPF host for a SignalR server. The host can stop and start the SignalR
    /// server, report errors when trying to start the server on a URI where a
    /// server is already being hosted, and monitor when clients connect and disconnect. 
    /// The hub used in this server is a simple echo service, and has the same 
    /// functionality as the other hubs in the SignalR Getting Started tutorials.
    /// For simplicity, MVVM will not be used for this sample.
    /// </summary>
    public partial class MainWindow : Window
    {
        public IDisposable SignalR { get; set; }
        const string ServerURI = "http://localhost:8080";
        public ObservableCollection<Certificate> certs { get; set; }

        public MainWindow()
        {
            InitializeComponent();
            certs = new ObservableCollection<Certificate>();
            grid.ItemsSource=certs;
            GetCerts();
            WriteToConsole("Для просмотра сертификата требуется дважды кликнуть по нему.");
            WriteToConsole("---------------------------");
            WriteToConsole("Сервер не запущен.");
            WriteToConsole("Требуется запустить сервер.");
        }

        /// <summary>
        /// Calls the StartServer method with Task.Run to not
        /// block the UI thread. 
        /// </summary>
        private void ButtonStart_Click(object sender, RoutedEventArgs e)
        {
            WriteToConsole("---------------------------");
            WriteToConsole("Запуск сервера...");
            ButtonStart.IsEnabled = false;            
            Task.Run(() => StartServer());
        }

        /// <summary>
        /// Stops the server and closes the form. Restart functionality omitted
        /// for clarity.
        /// </summary>
        private void ButtonStop_Click(object sender, RoutedEventArgs e)
        {
            SignalR.Dispose();
            ButtonStop.IsEnabled = false;
            ButtonStart.IsEnabled = true;
            WriteToConsole("---------------------------");
            WriteToConsole("Сервер остановлен.");
        }

        private void ButtonCertList_Click(object sender, RoutedEventArgs e)
        {
            GetCerts();
        }

        private void ListViewItem_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            CertView cwind = new CertView(certs, grid.SelectedIndex);
            cwind.ShowDialog();
        }

        private void GetCerts()
        {
            certs.Clear();
            X509Store computerCaStore = new X509Store(StoreName.My, StoreLocation.CurrentUser);

            try
            {
                computerCaStore.Open(OpenFlags.ReadOnly);
                X509Certificate2Collection certificatesInStore = computerCaStore.Certificates;
                foreach (X509Certificate2 cert in certificatesInStore)
                {
                    Certificate ncert = new Certificate();
                    ncert.ExpirationDate = cert.GetExpirationDateString();
                    ncert.Issuer = cert.Issuer;
                    ncert.EffectiveDate = cert.GetEffectiveDateString();
                    ncert.NameInfo = cert.GetNameInfo(X509NameType.SimpleName, true);
                    ncert.HasPrivateKey = cert.HasPrivateKey;
                    ncert.SubjectName = cert.SubjectName.Name;
                    certs.Add(ncert);
                }
            }
            finally
            {
                computerCaStore.Close();
            }
        }

        /// <summary>
        /// Starts the server and checks for error thrown when another server is already 
        /// running. This method is called asynchronously from Button_Start.
        /// </summary>
        private void StartServer()
        {
            try
            {
                SignalR = WebApp.Start(ServerURI);
            }
            catch (TargetInvocationException)
            {
                WriteToConsole("Сервер уже запущен на " + ServerURI);
                this.Dispatcher.Invoke(() => ButtonStop.IsEnabled = true);
                return;
            }
            this.Dispatcher.Invoke(() => ButtonStop.IsEnabled = true);
            WriteToConsole("Сервер запущен на " + ServerURI);
        }
        ///This method adds a line to the RichTextBoxConsole control, using Dispatcher.Invoke if used
        /// from a SignalR hub thread rather than the UI thread.
        public void WriteToConsole(String message)
        {
            if (!(RichTextBoxConsole.CheckAccess()))
            {
                this.Dispatcher.Invoke(() =>
                    WriteToConsole(message)
                );
                return;
            }
            RichTextBoxConsole.AppendText(message + "\r");
        }
    }
    /// <summary>
    /// Used by OWIN's startup process. 
    /// </summary>
    class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            app.UseCors(CorsOptions.AllowAll);
            app.MapSignalR();
        }
    }
    /// <summary>
    /// Echoes messages sent using the Send message by calling the
    /// addMessage method on the client. Also reports to the console
    /// when clients connect and disconnect.
    /// </summary>
    public class MyHub : Hub
    {
        public ObservableCollection<Certificate> certs { get; set; }
        public void Send(string name, string message)
        {
            Clients.All.addMessage(name, message);
        }
        public void TestConnection()
        {
            Clients.Caller.testMessage("Соединение установлено.");
        }
        public void ListCerts()
        {
            if (certs == null)
                certs = new ObservableCollection<Certificate>();
            certs.Clear();
            X509Store computerCaStore = new X509Store(StoreName.My, StoreLocation.CurrentUser);
            try
            {
                computerCaStore.Open(OpenFlags.ReadOnly);
                X509Certificate2Collection certificatesInStore = computerCaStore.Certificates;
                foreach (X509Certificate2 cert in certificatesInStore)
                {
                    Certificate ncert = new Certificate();
                    ncert.ExpirationDate = cert.GetExpirationDateString();
                    ncert.Issuer = cert.Issuer;
                    ncert.EffectiveDate = cert.GetEffectiveDateString();
                    ncert.NameInfo = cert.GetNameInfo(X509NameType.SimpleName, true);
                    ncert.HasPrivateKey = cert.HasPrivateKey;
                    ncert.SubjectName = cert.SubjectName.Name;
                    certs.Add(ncert);
                }
            }
            finally
            {
                computerCaStore.Close();
            }
            Clients.Caller.displayList(certs);
        }
        public void ShowCert(int certid)
        {
            if (certs == null)
                certs = new ObservableCollection<Certificate>();
            certs.Clear();
            X509Store computerCaStore = new X509Store(StoreName.My, StoreLocation.CurrentUser);
            try
            {
                computerCaStore.Open(OpenFlags.ReadOnly);
                X509Certificate2Collection certificatesInStore = computerCaStore.Certificates;
                foreach (X509Certificate2 cert in certificatesInStore)
                {
                    Certificate ncert = new Certificate();
                    ncert.ExpirationDate = cert.GetExpirationDateString();
                    ncert.Issuer = cert.Issuer;
                    ncert.EffectiveDate = cert.GetEffectiveDateString();
                    ncert.NameInfo = cert.GetNameInfo(X509NameType.SimpleName, true);
                    ncert.HasPrivateKey = cert.HasPrivateKey;
                    ncert.SubjectName = cert.SubjectName.Name;
                    certs.Add(ncert);
                }
            }
            finally
            {
                computerCaStore.Close();
            }
            if (certid-1>=0 && certid-1<certs.Count)
                Clients.Caller.displayCert(certs[certid-1]);
        }
        public override Task OnConnected()
        {
            //Use Application.Current.Dispatcher to access UI thread from outside the MainWindow class
            Application.Current.Dispatcher.Invoke(() => 
                ((MainWindow)Application.Current.MainWindow).WriteToConsole("Client connected: " + Context.ConnectionId));

            return base.OnConnected();
        }
        public override Task OnDisconnected()
        {
            //Use Application.Current.Dispatcher to access UI thread from outside the MainWindow class
            Application.Current.Dispatcher.Invoke(() => 
                ((MainWindow)Application.Current.MainWindow).WriteToConsole("Client disconnected: " + Context.ConnectionId));

            return base.OnDisconnected();
        }

    }
}
