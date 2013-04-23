using System;
using System.Collections.Generic;
using System.Linq;
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
using System.Data;

namespace Reports.Documents
{
    /// <summary>
    /// Interaction logic for ReportBeginDay.xaml
    /// </summary>
    public partial class ReportSalesAnalysis : Page
    {
        public ReportSalesAnalysis()
        {
            InitializeComponent();
        }

        public void SetData(DataTable table)
        {
            this.dataGridSales.ItemsSource = table.DefaultView;
        }
    }
}
