using System.Drawing;

namespace ClassicBee_GraphColoring
{
    public class NodePaint
    {
        public Node Node { get; set; }
        public int? PaintId { get; set; }
        public NodePaint( Node node, int? paint = null )
        {
            Node = node;
            PaintId = paint;
        }
        public override int GetHashCode( )
        {
            return Node.ID.GetHashCode();
        }

        public override bool Equals( object? obj )
        {
            if ( obj is not NodePaint ndpaint )
                return false;
            return ( ndpaint.Node == Node &&
                ndpaint.PaintId == PaintId );
        }

        public Color GetColor( )
        {
            if ( PaintId is null )
                return Color.LightGray;
            var random = new Random(
                PaintId.Value * 123456789);
            var r = random.Next(256);
            var g = random.Next(256);
            var b = random.Next(256);
            return Color.FromArgb(r, g, b);
        }
    }
}
