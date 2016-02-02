using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace EventManagementSystem.Models
{
    public class ReservationRequest
    {
        private int _attendeeCount;
        [NotMapped]
        public int attendeeCount
        {
            get { return _attendeeCount; }
            set { _attendeeCount = attendeeCount; }
        }

        public DateTime startTime { get; set; }
        public DateTime endTime { get; set; }
        public string repeatType { get; set; }
        public DateTime? repeatEndDate { get; set; }
        public string repeatFrequency { get; set; }
        public string repeatOption { get; set; }
        public int attendeeCountDGS { get; set; }
        public int attendeeCountNonDGS { get; set; }
        public List<string> repeatDOW { get; set; }
        public List<string> locations { get; set; }
        public List<string> options { get; set; }

        public List<ReservationViewModel> GetReservations(EventManagementSystemEntities dbEMS)
        {
            return CreateReservations(dbEMS);
        }

        private List<ReservationViewModel> CreateReservations(EventManagementSystemEntities dbEMS)
        {
            List<ReservationViewModel> reservations = new List<ReservationViewModel>();
            switch (repeatType)
            {
                case "1":
                    return CreateDailyRepeatingReservations(dbEMS);
                case "2":
                    return CreateWeeklyRepeatingReservations(dbEMS);
                case "3":
                    return CreateMonthlyRepeatingReservations(dbEMS);
                case "4":
                    return CreateYearlyRepeatingReservations(dbEMS);
                default:
                    reservations.Add(CreateSingleReservation(dbEMS, startTime, endTime));
                    break;
            }
            return reservations;
        }

        private List<ReservationViewModel> CreateYearlyRepeatingReservations(EventManagementSystemEntities dbEMS)
        {
            List<ReservationViewModel> reservations = new List<ReservationViewModel>();
            int frequency = Convert.ToInt32(repeatFrequency);
            if (repeatOption == "99") //the xx of every month
            {
                for (DateTime day = startTime.Date; day <= repeatEndDate.Value.Date; day = day.AddYears(frequency))
                {
                    reservations.Add(CreateSingleReservation(dbEMS, day.Date + startTime.TimeOfDay, day.Date + endTime.TimeOfDay));
                }
            }
            else
            {
                int xthOccurrence = Convert.ToInt16(repeatOption);
                for (DateTime day = new DateTime(startTime.Date.Year, startTime.Date.Month, 1); day <= repeatEndDate.Value.Date; day = day.AddYears(frequency))
                {
                    int firstDOMWeekDay = (int)(day.DayOfWeek);
                    int desiredDOW = Convert.ToInt16(repeatDOW[0]);
                    if (xthOccurrence < 5)
                    {
                        int jumpDays = (7 * (xthOccurrence - 1)) + (desiredDOW >= firstDOMWeekDay ? desiredDOW - firstDOMWeekDay : 7 + desiredDOW - firstDOMWeekDay);
                        reservations.Add(CreateSingleReservation(dbEMS, day.Date.AddDays(jumpDays) + startTime.TimeOfDay, day.Date.AddDays(jumpDays) + endTime.TimeOfDay));
                    }
                    else
                    {
                        DateTime lastDayOfMonth = day.AddMonths(1).AddDays(-1);
                        int lastDOMWeekDay = (int)(lastDayOfMonth.DayOfWeek);
                        int jumpDays = desiredDOW <= lastDOMWeekDay ? desiredDOW - lastDOMWeekDay : desiredDOW - lastDOMWeekDay - 7;
                        reservations.Add(CreateSingleReservation(dbEMS, lastDayOfMonth.Date.AddDays(jumpDays) + startTime.TimeOfDay, lastDayOfMonth.Date.AddDays(jumpDays) + endTime.TimeOfDay));
                    }
                }
            }
            return reservations;
        }

        private List<ReservationViewModel> CreateMonthlyRepeatingReservations(EventManagementSystemEntities dbEMS)
        {
            List<ReservationViewModel> reservations = new List<ReservationViewModel>();
            int frequency = Convert.ToInt32(repeatFrequency);
            if (repeatOption == "99") //the xx of every month
            {
                for (DateTime day = startTime.Date; day <= repeatEndDate.Value.Date; day = day.AddMonths(frequency))
                {
                    reservations.Add(CreateSingleReservation(dbEMS, day.Date + startTime.TimeOfDay, day.Date + endTime.TimeOfDay));
                }
            }
            else
            {
                int xthOccurrence = Convert.ToInt16(repeatOption);
                for (DateTime day = new DateTime(startTime.Date.Year, startTime.Date.Month, 1); day <= repeatEndDate.Value.Date; day = day.AddMonths(frequency))
                {
                    int firstDOMWeekDay = (int)(day.DayOfWeek);
                    int desiredDOW = Convert.ToInt16(repeatDOW[0]);
                    if (xthOccurrence < 5)
                    {
                        int jumpDays = (7 * (xthOccurrence - 1)) + (desiredDOW >= firstDOMWeekDay ? desiredDOW - firstDOMWeekDay : 7 + desiredDOW - firstDOMWeekDay);
                        reservations.Add(CreateSingleReservation(dbEMS, day.Date.AddDays(jumpDays) + startTime.TimeOfDay, day.Date.AddDays(jumpDays) + endTime.TimeOfDay));
                    }
                    else
                    {
                        DateTime lastDayOfMonth = day.AddMonths(1).AddDays(-1);
                        int lastDOMWeekDay = (int)(lastDayOfMonth.DayOfWeek);
                        int jumpDays = desiredDOW <= lastDOMWeekDay ? desiredDOW - lastDOMWeekDay : desiredDOW - lastDOMWeekDay - 7;
                        reservations.Add(CreateSingleReservation(dbEMS, lastDayOfMonth.Date.AddDays(jumpDays) + startTime.TimeOfDay, lastDayOfMonth.Date.AddDays(jumpDays) + endTime.TimeOfDay));
                    }
                }
            }
            return reservations;
        }

        private List<ReservationViewModel> CreateWeeklyRepeatingReservations(EventManagementSystemEntities dbEMS)
        {
            //starting this week, loop through every xx weeks and add reservation on selected days
            List<ReservationViewModel> reservations = new List<ReservationViewModel>();
            for (DateTime day = startTime.Date; day <= repeatEndDate.Value.Date; day = day.AddDays(1))
            {
                var dowNum = (int)(day.DayOfWeek);
                if (repeatDOW.Contains(dowNum.ToString()))
                {
                    reservations.Add(CreateSingleReservation(dbEMS, day.Date + startTime.TimeOfDay, day.Date + endTime.TimeOfDay));
                }
            }
            return reservations;
        }

        private List<ReservationViewModel> CreateDailyRepeatingReservations(EventManagementSystemEntities dbEMS)
        {
            List<ReservationViewModel> reservations = new List<ReservationViewModel>();
            for (DateTime day = startTime.Date; day <= repeatEndDate.Value.Date; day = day.AddDays(1))
            {
                if ((repeatOption == "on" && (day.DayOfWeek != DayOfWeek.Saturday && day.DayOfWeek != DayOfWeek.Sunday)) || repeatOption == string.Empty || repeatOption == null)
                {
                    reservations.Add(CreateSingleReservation(dbEMS, day.Date + startTime.TimeOfDay, day.Date + endTime.TimeOfDay));
                }
            }
            return reservations;
        }

        private ReservationViewModel CreateSingleReservation(EventManagementSystemEntities dbEMS, DateTime startTime, DateTime endTime)
        {
            try
            {
                
                    Reservation reservation = new Reservation
                    { 
                        startTime = startTime,
                        endTime = endTime,
                        attendeeCount = attendeeCount,
                        attendeeCountDGS = attendeeCountDGS,
                        attendeeCountNonDGS = attendeeCountNonDGS
                    };
                    if (locations != null)
                    {
                        foreach (var l in locations.Where(x => x.Length > 0))
                        {
                            Int32 locationId = Convert.ToInt32(l);
                            Location location = dbEMS.Locations.Where(ll => ll.locationId == locationId).FirstOrDefault();
                            reservation.Locations.Add(location);
                        }
                    }
                    if (options != null)
                    {
                        foreach (var o in options.Where(x => x.Length > 0))
                        {
                            Int32 optionId = Convert.ToInt32(o);
                            Option option = dbEMS.Options.Where(oo => oo.optionId == optionId).FirstOrDefault();
                            reservation.Options.Add(option);
                        }
                    }
                    ReservationViewModel rvm = new ReservationViewModel();
                    rvm.Reservation = reservation;
                    rvm.GetConflicts();
                    //reservation.GetConflicts();
                    return rvm;
                
            }
            catch (Exception ex)
            {
                return null;
            }
        }
    }
}