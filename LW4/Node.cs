using GraphX.Common.Models;

namespace ClassicBee_GraphColoring
{
    public class Node : VertexBase
    {
        public HashSet<Node> Neighbors {
            get; set;
        }
        public int Power { get => Neighbors.Count; }

        public Node( )
        {
            Neighbors = [];
        }

        public override int GetHashCode( )
        {
            return ID.GetHashCode();
        }

        public override bool Equals( object? obj )
        {
            if ( obj is not Node node )
                return false;
            return node.ID == ID;
        }

        public string ToStringWithNeighbors( )
        {
            string res = "";
            res += $"{ID,3}: ";

            if ( Neighbors.Count == 0 ) {
                res += $"_\n";
                return res;
            }
            foreach ( var neigNode in Neighbors ) {
                res += $"{neigNode.ID,3}, ";
            }
            return res[..^2];
        }


        //public Color GetColor( )
        //{
        //    if ( _nodePaints[_curGraphID] is null )
        //        return Color.LightGray;
        //    var random = new Random(
        //        _nodePaints[_curGraphID].Value * 123456789);
        //    var r = random.Next(256);
        //    var g = random.Next(256);
        //    var b = random.Next(256);
        //    return Color.FromArgb(r, g, b);
        //}

        public override string ToString( )
        {
            return ID.ToString();
        }
    }
}
