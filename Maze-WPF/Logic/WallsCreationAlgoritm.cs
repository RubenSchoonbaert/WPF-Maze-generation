using Global;
using Logica;
using System.Drawing;

namespace Logic {
    public class WallsCreationAlgoritm {
        public Maze maze;
        public bool isDone = false;
        public int wallpercentage = 25;
        public int destructionpercentage = 15;
        private static readonly Random random = new Random();
        private List<int> Rows = new List<int>();
        private List<int> Columns = new List<int>();
        private List<Cell> oldVisitedCells = new List<Cell>();
        private List<Cell> newVisitedCells = new List<Cell>();
        private Cell stopid;

        public WallsCreationAlgoritm(Maze maze) {
            this.maze = maze;
        }

        public void Generate() {
            CreateWalls(false); // Create walls for columns
            CreateWalls(true); // Create walls for rows
            PlaceWalls();
            GenerateBorder();
            Cell startid = CalculateStartStop(1);
            stopid = CalculateStartStop(maze.length - 2);
            SolveMaze(startid);
            SetWalls();

        }


        /// <summary>
        /// Source: https://learn.microsoft.com/en-us/dotnet/csharp/language-reference/operators/conditional-operator
        /// </summary>
        /// <param name="forRows"></param>
        private void CreateWalls(bool forRows) {
            int rowCount = maze.height; //rows
            int colCount = maze.length; //columns

            for (int i = 2; i < (forRows ? rowCount - 2 : colCount - 2); i++) {
                if (!HasAdjacentFilledRow(i, forRows) && random.Next(100) < wallpercentage) {
                    if (forRows) {
                        Rows.Add(i);
                    }
                    else {
                        Columns.Add(i);
                    }
                }
            }
        }

        /// <summary>
        /// Check if an adjacent row is not filled so theres always a gap
        /// </summary>
        /// <param name="index"></param>
        /// <param name="RowColumn"></param>
        /// <returns></returns>
        private bool HasAdjacentFilledRow(int index, bool RowColumn) {
            if (RowColumn) {
                if (Rows.Contains(index - 1) || Rows.Contains(index + 1)) {
                    return true;
                }
                else {
                    return false;
                }
            }
            else {
                if (Columns.Contains(index - 1) || Columns.Contains(index + 1)) {
                    return true;
                }
                else {
                    return false;
                }
            }
        }

        /// <summary>
        /// Place the vertical and horizontal walls
        /// </summary>
        private void PlaceWalls() {
            foreach (int x in Columns) {
                int[] randomIndex = Randomize(maze.height);
                bool oneGapPlaced = false;
                while (oneGapPlaced == false) {
                    for (int i = 0; i < maze.height; i++) {
                        int y = randomIndex[i];
                        if (!maze.cels[x, y].isWall) {
                            if (!oneGapPlaced) {
                                if (random.Next(100) < 15 && CheckCentrumOfCross(x, y)) {
                                    oneGapPlaced = true;
                                    continue; // gap
                                }
                            }
                            maze.cels[x, y].isWall = true;
                            maze.cels[x, y].color = Color.Red;
                        }
                    }

                }
                MakeGap(x, false);
            }

            foreach (int y in Rows) {
                int[] randomIndex = Randomize(maze.length);
                bool gapPlaced = false;
                while (gapPlaced == false) {
                    for (int i = 0; i < maze.length; i++) {
                        int x = randomIndex[i];
                        if (!maze.cels[x, y].isWall) {
                            if (!gapPlaced) {
                                if (random.Next(100) < 15 && CheckCentrumOfCross(x, y)) {
                                    gapPlaced = true;
                                    continue; //gap
                                }
                            }
                            maze.cels[x, y].isWall = true;
                            maze.cels[x, y].color = Color.Red;
                        }
                    }
                }
                MakeGap(y, true);
            }
        }

        /// <summary>
        /// Make a gap
        /// </summary>
        /// <param name="xy"></param>
        /// <param name="forRows"></param>
        private void MakeGap(int xy, bool forRows) {
            for (int i = 1; i < (forRows ? maze.height - 1 : maze.length - 1); i++) {
                if (random.Next(100) < destructionpercentage && (forRows ? CheckCentrumOfCross(i, xy) : CheckCentrumOfCross(xy, i))) {
                    if (forRows) {
                        maze.cels[i, xy].isWall = false;
                        //maze.cels[i, xy].visited = false;
                        maze.cels[i, xy].color = Color.Black;
                    }
                    else {
                        maze.cels[xy, i].isWall = false;
                        //  maze.cels[xy, i].visited = false;
                        maze.cels[xy, i].color = Color.Black;

                    }
                }
            }
        }

        /// <summary>
        /// Check if an empty cell is not surrounded by walls.
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns></returns>
        private bool CheckCentrumOfCross(int x, int y) {
            int i = 0;
            if (!(x - 1 < 0 || x + 1 >= maze.length || y - 1 < 0 || y + 1 >= maze.height)) {
                if (maze.cels[x + 1, y].color == Color.Red) {
                    i++;
                }
                if (maze.cels[x - 1, y].color == Color.Red) {
                    i++;
                }
                if (maze.cels[x, y + 1].color == Color.Red) {
                    i++;
                }
                if (maze.cels[x, y - 1].color == Color.Red) {
                    i++;
                }
            }
            if (i >= 3) {
                return false;
            }
            else {
                return true;
            }


        }

        /// <summary>
        /// Taken from https://stackoverflow.com/questions/56378647/fisher-yates-shuffle-in-c-sharp
        /// </summary>
        /// <param name="size"></param>
        /// <returns></returns>
        private int[] Randomize(int size) {
            int[] index = new int[size];
            for (int i = 0; i < size; i++) {
                index[i] = i;
            }
            for (int i = size - 1; i > 0; i--) {
                int j = random.Next(0, i + 1);
                int temp = index[i];
                index[i] = index[j];
                index[j] = temp;
            }

            return index;
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
        /// Create a border
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
        /// Solve the maze with an algoritm to see if its solvable
        /// </summary>
        /// <param name="start"></param>
        private void SolveMaze(Cell start) {
            int i = 0;
            while (!isDone && i < 1) {
                bool solved = false;
                oldVisitedCells.Add(start);
                while (!solved && !isDone) {
                    solved = true;
                    foreach (Cell cell in oldVisitedCells.ToList()) {
                        if (FillInAvailableNeighbours(cell.x, cell.y)) {
                            solved = false;
                        }
                        if (isDone) {
                            break;
                        }
                    }
                    oldVisitedCells = newVisitedCells.ToList();
                    newVisitedCells.Clear();

                }
                if (!isDone && i < 1) {
                    MakeAdditionalGaps();
                    i++;
                }
            }

        }

        /// <summary>
        /// Make additional gaps
        /// </summary>
        private void MakeAdditionalGaps() {
            foreach (int x in Columns) {
                MakeGap(x, false);
            }
            foreach (int y in Rows) {
                MakeGap(y, true);

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
        /// Set the cell as visited to prevent index array errors
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        private void SetVisited(int x, int y) {
            Cell cell = maze.cels[x, y];
            cell.setLocation(x, y);
            if (cell.isWall == false && cell.visited == false) {
                //cell.color = Color.White;
                cell.visited = true;
                newVisitedCells.Add(cell);
                if (cell.id == stopid.id) {
                    isDone = true;
                }
            }

        }

        /// <summary>
        /// Return if the cell has been visited
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns></returns>
        private bool IsVisited(int x, int y) {
            if (x >= 0 && x < maze.length && y >= 0 && y < maze.height) {
                return maze.cels[x, y].visited;
            }
            else if (x == -1 || y == -1 || x == maze.length || y == maze.height) {
                return true;
            }
            else {
                return true;
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
