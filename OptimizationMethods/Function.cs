using MathNet.Numerics.LinearAlgebra;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static OptimizationMethods.Compiler.ASTCompiler;

namespace OptimizationMethods
{
    abstract class Function1D
    {
        public abstract float exec(float x);
    }
    abstract class FunctionND
    {
        protected int dimensions;
        public int Dimensions{get;protected set;}
        public abstract float exec(float[] x);
        public abstract float[] getGradient(float[] x);
        public abstract float[] getHessiMatrix(float[] x);
        public float[] getNumericGradient(float[] x, float delta)
        {
            float[] grad=new float[dimensions];
            float sum = 0.0f;
            for (int i = 0; i < dimensions; i++)
            {
                float xi = x[i];
                x[i] = xi - delta;
                grad[i]=exec(x);
                x[i] = xi + delta;
                grad[i] = exec(x)-grad[i];
                x[i] = xi;
                sum += grad[i];
            }
            for (int i = 0; i < dimensions; i++)
            {
                grad[i] /= sum;
            }
            return grad;
        }
    }
    class SymbolicFunction1D:Function1D
    {
        ExpressionStack expression;
        public override float exec(float x)
        {
            expression.set("x",x);
            return expression.execute();
        }
    }
    class Function1DNDAdapter:Function1D
    {
        FunctionND func;
        Vector<float> x0;
        Vector<float> dx;
        public Function1DNDAdapter(FunctionND func, Vector<float> x0, Vector<float> dx)
        {
            this.func = func;
            this.x0 = x0;
            this.dx = dx;
        }
        public override float exec(float x)
        {
            return func.exec((x0+dx*x).AsArray());
        }
    }
    class SymbolicFunctionND : FunctionND
    {
        ExpressionStack expression;
        List<ExpressionStack> derivatives;
        List<ExpressionStack> secondDerivatives;
        string[] variables;
        public override float exec(float[] vars)
        {
            for(int i=0;i< Dimensions; i++)
                expression.set(variables[i], vars[i]);
            return expression.execute();
        }
        public SymbolicFunctionND()
        {
        }
        public override float[] getGradient(float[] x)
        {
            float[] result = new float[Dimensions];
            for (int i = 0; i < Dimensions; i++)
            {
                for (int j = 0; j < Dimensions; j++)
                    derivatives[i].set(variables[j], x[j]);
                result[i]= derivatives[i].execute();
            }
            return result;
        }
        public override float[] getHessiMatrix(float[] x)//column major flat matrix
        {
            float[] result = new float[Dimensions*Dimensions];
            for (int i = 0; i < Dimensions; i++)
            {
                for (int j = 0; j < Dimensions; j++)
                {
                    int index = i * Dimensions + j;
                    for (int k = 0; k < Dimensions; k++)
                        secondDerivatives[index].set(variables[k], x[k]);
                    result[i] = secondDerivatives[index].execute();
                }
            }
            return result;
        }
    }
}
