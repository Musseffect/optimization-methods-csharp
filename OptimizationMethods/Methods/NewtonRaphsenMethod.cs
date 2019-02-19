using MathNet.Numerics.LinearAlgebra;
using OptimizationMethods;
using System;
//Лаб 5
class NewtonRaphsenMethod : OptMethodND
{
    int iterations;
    double epsilonY;
    double epsilonX;
    OptMethod1D method;
    void setMethod(OptMethod1D method)
    {
        this.method = method;
    }
    public void setParameters(int iter, double epsY, double epsX)
    {
        iterations = iter;
        epsilonY = epsY;
        epsilonX = epsX;
    }
    public override OptMethodResult getMinimum(FunctionND func, int solutionCount)
    {
        LimitedQueue<MinPointND> queue = new LimitedQueue<MinPointND>(solutionCount);
        Matrix<double> n = Matrix<double>.Build.DenseIdentity(func.Dimensions);
        Vector<double> x = Vector<double>.Build.Dense(func.Dimensions);
        Vector<double> oldGrad = Vector<double>.Build.Dense(func.getGradient(x.AsArray()));
        double oldY = func.exec(x.AsArray());
        queue.Add(new MinPointND(x.AsArray(),oldY));
        int iter;
        for (iter = 0; iter < iterations; iter++)
        {
            //xk+1=xk-lambda*n(xk)*grad(xk);
            Vector<double> vec = n * oldGrad;
            double t0, t1;
            Utils.intersectHyperCube(x.AsArray(), vec.AsArray(), minBound, maxBound, func.Dimensions, out t0, out t1);
            MinPointND mp = method.getMinimum(t0, t1, new Function1DNDAdapter(func, x, vec));
            Vector<double> dx = vec * mp.x[0];
            x = x + vec * mp.x[0];
            double y = oldY;
            oldY = mp.y;
            queue.Add(new MinPointND(x.AsArray(), oldY));
            if (dx.L2Norm() < epsilonX)
            {
                break;
            }
            if (Math.Abs(y - oldY) < epsilonY)
            {
                break;
            }
            Vector<double> grad = Vector<double>.Build.Dense(func.getGradient(x.AsArray()));
            Vector<double> deltag = grad - oldGrad;
            Vector<double> ng = n * deltag;
            double denum = deltag * n * deltag;
            n = n - Vector<double>.OuterProduct(ng, ng) / denum;

        }
        return new OptMethodResult(queue.ToList(),iter);
    }
    public override MinPointND getMinimum(FunctionND func)
    {
        Matrix<double> n = Matrix<double>.Build.DenseIdentity(func.Dimensions);
        Vector<double> x = Vector<double>.Build.Dense(func.Dimensions);
        Vector<double> oldGrad= Vector<double>.Build.Dense(func.getGradient(x.AsArray()));
        double oldY = func.exec(x.AsArray());
        for (int i = 0; i < iterations; i++)
        {
            //xk+1=xk-lambda*n(xk)*grad(xk);
            Vector<double> vec = n* oldGrad;
            double t0, t1;
            Utils.intersectHyperCube(x.AsArray(), vec.AsArray(), minBound, maxBound, func.Dimensions, out t0, out t1);
            MinPointND mp = method.getMinimum(t0, t1, new Function1DNDAdapter(func, x, vec));
            Vector<double> dx = vec * mp.x[0];
            x = x +vec*mp.x[0];
            double y = oldY;
            oldY = mp.y;
            if (dx.L2Norm() < epsilonX)
            {
                break;
            }
            if (Math.Abs(y - oldY) < epsilonY)
            {
                break;
            }
            Vector<double> grad= Vector<double>.Build.Dense(func.getGradient(x.AsArray()));
            Vector<double> deltag =grad-oldGrad;
            Vector<double> ng = n * deltag;
            double denum = deltag*n*deltag;
            n = n - Vector<double>.OuterProduct(ng, ng)/denum;

        }
        return new MinPointND(x.AsArray(),func.exec(x.AsArray()));
    }
}
