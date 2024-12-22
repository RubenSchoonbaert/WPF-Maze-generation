using Global;
using Logica;
using System.Drawing;

namespace Logic {
    public class StaticMazeGenerator {
        public Maze maze;

        public StaticMazeGenerator() {
            maze = new Maze(20, 20);
            maze.GenerateGrid(Color.Black);
            GenerateStaticMaze();
        }

        /// <summary>
        /// First set the border of the maze.
        /// Then call the GenerateWallls() methode to fill in the rest of the maze
        /// &set the start and end cell.
        /// </summary>
        public void GenerateStaticMaze() {
            for (int x = 0; x < maze.length; x++) {
                maze.cels[x, 0].isWall = true;
                maze.cels[x, maze.height - 1].isWall = true;
                maze.cels[x, 0].color = Color.Red;
                maze.cels[x, maze.height - 1].color = Color.Red;
            }
            for (int y = 0; y < maze.height; y++) {
                maze.cels[0, y].isWall = true;
                maze.cels[maze.length - 1, y].isWall = true;
                maze.cels[0, y].color = Color.Red;
                maze.cels[maze.length - 1, y].color = Color.Red;

            }

            // Set the start & stop cell
            maze.ball = new Ball();
            maze.ball.SetLocation(1, 2);
            maze.cels[18, 16].color = Color.Blue;
            maze.endCell = maze.cels[18, 16];
            Cell endCell = new Cell();
            endCell.setLocation(maze.length - 2, maze.height - 2);
            maze.endCell = endCell;

            GenerateWalls();
        }

        /// <summary>
        /// Set the maze using a static list of id's that are walls.
        /// </summary>
        private void GenerateWalls() {
            //list of ids
            List<int> wallIds = new List<int> {
                0, 20, 40, 60, 80, 100, 120, 140, 160, 180,
                200, 220, 240, 260, 280, 300, 320, 340, 360, 380,
                1, 21, 121, 141, 261, 361, 381, 2, 62, 102, 122,
                182, 222, 302, 322, 382, 3, 43, 63, 163, 183, 203,
                223, 263, 283, 303, 323, 343, 383, 4, 64, 84, 104,
                124, 144, 164, 264, 304, 344, 384, 5, 45, 65, 205,
                225, 245, 265, 385, 6, 26, 46, 106, 126, 146, 166,
                186, 206, 226, 266, 286, 326, 346, 366, 386, 7, 87,
                107, 127, 167, 227, 267, 287, 327, 347, 387, 8, 48,
                68, 88, 208, 228, 388, 9, 89, 109, 149, 169, 269,
                309, 349, 369, 389, 10, 50, 90, 170, 190, 210, 230,
                250, 270, 290, 310, 370, 390, 11, 31, 51, 91, 131,
                231, 291, 311, 331, 391, 12, 92, 112, 132, 152, 172,
                192, 212, 232, 272, 292, 372, 392, 13, 33, 73, 93,
                113, 193, 213, 273, 333, 353, 373, 393, 14, 34, 94,
                114, 154, 234, 254, 274, 294, 334, 394, 15, 75, 95,
                155, 175, 195, 215, 235, 255, 335, 355, 395, 16, 56,
                76, 136, 156, 196, 236, 256, 296, 316, 336, 396, 17,
                57, 117, 137, 317, 377, 397, 18, 98, 118, 178, 218,
                258, 278, 358, 378, 398, 19, 39, 59, 79, 99, 119,
                139, 159, 179, 199, 219, 239, 259, 279, 299, 319, 339,
                359, 379, 399
            };
            var wallCells = wallIds.Select(id => new { X = id % maze.length, Y = id / maze.length });

            foreach (var cell in wallCells) {
                int x = cell.X;
                int y = cell.Y;

                if (x > 0 && x < maze.length && y > 0 && y < maze.height) {
                    maze.cels[x, y].isWall = true;
                    maze.cels[x, y].color = Color.Red;
                }
            }
        }

    }
}


