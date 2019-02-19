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
using System.Windows.Media.Media3D;
using System.Windows.Shapes;
using static OptimizationMethods.Compiler.ASTCompiler;

namespace OptimizationMethods
{
    /// <summary>
    /// Логика взаимодействия для PlotSettings.xaml
    /// </summary>
    public partial class PlotSettings : Window
    {
        public bool OneDim { get; } = false;
        ExpressionStack exp;
        public PlotSettings(bool line, ExpressionStack exp)
        {
            DataContext = this;
            InitializeComponent();
            OneDim = !line;
            if (!OneDim)
            {
                yResolution.Visibility=yMin.Visibility = yMax.Visibility=yLabel.Visibility = Visibility.Hidden;
            }
            this.exp = exp;
        }
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            if (!OneDim)
            {
                int resolution = (int)xResolution.Value;
                double x1 = xMin.Value.Value, x2 = xMax.Value.Value;
                double dx = (x2 - x1) /(resolution-1);
                List<Point> points = new List<Point>(resolution);
                for (int i = 0; i < resolution; i++)
                {
                    double x = x1 + dx * i;
                    exp.set("x",(float)x);
                    points.Add(new Point(x,exp.execute()));
                }
                PlotResult pr = new PlotResult(points,exp.getVariableNames()[0]);
                pr.Show();
            }
            else
            {
                int resolutionX = (int)xResolution.Value;
                int resolutionY = (int)yResolution.Value;
                double x1=xMin.Value.Value, x2 = xMax.Value.Value;
                double y1 = xMin.Value.Value, y2 = yMax.Value.Value;
                double dx = (x2 - x1) / (resolutionX - 1);
                double dy = (y2 - y1) / (resolutionY - 1);
                List<Point3D> points = new List<Point3D>(resolutionX*resolutionY);
                double min=double.MaxValue,max=double.MinValue;
                for (int j = 0; j < resolutionY; j++)
                {
                    double y = y1 + dy * j;
                    exp.set("y", (float)y);
                    for (int i = 0; i < resolutionX; i++)
                    {
                        double x = x1 + dx * i;
                        exp.set("x", (float)x);
                        double z = exp.execute();
                        min = Math.Min(min, z);
                        max = Math.Max(max, z);
                        points.Add(new Point3D(x,y,z));
                    }
                }
                PlotResult2D pr = new PlotResult2D(points,resolutionX,resolutionY,(float)min,(float)max, exp.getVariableNames());
                pr.Show();
            }
        }
    }
}
