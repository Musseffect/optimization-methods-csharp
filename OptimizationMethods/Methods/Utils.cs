using System;

class Utils
{
    static public void Swap<T>(ref T a, ref T b)
    {
        T c = a;
        a = b;
        b = c;
    }
    static public void intersectHyperCube(double[] rayOrigin, double[] rayDirection, double[] minBounds, double[] maxBounds, int dimensions, out double t0, out double t1)
    {
        double tmin, tmax;
        tmin = (minBounds[0] - rayOrigin[0]) / rayDirection[0];
        tmax = (maxBounds[0] - rayOrigin[0]) / rayDirection[0];
        if (tmin > tmax)
            Swap(ref tmin,ref tmax);
        for (int i = 1; i < dimensions; i++)
        {
            double _tmin = (minBounds[i] -rayOrigin[i]) / rayDirection[i];
            double _tmax = (maxBounds[i] - rayOrigin[i]) / rayDirection[i];
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
    static public double mix(double a, double b, double value)
    {
        return b * value + a * (1.0f-value);
    }
}
