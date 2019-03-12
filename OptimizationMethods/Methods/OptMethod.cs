using OptimizationMethods;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OptimizationMethods
{
    abstract class OptMethod1D
    {
        public abstract MinPointND getMinimum(double a, double b, Function1D func);
        public abstract MinPointND getMinimum(double a, double b, Function1D func,out int iters);
    }
    abstract class OptMethodND
    {
        protected double[] minBound;
        protected double[] maxBound;
        public abstract OptMethodResult getMinimum(FunctionND func,int solutions);
        public abstract MinPointND getMinimum(FunctionND func);
        public void setBounds(double[] min, double[] max)
        {
            minBound = min;
            maxBound = max;
        }
    }
    public struct MinPointND
    {
        public double[] x;
        public double y;
        public MinPointND(double[] x, double y)
        {
            this.x = new double[x.Length];
            Array.Copy(x,this.x, x.Length);
            this.y = y;
        }
    }
    struct OptMethodResult
    {
        public List<MinPointND> solutions;
        public int iterations;
        public OptMethodResult(List<MinPointND> solutions, int iterations)
        {
            this.solutions = solutions;
            this.iterations = iterations;
        }
    }
}
