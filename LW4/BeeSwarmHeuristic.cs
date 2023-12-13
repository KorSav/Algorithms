using QuickGraph.Collections;

namespace ClassicBee_GraphColoring
{
    public class BeeSwarmHeuristic
    {
        private int _plcCount;
        private int _beeCount;
        private int _sctBestCount;
        private int _sctRndCount;
        private List<KeyValuePair<int, GraphPainting>> _chosenPlaces;
        private Graph _graphBase;
        private List<Node> _nodesSorted;
        private StreamWriter? _swLog = null;
        private int _iterCount = 0;

        public List<KeyValuePair<int, GraphPainting>> ChosenPlaces { get => _chosenPlaces; }
        public GraphPainting Solution { get; private set; }

        public BeeSwarmHeuristic(
            Graph graphSkelet,
            string? logFileName = null,
            int placeCount = 30,
            int beeCount = 30,
            int scoutBestPlacesCount = 2,
            int scoutRandomPlacesCount = 1 )
        {
            _graphBase = graphSkelet;
            _plcCount = placeCount;
            _beeCount = beeCount;
            _sctBestCount = scoutBestPlacesCount;
            _sctRndCount = scoutRandomPlacesCount;
            _chosenPlaces = [];
            _nodesSorted = _graphBase.GetTopNodesByPower();
            if ( logFileName is not null ) {
                _swLog = new(logFileName);
                _swLog.WriteLine($"Initial graph:\n{_graphBase}");
            }
            Solution = new(_graphBase);
        }

        public void GeneratePlaces( )
        {
            _graphBase.ClearGraphs();
            for ( int i = 0; i < _plcCount; i++ ) {
                _graphBase.AddNewGraph();
                GraphPainter.Paint(_graphBase[i]);
            }
        }

        public void ChoosePlaces( )
        {
            int sctCount = _sctBestCount + _sctRndCount;
            BinaryHeap<int, GraphPainting> heap = new();
            _chosenPlaces.Clear();

            foreach ( var graph in _graphBase.Paintings ) {
                heap.Add(graph.ChromaticNumber, graph);
            }
            for ( int i = 0; i < _sctBestCount; i++ ) {
                _chosenPlaces.Add(heap.RemoveMinimum());
            }

            var remainedPlaces = heap.ToList();
            KeyValuePair<int, GraphPainting> kvp_best;
            int bpIndex;
            KeyValuePair<int, GraphPainting> kvp_cur;
            int rndPlaceIndex;
            Random rnd = new();

            for ( int i = 0; i < _sctRndCount; i++ ) {
                rndPlaceIndex = rnd.Next(0, remainedPlaces.Count);
                kvp_cur = remainedPlaces[rndPlaceIndex];
                kvp_best = kvp_cur;
                bpIndex = rndPlaceIndex;
                for ( int j = 1; j < sctCount; j++ ) {
                    rndPlaceIndex = rnd.Next(0, remainedPlaces.Count);
                    kvp_cur = remainedPlaces[rndPlaceIndex];
                    if ( kvp_cur.Key < kvp_best.Key ) {
                        kvp_best = kvp_cur;
                        bpIndex = rndPlaceIndex;
                    }
                }
                _chosenPlaces.Add(kvp_best);
                remainedPlaces.RemoveAt(bpIndex);
            }
            FindSolution();
            if (_swLog is not null ) {
                _swLog.WriteLine($"\nIteration {++_iterCount}\n" +
                    $"Places evaluated:");
                LogPaintedGraphs();
                _swLog.WriteLine("\nChosen places:");
                foreach ( var kvp in _chosenPlaces ) {
                    var gi = _graphBase.Paintings.IndexOf(kvp.Value) + 1;
                    _swLog.WriteLine($"{gi}, CN={kvp.Key}");
                }
            }
        }

        public void SendFurages( )
        {
            int fbeesLeft = _beeCount - _sctBestCount - _sctRndCount;

            for ( int placeIndex = 0; placeIndex < _chosenPlaces.Count; placeIndex++ ) {
                int indexPlaceInPlaces = _graphBase.Paintings
                    .IndexOf(_chosenPlaces[placeIndex].Value);
                var foundSuburbs = SendFuragesIn(placeIndex, ref fbeesLeft);
                if ( foundSuburbs.Count == 0 ) {
                    continue;
                }
                var kvp_bestSuburb = foundSuburbs.Minimum();
                if ( kvp_bestSuburb.Key < _chosenPlaces[placeIndex].Key ) {
                    SetBetterPlace(kvp_bestSuburb.Value);
                }
            }
            FindSolution();
            if ( _swLog is not null ) {
                _swLog.WriteLine($"\nAfter furages:\n");
                LogPaintedGraphs();
            }
        }

        public void SendScouts( )
        {
            int sctCount = _sctBestCount + _sctRndCount;
            for ( int i = 0; i < sctCount; i++ ) {
                GraphPainting randomGraph = new(_graphBase);
                GraphPainter.PaintGreedy(randomGraph);
                SetBetterPlace(randomGraph);
            }
            FindSolution();
        }

        private void SetBetterPlace( GraphPainting graph )
        {
            int i;
            var gcn = graph.ChromaticNumber;
            GraphPainting curGraph;
            BinaryHeap<int, int> heap = [];

            for ( i = 0; i < _graphBase.Paintings.Count; i++ ) {
                curGraph = _graphBase.Paintings[i];
                heap.Add(-curGraph.ChromaticNumber, i); //it is min heap
            }
            for ( i = 0; i < heap.Count; i++ ) {
                var kvp = heap.RemoveMinimum();
                if ( gcn <= -kvp.Key ) {
                    _graphBase[kvp.Value] = graph;
                    break;
                }
            }
            return;
        }

        private BinaryHeap<int, GraphPainting> SendFuragesIn( int placeIndex, ref int leftBees )
        {
            Random rnd = new();
            int curFitness = _chosenPlaces[placeIndex].Key;
            var place = _chosenPlaces[placeIndex].Value;
            int usedBeesCount;
            int neigIndex = 0;
            int curNodeIndex = 0;
            Node curNode;
            List<NodePaint> curNodeNeigbors;
            List<int> visitedNodes = [];
            bool isPaintPossible;
            BinaryHeap<int, GraphPainting> foundSuburbs = [];

            if ( placeIndex == _chosenPlaces.Count - 1 ) {
                usedBeesCount = leftBees;
            } else {
                usedBeesCount = _nodesSorted[0].Power;
            }

            curNodeIndex = rnd.Next(0, _nodesSorted.Count);
            visitedNodes.Add(curNodeIndex);
            curNode = _nodesSorted[curNodeIndex];
            curNodeNeigbors = place.GetNodeNeigbors(curNode);
            var str1 = "";
            var str2 = "";
            for ( int i = 0; i < usedBeesCount; i++ ) {
                var neigNP = curNodeNeigbors[neigIndex];
                str2 = place.ToStringPaints();
                GraphPainting suburb = new(_graphBase);
                suburb.Copy(place);
                str1 = suburb.ToStringPaints();
                suburb.SetPaint(curNode, null);
                suburb.SetPaint(neigNP.Node, null);
                str1 = suburb.ToStringPaints();

                isPaintPossible =
                    suburb.IsPaintPossible(curNode, neigNP.PaintId) &&
                    suburb.IsPaintPossible(neigNP.Node, place.PaintOf(curNode));

                if ( isPaintPossible ) {
                    suburb.SetPaint(curNode, place.PaintOf(neigNP.Node));
                    suburb.SetBestPaint(neigNP.Node);
                    str1 = suburb.ToStringPaints();
                    foundSuburbs.Add(suburb.ChromaticNumber, suburb);
                }
                if ( ++neigIndex == curNodeNeigbors.Count ) {
                    neigIndex = 0;
                    while ( visitedNodes.Contains(curNodeIndex) )
                        curNodeIndex = rnd.Next(0, _nodesSorted.Count);
                    visitedNodes.Add(curNodeIndex);
                    curNode = _nodesSorted[curNodeIndex];
                    curNodeNeigbors = place.GetNodeNeigbors(curNode);
                }
            }

            leftBees -= usedBeesCount;
            return foundSuburbs;
        }

        public void SaveLogFile( )
        {
            if ( _swLog is null )
                throw new InvalidOperationException("The process was not logging.");
            _swLog.Close();
        }

        private void LogPaintedGraphs( )
        {
            if ( _swLog is null )
                throw new InvalidOperationException("The process was not logging.");
            int gi = 0;
            foreach ( var graph in _graphBase.Paintings ) {
                _swLog.WriteLine($"{++gi} CN={graph.ChromaticNumber}:");
                //_swLog.WriteLine(graph.ToStringPaints());
            }
        }

        private void FindSolution( )
        {
            var glist = _graphBase.Paintings.OrderBy((g)=>g.ChromaticNumber).ToList();
            Solution = glist[0];
        }

    }
}
