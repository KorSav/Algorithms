using NQueensProblem;

namespace SearchInSetOfStates.Informative {
    class AStar: SearchAlgorithm {
        readonly InfSearchNode initialNode;
        readonly Func<State, int> GetHeuristic;
        public SearchInfo _searchInfo;
        public override SearchInfo Info {
            get {
                return _searchInfo;
            }
        }

        public AStar( State initialState, Func<State, int> heuristic ) {
            initialNode = new(initialState);
            GetHeuristic = heuristic;
            _searchInfo = new();
        }

        public override Node? Search() {
            if ( initialNode == null ) {
                throw new ArgumentException(
                    "Can't search from null initial node");
            }

            PriorityQueue<InfSearchNode, int> open = new();
            HashSet<InfSearchNode> closed = new();
            open.Enqueue(initialNode, initialNode.PathCost + 
                GetHeuristic(initialNode.State));
            InfSearchNode nodeCur;
            InfSearchNode nodeTmp;
            
            do {
                _searchInfo.MemStatesCount =
                    open.Count + closed.Count;
                _searchInfo.IterCount += 1;

                nodeCur = open.Dequeue();
                closed.Add(nodeCur);

                if ( nodeCur.State.IsGoal() ) {
                    open.Clear();
                    closed.Clear();
                    return nodeCur;
                }

                foreach ( Node node in nodeCur.Expand() ) {
                    _searchInfo.GenStatesCount += 1;
                    if ( closed.Contains(node) )
                        continue;
                    nodeTmp = new(node);
                    open.Enqueue( nodeTmp, 
                        nodeTmp.PathCost + GetHeuristic(nodeTmp.State));
                }
                if ( GC.GetTotalMemory(false) > _memlimit ) {
                    open.Clear();
                    closed.Clear();
                    GC.Collect();
                    Info.MemoryOverused = true;
                    break;
                }
            } while ( open.Count != 0 );

            return null;
        }
    }

    public class InfSearchNode : Node {

        public override bool Equals( object? obj ) {
            if ( obj == null || obj.GetType() != GetType() )
                return false;
            InfSearchNode node = (InfSearchNode) obj;
            return
                _state.Equals(node._state) &&
                _depth.Equals(node._depth);
        }

        public override int GetHashCode() {
            return PathCost.GetHashCode();
        }

        public InfSearchNode( State state ) :
            base(state) {
        }
        public InfSearchNode( Node node ) :
            base(node) {
        }
        public InfSearchNode( State state, in Node? parent, int depth ) :
            base(state, parent, depth) {
        }
    }
}
