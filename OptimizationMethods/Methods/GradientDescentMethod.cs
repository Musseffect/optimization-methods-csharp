using MathNet.Numerics.LinearAlgebra;
using OptimizationMethods;
using System;
using System.Collections.Generic;
//Лаб 3
class GradientDescentMethod : OptMethodND
{
    int iterations;
    double epsilonY;
    double epsilonX;
    OptMethod1D method;
    public void setParameters(int iter, double epsY, double epsX)
    {
        iterations = iter;
        epsilonY = epsY;
        epsilonX = epsX;
    }
    public void setMethod(OptMethod1D method)
    {
        this.method = method;
    }
    public static void normalize(ref double[] vec)
    {
        double sum = 0.0f;
        for (int j = 0; j < vec.Length; j++)
        {
            sum += vec[j] * vec[j];
        }
        sum = (double)Math.Sqrt(sum);
        for (int j = 0; j < vec.Length; j++)
        {
            vec[j] /= sum;
        }
    }
    public override OptMethodResult getMinimum(FunctionND func, int solutionCount)
    {
        LimitedQueue<MinPointND> queue = new LimitedQueue<MinPointND>(solutionCount);
        Vector<double> x = Vector<double>.Build.Dense(func.Dimensions);
        Random r = new Random();
        //генерация начальной точки
        for (int i = 0; i < func.Dimensions; i++)
            x[i] = Utils.mix(minBound[i], maxBound[i], (double)r.NextDouble());
        Vector<double> lastX = Vector<double>.Build.DenseOfVector(x);
        double lastY = func.exec(x.AsArray());
        queue.Add(new MinPointND(x.AsArray(),lastY));
        int iter = 0;
        for (; iter < iterations; iter++)
        {
            Vector<double> grad = Vector<double>.Build.Dense(func.getGradient(x.AsArray()));
            grad = grad.Normalize(2);
            //Вычисление ограничений параметра lambda
            double t0, t1;
            Utils.intersectHyperCube(x.AsArray(), grad.AsArray(), minBound, maxBound, func.Dimensions, out t0, out t1);
            //Одномерная оптимизация f(x-l*grad);
            MinPointND mp = method.getMinimum(t0, t1, new Function1DNDAdapter(func, x, grad));
            Vector<double> dx = mp.x[0] * grad;
            x += dx;
            double y = lastY;
            lastY = mp.y;
            queue.Add(new MinPointND(x.AsArray(), lastY));
            if (dx.L2Norm() < epsilonX)
            {
                break;
            }
            if (Math.Abs(lastY - y) < epsilonY)
            {
                break;
            }
            /*
            Условия останова
            |dx|<eps
            |f(x)-f(x+dx)|<eps
            |grad(f(x))|<eps
            */
        }
        return new OptMethodResult(queue.ToList(),iter);
    }
    public override MinPointND getMinimum(FunctionND func)
    {
        Vector<double> x = Vector<double>.Build.Dense(func.Dimensions);
        Random r = new Random();
        //генерация начальной точки
        for (int i = 0; i < func.Dimensions; i++)
            x[i] = Utils.mix(minBound[i], maxBound[i], (double)r.NextDouble());
        Vector<double> lastX = Vector<double>.Build.DenseOfVector(x);
        double lastY = func.exec(x.AsArray());
        for (int i = 0; i < iterations; i++)
        {
            Vector<double> grad=Vector<double>.Build.Dense(func.getGradient(x.AsArray()));
            grad=grad.Normalize(2);
            //Вычисление ограничений параметра lambda
            double t0,t1;
            Utils.intersectHyperCube(x.AsArray(),grad.AsArray(),minBound,maxBound,func.Dimensions,out t0,out t1);
            //Одномерная оптимизация f(x-l*grad);
            MinPointND mp = method.getMinimum(t0,t1,new Function1DNDAdapter(func,x,grad));
            Vector<double> dx = mp.x[0] * grad;
            x += dx;
            double y = lastY;
            lastY = mp.y;
            if (dx.L2Norm() < epsilonX)
            {
                break;
            }
            if (Math.Abs(lastY - y) < epsilonY)
            {
                break;
            }
            /*
            Условия останова
            |dx|<eps
            |f(x)-f(x+dx)|<eps
            |grad(f(x))|<eps
            */
        }
        return new MinPointND(x.AsArray(), lastY);
    }
}
