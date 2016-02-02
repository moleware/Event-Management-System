using EventManagementSystem.Helpers;
using EventManagementSystem.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Script.Serialization;

namespace EventManagementSystem.Controllers
{
    public class HomeController : Controller
    {
        private EventManagementSystemEntities db = new EventManagementSystemEntities();

        public ActionResult Index(DateTime? startTime, int? hoursToDisplay, int? minuteIncrements)
        {
            startTime = startTime ?? DateTime.Now.Date.AddHours(6);
            hoursToDisplay = hoursToDisplay ?? 12;
            minuteIncrements = minuteIncrements ?? 15;
            var endTime = startTime.Value.AddHours((double)hoursToDisplay);
            var locations = db.Locations.OrderBy(l => l.name);
            var reservations = db.Reservations.Include("Event").Where(r => r.startTime >= startTime && r.startTime < endTime );
            CalendarViewModel model = new CalendarViewModel() { 
                StartTime = startTime.Value,
                HoursToDisplay = hoursToDisplay.Value,
                MinuteIncrements = minuteIncrements.Value,
                Locations = locations.ToList(),
                Reservations = reservations.ToList()
            };
            return View(model);
        }

        [AuthorizeRedirect(Roles = "dgslaw\\ems_scheduling, dgslaw\\ems_hospitality")]
        public ActionResult Administration()
        {
            return View();
        }

        [AuthorizeRedirect(Roles = "dgslaw\\ems_scheduling")]
        [HttpPost]
        public PartialViewResult _Search(string terms) 
        {
            var events = db.Events.Where(e => e.organizer.ToLower().Contains(terms.ToLower()) || e.owner.ToLower().Contains(terms.ToLower()) || e.name.ToLower().Contains(terms.ToLower()));
            if (events == null || events.Count() == 0)
            {
                return PartialView("_NoSearchResults", terms);
            }
            return PartialView("_EventsSummaryView", events);
        }

        [AuthorizeRedirect(Roles = "dgslaw\\ems_scheduling")]
        [HttpPost]
        public string _SendNextDayConfirmationEmail(DateTime? date)
        {
            var tomorrow = date ?? DateTime.Now.Date.AddDays(1);
            var reservations = db.Reservations.Include("Event").ToList().Where(r => r.startTime.Date == tomorrow);
            if (reservations != null)
            {
                JavaScriptSerializer s = new JavaScriptSerializer();
                List<ReservationMailResult> results = new List<ReservationMailResult>();
                string userEmail = Helpers.Helpers.GetEmailFromUser(User.Identity);
                foreach (var r in reservations.Where(r => !r.Options.Any(ro => ro.optionId == 6))) //do not send for Out of Order rooms
                {
                    System.Net.Mail.MailAddressCollection to = new System.Net.Mail.MailAddressCollection();
                    to.Add(Helpers.Helpers.GetEmailFromUser(r.Event.organizer));
                    //to.Add("matt.turner@dgslaw.com");
                    System.Net.Mail.MailAddressCollection cc = new System.Net.Mail.MailAddressCollection();
                    cc.Add(userEmail);
                    System.Net.Mail.MailAddressCollection bcc = new System.Net.Mail.MailAddressCollection();
                    bcc.Add("matt.turner@dgslaw.com");
                    string from = userEmail ?? "receptionist@dgslaw.com";
                    string subject = String.Join(" | ",
                        "EMS Reservation Confirmation: " + r.Event.name,
                        r.startTime.ToShortDateString(),
                        r.startTime.ToShortTimeString(),
                        String.Join(", ", r.Locations.OrderBy(rl => rl.locationId).Select(rl => rl.name).Distinct())
                    );
                    string body = RenderViewToString(ControllerContext, "~/Views/Reservations/_ReservationConfirmationEmail.cshtml", r, true);
                    Exception mailResult = Helpers.Helpers.SendEmail(from, to, cc, bcc, subject, true, body);
                    ReservationMailResult mr = new ReservationMailResult();
                    if (mailResult.Message.ToString() != "success")
                    {
                        mr.Class = "warning";
                        mr.Message = mailResult.Message.ToString();
                    }
                    else
                    {
                        mr.Class = "success";
                        mr.Message = "Your mail was sent successfully";
                    }
                    mr.Recipient = Helpers.Helpers.GetUser(r.Event.organizer).shortName;
                    mr.Details = subject;
                    results.Add(mr);
                }
                return s.Serialize(results);
                //string body = RenderViewToString(ControllerContext, "~/Views/Shared/_ConfirmationEmail.cshtml", ev, true);
            }
            return string.Empty;
        }
        public struct ReservationMailResult
        {
            public string Recipient;
            public string Details;
            public string Class;
            public string Message;
        }

        static string RenderViewToString(ControllerContext context,
                                    string viewPath,
                                    object model = null,
                                    bool partial = false)
        {
            // first find the ViewEngine for this view
            ViewEngineResult viewEngineResult = null;
            if (partial)
                viewEngineResult = ViewEngines.Engines.FindPartialView(context, viewPath);
            else
                viewEngineResult = ViewEngines.Engines.FindView(context, viewPath, null);

            if (viewEngineResult == null)
                throw new FileNotFoundException("View cannot be found.");

            // get the view and attach the model to view data
            var view = viewEngineResult.View;
            context.Controller.ViewData.Model = model;

            string result = null;

            using (var sw = new StringWriter())
            {
                var ctx = new ViewContext(context, view,
                                            context.Controller.ViewData,
                                            context.Controller.TempData,
                                            sw);
                view.Render(ctx, sw);
                result = sw.ToString();
            }

            return result;
        }
    }
}