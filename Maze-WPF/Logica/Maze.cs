using Global;
using System.Drawing;

namespace Logica {
    public class Maze {
        public int height;
        public int length;
        public Cell endCell;
        public Cell[,] cels;
        public Ball ball {
            get;
            set;
        }

        public Maze(int height, int length) {
            this.height = height;
            this.length = length;
            cels = new Cell[length, height];
        }

        /// <summary>
        /// Generate empty cell grid
        /// </summary>
        /// <exception cref="Exception"></exception>
        public void GenerateGrid(Color color) {
            int idCount = 0;
            if (height == 0 || length == 0) {
                throw new Exception("Maze size can not be 0!");
            }
            for (int i = 0; i < height; i++) {

                for (int j = 0; j < length; j++) {
                    Cell cell = new Cell(idCount, color);
                    cell.SetDefaultWalls();
                    cels[j, i] = cell;
                    idCount++;
                }
            }

        }

    }
}