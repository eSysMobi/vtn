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
using Vlastelin.Data.Model;
using System.Data;

namespace VlastelinClient.Reports.Documents
{
    /// <summary>
    /// Interaction logic for ReportPassengerTrips.xaml
    /// </summary>
    public partial class ReportPassengerTrips : Page
    {
        public ReportPassengerTrips()
        {
            InitializeComponent();
        }

        public void SetData(DataTable data)
        {
            this.dataGridTrips.ItemsSource = data.DefaultView;
        }
    }
}
