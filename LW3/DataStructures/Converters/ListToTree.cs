using System.Collections.Generic;

namespace DataStructures.Converters
{
    internal class ListToTree<TValue> where TValue : class
    {
        public List<AVLNodeData<TValue>> NodesList { get; set; }

        public ListToTree( List<AVLNodeData<TValue>> nodesList )
        {
            NodesList = nodesList;
        }

        public AVLTree<TValue> Convert( )
        {
            AVLTree<TValue> tree = new();
            foreach ( var node in NodesList ) {
                tree.Add(node.Value, node.Key);
            }
            return tree;
        }
    }
}
