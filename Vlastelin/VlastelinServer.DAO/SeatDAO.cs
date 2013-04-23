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
    public class SeatMaterializator
        : MaterializatorBase<Seat>
    {

        public override Seat ReadSingleObject(DataReaderAdapter dataReader)
        {
            throw new NotImplementedException();
        }

        public override List<Seat> Materialize_List(DataReaderAdapter dataReader)
        {
            List<Seat> seats = new List<Seat>();

            while (dataReader.Read())
            {
                Seat seat = dataReader.ReadObject<Seat>();   
                seats.Add(seat);
            }

            dataReader.NextResult();
            while (dataReader.Read())
            {
                StationSchedule ss = dataReader.ReadObject<StationSchedule>();
                seats
                    .Where(s => s.SSid == ss.Id)
                    .ToList()
                    .ForEach(sss => sss.SS = ss);
            }

            dataReader.NextResult();
            while (dataReader.Read())
            {
                TripSchedule ts = dataReader.ReadObject<TripSchedule>();
                seats
                    .Where(s => s.SS._tsId == ts.Id)
                    .ToList()
                    .ForEach(sss => sss.SS.TS = ts);
            }
            dataReader.NextResult();
            while (dataReader.Read())
            {
                TripPrice tp = dataReader.ReadObject<TripPrice>();
                seats
                    .Where(s => s.TripPriceId == tp.Id)
                    .ToList()
                    .ForEach(sss => sss.TripPrice = tp);
            }
            dataReader.NextResult();
            while (dataReader.Read())
            {
                Trip t = dataReader.ReadObject<Trip>();
                seats
                    .Where(s => s.SS.TS.tripId == t.Id)
                    .ToList()
                    .ForEach(sss => sss.SS.TS.Trip = t);
            }

            dataReader.NextResult();
            while (dataReader.Read())
            {
                Town t = dataReader.ReadObject<Town>();

                seats
                    .Where(s => s.SS.TS.Trip._departureId == t.Id)
                    .ToList()
                    .ForEach(sss => sss.SS.TS.Trip.Departure = t);
                seats
                    .Where(ss => ss.SS.TS.Trip._arrivalId == t.Id)
                    .ToList()
                    .ForEach(sss => sss.SS.TS.Trip.Arrival = t);
                seats
                    .Where(ss => ss.TripPrice._arrivalId == t.Id)
                    .ToList()
                    .ForEach(sss => sss.TripPrice.Arrival = t);
                seats
                    .Where(ss => ss.TripPrice._departureId == t.Id)
                    .ToList()
                    .ForEach(sss => sss.TripPrice.Departure = t);

                seats
                    .Where(s => s.SS._tId == t.Id)
                    .ToList()
                    .ForEach(sss => sss.SS.Town = t);
            }

            dataReader.NextResult();
            while (dataReader.Read())
            {
                Bus b = dataReader.ReadObject<Bus>();
                seats
                    .Where(s => s.SS.TS.BusId == b.Id)
                    .ToList()
                    .ForEach(sss => sss.SS.TS.Bus = b);
            }

            dataReader.NextResult();
            while (dataReader.Read())
            {
                Owner o = dataReader.ReadObject<Owner>();
                seats
                    .Where(s => s.SS.TS.Bus.OwnerId == o.Id)
                    .ToList()
                    .ForEach(sss => sss.SS.TS.Bus.Owner = o);
            }
            dataReader.NextResult();
            while (dataReader.Read())
            {
                DirPosition dp = dataReader.ReadObject<DirPosition>();
                seats
                    .Where(s => s.SS.TS.Bus.Owner._dirPosition == dp.Id)
                    .ToList()
                    .ForEach(sss => sss.SS.TS.Bus.Owner.DirPosition = dp);
            }
            dataReader.NextResult();
            while (dataReader.Read())
            {
                Passenger p = dataReader.ReadObject<Passenger>();
                seats
                    .Where(s => s.PassengerId == p.Id)
                    .ToList()
                    .ForEach(sss => sss.Passenger = p);
            }

            // passengers shouldn't be nulls?
           /* seats
                    .Where(s => s.Passenger == null)
                    .ToList()
                    .ForEach(sss => sss.Passenger = new Passenger());*/

            return seats;
        }
    }

    public class SeatDAO
        : ItemDAO<Seat, SeatMaterializator>
    {
        #region .ctor & instance
        protected SeatDAO()
			: base(DatabaseConstants.DatabaseAlias, new DatabaseExceptionHanlder())
		{
		}

        public static new SeatDAO Instance
		{
			get
			{
				if (_instance == null)
				{
                    _instance = new SeatDAO();
				}

                return (SeatDAO)_instance;
			}
		}
		#endregion

        public List<Tuple<Seat, double, string>> SeatsGet(StationSchedule ss, TripPrice tPrice, DateTime tripDate)
        {
            List<MySqlParameter> parameters = new List<MySqlParameter>();
            parameters.AddErrorOutputParameter();
            parameters.AddInputParameter("ss_Id", DbType.Int32, ss.Id);
            parameters.AddInputParameter("priceId", DbType.Int32, tPrice.Id);
            parameters.AddInputParameter("dt", DbType.DateTime, tripDate);
            List<Seat> lst = this.Execute_GetList(CommandType.StoredProcedure, "sp_seats_get", parameters);

            List<Tuple<Seat, double, string>> ret = new List<Tuple<Seat, double, string>>();

            if (lst.Count > 0)
            {
                StringBuilder sbList = new StringBuilder();
                lst.Select(s => s.Id).ToList().ForEach(s1s => sbList.AppendFormat("{0}|", s1s));

                parameters.Clear();
                parameters.AddErrorOutputParameter();
                parameters.AddInputParameter("sList", DbType.String, sbList.ToString());

                DataTable dt = this.Execute_GetDataTable(CommandType.StoredProcedure, "sp_sales_get_by_many_seats", parameters);

                List<Tuple<long, double, string>> tpl = new List<Tuple<long, double, string>>();
                foreach (DataRow row in dt.Rows)
                    tpl.Add(new Tuple<long, double, string>((long)(int)row["seatId"], (double)(float)row["sum"], (string)row["Operator"]));



                lst
                    .Select(s => s.Id)
                    .ToList()
                    .ForEach(sId =>
                    {
                        Seat st=lst.Find(sss => sss.Id == sId);
                        double d = st.State == SeatState.Sold ? tpl.Find(t => t.Item1 == sId).Item2 : 0;
                        string str = st.State == SeatState.Sold ? tpl.Find(t => t.Item1 == sId).Item3 : "";
                        ret.Add(new Tuple<Seat, double, string>(st, d, str));
                    }
                );
            }
            else
                lst.ForEach(s => ret.Add(new Tuple<Seat, double, string>(s, 0, "")));
            return ret;
        }

        public long SeatLock(int seatNum, int ssID, int tripPriceId, DateTime tripDate)
        {
            long seatId = 0;
            List<MySqlParameter> parameters = new List<MySqlParameter>();
            parameters.AddErrorOutputParameter();
            parameters.AddInputParameter("ss_Id", DbType.Int32, ssID);
            parameters.AddInputParameter("priceId", DbType.Int32, tripPriceId);
            parameters.AddInputParameter("seatNum", DbType.Int32, seatNum);
            parameters.AddInputParameter("dt", DbType.DateTime, tripDate);
            seatId = this.Execute_ScalarStoredProcedure<long>(CommandType.StoredProcedure, "sp_seat_lock", parameters);

            TablesTimeDAO.Instance.SetLastModifiedTime(ModifiedObjects.Seats);

            return seatId;
        }

        public void SeatUnlock(long seatNum, long ssID, long tripPriceId, DateTime tripDate)
        {
            //if (seat == null)
            //    throw new ArgumentNullException("seat");
            //if (seat.Id == 0)
            //    throw new ArgumentNullException("seat.id");

            List<MySqlParameter> parameters = new List<MySqlParameter>();
            parameters.AddErrorOutputParameter();
            parameters.AddInputParameter("ss_Id", DbType.Int32, ssID);
            parameters.AddInputParameter("priceId", DbType.Int32, tripPriceId);
            parameters.AddInputParameter("seatNum", DbType.Int32, seatNum);
            parameters.AddInputParameter("dt", DbType.DateTime, tripDate);
            this.Execute_StoredProcedure(CommandType.StoredProcedure, "sp_seat_unlock", parameters);
            TablesTimeDAO.Instance.SetLastModifiedTime(ModifiedObjects.Seats);
        }

        public void SeatSell(Seat seat, Passenger pass)
        {
            if (seat == null)
                throw new ArgumentNullException("seat");

            long pass_id = PassengerDAO.Instance.PassengerAdd(pass);
            pass.Id = pass_id;

            long seatId = 0;
            List<MySqlParameter> parameters = new List<MySqlParameter>();
            parameters.AddErrorOutputParameter();
            parameters.AddInputParameter("ss_Id", DbType.Int32, seat.SSid);
            parameters.AddInputParameter("seatNum", DbType.Int32, seat.SeatNumber);
            parameters.AddInputParameter("passengerId", DbType.Int32, pass_id);
            parameters.AddInputParameter("priceId", DbType.Int32, seat.TripPrice.Id);
            parameters.AddInputParameter("dt", DbType.DateTime, seat.TripDate);
            parameters.AddInputParameter("dest", DbType.String, seat.DesiredDestination);
            this.Execute_StoredProcedure(CommandType.StoredProcedure, "sp_seat_sell", parameters);
            TablesTimeDAO.Instance.SetLastModifiedTime(ModifiedObjects.Seats);
            if (seat.Id != 0 && seatId != 0)
                seat.Id = seatId;
        }

        public void SeatReserve(Seat seat)
        {
            if (seat == null)
                throw new ArgumentNullException("seat");

            long seatId = 0;
            List<MySqlParameter> parameters = new List<MySqlParameter>();
            parameters.AddErrorOutputParameter();
            parameters.AddInputParameter("ss_Id", DbType.Int32, seat.SS.Id);
            parameters.AddInputParameter("seatNum", DbType.Int32, seat.SeatNumber);
            parameters.AddInputParameter("priceId", DbType.Int32, seat.TripPrice.Id);
            parameters.AddInputParameter("dt", DbType.DateTime, seat.TripDate);

            seatId = this.Execute_ScalarStoredProcedure<long>(CommandType.StoredProcedure, "sp_seat_reserve", parameters);

            TablesTimeDAO.Instance.SetLastModifiedTime(ModifiedObjects.Seats);

            if (seat.Id == 0 && seatId != 0)
                seat.Id = seatId;
        }

        public void SeatReserveCancel(Seat seat)
        {
            if (seat == null)
                throw new ArgumentNullException("seat");

            List<MySqlParameter> parameters = new List<MySqlParameter>();
            parameters.AddErrorOutputParameter();
            parameters.AddInputParameter("sId", DbType.Int32, seat.Id);

            this.Execute_StoredProcedure(CommandType.StoredProcedure, "sp_seat_reserve_cancel", parameters);

            TablesTimeDAO.Instance.SetLastModifiedTime(ModifiedObjects.Seats);
        }

        public void SeatSellReturn(Seat seat)
        {
            if (seat == null)
                throw new ArgumentNullException("seat");

            List<MySqlParameter> parameters = new List<MySqlParameter>();
            parameters.AddErrorOutputParameter();
            parameters.AddInputParameter("sId", DbType.Int32, seat.Id);

            this.Execute_StoredProcedure(CommandType.StoredProcedure, "sp_seat_return", parameters);

            TablesTimeDAO.Instance.SetLastModifiedTime(ModifiedObjects.Seats);
        }
    }
}
