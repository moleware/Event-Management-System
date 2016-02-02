using Postal;

namespace EventManagementSystem.Models
{
    public class ReservationModifiedEmail
    {
        public string From { get; set; }
        public string To { get; set; }
        public string Subject { get; set; }
        public string Body { get; set; }
    }
}