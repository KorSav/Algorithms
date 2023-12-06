using DataStructures.Converters;
using System;
using System.Collections;
using System.Collections.Generic;

namespace DataStructures
{
    public class AVLTree<TValue> : IDisposable, IEnumerable<AVLNode<TValue>> where TValue : class
    {
        public AVLNode<TValue>? Root { get; set; }

        public AVLTree( ) { }

        public AVLTree( TValue value )
        {
            Root = new AVLNode<TValue>(value);
        }

        public void Add( TValue value, int? key = null )
        {
            AVLNode<TValue> nodeNew;
            if ( key is not null ) {
                nodeNew = new((int) key) {
                    Value = value
                };
            } else {
                nodeNew = new(value);
            }
            if ( Root is null ) {
                Root = nodeNew;
                return;
            }
            if (FindNode(nodeNew, out var nodeTmp) is not null)
                throw new ArgumentException(
                    $"AVLNode with key = {nodeNew.Key} already exists in tree.");
            nodeTmp!.SetChild(nodeNew);
            int nodeBFactor;
            while ( nodeTmp is not null ) {
                nodeTmp.UpdateHeight();
                nodeBFactor = nodeTmp.GetBalanceFactor();
                if ( nodeBFactor == 2 ) {
                    BalanceLeftSubtree(nodeTmp);
                } else if ( nodeBFactor == -2 ) {
                    BalanceRightSubtree(nodeTmp);
                }
                nodeTmp = nodeTmp.Parent;
            }
        }

        public void Edit( int key, TValue value )
        {
            var node = FindNode(new(key)) ??
                throw new ArgumentException(
                    $"Can't edit curNode with keyDel = {key}, " +
                    $"because it is not in tree.");
            node.Value = value;
        }

        public void Remove( int keyDel )
        {
            if ( Root is null ) {
                throw new ArgumentException("The tree is empty.");
            }
            var nodeToDel = FindNode(new(keyDel)) ??
                throw new ArgumentException(
                    $"There is no AVLNode with key = {keyDel} in tree.");
            do {
                AVLNode<TValue> nodeClosest;
                nodeClosest = GetClosest(nodeToDel);
                nodeClosest.SwapPositon(nodeToDel);
                nodeToDel = nodeClosest;
            } while ( nodeToDel.IsLeaf is false );

            if (nodeToDel.Parent is null ) {
                Root = null;
                return;
            }
            if (nodeToDel.IsLeft) {
                nodeToDel.Parent.ChildLeft = null;
            } else {
                nodeToDel.Parent.ChildRight = null;
            }
            AVLNode<TValue>? nodeTmp = nodeToDel.Parent;
            int nodeBFactor;
            nodeToDel.Parent = null;
            while(nodeTmp is not null ) {
                nodeTmp.UpdateHeight();
                nodeBFactor = nodeTmp.GetBalanceFactor();
                if ( nodeBFactor == 2 ) {
                    BalanceLeftSubtree(nodeTmp);
                } else if ( nodeBFactor == -2 ) {
                    BalanceRightSubtree(nodeTmp);
                }
                nodeTmp = nodeTmp.Parent;
            }
        }

        public TValue? Find( int key, out int comparesCount )
        {
            comparesCount = 0;
            var nodeRes = Root;
            AVLNode<TValue> nodeToFind = new(key);
            while ( nodeRes is not null ) {
                comparesCount++;
                if ( nodeRes == nodeToFind ) {
                    break;
                } else if ( nodeToFind < nodeRes ) {
                    nodeRes = nodeRes.ChildLeft;
                } else if ( nodeToFind > nodeRes ) {
                    nodeRes = nodeRes.ChildRight;
                }
            }
            return nodeRes?.Value;
        }

        #region HelpMethods
        private AVLNode<TValue>? FindNode( AVLNode<TValue> nodeToFind)
        {
            AVLNode<TValue>? nodeRes = Root;
            while ( nodeRes is not null) {
                if ( nodeRes == nodeToFind ) {
                    break;
                } else if ( nodeToFind < nodeRes ) {
                    nodeRes = nodeRes.ChildLeft;
                }
                else if ( nodeToFind > nodeRes ) {
                    nodeRes = nodeRes.ChildRight;
                }
            }
            return nodeRes;
        }

        private AVLNode<TValue>? FindNode( AVLNode<TValue> nodeToFind,
            out AVLNode<TValue>? nodeParent )
        {
            AVLNode<TValue>? nodeRes = Root;
            nodeParent = null;
            while ( nodeRes is not null ) {
                nodeParent = nodeRes;
                if ( nodeRes == nodeToFind ) {
                    break;
                } else if ( nodeToFind < nodeRes ) {
                    nodeRes = nodeRes.ChildLeft;
                } else if ( nodeToFind > nodeRes ) {
                    nodeRes = nodeRes.ChildRight;
                }
            }
            return nodeRes;
        }

        private AVLNode<TValue> GetClosest( AVLNode<TValue> node)
        {
            bool isInLeft = node.GetBalanceFactor() == 1;
            if (node.GetBalanceFactor() == 0 ) {
                var nodeLeftClose = node.ChildLeft;
                var nodeRightClose = node.ChildRight;
                while ( nodeLeftClose!.ChildRight is not null ) {
                    nodeLeftClose = nodeLeftClose.ChildRight;
                }
                while ( nodeRightClose!.ChildLeft is not null ) {
                    nodeRightClose = nodeRightClose.ChildLeft;
                }
                if ( node.Key - nodeLeftClose.Key > nodeRightClose.Key - node.Key ) {
                    isInLeft = false;
                } else {
                    isInLeft = true;
                }
            }
            AVLNode<TValue> nodeRes;
            if (isInLeft ) {
                nodeRes = node.ChildLeft!;
                while ( nodeRes.ChildRight is not null ) {
                    nodeRes = nodeRes.ChildRight;
                }
            } else {
                nodeRes = node.ChildRight!;
                while ( nodeRes.ChildLeft is not null ) {
                    nodeRes = nodeRes.ChildLeft;
                }
            }
            return nodeRes;
        }

        private void BalanceLeftSubtree( AVLNode<TValue> node )
        {
            TreeRotator<TValue> rotator = new(node);
            if ( node.ChildLeft!.GetBalanceFactor() == -1 ) {
                Root = rotator.RotateLeftRight() ?? Root;
                node.UpdateHeight();
                node.Sibling!.UpdateHeight();
                node.Parent!.UpdateHeight();
            } else {
                Root = rotator.RotateRight() ?? Root;
                node.UpdateHeight();
                node.Parent!.UpdateHeight();
            }
        }

        private void BalanceRightSubtree( AVLNode<TValue> node )
        {
            TreeRotator<TValue> rotator = new(node);
            if ( node.ChildRight!.GetBalanceFactor() == 1 ) {
                Root = rotator.RotateRightLeft() ?? Root;
                node.UpdateHeight();
                node.Sibling!.UpdateHeight();
                node.Parent!.UpdateHeight();
            } else {
                Root = rotator.RotateLeft() ?? Root;
                node.UpdateHeight();
                node.Parent!.UpdateHeight();
            }
        }
        #endregion

        public override string ToString( )
        {
            if ( Root is null )
                return string.Empty;
            TreeConverter<TValue> converter = new(Root);
            return converter.ToString();
        }

        public void Dispose( )
        {
            if ( Root is null ) {
                GC.SuppressFinalize(this);
                return;
            }
            Root.Dispose();
            Root = null;
            GC.SuppressFinalize(this);
        }

        public IEnumerator<AVLNode<TValue>> GetEnumerator( )
        {
            Queue<AVLNode<TValue>> queue = new();
            AVLNode<TValue>? curNode;
            queue.Enqueue(Root);
            while ( queue.Count > 0 ) {
                curNode = queue.Dequeue();
                yield return curNode;
                if ( curNode.ChildLeft is not null ) {
                    queue.Enqueue(curNode.ChildLeft);
                }
                if ( curNode.ChildRight is not null ) {
                    queue.Enqueue(curNode.ChildRight);
                }
            }
        }

        public int Count {
            get {
                if ( Root is null )
                    return 0;
                int res = 0;
                Queue<AVLNode<TValue>> queue = new();
                AVLNode<TValue>? curNode;
                queue.Enqueue(Root);
                while ( queue.Count > 0 ) {
                    curNode = queue.Dequeue();
                    res++;
                    if ( curNode.ChildLeft is not null ) {
                        queue.Enqueue(curNode.ChildLeft);
                    }
                    if ( curNode.ChildRight is not null ) {
                        queue.Enqueue(curNode.ChildRight);
                    }
                }
                return res;
            }
        }

        IEnumerator IEnumerable.GetEnumerator( )
        {
            return GetEnumerator();
        }
    }
}
