using System;

namespace DataStructures
{
    internal class TreeRotator<TValue>
    {
        public AVLNode<TValue> PivotNode { get; set; }

        public TreeRotator( AVLNode<TValue> pivotNode )
        {
            PivotNode = pivotNode;
        }

        public AVLNode<TValue>? RotateRight( )
        {
            if ( PivotNode.ChildLeft is null ) {
                throw new InvalidOperationException(
                    $"Inappropraite use of rotate right:\n" +
                    $"node {PivotNode} does not have left child"
                    );
            }

            AVLNode<TValue>? parent = PivotNode.Parent;
            AVLNode<TValue>? son = PivotNode.ChildLeft;
            AVLNode<TValue>? grandson = son.ChildRight;

            PivotNode.ChildLeft = grandson;
            son.ChildRight = PivotNode;

            if ( grandson is not null )
                grandson.Parent = PivotNode;
            PivotNode.Parent = son;
            son.Parent = parent;

            if ( parent is null )
                return son;
            parent.SetChild(son);
            return null;
        }

        public AVLNode<TValue>? RotateLeft( )
        {
            if ( PivotNode.ChildRight is null ) {
                throw new InvalidOperationException(
                    $"Inappropraite use of rotate left:\n" +
                    $"node {PivotNode} does not have right child"
                    );
            }

            AVLNode<TValue>? parent = PivotNode.Parent;
            AVLNode<TValue>? son = PivotNode.ChildRight;
            AVLNode<TValue>? grandson = son.ChildLeft;

            PivotNode.ChildRight = grandson;
            son.ChildLeft = PivotNode;

            if ( grandson is not null )
                grandson.Parent = PivotNode;
            PivotNode.Parent = son;
            son.Parent = parent;

            if ( parent is null )
                return son;
            parent.SetChild(son);
            return null;
        }

        public AVLNode<TValue>? RotateRightLeft( )
        {
            if ( PivotNode.ChildRight is null ) {
                throw new InvalidOperationException(
                    $"Inappropraite use of rotate right-left:\n" +
                    $"node {PivotNode} does not have right child"
                    );
            }
            TreeRotator<TValue> rotator = new(PivotNode.ChildRight);
            rotator.RotateRight();
            return RotateLeft();
        }

        public AVLNode<TValue>? RotateLeftRight( )
        {
            if ( PivotNode.ChildLeft is null ) {
                throw new InvalidOperationException(
                    $"Inappropraite use of rotate left-right:\n" +
                    $"node {PivotNode} does not have left child"
                    );
            }
            TreeRotator<TValue> rotator = new(PivotNode.ChildLeft);
            rotator.RotateLeft();
            return RotateRight();
        }
    }
}
