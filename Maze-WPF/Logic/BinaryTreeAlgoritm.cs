using Global;
using Logica;
using System.Drawing;

namespace Logic {
    public class BinaryTreeAlgoritm {
        public Maze maze;
        private bool isDone = false;
        private static readonly Random random = new Random();
        private List<Cell> oldVisitedCells = new List<Cell>();
        private List<Cell> newVisitedCells = new List<Cell>();
        private Cell stop;
        private Cell start;

        public BinaryTreeAlgoritm(Maze maze) {
            this.maze = maze;
        }

        public void Generate() {
            while (!isDone) {
                SetAllWalls();
                GenerateCells(100);
                GenerateBorder();
                start = CalculateStartStop(1);
                stop = CalculateStartStop(maze.length - 2);
                SolveMaze(start);
            }
            if (isDone) {
                SetWalls();
            }
        }

        /// <summary>
        /// Generate Maze cel by cel
        /// </summary>
        /// <param name="placepercentage"></param>
        private void GenerateCells(int placepercentage) {
            for (int y = 0; y < maze.height; y++) {
                for (int x = 0; x < maze.length; x++) {
                    if (y % 2 == 0 && x % 2 == 0) {
                        FillCell(x, y);
                    }
                    else {
                        if (GetRandomNumber(placepercentage) >= 49) {
                            if (x < maze.length - 1) {
                                FillCell(x + 1, y);
                            }
                        }
                        else {
                            if (y < maze.height - 1) {
                                FillCell(x, y + 1);
                            }
                        }
                    }


                }
            }
        }

        private void SetAllWalls() {
            foreach (Cell cell in maze.cels) {
                maze.cels[cell.x, cell.y].isWall = true;
            }
        }

        /// <summary>
        /// Return a random number
        /// </summary>
        /// <param name="placepercentage"></param>
        /// <returns></returns>
        private int GetRandomNumber(int placepercentage) {
            int randomInt = random.Next(placepercentage);
            return randomInt;
        }


        /// <summary>
        /// Mark cell as visited and color it
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        private void FillCell(int x, int y) {
            maze.cels[x, y].color = Color.Black;
            maze.cels[x, y].isWall = false;
        }



        /// <summary>
        /// Generate borders
        /// </summary>
        private void GenerateBorder() {
            for (int i = 0; i < maze.length; i++) {
                maze.cels[i, 0].color = Color.Red;
                maze.cels[i, 0].isWall = true;
                maze.cels[i, maze.length - 1].color = Color.Red;
                maze.cels[i, maze.length - 1].isWall = true;
            }

            for (int j = 0; j < maze.height - 1; j++) {
                maze.cels[0, j].color = Color.Red;
                maze.cels[0, j].isWall = true;
                maze.cels[maze.height - 1, j].color = Color.Red;
                maze.cels[maze.height - 1, j].isWall = true;
            }
        }

        /// <summary>
        /// Find a usable cell in a row for the start or the stop of the maze.
        /// </summary>
        /// <param name="x"></param>
        private Cell CalculateStartStop(int x) {
            Cell cel = new Cell();
            if (x >= 0 && x < maze.length) {
                for (int i = 0; i < maze.length; i++) {
                    Random random = new Random();
                    int randomY = random.Next(0, maze.length);
                    if (maze.cels[x, randomY].color == Color.Black) {
                        if (x == 1) {
                            maze.ball = new Ball();
                            maze.ball.SetLocation(x, randomY);
                        }
                        else {
                            maze.cels[x, randomY].color = Color.Blue;
                        }
                        maze.cels[x, randomY].visited = true;
                        maze.cels[x, randomY].isWall = false;
                        cel.setLocation(x, randomY);
                        cel.id = maze.cels[x, randomY].id;
                        break;
                    }
                }
            }
            return cel;
        }


        /// <summary>
        /// Solve the maze with an algoritm to see if its solvable
        /// </summary>
        /// <param name="start"></param>
        private void SolveMaze(Cell start) {
            bool solved = false;
            oldVisitedCells.Add(start);
            while (!solved && !isDone) {
                solved = true;
                foreach (Cell cell in oldVisitedCells.ToList()) {
                    if (FillInAvailableNeighbours(cell.x, cell.y)) {
                        if (!(cell.color == Color.Red)) {
                            solved = false;

                        }
                    }
                }
                oldVisitedCells = newVisitedCells.ToList();
                newVisitedCells.Clear();

            }

        }

        /// <summary>
        /// See if the neighbouring cells are available
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns></returns>
        private bool FillInAvailableNeighbours(int x, int y) {
            bool isOk = false;
            if (!IsVisited(x + 1, y)) {
                SetVisited(x + 1, y);
                isOk = true;
            }
            if (!IsVisited(x - 1, y)) {
                SetVisited(x - 1, y);
                isOk = true;
            }
            if (!IsVisited(x, y + 1)) {
                SetVisited(x, y + 1);
                isOk = true;
            }
            if (!IsVisited(x, y - 1)) {
                SetVisited(x, y - 1);
                isOk = true;
            }

            return (isOk);

        }

        /// <summary>
        /// Return if the cell has been visited
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns></returns>
        private bool IsVisited(int x, int y) {
            if (maze.cels[x, y].id == stop.id) {
                isDone = true;
            }
            if (x >= 0 && x < maze.length && y >= 0 && y < maze.height) {
                if (maze.cels[x, y].isWall == false) {
                    return maze.cels[x, y].visited;
                }
                else return true;
            }
            else if (x == -1 || y == -1 || x == maze.length || y == maze.height) {
                return true;
            }
            else {
                return true;
            }
        }

        /// <summary>
        /// Set the cell as visited to prevent index array errors
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        private void SetVisited(int x, int y) {
            Cell cell = maze.cels[x, y];
            cell.setLocation(x, y);
            if (cell.isWall == false && cell.visited == false) {
                cell.visited = true;
                newVisitedCells.Add(cell);
            }

        }

        /// <summary>
        /// Not too proud of this fix
        /// </summary>
        private void SetWalls() {
            for (int x = 0; x < maze.length; x++) {
                for (int y = 0; y < maze.height; y++) {
                    if (maze.cels[x, y].color == Color.Red) {
                        maze.cels[x, y].isWall = true;
                    }
                }
            }

        }
    }
}
