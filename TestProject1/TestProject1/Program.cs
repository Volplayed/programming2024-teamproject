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
        public double Beta { get; set; }
        public double Sigma { get; set; }
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

    }
}

