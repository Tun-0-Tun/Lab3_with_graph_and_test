using Xunit;
using Lab3;
using FluentAssertions;
using static Lab3.V1DataArray;
using ViewModel;
using System;
using Moq;

namespace Test_VM
{
    public class MainTest
    {
        public ViewData MainViewData;

        public double[] GridBoundaries = new double[] { 0, 1 }; 
        public int NodesNum = 10;        
        public bool IsGridUniform = true;  
        public int FuncId = 0;  

        public int SmoothingSplineNum = 10;   
        public int UniformNum = 100; 
        public int MaxIterations = 500;  
        public double DiscrepancyRate = 1E-06;
     
        [Fact]
        public void TestBoundaryError()
        {
            var Model = new Mock<IUIServices>();
            MainViewData = new ViewData(Model.Object);
            MainViewData.GridBoundaries = GridBoundaries;
            MainViewData.NodesNum = NodesNum;
            MainViewData.IsGridUniform = IsGridUniform;
            MainViewData.FuncId = FuncId;

            MainViewData.SmoothingSplineNum = SmoothingSplineNum;
            MainViewData.UniformNum = UniformNum;
            MainViewData.MaxIterations = MaxIterations;
            MainViewData.DiscrepancyRate = DiscrepancyRate;
            

            MainViewData.GridBoundaries =  new double[] { 0, 1 };
   
            MainViewData.RunCommand.CanExecute(null).Should().BeTrue();

            MainViewData.GridBoundaries = new double[] { 1, 0 };
            MainViewData.RunCommand.CanExecute(null).Should().BeFalse();
        }

        [Fact]
        public void TestNodesNum1()
        {
            var Model = new Mock<IUIServices>();
            MainViewData = new ViewData(Model.Object);
            MainViewData.GridBoundaries = GridBoundaries;
            MainViewData.IsGridUniform = IsGridUniform;
            MainViewData.FuncId = FuncId;

            MainViewData.SmoothingSplineNum = SmoothingSplineNum;
            MainViewData.UniformNum = UniformNum;
            MainViewData.MaxIterations = MaxIterations;
            MainViewData.DiscrepancyRate = DiscrepancyRate;

            MainViewData.NodesNum = -1;
            MainViewData.SmoothingSplineNum = -1;

            MainViewData.RunCommand.CanExecute(null).Should().BeFalse();


            MainViewData.NodesNum = 11;
            MainViewData.SmoothingSplineNum = 15;

            MainViewData.RunCommand.CanExecute(null).Should().BeFalse();


            MainViewData.NodesNum = 5;
            MainViewData.SmoothingSplineNum = -1;

            MainViewData.RunCommand.CanExecute(null).Should().BeFalse();


            MainViewData.NodesNum = 5;
            MainViewData.SmoothingSplineNum = 1;
 
            MainViewData.RunCommand.CanExecute(null).Should().BeFalse();


            MainViewData.NodesNum = 10;
            MainViewData.SmoothingSplineNum = 10;
   
            MainViewData.RunCommand.CanExecute(null).Should().BeTrue();
        }
        [Fact]
        public void TestNodesNum2()
        {

            var Model = new Mock<IUIServices>();
            MainViewData = new ViewData(Model.Object);
            MainViewData.GridBoundaries = GridBoundaries;
            MainViewData.IsGridUniform = IsGridUniform;
            MainViewData.FuncId = FuncId;
            MainViewData.MaxIterations = MaxIterations;
            MainViewData.DiscrepancyRate = DiscrepancyRate;

            MainViewData.NodesNum = 15;
            MainViewData.SmoothingSplineNum = 15;
            MainViewData.UniformNum = -4;

            MainViewData.RunCommand.CanExecute(null).Should().BeFalse();


            MainViewData.NodesNum = 15;
            MainViewData.SmoothingSplineNum = 15;
            MainViewData.UniformNum = 2;

            MainViewData.RunCommand.CanExecute(null).Should().BeFalse();


            MainViewData.NodesNum = 15;
            MainViewData.SmoothingSplineNum = 15;
            MainViewData.UniformNum = 3;

            MainViewData.RunCommand.CanExecute(null).Should().BeTrue();
        }

    }

}
