using Global;
using System;
using System.Windows.Media.Media3D;

namespace Wpf_3D {
    /// <summary>
    /// This class was made with the help of this article 
    /// https://www.csharphelper.com/howtos/howto_3D_sphere.html
    /// Alot of code is reused from this.
    /// </summary>
    public class Ball3DGenerator {
        private MeshGeometry3D mesh;

        public Model3D GenerateBall(Ball ball) {
            Model3DGroup modelGroup = new Model3DGroup();
            DiffuseMaterial ballMaterial = new DiffuseMaterial(ToBrush(ball.color));
            mesh = new MeshGeometry3D();
            double x = ball.x + 0.1;
            double y = ball.y + 0.1;
            double z = 0.5;

            AddSphere(new Point3D(x + ball.size, y + ball.size, z), ball.size, 32, 32);
            GeometryModel3D ballModel = new GeometryModel3D(mesh, ballMaterial);
            modelGroup.Children.Add(ballModel);
            return modelGroup;
        }

        /// <summary>
        /// This conversion methode is taken from stackoverflow
        /// </summary>
        /// <href = https://stackoverflow.com/a/33523743></href>
        /// <param name="color"></param>
        /// <returns></returns>
        private System.Windows.Media.Brush ToBrush(System.Drawing.Color color) {
            return new System.Windows.Media.SolidColorBrush(System.Windows.Media.Color.FromArgb(color.A, color.R, color.G, color.B));
        }

        private void AddTriangle(Point3D p1, Point3D p2, Point3D p3) {
            int index1 = mesh.Positions.Count;
            int index2 = index1 + 1;
            int index3 = index1 + 2;
            mesh.Positions.Add(p1);
            mesh.Positions.Add(p2);
            mesh.Positions.Add(p3);
            mesh.TriangleIndices.Add(index1);
            mesh.TriangleIndices.Add(index2);
            mesh.TriangleIndices.Add(index3);
        }

        /// <summary>
        /// Taken from https://www.csharphelper.com/howtos/howto_3D_sphere.html
        /// </summary>
        /// <param name="center"></param>
        /// <param name="radius"></param>
        /// <param name="num_phi"></param>
        /// <param name="num_theta"></param>
        private void AddSphere(Point3D center,
        double radius, int num_phi, int num_theta) {
            double phi0, theta0;
            double dphi = Math.PI / num_phi;
            double dtheta = 2 * Math.PI / num_theta;

            phi0 = 0;
            double y0 = radius * Math.Cos(phi0);
            double r0 = radius * Math.Sin(phi0);
            for (int i = 0; i < num_phi; i++) {
                double phi1 = phi0 + dphi;
                double y1 = radius * Math.Cos(phi1);
                double r1 = radius * Math.Sin(phi1);

                // Point ptAB has phi value A and theta value B.
                // For example, pt01 has phi = phi0 and theta = theta1.
                // Find the points with theta = theta0.
                theta0 = 0;
                Point3D pt00 = new Point3D(
                    center.X + r0 * Math.Cos(theta0),
                    center.Y + y0,
                    center.Z + r0 * Math.Sin(theta0));
                Point3D pt10 = new Point3D(
                    center.X + r1 * Math.Cos(theta0),
                    center.Y + y1,
                    center.Z + r1 * Math.Sin(theta0));
                for (int j = 0; j < num_theta; j++) {
                    // Find the points with theta = theta1.
                    double theta1 = theta0 + dtheta;
                    Point3D pt01 = new Point3D(
                        center.X + r0 * Math.Cos(theta1),
                        center.Y + y0,
                        center.Z + r0 * Math.Sin(theta1));
                    Point3D pt11 = new Point3D(
                        center.X + r1 * Math.Cos(theta1),
                        center.Y + y1,
                        center.Z + r1 * Math.Sin(theta1));

                    // Create the triangles.
                    AddTriangle(pt00, pt11, pt10);
                    AddTriangle(pt00, pt01, pt11);

                    // Move to the next value of theta.
                    theta0 = theta1;
                    pt00 = pt01;
                    pt10 = pt11;
                }

                // Move to the next value of phi.
                phi0 = phi1;
                y0 = y1;
                r0 = r1;
            }
        }
    }
}