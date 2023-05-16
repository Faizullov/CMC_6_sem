using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ClassLibrary1;
using System.Windows;
using System.Globalization;
using System.Windows.Data;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace WpfApp1
{
    internal class ViewData : INotifyPropertyChanged
    {
        private double a;
        public double A
        {
            get { return a; }
            set
            {
                a = value;
                OnPropertyChanged("A");
            }
        }
        private double b;
        public double B
        {
            get { return b; }
            set
            {
                b = value;
                OnPropertyChanged("B");
            }
        }
        private int n_Points;
        public int N_Points
        {
            get { return n_Points; }
            set
            {
                n_Points = value;
                OnPropertyChanged("N_Points");
            }
        }
        private bool uniformGrid;
        public bool UniformGrid
        {
            get { return uniformGrid; }
            set
            {
                uniformGrid = value;
                OnPropertyChanged("UniformGrid");
            }
        }
        private int numSplines;
        public int NumSplines
        {
            get { return numSplines; }
            set
            {
                numSplines = value;
                OnPropertyChanged("NumSplines");
            }
        }
        public FRawEnum fRawEnum { get; set; } // Способ 1
        public List<FRaw> listFRaw { get; set; }  // Способ 2. Список делегатов
        public FRaw fRaw { get; set; }  // для способа 2
        private double lfdd;
        public double lfd
        {
            get { return lfdd; }
            set
            {
                lfdd = value;
                OnPropertyChanged("lsd");
            }
        }
        private double rfdd;
        public double rfd
        {
            get { return rfdd; }
            set
            {
                rfdd = value;
                OnPropertyChanged("rsd");
            }
        }
        private RawData rawdata;
        public RawData rawData
        {
            get { return rawdata; }
            set
            {
                rawdata = value;
                OnPropertyChanged("rawData");
            }
        }
        private SplineData splinedata;
        public SplineData splineData
        {
            get { return splinedata; }
            set
            {
                splinedata = value;
                OnPropertyChanged("splineData");
            }
        }
        public ViewData()
        {
            A = 0;
            B = 5;
            N_Points = 10;
            NumSplines = 5;
            lfd = 2;
            rfd = 2;
            UniformGrid = true;
            fRawEnum = FRawEnum.Linear;

            listFRaw = new List<FRaw>();       
            listFRaw.Add(RawData.Linear);      
            listFRaw.Add(RawData.Quadratic);   
            listFRaw.Add(RawData.Cubic); 
            listFRaw.Add(RawData.Random);
            fRaw = listFRaw[2];                
        }

        public event PropertyChangedEventHandler? PropertyChanged;
        public void OnPropertyChanged([CallerMemberName] string prop = "")
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(prop));
        }
        public void ExecuteSplines()
        {
            try
            {
                //// для способа 1
                if (fRawEnum == FRawEnum.Cubic) fRaw = RawData.Cubic;
                else if (fRawEnum == FRawEnum.Random) fRaw = RawData.Random;
                else fRaw = RawData.Linear;
                rawData = new RawData(A, B, N_Points, UniformGrid, fRaw);
                splineData = new SplineData(rawData, lfd, rfd, NumSplines);
                splineData.DoSplines();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
        public override string ToString()
        {
            return $"leftEnd = {A}\n" +
                   $"rightEnd = {B}\n" +
                   $"NumPoints = {N_Points}\n" +
                   $"LeftSecondDerivative = {lfd}\n" +
                   $"RightSecondDerivative = {rfd}\n" +
                   $"NumSplines = {NumSplines}\n" +
                   $"fRaw = {fRaw.Method.Name}\n";
        }
        public void Save(string filename)
        {
            try
            {
                rawData = new RawData(A, B, N_Points, UniformGrid, fRaw);
                rawData.Save(filename);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error saving data: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        public void Load(string filename)
        {
            try
            {
                RawData rData;
                if (RawData.Load(filename, out rData))
                {
                    A = rData.A;
                    B = rData.B;
                    N_Points = rData.N_points;
                    UniformGrid = rData.UniformGrid;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading data: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}
