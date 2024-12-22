using System.Drawing;

namespace Logica {
    public class Cell {
        public int id;
        public int x;
        public int y;
        public bool[] walls = new bool[4];
        public bool visited = false;
        public bool isWall = false;
        public bool isBacktracked = false;
        public Color color;
        //    ____  <-0
        //   |<-3 | <-1
        //   |    |  
        //    ----  <-2
        public Cell() {
        }
        public Cell(int id, Color color) {
            this.color = color;
            this.id = id;

        }
        public Cell(int id, int x, int y, bool[] muren) {
            this.id = id;
            this.x = x;
            this.y = y;
            this.walls = muren;
        }
        public int getId() {
            return this.id;
        }
        public void setLocation(int x, int y) {
            this.x = x;
            this.y = y;
        }
        public void setWalls(bool bool0, bool bool1, bool bool2, bool bool3) {
            this.walls[0] = bool0;
            this.walls[1] = bool1;
            this.walls[2] = bool2;
            this.walls[3] = bool3;
        }

        public void SetDefaultWalls() {
            this.walls[0] = false;
            this.walls[1] = false;
            this.walls[2] = false;
            this.walls[3] = false;
        }

        public void SetVisited(bool visited) {
            this.visited = visited;
        }
        public void SetWall(bool wall) {
            this.isWall = true;
        }
        public void SetIsBacktracked(bool isBacktracked) {
            this.isBacktracked = isBacktracked;
        }


    }
}
