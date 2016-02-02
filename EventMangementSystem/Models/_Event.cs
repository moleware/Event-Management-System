using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace EventManagementSystem.Models
{
    public partial class Event
    {
        public Nullable<int> attendeeCount  // Dee Dee wants to break out internal DGS employees from 
        {                                   // external guests for parking purposes.  But don't tell anyone  
            get                             // that it is for parking purposes...don't know why.
            {
                return attendeeCountDGS + attendeeCountNonDGS;
            }
                
            set { }
        }
    }
}