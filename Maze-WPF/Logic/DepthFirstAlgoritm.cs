using Global;
using Logica;
using System.Drawing;

namespace Logic {

    public class DepthFirstAlgoritm {
        public Maze maze;
        public bool isDone = false;
        private static readonly Random random = new Random();
        private int x;
        private int y;
        private int attempts = 0;
        private bool goodStep;
        private int startcellId;
        public DepthFirstAlgoritm(Maze maze) {
            this.maze = maze;
            x = 0;
            y = 0;
        }

        /// <summary>
        /// Generate the maze.
        /// </summary>
        public void Generate() {
            maze.length--;
            maze.height--;
            CalculateIdStartCell();
            while (isDone == false) {
                SetStep();
            }
            CalculateStartStop(1);
            CalculateStartStop(maze.length - 1);
            maze.height++;
            maze.length++;
            GenerateBorder();
            SetWalls();
        }

        /// <summary>
        /// Set a step in a random direction. If there are too many unsuccesfull attempts it will force a step
        /// </summary>
        private void SetStep() {
            goodStep = false;
            attempts = 0;
            while (goodStep == false && isDone == false) {
                goodStep = false;
                int tempX = x;
                int tempY = y;
                if (attempts > 5) {
                    ForceStep();
                    break;
                }
                else {
                    Direction direction = GenerateRandomMovement();
                    switch (direction) {
                        case Direction.up: tempY = y - 1; break;
                        case Direction.down: tempY = y + 1; break;
                        case Direction.right: tempX = x + 1; break;
                        case Direction.left: tempX = x - 1; break;
                    }
                    if (tempY >= 1 && tempX >= 1 && tempX < maze.length && tempY < maze.height) {
                        if (CheckStepInvalid(tempX, tempY) || CheckNeighbours(x, y, tempX, tempY)) {
                            attempts = attempts + 1;
                        }
                        else {
                            x = tempX;
                            y = tempY;
                            maze.cels[x, y].SetVisited(true);
                            maze.cels[x, y].color = Color.Red;
                            goodStep = true;
                        }
                    }
                    else {
                        attempts = attempts + 1;
                    }
                }
            }

        }

        /// <summary>
        /// will force check every direction for a good step.
        /// </summary>
        private void ForceStep() {
            if (CheckStepInvalid((x + 1), y) == false && CheckNeighbours(x, y, x + 1, y) == false) {
                goodStep = true;
                x++;
            }
            else if (CheckStepInvalid((x - 1), y) == false && CheckNeighbours(x, y, x - 1, y) == false) {
                goodStep = true;
                x--;
            }
            else if (CheckStepInvalid(x, (y + 1)) == false && CheckNeighbours(x, y, x, y + 1) == false) {
                goodStep = true;
                y++;
            }
            else if (CheckStepInvalid(x, (y - 1)) == false && CheckNeighbours(x, y, x, y - 1) == false) {
                goodStep = true;
                y--;
            }
            if (goodStep) {
                maze.cels[x, y].SetVisited(true);
                maze.cels[x, y].color = Color.Red;
                attempts = 0;
            }
            if (goodStep == false) {
                BacktrackStep();
            }
        }

        /// <summary>
        /// Backstep in a random direction untill a good step has been found
        /// if the step is back to the startcell then the maze is done.
        /// </summary>
        private void BacktrackStep() {
            int tempY = y;
            int tempX = x;
            maze.cels[x, y].isBacktracked = true;
            Direction[] directions = Enum.GetValues(typeof(Direction))
                                    .Cast<Direction>()
                                    .OrderBy(_ => Guid.NewGuid())
                                    .ToArray();
            //Random order
            foreach (Direction direction in directions) {
                switch (direction) {
                    case Direction.up: tempY = y - 1; break;
                    case Direction.down: tempY = y + 1; break;
                    case Direction.right: tempX = x + 1; break;
                    case Direction.left: tempX = x - 1; break;
                }
                if (IsVisited(tempX, tempY) == true) {
                    y = tempY;
                    x = tempX;
                    maze.cels[x, y].color = Color.Black;
                    break;
                }
            }
            if (maze.cels[x, y].id == startcellId) {
                isDone = true;
            }

        }


        /// <summary>
        /// Check if a cell has a neighbouring cell that has been visited
        /// </summary>
        /// <param name="x1"></param>
        /// <param name="y1"></param>
        /// <param name="x2"></param>
        /// <param name="y2"></param>
        /// <returns></returns>
        private bool CheckNeighbours(int x1, int y1, int x2, int y2) {
            bool notOk = false;

            // Check adjacent neighbors
            if (IsVisited(x2 + 1, y2) && x2 + 1 != x1) {
                notOk = true;
            }
            else if (IsVisited(x2 - 1, y2) && x2 - 1 != x1) {
                notOk = true;
            }
            else if (IsVisited(x2, y2 + 1) && y2 + 1 != y1) {
                notOk = true;
            }
            else if (IsVisited(x2, y2 - 1) && y2 - 1 != y1) {
                notOk = true;
            }

            // Check diagonal neighbors
            if (IsVisited(x2 + 1, y2 + 1) && x2 + 1 != x1 && y2 + 1 != y1) {
                notOk = true;
            }
            else if (IsVisited(x2 - 1, y2 - 1) && x2 - 1 != x1 && y2 - 1 != y1) {
                notOk = true;
            }
            else if (IsVisited(x2 + 1, y2 - 1) && x2 + 1 != x1 && y2 - 1 != y1) {
                notOk = true;
            }
            else if (IsVisited(x2 - 1, y2 + 1) && x2 - 1 != x1 && y2 + 1 != y1) {
                notOk = true;
            }

            return notOk;
        }



        /// <summary>
        /// Check if a cell has been visited to prevent array out of bounds errors
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns></returns>
        private bool IsVisited(int x, int y) {
            if (x >= 1 && x < maze.length && y >= 1 && y < maze.height) {
                return maze.cels[x, y].visited;
            }
            else if (x == 0 || y == 0 || x == maze.length || y == maze.height) {
                return false;
                //wall, so can ignore
            }
            else {
                return true;
            }
        }

        /// <summary>
        /// Generate a random direction
        /// </summary>
        /// <returns></returns>
        /// <exception cref="InvalidOperationException"></exception>
        private Direction GenerateRandomMovement() {
            int randomNumber = random.Next(1, 5);
            switch (randomNumber) {
                case 1:
                    return Direction.up;
                case 2:
                    return Direction.down;
                case 3:
                    return Direction.left;
                case 4:
                    return Direction.right;
                default:
                    throw new InvalidOperationException("Invalid random number generated.");
            }
        }

        /// <summary>
        /// Pick random cell at the left side to generate the maze from
        /// </summary>
        private void CalculateIdStartCell() {
            y = random.Next(1, maze.height - 1);
            x = random.Next(1, maze.length - 1);
            startcellId = maze.cels[x, y].id;
            maze.cels[x, y].SetVisited(true);
        }

        /// <summary>
        /// Find a usable cell in a row for the start or the stop of the maze.
        /// </summary>
        /// <param name="x"></param>
        private void CalculateStartStop(int x) {
            if (x >= 1 && x < maze.length) {
                for (int i = 1; i < maze.length; i++) {
                    Random random = new Random();
                    int randomY = random.Next(0, maze.length);
                    if (maze.cels[x, randomY].color == Color.Black) {
                        if (x == 1) {
                            maze.ball = new Ball();
                            maze.ball.SetLocation(x, randomY);
                        }
                        else {
                            maze.cels[x, randomY].color = Color.Blue;
                            Cell endCell = new Cell();
                            endCell.setLocation(x, randomY);
                            maze.endCell = endCell;
                        }
                        break;
                    }
                }
            }
        }


        public void GenerateBorder() {
            int width = maze.length;
            int height = maze.height;

            for (int x = 0; x < width; x++) {
                for (int y = 0; y < height; y++) {
                    if (x == 0 || x == width - 1 || y == 0 || y == height - 1) {
                        maze.cels[x, y].isWall = true;
                    }
                }
            }
        }



        /// <summary>
        /// Check if a step is invalid
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns></returns>
        private bool CheckStepInvalid(int x, int y) {
            bool isInvalid = true;
            if (x >= 1 && x < maze.length && y >= 1 && y < maze.height) {
                if (x < maze.length && x >= 1 && isInvalid != false) {
                    isInvalid = false;
                }
                if (y < maze.height && y >= 1 && isInvalid != false) {
                    isInvalid = false;
                }
                if (IsVisited(x, y) == true) {
                    isInvalid = true;
                }

            }
            return isInvalid;
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