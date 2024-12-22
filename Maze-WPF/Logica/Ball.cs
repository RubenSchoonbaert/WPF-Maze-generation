using System.Drawing;

namespace Global {
    public class Ball {
        public double x;
        public double y;
        public double size {
            get;
        }
        public Color color;

        public Ball() {
            this.x = 0;
            this.y = 0;
            this.size = 0.4;
            this.color = Color.Blue;
        }

        public void SetLocation(double x, double y) {
            this.x = x;
            this.y = y;
        }
    }
}
