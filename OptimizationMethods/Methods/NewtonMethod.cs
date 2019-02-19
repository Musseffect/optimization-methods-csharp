using MathNet.Numerics.LinearAlgebra;
using OptimizationMethods;
using System;
//Лаб 4
class NewtonMethod : OptMethodND
{
    int iterations;
    double alpha;
    double epsilonY;
    double epsilonX;
    public void setParameters(int iter, double epsY, double epsX,double alpha)
    {
        iterations = iter;
        epsilonY = epsY;
        epsilonX = epsX;
        this.alpha = alpha;
    }
    public override OptMethodResult getMinimum(FunctionND func,int solutionCount)
    {
        LimitedQueue<MinPointND> queue = new LimitedQueue<MinPointND>(solutionCount);
        Vector<double> x = Vector<double>.Build.Dense(func.Dimensions);
        double y = func.exec(x.AsArray());
        queue.Add(new MinPointND(x.AsArray(),y));
        int iter = 0;
        for (; iter < iterations; iter++)
        {
            Matrix<double> hessiMatrix = Matrix<double>.Build.Dense(func.Dimensions, func.Dimensions, func.getHessiMatrix(x.AsArray()));
            Vector<double> grad = Vector<double>.Build.Dense(func.getGradient(x.AsArray()));
            grad *= alpha;
            Vector<double> dx;
            try
            {
                dx = hessiMatrix.Solve(-grad);
            }
            catch (Exception)
            {
                throw new Exception("");
            }
            x += dx;
            double oldy = y;
            y = func.exec(x.AsArray());
            queue.Add(new MinPointND(x.AsArray(), y));
            if (dx.L2Norm() < epsilonX)
            {
                break;
            }
            if (Math.Abs(oldy - y) < epsilonY)
            {
                break;
            }
        }
        return new OptMethodResult(queue.ToList(),iter);
    }
    public override MinPointND getMinimum(FunctionND func)
    {
        Vector<double> x = Vector<double>.Build.Dense(func.Dimensions);
        double y = func.exec(x.AsArray());
        for (int i = 0; i < iterations; i++)
        {
            Matrix<double> hessiMatrix = Matrix<double>.Build.Dense(func.Dimensions,func.Dimensions,func.getHessiMatrix(x.AsArray()));
            Vector<double> grad = Vector<double>.Build.Dense(func.getGradient(x.AsArray()));
            grad *=alpha;
            Vector<double> dx;
            try
            {
                dx = hessiMatrix.Solve(-grad);
            } catch (Exception)
            {
                throw new Exception("");
            }
            x += dx;
            double oldy = y;
            y = func.exec(x.AsArray());
            if (dx.L2Norm() < epsilonX)
            {
                break;
            }
            if (Math.Abs(oldy - y) < epsilonY)
            {
                break;
            }
        }
        return new MinPointND(x.AsArray(),y);
    }
}
