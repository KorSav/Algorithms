namespace ClassicBee_GraphColoring
{
    public static class GraphGenerator
    {
        public static void Fill( out Graph graph, int VCount, int EMin, int EMax )
        {
            graph = new();
            var nodes = new Node[VCount];
            for ( int i = 0; i < nodes.Length; i++ ) {
                nodes[i] = new() { ID = i + 1 };
            }
            Random rnd = new();
            Node node;
            int ECount, indexRandNode, indexEdge;
            for ( int indexNode = 0; indexNode < VCount; indexNode++ ) {
                node = nodes[indexNode];
                if ( node.Power != 0 && rnd.Next(1, 3) <= 1 )
                    continue;
                ECount = rnd.Next(EMin, EMax + 1);
                for ( indexEdge = node.Power;
                      indexEdge < ECount; indexEdge++ ) {
                    indexRandNode = rnd.Next(0, VCount - 1);
                    if ( nodes[indexRandNode].Power == EMax ||
                         indexRandNode == indexNode ) {
                        indexEdge--;
                        continue;
                    }
                    graph.BindNodes(node, nodes[indexRandNode]);
                }
            }
        }
    }
}
