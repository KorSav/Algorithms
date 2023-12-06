using System;
using System.Collections.Generic;

namespace DataStructures.Converters
{
    internal class TreeToSring<TValue>
    {
        public AVLNode<TValue> Root { get; set; }
        private readonly bool _isValueDrawn;

        public TreeToSring( AVLNode<TValue> root, bool isValueDrawn )
        {
            Root = root;
            _isValueDrawn = isValueDrawn;
        }

        public string Convert( )
        {
            Queue<AVLNode<TValue>?> queue = new();
            AVLNode<TValue>? curNode;
            int treeHeight = Helper.GetNodeHeightAndMaxSpan(Root, _isValueDrawn, out int spanLen);
            int curLevel = treeHeight - 1;
            int levelNodesCount;
            string stringOutput = "";

            queue.Enqueue(Root);
            while ( queue.Count > 0 ) {
                levelNodesCount = queue.Count;

                while ( levelNodesCount > 0 ) {
                    curNode = queue.Dequeue();
                    stringOutput += Helper.GetBlock(spanLen, curLevel, curNode, _isValueDrawn);
                    if ( levelNodesCount > 1 )
                        stringOutput += Helper.GetShift(spanLen);

                    if ( curNode is not null ) {
                        queue.Enqueue(curNode.ChildLeft);
                        queue.Enqueue(curNode.ChildRight);
                    } else { 
                        queue.Enqueue(null); 
                        queue.Enqueue(null); 
                    }

                    levelNodesCount--;
                }
                stringOutput += '\n';
                if ( --curLevel == -1 )
                    break;
            }
            queue.Clear();

            return stringOutput;
        }

        private static class Helper
        {
            public static int Pow( int number, int power )
            {
                return System.Convert.ToInt32(Math.Pow(number, power));
            }

            public static string GetShift( int spanLen, int level )
            {
                if ( level < 0 ) {
                    throw new ArgumentOutOfRangeException(nameof(level));
                }
                int spanCount = Pow(2, level) - 1;
                return new(' ', spanLen * spanCount);
            }

            public static string GetShift( int spanLen )
            {
                return new(' ', spanLen);
            }

            public static string GetBlock(
                int spanLen, int level, AVLNode<TValue>? node, bool ivd ) =>
                (level, node) switch {
                    (0, null) => GetShift(spanLen),
                    (_, null) => GetShift(spanLen, level) +
                                 GetShift(spanLen) +
                                 GetShift(spanLen, level),
                    (0, _) => ToString(node, ivd).PadLeft(spanLen),
                    (_, _) => GetShift(spanLen, level) +
                              ToString(node, ivd).PadLeft(spanLen) +
                              GetShift(spanLen, level)
                };

            public static int GetNodeHeightAndMaxSpan( AVLNode<TValue> node, bool ivd,
                out int maxSpanLen )
            {
                int resHeight = 0;
                int curSpanLength;
                int levelNodesCount;
                Queue<AVLNode<TValue>> queue = new();
                AVLNode<TValue> curNode;

                maxSpanLen = 0;
                queue.Enqueue(node);
                while ( queue.Count > 0 ) {
                    levelNodesCount = queue.Count;
                    while ( levelNodesCount > 0 ) {
                        curNode = queue.Dequeue();
                        levelNodesCount--;

                        if ( curNode.ChildLeft is not null ) {
                            queue.Enqueue(curNode.ChildLeft);
                        }
                        if ( curNode.ChildRight is not null ) {
                            queue.Enqueue(curNode.ChildRight);
                        }

                        curSpanLength = ToString(curNode, ivd).Length;
                        maxSpanLen = curSpanLength > maxSpanLen ?
                            curSpanLength : maxSpanLen;
                    }
                    resHeight++;
                }
                return resHeight;
            }

            private static string ToString( AVLNode<TValue> node, bool isValueDrawn ) => ( isValueDrawn ) switch {
                ( false ) => $"{node.Key}",
                ( true ) => $"{node.Key}:{node.Value}"
            };

        }
    }
}
