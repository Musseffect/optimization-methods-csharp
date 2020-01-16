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
        static private SymbolicFunctionND compileFunctionND(string expression)
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
            if (variables.Length == 0)
            {
                throw new Exception("Выражение не содержит переменных");
            }
            List<ExpressionStack> derivatives=new List<ExpressionStack>();
            List<ExpressionStack> secondDerivatives=new List<ExpressionStack>();
            for (int i = 0; i < variables.Length; i++)
            {
                var derRoot = simplify(computeDerivative(rootSimple,variables[i]));
                derivatives.Add(compileASTExpression(derRoot));
                //Console.WriteLine("dfd"+variables[i]+" "+(new ASTPrint()).print(derRoot));
                for (int j = 0; j < variables.Length; j++)
                {
                    var secondDerRoot = simplify(computeDerivative(derRoot, variables[j]));
                    secondDerivatives.Add(compileASTExpression(secondDerRoot));
                    //Console.WriteLine("df2d" + variables[i] +"d" + variables[j] + " " + (new ASTPrint()).print(secondDerRoot));
                }
            }
            return new SymbolicFunctionND(exp,derivatives,secondDerivatives, variables);
        }
        static private ExpressionStack compileExpression(string expression)
        {
            AntlrInputStream inputStream = new AntlrInputStream(expression);
            ExpGrammarLexer expLexer = new ExpGrammarLexer(inputStream);
            expLexer.RemoveErrorListeners();
            ErrorListener lexerListener = new ErrorListener();
            expLexer.AddErrorListener(lexerListener);
            CommonTokenStream commonTokenStream = new CommonTokenStream(expLexer);
            ExpGrammarParser expParser = new ExpGrammarParser(commonTokenStream);
            ParserErrorListener parserListener = new ParserErrorListener();
            expParser.RemoveErrorListeners();
            expParser.AddErrorListener(parserListener);
            ExpGrammarParser.CompileUnitContext expContext = expParser.compileUnit();
            List<string> errors = lexerListener.getErrors();
            if (errors.Count > 0)
            {
                throw new Exception("Lexer Error");
            }
            errors = parserListener.getErrors();
            if (errors.Count > 0)
            {
                throw new Exception("Parser error");
            }
            ExpressionVisitor visitor = new ExpressionVisitor();
            ASTNode root = visitor.VisitCompileUnit(expContext);
            var rootSimple = Compiler.ASTCompiler.validate(root);
            rootSimple = Compiler.ASTCompiler.simplify(rootSimple);
            return compileASTExpression(rootSimple);
        }
        static private SymbolicFunction1D compileFunction1D(string expression)
        {
            ExpressionStack exp = compileExpression(expression);
            if (exp.getVariableCount() != 1)
            {
                throw new Exception("Ожидается функция одной переменной");
            }
            return new SymbolicFunction1D(exp);
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

        private void Swarm_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                SymbolicFunctionND function = compileFunctionND(Expression.Text);
                SwarmSettings window = new SwarmSettings(function);
                window.Show();
            }
            catch (Exception exc)
            {
                MessageBox.Show("Error",exc.Message);
            }
        }
        private void BestProbe_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                SymbolicFunctionND function = compileFunctionND(Expression.Text);
                BestProbeSettings window = new BestProbeSettings(function);
                window.Show();
            }
            catch (Exception exc)
            {
                MessageBox.Show("Error",exc.Message);
            }
}
        private void NewtonRaphson_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                SymbolicFunctionND function = compileFunctionND(Expression.Text);
                NewtonRaphsonSettings window = new NewtonRaphsonSettings(function);
                window.Show();
            }
            catch (Exception exc)
            {
                MessageBox.Show("Error", exc.Message);
            }
        }
        private void Newton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                SymbolicFunctionND function = compileFunctionND(Expression.Text);
                NewtonSettings window = new NewtonSettings(function);
                window.Show();
            }
            catch (Exception exc)
            {
                MessageBox.Show("Error", exc.Message);
            }
        }
        private void GradientDescent_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                SymbolicFunctionND function = compileFunctionND(Expression.Text);
                GradientDescentSettings window = new GradientDescentSettings(function);
                window.Show();
            }
            catch (Exception exc)
            {
                MessageBox.Show("Error", exc.Message);
            }
        }
        private void Gauss_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                SymbolicFunctionND function = compileFunctionND(Expression.Text);
                GaussSettings window = new GaussSettings(function);
                window.Show();
            }
            catch (Exception exc)
            {
                MessageBox.Show("Error", exc.Message);
            }
        }
        private void GoldenRule_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                SymbolicFunction1D function = compileFunction1D(Expression.Text);
                GoldenRuleSettings window = new GoldenRuleSettings(function);
                window.Show();
            }
            catch (Exception exc)
            {
                MessageBox.Show("Error", exc.Message);
            }
        }
        private void Passive_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                SymbolicFunction1D function = compileFunction1D(Expression.Text);
                PassiveSearchSettings window = new PassiveSearchSettings(function);
                window.Show();
            }
            catch (Exception exc)
            {
                MessageBox.Show("Error", exc.Message);
            }
        }

        private void MenuItem_Click(object sender, RoutedEventArgs e)
        {
            Expression.Text = "x*x";
        }

        private void MenuItem_Click_1(object sender, RoutedEventArgs e)
        {//Функция Розенброка
            Expression.Text = "sqr(1-x)+100*sqr(y-sqr(x))";
        }

        private void MenuItem_Click_2(object sender, RoutedEventArgs e)
        {
            Expression.Text = "x*x+y*y";
        }

        private void MenuItem_Click_3(object sender, RoutedEventArgs e)
        {
            //Химмельблау
            Expression.Text ="sqr(sqr(x)+y-11)+sqr(x+sqr(y)-7)";
        }

        private void MenuItem_Click_4(object sender, RoutedEventArgs e)
        {
            //Изома
            Expression.Text = "-cos(x)*cos(y)*exp(-sqr(x-pi())-sqr(y-pi()))";
        }

        private void MenuItem_Click_5(object sender, RoutedEventArgs e)
        {
            //Растригина
            Expression.Text = "10*2+x*x-10*cos(2*pi()*x)+y*y-10*cos(2*pi()*y)";
        }

        private void MenuItem_Click_6(object sender, RoutedEventArgs e)
        {
            Expression.Text = "10+x*x-10*cos(2*pi()*x)";
        }
    }
}
