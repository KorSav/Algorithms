using NQueensProblem;

namespace SearchInSetOfStates.NonInformative {

    class IterativeDeepingSearch : SearchAlgorithm {
        Node? initialNode;
        SearchInfo _searchInfo;
        public IterativeDeepingSearch( Node? initialNode = null ) {
            this.initialNode = initialNode;
            _searchInfo = new();
        }
        public IterativeDeepingSearch( State initialState ) {
            initialNode = new(initialState);
            _searchInfo = new();
        }
        public override SearchInfo Info{
            get {
                return _searchInfo;
            }
        }

        public override Node? Search() {
            int depth;
            DepthLimitedSearch DLSalgorithm;
            Node? result;

            for ( depth = 0; ; depth++ ) {
                DLSalgorithm = new(initialNode, depth) {
                    MemoryLimitMBs = MemoryLimitMBs
                };
                result = DLSalgorithm.Search();
                _searchInfo += DLSalgorithm.Info;
                _searchInfo.MemStatesCount = 
                    DLSalgorithm.Info.MemStatesCount;
                if ( DLSalgorithm.Info.MemoryOverused ) {
                    _searchInfo.MemoryOverused = true;
                    break;
                }
                if ( result != null ) {
                    return result;
                }
            }
            return null;
        }
    }

    class DepthLimitedSearch: SearchAlgorithm {
        int depth;
        Node? initialNode;
        SearchInfo _searchInfo;

        public DepthLimitedSearch( Node? initialNode, int depth ) {
            this.initialNode = initialNode;
            this.depth = depth;
            _searchInfo = new();
        }
        public DepthLimitedSearch( State initialState, int depth) {
            initialNode = new(initialState);
            this.depth = depth;
            _searchInfo = new();
        }
        public int Depth {
            set {
                depth = value;
            }
        }
        public override SearchInfo Info {
            get {
                return _searchInfo;
            }
        }

        public override Node? Search() {
            if ( depth < 0 ) {
                throw new ArgumentOutOfRangeException(
                    $"Searching Depth should be > 0, but {depth} got");
            }
            if ( initialNode == null ) {
                throw new ArgumentException(
                    "Can't search from null initial node");
            }
            
            Stack<Node> nodesToCheck = new();
            Node curNode;
            nodesToCheck.Push(initialNode);
            Info.MemStatesCount += 1;

            do {
                Info.IterCount += 1;
                Info.MemStatesCount = nodesToCheck.Count;

                curNode = nodesToCheck.Pop();

                if ( curNode.State.IsGoal() ) {
                    nodesToCheck.Clear();
                    return curNode;
                }

                foreach ( Node node in curNode.Expand() ) {
                    if ( node.Depth <= depth ) {
                        nodesToCheck.Push(node);
                    }
                    Info.GenStatesCount += 1;
                }

                if ( GC.GetTotalMemory(false) > _memlimit ) {
                    nodesToCheck.Clear();
                    GC.Collect();
                    Info.MemoryOverused = true;
                    break;
                }

            } while ( nodesToCheck.Count != 0 );

            return null;
        }

    }

}
