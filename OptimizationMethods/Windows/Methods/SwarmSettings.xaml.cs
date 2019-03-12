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
    /// Логика взаимодействия для SwarmSettings.xaml
    /// </summary>
    public partial class SwarmSettings : Window
    {
        SymbolicFunctionND function;
        public SwarmSettings(SymbolicFunctionND function)
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
            SwarmMethod method = new SwarmMethod();
            double[] min = BorderSetter.GetMin();
            double[] max = BorderSetter.GetMax();
            method.setBounds(min, max);
            method.setParameters(SwarmSize.Value.Value,SwarmW.Value.Value,SwarmPhiP.Value.Value,SwarmPhiG.Value.Value, SwarmIterations.Value.Value);
            Common.MethodResults(method, function, min, max);
        }
    }
}
