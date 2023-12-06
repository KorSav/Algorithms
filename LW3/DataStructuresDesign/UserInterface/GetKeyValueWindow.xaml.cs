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
    /// Interaction logic for GetKeyValueWindow.xaml
    /// </summary>
    public partial class GetKeyValueWindow : Window
    {
        public int? Key { get; private set; }
        public string? Value { get; private set; }
        public string Message { get; set; }

        public GetKeyValueWindow(string message)
        {
            Message = message;

            InitializeComponent();
            LblMessage.Text = Message;
            TboxKey.Focus();
        }

        public bool IsDataEntered => 
            (Key is not null && Value is not null);

        private void TboxKey_PreviewTextInput( object sender, TextCompositionEventArgs e )
        {
            var regex = new Regex("\\D+");
            e.Handled = regex.IsMatch( e.Text );
        }

        private void TboxKey_KeyDown( object sender, KeyEventArgs e )
        {
            var tbox = sender as TextBox;
            if (e.Key == System.Windows.Input.Key.Enter && tbox!.Text != "") {
                TboxValue.Focus();
            }
        }

        private void BtnConfirm_Click( object sender, RoutedEventArgs e )
        {
            if ( TboxKey.Text == "" ) {
                MessageBox.Show("Ви не ввели ключ.");
                e.Handled = true;
                return;
            }
            else if ( TboxValue.Text == "" ) {
                MessageBox.Show("Ви не ввели значення.");
                e.Handled = true;
                return;
            }
            Key = Convert.ToInt32(TboxKey.Text);
            Value = TboxValue.Text;
            e.Handled = true;
            Close();
        }

        private void TboxValue_KeyDown( object sender, KeyEventArgs e )
        {
            var tbox = sender as TextBox;
            if ( e.Key == System.Windows.Input.Key.Enter ) {
                if (tbox!.Text == "" ) {
                    tbox!.Text = "_";
                }
                BtnConfirm.Focus();
            }
        }

        private void BtnCancel_Click( object sender, RoutedEventArgs e )
        {
            Close();
        }
    }
}
