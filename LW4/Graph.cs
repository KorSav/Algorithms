using System.Collections;

namespace ClassicBee_GraphColoring
{
    public class Graph : IEnumerable<Node>
    {
        #region Properties
        public Dictionary<long, Node> NodesDict { get; set; }
        public List<GraphPainting> Paintings { get; set; }

        #endregion

        public Graph( )
        {
            NodesDict = [];
            Paintings = [];
        }

        public GraphPainting this[int i] {
            get => Paintings[i];
            set {
                if ( value.GBase != this ) {
                    throw new Exception("Can't insert painting of another graph");
                }
                Paintings[i] = value;
            }
        }

        public List<Node> GetTopNodesByPower( int amountOfTopNodes = -1 )
        {
            if ( amountOfTopNodes < 0 ) {
                amountOfTopNodes = NodesDict.Count;
            }
            var res = NodesDict.Values.ToList();
            res.Sort(
                ( node1, node2 ) => -node1.Power
                    .CompareTo(node2.Power)
            );
            return res.Take(amountOfTopNodes).ToList();
        }

        public void AddNodes( params Node[] nodes )
        {
            foreach ( var node in nodes ) {
                if ( NodesDict.ContainsKey(node.ID) ) {
                    throw new ArgumentException("Can't add node " +
                        $"with id {node.ID}. " +
                        $"It is already in graph!");
                }
                NodesDict[node.ID] = node;
            }
        }

        public void BindNodes( Node node, params Node[] nodes )
        {
            AddNewNode(node);
            foreach ( var nodeToBound in nodes ) {
                node.Neighbors.Add(nodeToBound);
                nodeToBound.Neighbors.Add(node);
                AddNewNode(nodeToBound);
            }
        }

        public void AddNewGraph( )
        {
            Paintings.Add(new(this));
        }

        public void ClearGraphs( )
        {
            Paintings.Clear();
        }

        public override string ToString( )
        {
            string res = "";
            foreach ( var node in NodesDict.Values.OrderBy(( node ) => node.ID) ) {
                res += $"{node.ToStringWithNeighbors()}\n";
            }
            return res;
        }

        private void AddNewNode( Node node )
        {
            if ( NodesDict.ContainsKey(node.ID) ) {
                return;
            }
            NodesDict[node.ID] = node;
        }

        public IEnumerator<Node> GetEnumerator( )
        {
            foreach ( var node in NodesDict.Values ) {
                yield return node;
            }
        }

        IEnumerator IEnumerable.GetEnumerator( )
        {
            throw new NotImplementedException();
        }
    }
}
