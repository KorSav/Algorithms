using System;

namespace DataStructures
{
    public class AVLNode<TValue> : IDisposable
    {
        private static int s_key;

        #region Properties
        public int Key { get; private set; }
        public TValue Value { get; set; }
        public int Height { get; private set; }
        public AVLNode<TValue>? Parent { get; set; }
        public AVLNode<TValue>? ChildLeft { get; set; }
        public AVLNode<TValue>? ChildRight { get; set; }
        public AVLNode<TValue>? Sibling {
            get {
                if ( Parent is null )
                    return null;
                if ( this < Parent )
                    return Parent.ChildRight;
                return Parent.ChildLeft;
            }
        }
        public bool IsLeaf {
            get {
                return ChildLeft is null && ChildRight is null;
            }
        }
        public bool IsLeft {
            get {
                if ( Parent is null || Parent.ChildLeft is null )
                    return false;
                return this == Parent.ChildLeft;
            }
        }
        #endregion

        public AVLNode( TValue value, int height = 0 )
        {
            Value = value;
            Height = height;
            Key = s_key++;
        }
        public AVLNode( int key )
        {
            Key = key;
        }

        public AVLNode( AVLNodeData<TValue> data )
        {
            Key = data.Key;
            Value = data.Value;
            Height = data.Height;
        }

        public AVLNode( ) { }

        public void Copy( AVLNode<TValue> node )
        {
            Parent = node.Parent;
            ChildLeft = node.ChildLeft;
            ChildRight = node.ChildRight;
            Key = node.Key;
            Value = node.Value;
            Height = node.Height;
        }

        public void SetChild( AVLNode<TValue> node )
        {
            if ( node == this )
                throw new ArgumentException(
                    "Can't make child equal to current node");
            if ( node < this ) {
                ChildLeft = node;
            } else {
                ChildRight = node;
            }
            node.Parent = this;
        }

        public void RemoveChildren( )
        {
            ChildLeft = null;
            ChildRight = null;
        }

        public void UpdateHeight( )
        {
            Height = 1 + Math.Max(
                GetLeftHeight(), GetRightHeight());
        }

        public void SwapPositon( AVLNode<TValue> node )
        {
            //AVLNode<TValue>? parent = node.Parent;
            //if ( parent is not null && parent == this ) {
            //    node.Parent = Parent;
            //    node.SetChild(this);
            //    if ( ChildLeft is not null && ChildLeft == node ) {
            //        ChildLeft = null;
            //    } else {
            //        ChildRight = null;
            //    }
            //    return;
            //} else if ( Parent is not null && Parent == node ) {
            //    Parent = node.Parent;
            //    SetChild(node);
            //    if ( node.ChildLeft is not null && node.ChildLeft == this ) {
            //        node.ChildLeft = null;
            //    } else {
            //        node.ChildRight = null;
            //    }
            //    return;
            //}
            //AVLNode<TValue>? childLeft =
            //    node.ChildLeft;
            //AVLNode<TValue>? childRight =
            //    node.ChildRight;
            //parent?.SetChild(this);
            //if ( ChildLeft is null ) {
            //    node.ChildLeft = null;
            //} else {
            //    node.SetChild(ChildLeft);
            //}
            //if ( ChildRight is null ) {
            //    node.ChildRight = null;
            //} else {  
            //    node.SetChild(ChildRight); 
            //}
            //Parent?.SetChild(node);
            //if ( childLeft is null ) {
            //    ChildLeft = null;
            //} else {
            //    SetChild(childLeft);
            //}
            //if ( childRight is null ) {
            //    ChildRight = null;
            //} else {
            //    SetChild(childRight);
            //}
            int key = Key;
            TValue value = Value;
            Key = node.Key;
            Value = node.Value;
            node.Key = key;
            node.Value = value;
        }

        #region Getters
        public int GetBalanceFactor( )
        {
            return GetLeftHeight() - GetRightHeight();
        }

        public int GetLeftHeight( )
        {
            return ChildLeft is null ?
                -1 : ChildLeft.Height;
        }

        public int GetRightHeight( )
        {
            return ChildRight is null ?
                -1 : ChildRight.Height;
        }
        #endregion

        #region Operations
        public bool IsLessThan( AVLNode<TValue> node )
        {
            return Key < node.Key;
        }
        public bool IsGreaterThan( AVLNode<TValue> node )
        {
            return Key > node.Key;
        }
        public bool IsEqual( AVLNode<TValue> node )
        {
            return Key == node.Key;
        }
        public static bool operator <( AVLNode<TValue> node1, AVLNode<TValue> node2 ) =>
            node1.IsLessThan(node2);
        public static bool operator >( AVLNode<TValue> node1, AVLNode<TValue> node2 ) =>
            node1.IsGreaterThan(node2);
        public static bool operator ==( AVLNode<TValue> node1, AVLNode<TValue> node2 ) =>
            node1.IsEqual(node2);
        public static bool operator !=( AVLNode<TValue> node1, AVLNode<TValue> node2 ) =>
            !node1.IsEqual(node2);
        #endregion

        public void Dispose( )
        {
            Parent = null;
            ChildLeft?.Dispose();
            ChildLeft = null;
            ChildRight?.Dispose();
            ChildRight = null;
            GC.SuppressFinalize(this);
        }
    }
}
