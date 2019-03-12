using System;
using System.Collections.Generic;

namespace OptimizationMethods
{
    class GoldenRuleMethod : OptMethod1D
    {
        double epsilon;
        int iterations;
        public void setParameters(double epsilon, int iterations)
        {
            this.epsilon = epsilon;
            this.iterations = iterations;
        }
        public override MinPointND getMinimum(double a, double b, Function1D func)
        {
            double x1;
            double x2;
            double t = (1.0f + (double)(Math.Sqrt(5.0))) / 2.0f;

            double fx1;
            double fx2;

            x2 = a + (b - a) / t;
            fx2 = func.exec(x2);
            x1 = a + b - x2;
            fx1 = func.exec(x1);
            int i = 0;
            while (i<iterations)
            {
                if (fx1 < fx2)
                {
                    b = x2;
                    x2 = x1;
                    fx2 = fx1;
                    x1 = a + b - x2;
                    fx1 = func.exec(x1);
                }
                else {
                    a = x1;
                    x1 = x2;
                    fx1 = fx2;
                    x2 = a + (b - a) / t;
                    fx2 = func.exec(x2);
                }
                if (Math.Abs(a - b) < epsilon)
                {
                    break;
                }
                i++;
            }
            double x = (a + b) * 0.5f;
            return new MinPointND(new double[] { x }, func.exec(x));
        }

        public override MinPointND getMinimum(double a, double b, Function1D func,out int iters)
        {
            double x1;
            double x2;
            double t = (1.0f + (double)(Math.Sqrt(5.0))) / 2.0f;

            double fx1;
            double fx2;
            List<MinPointND> list = new List<MinPointND>();
            x2 = a + (b - a) / t;
            fx2 = func.exec(x2);
            x1 = a + b - x2;
            fx1 = func.exec(x1);
            int i = 0;
            while (i < iterations)
            {
                if (fx1 < fx2)
                {
                    b = x2;
                    x2 = x1;
                    fx2 = fx1;
                    x1 = a + b - x2;
                    fx1 = func.exec(x1);
                }
                else
                {
                    a = x1;
                    x1 = x2;
                    fx1 = fx2;
                    x2 = a + (b - a) / t;
                    fx2 = func.exec(x2);
                }
                if (Math.Abs(a - b) < epsilon)
                {
                    double _x = (a + b) * 0.5f;
                    iters = i;
                    return new MinPointND(new double[] { _x }, func.exec(_x));
                }
                i++;
            }
            iters = i;
            double x = (a + b) * 0.5f;
            return new MinPointND(new double[] { x }, func.exec(x));
        }
    }
}
