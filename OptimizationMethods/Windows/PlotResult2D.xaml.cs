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
using System.Drawing;
using System.IO;
using System.Drawing.Drawing2D;

namespace OptimizationMethods
{
    /// <summary>
    /// Логика взаимодействия для PlotResult2D.xaml
    /// </summary>
    public partial class PlotResult2D : Window
    {
        public PlotResult2D(List<Point3D> points,int x,int y,float min,float max,string[] variables)
        {
            InitializeComponent();
            xName.Content = variables[0];
            yName.Content = variables[1];
            Point3D last = points[points.Count - 1];
            x1.Content = points[0].X.ToString();
            x2.Content = last.X.ToString();
            y1.Content = points[0].Y.ToString();
            y2.Content = last.Y.ToString();
            Bitmap mp=new Bitmap(x,y);
            float dz = max - min;
            for (int Ycount = 0; Ycount < y; Ycount++)
            {
                for (int Xcount = 0; Xcount < x; Xcount++)
                {
                    int color = (int)(255.0 * (points[Xcount+Ycount*x].Z-min)/dz);
                    mp.SetPixel(Xcount, Ycount, System.Drawing.Color.FromArgb(color,color,color));
                }
            }

            BitmapImage bitmapimage = new BitmapImage();
            using (MemoryStream memory = new MemoryStream())
            {
                mp.Save(memory, System.Drawing.Imaging.ImageFormat.Bmp);
                memory.Position = 0;
                bitmapimage.BeginInit();
                bitmapimage.StreamSource = memory;
                bitmapimage.CacheOption = BitmapCacheOption.OnLoad;
                bitmapimage.EndInit();
            }
            img.Source = bitmapimage;
        }
        static MinPointND rescale(MinPointND a, double mx, double my, double sx, double sy)
        {
            return new MinPointND(new double[] { (a.x[0]-mx)*sx,(a.x[1]-my)*sy},a.y);
        }
        public PlotResult2D(List<Point3D> points,List<MinPointND> solutions, int x, int y, double min, double max, string[] variables)
        {
            InitializeComponent();
            xName.Content = variables[0];
            yName.Content = variables[1];
            Bitmap mp = new Bitmap(x, y);
            Point3D last = points[points.Count - 1];
            double miny = points[0].Y;
            double minx = points[0].X;
            double dx = last.X - minx;
            double dy = last.Y - miny;
            x1.Content = points[0].X.ToString();
            x2.Content = last.X.ToString();
            y1.Content = points[0].Y.ToString();
            y2.Content = last.Y.ToString();
            double dz = max - min;
            for (int Ycount = 0; Ycount < y; Ycount++)
            {
                for (int Xcount = 0; Xcount < x; Xcount++)
                {
                    int color = (int)(255.0 * (points[Xcount + Ycount * x].Z - min) / dz);
                    mp.SetPixel(Xcount, Ycount, System.Drawing.Color.FromArgb(color, color, color));
                }
            }
            double sx =x/dx;
            double sy =y/dy;
            using (Graphics gr = Graphics.FromImage(mp))
            {
                gr.SmoothingMode = SmoothingMode.AntiAlias;
                MinPointND p1 = rescale(solutions[0], minx, miny, sx, sy);
                using (System.Drawing.Pen thick_pen = new System.Drawing.Pen(System.Drawing.Color.Blue, 1))
                {
                    for (int i = 0; i < solutions.Count - 1; i++)
                    {
                        MinPointND p2 = rescale(solutions[i+1], minx, miny, sx, sy);
                        gr.DrawLine(thick_pen,(float)p1.x[0], (float)p1.x[1], (float)p2.x[0], (float)p2.x[1]);
                        p1 = p2;
                    }
                }
                using (System.Drawing.Brush brush = new System.Drawing.SolidBrush(System.Drawing.Color.Red))
                {
                    gr.FillRectangle(brush, (float)p1.x[0], (float)p1.x[1], 1, 1);
                }
            }
            BitmapImage bitmapimage = new BitmapImage();
            using (MemoryStream memory = new MemoryStream())
            {
                mp.Save(memory, System.Drawing.Imaging.ImageFormat.Bmp);
                memory.Position = 0;
                bitmapimage.BeginInit();
                bitmapimage.StreamSource = memory;
                bitmapimage.CacheOption = BitmapCacheOption.OnLoad;
                bitmapimage.EndInit();
            }
            img.Source = bitmapimage;
        }
    }
}
