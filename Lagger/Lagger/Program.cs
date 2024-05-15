using System;
using System.Collections.Generic;

namespace Lagguer
{
    public class Integral
    {
        public static double IntegrateQuad(Func<double, double> f, double a, double b, int N = 10000)
        {
            double[] x = new double[N];
            double dx = (b - a) / N;
            double s = 0;

            for (int i = 0; i < N; i++)
            {
                x[i] = a + i * dx;
                s += f(x[i]);
            }

            return s * Math.Abs(b - a) / N;
        }
    }

    public class Lagguerre
    {
        private double Beta { get; set; }
        private double Sigma { get; set; }
        private double? _experimentValue;





        public Lagguerre(double beta, double sigma)
        {
            Beta = beta;
            Sigma = sigma;
            _experimentValue = null;
        }

        public Lagguerre()
        {
            Beta = 2;
            Sigma = 4;
            _experimentValue = null;
        }

        public double Laguerre(double t, int n)
        {
            if (Beta < 0 || Beta > Sigma)
                throw new ArgumentException("Wrong parameters");

            double lpp = Math.Sqrt(Sigma) * Math.Exp(-Beta * t / 2);
            double lp = Math.Sqrt(Sigma) * (1 - Sigma * t) * Math.Exp(-Beta * t / 2);

            if (n == 0)
                return lpp;
            if (n == 1)
                return lp;

            for (int i = 2; i <= n; i++)
            {
                double temp = lp;
                lp = ((2 * i - 1 - Sigma * t) * lp / i) - ((i - 1) * lpp / i);
                lpp = temp;
            }

            return lp;
        }

        public List<Tuple<double, double>> TabulateLaguerre(double T, int n, int k = 100)
        {
            List<Tuple<double, double>> results = new List<Tuple<double, double>>();
            double[] t = new double[k];
            for (int i = 0; i < k; i++)
            {
                t[i] = i * T / k;
                double l = Laguerre(t[i], n);
                results.Add(Tuple.Create(t[i], l));
            }

            return results;
        }

        public Tuple<double?, List<Tuple<double, List<double>>>> Experiment(double T, double epsilon = 1e-3, int N = 20)
        {
            List<double> t = new List<double>();
            for (int i = 0; i < 1000; i++)
                t.Add(i * T / 1000);

            List<Tuple<double, List<double>>> experimentValues = new List<Tuple<double, List<double>>>();
            double? result = null;
            foreach (double i in t)
            {
                bool flag = true;
                List<double> laguerreValues = new List<double>();
                for (int j = 1; j <= N; j++)
                {
                    double laguerreVal = Laguerre(i, j);
                    laguerreValues.Add(laguerreVal);
                    if (Math.Abs(laguerreVal) > epsilon)
                    {
                        flag = false;
                        break;
                    }
                }

                if (flag && !result.HasValue)
                {
                    result = i;
                }

                experimentValues.Add(Tuple.Create(i, laguerreValues));
            }

            _experimentValue = result;

            return Tuple.Create(result, experimentValues);
        }

        public double LaguerreTransformation(Func<double, double> f, int n)
        {
            Func<double, double> integrand = t => f(t) * Laguerre(t, n) * Math.Exp(-t * (Sigma - Beta));
            double b = Experiment(100).Item1 ?? double.PositiveInfinity;
            return Integral.IntegrateQuad(integrand, 0, b);
        }

        public List<double> TabulateTransformation(Func<double, double> f, int N)
        {
            List<double> results = new List<double>();
            for (int n = 0; n <= N; n++)
            {
                double transformation = LaguerreTransformation(f, n);
                results.Add(transformation);
            }

            return results;
        }

        public double ReversedLaguerreTransformation(List<double> hList, double t)
        {
            double resultSum = 0;
            List<double> hListNew = hList.FindAll(x => x != 0);
            for (int i = 0; i < hListNew.Count; i++)
            {
                resultSum += hListNew[i] * Laguerre(t, i);
            }

            return resultSum;
        }
        public List<double> ReversedLaguerreTransformationTabulate(List<double> hList, double t1, double t2)
        {
            List<double> result= new List<double>() {};
            int size = 1000;
            double step = (t2 - t1) / size;
            for (int i = 0; i < size+1; i++)
            {
                double t = t1 + i * step;
                
                result.Add(ReversedLaguerreTransformation(hList, t));
            }

            return result;
        }
    }

    public class Function
    {
        public static double F(double t)
        {
            if (t >= 0 && t <= 2 * Math.PI)
            {
                return Math.Sin(t - Math.PI / 2) + 1;
            }
            else
            {
                return 0;
            }
        }

        public static double CustomFunc(double t) {
            if (t <= Math.PI / 3) {
                return Math.Tan(t);
            }
            else {
                return Math.Tan(Math.PI/3);
            }
        }

        public static Func<double, double> Gauss(double mu, double lambda)
        {
            if (lambda <= 0)
                throw new ArgumentException("lambda must be greater than 0");

            return t =>
            {
                double exponent = -Math.Pow((t - mu), 2) / (2 * Math.Pow(lambda, 2));
                double denominator = lambda * Math.Sqrt(2 * Math.PI);

                return Math.Exp(exponent) / denominator;
            };
        }

        class Program
        {
            static void Main(string[] args)
            {
                var lag = new Lagguerre();
                Console.WriteLine(lag.Laguerre(1,2));
                var tabulate_Lagguer = lag.TabulateLaguerre(10,2);
                foreach (var tab in tabulate_Lagguer)
                {
                    Console.WriteLine($"t {tab.Item1} | l {tab.Item2}");
                }
                var experiment = lag.Experiment(100);
                Console.WriteLine($"Experiment result: {experiment.Item1}");
                var tabulate_transform = lag.TabulateTransformation(Function.CustomFunc, 20);
                foreach (var tab_transform in tabulate_transform)
                {
                    Console.WriteLine($"Value: {tab_transform}");
                }
                var rev_transform = lag.ReversedLaguerreTransformation(tabulate_transform, Math.PI/3);
                Console.WriteLine($"Rer transform: {rev_transform}");

                var rev_transform_tab = lag.ReversedLaguerreTransformationTabulate(tabulate_transform, 0, Math.PI/3);
                foreach (var rev_transform_tabulate in rev_transform_tab)
                {
                    Console.WriteLine($"Reverse transform tabulate: {rev_transform_tabulate}");
                }
                
                
                //Transform
                Console.WriteLine("\n");
                Console.WriteLine("Transform Experiment");
                var Gaus_transform_tabulate = lag.TabulateTransformation(Function.Gauss( 4, 3), 20);
                foreach (var gauss in Gaus_transform_tabulate)
                {
                    Console.WriteLine($"Gauss: {gauss}");
                }
                // Reverse transform
                Console.WriteLine("\n");
                Console.WriteLine("Reverse transform experiment");
                var rev_transform_any = lag.ReversedLaguerreTransformation(Gaus_transform_tabulate, Math.PI);
                Console.WriteLine($"Reverse transform: {rev_transform_any}");
                
                
                //Experiment mu = 3*lambda
                Console.WriteLine("\n");
                Console.WriteLine("Experiment mu = 3*lambda");
                double lambda_1 = 5;
                var Gaus_transform_tabulate_exp_3 = lag.TabulateTransformation(Function.Gauss( 3*lambda_1, lambda_1), 20);
                foreach (var gauss in Gaus_transform_tabulate_exp_3)
                {
                    Console.WriteLine($"Gauss: {gauss}");
                }
                //Reverse Experiment mu = 3*lambda
                Console.WriteLine("\n");
                Console.WriteLine("Reverse transform experiment mu = 3*lambda");
                var rev_transform_exp_3 = lag.ReversedLaguerreTransformation(Gaus_transform_tabulate_exp_3, Math.PI);
                Console.WriteLine($"Reverse transform: {rev_transform_exp_3}");

                
                //Experiment mu = 6*lambda
                Console.WriteLine("\n");
                Console.WriteLine("Experiment mu = 6*lambda");
                double lambda_2 = 2;
                var Gaus_transform_tabulate_exp_6 = lag.TabulateTransformation(Function.Gauss( 6*lambda_2, lambda_2), 20);
                foreach (var gauss in Gaus_transform_tabulate_exp_6)
                {
                    Console.WriteLine($"Gauss: {gauss}");
                }
                //Experiment mu = 6*lambda
                Console.WriteLine("\n");
                Console.WriteLine("Reverse transform experiment mu = 6*lambda");
                var rev_transform_exp_6 = lag.ReversedLaguerreTransformation(Gaus_transform_tabulate_exp_6, Math.PI);
                Console.WriteLine($"Reverse transform: {rev_transform_exp_6}");

                var rev_transform_exp_1_tab = lag.ReversedLaguerreTransformationTabulate(Gaus_transform_tabulate, 0, Math.PI);
                var rev_transform_exp_3_tab = lag.ReversedLaguerreTransformationTabulate(Gaus_transform_tabulate_exp_3, 0, Math.PI);
                var rev_transform_exp_6_tab = lag.ReversedLaguerreTransformationTabulate(Gaus_transform_tabulate_exp_6, 0, Math.PI);
                
                
                
                WriteDictToFile(tabulate_Lagguer, @"D:\Homework\programming2024-teamproject\Lagger\Lagger\tabulatedLaguerre.csv");
                writeDateToFile(tabulate_transform, @"D:\Homework\programming2024-teamproject\Lagger\Lagger\tabulateTransform.csv");
                writeDateToFile(rev_transform_tab, @"D:\Homework\programming2024-teamproject\Lagger\Lagger\tabulateReverseTransform.csv");
                writeDateToFile(rev_transform_exp_1_tab, @"D:\Homework\programming2024-teamproject\Lagger\Lagger\tabulateReverseGauss1.csv");
                writeDateToFile(rev_transform_exp_3_tab, @"D:\Homework\programming2024-teamproject\Lagger\Lagger\tabulateReverseGauss3.csv");
                writeDateToFile(rev_transform_exp_6_tab, @"D:\Homework\programming2024-teamproject\Lagger\Lagger\tabulateReverseGauss6.csv");
                var allValues = new Dictionary<string, double>()
                {
                    {"experiment", Convert.ToDouble(experiment.Item1)  },
                    {"rev_transform",rev_transform},
                    {"rev_transform_any", rev_transform_any},
                    {"rev_transform_exp_3",rev_transform_exp_3},
                    {"rev_transform_exp_6",rev_transform_exp_6}
                };
                writeValuesToFile(allValues, @"D:\Homework\programming2024-teamproject\Lagger\Lagger\singleValues.csv");
            }
    
            
            // Use this for all List writing
            static void writeDateToFile<T>(List<T> incomeData, string path)
            {
                using(StreamWriter writetext = new StreamWriter(path))
                {
                    foreach (T data in incomeData)
                    {
                        writetext.WriteLine(data);
                    }
                }
            }
            
            // use this for "tabulate_Laguerre" 
            
            static void WriteDictToFile<T1, T2>(List<Tuple<T1, T2>> list, string path)
            {
                using (StreamWriter writetext = new StreamWriter(path))
                {
                    foreach (var data in list)
                    {
                        writetext.WriteLine($"{data.Item1}. {data.Item2}");
                    }
                }
            }

            
            // use this for writing dictionary of single values
            
            
            static void writeValuesToFile(Dictionary<string,double> values, string path)
            {
                using (StreamWriter writetext = new StreamWriter(path))
                {
                    foreach(KeyValuePair<string, double> entry in values)
                    {
                        writetext.WriteLine($"{entry.Key} {entry.Value}");
                    }
                }
            }
        }
        
    }
}

