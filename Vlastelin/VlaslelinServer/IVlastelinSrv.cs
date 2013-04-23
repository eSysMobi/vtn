using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Net.Security;
using System.Text;
using Vlastelin.Data.Model;
using System.IdentityModel.Tokens;
using System.Data;
using Vlastelin.Common;
using System.Security.Permissions;

namespace VlastelinServer
{
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the interface name "IService1" in both code and config file together.
    [ServiceContract(Namespace="vlastelinService")]
    public interface IVlastelinSrv
    {
        [OperationContract]
        string GetData(int value);
        
        [OperationContract]
        int TestConnection();

        [OperationContract]
        DateTime GetLastModifiedTime(Vlastelin.Common.ModifiedObjects obj);

        #region Towns
        /// <summary>
        /// Получаем город(а)
        /// </summary>
        /// <param name="id">id города. если null - получаем весь список</param>
        /// <returns>список гродов</returns>
        [OperationContract]
        List<Town> TownsGet(long? id);

        /// <summary>
        /// Добавляем город в справочник городов
        /// </summary>
        /// <param name="town">город</param>
        /// <returns>ID нового города</returns>
        [OperationContract]
        long TownAdd(Town town);

        /// <summary>
        /// Удаляем город
        /// </summary>
        /// <param name="town">тот город, который надо удалить</param>
        [OperationContract]
        void TownDelete(Town town);

        /// <summary>
        /// редактируем город
        /// </summary>
        /// <param name="town">город</param>
        [OperationContract]
        void TownEdit(Town town);

        [OperationContract]
        string DocNumGetNextNumber(Town t);
        #endregion

        #region Owners
        /// <summary>
        /// Получаем владельца(ев)
        /// </summary>
        /// <param name="id">id владельца. если null - получаем весь список</param>
        /// <returns>список владельцев</returns>
        [OperationContract]
        List<Owner> OwnersGet(long? id);

        /// <summary>
        /// Добавляем нового перевозчика в справочник
        /// </summary>
        /// <param name="owner">владелец</param>
        /// <returns>ID нового перевозчика</returns>
        [OperationContract]
        long OwnerAdd(Owner owner);

        /// <summary>
        /// Удаляем владельца
        /// </summary>
        /// <param name="owner">тот перевозчик, которого надо удалить</param>
        [OperationContract]
        void OwnerDelete(Owner owner);

        /// <summary>
        /// редактируем данные перевозчика
        /// </summary>
        /// <param name="owner">перевозчик</param>
        [OperationContract]
        void OwnerEdit(Owner owner);
        #endregion

        #region Buses
        /// <summary>
        /// Получаем автобус(ы)
        /// </summary>
        /// <param name="id">id автобуса. если null - получаем весь список</param>
        /// <returns>список автобусов</returns>
        [OperationContract]
        List<Bus> BusesGet(long? id);

        /// <summary>
        /// Добавляем новый автобус в справочник
        /// </summary>
        /// <param name="bus">автобус</param>
        /// <returns>ID нового автобуса</returns>
        [OperationContract]
        long BusAdd(Bus bus);

        /// <summary>
        /// Удаляем автобус
        /// </summary>
        /// <param name="bus">тот автобус, которого надо удалить</param>
        [OperationContract]
        void BusDelete(Bus bus);

        /// <summary>
        /// редактируем данные автобуса
        /// </summary>
        /// <param name="bus">автобус</param>
        [OperationContract]
        void BusEdit(Bus bus);
        #endregion

        #region Trips
        /// <summary>
        /// Получаем маршруты
        /// </summary>
        /// <param name="id">id маршрута. если null - получаем весь список</param>
        /// <returns>список маршрутов</returns>
        [OperationContract]
        List<Trip> TripsGet(long? id);

        /// <summary>
        /// Добавляем новый маршрут в справочник
        /// </summary>
        /// <param name="trip">маршрут</param>
        /// <returns>ID нового маршрута</returns>
        [OperationContract]
        long TripAdd(Trip trip);

        /// <summary>
        /// Удаляем маршрут
        /// </summary>
        /// <param name="trip">тот маршрут, который надо удалить</param>
        [OperationContract]
        void TripDelete(Trip trip);

        /// <summary>
        /// редактируем данные маршрута
        /// </summary>
        /// <param name="trip">маршрут</param>
        [OperationContract]
        void TripEdit(Trip trip);
        #endregion

        #region TripsSchedule
        /// <summary>
        /// Получаем расписание
        /// </summary>
        /// <param name="id">id расписания. если null - получаем весь список</param>
        /// <returns>расписание</returns>
        [OperationContract]
        List<TripSchedule> TripScheduleGet(long? id);

        /// <summary>
        /// Добавляем новое расписание в справочник
        /// </summary>
        /// <param name="ts">расписание</param>
        /// <returns>ID нового расписания</returns>
        [OperationContract]
        long TripScheduleAdd(TripSchedule ts);

        /// <summary>
        /// Удаляем расписание
        /// </summary>
        /// <param name="ts">расписание</param>
        [OperationContract]
        void TripScheduleDelete(TripSchedule ts);

        /// <summary>
        /// редактируем расписание
        /// </summary>
        /// <param name="ts">расписание</param>
        [OperationContract]
        void TripScheduleEdit(TripSchedule ts);

        [OperationContract]
        void TripScheduleSave(TripSchedule ts, List<StationSchedule> ss);
        #endregion

        #region Drivers
        /// <summary>
        /// Получаем водителей
        /// </summary>
        /// <param name="id">id водителя. если null - получаем весь список</param>
        /// <returns>водитель</returns>
        [OperationContract]
        List<Driver> DriversGet(long? id);

        /// <summary>
        /// Добавляем нового водителя  в справочник
        /// </summary>
        /// <param name="driver">водитель</param>
        /// <returns>ID нового водителя</returns>
        [OperationContract]
        long DriverAdd(Driver driver);

        /// <summary>
        /// Удаляем водителя
        /// </summary>
        /// <param name="driver">водитель</param>
        [OperationContract]
        void DriverDelete(Driver driver);

        /// <summary>
        /// редактируем водителя
        /// </summary>
        /// <param name="driver">водитель</param>
        [OperationContract]
        void DriverEdit(Driver driver);

        [OperationContract]
        List<Driver> DriversGetByBus(Bus bus);

        #endregion

        #region Seats
        /// <summary>
        /// Получаем места для данного рейса.
        /// </summary>
        /// <param name="ts">Элемент расписания, для которого надо получить список мест</param>
        /// <returns>Список мест</returns>
        [OperationContract]
        List<Tuple<Seat,double,string>> SeatsGet(StationSchedule ss, TripPrice price, DateTime date);

        /// <summary>
        /// Блокируем место на время оформления докуметов.
        /// </summary>
        /// <param name="seat">Место</param>
        /// <returns>Время до автоматической разблокировки</returns>
        [OperationContract]
        Tuple<TimeSpan, long> SeatLock(int seatNum, int ssID, int tripPriceId, DateTime tripDate);

        /// <summary>
        /// разброкируем ошибочно заблокированное место
        /// </summary>
        /// <param name="seat">место</param>
        [OperationContract]
        void SeatUnlock(seatLockKey slk);

        /// <summary>
        /// Закрепление места за пассажиром
        /// </summary>
        /// <param name="seat">Место</param>
        /// <param name="passenger">Пассажир</param>
        [OperationContract]
        void SeatSell(Operator op, Seat seat, Passenger passenger, double sum, int checkNum);

        [OperationContract]
        void SeatSellReturn(Operator op, Seat seat, int returnCheckNumber, int commissionCheckNumber);

        /// <summary>
        /// Бронирование места
        /// </summary>
        /// <param name="seat">место</param>
        /// <returns>ID нового места</returns>
        [OperationContract]
        long SeatReserve(Seat seat);

        /// <summary>
        /// Отмена бронирования места
        /// </summary>
        /// <param name="seat">место</param>
        [OperationContract]
        void SeatReserveCancel(Seat seat);
        #endregion

        #region Passengers
        /// <summary>
        /// Получаем пассажиров
        /// </summary>
        /// <param name="id">id пассажира. если null - получаем весь список</param>
        /// <returns>ид пассажира</returns>
        [OperationContract]
        List<Passenger> PassengersGet(long? id);

        /// <summary>
        /// Добавляем нового водителя  в справочник
        /// </summary>
        /// <param name="driver">водитель</param>
        /// <returns>ID нового водителя</returns>
        [OperationContract]
        long PassengerAdd(Passenger passenger);

        #endregion

        #region DriverAuthorities
        /// <summary>
        /// Добавляяем доверенность в справочник
        /// </summary>
        /// <param name="da">доверенность</param>
        /// <returns>ID новой доверенности</returns>
        [OperationContract]
        long DriverAuthorityAdd(int driverId, int ownerId, string num, DateTime date);

        /// <summary>
        /// Удаляем доверенность
        /// </summary>
        /// <param name="da">доверенность</param>
        [OperationContract]
        void DriverAuthorityDelete(int daId);
        #endregion

        #region RKO
        [OperationContract]
        List<RKO> RKOGet(long? id);

        //[OperationContract]
        //List<RKO> RKOReport(DateTime from, DateTime to);

        [OperationContract]
        long RKOAdd(RKO rko);

        #endregion

        #region Operators
        [OperationContract]
        List<Operator> OperatorsGet(long? id);

        [OperationContract]
        long OperatorAdd(Operator op, string login, string password);

        [OperationContract]
        void OperatorDelete(Operator op);

        [OperationContract]
        void OperatorEdit(Operator op, string login, string password);

        [OperationContract]
        Operator OperatorGetByLogin(string login);
        #endregion

        #region Branches
        [OperationContract]
        List<Branch> BranchesGet(long? id);

        [OperationContract]
        long BranchAdd(Branch branch);

        [OperationContract]
        void BranchDelete(Branch branch);

        [OperationContract]
        void BranchEdit(Branch branch);
        #endregion

        #region TripScheduleFact

        [OperationContract]
        List<TripScheduleFact> TripScheduleFactGet(long? id);

        [OperationContract]
        List<TripScheduleFact> TripScheduleFactGetByDate(DateTime dt);

        [OperationContract]
        long TripScheduleFactAdd(TripScheduleFact tsf);

        [OperationContract]
        void TripScheduleFactDelete(TripScheduleFact tsf);

        #endregion

        #region MainSettings
        [OperationContract]
        MainSettings MainSettingsGet();

        [OperationContract]
        void MainSettingsEdit(MainSettings settings);
        #endregion

        #region StationOrder
        [OperationContract]
        List<StationOrder> StationOrderGet(Trip trip);

        [OperationContract]
        void StationOrderEdit(Trip trip, List<Town> towns);
        #endregion

        #region StationSchedule
        [OperationContract]
        List<StationSchedule> StationScheduleGet(TripSchedule ts);

        [OperationContract]
        long StationScheduleAdd(StationSchedule ss);

        [OperationContract]
        void StationScheduleEdit(StationSchedule ss);

        [OperationContract]
        void StationScheduleDelete(StationSchedule ss);

        [OperationContract]
        void StationScheduleSave(List<StationSchedule> list);
        #endregion

        #region TripPrices
        [OperationContract]
        List<TripPrice> TripPricesGet(Trip trip);

        [OperationContract]
        void TripPriceEdit(TripPrice tp);

        [OperationContract]
        void TripPriceSave(List<TripPrice> tPrices);
        #endregion

        #region PKO
        [OperationContract]
        List<PKO> PKOGet(long? id);

        [OperationContract]
        long PKOAdd(PKO pko);
        #endregion
        
        #region sales
        [OperationContract]
        List<SalesKind> SalesKindsGet(long? id);

        [OperationContract]
        void SalesKindSell(Operator op, SalesKind item, double sum, int checkNumber);

        [OperationContract]
        List<SalesHistory> SalesHistoryGet(DateTime? from, DateTime? to);

        [OperationContract]
        List<SalesHistory> SalesHistoryBySeat(Seat seat);
        #endregion

        #region reports
        [OperationContract]
        [ServiceKnownType(typeof(Bus))]
        [ServiceKnownType(typeof(Trip))]
        DataTable ReportGet(ReportTypes reportType, DateTime from, DateTime to, BaseItem param1, BaseItem param2);

        //[OperationContract]
        //DataTable SalesReportGetByBus(DateTime from, DateTime to, Bus bus);

        [OperationContract]
        DataTable ReportStatement(StationSchedule ss, DateTime depTime);

        [OperationContract]
        DataTable ReportPassengers(DateTime? from, DateTime? to, string surname, string name, string patronymic);

        #endregion

        #region export
        [OperationContract]
        List<Vlastelin.Data.Model.ExportedData.ExportedRKO> ExportRKO(DateTime? from, DateTime? to);

        #endregion

        [OperationContract]
        List<Bus> GetAvailableBuses(StationSchedule ss, DateTime tripDate, int minSeatsCount);

        [OperationContract]
        void OperatorChangeBus(TripSchedule ts, Bus newBus, DateTime tripDate);
    }
}
