using System;
using System.Collections.Generic;
using Lagguer;
using Xunit;


namespace Lagguer.Tests
{
    public class LagguerTestsData : IDisposable
    {
        public Lagguerre lagguerre { get; set; }
        public List<Tuple<double, double>> TabulationResult { get; }
        public Tuple<double?, List<Tuple<double, List<double>>>> ExperimentResult { get; }
        public List<double> TabulateTransformationResult { get; }
        public List<double> TabulateGauss3byLambda { get; }
        public List<double> TabulateGauss6byLambda { get; }
        public List<double> TabulateTransformationGaussResult { get; }
        public double ReversedTransformationResult { get; }
        public double ReversedTransformationGaussResult { get; }
        public double ReversedGauss3byLambda { get; }
        public double ReversedGauss6byLambda { get; }
        public double lambda_1 = 5;
        public double lambda_2 = 2;

        public Integral integral { get; set; }
        public LagguerTestsData()
        {
            lagguerre = new Lagguerre(2, 4);
            TabulationResult = lagguerre.TabulateLaguerre(10, 2);
            TabulateTransformationGaussResult = lagguerre.TabulateTransformation(Function.Gauss(4, 3), 20);
            TabulateTransformationResult = lagguerre.TabulateTransformation(Function.F, 20);
            ReversedTransformationResult = lagguerre.ReversedLaguerreTransformation(TabulateTransformationResult, Math.PI);
            ReversedTransformationGaussResult = lagguerre.ReversedLaguerreTransformation(TabulateTransformationGaussResult, Math.PI);
            ExperimentResult = lagguerre.Experiment(100);
            TabulateGauss6byLambda = lagguerre.TabulateTransformation(Function.Gauss(6 * lambda_2, lambda_2), 20);
            TabulateGauss3byLambda = lagguerre.TabulateTransformation(Function.Gauss(3 * lambda_1, lambda_1), 20);
            ReversedGauss6byLambda = lagguerre.ReversedLaguerreTransformation(TabulateGauss6byLambda, Math.PI); ;
            ReversedGauss3byLambda = lagguerre.ReversedLaguerreTransformation(TabulateGauss3byLambda, Math.PI);
            integral = new Integral();
        }
        public void Dispose()
        {
            ExperimentResult.Item2.Clear();
            TabulationResult.Clear() ;
            TabulateTransformationGaussResult.Clear();
            TabulateTransformationResult.Clear();
            TabulateGauss6byLambda.Clear();
            TabulateGauss3byLambda.Clear();
        }
    }
    public class MyTestClasses : IClassFixture<LagguerTestsData>
    {
        LagguerTestsData fixture;
        public MyTestClasses(LagguerTestsData fixture)
        {
            this.fixture = fixture;
        }


        [Fact]
        public void TestConstructorsAndGetters()
        {
            double beta = 2;
            double sigma = 4;
            var lag = fixture.lagguerre;
            Assert.Equal(beta, actual: lag.Beta);
            Assert.Equal(sigma, lag.Sigma);
        }

    }


    public class MyLagguerTests : IClassFixture<LagguerTestsData>
    {
        LagguerTestsData fixture;
        public MyLagguerTests(LagguerTestsData fixture)
        {
            this.fixture = fixture;
        }

        [Theory]
        [InlineData(0.7358)]
        public void LaguerreFuncTest(double result)
        {
            var result1 = fixture.lagguerre.Laguerre(1, 2);
            Assert.Equal(result, Math.Round(result1,4));
        }

        [Theory]
        [InlineData(new double[]{0,0.5067,9.9,0.0708})]
        public void TabulateLaguerreTest(double[] arr)
        {
            var result = fixture.TabulationResult;
            Assert.Equal(100, result.Count);
            Assert.Equal(arr[0], Math.Round(result[0].Item1,4));
            Assert.Equal(arr[1], Math.Round(result[1].Item2,4));
            Assert.Equal(arr[2], Math.Round(result[99].Item1,1));
            Assert.Equal(arr[3], Math.Round(result[99].Item2,4));
        }

        [Theory]
        [InlineData(79.09999999999999, 1000)]
        public void ExperimentTest(double result, int count)
        {
            var result3 = fixture.ExperimentResult;
            Assert.Equal(result, result3.Item1);
            Assert.Equal(count, result3.Item2.Count);
        }
        [Theory]
        [InlineData(1.99994)]
        public void ReversedLaguerreTest(double data)
        {
            var result = fixture.ReversedTransformationResult;
            Assert.Equal(data, Math.Round(result,5));
        }
        [Theory]
        [InlineData(new double[]{0.0425,0.0005,0.0005})]
        public void TabulateGaussTransformationTest(double[] arr)
        {
            var result = fixture.TabulateTransformationGaussResult;
            Assert.Equal(21, result.Count);
            Assert.Equal(arr[0], Math.Round(result[0],4));
            Assert.Equal(arr[1], Math.Round(result[11],4));
            Assert.Equal(arr[2], Math.Round(result[20],4));
        }
        [Theory]
        [InlineData(0.1256)]
        public void TabulateReversedGaussTransformationTest(double res)
        {
            var result = fixture.ReversedTransformationGaussResult;
            Assert.Equal(res,Math.Round(result,4));
        }
        [Fact]
        public void TabulateGauss3byLambdaTest()
        {
            var result = fixture.TabulateGauss3byLambda;
            Assert.Equal(21, result.Count);
            Assert.Equal(0.0007, Math.Round(result[0],4));
            Assert.Equal(7.07136699545659e-06, result[11]);
            Assert.Equal(7.771885685696491e-06, result[20]);
        }
        [Fact]
        public void TabulateGauss6byLambdaTest()
        {
            var result = fixture.TabulateGauss6byLambda;
            Assert.Equal(21, result.Count);
            Assert.Equal(2E-08, Math.Round(result[0],8));
            Assert.Equal(-2.49E-06, Math.Round(result[11],8));
            Assert.Equal(-1E-11, Math.Round(result[20],11));
        }
        [Theory]
        [InlineData(0.0048)]
        public void ReversedGauss3byLambdaTest(double res)
        {
            var result = fixture.ReversedGauss3byLambda;
            Assert.Equal(res, Math.Round(result,4));
        }
        [Fact]
        public void ReversedGauss6byLambdaTest()
        {
            var result = fixture.ReversedGauss6byLambda; 
            Assert.Equal(1E-05, Math.Round(result,5));
        }

        [Theory]
        [InlineData(0.25)]
        public void TestIntegrateQuad(double res)
        {
            Func<double, double> testFunction = x => x * x * x; 
            double lowerBound = 0;
            double upperBound = 1;
            int numberOfSteps = 100; 
 

            double calculatedIntegralValue = Integral.IntegrateQuad(testFunction, lowerBound, upperBound, numberOfSteps);
            
            Assert.Equal(res, Math.Round(calculatedIntegralValue,2)); 
        }
    }

}


