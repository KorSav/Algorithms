using NQueensProblem;
using System.Xml.Linq;

namespace SearchInSetOfStates {
    public class SearchAlgorithm {
        protected int _memlimit;
        public virtual SearchInfo Info { get; }
        public int MemoryLimitMBs {
            set {
                _memlimit = value*1024*1024;
            }
            get {
                return _memlimit / 1024 / 1024;
            }
        }
        public virtual Node? Search() {
            throw new NotImplementedException();
        }
    }

    public class Node {
        protected State _state;
        protected Node? _parent;
        protected int _depth;
        public State State {
            get {
                return _state;
            }
        }
        public Node? Parent {
            get {
                return _parent;
            }
        }
        public SwapAction? Action {
            get {
                if ( _parent == null )
                    return null;
                return new SwapAction(_state, _parent._state);
            }
        }
        public int Depth {
            get {
                return _depth;
            }
        }
        public virtual int PathCost {
            get {
                return _depth;
            }
        }

        public Node( Node node) {
            _state = node._state;
            _parent = node._parent;
            _depth = node._depth;
        }

        public Node( State state ) {
            _state = state.Validate();
            _parent = null;
            _depth = 0;
        }

        public Node( State state, in Node? parent, int depth ) {
            _state = state.Validate();
            _parent = parent;
            _depth = depth;
        }

        public Node[] Expand() {
            State[] successors = _state.GetSuccessors();
            Node[] result = new Node[successors.Length];

            for ( int i = 0; i < successors.Length; i++ ) {
                result[i] = new(
                    successors[i], this, _depth + 1);
            }

            return result;
        }

        public virtual bool Equals( Node? node ) {
            if ( this == null && node == null ) {
                return true;
            }
            if ( this == null || node == null ) {
                return false;
            }
            return _state.Equals(node._state);
        }

        public override string ToString() {
            return $"[ {string.Join(", ", _state.Placement)} ]";
        }
    }

}
