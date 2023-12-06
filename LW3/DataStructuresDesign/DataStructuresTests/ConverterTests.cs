using DataStructures;

namespace DataStructuresTests
{
    public class ConverterTests
    {
        [Fact]
        public void IsCorrect_ToListConvertion( )
        {
            // Arrange
            var tree = new AVLTree<int>();
            var input = new List<int> {
                 4,
              2,    5,
             1, 3,
            };
            var expected = new List<int> {
                4, 2, 5, 1, 3,
            };

            // Act
            foreach ( var num in input )
                tree.Add(num, num);
            TreeConverter<int> tConv = new(tree.Root!);
            var nodesList = tConv.ToList();
            var actual = from node in nodesList
                         select node.Key;

            // Assert
            Assert.Equal(expected, actual);
        }

        [Fact]
        public void IsCorrect_FromListConvertion( )
        {
            // Arrange
            var treeBefore = new AVLTree<int>();
            var treeAfter = new AVLTree<int>();
            var treeExpected = new AVLTree<int>();
            var treeBeforeList = new List<AVLNodeData<int>>();
            var treeAfterList = new List<AVLNodeData<int>>();
            var treeList = new List<int> {
                 4,
              2,    5,
             1, 3,
            };

            // Act
            foreach ( var num in treeList )
                treeBefore.Add(num, num);
            TreeConverter<int> tConv = new(treeBefore.Root!);
            treeBeforeList = tConv.ToList();
            tConv = new();
            treeAfter = tConv.FromList(treeBeforeList);
            tConv = new(treeAfter.Root!);

            // Assert
            var expected = treeList;
            var actual = from n in tConv.ToList()
                         select n.Key;
            Assert.Equal(expected, actual);
        }
    }
}
