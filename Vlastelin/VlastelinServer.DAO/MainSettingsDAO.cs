using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Vlastelin.Data.Model;
using VlastelinServer.DAO.DBAccess;
using Vlastelin.Common.Constants;
using Vlastelin.Common;
using MySql.Data.MySqlClient;
using System.Data;

namespace VlastelinServer.DAO
{
    public class MainSettingsMaterializator
       : MaterializatorBase<MainSettings>
    {
        public override MainSettings ReadSingleObject(DataReaderAdapter dataReader)
        {
            MainSettings ret = dataReader.ReadObject<MainSettings>();
            return ret;
        }
    }

    public class MainSettingsDAO
        : ItemDAO<MainSettings, MainSettingsMaterializator>
    {
        #region .ctor & instance
        protected MainSettingsDAO()
            : base(DatabaseConstants.DatabaseAlias, new DatabaseExceptionHanlder())
        {
        }

        public static new MainSettingsDAO Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new MainSettingsDAO();
                }

                return (MainSettingsDAO)_instance;
            }
        }
        #endregion

        public MainSettings MainSettingsGet()
        {
            string sql = "select * from MainSettings where id=1";
            List<MySqlParameter> parameters = new List<MySqlParameter>();
            parameters.AddErrorOutputParameter();
            return this.Execute_GetList(CommandType.Text, sql, parameters)[0];
        }

        public void MainSettingsEdit(MainSettings st)
        {
            string sql = "update MainSettings set " +
                "OrganizationName=@oName," +
                "OrganizationDirName=@oDName," +
                "OrganizationDirSurname=@oDSurname," +
                "OrganizationDirPatronymic=@oDPatr," +
                "OrganizationINN=@oINN," +
                "OrganizationKPP=@oKPP," +
                "OrganizationCorrAccount=@oKS, " +
                "ReturnedCommission=@rk " +
                "where id=1";

            List<MySqlParameter> parameters = new List<MySqlParameter>();
            parameters.AddErrorOutputParameter();
            parameters.AddInputParameter("oName", DbType.String, st.OrganizationName);
            parameters.AddInputParameter("oDName", DbType.String, st.OrganizationDirName);
            parameters.AddInputParameter("oDSurname", DbType.String, st.OrganizationDirSurname);
            parameters.AddInputParameter("oDPatr", DbType.String, st.OrganizationDirPatronymic);
            parameters.AddInputParameter("oINN", DbType.String, st.OrganizationINN);
            parameters.AddInputParameter("oKPP", DbType.String, st.OrganizationKPP);
            parameters.AddInputParameter("oKS", DbType.String, st.OrganizationCorrAccount);
            parameters.AddInputParameter("rk", DbType.Double, st.ReturnedCommission);

            this.Execute_StoredProcedure(CommandType.Text, sql, parameters);
            TablesTimeDAO.Instance.SetLastModifiedTime(ModifiedObjects.MainSettings);
        }
    }
}
