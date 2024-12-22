using Global;
using Logic;
using Logica;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media.Media3D;


namespace Wpf_3D {
    public partial class MainWindow : Window {
        private int vertR = 0; //vertical angle
        private int horzR = 0; //horizontal angle
        private ModelVisual3D visual3D;
        private Maze filled_maze;
        private Ball3DGenerator ball3DGenerator = new Ball3DGenerator();
        private Model3DGroup modelGroup = new Model3DGroup();
        Maze3DGenerator mazeGen = new Maze3DGenerator();
        private bool isGameOver = false;
        private bool popupBeingShown = false;
        private CancellationTokenSource gameLoopCancellationTokenSource;
        private PhysicsEngine physicsEngine;


        public MainWindow() {
            InitializeComponent();
            StartGamePopup();

        }

        /// <summary>
        /// initialise the maze generation & 3D models
        /// </summary>
        private void InitializeMaze() {
            Maze maze = new Maze(15, 15); //Change these parameters to change the size of the maze.
            maze.GenerateGrid(System.Drawing.Color.Red);
            DepthFirstAlgoritm algo = new DepthFirstAlgoritm(maze);
            algo.Generate();
            filled_maze = algo.maze;
            mazeGen = new Maze3DGenerator();
            AddGuiElements(mazeGen.GenerateMaze(algo.maze), ball3DGenerator.GenerateBall(maze.ball));
            physicsEngine = new PhysicsEngine(maze.ball, maze);
            isGameOver = false;
            Task.Run(async () => await GameLoopAsync());

        }

        /// <summary>
        /// calculate physics on the ball & redraw it every 50ms
        /// </summary>
        /// <returns></returns>
        private async Task GameLoopAsync() {
            gameLoopCancellationTokenSource = new CancellationTokenSource();
            while (!isGameOver) {
                await Task.Delay(50);
                Dispatcher.Invoke(() => {
                    CalculateBallMovement();
                });
            }
        }

        /// <summary>
        /// Popup for the start of the game
        /// Starts the game;
        /// </summary>
        private void StartGamePopup() {
            MessageBoxResult result = MessageBox.Show("Druk op OK om het spel te starten, \r\nKantel het bord met de pijltjes (↑ ↓ → ←)\r\nEn zoom de camera in & uit met de 'I' en 'O' toetsen.\r\nDuw op 'R' om het doolhof te resetten", "welkom", MessageBoxButton.OK);
            if (result == MessageBoxResult.OK) {
                InitializeMaze();
            }
            else {
                Application.Current.Shutdown();
            }
        }

        /// <summary>
        /// Popup for the end of the game.
        /// Can restart or end the game
        /// </summary>
        private async void ShowEndPopup() {
            if (physicsEngine.isFinished == true && !popupBeingShown) {
                popupBeingShown = true;
                MessageBoxResult result = MessageBox.Show("Proficiat!\r\nU hebt het doolhof opgelost. \r\nDuw op OK om opnieuw te beginnen en duw op cancel om te stoppen", "Proficiat!", MessageBoxButton.OKCancel);
                if (result == MessageBoxResult.OK) {
                    await Task.Delay(100);
                    InitializeMaze();
                }
                else if (result == MessageBoxResult.Cancel) {
                    gameLoopCancellationTokenSource.Cancel();
                    Application.Current.Shutdown();
                }
                popupBeingShown = false;
            }
        }

        /// <summary>
        /// Add all GUI elements to the modelgroup
        /// </summary>
        /// <param name="maze"></param>
        /// <param name="ball"></param>
        /// <returns></returns>
        private Task AddGuiElements(Model3D maze, Model3D ball) {
            modelGroup.Children.Clear();
            modelGroup.Children.Add(maze);
            modelGroup.Children.Add(ball);

            visual3D = new ModelVisual3D {
                Content = modelGroup
            };

            viewport3D1.Children[1] = visual3D;
            CentreScreen();
            return null;
        }


        /// <summary>
        /// Call the PhysicsEngine and move the ball.
        /// Check if the ball is in the end cell
        /// </summary>
        private void CalculateBallMovement() {
            if (!isGameOver) {
                Ball calcBall = physicsEngine.CalculateAngleMovement(vertR, horzR);
                Dispatcher.Invoke(() => {
                    modelGroup.Children[1] = ball3DGenerator.GenerateBall(calcBall);
                });

                if (physicsEngine.isFinished) {
                    ShowEndPopup();
                    isGameOver = true;
                }
            }
        }


        /// <summary>
        /// Center the screen
        /// </summary>
        private void CentreScreen() {
            vertR = 0;
            horzR = 0;
            Transform3DGroup rotationGroup = new Transform3DGroup();
            double centerX = filled_maze.length / 2;
            double centerY = filled_maze.height / 2;
            TranslateTransform3D centerTranslation = new TranslateTransform3D(-centerX, -centerY, 0);
            rotationGroup.Children.Add(centerTranslation);
            visual3D.Transform = rotationGroup;
        }


        /// <summary>
        /// Handle the GUI controls
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Window_KeyDown(object sender, KeyEventArgs e) {
            switch (e.Key) {
                case Key.Left:
                    horzR--;
                    HorizontalMovement();
                    break;
                case Key.Right:
                    horzR++;
                    HorizontalMovement();
                    break;
                case Key.Up:
                    vertR--;
                    VerticalMovement();
                    break;
                case Key.Down:
                    vertR++;
                    VerticalMovement();
                    break;
                case Key.I:
                    ZoomCamera(-5);
                    break;
                case Key.O:
                    ZoomCamera(5);
                    break;
                case Key.R:
                    InitializeMaze();
                    break;
            }
        }

        /// <summary>
        /// Angle the board on the horizontal axis
        /// </summary>
        private void HorizontalMovement() {
            if (horzR > 30) {
                horzR = 30;
            }
            if (horzR < -30) {
                horzR = -30;
            }

            double centerX = filled_maze.length / 2;
            double centerY = filled_maze.height / 2;

            TranslateTransform3D centerTranslation = new TranslateTransform3D(-centerX, -centerY, 0);
            RotateTransform3D horizontalRotation = new RotateTransform3D(new AxisAngleRotation3D(new Vector3D(0, 1, 0), horzR));
            RotateTransform3D verticalRotation = new RotateTransform3D(new AxisAngleRotation3D(new Vector3D(1, 0, 0), vertR));
            Transform3DGroup rotationGroup = new Transform3DGroup();
            rotationGroup.Children.Add(centerTranslation);
            rotationGroup.Children.Add(horizontalRotation);
            rotationGroup.Children.Add(verticalRotation);
            visual3D.Transform = rotationGroup;
        }



        /// <summary>
        /// Angle the board on the vertical axis
        /// </summary>
        private void VerticalMovement() {
            if (vertR > 30) {
                vertR = 30;
            }
            if (vertR < -30) {
                vertR = -30;
            }

            double centerX = filled_maze.length / 2;
            double centerY = filled_maze.height / 2;

            TranslateTransform3D centerTranslation = new TranslateTransform3D(-centerX, -centerY, 0);
            RotateTransform3D horizontalRotation = new RotateTransform3D(new AxisAngleRotation3D(new Vector3D(0, 1, 0), horzR));
            RotateTransform3D verticalRotation = new RotateTransform3D(new AxisAngleRotation3D(new Vector3D(1, 0, 0), vertR));
            Transform3DGroup rotationGroup = new Transform3DGroup();
            rotationGroup.Children.Add(centerTranslation);
            rotationGroup.Children.Add(horizontalRotation);
            rotationGroup.Children.Add(verticalRotation);
            visual3D.Transform = rotationGroup;
        }


        /// <summary>
        /// Change the perspective of the camera to zoom in & out
        /// </summary>
        /// <param name="zoomFactor"></param>
        private void ZoomCamera(double zoomFactor) {
            PerspectiveCamera camera = (PerspectiveCamera)viewport3D1.Camera;
            camera.FieldOfView += zoomFactor;

            if (camera.FieldOfView < 1.0) {
                camera.FieldOfView = 1.0;
            }
            if (camera.FieldOfView > 120.0) {
                camera.FieldOfView = 120.0;
            }
        }

    }

}
