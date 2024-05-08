using Xunit;
using Lab3;
using FluentAssertions;
using static Lab3.V1DataArray;

namespace Test_CL
{
    public class MainTest
    {

        public double b1 = 0;
        public double b2 = 1;
        public int nodesNum = 10;
        public int MainNodesCount = 10;
        public int AddonGridNodesCount = 100;
        public int MaximumIterations = 1000;
        public static double[] Function1(double x)
        {
            double y1 = x * x;
            double y2 = x;
            return new double[] { y1, y2 };
        }

        public static double[] Function2(double x)
        {
            double y1 = x;
            double y2 = x;
            return new double[] { y1, y2 };
        }

        public static double InitialFunction(double x) => x + 1;

        public (List<SplineDataItem>, List<double[]>) InitializeSplineAndRetrieveResults(int nodesNum,double b1,double b2,FValue F,int MainNodesCount,int AddonGridNodesCount,int MaximumIterations)
        {
            var VA = new V1DataArray("key", new DateTime(), nodesNum, b1, b2, F, true);
            var SD = new SplineData(VA, MainNodesCount, MaximumIterations, AddonGridNodesCount);
            SD.CalcSpline(SD, InitialFunction, true); // Calculate spline

            return (SD.ResSpline, SD.ResultOnAddonGrid); // Approximation result on the additional grid
        }

        [Fact]
        public void Test1()
        {
            (var SplineApproximationResult, var SplineOnAddonGridResult) = InitializeSplineAndRetrieveResults(nodesNum, b1, b2, Function1, MainNodesCount, AddonGridNodesCount, MaximumIterations);

            SplineApproximationResult
                .Zip(SplineApproximationResult, (first, second) => Math.Abs(first.realValue - second.resultValue))
                .Should()
                .OnlyContain(dy => dy < 10e-3);


            SplineOnAddonGridResult
                .Zip(SplineOnAddonGridResult, (first, second) => Math.Abs(first[0] * first[0] - second[1]))
                .Should()
                .OnlyContain(dy => dy < 10e-3); 


        }

        [Fact]
        public void Test2()
        {
            // Check approximation error 
            MainNodesCount = 5; 
            AddonGridNodesCount = 1000;
            (var SplineApproximationResult, var SplineOnAddonGridResult) = InitializeSplineAndRetrieveResults(nodesNum, b1, b2, Function1, MainNodesCount, AddonGridNodesCount, MaximumIterations);

            SplineApproximationResult
                .Zip(SplineApproximationResult, (first, second) => Math.Abs(first.realValue - second.resultValue))
                .Should()
                .OnlyContain(dy => dy < 10e-2);
            SplineOnAddonGridResult
                .Zip(SplineOnAddonGridResult, (first, second) => Math.Abs(second[1] - first[0] * first[0]))
                .Should()
                .OnlyContain(dy => dy < 10e-2);

        }

        [Fact]
        public void Test3()
        {
            // Check approximation error 
            MainNodesCount = 5; 
            AddonGridNodesCount = 1000;
            (var SplineApproximationResult, var SplineOnAddonGridResult) = InitializeSplineAndRetrieveResults(nodesNum, b1, b2, Function2, MainNodesCount, AddonGridNodesCount, MaximumIterations);

            SplineApproximationResult
                .Zip(SplineApproximationResult, (first, second) => Math.Abs(first.realValue - second.resultValue))
                .Should()
                .OnlyContain(dy => dy < 10e-2); 
            SplineOnAddonGridResult
                .Zip(SplineOnAddonGridResult, (first, second) => Math.Abs(first[0] - second[1]))
                .Should()
                .OnlyContain(dy => dy < 10e-2); 

        }
    }

}
