using MathNet.Numerics.Distributions;
using MathNet.Numerics.LinearAlgebra;
using OptimizationMethods;
using System;
//Лаб 7
class BestTrialMethod : OptMethodND
{
    int iterations;
    int vectorCount;
    double step;
    double epsilonY;
    double epsilonX;
    OptMethod1D method;
    public void setMethod(OptMethod1D method)
    {
        this.method = method;
    }
    public void setParameters(int iter, int vectorCount, double step,double epsX,double epsY)
    {
        iterations = iter;
        this.vectorCount = vectorCount;
        this.step = step;
        epsilonX = epsX;
        epsilonY = epsY;
    }
    static public double[] genUnitVector(int dimensions)
    {
        Normal normalDist = new Normal(0.0f,1.0f);
        double[] result = new double[dimensions];
        double norm = 0.0f;
        for (int i = 0; i < dimensions; i++)
        {
            result[i] = (double)normalDist.Sample();
            norm += result[i]* result[i];
        }
        norm = Math.Sqrt(norm);
        for (int i = 0; i < dimensions; i++)
        {
            result[i] /= (double)norm;
        }
        return result;
    }
    public override OptMethodResult getMinimum(FunctionND func,int solutions)
    {
        LimitedQueue<MinPointND> queue = new LimitedQueue<MinPointND>(solutions);
        Vector<double> x = Vector<double>.Build.Dense(func.Dimensions);
        Vector<double> bestPoint = x;
        double minBest = double.MaxValue;
        Random r = new Random();
        //инициализация x0
        for (int i = 0; i < func.Dimensions; i++)
            x[i] = Utils.mix(minBound[i], maxBound[i], (double)r.NextDouble());
        minBest = func.exec(x.AsArray());
        double oldY = minBest;
        queue.Add(new MinPointND(x.AsArray(),oldY));
        int iter = 0;
        for (; iter < iterations; iter++)
        {
            double minValue = double.MaxValue;
            Vector<double> _x = null;
            Vector<double> bestVector = _x;
            //выбор вектора с наименьшим значением функции
            for (int j = 0; j < vectorCount; j++)
            {
                Vector<double> vec = Vector<double>.Build.Dense(genUnitVector(func.Dimensions));
                _x = x + vec * step;
                double fValue = func.exec(_x.AsArray());
                if (fValue < minValue)
                {
                    minValue = fValue;
                    bestVector = vec;
                }
            }
            //минимизация f(x(k-1)+l*e(min))

            double t0, t1;
            Utils.intersectHyperCube(x.AsArray(), bestVector.AsArray(), minBound, maxBound, func.Dimensions, out t0, out t1);
            MinPointND mp = method.getMinimum(t0, t1, new Function1DNDAdapter(func, x, bestVector));
            Vector<double> dx = mp.x[0] * bestVector;
            _x = x + dx;
            double fMinValue = mp.y;
            double y = mp.y;
            if (fMinValue < minBest)
            {
                minBest = fMinValue;
                x = _x;
                bestPoint = _x;
                queue.Add(new MinPointND(_x.AsArray(), y));
            }
            if (dx.L2Norm() < epsilonX)
            {
                break;
            }
            if (Math.Abs(y - oldY) < epsilonY)
            {
                break;
            }
            oldY = y;
        }
        return new OptMethodResult(queue.ToList(), iter);
    }
    public override MinPointND getMinimum( FunctionND func)
    {
        Vector<double> x=Vector<double>.Build.Dense(func.Dimensions);
        Vector<double> bestPoint = x;
        double minBest =double.MaxValue;
        Random r = new Random();
        //инициализация x0
        for (int i=0;i<func.Dimensions;i++)
            x[i]= Utils.mix(minBound[i], maxBound[i], (double)r.NextDouble());
        minBest = func.exec(x.AsArray());
        double oldY=minBest;
        for (int i = 0; i < iterations; i++)
        {
            double minValue=double.MaxValue;
            Vector<double> _x=null;
            Vector<double> bestVector=_x;
            //выбор вектора с наименьшим значением функции
            for (int j = 0; j < vectorCount; j++)
            {
                Vector<double> vec=Vector<double>.Build.Dense(genUnitVector(func.Dimensions));
                _x = x + vec * step;
                double fValue =func.exec(_x.AsArray());
                if (fValue < minValue)
                {
                    minValue = fValue;
                    bestVector = vec;
                }
            }
            //минимизация f(x(k-1)+l*e(min))

            double t0, t1;
            Utils.intersectHyperCube(x.AsArray(), bestVector.AsArray(), minBound, maxBound, func.Dimensions, out t0, out t1);
            MinPointND mp = method.getMinimum(t0, t1, new Function1DNDAdapter(func, x, bestVector));
            Vector<double> dx = mp.x[0] * bestVector;
            _x = x + dx;
            double fMinValue = func.exec(_x.AsArray());
            if (fMinValue < minBest)
            {
                minBest = fMinValue;
                x = _x;
                bestPoint = _x;
            }
            if (dx.L2Norm() < epsilonX)
            {
                break;
            }
            double y = mp.y;
            if (Math.Abs(y - oldY) < epsilonY)
            {
                break;
            }
            oldY = y;
        }
        return new MinPointND(bestPoint.AsArray(), minBest);
        /*
         g-величина пробного шага
         сгенерировать x0
         зададим начальную длину шага l0 
         пока k<N
         {
            x(k-1)
            Сгенерировать m случайных векторов ei
            Сделать пробные шаги в направлениях g*ei
            выбираем направление с наименьшим значением функции
            делаем шаг в этом направлении l*e(min)
            l - результат минимизации f(x(k-1)+l*e(min))
         }
         */
    }

   
}
