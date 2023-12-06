using DataStructures.Converters;
using System.Collections.Generic;

namespace DataStructures
{
    public class TreeConverter<TValue> where TValue : class
    {
        public AVLNode<TValue>? Root { get; set; }

        public TreeConverter( AVLNode<TValue> root )
        {
            Root = root;
        }

        public TreeConverter( ) { }

        public string ToString( bool isValueDrawn)
        {
            if ( Root is null )
                return string.Empty;
            var converter = new TreeToSring<TValue>(Root, isValueDrawn);
            return converter.Convert();
        }

        public List<AVLNodeData<TValue>> ToList( )
        {
            if ( Root is null )
                return new List<AVLNodeData<TValue>>();
            var converter = new TreeToList<TValue>(Root);
            return converter.Convert();
        }

        public AVLTree<TValue> FromList( List<AVLNodeData<TValue>> nodesList )
        {
            var converter = new ListToTree<TValue>(nodesList);
            return converter.Convert();
        }
    }
}
