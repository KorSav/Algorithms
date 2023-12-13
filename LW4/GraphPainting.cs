using System.Collections;

namespace ClassicBee_GraphColoring
{
    public class GraphPainting : IEnumerable<NodePaint>
    {
        public Graph GBase { get; set; }
        public Dictionary<Node, NodePaint> _nodePaints;
        public HashSet<int> _usedPaints;

        public int Count { get => _nodePaints.Count; }
        public int ChromaticNumber {
            get {
                return _usedPaints.Count;
            }
        }
        public NodePaint this[Node node] {
            get => _nodePaints[node];
            private set => _nodePaints[node] = value;
        }
        public GraphPainting( Graph group )
        {
            GBase = group;
            _nodePaints = [];
            foreach ( var node in GBase.NodesDict.Values ) {
                _nodePaints[node] = new(node);
            }
            _usedPaints = [];
        }

        public List<Node> GetUncoloredNodes( )
        {
            List<Node> uncolored = [];
            foreach ( var nodePaint in _nodePaints.Values ) {
                if ( nodePaint.PaintId is null ) {
                    uncolored.Add(nodePaint.Node);
                }
            }
            return uncolored;
        }

        public void UnpaintAll( )
        {
            foreach ( var nodePaint in _nodePaints.Values ) {
                nodePaint.PaintId = null;
            }
            _usedPaints.Clear();
        }
        public int? PaintOf( Node node )
        {
            return _nodePaints[node].PaintId;
        }
        public bool SetPaint( Node node, int? paint = null )
        {
            if ( paint is null ) {
                _nodePaints[node].PaintId = null;
                CalculateUsedPaints();
                return true;
            }
            if ( IsPaintPossible(new(node, paint)) ) {
                _nodePaints[node].PaintId = paint;
                CalculateUsedPaints();
                return true;
            }
            return false;
        }

        public void SetBestPaint( Node node )
        {
            _nodePaints[node].PaintId = FindBestPaint(node);
            CalculateUsedPaints();
        }

        public int FindBestPaint( Node node )
        {
            foreach ( var paint in _usedPaints.Order() ) {
                if ( IsPaintPossible(new(node, paint)) ) {
                    return paint;
                }
            }
            for ( int paint = 0; ; paint++ )
                if ( IsPaintPossible(new(node, paint)) ) {
                    return paint;
                }
        }

        private bool IsPaintPossible( NodePaint nodePaint )
        {
            foreach ( var nodeNeig in nodePaint.Node.Neighbors )
                if ( _nodePaints[nodeNeig].PaintId == nodePaint.PaintId )
                    return false;
            return true;
        }

        public bool IsPaintPossible( Node node, int? paint )
        {
            if ( paint is null ) { return true; }
            return IsPaintPossible(new(node, paint));
        }

        public List<NodePaint> GetNodeNeigbors( Node node )
        {
            List<NodePaint> result = [];
            foreach ( var nodeNeig in node.Neighbors ) {
                result.Add(_nodePaints[nodeNeig]);
            }
            return result;
        }

        public void Copy( GraphPainting graph )
        {
            foreach ( var kvp in graph._nodePaints ) {
                NodePaint np = new(kvp.Key, kvp.Value.PaintId);
                _nodePaints[kvp.Key] = np;
                if ( np.PaintId is not null )
                    _usedPaints.Add(np.PaintId.Value);
            }
        }

        private void CalculateUsedPaints( )
        {
            _usedPaints.Clear();
            foreach ( var kvp in _nodePaints ) {
                var np = kvp.Value;
                if ( np.PaintId is not null )
                    _usedPaints.Add(np.PaintId.Value);
            }
        }

        public string ToStringPaints( )
        {
            string res = "";
            foreach ( var nodePaint in _nodePaints.OrderBy(( np ) => np.Value.Node.ID).ToList() ) {
                if ( nodePaint.Value.PaintId is null ) {
                    res += $"_ ";
                } else {
                    res += $"{nodePaint.Value.PaintId} ";
                }
            }
            return res;
        }

        public override string ToString( )
        {
            string res = "";
            foreach ( var nodePaint in _nodePaints.Values ) {
                if ( nodePaint.PaintId is null ) {
                    res += $"(c- ) ";
                } else {
                    res += $"(c-{nodePaint.PaintId}) ";
                }
                res += $"{nodePaint.Node.ToStringWithNeighbors()}\n";
            }
            return res;
        }

        public IEnumerator<NodePaint> GetEnumerator( )
        {
            foreach ( var nodePaint in _nodePaints.Values ) {
                yield return nodePaint;
            }
        }

        IEnumerator IEnumerable.GetEnumerator( )
        {
            throw new NotImplementedException();
        }

    }
}
