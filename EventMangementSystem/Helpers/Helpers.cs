using EventManagementSystem.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Security.Principal;
using System.Web;
using System.Web.Mvc;

namespace EventManagementSystem.Helpers
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
    public class AuthorizeRedirect : AuthorizeAttribute
    {
        private const string IS_AUTHORIZED = "isAuthorized";
        public string RedirectUrl = "~/Error/Unauthorized";

        protected override bool AuthorizeCore(System.Web.HttpContextBase httpContext)
        {
            bool isAuthorized = base.AuthorizeCore(httpContext);
            if (httpContext.Items.Contains(IS_AUTHORIZED))
            {
                httpContext.Items.Remove(IS_AUTHORIZED);
            }
            httpContext.Items.Add(IS_AUTHORIZED, isAuthorized);
            return isAuthorized;
        }

        public override void OnAuthorization(AuthorizationContext filterContext)
        {
            base.OnAuthorization(filterContext);
            var isAuthorized = filterContext.HttpContext.Items[IS_AUTHORIZED] != null
                ? Convert.ToBoolean(filterContext.HttpContext.Items[IS_AUTHORIZED])
                : false;

            if (!isAuthorized && filterContext.RequestContext.HttpContext.User.Identity.IsAuthenticated)
            {
                filterContext.RequestContext.HttpContext.Response.Redirect(RedirectUrl);
            }
        }
    }

    public static class HelperExtensions
    {
        public static bool IsAdmin(this IPrincipal p)
        {
            return p.IsInRole(@"dgslaw\ems_owner");
        }
        public static bool IsManager(this IPrincipal p)
        {
            return p.IsInRole(@"dgslaw\ems_manager");
        }
        public static bool IsScheduler(this IPrincipal p)
        {
            return p.IsInRole(@"dgslaw\ems_scheduling");
        }
        public static bool IsVisitor(this IPrincipal p)
        {
            return p.IsInRole(@"dgslaw\ems_visitor");
        }
        public static bool IsHospitality(this IPrincipal p)
        {
            return p.IsInRole(@"dgslaw\ems_hospitality");
        }
    }

    
    public struct Result
    {
        public string Class;
        public string Message;
    }

    public class Helpers
    {
        public static int GetIntOrZero(int? i)
        {
            int j;
            j = int.TryParse(i.ToString(), out j) ? j : 0;
            return j;
        }

        public static int GetTotalAttendees(int? AttendeeCountDGS, int? AttendeeCountNonDGS)
        {
            return GetIntOrZero(AttendeeCountDGS) + GetIntOrZero(AttendeeCountNonDGS);
        }

        internal static Exception SendEmail(string from, string to, string cc, string subject, string body, bool isHtml)
        { 
            MailAddressCollection toList = new MailAddressCollection();
            toList.Add(to);
            MailAddressCollection ccList = new MailAddressCollection();
            toList.Add(cc);
            MailAddressCollection bccList = new MailAddressCollection();

            return SendEmail(from, toList, ccList, bccList, subject, isHtml, body);
        }
        internal static Exception SendEmail(string from, MailAddressCollection to, MailAddressCollection cc, MailAddressCollection bcc, string subject, bool isHtml, string body)
        {
            try
            {
                MailMessage mail = new MailMessage();
                SmtpClient client = new SmtpClient();
                client.Port = 25;
                client.DeliveryMethod = SmtpDeliveryMethod.Network;
                client.UseDefaultCredentials = false;
                client.Host = "172.16.0.47";
                mail.IsBodyHtml = isHtml;
                mail.From = new MailAddress(from);
                mail.Subject = subject;
                foreach (var t in to)
                {
                    mail.To.Add(t);
                }
                foreach (var c in cc)
                {
                    mail.CC.Add(c);
                }
                foreach (var b in bcc)
                {
                    mail.Bcc.Add(b);
                }
                mail.Body = body;
                client.Send(mail);
                Exception success = new Exception("success");
                return success;
            }
            catch (Exception ex)
            {
                return ex;
            }
        }
        public static Exception SendReservationModifiedEmail(Reservation reservation)
        {
            try
            {
                string subject = "Reservation for event " + reservation.Event.name + " has been modified.";
                string body = "The reservation for event " + reservation.Event.name +
                    " has been modified by " + GetUserShortName(reservation.modifiedBy) + " on " + reservation.modified.ToString() + "." +
                    "<br /><br />" +
                    "<a href='http://ems.dgslaw.com/Reservations/Details/" + reservation.reservationId + "'>Click here</a>" +
                    " for details.<br/><br/></br></br>This email was auto-generated. Please do not reply.";

                SendEmail("matt.turner@dgslaw.com", "receptionist@dgslaw.com", "matt.turner@dgslaw.com", subject, body, true);

                Exception success = new Exception("success");
                return success;
            }
            catch (Exception ex)
            {
                return ex;
            }
        }

        public static User GetUser(IIdentity i)
        {
            using (EventManagementSystemEntities dbEMS = new EventManagementSystemEntities())
            {
                var userId = GetUserIdFromIdentity(i);
                return GetUser(userId);
            }
        }
        internal static string GetUserIdFromIdentity(IIdentity i)
        {
            string username = i.Name.ToLower().Replace("dgslaw\\", "");
            return username;
        }
        internal static string GetEmailFromUser(IIdentity i)
        {
            var user = GetUser(i);
            if (user != null)
            {
                return user.email;
            }
            return null;
        }
        public static T Max<T>(T value1, T value2) where T : IComparable
        {
            return value1.CompareTo(value2) > 0 ? value1 : value2;
        }
        public static DateTime RoundUp(DateTime dt, TimeSpan d)
        {
            return new DateTime(((dt.Ticks + d.Ticks - 1) / d.Ticks) * d.Ticks);
        }


        public static User GetUser(string loginName)
        {
            using (EventManagementSystemEntities dbEMS = new EventManagementSystemEntities())
            {
                var user = dbEMS.Users.Where(u => u.loginName.ToLower() == loginName).FirstOrDefault();
                if (user != null)
                {
                    return user;
                }
                return null;
            }
        }

        public static string GetUserShortName(string loginName)
        {
            var user = GetUser(loginName);
            if (user != null)
            {
                return String.Join(", ", user.lastName, user.firstName);
            }
            return "Unknown User (" + loginName + ")";
        }

        public static bool ReservationHasOption(Reservation currentReservation, int p)
        {
            return currentReservation.Options.Select(ro => ro.optionId).Contains(p);
        }
        public static bool ReservationHasOption(Reservation currentReservation, string p)
        {
            return currentReservation.Options.Select(ro => ro.name).Contains(p);
        }


        internal static string GetEmailFromUser(string p)
        {
            var user = GetUser(p);
            if (user != null)
            {
                return user.email;
            }
            return null;
        }

        internal static List<SelectListItem> GetLocationList(List<int> roomNum)
        {
            using (EventManagementSystemEntities dbEMS = new EventManagementSystemEntities())
            {
                var ul = dbEMS.Locations.OrderBy(l => l.name).ToList().Select(l =>
                    new SelectListItem
                    {
                        Value = l.locationId.ToString(),
                        Text = l.name,
                        Selected = roomNum.Contains(l.locationId)
                    });
                return ul.ToList();
            }
        }
        internal static List<SelectListItem> GetOptionList(List<int> option)
        {
            using (EventManagementSystemEntities dbEMS = new EventManagementSystemEntities())
            {
                var ul = dbEMS.Options.OrderBy(o => o.name).ToList().Select(o =>
                    new SelectListItem
                    {
                        Value = o.optionId.ToString(),
                        Text = o.name,
                        Selected = option.Contains(o.optionId)
                    });
                return ul.ToList();
            }
        }

        internal static Log WriteLog(IPrincipal User, string action, Reservation reservation)
        {
            Log l = new Log();
            l.userId = User.Identity.Name.ToString();
            l.action = action;
            var data = new
            {
                created = reservation.created,
                createdBy = reservation.createdBy,
                endTime = reservation.endTime,
                eventId = reservation.eventId,
                modified = reservation.modified,
                modifiedBy = reservation.modifiedBy,
                ReservationConsumables = reservation.ReservationConsumables.Select(r => r.consumableId),
                reservationId = reservation.reservationId,
                ReservationLocations = reservation.Locations.Select(rl => rl.locationId),
                ReservationParticipants = reservation.Users.Select(rp => rp.userId),
                ReservationResources = reservation.Resources.Select(rr => rr.resourceId),
                startTime = reservation.startTime
            };

            l.data = Newtonsoft.Json.JsonConvert.SerializeObject(data, Formatting.Indented,
                new JsonSerializerSettings
                {
                    ReferenceLoopHandling = ReferenceLoopHandling.Ignore
                });
            return l;
        }
        internal static Log WriteLog(IPrincipal User, string action, string model)
        {
            Log l = new Log();
            l.userId = User.Identity.Name.ToString();
            l.action = action;
            l.data = model;

            return l;
        }


        internal static dynamic GetReservationOptions(List<int> options)
        {
            using (EventManagementSystemEntities dbEMS = new EventManagementSystemEntities())
            {
                var ul = dbEMS.Options.OrderBy(o => o.name).ToList().Select(o =>
                    new SelectListItem
                    {
                        Value = o.optionId.ToString(),
                        Text = o.name,
                        Selected = options.Contains(o.optionId)
                    });
                return ul.ToList();
            }
        }

        internal static List<SelectListItem> GetUserSelectList()
        {
            using (EventManagementSystemEntities dbEMS = new EventManagementSystemEntities())
            {
                var userList = dbEMS.Users.Where(u => u.isActive.HasValue && u.isActive.Value).OrderBy(u => u.lastName).Select(u => new { loginName = u.loginName, displayName = u.lastName + ", " + u.firstName });
                List<SelectListItem> users = new SelectList(userList, "loginName", "displayName").ToList();
                users.Insert(0, (new SelectListItem { Text = "", Value = "" }));
                return users;
            }
        }
    }
}