using MathNet.Numerics.LinearAlgebra;
using OptimizationMethods;
using System;
//Лаб 2
class GaussMethod : OptMethodND
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
    public override OptMethodResult getMinimum(FunctionND func, int solutionCount)
    {
        LimitedQueue<MinPointND> queue = new LimitedQueue<MinPointND>(solutionCount);
        double[] x = new double[func.Dimensions];
        double[] lastX = new double[func.Dimensions];
        Random r = new Random();
        //генерация начальной точки
        for (int k = 0; k < func.Dimensions; k++)
            x[k] = Utils.mix(minBound[k], maxBound[k], (double)r.NextDouble());
        Array.Copy( x,lastX, func.Dimensions);
        double lastY = func.exec(x);
        queue.Add(new MinPointND ( x,lastY));
        int i=0;
        for (i = 0; i < iterations; i++)
        {
            //оптимизация по каждой координате
            for (int j = 0; j < func.Dimensions; j++)
            {
                Vector<double> xv = Vector<double>.Build.Dense(func.Dimensions, 0.0f);
                Vector<double> dx = Vector<double>.Build.Dense(func.Dimensions, 0.0f);
                dx[j] = 1.0f;
                MinPointND mp = method.getMinimum(minBound[j], maxBound[j], new Function1DNDAdapter(func, xv, dx));
                x[j] = mp.x[0];
            }
            double fValue = func.exec(x);
            //критерий останова по разнице значений функции на соседних итерациях
            queue.Add(new MinPointND(x, fValue));
            double dY = Math.Abs(fValue - lastY);
            lastY = fValue;
            if (dY <= epsilonY)
            {
                break;
            }
            double dX = 0.0f;
            //критерий останова по норме разницы координат приближений на соседних итерациях
            for (int j = 0; j < func.Dimensions; j++)
            {
                dX += (double)Math.Pow(x[j] - lastX[j], 2.0);
            }
            dX = (double)Math.Sqrt(dX);
            if (dX <= epsilonX)
            {
                break;
            }
            Array.Copy(x,lastX, func.Dimensions);
        }
        return new OptMethodResult ( queue.ToList(),i);
    }
    public override MinPointND getMinimum(FunctionND func)
    {
        double[] x = new double[func.Dimensions];
        double[] lastX = new double[func.Dimensions];
        Random r = new Random();
        //генерация начальной точки
        for (int i = 0; i < func.Dimensions; i++)
            x[i] = Utils.mix(minBound[i], maxBound[i], (double)r.NextDouble());
        Array.Copy(x, lastX,func.Dimensions);
        double lastY=func.exec(x);
        for (int i = 0; i < iterations; i++)
        {
            //оптимизация по каждой координате
            for (int j = 0; j < func.Dimensions; j++)
            {
                Vector<double> xv=Vector<double>.Build.Dense(func.Dimensions,0.0f);
                Vector<double> dx= Vector<double>.Build.Dense(func.Dimensions, 0.0f);
                dx[j] = 1.0f;
                MinPointND mp=method.getMinimum(minBound[j],maxBound[j],new Function1DNDAdapter(func,xv,dx));
                x[j]=mp.x[0];
            }
            double fValue=func.exec(x);
            //критерий останова по разнице значений функции на соседних итерациях
            double dY=Math.Abs(fValue-lastY);
            lastY = fValue;
            if (dY <= epsilonY)
            {
                break;
            }
            double dX=0.0f;
            //критерий останова по норме разницы координат приближений на соседних итерациях
            for (int j = 0; j < func.Dimensions; j++)
            {
                dX += (double)Math.Pow(x[j]-lastX[j],2.0);
            }
            dX = (double)Math.Sqrt(dX);
            if (dX <= epsilonX)
            {
                break;
            }
            Array.Copy(x, lastX, func.Dimensions);
        }
        return new MinPointND(x, lastY);
    }
}
