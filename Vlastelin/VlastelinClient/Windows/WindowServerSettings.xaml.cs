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
using System.Windows.Shapes;
using Vlastelin.Data.Model;
using VlastelinClient.Util;

namespace VlastelinClient.Windows
{
    /// <summary>
    /// Interaction logic for WindowServerSetting.xaml
    /// </summary>
    public partial class WindowServerSettings : WindowBase
    {
        public MainSettings Settings { get; set; }
        
        public WindowServerSettings()
        {
            InitializeComponent();
        }

        private void WindowBase_Loaded(object sender, RoutedEventArgs e)
        {
            this.textBoxOrg.Text = Settings.OrganizationName;
            this.textBoxName.Text = Settings.OrganizationDirName;
            this.textBoxSurname.Text = Settings.OrganizationDirSurname;
            this.textBoxPatr.Text = Settings.OrganizationDirPatronymic;
            this.textBoxINN.Text = Settings.OrganizationINN;
            this.textBoxKPP.Text = Settings.OrganizationKPP;
            this.textBoxCorr.Text = Settings.OrganizationCorrAccount;
			this.doubleUpDownSum.Value = (double)Settings.ReturnedCommission / 100;
        }

        private void ButtonSave_Click(object sender, RoutedEventArgs e)
        {
			FuncExec.Execute(() =>
				{
					Settings.OrganizationName = this.textBoxOrg.Text;
					Settings.OrganizationDirName = this.textBoxName.Text;
					Settings.OrganizationDirSurname = this.textBoxSurname.Text;
					Settings.OrganizationDirPatronymic = this.textBoxPatr.Text;
					Settings.OrganizationINN = this.textBoxINN.Text;
					Settings.OrganizationKPP = this.textBoxKPP.Text;
					Settings.OrganizationCorrAccount = this.textBoxCorr.Text;
					Settings.ReturnedCommission = (decimal)this.doubleUpDownSum.Value * 100;

					UtilManager.Instance.Client.MainSettingsEdit(Settings);
					UtilManager.Instance.MessageProvider.ShowInformationWindow("Настройки успешно сохранены!");
				});
        }

        private void ButtonExit_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
