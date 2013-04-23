using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;

using VlastelinServer.DAO;
using Vlastelin.Common;
using System.ServiceModel.Channels;
using System.IdentityModel.Tokens;
using System.Configuration;
using System.Threading;
using System.Security.Permissions;
using Vlastelin.Data.Model;

namespace VlastelinServer
{
    public struct seatLockKey
    {
        public long seatNum, ssId, tpId;
        public DateTime dt;
        public seatLockKey(long seatNum, long ssID, long tripPriceId, DateTime tripDate)
        {
            this.seatNum = seatNum;
            this.ssId = ssID;
            this.tpId = tripPriceId;
            this.dt = tripDate;
        }
    }

    [ServiceBehavior(InstanceContextMode=InstanceContextMode.Single)]
    [ErrorBehavior(typeof(VlastelinErrorHandler))]
    public class VlastelinSrv : IVlastelinSrv
    {
        private Logger logger;
        private TimeSpan seatLockTimeout = TimeSpan.Zero;

        

        private static Dictionary<seatLockKey, Thread> waitingThreads
            = new Dictionary<seatLockKey, Thread>();

        public VlastelinSrv()
        {

            // считываем значение с конфига, если оно там есть
            if (ConfigurationManager.AppSettings.AllKeys.Contains("seatLockTimeout"))
                seatLockTimeout = TimeSpan.Parse(ConfigurationManager.AppSettings["seatLockTimeout"]);

            logger = new Logger();
        }
        [PrincipalPermission(SecurityAction.Demand, Role = "operators")]
        public string GetData(int value)
        {
            //ServiceSecurityContext.Current.PrimaryIdentity.Name;
            //throw new DivideByZeroException();
            return string.Format("You entered: {0}", value);
        }

        //[PrincipalPermission(SecurityAction.Demand, Role = "admins")]
        public int TestConnection()
        {
            try
            {
                TablesTimeDAO.Instance.GetLastModifiedTime(ModifiedObjects.TripsSchedule);
                return 0;
            }
            catch
            {
                return -1;
            }
        }

        public List<Vlastelin.Data.Model.Town> TownsGet(long? id)
        {
            if (OperationContext.Current != null)
            {
                RemoteEndpointMessageProperty endpoint = OperationContext.Current.IncomingMessageProperties[RemoteEndpointMessageProperty.Name] as RemoteEndpointMessageProperty;
                logger.LogMessage(LogEventType.Info, string.Format("Client {0} calling TownsGet({1})",
                    endpoint.Address,
                    id.ToString()));
            }
            return TownDAO.Instance.TownsGet(id);
        }
        [PrincipalPermission(SecurityAction.Demand, Role = "Administrator")]
        public long TownAdd(Vlastelin.Data.Model.Town town)
        {
            if (OperationContext.Current != null)
            {
                RemoteEndpointMessageProperty endpoint = OperationContext.Current.IncomingMessageProperties[RemoteEndpointMessageProperty.Name] as RemoteEndpointMessageProperty;
                logger.LogMessage(LogEventType.Info, string.Format("Client {0} calling TownsAdd('{1}')",
                    endpoint.Address,
                    town.Name));
            }
            return TownDAO.Instance.TownsAdd(town);
        }
        [PrincipalPermission(SecurityAction.Demand, Role = "Administrator")]
        public void TownDelete(Vlastelin.Data.Model.Town town)
        {
            if (OperationContext.Current != null)
            {
                RemoteEndpointMessageProperty endpoint = OperationContext.Current.IncomingMessageProperties[RemoteEndpointMessageProperty.Name] as RemoteEndpointMessageProperty;
                logger.LogMessage(LogEventType.Info, string.Format("Client {0} calling TownDelete('{1}')",
                    endpoint.Address,
                    town.Name));
            }
            TownDAO.Instance.TownsDelete(town);
        }
        [PrincipalPermission(SecurityAction.Demand, Role = "Administrator")]
        public void TownEdit(Vlastelin.Data.Model.Town town)
        {
            if (OperationContext.Current != null)
            {
                RemoteEndpointMessageProperty endpoint = OperationContext.Current.IncomingMessageProperties[RemoteEndpointMessageProperty.Name] as RemoteEndpointMessageProperty;
                logger.LogMessage(LogEventType.Info, string.Format("Client {0} calling TownEdit({1})",
                    endpoint.Address,
                    town.Id.ToString()));
            }
            TownDAO.Instance.TownsEdit(town);
        }


        public List<Vlastelin.Data.Model.Owner> OwnersGet(long? id)
        {
            if (OperationContext.Current != null)
            {
                RemoteEndpointMessageProperty endpoint = OperationContext.Current.IncomingMessageProperties[RemoteEndpointMessageProperty.Name] as RemoteEndpointMessageProperty;
                logger.LogMessage(LogEventType.Info, string.Format("Client {0} calling OwnersGet({1})",
                    endpoint.Address,
                    id.ToString()));
            }
            return OwnerDAO.Instance.OwnersGet(id);
        }
        [PrincipalPermission(SecurityAction.Demand, Role = "Administrator")]
        public long OwnerAdd(Vlastelin.Data.Model.Owner owner)
        {
            if (OperationContext.Current != null)
            {
                RemoteEndpointMessageProperty endpoint = OperationContext.Current.IncomingMessageProperties[RemoteEndpointMessageProperty.Name] as RemoteEndpointMessageProperty;
                logger.LogMessage(LogEventType.Info, string.Format("Client {0} calling OwnerAdd('{1}')",
                    endpoint.Address,
                    owner.Name));
            }
            return OwnerDAO.Instance.OwnerAdd(owner);
        }
        [PrincipalPermission(SecurityAction.Demand, Role = "Administrator")]
        public void OwnerDelete(Vlastelin.Data.Model.Owner owner)
        {
            if (OperationContext.Current != null)
            {
                RemoteEndpointMessageProperty endpoint = OperationContext.Current.IncomingMessageProperties[RemoteEndpointMessageProperty.Name] as RemoteEndpointMessageProperty;
                logger.LogMessage(LogEventType.Info, string.Format("Client {0} calling OwnerDelete('{1}')",
                    endpoint.Address,
                    owner.Name));
            }
            OwnerDAO.Instance.OwnerDelete(owner);
        }
        [PrincipalPermission(SecurityAction.Demand, Role = "Administrator")]
        public void OwnerEdit(Vlastelin.Data.Model.Owner owner)
        {
            if (OperationContext.Current != null)
            {
                RemoteEndpointMessageProperty endpoint = OperationContext.Current.IncomingMessageProperties[RemoteEndpointMessageProperty.Name] as RemoteEndpointMessageProperty;
                logger.LogMessage(LogEventType.Info, string.Format("Client {0} calling OwnerEdit({1})",
                    endpoint.Address,
                    owner.Id.ToString()));
            }
            OwnerDAO.Instance.OwnerEdit(owner);
        }


        public List<Vlastelin.Data.Model.Bus> BusesGet(long? id)
        {
            if (OperationContext.Current != null)
            {
                RemoteEndpointMessageProperty endpoint = OperationContext.Current.IncomingMessageProperties[RemoteEndpointMessageProperty.Name] as RemoteEndpointMessageProperty;
                logger.LogMessage(LogEventType.Info, string.Format("Client {0} calling BusesGet({1})",
                    endpoint.Address,
                    id.ToString()));
            }
            return BusDAO.Instance.BusesGet(id);
        }
        [PrincipalPermission(SecurityAction.Demand, Role = "Administrator")]
        public long BusAdd(Vlastelin.Data.Model.Bus bus)
        {
            if (OperationContext.Current != null)
            {
                RemoteEndpointMessageProperty endpoint = OperationContext.Current.IncomingMessageProperties[RemoteEndpointMessageProperty.Name] as RemoteEndpointMessageProperty;
                logger.LogMessage(LogEventType.Info, string.Format("Client {0} calling BusAdd('{1} {2}')",
                    endpoint.Address,
                    bus.Manufacter,
                    bus.Model));
            }
            return BusDAO.Instance.BusAdd(bus);
        }
        [PrincipalPermission(SecurityAction.Demand, Role = "Administrator")]
        public void BusDelete(Vlastelin.Data.Model.Bus bus)
        {
            if (OperationContext.Current != null)
            {
                RemoteEndpointMessageProperty endpoint = OperationContext.Current.IncomingMessageProperties[RemoteEndpointMessageProperty.Name] as RemoteEndpointMessageProperty;
                logger.LogMessage(LogEventType.Info, string.Format("Client {0} calling BusDelete('{1} {2}')",
                    endpoint.Address,
                    bus.Manufacter,
                    bus.Model));
            }
            BusDAO.Instance.BusDelete(bus);
        }
        [PrincipalPermission(SecurityAction.Demand, Role = "Administrator")]
        public void BusEdit(Vlastelin.Data.Model.Bus bus)
        {
            if (OperationContext.Current != null)
            {
                RemoteEndpointMessageProperty endpoint = OperationContext.Current.IncomingMessageProperties[RemoteEndpointMessageProperty.Name] as RemoteEndpointMessageProperty;
                logger.LogMessage(LogEventType.Info, string.Format("Client {0} calling BusEdit({1})",
                    endpoint.Address,
                    bus.Id.ToString()));
            }
            BusDAO.Instance.BusEdit(bus);
        }


        public List<Vlastelin.Data.Model.Trip> TripsGet(long? id)
        {
            if (OperationContext.Current != null)
            {
                RemoteEndpointMessageProperty endpoint = OperationContext.Current.IncomingMessageProperties[RemoteEndpointMessageProperty.Name] as RemoteEndpointMessageProperty;
                logger.LogMessage(LogEventType.Info, string.Format("Client {0} calling TripsGet({1})",
                    endpoint.Address,
                    id.ToString()));
            }
            List<Trip> lst = TripDAO.Instance.TripsGet(id);
            return lst;
        }
        [PrincipalPermission(SecurityAction.Demand, Role = "Administrator")]
        public long TripAdd(Vlastelin.Data.Model.Trip trip)
        {
            if (OperationContext.Current != null)
            {
                RemoteEndpointMessageProperty endpoint = OperationContext.Current.IncomingMessageProperties[RemoteEndpointMessageProperty.Name] as RemoteEndpointMessageProperty;
                logger.LogMessage(LogEventType.Info, string.Format("Client {0} calling TripAdd('{1}')",
                    endpoint.Address,
                    trip.Name));
            }
            
            long tripId = TripDAO.Instance.TripAdd(trip);
            return tripId;
        }
        [PrincipalPermission(SecurityAction.Demand, Role = "Administrator")]
        public void TripDelete(Vlastelin.Data.Model.Trip trip)
        {
            if (OperationContext.Current != null)
            {
                RemoteEndpointMessageProperty endpoint = OperationContext.Current.IncomingMessageProperties[RemoteEndpointMessageProperty.Name] as RemoteEndpointMessageProperty;
                logger.LogMessage(LogEventType.Info, string.Format("Client {0} calling TripDelete('{1}')",
                    endpoint.Address,
                    trip.Name));
            }
            TripDAO.Instance.TripDelete(trip);
        }
        [PrincipalPermission(SecurityAction.Demand, Role = "Administrator")]
        public void TripEdit(Vlastelin.Data.Model.Trip trip)
        {
            if (OperationContext.Current != null)
            {
                RemoteEndpointMessageProperty endpoint = OperationContext.Current.IncomingMessageProperties[RemoteEndpointMessageProperty.Name] as RemoteEndpointMessageProperty;
                logger.LogMessage(LogEventType.Info, string.Format("Client {0} calling TripEdit({1})",
                    endpoint.Address,
                    trip.Id.ToString()));
            }
            TripDAO.Instance.TripEdit(trip);
        }


        public List<Vlastelin.Data.Model.TripSchedule> TripScheduleGet(long? id)
        {
            if (OperationContext.Current != null)
            {
                RemoteEndpointMessageProperty endpoint = OperationContext.Current.IncomingMessageProperties[RemoteEndpointMessageProperty.Name] as RemoteEndpointMessageProperty;
                logger.LogMessage(LogEventType.Info, string.Format("Client {0} calling TripScheduleGet({1})",
                    endpoint.Address,
                    id.ToString()));
            }
            return TripScheduleDAO.Instance.TripSchedulesGet(id);
        }
        [PrincipalPermission(SecurityAction.Demand, Role = "Administrator")]
        public long TripScheduleAdd(Vlastelin.Data.Model.TripSchedule ts)
        {
            if (OperationContext.Current != null)
            {
                RemoteEndpointMessageProperty endpoint = OperationContext.Current.IncomingMessageProperties[RemoteEndpointMessageProperty.Name] as RemoteEndpointMessageProperty;
                logger.LogMessage(LogEventType.Info, string.Format("Client {0} calling TripScheduleAdd()",
                    endpoint.Address));
            }
            return TripScheduleDAO.Instance.TripScheduleAdd(ts);
        }
        [PrincipalPermission(SecurityAction.Demand, Role = "Administrator")]
        public void TripScheduleDelete(Vlastelin.Data.Model.TripSchedule ts)
        {
            if (OperationContext.Current != null)
            {
                RemoteEndpointMessageProperty endpoint = OperationContext.Current.IncomingMessageProperties[RemoteEndpointMessageProperty.Name] as RemoteEndpointMessageProperty;
                logger.LogMessage(LogEventType.Info, string.Format("Client {0} calling TripScheduleDelete('{1}')",
                    endpoint.Address,
                    ts.Id));
            }
            TripScheduleDAO.Instance.TripScheduleDelete(ts);
        }
        [PrincipalPermission(SecurityAction.Demand, Role = "Administrator")]
        public void TripScheduleEdit(Vlastelin.Data.Model.TripSchedule ts)
        {
            if (OperationContext.Current != null)
            {
                RemoteEndpointMessageProperty endpoint = OperationContext.Current.IncomingMessageProperties[RemoteEndpointMessageProperty.Name] as RemoteEndpointMessageProperty;
                logger.LogMessage(LogEventType.Info, string.Format("Client {0} calling TripScheduleEdit({1})",
                    endpoint.Address,
                    ts.Id.ToString()));
            }
            TripScheduleDAO.Instance.TripScheduleEdit(ts);
        }
        [PrincipalPermission(SecurityAction.Demand, Role = "Administrator")]
        public void TripScheduleSave(TripSchedule ts, List<StationSchedule> ss)
        {
            if (OperationContext.Current != null)
            {
                RemoteEndpointMessageProperty endpoint = OperationContext.Current.IncomingMessageProperties[RemoteEndpointMessageProperty.Name] as RemoteEndpointMessageProperty;
                logger.LogMessage(LogEventType.Info, string.Format("Client {0} calling TripScheduleSave({1},{2})",
                    endpoint.Address,
                    ts,ss));
            }
            TripScheduleDAO.Instance.TripScheduleSave(ts,ss);
        }

        public List<Vlastelin.Data.Model.Driver> DriversGet(long? id)
        {
            if (OperationContext.Current != null)
            {
                RemoteEndpointMessageProperty endpoint = OperationContext.Current.IncomingMessageProperties[RemoteEndpointMessageProperty.Name] as RemoteEndpointMessageProperty;
                logger.LogMessage(LogEventType.Info, string.Format("Client {0} calling DriversGet({1})",
                    endpoint.Address,
                    id.ToString()));
            }
            return DriverDAO.Instance.DriversGet(id);
        }

        public List<Vlastelin.Data.Model.Driver> DriversGetByBus(Bus bus)
        {
            if (OperationContext.Current != null)
            {
                RemoteEndpointMessageProperty endpoint = OperationContext.Current.IncomingMessageProperties[RemoteEndpointMessageProperty.Name] as RemoteEndpointMessageProperty;
                logger.LogMessage(LogEventType.Info, string.Format("Client {0} calling DriversGetByBus()",
                    endpoint.Address));
            }
            return DriverDAO.Instance.DriversGetByBus(bus);
        }

        [PrincipalPermission(SecurityAction.Demand, Role = "Administrator")]
        public long DriverAdd(Vlastelin.Data.Model.Driver driver)
        {
            if (OperationContext.Current != null)
            {
                RemoteEndpointMessageProperty endpoint = OperationContext.Current.IncomingMessageProperties[RemoteEndpointMessageProperty.Name] as RemoteEndpointMessageProperty;
                logger.LogMessage(LogEventType.Info, string.Format("Client {0} calling DriverAdd('{1} {2}')",
                    endpoint.Address,
                    driver.Surname, driver.Name));
            }
            return DriverDAO.Instance.DriverAdd(driver);
        }
        [PrincipalPermission(SecurityAction.Demand, Role = "Administrator")]
        public void DriverDelete(Vlastelin.Data.Model.Driver driver)
        {
            if (OperationContext.Current != null)
            {
                RemoteEndpointMessageProperty endpoint = OperationContext.Current.IncomingMessageProperties[RemoteEndpointMessageProperty.Name] as RemoteEndpointMessageProperty;
                logger.LogMessage(LogEventType.Info, string.Format("Client {0} calling DriverDelete('{1} {2}')",
                    endpoint.Address,
                    driver.Surname, driver.Name));
            }
            DriverDAO.Instance.DriverDelete(driver);
        }
        [PrincipalPermission(SecurityAction.Demand, Role = "Administrator")]
        public void DriverEdit(Vlastelin.Data.Model.Driver driver)
        {
            if (OperationContext.Current != null)
            {
                RemoteEndpointMessageProperty endpoint = OperationContext.Current.IncomingMessageProperties[RemoteEndpointMessageProperty.Name] as RemoteEndpointMessageProperty;
                logger.LogMessage(LogEventType.Info, string.Format("Client {0} calling DriverEdit({1})",
                    endpoint.Address,
                    driver.Id.ToString()));
            }
            DriverDAO.Instance.DriverEdit(driver);
        }


        public List<Tuple<Seat, double, string>> SeatsGet(StationSchedule ss, TripPrice price, DateTime date)
        {
            RemoteEndpointMessageProperty endpoint = OperationContext.Current.IncomingMessageProperties[RemoteEndpointMessageProperty.Name] as RemoteEndpointMessageProperty;
            logger.LogMessage(LogEventType.Info, string.Format("Client {0} calling SeatsGet(ssId: {1} TPriceId:{2}, date:{3})",
                endpoint.Address,
                ss,price,date));
            return SeatDAO.Instance.SeatsGet(ss, price, date);
        }

        public Tuple<TimeSpan, long> SeatLock(int seatNum, int ssID, int tripPriceId, DateTime tripDate)
        {
            RemoteEndpointMessageProperty endpoint = OperationContext.Current.IncomingMessageProperties[RemoteEndpointMessageProperty.Name] as RemoteEndpointMessageProperty;
            logger.LogMessage(LogEventType.Info, string.Format("Client {0} calling SeatLock(seat {1}, trip {2} at {3})",
                endpoint.Address,
                seatNum,
                tripPriceId,
                tripDate));

            seatLockKey slk = new seatLockKey(seatNum, ssID, tripPriceId, tripDate);

            if (waitingThreads.ContainsKey(slk))
                throw new ArgumentException("Место уже заблокировано");

            if (seatLockTimeout == TimeSpan.Zero)
                return new Tuple<TimeSpan, long>(TimeSpan.Zero, 0);

            Thread waitingThread = new Thread(s =>
            {
                seatLockKey sk=(seatLockKey)s;
                Thread.Sleep(seatLockTimeout);
                waitingThreads.Remove(sk);
                SeatDAO.Instance.SeatUnlock(sk.seatNum, sk.ssId, sk.tpId, sk.dt);
            });

            long seatId = SeatDAO.Instance.SeatLock(seatNum, ssID, tripPriceId, tripDate);
            waitingThreads.Add(slk, waitingThread);
            logger.LogMessage(LogEventType.Info, "Starting waiting thread");
            waitingThread.Start(slk);

            return new Tuple<TimeSpan, long>(seatLockTimeout, seatId);
        }

        public void SeatUnlock(seatLockKey slk)
        {
            RemoteEndpointMessageProperty endpoint = OperationContext.Current.IncomingMessageProperties[RemoteEndpointMessageProperty.Name] as RemoteEndpointMessageProperty;
            logger.LogMessage(LogEventType.Info, string.Format("Client {0} calling SeatUnlock()",
                endpoint.Address));

            if (waitingThreads.ContainsKey(slk))
            {
                waitingThreads[slk].Abort();
                waitingThreads.Remove(slk);
                SeatDAO.Instance.SeatUnlock(slk.seatNum, slk.ssId, slk.tpId, slk.dt);
            }
            /*else
                throw new ArgumentException("Место не заблокировано");*/
        }

        public void SeatSell(Vlastelin.Data.Model.Operator op, Vlastelin.Data.Model.Seat seat, Vlastelin.Data.Model.Passenger passenger, double sum, int checkNum)
        {
            RemoteEndpointMessageProperty endpoint = OperationContext.Current.IncomingMessageProperties[RemoteEndpointMessageProperty.Name] as RemoteEndpointMessageProperty;
            logger.LogMessage(LogEventType.Info, string.Format("Client {0} calling SeatSell(seat {1})",
                endpoint.Address,
                seat));

            seatLockKey slk = new seatLockKey(seat.SeatNumber, seat.SSid, seat.TripPriceId, seat.TripDate);
            waitingThreads.Remove(slk);

            SeatDAO.Instance.SeatSell(seat, passenger);
            SalesHistoryDAO.Instance.SellItem(op, sum, SalesKind.Ticket, seat, checkNum);
        }
        public void SeatSellReturn(Operator op, Seat seat, int returnCheckNumber, int commissionCheckNumber)
        {
            if (OperationContext.Current != null)
            {
                RemoteEndpointMessageProperty endpoint = OperationContext.Current.IncomingMessageProperties[RemoteEndpointMessageProperty.Name] as RemoteEndpointMessageProperty;
                logger.LogMessage(LogEventType.Info, string.Format("Client {0} calling SeatSellReturn()",
                    endpoint.Address));
            }

            SeatDAO.Instance.SeatSellReturn(seat);
            SalesHistoryDAO.Instance.ReturnTicket(op, seat, returnCheckNumber,commissionCheckNumber);
        }

        public void SeatReserveCancel(Seat seat)
        {
            if (OperationContext.Current != null)
            {
                RemoteEndpointMessageProperty endpoint = OperationContext.Current.IncomingMessageProperties[RemoteEndpointMessageProperty.Name] as RemoteEndpointMessageProperty;
                logger.LogMessage(LogEventType.Info, string.Format("Client {0} calling SeatReserveCancel()",
                    endpoint.Address));
            }

            SeatDAO.Instance.SeatReserveCancel(seat);
        }
        public long SeatReserve(Seat seat)
        {
            RemoteEndpointMessageProperty endpoint = OperationContext.Current.IncomingMessageProperties[RemoteEndpointMessageProperty.Name] as RemoteEndpointMessageProperty;
            logger.LogMessage(LogEventType.Info, string.Format("Client {0} calling SeatReserve()",
                endpoint.Address));
            SeatDAO.Instance.SeatReserve(seat);

            return seat.Id;
        }

        public List<Passenger> PassengersGet(long? id)
        {
            RemoteEndpointMessageProperty endpoint = OperationContext.Current.IncomingMessageProperties[RemoteEndpointMessageProperty.Name] as RemoteEndpointMessageProperty;
            logger.LogMessage(LogEventType.Info, string.Format("Client {0} calling PassengersGet({1})",
                endpoint.Address,
                id.ToString()));
            return PassengerDAO.Instance.PassengersGet(id);
        }

        public long PassengerAdd(Passenger passenger)
        {
            RemoteEndpointMessageProperty endpoint = OperationContext.Current.IncomingMessageProperties[RemoteEndpointMessageProperty.Name] as RemoteEndpointMessageProperty;
            logger.LogMessage(LogEventType.Info, string.Format("Client {0} calling PassengerAdd('{1} {2}')",
                endpoint.Address,
                passenger.Surname, passenger.Name));
            return PassengerDAO.Instance.PassengerAdd(passenger);
        }

        [PrincipalPermission(SecurityAction.Demand, Role = "Administrator")]
        public long DriverAuthorityAdd(int driverId, int ownerId, string num, DateTime date)
        {
            RemoteEndpointMessageProperty endpoint = OperationContext.Current.IncomingMessageProperties[RemoteEndpointMessageProperty.Name] as RemoteEndpointMessageProperty;
            logger.LogMessage(LogEventType.Info, string.Format("Client {0} calling DriverAuthorityAdd()",
                endpoint.Address));
            return DriverAuthorityDAO.Instance.DriverAuthorityAdd(driverId, ownerId, num, date);
        }
        [PrincipalPermission(SecurityAction.Demand, Role = "Administrator")]
        public void DriverAuthorityDelete(int daId)
        {
            RemoteEndpointMessageProperty endpoint = OperationContext.Current.IncomingMessageProperties[RemoteEndpointMessageProperty.Name] as RemoteEndpointMessageProperty;
            logger.LogMessage(LogEventType.Info, string.Format("Client {0} calling DriverAuthorityDelete()",
                endpoint.Address));
            DriverAuthorityDAO.Instance.DriverAuthorityDelete(daId);
        }

        public List<RKO> RKOGet(long? id)
        {
            RemoteEndpointMessageProperty endpoint = OperationContext.Current.IncomingMessageProperties[RemoteEndpointMessageProperty.Name] as RemoteEndpointMessageProperty;
            logger.LogMessage(LogEventType.Info, string.Format("Client {0} calling RKOGet('{1}')",
                endpoint.Address,
                id.ToString()));
            return RKODAO.Instance.RKOGet(id);
        }

        /*public List<RKO> RKOReport(DateTime from, DateTime to)
        {
            RemoteEndpointMessageProperty endpoint = OperationContext.Current.IncomingMessageProperties[RemoteEndpointMessageProperty.Name] as RemoteEndpointMessageProperty;
            logger.LogMessage(LogEventType.Info, string.Format("Client {0} calling RKOReport('{1}','{2}')",
                endpoint.Address,
                from.ToString("dd.MM.yyyy"),
                to.ToString("dd.MM.yyyy")));
            return RKODAO.Instance.RKOReport(from, to);
        }*/

        public long RKOAdd(RKO rko)
        {
            RemoteEndpointMessageProperty endpoint = OperationContext.Current.IncomingMessageProperties[RemoteEndpointMessageProperty.Name] as RemoteEndpointMessageProperty;
            logger.LogMessage(LogEventType.Info, string.Format("Client {0} calling RKOAdd(DocNum: {1}, DocDate: {2})",
                endpoint.Address,
                rko.Number,
                rko.DocDate.ToString("dd.MM.yyyy")));
            return RKODAO.Instance.RKOAdd(rko);
        }


        public List<Operator> OperatorsGet(long? id)
        {
            if (OperationContext.Current != null)
            {
                RemoteEndpointMessageProperty endpoint = OperationContext.Current.IncomingMessageProperties[RemoteEndpointMessageProperty.Name] as RemoteEndpointMessageProperty;
                logger.LogMessage(LogEventType.Info, string.Format("Client {0} calling OperatorsGet('{1}')",
                    endpoint.Address,
                    id.ToString()));
            }
            return OperatorDAO.Instance.OperatorsGet(id);
        }
        [PrincipalPermission(SecurityAction.Demand, Role = "Administrator")]
        public long OperatorAdd(Operator op, string login, string password)
        {
            if (OperationContext.Current != null)
            {
                RemoteEndpointMessageProperty endpoint = OperationContext.Current.IncomingMessageProperties[RemoteEndpointMessageProperty.Name] as RemoteEndpointMessageProperty;
                logger.LogMessage(LogEventType.Info, string.Format("Client {0} calling OperatorAdd('{1} {2} {3}')",
                    endpoint.Address,
                    op.Name, op.Surname, op.Patronymic));
            }
            return OperatorDAO.Instance.OperatorAdd(op, login, password);
        }
        [PrincipalPermission(SecurityAction.Demand, Role = "Administrator")]
        public void OperatorDelete(Operator op)
        {
            if (OperationContext.Current != null)
            {
                RemoteEndpointMessageProperty endpoint = OperationContext.Current.IncomingMessageProperties[RemoteEndpointMessageProperty.Name] as RemoteEndpointMessageProperty;
                logger.LogMessage(LogEventType.Info, string.Format("Client {0} calling OperatorDelete( ID: {4}, '{1} {2} {3}')",
                    endpoint.Address,
                    op.Name, op.Surname, op.Patronymic, op.Id));
            }
            OperatorDAO.Instance.OperatorDelete(op);
        }
        [PrincipalPermission(SecurityAction.Demand, Role = "Administrator")]
        public void OperatorEdit(Operator op, string login, string password)
        {
            if (OperationContext.Current != null)
            {
                RemoteEndpointMessageProperty endpoint = OperationContext.Current.IncomingMessageProperties[RemoteEndpointMessageProperty.Name] as RemoteEndpointMessageProperty;
                logger.LogMessage(LogEventType.Info, string.Format("Client {0} calling OperatorEdit( ID: {4}, '{1} {2} {3}')",
                    endpoint.Address,
                    op.Name, op.Surname, op.Patronymic, op.Id));
            }
            OperatorDAO.Instance.OperatorEdit(op);
            if (!string.IsNullOrWhiteSpace(password))
                MembershipUserDAO.Instance.SetPassword(op.User.Username, "VlastelinServer", password);
            MembershipRoleDAO.Instance.EnsureUserInRole(op.Login, op.Role);
        }

        public Operator OperatorGetByLogin(string login)
        {
            if (OperationContext.Current != null)
            {
                RemoteEndpointMessageProperty endpoint = OperationContext.Current.IncomingMessageProperties[RemoteEndpointMessageProperty.Name] as RemoteEndpointMessageProperty;
                logger.LogMessage(LogEventType.Info, string.Format("Client {0} calling OperatorGetByLogin({1}')",
                    endpoint.Address,
                    login));
            }
            return OperatorDAO.Instance.OperatorGetByLogin(login);
        }

        public List<TripScheduleFact> TripScheduleFactGet(long? id)
        {
            RemoteEndpointMessageProperty endpoint = OperationContext.Current.IncomingMessageProperties[RemoteEndpointMessageProperty.Name] as RemoteEndpointMessageProperty;
            logger.LogMessage(LogEventType.Info, string.Format("Client {0} calling TripScheduleFactGet({1})",
                endpoint.Address,
                id.ToString()));

            return TripScheduleFactDAO.Instance.TripScheduleFactGet(id);
        }

        public List<TripScheduleFact> TripScheduleFactGetByDate(DateTime dt)
        {
            RemoteEndpointMessageProperty endpoint = OperationContext.Current.IncomingMessageProperties[RemoteEndpointMessageProperty.Name] as RemoteEndpointMessageProperty;
            logger.LogMessage(LogEventType.Info, string.Format("Client {0} calling TripScheduleFactGetByDate({1})",
                endpoint.Address,
                dt.ToString()));

            return TripScheduleFactDAO.Instance.TripScheduleFactGet(dt);
        }

        public long TripScheduleFactAdd(TripScheduleFact tsf)
        {
            RemoteEndpointMessageProperty endpoint = OperationContext.Current.IncomingMessageProperties[RemoteEndpointMessageProperty.Name] as RemoteEndpointMessageProperty;
            logger.LogMessage(LogEventType.Info, string.Format("Client {0} calling TripScheduleFactAdd()",
                endpoint.Address));

            return TripScheduleFactDAO.Instance.TripScheduleFactAdd(tsf);
        }

        public void TripScheduleFactDelete(TripScheduleFact tsf)
        {
            RemoteEndpointMessageProperty endpoint = OperationContext.Current.IncomingMessageProperties[RemoteEndpointMessageProperty.Name] as RemoteEndpointMessageProperty;
            logger.LogMessage(LogEventType.Info, string.Format("Client {0} calling TripScheduleFactDelete(TSF ID: {1})",
                endpoint.Address,
                tsf.Id));

            TripScheduleFactDAO.Instance.TripScheduleFactDelete(tsf);
        }



        public DateTime GetLastModifiedTime(ModifiedObjects obj)
        {
            return TablesTimeDAO.Instance.GetLastModifiedTime(obj);
        }

        
        public List<Branch> BranchesGet(long? id)
        {
            if (OperationContext.Current != null)
            {
                RemoteEndpointMessageProperty endpoint = OperationContext.Current.IncomingMessageProperties[RemoteEndpointMessageProperty.Name] as RemoteEndpointMessageProperty;
                logger.LogMessage(LogEventType.Info, string.Format("Client {0} calling BranchesGet({1})",
                    endpoint.Address,
                    id.ToString()));
            }
            return BranchDAO.Instance.BranchesGet(id);
        }
        [PrincipalPermission(SecurityAction.Demand, Role = "Administrator")]
        public long BranchAdd(Branch branch)
        {
            if (OperationContext.Current != null)
            {
                RemoteEndpointMessageProperty endpoint = OperationContext.Current.IncomingMessageProperties[RemoteEndpointMessageProperty.Name] as RemoteEndpointMessageProperty;
                logger.LogMessage(LogEventType.Info, string.Format("Client {0} calling BranchAdd()",
                    endpoint.Address));
            }
            return BranchDAO.Instance.BranchAdd(branch);
        }
        [PrincipalPermission(SecurityAction.Demand, Role = "Administrator")]
        public void BranchDelete(Branch branch)
        {
            if (OperationContext.Current != null)
            {
                RemoteEndpointMessageProperty endpoint = OperationContext.Current.IncomingMessageProperties[RemoteEndpointMessageProperty.Name] as RemoteEndpointMessageProperty;
                logger.LogMessage(LogEventType.Info, string.Format("Client {0} calling BranchDelete(ID: {1})",
                    endpoint.Address,
                    branch.Id));
            }
            BranchDAO.Instance.BranchDelete(branch);
        }
        [PrincipalPermission(SecurityAction.Demand, Role = "Administrator")]
        public void BranchEdit(Branch branch)
        {
            if (OperationContext.Current != null)
            {
                RemoteEndpointMessageProperty endpoint = OperationContext.Current.IncomingMessageProperties[RemoteEndpointMessageProperty.Name] as RemoteEndpointMessageProperty;
                logger.LogMessage(LogEventType.Info, string.Format("Client {0} calling BranchEdit(ID: {1})",
                    endpoint.Address,
                    branch.Id));
            }
            BranchDAO.Instance.BranchEdit(branch);
        }


        public MainSettings MainSettingsGet()
        {
            return MainSettingsDAO.Instance.MainSettingsGet();
        }
        [PrincipalPermission(SecurityAction.Demand, Role = "Administrator")]
        public void MainSettingsEdit(MainSettings settings)
        {
            MainSettingsDAO.Instance.MainSettingsEdit(settings);
        }

        public string DocNumGetNextNumber(Town t)
        {
            RemoteEndpointMessageProperty endpoint = OperationContext.Current.IncomingMessageProperties[RemoteEndpointMessageProperty.Name] as RemoteEndpointMessageProperty;
            logger.LogMessage(LogEventType.Info, string.Format("Client {0} calling DocNumGetNextNumber({1})",
                endpoint.Address,
                t.Name));

            return TownDAO.Instance.GetNextNumber(t);
        }


        public List<StationOrder> StationOrderGet(Trip trip)
        {
            if (OperationContext.Current != null)
            {
                RemoteEndpointMessageProperty endpoint = OperationContext.Current.IncomingMessageProperties[RemoteEndpointMessageProperty.Name] as RemoteEndpointMessageProperty;
                logger.LogMessage(LogEventType.Info, string.Format("Client {0} calling StationOrderGet(tripId: {1})",
                    endpoint.Address,
                    trip));
            }
            return StationOrderDAO.Instance.StationOrderGet(trip);
        }
        [PrincipalPermission(SecurityAction.Demand, Role = "Administrator")]
        public void StationOrderEdit(Trip trip, List<Town> towns)
        {
            if (OperationContext.Current != null)
            {
                RemoteEndpointMessageProperty endpoint = OperationContext.Current.IncomingMessageProperties[RemoteEndpointMessageProperty.Name] as RemoteEndpointMessageProperty;
                logger.LogMessage(LogEventType.Info, string.Format("Client {0} calling StationOrderEdit(tripId: {1})",
                    endpoint.Address,
                    trip.Id));
            }
            StationOrderDAO.Instance.StationOrderEdit(trip,towns);
        }

  
        public List<StationSchedule> StationScheduleGet(TripSchedule ts)
        {
            if (OperationContext.Current != null)
            {
                RemoteEndpointMessageProperty endpoint = OperationContext.Current.IncomingMessageProperties[RemoteEndpointMessageProperty.Name] as RemoteEndpointMessageProperty;
                logger.LogMessage(LogEventType.Info, string.Format("Client {0} calling StationOScheduleGet(tsId: {1})",
                    endpoint.Address,
                    ts));
            }
            return StationScheduleDAO.Instance.StationScheduleGet(ts);
        }
        [PrincipalPermission(SecurityAction.Demand, Role = "Administrator")]
        public void StationScheduleEdit(StationSchedule ss)
        {
            if (OperationContext.Current != null)
            {
                RemoteEndpointMessageProperty endpoint = OperationContext.Current.IncomingMessageProperties[RemoteEndpointMessageProperty.Name] as RemoteEndpointMessageProperty;
                logger.LogMessage(LogEventType.Info, string.Format("Client {0} calling StationScheduleEdit(ssId: {1})",
                    endpoint.Address,
                    ss));
            }
            StationScheduleDAO.Instance.StationScheduleEdit(ss);
        }
        [PrincipalPermission(SecurityAction.Demand, Role = "Administrator")]
        public long StationScheduleAdd(StationSchedule ss)
        {
            if (OperationContext.Current != null)
            {
                RemoteEndpointMessageProperty endpoint = OperationContext.Current.IncomingMessageProperties[RemoteEndpointMessageProperty.Name] as RemoteEndpointMessageProperty;
                logger.LogMessage(LogEventType.Info, string.Format("Client {0} calling StationScheduleAdd()",
                    endpoint.Address));
            }
            return StationScheduleDAO.Instance.StationScheduleAdd(ss);
        }
        [PrincipalPermission(SecurityAction.Demand, Role = "Administrator")]
        public void StationScheduleDelete(StationSchedule ss)
        {
            if (OperationContext.Current != null)
            {
                RemoteEndpointMessageProperty endpoint = OperationContext.Current.IncomingMessageProperties[RemoteEndpointMessageProperty.Name] as RemoteEndpointMessageProperty;
                logger.LogMessage(LogEventType.Info, string.Format("Client {0} calling StationScheduleDelete(ssId: {1})",
                    endpoint.Address,
                    ss));
            }
            StationScheduleDAO.Instance.StationScheduleDelete(ss);
        }
        [PrincipalPermission(SecurityAction.Demand, Role = "Administrator")]
        public void StationScheduleSave(List<StationSchedule> list)
        {
            if (OperationContext.Current != null)
            {
                RemoteEndpointMessageProperty endpoint = OperationContext.Current.IncomingMessageProperties[RemoteEndpointMessageProperty.Name] as RemoteEndpointMessageProperty;
                logger.LogMessage(LogEventType.Info, string.Format("Client {0} calling StationScheduleSave(count: {1}): NOT IMPLEMENTED on DAO side!",
                    endpoint.Address,
                    list.Count));
            }
            throw new NotImplementedException();
            //StationScheduleDAO.Instance.StationSchedulesSave(list);
        }

        public List<TripPrice> TripPricesGet(Trip trip)
        {
            if (OperationContext.Current != null)
            {
                RemoteEndpointMessageProperty endpoint = OperationContext.Current.IncomingMessageProperties[RemoteEndpointMessageProperty.Name] as RemoteEndpointMessageProperty;
                logger.LogMessage(LogEventType.Info, string.Format("Client {0} calling TripPricesGet(tripId: {1})",
                    endpoint.Address,
                    trip));
            }
            return TripPriceDAO.Instance.TripPricesGet(trip);
        }
        [PrincipalPermission(SecurityAction.Demand, Role = "Administrator")]
        public void TripPriceEdit(TripPrice tp)
        {
            if (OperationContext.Current != null)
            {
                RemoteEndpointMessageProperty endpoint = OperationContext.Current.IncomingMessageProperties[RemoteEndpointMessageProperty.Name] as RemoteEndpointMessageProperty;
                logger.LogMessage(LogEventType.Info, string.Format("Client {0} calling TripPriceEdit(tripPriceId: {1})",
                    endpoint.Address,
                    tp));
            }
            TripPriceDAO.Instance.TripPriceEdit(tp);
        }
        [PrincipalPermission(SecurityAction.Demand, Role = "Administrator")]
        public void TripPriceSave(List<TripPrice> tPrices)
        {
            if (OperationContext.Current != null)
            {
                RemoteEndpointMessageProperty endpoint = OperationContext.Current.IncomingMessageProperties[RemoteEndpointMessageProperty.Name] as RemoteEndpointMessageProperty;
                logger.LogMessage(LogEventType.Info, string.Format("Client {0} calling TripPriceSave(tripPrices: {1})",
                    endpoint.Address,
                    tPrices));
            }
            TripPriceDAO.Instance.TripPriceSave(tPrices);
        }
     
        public List<SalesHistory> SalesHistoryGet(DateTime? from, DateTime? to)
        {
            if (OperationContext.Current != null)
            {
                RemoteEndpointMessageProperty endpoint = OperationContext.Current.IncomingMessageProperties[RemoteEndpointMessageProperty.Name] as RemoteEndpointMessageProperty;
                logger.LogMessage(LogEventType.Info, string.Format("Client {0} calling SalesHistoryGet({1},{2})",
                    endpoint.Address,
                    from, to));
            }
            return SalesHistoryDAO.Instance.SalesHistoryGet(from, to);
        }

        public void SellItem(Operator op, double sum, SalesKind kind, object item, int checkNumber)
        {
            /*if (OperationContext.Current != null)
            {
                RemoteEndpointMessageProperty endpoint = OperationContext.Current.IncomingMessageProperties[RemoteEndpointMessageProperty.Name] as RemoteEndpointMessageProperty;
                logger.LogMessage(LogEventType.Info, string.Format("Client {0} calling SellItem()",
                    endpoint.Address));
            }

            if (kind == SalesKind.Ticket)
                throw new InvalidOperationException("Cannot directly sell items of this item kind");
            if(item is IItemForSale)
                SalesHistoryDAO.Instance.SellItem(op,sum,kind,(IItemForSale)item,checkNumber);*/
            throw new NotImplementedException();
        }

        public List<SalesKind> SalesKindsGet(long? id)
        {
            if (OperationContext.Current != null)
            {
                RemoteEndpointMessageProperty endpoint = OperationContext.Current.IncomingMessageProperties[RemoteEndpointMessageProperty.Name] as RemoteEndpointMessageProperty;
                logger.LogMessage(LogEventType.Info, string.Format("Client {0} calling SalesKindsGet({1})",
                    endpoint.Address,
                    id));
            }
            return SalesKindDAO.Instance.SalesKindsGet(id);
        }
        


        public void SalesKindSell(Operator op, SalesKind item, double sum, int checkNumber)
        {
            if (OperationContext.Current != null)
            {
                RemoteEndpointMessageProperty endpoint = OperationContext.Current.IncomingMessageProperties[RemoteEndpointMessageProperty.Name] as RemoteEndpointMessageProperty;
                logger.LogMessage(LogEventType.Info, string.Format("Client {0} calling SellItem()",
                    endpoint.Address));
            }

            
            SalesHistoryDAO.Instance.SellItem(op, sum, item, item, checkNumber);
        }

        public System.Data.DataTable ReportGet(ReportTypes reportType, DateTime from, DateTime to, BaseItem param1, BaseItem param2)
        {
            if (OperationContext.Current != null)
            {
                RemoteEndpointMessageProperty endpoint = OperationContext.Current.IncomingMessageProperties[RemoteEndpointMessageProperty.Name] as RemoteEndpointMessageProperty;
                logger.LogMessage(LogEventType.Info, string.Format("Client {0} calling ReportGet({1})",
                    endpoint.Address,reportType.ToString()));
            }

            return ReportDAO.Instance.ReportGet(reportType, from, to, param1, param2);
        }

        public System.Data.DataTable ReportStatement(StationSchedule ss, DateTime depTime)
        {
            if (OperationContext.Current != null)
            {
                RemoteEndpointMessageProperty endpoint = OperationContext.Current.IncomingMessageProperties[RemoteEndpointMessageProperty.Name] as RemoteEndpointMessageProperty;
                logger.LogMessage(LogEventType.Info, string.Format("Client {0} calling ReportStatement()",
                    endpoint.Address));
            }

            return ReportDAO.Instance.ReportStatement(ss,depTime);
        }

        public List<PKO> PKOGet(long? id)
        {
            RemoteEndpointMessageProperty endpoint = OperationContext.Current.IncomingMessageProperties[RemoteEndpointMessageProperty.Name] as RemoteEndpointMessageProperty;
            logger.LogMessage(LogEventType.Info, string.Format("Client {0} calling PKOGet('{1}')",
                endpoint.Address,
                id));
            return PKODAO.Instance.PKOGet(id);
        }

        public long PKOAdd(PKO pko)
        {
            RemoteEndpointMessageProperty endpoint = OperationContext.Current.IncomingMessageProperties[RemoteEndpointMessageProperty.Name] as RemoteEndpointMessageProperty;
            logger.LogMessage(LogEventType.Info, string.Format("Client {0} calling PKOAdd(DocNum: {1}, DocDate: {2})",
                endpoint.Address,
                pko.DocNum,
                pko.DocDate.ToString("dd.MM.yyyy")));
            return PKODAO.Instance.PKOAdd(pko);
        }


        public List<SalesHistory> SalesHistoryBySeat(Seat seat)
        {
            if (OperationContext.Current != null)
            {
                RemoteEndpointMessageProperty endpoint = OperationContext.Current.IncomingMessageProperties[RemoteEndpointMessageProperty.Name] as RemoteEndpointMessageProperty;
                logger.LogMessage(LogEventType.Info, string.Format("Client {0} calling SalesHistoryBySeat()",
                    endpoint.Address));
            }
            return SalesHistoryDAO.Instance.SalesHistoryGet(seat);
        }

        public List<Bus> GetAvailableBuses(StationSchedule ss, DateTime tripDate, int minSeatsCount)
        {
            if (OperationContext.Current != null)
            {
                RemoteEndpointMessageProperty endpoint = OperationContext.Current.IncomingMessageProperties[RemoteEndpointMessageProperty.Name] as RemoteEndpointMessageProperty;
                logger.LogMessage(LogEventType.Info, string.Format("Client {0} calling GetAvailableBuses()",
                    endpoint.Address));
            }
            return BusDAO.Instance.GetAvailableBuses(ss, tripDate, minSeatsCount);
        }

        public void OperatorChangeBus(TripSchedule ts, Bus newBus, DateTime tripDate)
        {
            if (OperationContext.Current != null)
            {
                RemoteEndpointMessageProperty endpoint = OperationContext.Current.IncomingMessageProperties[RemoteEndpointMessageProperty.Name] as RemoteEndpointMessageProperty;
                logger.LogMessage(LogEventType.Info, string.Format("Client {0} calling OperatorChangeBus()",
                    endpoint.Address));
            }
            TripScheduleDAO.Instance.OperatorChangeBus(ts, newBus, tripDate);
        }

        public System.Data.DataTable ReportPassengers(DateTime? from, DateTime? to, string surname, string name, string patronymic)
        {
            if (OperationContext.Current != null)
            {
                RemoteEndpointMessageProperty endpoint = OperationContext.Current.IncomingMessageProperties[RemoteEndpointMessageProperty.Name] as RemoteEndpointMessageProperty;
                logger.LogMessage(LogEventType.Info, string.Format("Client {0} calling ReportPassengers()",
                    endpoint.Address));
            }
            return ReportDAO.Instance.ReportPassengers(from, to, surname, name, patronymic);
        }

        public List<Vlastelin.Data.Model.ExportedData.ExportedRKO> ExportRKO(DateTime? from, DateTime? to)
        {
            if (OperationContext.Current != null)
            {
                RemoteEndpointMessageProperty endpoint = OperationContext.Current.IncomingMessageProperties[RemoteEndpointMessageProperty.Name] as RemoteEndpointMessageProperty;
                logger.LogMessage(LogEventType.Info, string.Format("Client {0} calling ExportRKO()",
                    endpoint.Address));
            }
            return ExportedRKODAO.Instance.ExportRKO(from, to);
        }

        
    }
}
