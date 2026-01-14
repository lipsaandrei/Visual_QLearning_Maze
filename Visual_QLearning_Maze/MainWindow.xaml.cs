using System.Diagnostics;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Visual_QLearning_Maze;

namespace Visual_QLearning_Maze
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private int rows = 7;
        private int cols = 7;
        private int cellSize = 50;
        private int[,] maze;

        private enum DrawMode { Wall, Start, Goal }
        private DrawMode currentMode = DrawMode.Wall;

        private int startX = 0, startY = 0;
        private int goalX = -1, goalY = -1;

        private MazeEnvironment env;
        private QLearningAgent agent;
        private Simulator simulator;

        private CancellationTokenSource trainingCancel;

        private bool isTraining = false;
        private TrainingWindow currentTrainingWindow;

        public MainWindow()
        {
            InitializeComponent();

            goalX = cols - 1;
            goalY = rows - 1;

            // initializare maze
            maze = new int[rows, cols];

            // pozitii implicite pentru start si goal
            maze[startY, startX] = 2; // start
            maze[goalY, goalX] = 3; // goal

            DrawMaze();
        }

        private void WallButton_Click(object sender, RoutedEventArgs e)
        {
            currentMode = DrawMode.Wall;
        }

        private void StartButton_Click(object sender, RoutedEventArgs e)
        {
            currentMode = DrawMode.Start;
        }

        private void GoalButton_Click(object sender, RoutedEventArgs e)
        {
            currentMode = DrawMode.Goal;
        }

        private void ClearButton_Click(object sender, RoutedEventArgs e)
        {
            maze = new int[rows, cols];

            maze[startY, startX] = 2; // start
            maze[goalY, goalX] = 3; // goal

            env = null;
            agent = null;
            simulator = null;

            // redesenare maze
            DrawMaze();
        }


        private void DrawMaze()
        {
            MazeCanvas.Children.Clear();

            for (int y = 0; y < rows; y++)
            {
                for (int x = 0; x < cols; x++)
                {
                    Rectangle r = new Rectangle();
                    r.Width = cellSize;
                    r.Height = cellSize;
                    r.Stroke = Brushes.Gray;

                    if (x == startX && y == startY)
                        r.Fill = Brushes.Blue;       // start activ
                    else if (x == goalX && y == goalY)
                        r.Fill = Brushes.LightGreen; // goal activ
                    else if (maze[y, x] == 1)
                        r.Fill = Brushes.Black;      // perete
                    else
                        r.Fill = Brushes.White;      // liber

                    Canvas.SetLeft(r, x * cellSize);
                    Canvas.SetTop(r, y * cellSize);
                    MazeCanvas.Children.Add(r);
                }
            }

            if(env != null)
                DrawAgent();
        }

        void DrawAgent()
        {
            Ellipse e = new Ellipse();
            e.Width = cellSize - 10;
            e.Height = cellSize - 10;
            e.Fill = Brushes.Red;

            Canvas.SetLeft(e, env.AgentX * cellSize + 5);
            Canvas.SetTop(e, env.AgentY * cellSize + 5);

            MazeCanvas.Children.Add(e);
        }


        private void MazeCanvas_MouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            var point = e.GetPosition(MazeCanvas);
            int x = (int)(point.X / cellSize);
            int y = (int)(point.Y / cellSize);

            if (x < 0 || x >= cols || y < 0 || y >= rows)
                return;

            switch (currentMode)
            {
                case DrawMode.Wall:
                    if ((x == startX && y == startY) || (x == goalX && y == goalY))
                        return;
                    maze[y, x] = 1; // perete
                    break;
                case DrawMode.Start:
                    maze[startY, startX] = 0; // sterge start vechi
                    startX = x;
                    startY = y;
                    maze[startY, startX] = 2;
                    break;

                case DrawMode.Goal:
                    maze[goalY, goalX] = 0; // sterge goal vechi
                    goalX = x;
                    goalY = y;
                    maze[goalY, goalX] = 3;
                    break;
            }

            DrawMaze();
        }

        private void TrainButton_Click(object sender, RoutedEventArgs e)
        {
            env = new MazeEnvironment(maze);
            agent = new QLearningAgent(rows * cols, 4);

            // citeste valorile din UI
            if (double.TryParse(AlphaBox.Text, out double alpha))
                agent.Alpha = alpha;
            if (double.TryParse(GammaBox.Text, out double gamma))
                agent.Gamma = gamma;
            if (double.TryParse(EpsilonBox.Text, out double epsilon))
                agent.Epsilon = epsilon;

            simulator = new Simulator(env, agent);
            
            // numar episoade
            int episodes = 5000;
            if (int.TryParse(EpisodesBox.Text, out int ep)) 
                episodes = ep;

            simulator.Train(episodes);
            MessageBox.Show("Training complet!");
        }

        private async void RunButton_Click(object sender, RoutedEventArgs e)
        {
            if (env == null)
            {
                MessageBox.Show("Trebuie să antrenezi agentul mai întâi!");
                return;
            }

            int delay = 200;
            if (int.TryParse(DelayBox.Text, out int d)) delay = d;

            env.Reset();
            DrawMaze();

            for (int i = 0; i < 200; i++)
            {
                int state = env.GetState();
                int action = agent.ChooseAction(state);
                env.Step(action, out _);
                DrawMaze();
                await Task.Delay(delay);

                if (env.IsGoal())
                    break;
            }
        }

        // INCOMPLET
        // antrenare vizuala a agentului cu grafic de performanta
        private async void TrainVisualButton_Click(object sender, RoutedEventArgs e)
        {
            if (isTraining)
                return;

            isTraining = true;

            int episodes = 5000;
            int delay = 50;
            if (env == null)
            {
                env = new MazeEnvironment(maze);
                agent = new QLearningAgent(rows * cols, 4);

                if (double.TryParse(AlphaBox.Text, out double alpha)) agent.Alpha = alpha;
                if (double.TryParse(GammaBox.Text, out double gamma)) agent.Gamma = gamma;
                if (double.TryParse(EpsilonBox.Text, out double epsilon)) agent.Epsilon = epsilon;
                if (int.TryParse(EpisodesBox.Text, out int ep)) episodes = ep;
                if (int.TryParse(DelayBox.Text, out int d)) delay = d;
            }

            trainingCancel = new CancellationTokenSource();
            var token = trainingCancel.Token;

            //var qAgent = new QLearningAgent(rows * cols, 4);

            // creeaza fereastra de training
            currentTrainingWindow = new TrainingWindow(episodes);
            currentTrainingWindow.WindowClosed += () =>
            {
                trainingCancel.Cancel();
            };
            currentTrainingWindow.Show();
            try
            {
                for (int i = 0; i < episodes; i++)
                {
                    if (token.IsCancellationRequested)
                        break;

                    env.Reset();
                    double totalReward = 0;

                    env.Reset();

                    //while (!env.IsGoal())
                    //{
                    //    int state = env.GetState();
                    //    int action = qAgent.ChooseAction(state);
                    //    double reward = env.Step(action, out int nextState);
                    //    totalReward += reward;

                    //    DrawMaze();
                    //    await Task.Delay(delay);
                    //}

                    var win = currentTrainingWindow;

                    if (win != null && win.IsLoaded && !token.IsCancellationRequested)
                    {
                        win.Dispatcher.Invoke(() =>
                        {
                            if (win.IsLoaded)
                                win.UpdateChart(totalReward, episodes);
                        });
                    }
                }
            }
            finally
            {
                isTraining = false;
                currentTrainingWindow = null;
            }
        }

    }
}