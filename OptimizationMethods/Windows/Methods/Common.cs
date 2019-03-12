using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media.Media3D;
using static OptimizationMethods.Compiler.ASTCompiler;

namespace OptimizationMethods.Windows.Methods
{
    class Common
    {
        public static List<Point> calc1D(int resolution, double x1, double x2, ExpressionStack exp)
        {
            double dx = (x2 - x1) / (resolution - 1);
            List<Point> points = new List<Point>(resolution);
            for (int i = 0; i < resolution; i++)
            {
                double x = x1 + dx * i;
                exp.set("x", (float)x);
                points.Add(new Point(x, exp.execute()));
            }
            return points;
        }
        public static List<Point3D> calc2D(int resolutionX, int resolutionY, double x1, double x2, double y1, double y2, ExpressionStack exp)
        {
            double dx = (x2 - x1) / (resolutionX - 1);
            double dy = (y2 - y1) / (resolutionY - 1);
            List<Point3D> points = new List<Point3D>(resolutionX * resolutionY);
            double min = double.MaxValue, max = double.MinValue;
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
                    points.Add(new Point3D(x, y, z));
                }
            }
            return points;
        }
        public static void MethodResults(OptMethod1D method, SymbolicFunction1D function, double[] min, double[] max)
        {
            try
            {
                ExpressionStack expressionStack = function.getExpression();
                int iters;
                MinPointND mp = method.getMinimum(min[0], max[0], function,out iters);
                MethodResults mr = new MethodResults(calc1D(100, min[0], max[0], expressionStack), 1, new List<MinPointND> { mp },  expressionStack.getVariableNames(), new List<int> { 100 }, "Итераций: " + iters.ToString());
                mr.Show();
            }
            catch (Exception exc)
            {
                MessageBox.Show(exc.Message, "Error");
                return;
            }
        }
        public static void MethodResults(OptMethodND method,SymbolicFunctionND function,double[] min,double[] max)
        {
            try
            {
                OptMethodResult mp = method.getMinimum(function, 10);
                ExpressionStack expressionStack = function.getExpression();
                int dimensions=function.Dimensions;
                object points = null;
                List<int> resolution = new List<int>();
                switch (dimensions)
                {
                    case 1:
                        resolution.Add(100);
                        points = calc1D(100,min[0],max[0],expressionStack);
                        break;
                    case 2:
                        resolution.Add(100);
                        resolution.Add(100);
                        points = calc2D(100, 100, min[0], max[0], min[1], max[1], expressionStack);
                        break;
                }
                MethodResults mr = new MethodResults(points, dimensions, mp.solutions, expressionStack.getVariableNames(), resolution,
                    "Итераций: " + mp.iterations.ToString());
                mr.Show();
            }
            catch (Exception exc)
            {
                MessageBox.Show(exc.Message, "Error");
                return;
            }
        }
    }
}
