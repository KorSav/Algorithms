namespace ClassicBee_GraphColoring
{
    public static class GraphPainter
    {
        public static void PaintGreedy( in GraphPainting graph )
        {
            graph.UnpaintAll();
            Random rnd = new();
            for ( int i = 0; i < graph.Count; i++ ) {
                var uncoloredNodes = graph.GetUncoloredNodes();
                var nodeToColor = uncoloredNodes
                    [rnd.Next(0, uncoloredNodes.Count)];
                graph.SetBestPaint(nodeToColor);
            }
        }

        public static void Paint( in GraphPainting graph )
        {
            graph.UnpaintAll();
            for ( int i = 0; i < graph.Count; i++ ) {
                graph.SetPaint(graph.GBase.NodesDict[i + 1], i);
            }
        }
    }
}
