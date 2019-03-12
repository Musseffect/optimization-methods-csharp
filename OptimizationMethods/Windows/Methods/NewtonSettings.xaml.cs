using OptimizationMethods.Windows.Methods;
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
using System.Windows.Shapes;
using static OptimizationMethods.Compiler.ASTCompiler;

namespace OptimizationMethods.Windows
{
    /// <summary>
    /// Логика взаимодействия для NewtonSettings.xaml
    /// </summary>
    public partial class NewtonSettings : Window
    {
        SymbolicFunctionND function;
        public NewtonSettings(SymbolicFunctionND function)
        {
            InitializeComponent();
            this.function = function;
            ExpressionStack expressionStack = function.getExpression();
            int varCount = expressionStack.getVariableCount();
            string[] variables = expressionStack.getVariableNames();
            BorderSetter.VariableCollection.Clear();
            foreach (string variable in variables)
            {
                BorderSetter.VariableCollection.Add(new BorderSetter.VariableEntry() { Label = variable, ValueMin = -2, ValueMax = 2 });
            }
        }
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            NewtonMethod method = new NewtonMethod();
            double[] min = BorderSetter.GetMin();
            double[] max = BorderSetter.GetMax();
            method.setBounds(min, max);
            method.setParameters(NewtonIters.Value.Value,NewtonEpsilonY.Value.Value,NewtonEpsilonX.Value.Value,NewtonAlpha.Value.Value);
            Common.MethodResults(method, function, min, max);
        }
    }
}
