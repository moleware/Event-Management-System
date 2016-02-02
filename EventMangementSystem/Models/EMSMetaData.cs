using EventManagementSystem.Helpers;
using EventManagementSystem.Utility;
using Foolproof;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace EventManagementSystem.Models
{
    [MetadataType(typeof(ReservationMetaData))]
    public partial class Reservation
    {
    }
    public class ReservationMetaData
    {
        [NotMapped]
        [DisplayName("Total Attendee Count")]
        public object attendeeCount { get; set; }

        /*    private int _attendeeCount;
                 [NotMapped]
                      [DisplayName("Total Attendee Count")]
                      public object attendeeCount
                      {
                          get { return _attendeeCount; }  // This field is no longer mapped to the DB as Dee Dee requested that it be
                          set                             // seperated out into DGS and Non-DGS attendees it is now a calculated value.
                          {
                              if (!int.TryParse(attendeeCount.ToString(), out _attendeeCount))
                                  _attendeeCount = 0;
                          }
                      }*/

        [DisplayName("Start Time")]
        public object startTime { get; set; }

        [DisplayName("End Time")]
        public object endTime { get; set; }

        [DisplayName("DGS Attendee Count")]
        public object attendeeCountDGS { get; set; }

        [DisplayName("Non-DGS Attendee Count")]
        public object attendeeCountNonDGS { get; set; }
    }
    [MetadataType(typeof(EventMetaData))]
    public partial class Event
    {
    }
    public class EventMetaData
    {
        [NotMapped]
        [DisplayName("Total Attendee Count")]
        public object attendeeCount { get; set; }

        /*  private int _attendeeCount;

        [NotMapped]
         [DisplayName("Total Attendee Count")]
         public object attendeeCount 
         {
             get { return _attendeeCount; }  // This field is no longer mapped to the DB as Dee Dee requested that it be
             set                             // seperated out into DGS and Non-DGS attendees it is now a calculated value.
             {
                 if (!int.TryParse(attendeeCount.ToString(), out _attendeeCount))
                     _attendeeCount = 0;
             }
         }*/

        [DisplayName("Event Name")]
        [Required]
        public object name { get; set; }
        [DisplayName("DGS Attendee Count")]
        public object attendeeCountDGS { get; set; }
        [DisplayName("Non-DGS Attendee Count")]
        public object attendeeCountNonDGS { get; set; }
        [DisplayName("Notes")]
        public object notes { get; set; }
        public object eventId { get; set; }
        [DisplayName("Owner")]
        [Required]
        public object owner { get; set; }
        [DisplayName("Organizer")]
        [Required]
        public object organizer { get; set; }
        public object eventStatusId { get; set; }
        public object requestDate { get; set; }
        [DisplayName("Start Time")]
        [DisplayFormat(DataFormatString = "{0:MM/dd/yyyy hh:mm tt}", ApplyFormatInEditMode = true)]
        public object startTime { get; set; }
        [DisplayName("End Time")]
        [DisplayFormat(DataFormatString = "{0:MM/dd/yyyy hh:mm tt}", ApplyFormatInEditMode = true)]
        public object endTime { get; set; }
        public object repeatDay { get; set; }
        public object repeatMonth { get; set; }
        public object repeatYear { get; set; }
        public object repeatWeek { get; set; }
        public object repeatWeekday { get; set; }
        [DisplayName("Repeat End Date")]
        [DisplayFormat(DataFormatString = "{0:MM/dd/yyyy hh:mm tt}", ApplyFormatInEditMode = true)]
        public object repeatEnd { get; set; }
    }
}