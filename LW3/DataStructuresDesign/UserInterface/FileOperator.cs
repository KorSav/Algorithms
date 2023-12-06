using DataStructures;
using System;
using System.ComponentModel;
using System.IO;

namespace UserInterface
{
    public class FileOperator : INotifyPropertyChanged
    {
        public string Extension {  get; set; }
        private string? _path = null;

        public FileOperator( string extension )
        {
            Extension = extension;
        }
        public string Path {
            get {
                return _path ?? "Не відкрито файл.";
            }
            set {
                _path = value;
                OnPropertyChanged("Path");    
                OnPropertyChanged("IsFileOpened");
            }
        }
        public bool IsFileOpened {
            get { 
                return _path is not null;
            }
        }

        public AVLTree<string> ReadTree( )
        {
            AVLTree<string> tree = new();
            BinaryReader br = new(new FileStream(_path, FileMode.Open, FileAccess.Read));
            while (br.BaseStream.Position < br.BaseStream.Length ) {
                tree.Add(br.ReadString(), br.ReadInt32());
            }
            br.Close();
            return tree;
        }

        public void WriteTree( AVLTree<string> tree )
        {
            BinaryWriter bw = new(new FileStream(_path, FileMode.Create, FileAccess.Write));
            if ( tree.Root is null ) {
                bw.Close();
                return;
            }
            foreach (var node in tree) {
                bw.Write(node.Value);
                bw.Write(node.Key);
            }
            bw.Close();
        }

        public event PropertyChangedEventHandler? PropertyChanged;

        private void OnPropertyChanged( string info )
        {
            PropertyChanged?.Invoke( this, new PropertyChangedEventArgs( info ) );
        }
    }
}
