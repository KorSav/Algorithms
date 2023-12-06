using SearchInSetOfStates;

namespace NQueensProblem.Visualization {

    static class StringConverter {
        public static string Convert( Node node, bool highlightAction = false ) {
            int boardWidth = node.State.Placement.Length;
            Delimeter delim = new(boardWidth);
            string res = new("\n");
            string cell;
            res += $"{delim.Head}\n|   |";
            foreach ( char c in Enumerable.Range('a', boardWidth) ) {
                res += $" {c} " + '|';
            }
            res += "\n";
            for ( int irow = 0; irow < boardWidth; irow++ ) {
                res += $"{delim.Middle}\n|{irow+1, -3}|";
                for ( int icol = 0; icol < boardWidth; icol++ ) {
                    cell = ( node.State.Placement[irow] == icol ) ?
                        " Q " : "   ";
                    if ( highlightAction && node.Action != null ) {
                        if ( irow == node.Action.FirstIndex ||
                            irow == node.Action.SecondIndex ) {
                            cell = ( node.State.Placement[irow] == icol ) ?
                                "*Q*" : "   ";
                        }
                    }
                    res += cell + '|';
                }
                res += "\n";
            }
            res += delim.Foot + "\n";
            return res;
        }

        public static string Convert( State state) {
            int boardWidth = state.Placement.Length;
            Delimeter delim = new(boardWidth);
            string res = new("\n");
            string cell;
            res += $"{delim.Head}\n|   |";
            foreach ( char c in Enumerable.Range('a', boardWidth) ) {
                res += $" {c} " + '|';
            }
            res += "\n";
            for ( int irow = 0; irow < boardWidth; irow++ ) {
                res += $"{delim.Middle}\n|{irow + 1,-3}|";
                for ( int icol = 0; icol < boardWidth; icol++ ) {
                    cell = ( state.Placement[irow] == icol ) ?
                        " Q " : "   ";
                    res += cell + '|';
                }
                res += "\n";
            }
            res += delim.Foot + "\n";
            return res;
        }
    }
   internal class Delimeter {
            readonly int boardWidth;
            public Delimeter( int boardWidth ) {
                this.boardWidth = boardWidth;
            }

            public string Head {
                get {
                    return
                       "┌───" +
                       string.Join("", Enumerable.
                              Repeat("┬───", boardWidth)) +
                       "┐";
                }
            }
            public string Middle {
                get {
                    return
                       "├───" +
                        string.Join("", Enumerable.
                               Repeat("┼───", boardWidth)) +
                        "┤";
                }
            }
            public string Foot {
                get {
                    return
                       "└───" +
                        string.Join("", Enumerable.
                               Repeat("┴───", boardWidth)) +
                        "┘";
                }
            }
        }
}
