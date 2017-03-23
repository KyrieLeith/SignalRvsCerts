using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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

namespace WPFServer
{
    /// <summary>
    /// Логика взаимодействия для CertView.xaml
    /// </summary>
    public partial class CertView : Window
    {
        public ObservableCollection<Certificate> certs;
        public int SelectedIndex;
        public CertView(ObservableCollection<Certificate> inp_certs, int ind)
        {
            InitializeComponent();
            certs = inp_certs;
            SelectedIndex = ind;
            itemGrid.DataContext = certs[SelectedIndex];
        }
        private void ButtonClose_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
