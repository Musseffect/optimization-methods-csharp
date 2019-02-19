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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace OptimizationMethods.Windows
{
    /// <summary>
    /// Логика взаимодействия для BorderSetter.xaml
    /// </summary>
    /// 
    [TemplatePart(Name = "Min", Type = typeof(float)),
    TemplatePart(Name = "Max", Type = typeof(float))]
    public partial class BorderSetter : UserControl
    {
        class VariableEntry
        {
            public float ValueMin{ get; set; }
            public float ValueMax{ get; set; }

        };
        public List<VariableEntry> VariableCollection;
        string[] variables;
        public BorderSetter()
        {
            variables = null;
            min = null;
            max = null;
        }
        public float[] GetMin()
        {
            return min;
        }
        public float[] GetMax()
        {
            return max;
        }
    }
}
