using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
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
    public partial class BorderSetter : UserControl
    {
        public class VariableEntry: INotifyPropertyChanged
        {
            public double ValueMin { get; set; }
            public double ValueMax { get; set; }
            public string Label { get; set; }
            public event PropertyChangedEventHandler PropertyChanged;
            public void OnPropertyChanged([CallerMemberName]string prop = "")
            {
                if (PropertyChanged != null)
                    PropertyChanged(this, new PropertyChangedEventArgs(prop));
            }
        };
        public double Min
        {
            get { return (double)GetValue(MinProperty); }
            set { SetValue(MinProperty, value); }
        }
        public double Max
        {
            get { return (double)GetValue(MaxProperty); }
            set { SetValue(MaxProperty, value); }
        }
        public static readonly DependencyProperty MinProperty =
         DependencyProperty.Register("Min", typeof(double), typeof(BorderSetter));
        public static readonly DependencyProperty MaxProperty =
         DependencyProperty.Register("Max", typeof(double), typeof(BorderSetter));
        public List<VariableEntry> VariableCollection { get; set; } = new List<VariableEntry>() {};
        public BorderSetter()
        {
            InitializeComponent();
        }
        public double[] GetMin()
        {
            double[] min = new double[VariableCollection.Count];
            for (int i = 0; i < VariableCollection.Count; i++)
            {
                min[i] = VariableCollection[i].ValueMin;
            }
            return min;
        }
        public double[] GetMax()
        {
            double[] max = new double[VariableCollection.Count];
            for (int i = 0; i < VariableCollection.Count; i++)
            {
                max[i] = VariableCollection[i].ValueMax;
            }
            return max;
        }
    }
}
