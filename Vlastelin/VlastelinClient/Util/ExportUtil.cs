using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using Vlastelin.Data.Model.ExportedData;
using System.Xml.Serialization;
using System.Xml;
using System.Xml.Xsl;


namespace VlastelinClient.Util
{
    public class ExportUtil
    {
        public static string GetExportegRKOFile(List<ExportedRKO> list)
        {
            string result = "";
            using (MemoryStream msIN = new MemoryStream())
            {
                StreamWriter sw = new StreamWriter(msIN);

                XmlSerializer ser = new XmlSerializer(typeof(ExportedRKO[]));
                ser.Serialize(sw, list.ToArray());

                msIN.Seek(0, SeekOrigin.Begin);

                XmlReader xrd = XmlReader.Create(msIN);
                MemoryStream msOUT = new MemoryStream();
                StreamWriter sr2 = new StreamWriter(msOUT);
                XslCompiledTransform trans = new XslCompiledTransform();
                trans.Load(@"xslt\RKOExport.xslt");
                trans.Transform(xrd, null, sr2);

                msOUT.Seek(0, SeekOrigin.Begin);
                StreamReader sr = new StreamReader(msOUT);

                result = sr.ReadToEnd();
                sw.Close();
                sr.Close();
            }            
            return result;
        }
    }
}
