using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Net;
using System.Web;
using System.Web.Mvc;
using EventManagementSystem.Models;
using System.Web.Script.Serialization;
using System.IO;

namespace EventManagementSystem.Controllers
{
    public class EventsController : Controller
    {
        private EventManagementSystemEntities db = new EventManagementSystemEntities();

        // GET: Events
        public async Task<ActionResult> Index()
        {
            var events = db.Events.Include(e => e.EventStatusChoice).Include(e => e.RepeatType);
            return View(await events.ToListAsync());
        }

        // GET: Events/Details/5
        public async Task<ActionResult> Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Event @event = await db.Events.FindAsync(id);
            if (@event == null)
            {
                return HttpNotFound();
            }
            EventViewModel model = new EventViewModel
            {
                Event = @event,
                LocationList = Helpers.Helpers.GetLocationList(@event.Locations.Select(l => l.locationId).ToList()),
                OptionList = Helpers.Helpers.GetOptionList(@event.Locations.Select(l => l.locationId).ToList())
            };
            return View(model);
        }

        // GET: Events/Create
        public ActionResult Create(List<int> roomNum, List<int> options, string name, int? attendeeCountDGS, int? attendeeCountNonDGS, DateTime? startTime, DateTime? endTime)
        {
            TempData["ReturnPath"] = Request.UrlReferrer;
            EventViewModel model = new EventViewModel();
            Event e = new Event {
                attendeeCount = attendeeCountDGS + attendeeCountNonDGS,
                attendeeCountDGS = attendeeCountDGS,
                attendeeCountNonDGS = attendeeCountNonDGS,
                name = name ?? string.Empty,
                startTime = startTime ?? DateTime.Now,
                endTime = endTime ?? DateTime.Now,
                created = DateTime.Now,
                createdBy = Helpers.Helpers.GetUser(User.Identity).loginName,
                modified = DateTime.Now,
                modifiedBy = Helpers.Helpers.GetUser(User.Identity).loginName
            };
            model.Event = e;
            roomNum = roomNum ?? new List<int>();
            options = options ?? new List<int>();
            model.LocationList = Helpers.Helpers.GetLocationList(roomNum);
            model.OptionList = Helpers.Helpers.GetOptionList(options);
            //ViewBag.LocationList = new SelectList(db.Locations.OrderBy(l => l.name), "locationId", "name");
            //ViewBag.LocationList = Helpers.Helpers.GetLocationList(roomNum);
            //ViewBag.EventEventOptions = new SelectList(db.Options.OrderBy(o => o.name), "optionId", "name");
            List<SelectListItem> users = Helpers.Helpers.GetUserSelectList();
            model.UserList = users;
            //ViewBag.owner = users;
            //ViewBag.organizer = users; 
            return View(model);
        }

        // POST: Events/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public async Task<ActionResult> Create([Bind(Include = "eventId,name,owner,organizer,eventStatusId,requestDate,startTime,endTime,attendeeCount,repeatEnd,repeatDay,repeatMonth,repeatMonthWeek,repeatMonthlyWeekday,repeatYear,repeatWeek,repeatWeekday,notes,repeatTypeId,repeatIrregular")] Event @event)
        //{
        //    if (ModelState.IsValid)
        //    {
        //        db.Events.Add(@event);
        //        await db.SaveChangesAsync();
        //        return RedirectToAction("Index");
        //    }
        //    return View(@event);
        //}

        [HttpPost]
        public String AsyncCreate(EventViewModel ev, List<ReservationRequest> reservations, List<int> EventLocations, List<int> EventOption)
        {
            var returnPath = TempData["ReturnPath"] ?? Request.UrlReferrer;
            var user = Helpers.Helpers.GetUser(User.Identity).loginName;
            EventLocations = EventLocations ?? new List<int>();
            EventOption = EventOption ?? new List<int>();
            try
            {
                foreach (var r in reservations)
                {
                    foreach (var reservation in r.GetReservations(db))
                    {
                        var thisReservation = reservation.Reservation;
                        thisReservation.ReservationNotes.Add(new ReservationNote { 
                            created = DateTime.Now,
                            createdBy = user,
                            note = ev.Event.notes
                        });
                        thisReservation.created = DateTime.Now;
                        thisReservation.createdBy = user;
                        thisReservation.modified = DateTime.Now;
                        thisReservation.modifiedBy = user;
                        ev.Event.Reservations.Add(thisReservation);
                    }
                    //r.GetReservations().ForEach(rr => ev.Reservations.Add(rr.Reservation));
                }
                foreach (var location in EventLocations)
                {
                    ev.Event.Locations.Add(db.Locations.Find(location));
                }
                foreach (var option in EventOption)
                {
                    ev.Event.Options.Add(db.Options.Find(option));
                }
                db.Events.Add(ev.Event);
                db.SaveChanges();
                if (returnPath != null)
                {
                    return returnPath.ToString();
                }
                return "Success";

            }
            catch (System.Data.Entity.Validation.DbEntityValidationException dbex)
            {
                return "Oops. Looks like you're missing some information";
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }

        // GET: Events/Edit/5
        public async Task<ActionResult> Edit(int? id)
        {
            TempData["ReturnPath"] = Request.UrlReferrer;
            EventViewModel model = new EventViewModel();
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Event @event = await db.Events.FindAsync(id);
            if (@event == null)
            {
                return HttpNotFound();
            }
            model.Event = @event;
            model.UserList = Helpers.Helpers.GetUserSelectList(); ;
            return View(model);
        }

        // POST: Events/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit(EventViewModel model)
        {
            var returnPath = TempData["ReturnPath"] ?? Request.UrlReferrer;

            if (ModelState.IsValid)
            {
                db.Entry(model.Event).State = EntityState.Modified;
                await db.SaveChangesAsync();     
                return Redirect(returnPath.ToString());
            }
            return View(model);
        }

        // GET: Events/Delete/5
        public async Task<ActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Event @event = await db.Events.FindAsync(id);
            if (@event == null)
            {
                return HttpNotFound();
            }
            return View(@event);
        }

        // POST: Events/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(int id)
        {
            Event @event = await db.Events.FindAsync(id);
            db.Events.Remove(@event);
            await db.SaveChangesAsync();
            return RedirectToAction("Index");
        }

        public PartialViewResult _GenerateReservations(ReservationRequest reservationRequest)
        {
            if (reservationRequest != null)
            {
                var reservations = reservationRequest.GetReservations(db);
                return PartialView(reservations);
            }
            else
            {
                return null;
            }
        }

        public PartialViewResult _FetchReservations(int? eventId)
        {
            if (eventId != null)
            {
                Event e = db.Events.Find(eventId); 
                if (e != null)
                {
                    List<ReservationViewModel> model = new List<ReservationViewModel>();
                    foreach (var reservation in e.Reservations.OrderBy(r => r.startTime))
                    {
                        ReservationViewModel rvm = new ReservationViewModel();
                        rvm.Reservation = reservation;
                        model.Add(rvm);
                    }
                    return PartialView(model);
                }
            }
            return null;
        }


        public string _ShowConfirmationEmail(int? eventId)
        {
            var ev = db.Events.Include("Reservations").Where(e => e.eventId == eventId).FirstOrDefault();
            if (ev != null)
            {
                return RenderViewToString(ControllerContext, "~/Views/Shared/_ConfirmationEmail.cshtml", ev, true);
            }
            return string.Empty;
        }

        [ValidateInput(false)]
        [HttpPost]
        public string _SendConfirmationEmail(int? eventId, string body, string isAddOn)
        {
            //Int32 id = Convert.ToInt32(eventId);
            if (eventId != null)
            {
                var ev = db.Events.Include("Reservations").Where(e => e.eventId == eventId).FirstOrDefault();
                if (ev != null)
                {
                    bool addon = Convert.ToBoolean(isAddOn);
                    string userEmail = Helpers.Helpers.GetEmailFromUser(User.Identity);
                    JavaScriptSerializer s = new JavaScriptSerializer();

                    System.Net.Mail.MailAddressCollection to = new System.Net.Mail.MailAddressCollection();
                    to.Add(Helpers.Helpers.GetEmailFromUser(ev.organizer));
                    //to.Add("matt.turner@dgslaw.com");
                    if (addon)
                    {
                        to.Add("hospitality@dgslaw.com");
                    }
                    List<string> optionRecipients = new List<string>();
                    foreach (var reservation in ev.Reservations)
                    {
                        foreach (var option in reservation.Options)
                        {
                            foreach (var user in option.Users)
                            {
                                optionRecipients.Add(user.email);
                            }
                        }
                    }
                    foreach (var recipient in optionRecipients.Distinct())
                    {
                        to.Add(recipient);
                    }
                    System.Net.Mail.MailAddressCollection cc = new System.Net.Mail.MailAddressCollection();
                    cc.Add(userEmail);

                    System.Net.Mail.MailAddressCollection bcc = new System.Net.Mail.MailAddressCollection();
                    bcc.Add("matt.turner@dgslaw.com");
                    string from = userEmail ?? "receptionist@dgslaw.com";
                    //string body = RenderViewToString(ControllerContext, "~/Views/Shared/_ConfirmationEmail.cshtml", ev, true);
                    string subject = String.Join(" | ",
                        "EMS Reservation Confirmation: " + ev.name,
                        ev.startTime.ToShortDateString(),
                        ev.startTime.ToShortTimeString(),
                        String.Join(", ", ev.Locations.OrderBy(l => l.locationId).Select(el => el.name).Distinct()));
                    subject = addon ? "Add On: " + subject : subject;
                    Exception mailResult = Helpers.Helpers.SendEmail(from, to, cc, bcc, subject, true, body);
                    MailResult r = new MailResult();
                    if (mailResult.Message.ToString() != "success")
                    {
                        r.Class = "warning";
                        r.Message = mailResult.Message.ToString();
                    }
                    else
                    {
                        r.Class = "success";
                        r.Message = "Your mail was sent successfully";
                    }
                    return s.Serialize(r);
                }
            }

            return string.Empty;
        }

        public struct MailResult
        {
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

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
