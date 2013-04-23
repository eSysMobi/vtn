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
    public class TripPriceMaterializator
        : IMaterializator<TripPrice>
    {
        public TripPrice Materialize(DataReaderAdapter dataReader)
        {
            throw new NotImplementedException();
        }

        public List<TripPrice> Materialize_List(DataReaderAdapter dataReader)
        {
            List<TripPrice> tPrices = new List<TripPrice>();

            while (dataReader.Read())
            {
                TripPrice tp = dataReader.ReadObject<TripPrice>();
                tPrices.Add(tp);
            }

            dataReader.NextResult();
            while (dataReader.Read())
            {
                Town t = dataReader.ReadObject<Town>();

                tPrices
                    .Where(tp => tp._departureId == t.Id)
                    .ToList()
                    .ForEach(tpp => tpp.Departure = t);
                tPrices
                    .Where(tp => tp._arrivalId == t.Id)
                    .ToList()
                    .ForEach(tpp => tpp.Arrival = t);
            }

            return tPrices;
        }
    }

    public class TripPriceDAO
        : ItemDAO<TripPrice, TripPriceMaterializator>
    {
        #region .ctor & instance
        protected TripPriceDAO()
            : base(DatabaseConstants.DatabaseAlias, new DatabaseExceptionHanlder())
        {
        }

        public static new TripPriceDAO Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new TripPriceDAO();
                }

                return (TripPriceDAO)_instance;
            }
        }
        #endregion

        public List<TripPrice> TripPricesGet(Trip trip)
        {
            List<MySqlParameter> parameters = new List<MySqlParameter>();
            parameters.AddErrorOutputParameter();
            parameters.AddInputParameter("tId", DbType.Int32, trip == null ? -1 : trip.Id);
            return this.Execute_GetList(CommandType.StoredProcedure, "sp_tprices_get_bytrip", parameters);
        }

        public void TripPriceEdit(TripPrice tp)
        {
            List<MySqlParameter> parameters = new List<MySqlParameter>();
            parameters.AddErrorOutputParameter();
            parameters.AddInputParameter("tpId", DbType.Int32, tp.Id);
            parameters.AddInputParameter("arr", DbType.Int32, tp._arrivalId);
            parameters.AddInputParameter("dep", DbType.Int32, tp._departureId);
            parameters.AddInputParameter("prc", DbType.Double, tp.Price);
            this.Execute_StoredProcedure(CommandType.StoredProcedure, "sp_tprice_edit", parameters);

            TablesTimeDAO.Instance.SetLastModifiedTime(ModifiedObjects.Trips);
        }

        public void TripPriceSave(List<TripPrice> tPrices)
        {
            if (tPrices == null)
                throw new ArgumentNullException("tPrices");
            if (tPrices.Count == 0)
                throw new ArgumentNullException("tPrices.Count=0");

            StringBuilder tpSB = new StringBuilder();
            tPrices.ForEach(t => tpSB.AppendFormat("{0} {1} {2} {3} |", t.Id, t._arrivalId, t._departureId, t.Price));

            List<MySqlParameter> parameters = new List<MySqlParameter>();
            parameters.AddErrorOutputParameter();
            parameters.AddInputParameter("tpList", DbType.String, tpSB.ToString());
            this.Execute_StoredProcedure(CommandType.StoredProcedure, "sp_tprice_save", parameters);

            TablesTimeDAO.Instance.SetLastModifiedTime(ModifiedObjects.Trips);
        }
    }
}
