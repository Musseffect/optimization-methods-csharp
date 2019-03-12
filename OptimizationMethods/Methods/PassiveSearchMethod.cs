using System.Collections.Generic;

namespace OptimizationMethods
{
    //Лаб 1
    class PassiveSearchMethod : OptMethod1D
    {
        int pointCount;
        public PassiveSearchMethod()
        {
            pointCount = 0;
        }
        public void setParameters(int pointCount)
        {
            this.pointCount = pointCount;
        }
        public override MinPointND getMinimum(double a, double b, Function1D func)
        {
            double interval = getError(a, b);
            double minY = func.exec(a);
            double minX = a;
            for (int i = 1; i <= pointCount+1; i++)
            {
                double x = a + interval * i;
                double y = func.exec(x);
                if (y < minY)
                {
                    minY = y;
                    minX = x;
                }
            }
            return new MinPointND(new double[] { minX },minY);
        }
        public double getError(double a, double b)
        {
            return (b - a) / (pointCount + 1);
        }

        public override MinPointND getMinimum(double a, double b, Function1D func,out int iters)
        {
            double interval = getError(a, b);
            double minY = func.exec(a);
            double minX = a;
            for (int i = 1; i <= pointCount + 1; i++)
            {
                double x = a + interval * i;
                double y = func.exec(x);
                if (y < minY)
                {
                    minY = y;
                    minX = x;
                }
            }
            iters = pointCount;
            return new MinPointND(new double[] { minX }, minY);
        }

    }
}
