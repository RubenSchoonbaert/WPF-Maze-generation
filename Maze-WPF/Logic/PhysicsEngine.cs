using Global;
using Logica;

namespace Logic {
    public class PhysicsEngine {
        public Ball ball { get; set; }

        private Maze maze;
        private double timeInterval = 0.1; //(100ms)
        public bool isFinished = false;
        double Dv = 0;
        double Dh = 0;
        private double elasticity = 0.5;


        public PhysicsEngine(Ball ball, Maze maze) {
            this.ball = ball;
            this.maze = maze;
        }


        /// <summary>
        /// Checks for collisions on both axis
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        public void CheckForCollision(double x, double y) {
            CheckXAxis(x);
            CheckYAxis(y);
        }

        /// <summary>
        /// Check the x axis for collisions
        /// </summary>
        /// <param name="x"></param>
        internal void CheckXAxis(double x) {
            double arrayX = x - 0.5;
            bool directionCheck = ball.x > x; // true for left, false for right

            bool axisCheck = false;

            for (double i = -ball.size; i <= ball.size; i += 0.1) {
                double checkX = directionCheck ? arrayX - ball.size : arrayX + ball.size;
                double checkY = ball.y - 0.5 + i;

                if (maze.cels[RoundToNearest(checkX), RoundToNearest(checkY)].isWall) {
                    axisCheck = true;
                    break;
                }
            }

            if (!axisCheck) {
                ball.x = x;
            }
            else {
                Dv = -Dv * elasticity;
            }
        }




        /// <summary>
        /// Check if the y axis collides with the edge
        /// </summary>
        /// <param name="y"></param>
        internal void CheckYAxis(double y) {
            double arrayY = y - 0.5;
            bool directionCheck = ball.y > y; // true for up, false for down

            bool axisCheck = false;

            for (double i = -ball.size; i <= ball.size; i += 0.1) {
                double checkX = ball.x - 0.5 + i;
                double checkY = directionCheck ? arrayY - ball.size : arrayY + ball.size;

                if (maze.cels[RoundToNearest(checkX), RoundToNearest(checkY)].isWall) {
                    axisCheck = true;
                    break;
                }
            }

            if (!axisCheck) {
                ball.y = y;
            }
            else {
                Dh = -Dh * elasticity;
            }
        }





        /// <summary>
        /// Check if the ball is in the end cell.
        /// </summary>
        /// <returns></returns>
        internal bool CheckFinished() {
            double tolerance = 0.5; // Adjust the tolerance as needed

            if (Math.Abs(ball.x - maze.endCell.x) < tolerance && Math.Abs(ball.y - maze.endCell.y) < tolerance) {
                return true;
            }
            else {
                return false;
            }
        }



        //round double to the nearest integer
        public int RoundToNearest(double value) {
            return (int)Math.Ceiling(value);
        }

        /// <summary>
        /// Calculate angle to x and y forces
        /// </summary>
        /// <param name="vertR"></param>
        /// <param name="horzR"></param>
        /// <returns></returns>
        public Ball CalculateAngleMovement(double vertR, double horzR) {
            double radianAngleX = vertR * (Math.PI / 180);
            double radianAngleY = horzR * (Math.PI / 180);

            double aX = 9.81 * Math.Sin(radianAngleX);
            double aY = 9.81 * Math.Sin(radianAngleY);
            Dh += (1.0 / 2.0) * aX * Math.Pow(timeInterval, 2);
            Dv += (1.0 / 2.0) * aY * Math.Pow(timeInterval, 2);

            AddFriction();
            double nextX = ball.x + Dv;
            double nextY = ball.y - Dh;
            CheckForCollision(nextX, nextY);
            if (CheckFinished()) {
                isFinished = true;
            }
            else {
                isFinished = false;
            }
            return ball;
        }

        /// <summary>
        /// Add friction the the ball
        /// </summary>
        private void AddFriction() {
            double friction = 0.15;
            Dv *= (1.0 - friction * timeInterval);
            Dh *= (1.0 - friction * timeInterval);
        }

    }
}
