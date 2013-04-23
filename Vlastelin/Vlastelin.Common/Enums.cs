using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Vlastelin.Common
{
    public enum ModifiedObjects
    {
        Branches,
        Buses,
        DriverAuthorities,
        Drivers,
        Operators,
        Owners,
        Passengers,
        SalesHistory,
        Seats,
        Subtrips,
        Towns,
        Trips,
        TripsSchedule,
        MainSettings
    };

    public enum ReportTypes
    {
        SalesReportFull,
        SalesReportTickets,
        SalesReportNonTickets,
        /// <summary>
        /// Param1 - Bus
        /// </summary>
        SalesReportByBus,
        /// <summary>
        /// Param1 - Trip
        /// </summary>
        SalesReportByTrip,
        RKOReport,
        PKOReport,
        ReturnedTickets
    };
}
