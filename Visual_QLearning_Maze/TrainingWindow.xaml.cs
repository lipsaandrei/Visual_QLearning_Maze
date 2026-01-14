using OxyPlot;
using OxyPlot.Axes;
using OxyPlot.Series;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace Visual_QLearning_Maze
{
    /// <summary>
    /// Interaction logic for TrainingWindow.xaml
    /// </summary>
    public partial class TrainingWindow : Window
    {
        private LineSeries rewardSeries;
        private PlotModel model;
        private LineSeries avgSeries;
        private List<double> rewardHistory = new List<double>();

        public event Action WindowClosed;

        public TrainingWindow(int episodes)
        {
            InitializeComponent();

            model = new PlotModel { Title = "Total Reward per Episode" };

            model.Axes.Add(new LinearAxis
            {
                Position = AxisPosition.Left,
                Title = "Total Reward",
                Minimum = -60,
                Maximum = 0
            });

            model.Axes.Add(new LinearAxis
            {
                Position = AxisPosition.Bottom,
                Title = "Episode",
                Minimum = 0,
                Maximum = episodes
            });


            rewardSeries = new LineSeries
            {
                Title = "Reward",
                StrokeThickness = 2,
                Color = OxyColors.SkyBlue,
                LineStyle = LineStyle.Solid,
                MarkerType = MarkerType.None
            };

            //model.Series.Add(rewardSeries);
            
            avgSeries = new LineSeries
            {
                Title = "Moving Avg (10)",
                StrokeThickness = 3,
                Color = OxyColors.OrangeRed
            };

            model.Series.Add(rewardSeries);
            model.Series.Add(avgSeries);
            Plot.Model = model;

        }

        public void UpdateChart(double reward, int episode)
        {
            rewardHistory.Add(reward);

            rewardSeries.Points.Add(new DataPoint(episode + 1, reward));

            if (rewardHistory.Count >= 10)
            {
                double avg = rewardHistory
                    .Skip(Math.Max(0, rewardHistory.Count - 10))
                    .Average();

                avgSeries.Points.Add(new DataPoint(episode + 1, avg));
            }

            EpisodeLabel.Text = $"Episode {episode + 1}";
            model.InvalidatePlot(true);
        }

        protected override void OnClosed(EventArgs e)
        {
            WindowClosed?.Invoke();
            base.OnClosed(e);
        }

    }
}
