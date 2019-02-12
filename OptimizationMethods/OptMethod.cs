using MathNet.Numerics.Distributions;
using MathNet.Numerics.LinearAlgebra;
using OptimizationMethods;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OptimizationMethods
{
    struct MinPoint
    {
        public float x;
        public float y;
        public MinPoint(float x, float y)
        {
            this.x = x;
            this.y = y;
        }
    }
    abstract class OptMethod1D
    {
        public abstract MinPoint getMinimum(float a, float b, Function1D func);
    }
    //Лаб 1
    class PassiveSearchMethod : OptMethod1D
    {
        int pointCount;
        public int PointCount { private get; set; }
        public PassiveSearchMethod()
        {
            pointCount = 0;
        }
        public override MinPoint getMinimum(float a, float b, Function1D func)
        {
            float interval = getError(a, b);
            float minY = func.exec(a);
            float minX = a;
            for (int i = 1; i <= pointCount+1; i++)
            {
                float x = a + interval * i;
                float y = func.exec(x);
                if (y < minY)
                {
                    y = minY;
                    minX = x;
                }
            }
            return new MinPoint(minX,minY);
        }
        public float getError(float a, float b)
        {
            return (a - b) / (pointCount + 1);
        }
    }
    class GoldenRuleMethod : OptMethod1D
    {
        float epsilon;
        public float Epsilon { private get { return epsilon; } set { epsilon = value; } }
        int iterations;
        public int Iterations { private get { return iterations; } set { iterations = value; } }
        public override MinPoint getMinimum(float a, float b, Function1D func)
        {
            float x1;
            float x2;
            float t = (1.0f + (float)(Math.Sqrt(5.0))) / 2.0f;

            float fx1;
            float fx2;

            x2 = a + (b - a) / t;
            fx2 = func.exec(x2);
            int i = 0;
            while (i<iterations)
            {
                x1 = a + b - x2;
                fx1 = func.exec(x1);
                if (fx1 < fx2)
                {
                    b = x2;
                    x2 = x1;
                    fx2 = fx1;
                }
                else {
                    a = x1;
                }
                if (Math.Abs(a - b) < epsilon)
                {
                    float x = (a + b) * 0.5f;
                    return new MinPoint(x,func.exec(x));
                }
                i++;
            }
            throw new Exception("");
        }
    }
}

struct MinPointND
{
    float []x;
    float y;
    public MinPointND(float[] x, float y)
    {
        this.x = x;
        this.y = y;
    }
}

class Utils
{
    static public void Swap<T>(ref T a, ref T b)
    {
        T c = a;
        a = b;
        b = c;
    }
    static public void intersectHyperCube(float[] rayOrigin, float[] rayDirection, float[] minBounds, float[] maxBounds, int dimensions, out float t0, out float t1)
    {
        float tmin, tmax;
        tmin = (minBounds[0] - rayOrigin[0]) / rayDirection[0];
        tmax = (maxBounds[0] - rayOrigin[0]) / rayDirection[0];
        if (tmin > tmax)
            Swap(ref tmin,ref tmax);
        for (int i = 1; i < dimensions; i++)
        {
            float _tmin = (minBounds[i] -rayOrigin[i]) / rayDirection[i];
            float _tmax = (maxBounds[i] - rayOrigin[i]) / rayDirection[i];
            if (_tmin > _tmax)
                Swap(ref _tmin, ref _tmax);
            if (tmin > _tmax || _tmin > tmax)
                throw new Exception("Impossible case. Ray is not pointing in hyperbox.");
            tmin = Math.Max(tmin,_tmin);
            tmax = Math.Min(tmax, _tmax);
        }
        t0 = tmin;
        t1 = tmax;
    }
    static public float mix(float a, float b, float value)
    {
        return b * value + a * (1.0f-value);
    }
}

abstract class OptMethodND
{
    protected float[] minBound;
    protected float[] maxBound;
    public abstract MinPointND getMinimum( FunctionND func);
}

//Лаб 2
class GaussMethod : OptMethodND
{
    int iterations;
    float epsilonY;
    float epsilonX;
    int pointCount;
    OptMethod1D method;
    void setMethod(OptMethod1D method)
    {
        this.method = method;
    }
    public override MinPointND getMinimum(FunctionND func)
    {
        float[] x = new float[func.Dimensions];
        float[] lastX = new float[func.Dimensions];
        Random r = new Random();
        //генерация начальной точки
        for (int i = 0; i < func.Dimensions; i++)
            x[i] = Utils.mix(minBound[i], maxBound[i], (float)r.NextDouble());
        Array.Copy(lastX,x,func.Dimensions);
        float lastY=func.exec(x);
        for (int i = 0; i < iterations; i++)
        {
            //оптимизация по каждой координате
            for (int j = 0; j < func.Dimensions; j++)
            {
                Vector<float> xv=Vector<float>.Build.Dense(func.Dimensions,0.0f);
                Vector<float> dx= Vector<float>.Build.Dense(func.Dimensions, 0.0f);
                dx[j] = 1.0f;
                MinPoint mp=method.getMinimum(minBound[j],maxBound[j],new Function1DNDAdapter(func,xv,dx));
                x[j]=mp.x;
            }
            float fValue=func.exec(x);
            //критерий останова по разнице значений функции на соседних итерациях
            float dY=Math.Abs(fValue-lastY);
            if (dY <= epsilonY)
            {
                return new MinPointND(x,fValue);
            }
            lastY = fValue;
            float dX=0.0f;
            //критерий останова по норме разницы координат приближений на соседних итерациях
            for (int j = 0; j < func.Dimensions; j++)
            {
                dX += (float)Math.Pow(x[j]-lastX[j],2.0);
            }
            dX = (float)Math.Sqrt(dX);
            if (dX <= epsilonY)
            {
                return new MinPointND(x, fValue);
            }
            Array.Copy(lastX, x, func.Dimensions);
        }
        return new MinPointND(x, lastY);
    }
}


//Лаб 3
class GradientDescentMethod : OptMethodND
{
    float epsilon;
    int iterations;
    int pointCount;
    OptMethod1D method;
    void setMethod(OptMethod1D method)
    {
        this.method = method;
    }
    public static void normalize(ref float[] vec)
    {
        float sum = 0.0f;
        for (int j = 0; j < vec.Length; j++)
        {
            sum += vec[j] * vec[j];
        }
        sum = (float)Math.Sqrt(sum);
        for (int j = 0; j < vec.Length; j++)
        {
            vec[j] /= sum;
        }
    }
    public override MinPointND getMinimum(FunctionND func)
    {
        Vector<float> x = Vector<float>.Build.Dense(func.Dimensions);
        Random r = new Random();
        //генерация начальной точки
        for (int i = 0; i < func.Dimensions; i++)
            x[i] = Utils.mix(minBound[i], maxBound[i], (float)r.NextDouble());
        Vector<float> lastX = Vector<float>.Build.DenseOfVector(x);
        float lastY = func.exec(x.AsArray());
        for (int i = 0; i < iterations; i++)
        {
            Vector<float> grad=Vector<float>.Build.Dense(func.getGradient(x.AsArray()));
            grad=grad.Normalize(2);
            //Вычисление ограничений параметра lambda
            float t0,t1;
            Utils.intersectHyperCube(x.AsArray(),grad.AsArray(),minBound,maxBound,func.Dimensions,out t0,out t1);
            //Одномерная оптимизация f(x-l*grad);
            MinPoint mp = method.getMinimum(t0,t1,new Function1DNDAdapter(func,x,grad));
            x += mp.x * grad;
            /*
            Условия останова
            |dx|<eps
            |f(x)-f(x+dx)|<eps
            |grad(f(x))|<eps
            */
        }
        return new MinPointND(x.AsArray(), func.exec(x.AsArray()));
    }
}
//Лаб 4
class NewtonMethod : OptMethodND
{
    int iterations;
    float alpha;
    public override MinPointND getMinimum(FunctionND func)
    {
        Vector<float> x = Vector<float>.Build.Dense(func.Dimensions);


        float y = func.exec(x.AsArray());
        for (int i = 0; i < iterations; i++)
        {
            Matrix<float> hessiMatrix = Matrix<float>.Build.Dense(func.Dimensions,func.Dimensions,func.getHessiMatrix(x.AsArray()));
            Vector<float> grad = Vector<float>.Build.Dense(func.getGradient(x.AsArray()));
            grad *=alpha;
            Vector<float> dx;
            try
            {
                dx = hessiMatrix.Solve(-grad);
            } catch (Exception)
            {
                throw new Exception("");
            }
            x += dx;
            y = func.exec(x.AsArray());
        }
        return new MinPointND(x.AsArray(),y);
    }
}
//Лаб 5
class NewtonRaphsenMethod : OptMethodND
{
    int iterations;
    OptMethod1D method;
    void setMethod(OptMethod1D method)
    {
        this.method = method;
    }
    public override MinPointND getMinimum(FunctionND func)
    {
        Matrix<float> n = Matrix<float>.Build.DenseIdentity(func.Dimensions);
        Vector<float> x = Vector<float>.Build.Dense(func.Dimensions);
        Vector<float> oldGrad= Vector<float>.Build.Dense(func.getGradient(x.AsArray()));
        for (int i = 0; i < iterations; i++)
        {
            //xk+1=xk-lambda*n(xk)*grad(xk);
            Vector<float> vec = n* oldGrad;
            float t0, t1;
            Utils.intersectHyperCube(x.AsArray(), vec.AsArray(), minBound, maxBound, func.Dimensions, out t0, out t1);
            MinPoint mp = method.getMinimum(t0, t1, new Function1DNDAdapter(func, x, vec));
            x = x +vec*mp.x;
            Vector<float> grad= Vector<float>.Build.Dense(func.getGradient(x.AsArray()));
            Vector<float> deltag =grad-oldGrad;
            Vector<float> ng = n * deltag;
            float denum = deltag*n*deltag;
            n = n - Vector<float>.OuterProduct(ng, ng)/denum;

        }
        return new MinPointND(x.AsArray(),func.exec(x.AsArray()));
    }
}

//Лаб 7
class BestTrialMethod : OptMethodND
{
    int iterations;
    int vectorCount;
    float step;
    OptMethod1D method;
    void setMethod(OptMethod1D method)
    {
        this.method = method;
    }
    static public float[] genUnitVector(int dimensions)
    {
        Normal normalDist = new Normal(0.0f,1.0f);
        float[] result = new float[dimensions];
        double norm = 0.0f;
        for (int i = 0; i < dimensions; i++)
        {
            result[i] = (float)normalDist.Sample();
            norm += result[i]* result[i];
        }
        norm = Math.Sqrt(norm);
        for (int i = 0; i < dimensions; i++)
        {
            result[i] /= (float)norm;
        }
        return result;
    }
    public override MinPointND getMinimum( FunctionND func)
    {
        Vector<float> x=Vector<float>.Build.Dense(func.Dimensions);
        Vector<float> bestPoint = x;
        float minBest =float.MaxValue;
        Random r = new Random();
        //инициализация x0
        for (int i=0;i<func.Dimensions;i++)
            x[i]= Utils.mix(minBound[i], maxBound[i], (float)r.NextDouble());
        minBest = func.exec(x.AsArray());
        for (int i = 0; i < iterations; i++)
        {
            float minValue=float.MaxValue;
            Vector<float> _x=null;
            Vector<float> bestVector=_x;
            //выбор вектора с наименьшим значением функции
            for (int j = 0; j < vectorCount; j++)
            {
                Vector<float> vec=Vector<float>.Build.Dense(genUnitVector(func.Dimensions));
                _x = x + vec * step;
                float fValue =func.exec(_x.AsArray());
                if (fValue < minValue)
                {
                    minValue = fValue;
                    bestVector = vec;
                }
            }
            //минимизация f(x(k-1)+l*e(min))

            float t0, t1;
            Utils.intersectHyperCube(x.AsArray(), bestVector.AsArray(), minBound, maxBound, func.Dimensions, out t0, out t1);
            MinPoint mp = method.getMinimum(t0, t1, new Function1DNDAdapter(func, x, bestVector));
            _x = x + mp.x * bestVector;

            float fMinValue = func.exec(_x.AsArray());
            if (fMinValue < minBest)
            {
                minBest = fMinValue;
                x = _x;
                bestPoint = _x;
            }
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

//Лаб 8
class SwarmMethod : OptMethodND
{

    int swarmSize;
    float w;
    float phiP;
    float phiG;
    int iterations;
    public int SwarmSize { get { return swarmSize; } set {swarmSize=value; } }
    public float W { get { return w;}set { w = value; } }
    public float PhiP { get { return phiP; }set { phiP = value; } }
    public float PhiG { get { return phiG; } set { phiG = value; } }
    public int Iterations { get { return iterations; } set { iterations = value; } }
    public override MinPointND getMinimum(FunctionND func)
    {
        Random r = new Random();
        float[][] x = new float[swarmSize][];
        float[][] v = new float[swarmSize][];

        //инициализация координат частиц
        for (int j = 0; j < swarmSize; j++)
        {
            x[j] = new float[func.Dimensions];
            for (int k = 0; k < func.Dimensions; k++)
                x[j][k] = Utils.mix(minBound[k], maxBound[k], (float)r.NextDouble());
        }
        int bestIndex = 0;
        float minF = func.exec(x[0]);
        for (int j = 1; j < swarmSize; j++)
        {
            float f = func.exec(x[j]);
            if (f < minF)
            {
                bestIndex = j;
                minF = f;
            }
        }
        float bestF = minF;
        float[] bestState = new float[func.Dimensions];
        Array.Copy(x[bestIndex],bestState,func.Dimensions);
        for (int j = 0; j < swarmSize; j++)
        {
            v[j] = new float[func.Dimensions];
            for (int k = 0; k < func.Dimensions; k++)
                v[j][k] = Utils.mix(minBound[k]-maxBound[k], maxBound[k]-minBound[k], (float)r.NextDouble());
        }
        for (int i = 0; i < iterations; i++)
        {
            //Обновить скорость
            for (int j = 0; j < swarmSize; j++)
            {
                v[j] = new float[func.Dimensions];
                for (int k = 0; k < func.Dimensions; k++)
                {
                    v[j][k] = w * v[j][k] + phiP * (float)r.NextDouble() * (x[bestIndex][k] - x[j][k]) +
                        phiG * (float)r.NextDouble() * (bestState[k] - x[j][k]);
                }
            }
            //Обновить положения
            for (int j = 0; j < swarmSize; j++)
            {
                for (int k = 0; k < func.Dimensions; k++)
                {
                    x[j][k] += v[j][k];
                }
            }
            //найти лучшую на интерации точку
            bestIndex = 0;
            minF = func.exec(x[0]);
            for (int j = 1; j < swarmSize; j++)
            {
                float f = func.exec(x[j]);
                if (f < minF)
                {
                    bestIndex = j;
                    minF = f;
                }
            }
            //обновить глобальную лучшую точку
            if (bestF > minF)
            {
                Array.Copy(x[bestIndex], bestState, func.Dimensions);
            }
        }
        //вернуть точку bestState
        return new MinPointND(bestState,bestF);
    }
}