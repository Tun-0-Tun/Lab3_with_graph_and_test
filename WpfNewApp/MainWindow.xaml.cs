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
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Win32;

using OxyPlot;
using OxyPlot.Series;

namespace WpfNewApp
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        Data mainViewData = new Data();
        public MainWindow()
        {
            InitializeComponent();

            //mainViewData = new Data();
            BindConnections(mainViewData);
        }


        public void BindConnections(Data VD)
        {
            Binding GridBoundariesBoxBind = new Binding();
            GridBoundariesBoxBind.Source = VD;
            GridBoundariesBoxBind.Path = new PropertyPath("LeftBound");
            GridBoundariesBox.SetBinding(TextBox.TextProperty, GridBoundariesBoxBind);

            Binding NumberOfNodesBoxBind = new Binding(); // Ввод границ равномерной сетки
            NumberOfNodesBoxBind.Source = VD;
            NumberOfNodesBoxBind.Path = new PropertyPath("NodesNum");

            NumberOfNodesBox.SetBinding(TextBox.TextProperty, NumberOfNodesBoxBind);    
        }

        public void CheckControlsItem_Click(object sender, CanExecuteRoutedEventArgs e)
        {
            try
            {
                bool result = true;
                string? error = null;
                string[] error_types = { "arrayNodesAmount", "splineCalculationNodesAmount" };

                foreach (var field in error_types)
                {
                    if ((mainViewData.NodesNum < 100 && mainViewData.NodesNum > 3) && (mainViewData.LeftBound <= 1))
                    {
                        e.CanExecute = true;

                    }
                    else { e.CanExecute = false; }
                }
            }
            catch (Exception ex)
            {
                e.CanExecute = false;
            }
        }
        private void DataFromControlsItem_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                mainViewData.Calculation();
                mainViewData.plotModel = new MainPlot(mainViewData);
                this.DataContext = null;
                this.DataContext = mainViewData;


            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }

    }
}