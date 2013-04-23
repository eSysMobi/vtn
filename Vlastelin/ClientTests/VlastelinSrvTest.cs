using VlastelinServer;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using Vlastelin.Data.Model;
using System.Collections.Generic;

namespace ServerTests
{
    
    
    /// <summary>
    ///This is a test class for VlastelinSrvTest and is intended
    ///to contain all VlastelinSrvTest Unit Tests
    ///</summary>
    [TestClass()]
    public class VlastelinSrvTest
    {
        private delegate List<T> getMethod<T>(long? id);
        private delegate long addMethod<T>(T item);
        private delegate void editMethod<T>(T item);
        private delegate void deleteMethod<T>(T item);

        VlastelinSrv target;
        private TestContext testContextInstance;

        public VlastelinSrvTest()
        {
            target = new VlastelinSrv();
        }

        /// <summary>
        ///Gets or sets the test context which provides
        ///information about and functionality for the current test run.
        ///</summary>
        public TestContext TestContext
        {
            get
            {
                return testContextInstance;
            }
            set
            {
                testContextInstance = value;
            }
        }

        #region Additional test attributes
        // 
        //You can use the following additional attributes as you write your tests:
        //
        //Use ClassInitialize to run code before running the first test in the class
        //[ClassInitialize()]
        //public static void MyClassInitialize(TestContext testContext)
        //{
        //}
        //
        //Use ClassCleanup to run code after all tests in a class have run
        //[ClassCleanup()]
        //public static void MyClassCleanup()
        //{
        //}
        //
        //Use TestInitialize to run code before running each test
        //[TestInitialize()]
        //public void MyTestInitialize()
        //{
        //}
        //
        //Use TestCleanup to run code after each test has run
        //[TestCleanup()]
        //public void MyTestCleanup()
        //{
        //}
        //
        #endregion


        /// <summary>
        ///A test for Branches
        ///</summary>
        [TestMethod()]
        public void BranchTests()
        {
            Branch etalon = new Branch()
            {
                Name = "Тилимилитрямдский филиал",
                Town = new Town() { Name = "Саратов", Id = 1 },
                townId = 1,
                Address = "Saratoff Astrahanskaya 1",
                Phone = "003003"
            };

            Branch modified = new Branch()
            {
                Name = "Тилимилитрямдский филиал 123",
                townId = 2,
                Town = new Town() { Name = "Саратов", Id = 1 },
                Address = "Saratoff Astra 112",
                Phone = "003333003"
            };

            SimpleOperationsTest<Branch>(
                etalon, 
                modified,
                target.BranchesGet, 
                target.BranchAdd, 
                target.BranchEdit, 
                target.BranchDelete);
        }

        /// <summary>
        ///A test for Buses
        ///</summary>
        [TestMethod()]
        public void BusTests()
        {
            Bus etalon = new Bus()
            {
                Manufacter="Ябадабаду",
                Model="CX-WW-22",
                Owner = new Owner(){ Name="123"},
                OwnerId=1,
                RegNumber="ан123 64rus",
                PassengersCount=12
            };

            Bus modified = new Bus()
            {
                Manufacter = "Мерс",
                Model = "600",
                Owner = new Owner() { Name = "123" },
                OwnerId = 1,
                RegNumber = "ее333 64rus",
                PassengersCount = 7
            };

            SimpleOperationsTest<Bus>(
                etalon, 
                modified, 
                target.BusesGet, 
                target.BusAdd, 
                target.BusEdit, 
                target.BusDelete);
        }

        /// <summary>
        ///A test for Drivers
        ///</summary>
        [TestMethod()]
        public void DriversTests()
        {
            Driver etalon = new Driver()
            {
                Name = "Иван",
                Surname = "Иванов",
                Patronymic = "Иванович",
                PassportSer = "1234",
                PassportNum = "123456",
                PassportIssuer = "УВД местное",
                PassportDate = DateTime.Today.AddYears(-5)
            };

            Driver modified = new Driver()
            {
                Name = "Семен",
                Surname = "Петрович",
                Patronymic = "Зильберштейн",
                PassportSer = "5432",
                PassportNum = "333322",
                PassportIssuer = "УВД неместное",
                PassportDate = DateTime.Today.AddYears(-2).AddDays(-3)
            };

            SimpleOperationsTest<Driver>(
                etalon,
                modified,
                target.DriversGet,
                target.DriverAdd,
                target.DriverEdit,
                target.DriverDelete);
        }

        /// <summary>
        ///A test for Operators
        ///</summary>
        [TestMethod()]
        public void OperatorsTests()
        {
            Operator etalon = new Operator()
            {
                Branch = new Branch() { Name = "Саратовский" },
                branchId=21,
                Name="Мария",
                Surname="Сыбындыц",
                Patronymic="Никитишна",
            };

            Operator modified = new Operator()
            {
                Branch = new Branch() { Name = "Саратовский" },
                branchId = 21,
                Name = "Сефирот",
                Surname = "Ололоев",
                Patronymic = "Евграфьевич",
            };
            /*
            SimpleOperationsTest<Operator>(
                etalon,
                modified,
                target.OperatorsGet,
                target.OperatorAdd,
                target.OperatorEdit,
                target.OperatorDelete);*/
        }

        /// <summary>
        ///A test for Towns
        ///</summary>
        [TestMethod()]
        public void TownsTests()
        {
            Town etalon = new Town()
            {
                Name="Сибирь-за-ногу-задерищенко",
                Prefix="Сибр-",
                LastNumber=0
            };

            Town modified = new Town()
            {
                Name = "Ололоевск",
                Prefix = "Ололо-",
                LastNumber = 123
            };

            SimpleOperationsTest<Town>(
                etalon,
                modified,
                target.TownsGet,
                target.TownAdd,
                target.TownEdit,
                target.TownDelete);
        }

        /// <summary>
        ///A test for Trips
        ///</summary>
        [TestMethod()]
        public void TripsTests()
        {
            Trip etalon = new Trip()
            {
                _arrivalId=1,
                _departureId=2,
                Arrival=new Town(),
                Departure=new Town(),
                //Bus=new Bus(),
                //BusId=7,
                Description="Тестовый маршрут"
            };

            Trip modified = new Trip()
            {
                _arrivalId = 3,
                _departureId = 4,
                Arrival = new Town(),
                Departure = new Town(),
                //Bus = new Bus(),
                //BusId = 7,
                Description = "Тестовый маршрут12123123123"
            };

            SimpleOperationsTest<Trip>(
                etalon,
                modified,
                target.TripsGet,
                target.TripAdd,
                target.TripEdit,
                target.TripDelete);
        }

        /// <summary>
        ///A test for TripSchedule
        ///</summary>
        [TestMethod()]
        public void TripScheduleTests()
        {
            TripSchedule etalon = new TripSchedule()
            {
                tripId = 1,
                ScheduleType = TripScheduleType.Daily,
                StartTime = DateTime.Today.AddYears(-2).AddMonths(2),
                EndTime = DateTime.Today.AddYears(100)
            };

            TripSchedule modified = new TripSchedule()
            {
                tripId = 2,
                ScheduleType = TripScheduleType.Daily,
                StartTime = DateTime.Today.AddYears(-4).AddMonths(-2).AddDays(12),
                EndTime = DateTime.Today.AddYears(10)
            };

            SimpleOperationsTest<TripSchedule>(
                etalon,
                modified,
                target.TripScheduleGet,
                target.TripScheduleAdd,
                target.TripScheduleEdit,
                target.TripScheduleDelete);
        }

        /// <summary>
        ///A test for Owners
        ///</summary>
        [TestMethod()]
        public void OwnersTests()
        {
            Owner etalon = new Owner()
            {
                _dirPosition = 1,
                _feeType = 1,
                Address = "Хвалынская 12",
                DirName = "Петенька",
                DirSurname = "Семенов",
                DirPatronymic = "Сергеич",
                DirPosition = new DirPosition() { Id=1 },
                DocDate=DateTime.Today.AddDays(-5),
                DocNum="123",
                FeeAmount=10,
                INN="1234567890",
                Name="ИП Залялов ЗГ",
                NumSv="12345",
                OGRN="123456789012345"
            };

            Owner modified = new Owner()
            {
                _dirPosition = 1,
                _feeType = 1,
                Address = "еврейская 12",
                DirName = "Тролль",
                DirSurname = "Тауренов",
                DirPatronymic = "Оркович",
                DirPosition = new DirPosition() { Id = 1 },
                DocDate = DateTime.Today.AddDays(-10),
                DocNum = "12345",
                FeeAmount = 100,
                INN = "0987654321",
                Name = "ЮЛ1",
                NumSv = "378623456234",
                OGRN = "543210987654321"
            };

            SimpleOperationsTest<Owner>(
                etalon,
                modified,
                target.OwnersGet,
                target.OwnerAdd,
                target.OwnerEdit,
                target.OwnerDelete);
        }

        [TestMethod()]
        public void MainSettingsTests()
        {
            MainSettings backup;
            MainSettings testValue = new MainSettings()
            {
                OrganizationName = "ЗАО ТрансМетанГазНефтьМясЗолотоАлмазКирпич",
                OrganizationDirName = "Иван123",
                OrganizationDirSurname = "Прокопенко",
                OrganizationDirPatronymic = "Палыч",
                OrganizationINN = "1234567890",
                OrganizationKPP = "1234568900",
                OrganizationCorrAccount = "12345678901234567890"
            };
            MainSettings controlValue;
            backup = target.MainSettingsGet();
            Assert.IsNotNull(backup);

            target.MainSettingsEdit(testValue);
            controlValue=target.MainSettingsGet();
            Assert.AreEqual(testValue, controlValue);

            target.MainSettingsEdit(backup);
        }

        /// <summary>
        /// Прикольный шаблонный метод для проверки основного функционала справочников:
        /// медотов get, add,delete и edit
        /// </summary>
        /// <typeparam name="T">тип зрачения</typeparam>
        /// <param name="etalon">первое значение, для доббавления в базу</param>
        /// <param name="modified">модифицированное значение для edit</param>
        /// <param name="get"></param>
        /// <param name="add"></param>
        /// <param name="edit"></param>
        /// <param name="delete"></param>
        private void SimpleOperationsTest<T>(
            T etalon,
            T modified,
            getMethod<T> get,
            addMethod<T> add,
            editMethod<T> edit,
            deleteMethod<T> delete)
        {
            long id = 0;
            // ADD
            try
            {
                id = add(etalon);
                Assert.AreNotEqual(id, 0, "{0} - id = 0", add.Method.Name);
                (etalon as BaseItem).Id = id;
            }
            catch (Exception ex) { Assert.Fail("{0} fails: {1}", add.Method.Name, ex.Message); }

            // GET
            try
            {
                List<T> list = get(id);
                Assert.IsNotNull(list, "{0} returned null",get.Method.Name);
                Assert.AreEqual(list.Count, 1, "{0} returned <> 1 items by ID", get.Method.Name);
                Assert.AreEqual(list[0], etalon, "{0} returned wrong data", get.Method.Name);
            }
            catch (Exception ex) { Assert.Fail("{0} fails: {1}", get.Method.Name, ex.Message); }

            // EDIT
            try
            {
                (modified as BaseItem).Id = id;
                edit(modified);
                List<T> list = get(id);
                Assert.IsNotNull(list, "after {0} {1} returned null", edit.Method.Name, get.Method.Name);
                Assert.AreEqual(list[0], modified, "after {0} {1} returned wrong data", edit.Method.Name, get.Method.Name);
            }
            catch (Exception ex) { Assert.Fail("{0} fails: {1}", edit.Method.Name, ex.Message); }

            // DELETE
            try
            {
                delete(modified);
                List<Branch> list = target.BranchesGet(id);
                Assert.IsNotNull(list, "after {0} {1} returned null",delete.Method.Name, get.Method.Name);
                Assert.AreEqual(list.Count, 0, "after {0} {1} returned data",delete.Method.Name, get.Method.Name);
            }
            catch (Exception ex) { Assert.Fail("{0} fails: {1}",delete.Method.Name, ex.Message); }
        }
    }
}
