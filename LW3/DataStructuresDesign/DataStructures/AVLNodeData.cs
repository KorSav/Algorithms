namespace DataStructures
{
    public struct AVLNodeData<TValue>
    {
        public int Key;
        public TValue Value;
        public int Height;

        public AVLNodeData( int key, TValue value, int height )
        {
            Key = key;
            Value = value;
            Height = height;
        }

        public AVLNodeData( AVLNode<TValue> node )
        {
            Key = node.Key;
            Value = node.Value;
            Height = node.Height;
        }
    }
}
