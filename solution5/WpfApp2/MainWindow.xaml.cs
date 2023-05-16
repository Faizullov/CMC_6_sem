using ClassLibrary1;
using System;
using System.Collections.Generic;
using System.Globalization;
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

namespace WpfApp1
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    /// 
    public class GridValue
    {
        public double Point { get; set; }
        public double Value { get; set; }
        public override string ToString()
        {
            return $"{Point} , {Value}";
        }
    }

    public partial class MainWindow : Window
    {
        public struct GridValue
        {
            public double Point { get; set; }
            public double Value { get; set; }
        }
        ViewData viewData = new ViewData();
        public MainWindow()
        {
            InitializeComponent();
            this.DataContext = viewData;
            comboBox_Enum.ItemsSource = Enum.GetValues(typeof(ClassLibrary1.FRawEnum));

        }

        private void ToSave(object sender, RoutedEventArgs e)
        {
            Microsoft.Win32.SaveFileDialog dlg = new Microsoft.Win32.SaveFileDialog();
            dlg.FileName = "RawData";
            dlg.DefaultExt = ".txt";
            dlg.Filter = "Text documents (.txt)|*.txt";

            Nullable<bool> result = dlg.ShowDialog();

            if (result == true)
            {
                string filename = dlg.FileName;
                viewData.Save(filename);
            }
        }

        private void RawDataFromControlsButton_Click(object sender, RoutedEventArgs e)
        {
            viewData.ExecuteSplines();
            //integralTextBlock.Text = viewData.splineData.Integral.ToString();
            //ListsRawData.ItemsSource = (System.Collections.IEnumerable)viewData.rawData;
        }

        private void RawDataFromFileButton_Click(object sender, RoutedEventArgs e)
        {
            Microsoft.Win32.OpenFileDialog dlg = new Microsoft.Win32.OpenFileDialog();
            dlg.DefaultExt = ".txt";
            dlg.Filter = "Text documents (.txt)|*.txt";

            Nullable<bool> result = dlg.ShowDialog();

            if (result == true)
            {
                string filename = dlg.FileName;
                viewData.Load(filename);
                viewData.ExecuteSplines();
            }
        }
    }
    public class TwoValuesConverter : IMultiValueConverter 
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            return $"{values[0]};{values[1]}";
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            var str = (string)value;
            var values = str.Split(';');
            return new object[] { double.Parse(values[0]), double.Parse(values[1]) };
        }
    }
    public class InverseBooleanConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is bool boolValue)
            {
                return !boolValue;
            }
            return value;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is bool boolValue)
            {
                return !boolValue;
            }
            return value;
        }
    }
    public class RawDataToStringConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is RawData rawData)
            {
                List<GridValue> gridValues = new List<GridValue>();
                for (int i = 0; i < rawData.N_points; i++)
                {
                    GridValue gridValue = new()
                    {
                        Point = rawData.Points[i],
                        Value = rawData.Values[i]
                    };
                    gridValues.Add(gridValue);
                }

                return gridValues;
            }
            return "";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
    public class SplineDataItemTostringConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is SplineDataItem sItem)
            {
                return $"Point: {sItem.Point_counted:F2}\nSplineValue: {sItem.SplineValue:F2}\nFirstDerivative: {sItem.FirstDerivative:F2}\nSecondDerivative: {sItem.SecondDerivative:F2}";
            }
            return Binding.DoNothing;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

}
