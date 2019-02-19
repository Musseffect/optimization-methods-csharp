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
using LiveCharts;
using LiveCharts.Wpf;
using System.Data;
using LiveCharts.Configurations;
using LiveCharts.Defaults;

namespace OptimizationMethods
{
    /// <summary>
    /// Логика взаимодействия для PlotResult.xaml
    /// </summary>
    public partial class PlotResult : Window
    {
        public SeriesCollection Series { get; set; }
        public PlotResult(List<Point> points,string variable)
        {
            InitializeComponent();
            XAxis.Title = variable;
            Series = new SeriesCollection();
            // var ySeriesTrue=new 
            List<ScatterPoint> list = new List<ScatterPoint>();
            for (int i = 0; i < points.Count; i++)
            list.Add(new ScatterPoint(points[i].X,points[i].Y));
            var ySeries = new LineSeries
            {
                Title = "f("+variable+")",
                Values = new ChartValues<ScatterPoint>(list),
                LineSmoothness = 0
            };
            Series.Add(ySeries);
            FunctionChart.Series = Series;

        }
        public PlotResult(List<Point> points,List<MinPointND> solutions,string variable)
        {
            InitializeComponent();
            XAxis.Title = variable;
            Series = new SeriesCollection();
            // var ySeriesTrue=new 
            List<ScatterPoint> list = new List<ScatterPoint>();
            for (int i = 0; i < points.Count; i++)
                list.Add(new ScatterPoint(points[i].X, points[i].Y));
            var ySeries = new LineSeries
            {
                Title = "f(" + variable + ")",
                Values = new ChartValues<ScatterPoint>(list),
                LineSmoothness = 0
            };
            Series.Add(ySeries);
            list = new List<ScatterPoint>();
            for (int i = 0; i < solutions.Count; i++)
                list.Add(new ScatterPoint(solutions[i].x[0], solutions[i].y));
            var solutionSeries = new LineSeries
            {
                Title = "Решение",
                Values = new ChartValues<ScatterPoint>(list),
                LineSmoothness = 0
            };
            Series.Add(solutionSeries);
            FunctionChart.Series = Series;

        }
    }
}
