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
    public partial class ReportBeginDay : Page
    {
        public ReportBeginDay()
        {
            InitializeComponent();
        }

        public void SetData(DataTable table, DateTime beginDate, DateTime endDate)
        {
            this.textBlockHeader.Text = String.Format("Анализ продаж за период с {0:dd.MM.yyyy} по {1:dd.MM.yyyy}", beginDate, endDate);
            this.dataGridSeats.ItemsSource = table.DefaultView;
        }
    }
}
