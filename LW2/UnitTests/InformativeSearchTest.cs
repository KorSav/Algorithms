using SearchInSetOfStates.Informative;
using NQueensProblem;

namespace UnitTests {
    public class HeuristicsTest {
        [Fact]
        public void test1() {
            InfSearchNode node = new(Reader.State(
                "|Q| | | |" +
                "| |Q| | |" +
                "| | |Q| |" +
                "| | | |Q|"));
            Assert.Equal(3, Heuristic.F1(node.State));
        }

        [Fact]
        public void test2() {
            InfSearchNode node = new(Reader.State(
                "|Q| | | |" +
                "| | | |Q|" +
                "| |Q| | |" +
                "| | |Q| |"));
            Assert.Equal(1, Heuristic.F1(node.State));
        
        }

        [Fact]
        public void test3() {
            InfSearchNode node = new(Reader.State(
                "| |Q| | |" +
                "| | |Q| |" +
                "| | | |Q|" +
                "|Q| | | |"));
            Assert.Equal(3, Heuristic.F1(node.State));

        }
    }
}
