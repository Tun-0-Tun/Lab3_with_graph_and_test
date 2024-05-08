using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Lab3;
using Microsoft.Win32;

using OxyPlot;
using OxyPlot.Series;
using OxyPlot.Wpf;
using ViewModel;

namespace lab1_sem6_wpf
{
    public partial class MainWindow : Window, IUIServices
    {

        ViewData mainViewData;
        public MainWindow()
        {
            InitializeComponent();

            mainViewData = new ViewData(this);
            BindConnections(mainViewData);
            DataContext = mainViewData;

            this.DataContext = null;
            this.DataContext = mainViewData;
        }
        public void ReportError(string message)
        {
            MessageBox.Show($"Error:\n" + message);
        }

        public void BindConnections(ViewData VD)
        {


            ParseBoundaries SD_Converter = new ParseBoundaries();
            Binding GridBoundariesBoxBind = new Binding();
            GridBoundariesBoxBind.Source = VD;
            GridBoundariesBoxBind.Path = new PropertyPath("GridBoundaries");
            GridBoundariesBoxBind.Converter = SD_Converter;
            GridBoundariesBox.SetBinding(TextBox.TextProperty, GridBoundariesBoxBind);



            Binding NumberOfNodesBoxBind = new Binding(); // Ввод границ равномерной сетки
            NumberOfNodesBoxBind.Source = VD;
            NumberOfNodesBoxBind.Path = new PropertyPath("NodesNum");

            NumberOfNodesBox.SetBinding(TextBox.TextProperty, NumberOfNodesBoxBind);

            Binding FuncBoxBind = new Binding(); // Ввод числа узлов сетки
            FuncBoxBind.Source = VD;
            FuncBoxBind.Path = new PropertyPath("FuncId");
            FuncBox.SetBinding(ComboBox.SelectedIndexProperty, FuncBoxBind);


            Binding Radio1Bind = new Binding(); // Ввод числа узлов сглаживающего сплайна
            Radio1Bind.Source = VD;
            Radio1Bind.Path = new PropertyPath("IsGridUniform");
            Radio1.SetBinding(RadioButton.IsCheckedProperty, Radio1Bind);


            // Создаём экземпляры RadioButton
            RadioButton radioButtonUniform = new RadioButton();
            RadioButton radioButtonNonuniform = new RadioButton();

            radioButtonUniform.Content = "Uniform";
            radioButtonNonuniform.Content = "Nonuniform";

            Binding bindingUniform = new Binding("IsGridUniform");
            Binding bindingNonuniform = new Binding("IsGridUniform");

            // Установите источник привязки (ваша ViewModel)
            bindingUniform.Source = VD;
            bindingNonuniform.Source = VD;

            bindingUniform.Mode = BindingMode.OneWayToSource;
            bindingNonuniform.Mode = BindingMode.OneWayToSource;

            radioButtonUniform.SetBinding(RadioButton.IsCheckedProperty, bindingUniform);
            radioButtonNonuniform.SetBinding(RadioButton.IsCheckedProperty, bindingNonuniform);


            Binding SmoothingSplineBoxBind = new Binding();
            SmoothingSplineBoxBind.Source = VD;
            SmoothingSplineBoxBind.Path = new PropertyPath("SmoothingSplineNum");
            SmoothingSplineBox.SetBinding(TextBox.TextProperty, SmoothingSplineBoxBind);

            Binding UniformNumBoxBind = new Binding();
            UniformNumBoxBind.Source = VD;
            UniformNumBoxBind.Path = new PropertyPath("UniformNum");
            UniformNumBox.SetBinding(TextBox.TextProperty, UniformNumBoxBind);

            Binding DiscrepancyRateBoxBind = new Binding();
            DiscrepancyRateBoxBind.Source = VD;
            DiscrepancyRateBoxBind.Path = new PropertyPath("DiscrepancyRate");
            DiscrepancyRateBox.SetBinding(TextBox.TextProperty, DiscrepancyRateBoxBind);

            Binding MaxIterationsBoxBind = new Binding();
            MaxIterationsBoxBind.Source = VD;
            MaxIterationsBoxBind.Path = new PropertyPath("MaxIterations");
            MaxIterationsBox.SetBinding(TextBox.TextProperty, MaxIterationsBoxBind);

            Binding PlotBind = new Binding();
            PlotBind.Source = VD;
            PlotBind.Path = new PropertyPath("plotModel.plotModel");
            //PlotViewBox.SetBinding(PlotView.ModelProperty, PlotBind);
        }


        private void SaveItem_Click(object sender, RoutedEventArgs e)
        {
            try
            {

                SaveFileDialog saveFileDialog = new SaveFileDialog();
                string FilePath = "";
                if (saveFileDialog.ShowDialog() == true)
                {
                    FilePath = saveFileDialog.FileName;
                }
                mainViewData.InitializeThroughControl();
                mainViewData.Save(FilePath);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }

    }
}