using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace EventManagementSystem.Models
{
    public class ReservationViewModel
    {
        public Reservation Reservation { get; set; }
        public List<Reservation> conflicts = new List<Reservation>();

        public void GetConflicts()
        {
            if (Reservation.startTime != null && Reservation.endTime != null)
            {
                using (EventManagementSystemEntities dbEMS = new EventManagementSystemEntities())
                {
                    var conflicts = dbEMS.Reservations.Include("Locations").Include("Event").Where(r => (Reservation.startTime <= r.endTime) && (Reservation.endTime >= r.startTime));
                    foreach (var reservation in conflicts)
                    {
                        bool shareLocation = false;
                        foreach (var l in reservation.Locations.Select(rl => rl.locationId))
                        {
                            if (this.Reservation.Locations.Select(trl => trl.locationId).Contains(l))
                            {
                                shareLocation = true;
                                break;
                            }
                        }
                        if (shareLocation)
                        {
                            this.conflicts.Add(reservation);
                        }
                    }
                }
            }
        }
    }
}