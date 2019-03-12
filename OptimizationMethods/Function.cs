using MathNet.Numerics.LinearAlgebra;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static OptimizationMethods.Compiler.ASTCompiler;

namespace OptimizationMethods
{
    public abstract class Function1D
    {
        public abstract double exec(double x);
    }
    public abstract class FunctionND
    {
        protected int dimensions;
        public int Dimensions{ get { return dimensions; } protected set { dimensions = value; } }
        public abstract double exec(double[] x);
        public abstract double[] getGradient(double[] x);
        public abstract double[] getHessiMatrix(double[] x);
        public double[] getNumericGradient(double[] x, double delta)
        {
            double[] grad=new double[dimensions];
            double sum = 0.0f;
            for (int i = 0; i < dimensions; i++)
            {
                double xi = x[i];
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
    public class SymbolicFunction1D : Function1D
    {
        ExpressionStack expression;
        public SymbolicFunction1D(ExpressionStack exp)
        {
            expression = exp;
        }
        public ExpressionStack getExpression()
        {
            return expression;
        }
        public string getVariableName()
        {
            return expression.getVariableNames()[0];
        }
        public override double exec(double x)
        {
            expression.set("x",x);
            return (double)expression.execute();
        }
    }
    public class Function1DNDAdapter:Function1D
    {
        FunctionND func;
        Vector<double> x0;
        Vector<double> dx;
        public Function1DNDAdapter(FunctionND func, Vector<double> x0, Vector<double> dx)
        {
            this.func = func;
            this.x0 = x0;
            this.dx = dx;
        }
        public override double exec(double x)
        {
            return func.exec((x0+dx*x).AsArray());
        }
    }
    public class SymbolicFunctionND : FunctionND
    {
        ExpressionStack expression;
        List<ExpressionStack> derivatives;
        List<ExpressionStack> secondDerivatives;
        string[] variables;
        public ExpressionStack getExpression()
        {
            return expression;
        }
        public SymbolicFunctionND(ExpressionStack exp,List<ExpressionStack> der,List<ExpressionStack> secondDer,string[] variables)
        {
            dimensions = exp.getVariableCount();
            expression = exp;
            derivatives = der;
            secondDerivatives = secondDer;
            this.variables = variables;
        }
        public override double exec(double[] vars)
        {
            for(int i=0;i< Dimensions; i++)
                expression.set(variables[i], vars[i]);
            return (double)expression.execute();
        }
        public override double[] getGradient(double[] x)
        {
            double[] result = new double[Dimensions];
            for (int i = 0; i < Dimensions; i++)
            {
                for (int j = 0; j < Dimensions; j++)
                    derivatives[i].set(variables[j], x[j]);
                result[i]= (double)derivatives[i].execute();
            }
            return result;
        }
        public override double[] getHessiMatrix(double[] x)//column major flat matrix
        {
            double[] result = new double[Dimensions*Dimensions];
            for (int i = 0; i < Dimensions; i++)
            {
                for (int j = 0; j < Dimensions; j++)
                {
                    int index = i * Dimensions + j;
                    for (int k = 0; k < Dimensions; k++)
                        secondDerivatives[index].set(variables[k], x[k]);
                    result[index] = (double)secondDerivatives[index].execute();
                }
            }
            return result;
        }
    }
}
