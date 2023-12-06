using DataStructures;
using Microsoft.Win32;
using Notification.Wpf;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace UserInterface
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private FileOperator _fileOperator = new("dbavl");
        private AVLTree<string>? tree = null;
        private bool _isValueDrawn = true;
        private bool _isTreeDrawn = false;

        public MainWindow( )
        {
            InitializeComponent();
            LblFileName.DataContext = _fileOperator;
            PanelButtons.DataContext = _fileOperator;
            MIFileSave.DataContext = _fileOperator;
        }

        private void FileOpen_executed( object sender, ExecutedRoutedEventArgs e )
        {
            ( BtnDraw.Content as TextBlock ).Text = "Відобразити деревом";
            _isTreeDrawn = false;
            OpenFileDialog dlg = new() {
                DefaultExt = _fileOperator.Extension,
                Filter = $"Database files|*.{_fileOperator.Extension}"
            };
            Console.WriteLine("opening file...");
            var res = dlg.ShowDialog();
            if ( res is false ) {
                Console.WriteLine("File opening canceled.");
                return;
            }
            _fileOperator.Path = dlg.FileName;
            tree = _fileOperator.ReadTree();
            DrawTree();
        }

        private void FileSave_Executed( object sender, ExecutedRoutedEventArgs e )
        {
            if ( tree is null )
                return;
            Console.WriteLine("saving file...");
            if ( tree.Root is null ) {
                Console.WriteLine("saving empty file");
            }
            _fileOperator.WriteTree(tree);
            ShowPopupMessage("Файл збережено");
        }

        private void BtnFind_Click( object sender, RoutedEventArgs e )
        {
            var gkw = new GetKeyWindow("Введіть ключ, за яким потрібно знайти елемент.");
            gkw.ShowDialog();
            if ( gkw.IsDataEntered is false ) {
                return;
            }
            int key = gkw.Key!.Value;
            string? res = tree!.Find(key, out int comparesCount);
            if ( res is null ) {
                MessageBox.Show("Значення за таким ключем відсутнє.");
                return;
            }
            MessageBox.Show($"За ключем {key} знаходиться значення {res}\n" +
                $"Знайдено за {comparesCount} порівнянь.");
        }

        private void BtnAdd_Click( object sender, RoutedEventArgs e )
        {
            var gkvw = new GetKeyValueWindow("Введіть дані, які необхідно вставити в бд:");
            gkvw.ShowDialog();
            if ( gkvw.IsDataEntered is false ) {
                Console.WriteLine("Adding canceled.");
                return;
            }
            try {
                tree!.Add(gkvw.Value!, gkvw.Key);
                DrawTree();
            }
            catch ( Exception ex ) {
                MessageBox.Show(ex.Message);
            }
        }

        private void BtnRemove_Click( object sender, RoutedEventArgs e )
        {
            var gkw = new GetKeyWindow("Введіть ключ за яким потрібно видалити дані:");
            gkw.ShowDialog();
            if ( gkw.IsDataEntered is false ) {
                Console.WriteLine("Removing canceled.");
                return;
            }
            int key = gkw.Key!.Value;
            try {
                tree!.Remove(key);
                DrawTree();
            }catch ( Exception ex ) {
                MessageBox.Show(ex.Message);
            }
        }

        private void BtnEdit_Click( object sender, RoutedEventArgs e )
        {
            var gkvw = new GetKeyValueWindow("Введіть ключ та нові дані для нього:");
            gkvw.ShowDialog();
            if ( gkvw.IsDataEntered is false ) {
                Console.WriteLine("Editing canceled.");
                return;
            }
            Console.WriteLine("editing...");
            int key = gkvw.Key!.Value;
            string value = gkvw.Value!;
            try {
                tree!.Edit(key, value);
                DrawTree();
            }
            catch ( Exception ex ) {
                MessageBox.Show(ex.Message);
            }
        }

        private void DrawTree( )
        {
            if ( tree!.Root is null ) {
                TxtBlockTree.Text = "Дерево порожнє.";
                return;
            }
            TreeConverter<string> converter = new(tree.Root);
            if ( _isTreeDrawn ) {
                if ( tree.Count > 1000 ) {
                    MessageBox.Show("Відобразити можна дерева " +
                        "розміром до 1000 елементів");
                    (BtnDraw.Content as TextBlock).Text = "Відобразити деревом";
                    _isTreeDrawn = false;
                    return;
                }
                TxtBlockTree.TextAlignment = TextAlignment.Center;
                TxtBlockTree.Text = converter.ToString(false);
                return;
            }
            TxtBlockTree.Text = "";
            TxtBlockTree.TextAlignment = TextAlignment.Left;
            foreach ( var node in tree ) {
                TxtBlockTree.Text += $"{node.Key}\t{node.Value}\n";
            }
        }

        private static void ShowPopupMessage( string message )
        {
            var notificationManager = new NotificationManager();
            notificationManager.Show(new NotificationContent {
                Message = message,
                Type = NotificationType.Information,
            });

        }

        private void BtnFillRandom_Click( object sender, RoutedEventArgs e )
        {
            ( BtnDraw.Content as TextBlock ).Text = "Відобразити деревом";
            _isTreeDrawn = false;
            tree.Dispose();
            Console.WriteLine(tree.ToString());
            for ( int i = 0; i < 10000; i++ ) {
                tree.Add($"Value{i + 1}", i + 1);
            }
            DrawTree();
        }

        private void BtnDraw_Click( object sender, RoutedEventArgs e )
        {
            _isTreeDrawn = !_isTreeDrawn;
            if ( tree.Count > 1000 ) {
                DrawTree();
                return;
            }
            var tblock = BtnDraw.Content as TextBlock;
            if ( tblock is null ) {
                Console.WriteLine("Button remove value does not have text block!");
                return;
            }
            if ( _isTreeDrawn ) {
                tblock.Text = "Відобразити списком";
            } else {
                tblock.Text = "Відобразити деревом";
            }
            DrawTree();
        }
    }
}
