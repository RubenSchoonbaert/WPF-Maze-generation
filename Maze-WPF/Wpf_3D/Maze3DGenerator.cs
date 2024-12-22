using Logica;
using System.Collections.Generic;

namespace Wpf_3D {
    using System.Windows.Media;
    using System.Windows.Media.Media3D;

    public class Maze3DGenerator {
        List<GeometryModel3D> geom = new List<GeometryModel3D>();
        private Maze maze;


        public Model3D GenerateMaze(Maze maze) {
            this.maze = maze;
            geom.Clear();
            GenCubes();
            Model3D mazeModel = ConvertToSingleModel(geom);

            return mazeModel;
        }

        private void GenCubes() {
            for (int x = 0; x < maze.cels.GetLength(0); x++) {
                for (int y = 0; y < maze.cels.GetLength(1); y++) {
                    if (maze.cels[x, y].isWall) {
                        GenCube(x, y);
                    }
                    else {
                        GenFloor(x, y);
                    }
                }
            }
        }

        private void GenCube(int x, int y) {
            int z = 0;
            var material = new MaterialGroup();
            material.Children.Add(new DiffuseMaterial(Brushes.Red));
            material.Children.Add(new SpecularMaterial(Brushes.White, 30));

            var geometry = new MeshGeometry3D {
                Positions = new Point3DCollection
                {
                    new Point3D(x, y, 0 + z),
                    new Point3D(x + 1, y, 0 + z),
                    new Point3D(x, y + 1, 0 + z),
                    new Point3D(x + 1, y + 1, 0 + z),
                    new Point3D(x, y, 1 + z),
                    new Point3D(x + 1, y, 1 + z),
                    new Point3D(x, y + 1, 1 + z),
                    new Point3D(x + 1, y + 1, 1 + z)
                },
                TriangleIndices = new Int32Collection
                {
                    // Bottom face
                    0, 1, 2, 1, 3, 2,

                    // Top face
                    4, 6, 5, 5, 6, 7,

                    // Side faces
                    0, 4, 1, 1, 4, 5,
                    2, 3, 6, 3, 7, 6,
                    0, 2, 4, 2, 6, 4,
                    1, 5, 3, 3, 5, 7
                },
                Normals = new Vector3DCollection
                {
                    new Vector3D(0, 0, -1), // Bottom face
                    new Vector3D(0, 0, 1),  // Top face
                    new Vector3D(-1, 0, 0), // Left face
                    new Vector3D(1, 0, 0),  // Right face
                    new Vector3D(0, -1, 0), // Front face
                    new Vector3D(0, 1, 0)   // Back face
                }
            };

            var model = new GeometryModel3D {
                Geometry = geometry,
                Material = material
            };
            geometry.Freeze();
            geom.Add(model);
        }

        /// <summary>
        /// Generate a floor tile and add it to the model
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        private void GenFloor(int x, int y) {
            int z = 0;
            var floorMaterial = new DiffuseMaterial();
            if (maze.cels[x, y].color == System.Drawing.Color.Blue) {
                floorMaterial = new DiffuseMaterial(Brushes.Blue);
            }
            else {
                floorMaterial = new DiffuseMaterial(Brushes.Green);
            }
            var floorGeometry = new MeshGeometry3D {
                Positions = new Point3DCollection
                {
                    new Point3D(x, y, 0 + z),
                    new Point3D(x + 1, y, 0 + z),
                    new Point3D(x, y + 1, 0 + z),
                    new Point3D(x + 1, y + 1, 0 + z),
                },
                TriangleIndices = new Int32Collection {
                    0, 2, 1,
                    1, 2, 3,
                    1, 3, 2,
                    2, 0, 1
                }
            };

            var floorModel = new GeometryModel3D {
                Geometry = floorGeometry,
                Material = floorMaterial
            };

            geom.Add(floorModel);
        }

        /// <summary>
        /// Convert the List of Geometry 3D models to one single modelgroup
        /// </summary>
        /// <param name="geom"></param>
        /// <returns></returns>
        private Model3D ConvertToSingleModel(List<GeometryModel3D> geom) {
            Model3DGroup modelGroup = new Model3DGroup();
            foreach (var geometryModel in geom) {
                modelGroup.Children.Add(geometryModel);
            }
            return modelGroup;
        }
    }

}
