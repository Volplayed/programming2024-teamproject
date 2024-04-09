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

        [Fact]
        public void LaguerreFuncTest()
        {
            var result1 = fixture.lagguerre.Laguerre(1, 2);
            Assert.Equal(0.7357588823428847, result1);
        }

        [Fact]
        public void TabulateLaguerreTest()
        {
            var result = fixture.TabulationResult;
            Assert.Equal(100, result.Count);
            Assert.Equal(0, result[0].Item1);
            Assert.Equal(0.5067089541001375, result[1].Item2);
            Assert.Equal(9.9, result[99].Item1);
            Assert.Equal(0.07083460913962603, result[99].Item2);
        }

        [Fact]
        public void ExperimentTest()
        {
            var result3 = fixture.ExperimentResult;
            Assert.Equal(79.09999999999999, result3.Item1);
            Assert.Equal(1000, result3.Item2.Count);
        }
        [Fact]
        public void ReversedLaguerreTest()
        {
            var result = fixture.ReversedTransformationResult;
            Assert.Equal(1.9999391945011342, result);
        }
        [Fact]
        public void TabulateGaussTransformationTest()
        {
            var result = fixture.TabulateTransformationGaussResult;
            Assert.Equal(21, result.Count);
            Assert.Equal(0.042525538269538296, result[0]);
            Assert.Equal(0.00045891574543354086, result[11]);
            Assert.Equal(0.00047944669028616326, result[20]);
        }
        [Fact]
        public void TabulateReversedGaussTransformationTest()
        {
            var result = fixture.ReversedTransformationGaussResult;
            Assert.Equal(0.1255524823358316,result);
        }
        [Fact]
        public void TabulateGauss3byLambdaTest()
        {
            var result = fixture.TabulateGauss3byLambda;
            Assert.Equal(21, result.Count);
            Assert.Equal(0.0007406486382301851, result[0]);
            Assert.Equal(7.07136699545659e-06, result[11]);
            Assert.Equal(7.771885685696491e-06, result[20]);
        }
        [Fact]
        public void TabulateGauss6byLambdaTest()
        {
            var result = fixture.TabulateGauss6byLambda;
            Assert.Equal(21, result.Count);
            Assert.Equal(1.5254009861383603e-08, result[0]);
            Assert.Equal(-2.4906530634117345e-06, result[11]);
            Assert.Equal(-1.4782714335838103e-11, result[20]);
        }
        [Fact]
        public void ReversedGauss3byLambdaTest()
        {
            var result = fixture.ReversedGauss3byLambda;
            Assert.Equal(0.00475802334115808, result);
        }
        [Fact]
        public void ReversedGauss6byLambdaTest()
        {
            var result = fixture.ReversedGauss6byLambda; 
            Assert.Equal(1.098169283367328e-05, result);
        }
    }

}


