using System;
using System.Collections.Generic;
using System.Data;
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
using System.Windows.Media.Media3D;
using System.Windows.Shapes;

namespace OptimizationMethods.Windows
{
    /// <summary>
    /// Логика взаимодействия для MethodResults.xaml
    /// </summary>
    public partial class MethodResults : Window
    {
        public MethodResults(object points,int dimensions,List<MinPointND> solutions, string[] variableNames,List<int> resolution,string text)
        {
            InitializeComponent();
            OutputTextBlock.Text = text;
            switch (dimensions)
            {
                case 1:
                    Init1D((List<Point>)points,solutions,variableNames[0]);
                    break;
                case 2:
                    Init2D((List<Point3D>)points,resolution,solutions,variableNames);
                    break;
                default:
                    break;
            }
            InitGrid(dimensions,solutions, variableNames);
        }
        void InitGrid(int dimensions,List<MinPointND> solutions,string[] variableNames)
        {
            DataTable dt = new DataTable();
            dt.Columns.Add("ID");
            dt.Columns.Add("f(x)");
            for (int i=0;i<dimensions;i++)
                dt.Columns.Add(variableNames[i]);
            int j = 0;
            foreach(var point in solutions)
            {
                j++;
                DataRow dr = dt.NewRow();
                dr[0] = j;
                dr[1] = point.y;
                for (int i = 0; i < dimensions; i++)
                {
                    dr[i + 2] =point.x[i];
                }
                dt.Rows.Add(dr);
            }
            grid.ItemsSource = dt.DefaultView;
        }
        void Init1D(List<Point> points,List<MinPointND> solutions,string variableName)
        {
            PlotResult pr = new PlotResult(points,solutions,variableName);
            pr.Show();
        }
        void Init2D(List<Point3D> points,List<int> resolution, List<MinPointND> solutions,string[]variableNames)
        {
            double min= double.MaxValue, max= double.MinValue;
            foreach (var point in points)
            {
                min = Math.Min(min,point.Z);
                max = Math.Max(max, point.Z);
            }
            PlotResult2D pr = new PlotResult2D(points,solutions,resolution[0],resolution[1],(float)min,(float)max, variableNames);
            pr.Show();
        }
    }
}
