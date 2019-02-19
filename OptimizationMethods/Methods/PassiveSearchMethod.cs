namespace OptimizationMethods
{
    //Лаб 1
    class PassiveSearchMethod : OptMethod1D
    {
        int pointCount;
        public int PointCount { private get { return pointCount; } set { pointCount = value; } }
        public PassiveSearchMethod()
        {
            pointCount = 0;
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
    }
}
