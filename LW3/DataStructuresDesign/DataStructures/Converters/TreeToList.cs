using System.Collections.Generic;

namespace DataStructures.Converters
{
    internal class TreeToList<TValue>
    {
        public AVLNode<TValue> Root { get; set; }

        public TreeToList( AVLNode<TValue> root )
        {
            Root = root;
        }

        public List<AVLNodeData<TValue>> Convert( )
        {
            Queue<AVLNode<TValue>> queue = new();
            AVLNode<TValue>? curNode;
            List<AVLNodeData<TValue>> result = new();

            queue.Enqueue(Root);
            while ( queue.Count > 0 ) {
                curNode = queue.Dequeue();
                result.Add(new(curNode));

                if ( curNode.ChildLeft is not null ) {
                    queue.Enqueue(curNode.ChildLeft);
                }
                if ( curNode.ChildRight is not null ) {
                    queue.Enqueue(curNode.ChildRight);
                }
            }
            queue.Clear();

            return result;
        }
    }
}
