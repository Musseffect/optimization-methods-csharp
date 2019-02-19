using OptimizationMethods;
using System;
using System.Collections.Generic;
//Лаб 8
class SwarmMethod : OptMethodND
{

    int swarmSize;
    double w;
    double phiP;
    double phiG;
    int iterations;
    public int SwarmSize { get { return swarmSize; } set {swarmSize=value; } }
    public double W { get { return w;}set { w = value; } }
    public double PhiP { get { return phiP; }set { phiP = value; } }
    public double PhiG { get { return phiG; } set { phiG = value; } }
    public int Iterations { get { return iterations; } set { iterations = value; } }
    public override OptMethodResult getMinimum(FunctionND func, int solutions)
    {
        Random r = new Random();
        double[][] x = new double[swarmSize][];
        double[][] v = new double[swarmSize][];

        //инициализация координат частиц
        for (int j = 0; j < swarmSize; j++)
        {
            x[j] = new double[func.Dimensions];
            for (int k = 0; k < func.Dimensions; k++)
                x[j][k] = Utils.mix(minBound[k], maxBound[k], (double)r.NextDouble());
        }
        int bestIndex = 0;
        double minF = func.exec(x[0]);
        for (int j = 1; j < swarmSize; j++)
        {
            double f = func.exec(x[j]);
            if (f < minF)
            {
                bestIndex = j;
                minF = f;
            }
        }
        double bestF = minF;
        double[] bestState = new double[func.Dimensions];
        Array.Copy(x[bestIndex], bestState, func.Dimensions);
        for (int j = 0; j < swarmSize; j++)
        {
            v[j] = new double[func.Dimensions];
            for (int k = 0; k < func.Dimensions; k++)
                v[j][k] = Utils.mix(minBound[k] - maxBound[k], maxBound[k] - minBound[k], (double)r.NextDouble());
        }
        int i = 0;
        for (; i < iterations; i++)
        {
            //Обновить скорость
            for (int j = 0; j < swarmSize; j++)
            {
                for (int k = 0; k < func.Dimensions; k++)
                {
                    v[j][k] = w * v[j][k] + phiP * (double)r.NextDouble() * (x[bestIndex][k] - x[j][k]) +
                        phiG * (double)r.NextDouble() * (bestState[k] - x[j][k]);
                }
            }
            //Обновить положения
            for (int j = 0; j < swarmSize; j++)
            {
                for (int k = 0; k < func.Dimensions; k++)
                {
                    x[j][k] += v[j][k];
                    if (x[j][k] > maxBound[k] || x[j][k] < minBound[k])
                    {
                        for (int m = 0; m < func.Dimensions; m++)
                            x[j][m] = Utils.mix(minBound[m], maxBound[m], (double)r.NextDouble());
                        break;
                    }
                }
            }
            //найти лучшую на интерации точку
            bestIndex = 0;
            minF = func.exec(x[0]);
            for (int j = 1; j < swarmSize; j++)
            {
                double f = func.exec(x[j]);
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
                bestF = minF;
            }
        }
        //вернуть точку bestState
        return new OptMethodResult(new List<MinPointND> { new MinPointND(bestState, bestF) },i);
    }
    public override MinPointND getMinimum(FunctionND func)
    {
        Random r = new Random();
        double[][] x = new double[swarmSize][];
        double[][] v = new double[swarmSize][];

        //инициализация координат частиц
        for (int j = 0; j < swarmSize; j++)
        {
            x[j] = new double[func.Dimensions];
            for (int k = 0; k < func.Dimensions; k++)
                x[j][k] = Utils.mix(minBound[k], maxBound[k], (double)r.NextDouble());
        }
        int bestIndex = 0;
        double minF = func.exec(x[0]);
        for (int j = 1; j < swarmSize; j++)
        {
            double f = func.exec(x[j]);
            if (f < minF)
            {
                bestIndex = j;
                minF = f;
            }
        }
        double bestF = minF;
        double[] bestState = new double[func.Dimensions];
        Array.Copy(x[bestIndex],bestState,func.Dimensions);
        for (int j = 0; j < swarmSize; j++)
        {
            v[j] = new double[func.Dimensions];
            for (int k = 0; k < func.Dimensions; k++)
                v[j][k] = Utils.mix(minBound[k]-maxBound[k], maxBound[k]-minBound[k], (double)r.NextDouble());
        }
        for (int i = 0; i < iterations; i++)
        {
            //Обновить скорость
            for (int j = 0; j < swarmSize; j++)
            {
                for (int k = 0; k < func.Dimensions; k++)
                {
                    v[j][k] = w * v[j][k] + phiP * (double)r.NextDouble() * (x[bestIndex][k] - x[j][k]) +
                        phiG * (double)r.NextDouble() * (bestState[k] - x[j][k]);
                }
            }
            //Обновить положения
            for (int j = 0; j < swarmSize; j++)
            {
                for (int k = 0; k < func.Dimensions; k++)
                {
                    x[j][k] += v[j][k];
                    if (x[j][k] > maxBound[k] || x[j][k] < minBound[k])
                    {
                        for (int m = 0; m < func.Dimensions; m++)
                            x[j][m] = Utils.mix(minBound[m], maxBound[m], (double)r.NextDouble());
                        break;
                    }
                }
            }
            //найти лучшую на интерации точку
            bestIndex = 0;
            minF = func.exec(x[0]);
            for (int j = 1; j < swarmSize; j++)
            {
                double f = func.exec(x[j]);
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
                bestF = minF;
            }
        }
        //вернуть точку bestState
        return new MinPointND(bestState,bestF);
    }
}