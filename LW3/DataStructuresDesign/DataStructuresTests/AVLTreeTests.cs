using DataStructures;

namespace DataStructuresTests
{
    public class AVLTreeTests
    {
        [Fact]
        public void IsCorrect_LeftRotation( )
        {
            // Arrange
            var tree = new AVLTree<int>();
            var input = new List<int> {
                  1,
                0,   3,
                    2, 4,
                         5,
               };
            var expected = new List<int> {
                   3,
                1,    4,
               0, 2,    5,
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
        public void IsCorrect_RightRotation( )
        {
            // Arrange
            var tree = new AVLTree<int>();
            var input = new List<int> {
                   4,
                2,   5,
              1,  3,
             0,
            };
            var expected = new List<int> {
                2,
              1,  4,
            0,   3, 5,
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
        public void IsCorrect_LeftRightRotation( )
        {
            // Arrange
            var tree = new AVLTree<int>();
            var input = new List<int> {
                   5,
              2,        6,
             1, 3,
                  4,
            };
            var expected = new List<int> {
            /*      5,    */       3, 
            /*    3,   6, */    2,    5,
            /*  2, 4,     */  1,    4,  6, 
            /* 1,         */
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
        public void IsCorrect_RightLeftRotation( )
        {
            // Arrange
            var tree = new AVLTree<int>();
            var input = new List<int> {
                  2,
               1,     5,
                    4,  6,
                   3,
            };
            var expected = new List<int> {
            /*   2,        */       4, 
            /* 1,   4,     */    2,    5, 
            /*     3, 5,   */   1, 3,    6,
            /*          6, */
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
        public void IsCorrect_HeightCalculation( )
        {
            // Arrange
            var tree = new AVLTree<int>();
            var input = new List<int> {
                    5,
                3,     7,
               2, 4,  6, 8,
              1,          9,
            };
            var expected = new List<int> {
                    3,
                2,     2,
               1, 0,  0, 1,
              0,          0,
            };

            // Act
            foreach ( var num in input )
                tree.Add(num, num);
            TreeConverter<int> tConv = new(tree.Root!);
            var nodesList = tConv.ToList();
            var actual = from node in nodesList
                         select node.Height;

            // Assert
            Assert.Equal(expected, actual);
        }

        [Fact]
        public void IsAdded_NodeToEmptyTree( )
        {
            // Arrange
            var tree2 = new AVLTree<int>();
            int value = 1;
            var expected = new List<AVLNodeData<int>> {
                new(0, value, 0)
            };

            // Act
            tree2.Add(value);
            var tConv = new TreeConverter<int>(tree2.Root!);
            var actual = tConv.ToList();

            //Assert
            Assert.Equal(expected, actual);
            //tree2.Dispose();
        }

        [Fact]
        public void ThrowsArgumentException_ifNodeInTree( )
        {
            // Arrange
            var tree1 = new AVLTree<int>();

            // Act
            tree1.Add(1);
            tree1.Add(2);
            tree1.Add(3);

            //Assert
            Assert.Throws<ArgumentException>(( ) => tree1.Add(1, 0));
            //Assert.Equal(1, 1);
        }
    }
}