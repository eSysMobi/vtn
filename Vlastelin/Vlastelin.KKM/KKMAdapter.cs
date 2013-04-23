using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Vlastelin.Common;
using System.Drawing;
using System.Threading;

namespace Vlastelin.KKM
{
    /// <summary>
    /// Адаптер для методов ККМ, предоставляемых драйвером ККМ
    /// </summary>
    internal class KKMAdapter
    {
        public enum MoneyRegisters
        {
            SessionIncome = 242,
            SessionOutcome = 243,

            SessionSellEKLZ = 245,
            SessionBuyEKLZ = 246,
            SessionSellReturnEKLZ = 247,
            SessionBuyReturnEKLZ = 248,

            TotalMoneyAmountBeforeFiscalization=244,

            SessionSellReturn = 195,
            SessionSell = 193,
        };

        public enum OpRegisters
        {
            LastDocNum = 152,
            LastZReportNum=159,

            SessionBuyCheckCount = 145,
            SessionSellCheckCount = 144,
            SessionBuyReturnCount = 143,
            SessionSellReturnCount = 142,
            LastCheckBuy = 149,
            LastCheckSell = 148,
            LastCheckSellReturn = 150,
            LastCheckBuyReturn = 151
        };

        private Dictionary<int, string> models;

        private DrvFRLib.DrvFR _device;

        internal KKMAdapter()
        {
            _device = new DrvFRLib.DrvFR();
            
            // задаем пароль оператора на всякий случай.
            _device.Password = 30;
            
            

            // заполняем справочник моделей
            models = new Dictionary<int, string>();
            models.Add(0, "ШТРИХ-ФР-Ф");
            models.Add(1, "ШТРИХ-ФР-Ф (КАЗ)");
            models.Add(2, "ЭЛВЕС-МИНИ-ФР-Ф");
            models.Add(3, "ФЕЛИКС-РФ");
            models.Add(4, "ШТРИХ-ФР-К");
            models.Add(5, "ШТРИХ-950К");
            models.Add(6, "ЭЛВЕС-ФР-К");
            models.Add(7, "ШТРИХ-МИНИ-ФР-К");
            models.Add(8, "ШТРИХ-ФР-Ф (Белоруссия)");
            models.Add(9, "ШТРИХ-КОМБО-ФР-К");
            models.Add(10, "Фискальный блок Штрих-POS-Ф");
            models.Add(11, "ШТРИХ-950К (версия 02)");
            models.Add(12, "ШТРИХ-КОМБО-ФР-К (версия 02)");
            models.Add(14, "ШТРИХ-МИНИ-ФР-К (версия 02, 57 мм)");
            models.Add(15, "ШТРИХ-КИОСК-ФР-К");
            models.Add(17, "NCR-001К");
            models.Add(19, "ШТРИХ-MOBILE-ПТК");
            models.Add(239, "ШТРИХ-М 200");
            models.Add(243, "ЯРУС-01К");
            models.Add(244, "ШТРИХ-КИОСК-ФР-К");
            models.Add(248, "ЯРУС-02К");
            models.Add(250, "ШТРИХ-М-ФР-К");
            models.Add(252, "ШТРИХ-LIGHT-ФР-К");
        }

        public void WriteLine(string str)
        {
            _device.StringForPrinting = str;
            _device.PrintString();
        }

        public void WriteLine57(string str)
        {
            _device.FontType = 6;
            _device.StringForPrinting = str;
            _device.PrintStringWithFont();
        }

        public string GetModel()
        {
            int i = _device.GetDeviceMetrics();
            if (i != 0)
                throw new Exception(string.Format("Ошибка при вызове метода ККМ: {0}", _device.ResultCodeDescription));
            string desc = _device.UDescription;
            int modelID = _device.UModel;
            return models[modelID];
        }

        public string GetSerialNum()
        {
            if(string.IsNullOrWhiteSpace(_device.SerialNumber))
            {
                _device.Password = 30;
                int i = _device.GetECRStatus();
                if (i != 0)
                    throw new Exception(string.Format("Ошибка при вызове метода ККМ: {0}", _device.ResultCodeDescription));
            }
            return _device.SerialNumber;
        }

        public string GetRegistrationNum()
        {
            if (string.IsNullOrWhiteSpace(_device.RNM))
            {
                _device.Password = 30;
                int i = _device.GetECRStatus();
                if (i != 0)
                    throw new Exception(string.Format("Ошибка при вызове метода ККМ: {0}", _device.ResultCodeDescription));
            }
            return _device.RNM;
        }

        public long GetControlCounterValue()
        {
            return _device.JournalRowCount;
        }

        public decimal GetTotalRevenue()
        {
            _device.Password = 30;
            _device.TypeOfSumOfEntriesFM = true;
            int i = _device.GetFMRecordsSum();
            if (i != 0)
                throw new Exception(string.Format("Ошибка при вызове метода ККМ: {0}", _device.ResultCodeDescription));

            return _device.Summ1;
        }

        public decimal GetTotalReturn()
        {
            _device.Password = 30;
            _device.TypeOfSumOfEntriesFM = true;
            int i = _device.GetFMRecordsSum();
            if (i != 0)
                throw new Exception(string.Format("Ошибка при вызове метода ККМ: {0}", _device.ResultCodeDescription));

            return _device.Summ3;
        }

        internal KKMMode GetKKMMode()
        {
            _device.Password = 30;
            int res = _device.GetECRStatus();
            if (res != 0)
                throw new Exception(string.Format("Ошибка при вызове метода ККМ: {0}", _device.ResultCodeDescription));
            return (KKMMode)_device.ECRMode;
        }

        internal string GetKKMModeString()
        {
            _device.Password = 30;
            int res = _device.GetECRStatus();
            if (res != 0)
                throw new Exception(string.Format("Ошибка при вызове метода ККМ: {0}", _device.ResultCodeDescription));
            return _device.ECRModeDescription;
        }

        internal void Connect(int COMPortNumber)
        {
            _device.Password = 30;
            _device.ComNumber = COMPortNumber;
            _device.BaudRate = 6;
            
            int res=_device.Connect2();
            if (res != 0)
                throw new Exception(string.Format("Ошибка при вызове метода ККМ: {0}", _device.ResultCodeDescription));
        }

        internal void Connect()
        {
            _device.Password = 30;
            _device.ComputerName = Environment.MachineName;
            _device.TimeoutsUsing = 100;

            int res = _device.FindDevice();
            if (res != 0)
                throw new Exception(string.Format("Ошибка при вызове метода ККМ: {0}", _device.ResultCodeDescription));

            res = _device.Connect2();
            if (res != 0)
                throw new Exception(string.Format("Ошибка при вызове метода ККМ: {0}", _device.ResultCodeDescription));
        }

        internal void Disconnect()
        {
            int res = _device.Disconnect();
            if (res != 0)
                throw new Exception(string.Format("Ошибка при вызове метода ККМ: {0}", _device.ResultCodeDescription));
        }

        internal void OpenCheck(CheckType type)
        {
            _device.Password = 30;
            _device.CheckType = (int)type;
            int res = _device.OpenCheck();
            if (res != 0)
                throw new Exception(string.Format("Ошибка при вызове метода ККМ: {0}", _device.ResultCodeDescription));
        }

        internal void SaleOneItem(decimal price, int tax1, string itemName)
        {
            _device.Password = 30;
            
            _device.Tax1 = tax1;
            _device.Tax2 = 0;
            _device.Tax3 = 0;
            _device.Tax4 = 0;

            _device.Quantity = 1;
            _device.Price = price;
            _device.Department = 1;
            _device.StringForPrinting = itemName;

            int res = _device.Sale();
            if (res != 0)
                throw new Exception(string.Format("Ошибка при вызове метода ККМ: {0}", _device.ResultCodeDescription));
        }

        internal void CloseCheck(decimal sum, int tax1,string msg)
        {
            _device.Password = 30;

            _device.Summ1 = sum;
            _device.Summ2 = 0;
            _device.Summ3 = 0;
            _device.Summ4 = 0;

            _device.Tax1 = tax1;
            _device.Tax2 = 0;
            _device.Tax3 = 0;
            _device.Tax4 = 0;

            _device.StringForPrinting = msg;

            int res = _device.CloseCheck();
            if (res != 0)
                throw new Exception(string.Format("Ошибка при вызове метода ККМ: {0}", _device.ResultCodeDescription));
        }

        internal void CloseSession()
        {
            _device.Password = 30;

            int res = _device.PrintReportWithCleaning();
            if (res != 0)
                throw new Exception(string.Format("Ошибка при вызове метода ККМ: {0}", _device.ResultCodeDescription));
        }

        internal void CashIncome(double sum)
        {
            _device.Password = 30;
            _device.Summ1 = (decimal)sum;

            int res = _device.CashIncome();
            if (res != 0)
                throw new Exception(string.Format("Ошибка при вызове метода ККМ: {0}", _device.ResultCodeDescription));
        }

        internal void CashOutcome(double sum)
        {
            _device.Password = 30;
            _device.Summ1 = (decimal)sum;

            int res = _device.CashOutcome();
            if (res != 0)
                throw new Exception(string.Format("Ошибка при вызове метода ККМ: {0}", _device.ResultCodeDescription));
        }

        internal string[] ImageToLineData(Bitmap bmp, bool centerImage)
        {
            List<string> retVal = new List<string>();
            // линия
            List<byte> _40bytes = new List<byte>();

            for (int h = 0; h < bmp.Height; h++)
            {
                _40bytes.Clear();
                for (int w = 0; (w+8) < bmp.Width; w += 8)
                {
                    // хитро получаем байт цвета, в терминах ККМ
                    // картинка у нас монохромная, порог в 150 взят от балды
                    // если байт цвета у нас больеш 150 - считаем его черным и кодируем битом 1
                    // иначе кодируем битом 0
                    // таким образом получаем байт с цветами 8ми монохромных пикселов
                    int b =
                        ((bmp.GetPixel(w + 7, h).ToArgb() != -1) ? 128 : 0) +   // 10000000
                        ((bmp.GetPixel(w + 6, h).ToArgb() != -1) ? 64 : 0) +    // 01000000
                        ((bmp.GetPixel(w + 5, h).ToArgb() != -1) ? 32 : 0) +    // 00100000
                        ((bmp.GetPixel(w + 4, h).ToArgb() != -1) ? 16 : 0) +    // 00010000
                        ((bmp.GetPixel(w + 3, h).ToArgb() != -1) ? 8 : 0) +     // 00001000
                        ((bmp.GetPixel(w + 2, h).ToArgb() != -1) ? 4 : 0) +     // 00000100
                        ((bmp.GetPixel(w + 1, h).ToArgb() != -1) ? 2 : 0) +     // 00000010
                        ((bmp.GetPixel(w + 0, h).ToArgb() != -1) ? 1 : 0);      // 00000001
                    _40bytes.Add((byte)b);
                } //  for w
                // если нужно центрировать картинку
                if (centerImage && _40bytes.Count < 40)
                {
                    int paddingLen = (40 - _40bytes.Count) / 2;
                    List<byte> padding = new List<byte>();
                    for (int i = 0; i < paddingLen; i++) padding.Add(0);
                    _40bytes.InsertRange(0, padding);
                    for (int i = _40bytes.Count; i < 40; i++)
                        _40bytes.Add(0);
                } // if centerImage
                else // иначе дополняем до 40 байт
                    for (int i = _40bytes.Count; i < 40; i++)
                        _40bytes.Add(0);

                StringBuilder sb = new StringBuilder();
                _40bytes.ForEach(b => sb.Append(String.Format("{0:x2} ", b)));

                retVal.Add(sb.ToString().Trim());
            }// for h

            return retVal.ToArray();
        }

        internal void LoadAndPrintImage(Bitmap im, bool centerImage)
        {
            // файл нужно открыть и преобразовать в LineData
            int lineNum = 0;
            foreach (string s in ImageToLineData(im, centerImage))
            {
                _device.Password = 30;
                _device.LineNumber = lineNum++;
                _device.LineDataHex = s;
                _device.CenterImage = centerImage;

                int res = _device.LoadLineData();
                if (res != 0)
                    throw new Exception(string.Format("Ошибка при вызове метода ККМ: {0}", _device.ResultCodeDescription));
            }
            PrintLastImage();
        }

        internal void PrintLastImage()
        {
            _device.Password = 30;
            _device.FirstLineNumber = 1;
            _device.LastLineNumber = 120;
            int res = _device.Draw();
            if (res != 0)
                throw new Exception(string.Format("Ошибка при вызове метода ККМ: {0}", _device.ResultCodeDescription));
        }

        internal int GetOperationRegister(OpRegisters register)
        {
            _device.Password = 30;
            _device.RegisterNumber = (int)register;
            int res = _device.GetOperationReg();
            if (res != 0)
                throw new Exception(string.Format("Ошибка при вызове метода ККМ: {0}", _device.ResultCodeDescription));

            return _device.ContentsOfOperationRegister;
        }
        internal decimal GetMoneyRegister(MoneyRegisters register)
        {
            _device.Password = 30;
            _device.RegisterNumber = (int)register;
            int res = _device.GetCashReg();
            if (res != 0)
                throw new Exception(string.Format("Ошибка при вызове метода ККМ: {0}", _device.ResultCodeDescription));

            return _device.ContentsOfCashRegister;
        }

        internal void ShowTablesDlg(IntPtr HWNDParent)
        {
            _device.ParentWnd = HWNDParent.ToInt32();
            int res = _device.ShowTablesDlg();
            if (res != 0)
                throw new Exception(string.Format("Ошибка при вызове метода ККМ: {0}", _device.ResultCodeDescription));
        }

        internal void ResetTables()
        {
            _device.Password = 30;
            int res = _device.InitTable();
            if (res != 0)
                throw new Exception(string.Format("Ошибка при вызове метода ККМ: {0}", _device.ResultCodeDescription));
        }

        internal int ReadTableIntValue(int TableNum, int RowNumber, int FieldNumber)
        {
            _device.Password = 30;
            _device.TableNumber = TableNum;
            _device.RowNumber = RowNumber;
            _device.FieldNumber = FieldNumber;

            int res = _device.ReadTable();
            if (res != 0)
                throw new Exception(string.Format("Ошибка при вызове метода ККМ: {0}", _device.ResultCodeDescription));

            return _device.ValueOfFieldInteger;
        }

        internal string ReadTableStringValue(int TableNum, int RowNumber, int FieldNumber)
        {
            _device.Password = 30;
            _device.TableNumber = TableNum;
            _device.RowNumber = RowNumber;
            _device.FieldNumber = FieldNumber;

            int res = _device.ReadTable();
            if (res != 0)
                throw new Exception(string.Format("Ошибка при вызове метода ККМ: {0}", _device.ResultCodeDescription));

            return _device.ValueOfFieldString;
        }

        internal void WriteTableValue(int TableNum, int RowNumber, int FieldNumber, int Value)
        {
            _device.Password = 30;
            _device.TableNumber = TableNum;
            _device.RowNumber = RowNumber;
            _device.FieldNumber = FieldNumber;
            _device.ValueOfFieldInteger = Value;

            int res = _device.WriteTable();
            if (res != 0)
                throw new Exception(string.Format("Ошибка при вызове метода ККМ: {0}", _device.ResultCodeDescription));
        }
        internal void WriteTableValue(int TableNum, int RowNumber, int FieldNumber, string Value)
        {
            _device.Password = 30;
            _device.TableNumber = TableNum;
            _device.RowNumber = RowNumber;
            _device.FieldNumber = FieldNumber;
            _device.ValueOfFieldString = Value;

            int res = _device.WriteTable();
            if (res != 0)
                throw new Exception(string.Format("Ошибка при вызове метода ККМ: {0}", _device.ResultCodeDescription));
        }

        internal void SessionReport()
        {
            _device.Password = 30;
            int res = _device.PrintReportWithoutCleaning();
            if (res != 0)
                throw new Exception(string.Format("Ошибка при вызове метода ККМ: {0}", _device.ResultCodeDescription));
        }

        internal void SaleReturn(decimal price, int tax1, string msg)
        {
            _device.Password = 30;
            _device.Quantity = 1;
            _device.Price = price;
            _device.Tax1 = tax1;
            _device.Tax2 = 0;
            _device.Tax3 = 0;
            _device.Tax4 = 0;
            _device.StringForPrinting = msg;
            _device.Department = 1;

            int res = _device.ReturnSale();
            if (res != 0)
                throw new Exception(string.Format("Ошибка при вызове метода ККМ: {0}", _device.ResultCodeDescription));
        }

        internal DateTime GetTime()
        {
            _device.Password = 30;
            int res = _device.GetECRStatus();
            if (res != 0)
                throw new Exception(string.Format("Ошибка при вызове метода ККМ: {0}", _device.ResultCodeDescription));

            return new DateTime(_device.Date.Year, _device.Date.Month, _device.Date.Day, _device.Time.Hour, _device.Time.Minute, _device.Time.Second);
        }

        internal void SetTime(DateTime value)
        {
            _device.Password = 30;
            _device.Time = value;
            int res = _device.SetTime();
            if (res != 0)
                throw new Exception(string.Format("Ошибка при вызове метода ККМ: {0}", _device.ResultCodeDescription));

            _device.Password = 30;
            _device.Date = value.Date;
            res = _device.SetDate();
            if (res != 0)
                throw new Exception(string.Format("Ошибка при вызове метода ККМ: {0}", _device.ResultCodeDescription));

            _device.Password = 30;
            _device.Date = value.Date;
            res = _device.ConfirmDate();
            if (res != 0)
                throw new Exception(string.Format("Ошибка при вызове метода ККМ: {0}", _device.ResultCodeDescription));
        }

        internal void ResetKKM()
        {
            _device.Password = 30;
            int res = _device.ResetECR();
            if (res != 0)
                throw new Exception(string.Format("Ошибка при вызове метода ККМ: {0}", _device.ResultCodeDescription));
        }
    }
}
