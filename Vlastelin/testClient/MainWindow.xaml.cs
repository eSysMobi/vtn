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
using System.ServiceModel;
using System.ServiceModel.Security;
using Vlastelin.Data.Model;
using System.Reflection;
using Vlastelin.Common;
using System.Data;
using System.Xml;
using System.Xml.Serialization;
using System.IO;
using Vlastelin.Data.Model.ExportedData;
using System.Xml.Xsl;

namespace testClient
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        [Serializable]
        public class DBError
        {
            [XmlElement]
            public int code;
            [XmlElement]
            public string description;
        }

        ServiceReference1.VlastelinSrvClient client;
        public MainWindow()
        {

            InitializeComponent();
        }

        private void button1_Click(object sender, RoutedEventArgs e)
        {
            try
            {

                MessageBox.Show(client.GetData(int.Parse(textBox1.Text)));
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void button2_Click(object sender, RoutedEventArgs e)
        {
            // getdata
            ServiceReference1.VlastelinSrvClient _client = new ServiceReference1.VlastelinSrvClient();
            {
                _client.ClientCredentials.UserName.UserName = textBox1.Text;
                _client.ClientCredentials.UserName.Password = textBox2.Text;
                _client.ClientCredentials.ServiceCertificate.Authentication.CertificateValidationMode =
                    System.ServiceModel.Security.X509CertificateValidationMode.PeerOrChainTrust;

                try
                {
                    client = _client;

                    string res = client.GetData(123);

                    MessageBox.Show(res);



                    //_client.SeatLock(seat);

                }
                /*catch (FaultException<ServiceReference1.VlastelinFault> ex)
                {
                    client.Abort();
                    MessageBox.Show(ex.Detail.message, "VlastelinFault");
                }*/
                catch (FaultException ex)
                {
                    client.Abort();
                    MessageBox.Show(ex.Message, "-=FaultException=-");
                }
                catch (MessageSecurityException ex)
                {
                    client.Abort();
                    MessageBox.Show(ex.InnerException.Message, "-=MessageSecurityException=-");
                }
                
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message,"-=Exception=-");
                }
                
            }
            //long newId=client.Buses
        }

        private void button1_Click_1(object sender, RoutedEventArgs e)
        {
            try
            {
                MessageBox.Show(client.GetData(1));
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void button3_Click(object sender, RoutedEventArgs e)
        {
            Vlastelin.KKM.KKM kkm = new Vlastelin.KKM.KKM();

            //string status = string.Format("Модель: {0}\nСерНом: {1}\nРегНом: {2}\nКонтрСчетчик: {3}\nMode: {4}",
            //    kkm.KKMModel,
            //    kkm.SerialNum,
            //    kkm.RegistrationNum,
            //    kkm.ControlCounterValue,
            //    kkm.ModeString);
            //MessageBox.Show(status);
            string fileName =
                System.IO.Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location)+ "\\logo1.bmp";
            kkm.LogoForCheck = new System.Drawing.Bitmap(fileName);

            Vlastelin.Data.Model.Seat ticket = new Vlastelin.Data.Model.Seat()
            {
                Passenger = new Passenger() { Name = "test", Surname = "test", Patronymic = "test", DocSer = "1234", DocNum = 123456 },
                SeatNumber = 100,
                SS = new StationSchedule()
                {
                    DepartureTime = new DateTime(2012, 11, 11),
                    TS = new TripSchedule()
                    {
                        Trip = new Trip() { Name = "123-312" },
                        Bus = new Bus() { Manufacter = "TEST", Model = "mmm", RegNumber = "аа 111 аа 64rus" }
                    }
                },
                TripPrice = new TripPrice() { Arrival = new Town() { Name = "Мухосранск" }, Departure = new Town() { Name = "Саратов" } },
                DesiredDestination = ""
            };
            kkm.SellTicket(ticket,1500.0);
            string docNum = string.Format("Последний номер чека продажи: {0}\nПоследний сквозной номер: {1}", kkm.LastSellCheckNum, kkm.LastPrintedDocNum);
            kkm.SellTicket(ticket, 2500.0);
            kkm.SellTicket(ticket, 4500.0);
            docNum += string.Format("\nПоследний номер чека продажи: {0}\nПоследний сквозной номер: {1}", kkm.LastSellCheckNum, kkm.LastPrintedDocNum);

            MessageBox.Show(docNum);
        }

        private void button4_Click(object sender, RoutedEventArgs e)
        {
            Vlastelin.KKM.KKM kkm = new Vlastelin.KKM.KKM();

            string str = string.Format("Номер последнего платежного документа: {0}\nНомер последнего чека продажи: {1}\nНомер последнего чека покупки: {2}",
                kkm.LastPrintedDocNum, kkm.LastSellCheckNum, kkm.LastBuyCheckNum);
            MessageBox.Show(str);
        }

        private void button5_Click(object sender, RoutedEventArgs e)
        {

            string fName = "e:\\1.xml";
            string fName2 = "e:\\1_done.txt";
            //StreamWriter sw = File.CreateText(fName);
            // XmlSerializer ser = new XmlSerializer(typeof(ExportedRKO[]));
            //ser.Serialize(sw, list.ToArray());
            //sw.Close();

            XslCompiledTransform trans = new XslCompiledTransform();

            trans.Load(@"e:\\2.xsl");
            trans.Transform(fName, fName2);


            //return File.ReadAllText(fName2);
        }
    }
}
