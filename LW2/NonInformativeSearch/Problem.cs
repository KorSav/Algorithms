namespace NQueensProblem {

    public class State {
        byte[] queenPlacement;
        public State( int boardSize = 8 ) {
            if ( boardSize <= 3 ) {
                throw new ArgumentException(
                    $"{boardSize}-queens problem has no solution!");
            }
            queenPlacement = new byte[boardSize];
        }
        public byte[] Placement {
            get {
                return queenPlacement;
            }
        }
        public State( byte[] queenPlacement) {
            this.queenPlacement = queenPlacement;
        }

        public bool IsGoal() {
            Queen q1;
            Queen q2;

            for ( int i = 0; i < queenPlacement.Length - 1; i++ ) {
                for ( int j = i + 1;  j < queenPlacement.Length; j++ ) {
                    q1 = new(queenPlacement[i], i);
                    q2 = new(queenPlacement[j], j);
                    if ( q1.Beat(q2) )
                        return false;
                }
            }

            return true;
        }
        public State Swap( int i1, int i2 ) {
            if (
                i1 >= queenPlacement.Length || 
                i2 >= queenPlacement.Length ||
                i1 < 0 || i2 < 0 )
                throw new ArgumentException(
                    $"Can't swap {i1} and {i2} positions on board " +
                    $"{queenPlacement.Length}x{queenPlacement.Length}");
            byte[] tmp = new byte[queenPlacement.Length];
            Array.Copy(queenPlacement, tmp, tmp.Length);
            (tmp[i1], tmp[i2]) =
            (tmp[i2], tmp[i1]);

            return new State(tmp);
        }

        public State[] GetSuccessors() {
            int n = queenPlacement.Length;
            State[] states = new State[n * ( n - 1 ) / 2];
            int curResIndex = 0;

            for ( int i = 0; i < n - 1; i++ ) {
                for ( int j = i + 1; j < n; j++ ) {
                    states[curResIndex++] = Swap(i, j);
                }
            }

            return states;
        }

        public void Shuffle() {
            Random rnd = new();
            queenPlacement = queenPlacement.
                OrderBy(i => rnd.Next()).ToArray();
        }

        public State Validate() {
            foreach ( int num in Enumerable.Range
                (0, queenPlacement.Length) ) {
                if ( !queenPlacement.Contains(Convert.ToByte(num)) ) {
                    throw new ArgumentException(
                        "\nInvalid state:\n" +
                        $"[ {string.Join(", ", queenPlacement)} ]\n" +
                        $"Expected {num} to be in state!");
                }
            }
            return this;
        }

        public void PlaceDiagonal() {
            for ( int i = 0; i < queenPlacement.Length; i++ ) {
                queenPlacement[i] = Convert.ToByte(i);
            }
        }

        public override bool Equals( object? obj ) {
            if ( obj == null ) {
                throw new ArgumentException(
                    $"Can't compare null object with State");
            }
            if (obj.GetType() != GetType() ) {
                throw new ArgumentException(
                    $"Can't compare {obj.GetType()} with State");
            }
            State state = (State)obj;
            return queenPlacement.SequenceEqual(state.Placement);
        }
    }
    
    public class Queen {
        byte x, y;

        public Queen( byte x = 0, byte y = 0 ) {
            this.x = x;
            this.y = y;
        }

        public Queen( int x = 0, int y = 0 ) {
            this.x = Convert.ToByte(x);
            this.y = Convert.ToByte(y);
        }

        public Queen( Queen queen ) {
            x = queen.x;
            y = queen.y;
        }

        public bool Beat( Queen queen ) {
            if ( this == queen ) {
                throw new ArgumentException(
                    $"Queen at ({x}; {y}) can’t beat itself!");
            }
            bool result = 
            x == queen.x || y == queen.y || 
            Math.Abs(x - queen.x) == Math.Abs(y - queen.y);
            return result;
        }

        public bool IsEqual( Queen queen ) {
            return ( x == queen.x && y == queen.y );
        }

        public bool IsBetween( Queen queen1, Queen queen2 ) {
            if ( !queen1.Beat( queen2) ) {
                throw new ArgumentException(
                    "Edge queens don't beat each other!");
            }
            if ( queen1.IsEqual(queen2) ) {
                throw new ArgumentException(
                    "The same queens were passed!");
            }
            if ( IsEqual(queen1) || IsEqual(queen2) ) {
                throw new ArgumentException(
                    "Checked queen was passed as argument!");
            }

            int xdiff = queen1.x - queen2.x;
            int ydiff = queen1.y - queen2.y;
            bool result;

            if ( xdiff == 0 ) {
                if ( ydiff > 0 )
                    queen1.Swap(ref queen2);
                result = y > queen1.y && y < queen2.y;
            } else if ( ydiff == 0 ) {
                if ( xdiff > 0 )
                    queen1.Swap(ref queen2);
                result = x > queen1.x && x < queen2.x;
            } else {
                if ( xdiff > 0 )
                    queen1.Swap(ref queen2);
                result = x > queen1.x && x < queen2.x;
                if ( ydiff > 0 )
                    queen1.Swap(ref queen2);
                result = result && y > queen1.y && y < queen2.y;
            }
            return result;
        }

        private void Swap( ref Queen queen ) {
            Queen tmp = new(queen);
            (queen.x, queen.y) = (x, y);
            (x, y) = (tmp.x, tmp.y);
        }
    }

    public class SwapAction {
        readonly byte index1, index2;
        public SwapAction( byte index1, byte index2 ) {
            this.index1 = index1;
            this.index2 = index2;
        }
        public SwapAction( int index1, int index2 ) {
            this.index1 = Convert.ToByte(index1);
            this.index2 = Convert.ToByte(index2);
        }
        public SwapAction( State primaryState, State secondaryState ) {
            if (primaryState.Placement.Length != secondaryState.Placement.Length) {
                throw new ArgumentException("States lengths should be same!");
            }
            
            int[] diff = new int[primaryState.Placement.Length];
            for ( int i = 0; i < primaryState.Placement.Length; i++ ) {
                diff[i] = primaryState.Placement[i] - secondaryState.Placement[i];
            }

            int swapsAmount = diff.Count(num=>num!=0);
            if ( swapsAmount > 2 ) {
                throw new ArgumentException("More than 2 swaps were made!");
            }
            if ( swapsAmount < 2 ) {
                throw new ArgumentException("Less than 2 swaps were made!");
            }

            index1 = Convert.ToByte(Array.FindIndex(diff, n => n != 0));
            index2 = Convert.ToByte(Array.FindIndex(diff, index1 + 1, n => n != 0));
            if ( diff[index1] != -diff[index2]) {
                string arr1 = "[ " + String.Join(", ", primaryState.Placement) + "]";
                string arr2 = "[ " + String.Join(", ", secondaryState.Placement) + "]";
                throw new ArgumentException(
                    $"Wrong swap: {arr1}\n{arr2}\n" +
                    $"Swapped elements indexes: {index1+1}, {index2+1}");
            }
        }
        public int FirstIndex {
            get {
                return index1;
            }
        }
        public int SecondIndex {
            get {
                return index2;
            }
        }
    }

}
