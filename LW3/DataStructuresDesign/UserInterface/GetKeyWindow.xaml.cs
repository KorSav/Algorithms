using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace UserInterface
{
    /// <summary>
    /// Interaction logic for GetKeyWindow.xaml
    /// </summary>
    public partial class GetKeyWindow : Window
    {
        public int? Key { get; set; }
        public string Message { get; set; }

        public GetKeyWindow(string message)
        {
            Message = message;

            InitializeComponent();
            LblMessage.Text = Message;
            TboxKey.Focus();
        }

        public bool IsDataEntered =>
            Key is not null;

        private void TextBox_PreviewTextInput( object sender, TextCompositionEventArgs e )
        {
            var regex = new Regex("\\D+");
            e.Handled = regex.IsMatch(e.Text);
        }

        private void BtnConfirm_Click( object sender, RoutedEventArgs e )
        {
            if (TboxKey.Text == "") {
                MessageBox.Show("Ви не ввели ключ.");
                e.Handled=true;
                return;
            }
            Key = Convert.ToInt32(TboxKey.Text);
            e.Handled = true;
            Close();
        }

        private void TboxKey_KeyDown( object sender, KeyEventArgs e )
        {
            if (e.Key == System.Windows.Input.Key.Enter ) {
                BtnConfirm.Focus();
            }
        }

        private void Button_Click( object sender, RoutedEventArgs e )
        {
            Close();
        }
    }
}
