using NQueensProblem;

namespace UnitTests {
    public static class Reader {
        public static State State( string strPlacement ) {
            strPlacement = strPlacement.Replace("|", "").ToLower();
            int n = strPlacement.Length;
            int side;
            for ( side = 1; side * side != n; side++ )
                ;
            int byteArrIndex = 0;
            byte[] bytePlacement = new byte[side];
            int i = strPlacement.IndexOf('q');
            do {
                bytePlacement[byteArrIndex++] =
                    Convert.ToByte(i % side);
                i = strPlacement.IndexOf('q', i + 1);
            } while ( i != -1 );
            return new State(bytePlacement);
        }

        public static (Queen, Queen) Queens( string placement ) {
            placement = placement.Replace("|", "").ToLower();
            int n = placement.Length;
            int side;
            for ( side = 1; side * side != n; side++ ) ;
            int i1 = placement.IndexOf('q');
            int i2 = placement.IndexOf('q', i1 + 1);
            Queen q1 = new(i1 % side, i1 / side);
            Queen q2 = new(i2 % side, i2 / side);
            return (q1, q2);
        }
    }
    public class QueenTests {
        [Theory]
        [InlineData(
            "|Q| | |"+
            "| | | |"+
            "| | |Q|", 
            true)]
        [InlineData(
            "|Q| | |" +
            "| | | |" +
            "|Q| | |",
            true)]
        [InlineData(
            "| |Q|Q|" +
            "| | | |" +
            "| | | |",
            true)]
        [InlineData(
            "|Q| | |" +
            "| | |Q|" +
            "| | | |",
            false)]
        public void BeatTest( 
            string board, bool expected) {
            Queen q1, q2;
            (q1, q2) = Reader.Queens(board);
            Assert.Equal(expected, q1.Beat(q2));
        }

        [Theory]
        [InlineData(
            "|Q| | | |" +
            "| | | | |" +
            "| | | | |" +
            "| | | |Q|",
            1, 1,
            true)]
        [InlineData(
            "| | | | |" +
            "| | | | |" +
            "| | |Q| |" +
            "| | | |Q|",
            0, 0,
            false)]
        [InlineData(
            "| | | | |" +
            "| | | | |" +
            "| | | | |" +
            "| |Q| |Q|",
            1, 1,
            false)]
        [InlineData(
            "| | | | |" +
            "| |Q| | |" +
            "| | | | |" +
            "| | | |Q|",
            2, 2,
            true)]
        [InlineData(
            "|Q| |Q|" +
            "| | | |" +
            "| | | |",
            1, 0,
            true)]
        [InlineData(
            "|Q| | |" +
            "| | | |" +
            "|Q| | |",
            0, 1,
            true)]
        public void BetweenTest_ValidInput(
            string board, int x, int y, bool expected ) {
            Queen q1, q2, q3 = new(x, y);
            (q1, q2) = Reader.Queens(board);
            Assert.Equal(expected, q3.IsBetween(q1, q2));
        }

        [Theory]
        [InlineData(
            "|Q| | |" +
            "| | | |" +
            "| | |Q|",
            0, 0)]
        [InlineData(
            "|Q| | |" +
            "| | |Q|" +
            "| | | |",
            1, 1)]
        [InlineData(
            "|Q| |Q|" +
            "| | | |" +
            "| | | |",
            2, 0)]
        public void BetweenTest_InvalidInput(
            string board, int x, int y) {
            Queen q1, q2, q3 = new(x, y);
            (q1, q2) = Reader.Queens(board);
            Assert.Throws<ArgumentException>(()=>q3.IsBetween(q1, q2));
        }
    }
}