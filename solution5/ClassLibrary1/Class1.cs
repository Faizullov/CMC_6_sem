using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
using static System.Net.Mime.MediaTypeNames;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace ClassLibrary1

{
    public struct GridValue
    {
        public double Point { get; set; }
        public double Value { get; set; }
    }
 
    public delegate double FRaw(double x); // предоставляет ссылку на метод

    public enum FRawEnum // как выполнять сплайн интерполяцию
    {
        Linear,
        Cubic,
        Random
    }

    public class RawData
    {
        public double A { get; set; }
        public double B { get; set; }
        public int N_points { get; set; }
        public bool UniformGrid { get; set; }
        public FRaw? F { get; set; }
        public double[]? Points { get; set; }
        public double[]? Values { get; set; }
        public RawData(double left, double right, int numPoints, bool check_uniforn, FRaw fRaw) // конструктор
        {
            A = left;
            B = right;
            N_points = numPoints;
            UniformGrid = check_uniforn;
            F = fRaw;
            Points = new double[numPoints];
            Values = new double[numPoints];

            if (UniformGrid)   // равномерная
            {
                double h = (B - A) / (numPoints - 1);
                for (int i = 0; i < numPoints; i++)
                {
                    Points[i] = A + i * h;    // равномерно распределяем
                }
            }
            else
            {
                double h = (B - A) / (numPoints - 1);
                Points[0] = A;
                Points[numPoints - 1] = B;
                for (int i = 1; i < numPoints - 1; i++)
                {
                    Random randomize = new Random();
                    Points[i] = A + h * (i + randomize.NextDouble()); // шаг + рандомное число от 0 до 1
                }
            }

            for (int i = 0; i < numPoints; i++) // инициализируем данные для сплайн интерполяции
            {
                Values[i] = F(Points[i]);
            }
        }
        public RawData(string fileName) // констурктор для чтения файла
        {
            StreamReader? reader = null;
            try
            {
                reader = new StreamReader(fileName);
                string line = reader.ReadLine();
                string[] arr = line.Split(' '); // разбить по пробелам
                A = double.Parse(arr[0]);
                B = double.Parse(arr[1]);
                N_points = int.Parse(arr[2]);
                UniformGrid = bool.Parse(arr[3]);

                Points = new double[N_points];
                Values = new double[N_points];

                for (int i = 0; i < N_points; i++)
                {
                    line = reader.ReadLine();
                    arr = line.Split(' ');
                    Points[i] = double.Parse(arr[0]); // преобразуем в double
                    Values[i] = double.Parse(arr[1]);
                }
            }
            catch (Exception e)
            {
                throw new Exception("An error with file.", e);
            }
            finally
            {
                if (reader != null)
                {
                    reader.Close();
                }
            }
        }

        public static double[] CreateValues(FRaw fRaw, double[] points) // инициализация массива в узлах
        {
            double[] values = new double[points.Length];
            for (int i = 0; i < points.Length; i++)
            {
                values[i] = fRaw(points[i]);
            }
            return values;
        }

        public void Save(string filename)
        {
            StreamWriter writer = null;
            try
            {
                writer = new StreamWriter(filename);
                writer.WriteLine($"{A} {B} {N_points} {UniformGrid}");
                for (int i = 0; i < N_points; i++)
                {
                    writer.WriteLine($"{Points[i]} {Values[i]}"); // записываем каждую коордианту
                }
            }
            catch (Exception e)
            {
                throw e;
            }
            finally
            {
                if (writer != null)
                {
                    writer.Close();
                }
            }
        }

        public static bool Load(string filename, out RawData rawData)
        {
            rawData = null;
            try
            {
                rawData = new RawData(filename); // просто конструктор используем
                return true;
            }
            catch (Exception e)
            {
                Console.WriteLine($"Failed to read data from file {filename}. {e.Message}");
                return false;
            }
        }

        public static double[] InitValues(FRawEnum fRawEnum, double[] points)
        {
            double[] values = new double[points.Length];
            for (int i = 0; i < points.Length; i++)
            {
                switch (fRawEnum)
                {
                    case FRawEnum.Linear:
                        values[i] = points[i];
                        break;
                    case FRawEnum.Cubic:
                        values[i] = points[i] * points[i] * points[i];
                        break;
                    case FRawEnum.Random:
                        Random rand = new Random();
                        values[i] = rand.NextDouble();
                        break;
                    default:
                        throw new ArgumentException("Invalid FRawEnum value");
                }
            }
            return values;
        }

        public static double Linear(double x)
        { return x; }
        public static double Quadratic(double x)
        { return x * (x - 1) + 2; }
        public static double Cubic(double x)
        { return x * (x - 1) * (x - 2) + 2; }

        public static double Random(double x)
        {
            Random rand = new Random();
            return rand.NextDouble();
        }

        public override string ToString()
        {
            string str = "";
            for (int i = 0; i < N_points; i++)
            {
                str += Points[i] + Values[i] + '\n';
            }
            return str;
        }
    }

    public struct SplineDataItem
    {
        public double Point_counted { get; set; }
        public double SplineValue { get; set; }
        public double FirstDerivative { get; set; }
        public double SecondDerivative { get; set; }

        public SplineDataItem(double point, double splineValue, double firstDerivative, double secondDerivative)
        {
            Point_counted = point;
            SplineValue = splineValue;
            FirstDerivative = firstDerivative;
            SecondDerivative = secondDerivative;
        }

        public override string ToString()
        {
            return $"{Point_counted} , {SplineValue},  {FirstDerivative}, {SecondDerivative} ";
        }

        public string ToString(string format)
        {
            return $"{Point_counted.ToString(format)}, {SplineValue.ToString(format)}, {FirstDerivative.ToString(format)}, {SecondDerivative.ToString(format)}";
        }
    }

    public class SplineData : INotifyPropertyChanged
    {
        public RawData Value_RawData { get; set; }
        public int NumNodes { get; set; }

        public double left { get; set; }
        public double right { get; set; }

        private List<SplineDataItem> splineDataItems;
        public List<SplineDataItem> SplineDataItems
        {
            get { return splineDataItems; }
            set
            {
                splineDataItems = value;
                OnPropertyChanged("SplineDataItems");
            }
        }
        private double integ;
        public double Integral
        {
            get { return integ; }
            set
            {
                integ = value;
                OnPropertyChanged("Integral");
            }
        }

        public SplineData(RawData data, double leftFirstDerivative, double rightFirstDerivative, int nodeCount)
        {
            Value_RawData = data;
            NumNodes = nodeCount;
            left = leftFirstDerivative;
            right = rightFirstDerivative;
            Integral = 0;
            SplineDataItems = new List<SplineDataItem>();
        }

        public event PropertyChangedEventHandler? PropertyChanged;
        public void OnPropertyChanged([CallerMemberName] string prop = "")
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(prop));
        }

        public void DoSplines()
        {
            double[] ldrd = { left, right };
            double[] coeff = new double[4 * (Value_RawData.N_points - 1)];
            double[] res = new double[3 * NumNodes];
            double[] integral = new double[1];
            double result = func(NumNodes, Value_RawData.N_points, Value_RawData.Points, Value_RawData.Values, Value_RawData.UniformGrid, ldrd, coeff, res, integral);
            Integral = integral[0];
            double step = (Value_RawData.B - Value_RawData.A) / (NumNodes - 1);
            for (int i = 0; i < NumNodes; i++)
            {
                SplineDataItem sdi = new SplineDataItem()
                {
                    Point_counted = Value_RawData.A + step * i,
                    SplineValue = res[3 * i],
                    FirstDerivative = res[3 * i + 1],
                    SecondDerivative = res[3 * i + 2]
                };
                SplineDataItems.Add(sdi);
            }
        }
        [DllImport("C:\\Users\\af40\\Desktop\\hw1\\Solution5\\x64\\Debug\\Dll1.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern double func(int NumSplines, int NumNodes, double[] Points, double[] Values, bool isUniform, double[] ldrd, double[] coeff, double[] res, double[] integral);

    }
}