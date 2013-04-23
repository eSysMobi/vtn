using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Vlastelin.Data.Model;
using System.Drawing;
using Vlastelin.Common;
using System.ComponentModel;

namespace Vlastelin.KKM
{   
    public enum KKMMode
    {
        PrinterWorking=0,
        DataOutput,
        OpenedSession_before24h,
        OpenedSession_after24h,
        ClosedSession,
        Locked_invalidInspectorPwd,
        WaitingDateConfirmation,
        AllowDotChanging,
        DocumentOpened,
        AllowReset,
        Test=10,
        FullFReportPrint,
        LongEKLZReportPrint,
        FPDocWork,
        PDocPrint,
        FPDocReady
    }

    public enum CheckType
    {
        Sell=0,
        Buy=1,
        SellReturn,
        BuyReturn
    };

    public enum CheckCutType
    {
        [Description("Отсутствует")]
        None=0,
        [Description("Полностью")]
        Full=1,
        [Description("Частично")]
        Partial=2
    };

    public enum TaxesPrintingFormat
    {
        [Description("Отсутствует")]
        None=0,
        [Description("Короткий")]
        Short=1,
        [Description("Длинный")]
        Long=2
    };

    /// <summary>
    /// Класс для прозрачной работы с ККМ
    /// </summary>
    public class KKM : IDisposable
    {
        [System.Runtime.InteropServices.DllImport("user32.dll")]
        private static extern IntPtr GetForegroundWindow();
        /// <summary>
        /// признак работы в тестовом режиме
        /// </summary>
        public bool TestMode { get { return true; } }

        public Bitmap LogoForCheck;
        private bool isImageFirstTimeLoaded = true;

        private KKMAdapter _kkm;

        public KKM()
        {
            _kkm = new KKMAdapter();
            _kkm.Connect();
        }

        public KKM(int comPortIndex)
        {
            _kkm = new KKMAdapter();
            _kkm.Connect(comPortIndex);
        }

        #region Settings
        /// <summary>
        /// Автоматическое обнуление денежной наличности при закрытии смены
        /// </summary>
        public bool ZeroCashWhenCloseSession
        {
            get { return _kkm.ReadTableIntValue(1, 1, 2) == 1; }
            set { _kkm.WriteTableValue(1, 1, 2, value ? 1 : 0); }
        }

        /// <summary>
        /// Печать рекламного текста
        /// </summary>
        public bool PrintFooterext
        {
            get { return _kkm.ReadTableIntValue(1, 1, 3) == 1; }
            set { _kkm.WriteTableValue(1, 1, 3, value ? 1 : 0); }
        }

        /// <summary>
        /// Отрезка чека
        /// </summary>
        public CheckCutType CheckCutType
        {
            get { return (CheckCutType)_kkm.ReadTableIntValue(1, 1, 7); }
            set { _kkm.WriteTableValue(1, 1, 7, (int)value); }
        }

        /// <summary>
        /// Открывать денежный ящик после печати чека
        /// </summary>
        public bool OpenCashBoxWhenFheckClosed
        {
            get { return _kkm.ReadTableIntValue(1, 1, 6) == 1; }
            set { _kkm.WriteTableValue(1, 1, 6, value ? 1 : 0); }
        }

        /// <summary>
        /// Автоматический перевод времени
        /// </summary>
        public bool AutomaticDaylightShift
        {
            get { return _kkm.ReadTableIntValue(1, 1, 15) == 1; }
            set { _kkm.WriteTableValue(1, 1, 15, value ? 1 : 0); }
        }

        /// <summary>
        /// Печать налогов
        /// </summary>
        public TaxesPrintingFormat TaxesPrintingFormat
        {
            get { return (TaxesPrintingFormat)_kkm.ReadTableIntValue(1, 1, 16); }
            set { _kkm.WriteTableValue(1, 1, 16, (int)value); }
        }

        /// <summary>
        /// Промотка ленты перед отрезкой чека
        /// </summary>
        public bool CheckScrollBeforeCut
        {
            get { return _kkm.ReadTableIntValue(1, 1, 21) == 1; }
            set { _kkm.WriteTableValue(1, 1, 21, value ? 1 : 0); }
        }
        #endregion

        public DateTime InternalTime
        {
            get 
            {
                return _kkm.GetTime();
            }
            set 
            {
                _kkm.SetTime(value);
            }
        }

        public void TestPrint(string str)
        {
            _kkm.WriteLine(str);
        }

        public string KKMModel { get { return _kkm.GetModel(); } }

        public string SerialNum { get { return _kkm.GetSerialNum(); } }
        public string RegistrationNum { get { return _kkm.GetRegistrationNum(); } }

        // значения для справка-отчета кассира
        // 1я графа
        public long ControlCounterValue { get { return _kkm.GetOperationRegister(KKMAdapter.OpRegisters.LastZReportNum)+1; } }

        // суммирующий денежный счетчик
        public decimal IncrementalControlCounterValue {get{return _kkm.GetMoneyRegister(KKMAdapter.MoneyRegisters.TotalMoneyAmountBeforeFiscalization);}}


        public decimal AccMoneyCounterValue { get { return _kkm.GetMoneyRegister(KKMAdapter.MoneyRegisters.TotalMoneyAmountBeforeFiscalization); } }
        public decimal TotalRevenue { 
            get {
                decimal sales = _kkm.GetMoneyRegister(KKMAdapter.MoneyRegisters.SessionSell);
                return sales;
            } 
        }
        public decimal TotalReturn { 
            get {
                decimal ret = _kkm.GetMoneyRegister(KKMAdapter.MoneyRegisters.SessionSellReturn);
                return ret;
            }  }

        public KKMMode Mode { get { return _kkm.GetKKMMode(); } }
        public string ModeString { get { return _kkm.GetKKMModeString(); } }

        // номера последних зафиксированных документов
        public int LastSellCheckNum { 
            get {
                System.Diagnostics.Debug.WriteLine(string.Format("LastSellCheckNum call. KKMmode: {0}", _kkm.GetKKMMode())); 
                return _kkm.GetOperationRegister(KKMAdapter.OpRegisters.LastCheckSell); 
            } 
        }
        public int LastSellReturnCheckNum { get { return _kkm.GetOperationRegister(KKMAdapter.OpRegisters.LastCheckSellReturn); } }
        public int LastBuyCheckNum { get { return _kkm.GetOperationRegister(KKMAdapter.OpRegisters.LastCheckBuy); } }
        public int LastPrintedDocNum { get { return _kkm.GetOperationRegister(KKMAdapter.OpRegisters.LastDocNum); } }


        public void SellTicket(Seat seat, double sum)
        {
            //axDrvFR1.UseReceiptRibbon = true;
            //_kkm.WriteLine("ИП Борунов");

            if (LogoForCheck!= null)
                PrintLogo();
            _kkm.OpenCheck(CheckType.Sell);

            _kkm.WriteLine57("------------------------------------------------");
            _kkm.WriteLine("БИЛЕТ НА ПРОЕЗД");
            _kkm.WriteLine57("------------------------------------------------");
            _kkm.WriteLine("Телефон: (8452) 50-18-74, 91-31-61");


            _kkm.WriteLine57(string.Format("МАРШРУТ: {0}", seat.SS.TS.Trip.Name));
            _kkm.WriteLine57(string.Format("ПУНКТ ОТПРАВЛЕНИЯ: {0}", seat.TripPrice.Departure.Name));
            _kkm.WriteLine57(string.Format("ПУНКТ НАЗНАЧЕНИЯ: {0}", seat.DesiredDestination));
            _kkm.WriteLine(string.Format("ДАТА РЕЙСА: {0:dd.MM.yyyy}", seat.TripDate));
            _kkm.WriteLine(string.Format("ВРЕМЯ {0:HH:mm:ss; } МЕСТО {1}",
                seat.SS.DepartureTime,
                seat.SeatNumber));
            _kkm.WriteLine57("------------------------------------------------");
            _kkm.WriteLine57(string.Format("ФИО {0} {1}. {2}.",
                seat.Passenger.Surname,
                seat.Passenger.Name.Substring(0, 1),
                seat.Passenger.Patronymic.Substring(0, 1)));
            _kkm.WriteLine(string.Format("ПАСПОРТ:  СЕР. {0} № {1}",
                seat.Passenger.DocSer,
                seat.Passenger.DocNum));
            _kkm.WriteLine57("------------------------------------------------");
            _kkm.WriteLine57(string.Format("АВТОБУС - {0} {1}( {2} )",
                seat.SS.TS.Bus.Manufacter,
                seat.SS.TS.Bus.Model,
                seat.SS.TS.Bus.RegNumber));
            _kkm.WriteLine57("------------------------------------------------");
            _kkm.WriteLine("");

            _kkm.SaleOneItem((decimal)sum, 1, "");
            _kkm.CloseCheck((decimal)sum, 1, "");
            WaitForPrintingToComplete();
        }

        public void SellItem(SalesKind item, double sum)
        {
            if (LogoForCheck != null)
                PrintLogo();
            _kkm.OpenCheck(CheckType.Sell);

            _kkm.WriteLine57("------------------------------------------------");
            _kkm.WriteLine(item.Name);
            _kkm.WriteLine57("------------------------------------------------");
            if (sum == 0) sum = item.Price;

            _kkm.SaleOneItem((decimal)sum, 1, "");
            _kkm.CloseCheck((decimal)sum, 1, "");
            WaitForPrintingToComplete();
        }

        public void CashIncome(double sum)
        {
            if (LogoForCheck != null)
                PrintLogo();
            _kkm.CashIncome(sum);
        }

        public void CashOutcome(double sum)
        {
            if (LogoForCheck != null)
                PrintLogo();
            _kkm.CashOutcome(sum);
        }


        public void PrintLogo()
        {
            WaitForPrintingToComplete();
            if (isImageFirstTimeLoaded)
            {
                _kkm.LoadAndPrintImage(LogoForCheck, true);
                isImageFirstTimeLoaded = false;
            }
            else
                _kkm.PrintLastImage();
            WaitForPrintingToComplete();
        }

        public void ShowTablesConfig()
        {
            _kkm.ShowTablesDlg(GetForegroundWindow());
        }

        public void SessionReport()
        {
            _kkm.SessionReport();
        }

        public void ReturnTicket(Seat seat, double sum, double commission)
        {
            if (LogoForCheck != null)
                PrintLogo();
            _kkm.OpenCheck(CheckType.SellReturn);

            _kkm.WriteLine57("------------------------------------------------");
            _kkm.WriteLine("БИЛЕТ НА ПРОЕЗД");
            _kkm.WriteLine57("------------------------------------------------");
            _kkm.WriteLine("Телефон: (8452) 50-18-74, 91-31-61");


            _kkm.WriteLine57(string.Format("МАРШРУТ: {0}", seat.SS.TS.Trip.Name));
            _kkm.WriteLine57(string.Format("ПУНКТ ОТПРАВЛЕНИЯ: {0}", seat.TripPrice.Departure.Name));
            _kkm.WriteLine57(string.Format("ПУНКТ НАЗНАЧЕНИЯ: {0}", seat.DesiredDestination));
            _kkm.WriteLine(string.Format("ДАТА РЕЙСА: {0:dd.MM.yyyy}", seat.TripDate));
            _kkm.WriteLine(string.Format("ВРЕМЯ {0:HH:mm:ss; } МЕСТО {1}",
                seat.SS.DepartureTime,
                seat.SeatNumber));
            _kkm.WriteLine57("------------------------------------------------");
            _kkm.WriteLine57(string.Format("ФИО {0} {1}. {2}.",
                seat.Passenger.Surname,
                seat.Passenger.Name.Substring(0, 1),
                seat.Passenger.Patronymic.Substring(0, 1)));
            _kkm.WriteLine(string.Format("ПАСПОРТ:  СЕР. {0} № {1}",
                seat.Passenger.DocSer,
                seat.Passenger.DocNum));
            _kkm.WriteLine57("------------------------------------------------");
            _kkm.WriteLine57(string.Format("АВТОБУС - {0} {1}( {2} )",
                seat.SS.TS.Bus.Manufacter,
                seat.SS.TS.Bus.Model,
                seat.SS.TS.Bus.RegNumber));
            _kkm.WriteLine57("------------------------------------------------");
            _kkm.WriteLine("");

            _kkm.SaleReturn((decimal)sum, 1, "");
            _kkm.CloseCheck((decimal)sum, 1, "");

            WaitForPrintingToComplete();

            SellItem(SalesKind.ReturnComission, sum * (commission / 100.0));
        }

        private void WaitForPrintingToComplete()
        {
            KKMMode mode = _kkm.GetKKMMode();
            System.Diagnostics.Debug.WriteLine(mode.ToString());
            while (mode == KKMMode.PrinterWorking || mode == KKMMode.DocumentOpened)
            {
                System.Diagnostics.Debug.WriteLine(mode.ToString());
                System.Threading.Thread.Sleep(100);
                mode = _kkm.GetKKMMode();
            }
            System.Diagnostics.Debug.WriteLine(string.Format("finished waiting. Mode: {0}",mode));
        }

        public void CloseSession()
        {
            _kkm.CloseSession();
        }

        public void Dispose()
        {
            _kkm.Disconnect();
        }

        public void Reset()
        {
            _kkm.ResetKKM();
        }
    }
}
