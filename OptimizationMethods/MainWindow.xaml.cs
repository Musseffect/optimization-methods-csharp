using Antlr4.Runtime;
using OptimizationMethods.Compiler;
using OptimizationMethods.Test;
using OptimizationMethods.Windows;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Media.Media3D;
using System.Windows.Navigation;
using System.Windows.Shapes;
using static OptimizationMethods.Compiler.ASTCompiler;

namespace OptimizationMethods
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            ASTCompiler.MetaData.Init();
        }
        private SymbolicFunctionND compileFunctionND(string expression)
        {
            AntlrInputStream inputStream = new AntlrInputStream(expression);
            ExpGrammarLexer expLexer = new ExpGrammarLexer(inputStream);
            expLexer.RemoveErrorListeners();
            ErrorListener lexerListener = new ErrorListener();
            expLexer.AddErrorListener(lexerListener);
            List<string> errors = lexerListener.getErrors();
            if (errors.Count > 0)
            {
                throw new Exception("LexerError");
            }
            CommonTokenStream commonTokenStream = new CommonTokenStream(expLexer);
            ExpGrammarParser expParser = new ExpGrammarParser(commonTokenStream);
            ParserErrorListener parserListener = new ParserErrorListener();
            expParser.RemoveErrorListeners();
            expParser.AddErrorListener(parserListener);
            errors = parserListener.getErrors();
            if (errors.Count > 0)
            {
                throw new Exception("Parser error");
            }
            ExpGrammarParser.CompileUnitContext expContext = expParser.compileUnit();
            ExpressionVisitor visitor = new ExpressionVisitor();
            ASTNode root = visitor.VisitCompileUnit(expContext);
            var rootSimple = Compiler.ASTCompiler.validate(root);
            rootSimple = Compiler.ASTCompiler.simplify(rootSimple);
            var exp=compileASTExpression(rootSimple);
            string[]variables = exp.getVariableNames();
            List<ExpressionStack> derivatives=new List<ExpressionStack>();
            List<ExpressionStack> secondDerivatives=new List<ExpressionStack>();
            for (int i = 0; i < variables.Length; i++)
            {
                var derRoot = simplify(computeDerivative(rootSimple,variables[i]));
                derivatives.Add(compileASTExpression(derRoot));
                for (int j = 0; j < variables.Length; j++)
                {
                    var secondDerRoot = simplify(computeDerivative(derRoot, variables[j]));
                    secondDerivatives.Add(compileASTExpression(secondDerRoot));
                }
            }
            return new SymbolicFunctionND(exp,derivatives,secondDerivatives, variables);
        }
        private ExpressionStack compileExpression(string expression)
        {
            AntlrInputStream inputStream = new AntlrInputStream(expression);
            ExpGrammarLexer expLexer = new ExpGrammarLexer(inputStream);
            expLexer.RemoveErrorListeners();
            ErrorListener lexerListener = new ErrorListener();
            expLexer.AddErrorListener(lexerListener);
            List<string> errors = lexerListener.getErrors();
            if (errors.Count > 0)
            {
                throw new Exception("LexerError");
            }
            CommonTokenStream commonTokenStream = new CommonTokenStream(expLexer);
            ExpGrammarParser expParser = new ExpGrammarParser(commonTokenStream);
            ParserErrorListener parserListener = new ParserErrorListener();
            expParser.RemoveErrorListeners();
            expParser.AddErrorListener(parserListener);
            errors = parserListener.getErrors();
            if (errors.Count > 0)
            {
                throw new Exception("Parser error");
            }
            ExpGrammarParser.CompileUnitContext expContext = expParser.compileUnit();
            ExpressionVisitor visitor = new ExpressionVisitor();
            ASTNode root = visitor.VisitCompileUnit(expContext);
            var rootSimple = Compiler.ASTCompiler.validate(root);
            rootSimple = Compiler.ASTCompiler.simplify(rootSimple);
            return compileASTExpression(rootSimple);
        }
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            string exp = Expression.Text;
            try
            {
                ExpressionStack expressionStack = compileExpression(exp);
                int varCount = expressionStack.getVariableCount();
                if (varCount < 1 || varCount > 2)
                    throw new Exception("Некорректное число переменных");
                bool twoDim = varCount==2;
                PlotSettings settings = new PlotSettings(!twoDim,expressionStack);
                settings.Show();
            } catch (Exception exc)
            {
                MessageBox.Show(exc.Message,"Error");
                return;
            }
            //Tester.TestASTRPN();
        }
        private ExpressionStack getOneDim()
        {
            string exp = Expression.Text;
            ExpressionStack expressionStack = compileExpression(exp);
            int varCount = expressionStack.getVariableCount();
            if (varCount != 1)
                throw new Exception("Некорректное число переменных");
            return expressionStack;
        }
        List<Point> calc1D(int resolution, double x1, double x2,ExpressionStack exp)
        {
            double dx = (x2 - x1) / (resolution - 1);
            List<Point> points = new List<Point>(resolution);
            for (int i = 0; i < resolution; i++)
            {
                double x = x1 + dx * i;
                exp.set("x", (float)x);
                points.Add(new Point(x, exp.execute()));
            }
            return points;
        }
        List<Point3D> calc2D(int resolutionX,int resolutionY,double x1,double x2,double y1,double y2,ExpressionStack exp)
        {
            double dx = (x2 - x1) / (resolutionX - 1);
            double dy = (y2 - y1) / (resolutionY - 1);
            List<Point3D> points = new List<Point3D>(resolutionX * resolutionY);
            double min = double.MaxValue, max = double.MinValue;
            for (int j = 0; j < resolutionY; j++)
            {
                double y = y1 + dy * j;
                exp.set("y", (float)y);
                for (int i = 0; i < resolutionX; i++)
                {
                    double x = x1 + dx * i;
                    exp.set("x", (float)x);
                    double z = exp.execute();
                    min = Math.Min(min, z);
                    max = Math.Max(max, z);
                    points.Add(new Point3D(x, y, z));
                }
            }
            return points;
        }
        private void ButtonPassive_Click(object sender, RoutedEventArgs e)
        {
            PassiveSearchMethod method = new PassiveSearchMethod();
            method.PointCount = PassiveSearchPointCount.Value.Value;
            try
            {
                ExpressionStack stack = getOneDim();
                double b1, b2;
                b1 = X1.Value.Value;
                b2 = X2.Value.Value;
                MinPointND mp=method.getMinimum((float)b1,(float)b2,new SymbolicFunction1D(stack));
                MethodResults mr = new MethodResults(calc1D(100,b1,b2,stack), 1, new List<MinPointND> { mp },stack.getVariableNames(),new List<int> { 100},"");
                mr.Show();
            } catch (Exception exc)
            {
                MessageBox.Show(exc.Message,"Error");
                return;
            }
        }
        private void ButtonGolden_Click(object sender, RoutedEventArgs e)
        {
            GoldenRuleMethod method = new GoldenRuleMethod();
            method.Epsilon = (float)GoldenEpsilon.Value.Value;
            method.Iterations = GoldenIterations.Value.Value;
            try {
                ExpressionStack stack = getOneDim();
                double b1, b2;
                b1 = X1.Value.Value;
                b2 = X2.Value.Value;
                int iters;
                List<MinPointND> mp = method.getMinimumSolutions((float)b1, (float)b2, new SymbolicFunction1D(stack),out iters);
                MethodResults mr = new MethodResults(calc1D(100, b1, b2, stack), 1, mp, stack.getVariableNames(), new List<int> { 100 },"Iterations: "+iters.ToString());
                mr.Show();
            }
            catch (Exception exc)
            {
                MessageBox.Show(exc.Message, "Error");
                return;
            }

        }
        private void executeNDMethod(OptMethodND method)
        {
            try
            {
                string exp = Expression.Text;
                SymbolicFunctionND func= compileFunctionND(exp);
                ExpressionStack expressionStack = func.getExpression();
                int varCount = expressionStack.getVariableCount();
                if ( varCount!=2)
                    throw new Exception("Некорректное число переменных");
                double x1, x2, y1, y2;
                x1 = X1.Value.Value;
                x2 = X2.Value.Value;
                y1 = Y1.Value.Value;
                y2 = Y2.Value.Value;
                method.setBounds(new double[] { x1,y1 }, new double[] { x2, y2 });
                OptMethodResult mp = method.getMinimum(func,10);
                MethodResults mr = new MethodResults(calc2D(100, 100, x1, x2, y1, y2, expressionStack), 2, mp.solutions, expressionStack.getVariableNames(), new List<int> { 100, 100 },
                    "Iterations: "+mp.iterations.ToString());
                mr.Show();
            }
            catch (Exception exc)
            {
                MessageBox.Show(exc.Message, "Error");
                return;
            }
        }
        private void SwarmButton_Click(object sender, RoutedEventArgs e)
        {
            SwarmMethod method = new SwarmMethod()
            {
                Iterations=SwarmIterations.Value.Value,
                PhiG=SwarmPhiG.Value.Value,
                PhiP=SwarmPhiP.Value.Value,
                SwarmSize=SwarmSize.Value.Value,
                W=SwarmW.Value.Value
            };
                executeNDMethod(method);
           
        }

        private void BestProbe_Click(object sender, RoutedEventArgs e)
        {
            BestTrialMethod method = new BestTrialMethod();
            method.setParameters(ProbeIters.Value.Value,ProbeVectorCount.Value.Value, ProbeStep.Value.Value, ProbeEpsilonX.Value.Value, ProbeEpsilonY.Value.Value);
            executeNDMethod(method);
        }

        private void ButtonGauss_Click(object sender, RoutedEventArgs e)
        {
            GaussMethod method = new GaussMethod();
            GoldenRuleMethod method1D = new GoldenRuleMethod();
            method1D.Epsilon = 0.1;
            method1D.Iterations = 10;
            method.setMethod(method1D);
            method.setParameters(GaussIters.Value.Value,GaussEpsilonY.Value.Value,GaussEpsilonX.Value.Value);
            executeNDMethod(method);
        }

        private void Descent_Click(object sender, RoutedEventArgs e)
        {
            GradientDescentMethod method = new GradientDescentMethod();
            method.setParameters(DescentIters.Value.Value,DescentEpsilonY.Value.Value,DescentEpsilonX.Value.Value);
            executeNDMethod(method);
        }

        private void Newton_Click(object sender, RoutedEventArgs e)
        {
            NewtonMethod method = new NewtonMethod();
            method.setParameters(NewtonIters.Value.Value,NewtonEpsilonY.Value.Value, NewtonEpsilonX.Value.Value,NewtonAlpha.Value.Value);
            executeNDMethod(method);
        }

        private void NewtonRaphson_Click(object sender, RoutedEventArgs e)
        {
            NewtonRaphsenMethod method = new NewtonRaphsenMethod();
            method.setParameters(NRIters.Value.Value, NREpsilonY.Value.Value,NREpsilonX.Value.Value);
            executeNDMethod(method);
        }
    }
}
