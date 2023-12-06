namespace NQueensProblem{
    public static class Heuristic {
        public static int F1( State state ) {
            Queen q1, q2, q3;
            int resValue = 0;
            for ( int i = 0; i < state.Placement.Length - 1; i++ ) {
                for ( int j = i + 1; j < state.Placement.Length; j++ ) {
                    q1 = new(state.Placement[i], i);
                    q2 = new(state.Placement[j], j);
                    if ( q1.Beat(q2) ) {
                        for ( int k = i + 1; k < j; k++ ) {
                            q3 = new(state.Placement[k], k);
                            if ( q3.IsBetween(q1, q2) ) {
                                resValue--;
                                break;
                            }
                        }
                        resValue++;
                    }
                }
            }
            return resValue;
        }
    }
}
