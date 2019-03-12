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
    /// Логика взаимодействия для GoldenRuleSettings.xaml
    /// </summary>
    public partial class GoldenRuleSettings : Window
    {
        SymbolicFunction1D function;
        public GoldenRuleSettings(SymbolicFunction1D function)
        {
            InitializeComponent();
            this.function = function;
            ExpressionStack expressionStack = function.getExpression();
            string variable = expressionStack.getVariableNames()[0];
            BorderSetter.VariableCollection.Clear();
            BorderSetter.VariableCollection.Add(new BorderSetter.VariableEntry() { Label = variable, ValueMin = -2, ValueMax = 2 });
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            GoldenRuleMethod method = new GoldenRuleMethod();
            double[] min = BorderSetter.GetMin();
            double[] max = BorderSetter.GetMax();
            method.setParameters(GoldenEpsilon.Value.Value, GoldenIterations.Value.Value);
            Common.MethodResults(method, function, min, max);
        }
    }
}
