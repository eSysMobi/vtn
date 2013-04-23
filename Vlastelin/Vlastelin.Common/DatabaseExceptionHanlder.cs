using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

namespace Vlastelin.Common
{
    [Serializable]
    public class DBError
    {
        [XmlElement]
        public int code;
        [XmlElement]
        public string description;
    }

    public class DataBaseException : Exception
    {
        public DataBaseException()
            : base()
        {
        }

        public DataBaseException(String message)
            : base(message)
        {
        }
    }

    public class DatabaseExceptionHanlder : IDatabaseErrorHandler
    {
        private Dictionary<int, string> errorsDic;

        public DatabaseExceptionHanlder()
        {
            // загружаем список ошибок из файла
            XmlSerializer s = new XmlSerializer(typeof(List<DBError>));
            string fileName = System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location) + @"\DBErrors.xml";
            System.IO.StreamReader sr = new System.IO.StreamReader(fileName);
            List<DBError> errors = s.Deserialize(sr) as List<DBError>;

            // пихаем их в Dictionary
            errorsDic = new Dictionary<int, string>();
            errors.ForEach(err => errorsDic.Add(err.code, err.description));
        }

        #region IDatabaseErrorHandler Members

        public void RaiseException(int errorCode, string extendedMessage)
        {
            if (errorCode != 0)
            {
                if (errorsDic.ContainsKey(errorCode))
                    throw new DataBaseException(errorsDic[errorCode]);
                else
                    throw new DataBaseException(extendedMessage);
            }
        }

        #endregion
    }
}
