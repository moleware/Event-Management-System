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
using EventManagementSystem.Helpers;
using System.Web.Script.Serialization;
using System.IO;

namespace EventManagementSystem.Controllers
{
    public class ReservationsController : Controller
    {
        private EventManagementSystemEntities db = new EventManagementSystemEntities();
        JavaScriptSerializer s = new JavaScriptSerializer();


        // GET: Reservations
        //public async Task<ActionResult> Index()
        //{
        //    var reservations = db.Reservations.Include(r => r.Event);
        //    return View(await reservations.ToListAsync());
        //}

        // GET: Reservations/Details/5
        
        public async Task<ActionResult> Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Reservation reservation = await db.Reservations.FindAsync(id);
            if (reservation == null)
            {
                return HttpNotFound();
            }
            return View(reservation);
        }

        public ActionResult Create(int? eventId)
        {
            TempData["ReturnPath"] = Request.UrlReferrer;
            if (eventId == null)
            {
                return Redirect(Request.UrlReferrer.ToString());
            }
            var ev = db.Events.Find(eventId);
            if (ev == null)
            {
                return Redirect(Request.UrlReferrer.ToString());
            }
            
            var reservation = new Reservation { 
                Event = ev,
                startTime = ev.startTime,
                endTime = ev.endTime,
                attendeeCount = ev.attendeeCount,
                attendeeCountDGS = ev.attendeeCountDGS,
                attendeeCountNonDGS = ev.attendeeCountNonDGS,
                Locations = ev.Locations,
                Options = ev.Options,
                created = DateTime.Now,
                createdBy = Helpers.Helpers.GetUser(User.Identity).loginName,
                modified = DateTime.Now,
                modifiedBy = Helpers.Helpers.GetUser(User.Identity).loginName
            };
            ViewBag.Options = Helpers.Helpers.GetReservationOptions(ev.Options.Select(ro => ro.optionId).ToList());
            ViewBag.LocationList = Helpers.Helpers.GetLocationList(ev.Locations.Select(rl => rl.locationId).ToList());
            ViewBag.eventId = eventId;
            return View(reservation);
        }

        //// POST: Reservations/Create
        //// To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        //// more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public async Task<ActionResult> Create([Bind(Include = "reservationId,eventId,startTime,endTime,created,modified,createdBy,modifiedBy")] Reservation reservation)
        //{
        //    if (ModelState.IsValid)
        //    {
        //        db.Reservations.Add(reservation);
        //        await db.SaveChangesAsync();
        //        return RedirectToAction("Index");
        //    }

        //    ViewBag.eventId = new SelectList(db.Events, "eventId", "name", reservation.eventId);
        //    return View(reservation);
        //}

        // GET: Reservations/Edit/5
        
        [AuthorizeRedirect(Roles = "dgslaw\\ems_scheduling")]
        public async Task<ActionResult> Edit(int? id)
        {
            TempData["ReturnPath"] = Request.UrlReferrer;

            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Reservation reservation = await db.Reservations.FindAsync(id);
            if (reservation == null)
            {
                return HttpNotFound();
            }
            ViewBag.Options = Helpers.Helpers.GetReservationOptions(reservation.Options.Select(ro => ro.optionId).ToList());
            ViewBag.LocationList = Helpers.Helpers.GetLocationList(reservation.Locations.Select(rl => rl.locationId).ToList());
            ViewBag.eventId = new SelectList(db.Events, "eventId", "name", reservation.eventId);
            return View(reservation);
        }

        [AuthorizeRedirect(Roles = "dgslaw\\ems_scheduling, dgslaw\\ems_hospitality")]
        public async Task<ActionResult> HospitalityEdit(int? id)
        {
            TempData["ReturnPath"] = Request.UrlReferrer;

            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Reservation reservation = await db.Reservations.FindAsync(id);
            if (reservation == null)
            {
                return HttpNotFound();
            }
            ViewBag.Options = Helpers.Helpers.GetReservationOptions(reservation.Options.Select(ro => ro.optionId).ToList());
            //ViewBag.LocationList = Helpers.Helpers.GetLocationList(reservation.Locations.Select(rl => rl.locationId).ToList());
            //ViewBag.eventId = new SelectList(db.Events, "eventId", "name", reservation.eventId);
            return View(reservation);
        }


        // POST: Reservations/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public async Task<ActionResult> Edit([Bind(Include = "reservationId,eventId,startTime,endTime,created,modified,createdBy,modifiedBy")] Reservation reservation, List<int> ReservationLocation)
        //{
        //    var returnPath = TempData["ReturnPath"] ?? Request.UrlReferrer;

        //    if (ModelState.IsValid)
        //    {
        //        if (ReservationLocation != null)
        //        {
        //            var existingReservation = db.Reservations.Where(rl => rl.reservationId == reservation.reservationId);
        //            foreach (var er in existingReservation)
        //            {
        //                er.Locations.Clear();
        //            }
        //            foreach (var rl in ReservationLocation)
        //            {
        //                Location location = db.Locations.Where(ll => ll.locationId == rl).FirstOrDefault();
        //                reservation.Locations.Add(location);
        //            }
        //        }
        //        db.Logs.Add(Helpers.Helpers.WriteLog(User, "ReservationsController:edit", reservation));
        //        db.SaveChanges();
        //        return Redirect(returnPath.ToString());
        //    }
        //    ViewBag.eventId = new SelectList(db.Events, "eventId", "name", reservation.eventId);
        //    return View(reservation);
        //}

        [HttpPost]
        public String AsyncEdit(Reservation reservation, List<int> ReservationLocation, List<int> ReservationOption)
        {
            var returnPath = TempData["ReturnPath"] ?? Request.UrlReferrer;
            if (ReservationLocation == null || ReservationLocation.Count() == 0)
            {
                return s.Serialize(new { @class = "warning", message = "At least one location is required" });
            }
            if (reservation.Event == null)
            {
                reservation.Event = db.Events.Find(reservation.eventId);
            }
            if (ModelState.IsValid)
            {
                try
                {
                    db.Entry(reservation).State = EntityState.Modified;
                    var existingLocations = db.Reservations.Include("Locations").Where(r => r.reservationId == reservation.reservationId).FirstOrDefault().Locations.ToList();
                    existingLocations.ForEach(erl =>
                        reservation.Locations.Add(erl)
                        );
                    foreach (var location in db.Locations)
                    {
                        if (ReservationLocation == null || (reservation.Locations.Contains(location) && !ReservationLocation.Contains(location.locationId)))
                        {
                            reservation.Locations.Remove(location);
                        }
                        if (!reservation.Locations.Contains(location) && ReservationLocation.Contains(location.locationId))
                        {
                            reservation.Locations.Add(location);
                        }
                    }

                    var existingOptions = db.Reservations.Include("Options").Where(r => r.reservationId == reservation.reservationId).FirstOrDefault().Options.ToList();
                    existingOptions.ForEach(ero => reservation.Options.Add(ero));
                    foreach (var option in db.Options)
                    {
                        if (ReservationOption == null || (reservation.Options.Contains(option) && !ReservationOption.Contains(option.optionId)))
                        {
                            reservation.Options.Remove(option);
                        }
                        else if (!reservation.Options.Contains(option) && ReservationOption.Contains(option.optionId))
                        {
                            reservation.Options.Add(option);
                        }
                    }
                    reservation.modified = DateTime.Now;
                    reservation.modifiedBy = Helpers.Helpers.GetUser(User.Identity).loginName;
                    db.Logs.Add(Helpers.Helpers.WriteLog(User, "ReservationsController:edit", reservation));
                    db.SaveChanges();


                    // This is where we send the reservation modified email
                    Helpers.Helpers.SendReservationModifiedEmail(reservation);


                    if (returnPath != null)
                    {
                        return s.Serialize(new { @class = "success", message = "Reservation Updated", returnpath = returnPath });
                    }
                    return s.Serialize(new { @class = "success", message = "Reservation Updated" });
                }
                catch (Exception ex)
                {
                    return s.Serialize(new { @class = "warning", message = ex.Message.ToString() });
                }
            }
            return "Model not valid";
        }

        [HttpPost]
        public String AsyncHospitalityEdit(Reservation reservation, List<int> ReservationOption)
        {
            var returnPath = TempData["ReturnPath"] ?? Request.UrlReferrer;
            var existingReservation = db.Reservations.Find(reservation.reservationId);
            
            if (existingReservation != null)
            {
                try
                {
                    if (reservation.attendeeCount != null && reservation.attendeeCount != existingReservation.attendeeCount)
                    {
                        existingReservation.attendeeCount = reservation.attendeeCount;
                    }
                    if (reservation.attendeeCountDGS != null && reservation.attendeeCountDGS != existingReservation.attendeeCountDGS)
                    {
                        existingReservation.attendeeCountDGS = reservation.attendeeCountDGS;
                    }
                    if (reservation.attendeeCountNonDGS != null && reservation.attendeeCountNonDGS != existingReservation.attendeeCountNonDGS)
                    {
                        existingReservation.attendeeCountNonDGS = reservation.attendeeCountNonDGS;
                    }
                    //db.Entry(existingReservation).State = EntityState.Modified;

                    //var existingOptions = db.Reservations.Include("Options").Where(r => r.reservationId == existingReservation.reservationId).FirstOrDefault().Options.ToList();
                    //existingOptions.ForEach(ero => reservation.Options.Add(ero));
                    foreach (var option in db.Options)
                    {
                        if (ReservationOption == null || (existingReservation.Options.Contains(option) && !ReservationOption.Contains(option.optionId)))
                        {
                            existingReservation.Options.Remove(option);
                        }
                        else if (!existingReservation.Options.Contains(option) && ReservationOption.Contains(option.optionId))
                        {
                            existingReservation.Options.Add(option);
                        }
                    }
                    db.Logs.Add(Helpers.Helpers.WriteLog(User, "ReservationsController:edit", existingReservation));
                    db.SaveChanges();
                    if (returnPath != null)
                    {
                        return s.Serialize(new { @class = "success", message = "Reservation Updated", returnpath = returnPath });
                    }
                    return s.Serialize(new { @class = "success", message = "Reservation Updated" });
                }
                catch (Exception ex)
                {
                    return s.Serialize(new { @class = "warning", message = ex.Message.ToString() });
                }
            }
            return "Model not valid";
        }

        [HttpPost]
        public String AsyncCreate(Reservation reservation, List<int> ReservationLocation, List<int> ReservationOption, string NewNote)
        {
            try 
            {
                var returnPath = TempData["ReturnPath"] ?? Request.UrlReferrer;

                if (ReservationLocation == null || ReservationLocation.Count() == 0)
                {
                    return s.Serialize(new { @class = "warning", message = "At least one location is required" });
                }
                if (reservation.eventId == 0)
                {
                    return s.Serialize(new { @class = "warning", message = "Event must already exist" });
                }
                var ev = db.Events.Find(reservation.eventId);
                if (ev == null)
                {
                    return s.Serialize(new { @class = "warning", message = "Event must already exist" });
                }

                foreach (var location in ReservationLocation)
                {
                    var loc = db.Locations.Find(location);
                    reservation.Locations.Add(loc);
                }
                if (ReservationOption != null)
                {
                    foreach (var option in ReservationOption)
                    {
                        var opt = db.Options.Find(option);
                        reservation.Options.Add(opt);
                    }
                }
                if (NewNote != null && NewNote != string.Empty)
                {
                    var note = new ReservationNote { note = NewNote, createdBy = reservation.createdBy, created = reservation.created.Value };
                    reservation.ReservationNotes.Add(note);
                }

                db.Reservations.Add(reservation);
                //db.Logs.Add(Helpers.Helpers.WriteLog(User, "CreateReservationFromExistingEvent", reservation));
                db.SaveChanges();
                if (returnPath != null)
                {
                    return s.Serialize(new { @class = "success", message = "Reservation Created", returnpath = returnPath });
                }
                return s.Serialize(new { @class = "success", message = "Reservation Created" });
            }
            catch (Exception ex)
            {
                return s.Serialize(new { @class = "warning", message = ex.Message.ToString() });
            }
        }

        [HttpPost]
        public String AsyncDelete(Reservation reservation)
        {
            var returnPath = TempData["ReturnPath"] ?? Request.UrlReferrer;
            try
            {
                db.Entry(reservation).State = EntityState.Deleted;
                db.Logs.Add(Helpers.Helpers.WriteLog(User, "ReservationsController:delete", reservation));
                db.SaveChanges();
                if (returnPath != null)
                {
                    return s.Serialize(new { @class = "success", message = "Reservation Deleted", returnpath = returnPath });
                }
                return s.Serialize(new { @class = "success", message = "Reservation Deleted" });
            }
            catch (Exception ex)
            {
                return s.Serialize(new { @class = "warning", message = ex.Message.ToString() });
            }
        }

        public ActionResult ListView(DateTime? startDate, DateTime? endDate, List<string> filter)
        {
            var reservations = db.Reservations.Include("Event").ToList();

            if (startDate.HasValue)
            {
                reservations = reservations.Where(r => r.startTime >= startDate).ToList();
            }
            if (endDate.HasValue)
            {
                reservations = reservations.Where(r => r.endTime <= endDate).ToList();
            }
            if (filter != null && filter.Count() > 0)
            {
                int[] optionsList = filter.Select(f => Convert.ToInt32(f)).ToArray();
                reservations = reservations.Where(r => optionsList.Any(o => r.Options.Select(ro => ro.optionId).Contains(o))).ToList();
            }

            return View(reservations.OrderBy(r => r.startTime));
        }

        // GET: Reservations/Delete/5
        //public async Task<ActionResult> Delete(int? id)
        //{
        //    if (id == null)
        //    {
        //        return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
        //    }
        //    Reservation reservation = await db.Reservations.FindAsync(id);
        //    if (reservation == null)
        //    {
        //        return HttpNotFound();
        //    }
        //    return View(reservation);
        //}

        //// POST: Reservations/Delete/5
        //[HttpPost, ActionName("Delete")]
        //[ValidateAntiForgeryToken]
        //public async Task<ActionResult> DeleteConfirmed(int id)
        //{
        //    Reservation reservation = await db.Reservations.FindAsync(id);
        //    db.Reservations.Remove(reservation);
        //    await db.SaveChangesAsync();
        //    return RedirectToAction("Index");
        //}

        [HttpPost]
        [AuthorizeRedirect(Roles = "dgslaw\\ems_scheduling")]
        public string _DeleteReservationNote(int? id)
        {
            try {
                if (id.HasValue)
                {
                    ReservationNote note = db.ReservationNotes.Find(id);
                    db.ReservationNotes.Remove(note);
                    db.SaveChanges();
                    return s.Serialize(new { @class = "success", message = "Note deleted" });
                }
                else
                {
                    return s.Serialize(new { @class = "warning", message = "Unable to find note with this ID" });
                }
            }
            catch (Exception ex)
            {
                return s.Serialize(new { @class = "warning", message = ex.Message.ToString() });
            }
        }

        [AuthorizeRedirect(Roles = "dgslaw\\ems_scheduling, dgslaw\\ems_hospitality")]
        [HttpPost]
        public string _AddNote(int? id, DateTime? date, string note)
        {
            JavaScriptSerializer s = new JavaScriptSerializer();
            if (id != null)
            {
                var reservation = db.Reservations.Find(id);
                if (reservation != null)
                {
                    try
                    {
                        ReservationNote n = new ReservationNote();
                        n.created = date ?? DateTime.Now;
                        n.note = note;
                        n.createdBy = Helpers.Helpers.GetUser(User.Identity).loginName;
                        n.reservationId = reservation.reservationId;
                        reservation.ReservationNotes.Add(n);
                        Log l = new Log();
                        l.userId = Helpers.Helpers.GetUser(User.Identity).loginName;
                        l.action = "Add Note";
                        l.data = s.Serialize(n);
                        db.Logs.Add(l);
                        db.SaveChanges();
                        return s.Serialize(new { @class = "success", message = "Note saved successfully" });
                    }
                    catch (Exception ex)
                    {
                        return s.Serialize(new { @class = "alert", message = ex.Message.ToString() });
                    }
                }
            }
            return null;

        }
        
        [HttpGet]
        public PartialViewResult _GetNotes(int? id)
        {
            if (id != null)
            {
                var reservationNotes = db.ReservationNotes.Where(rn => rn.reservationId == id);
                if (reservationNotes != null)
                {
                    return PartialView(reservationNotes.OrderByDescending(rn => rn.created));
                }
            }
            return null;
        }

        [HttpGet]
        public string _ReservationConfirmationEmail(int? id)
        {
            if (id != null)
            {
                var reservation = db.Reservations.Include("Event").Where(r => r.reservationId == id).FirstOrDefault();
                if (reservation != null)
                {
                    return RenderViewToString(ControllerContext, "~/Views/Reservations/_ReservationConfirmationEmail.cshtml", reservation, true);
                }
            }
            return null;
        }

        [HttpGet]
        public string _FindRecipients(string q)
        {
            var recipients = db.Users.Where(u => u.shortName.ToLower().Contains(q.ToLower()) || u.loginName.ToLower() == q.ToLower());
            List<object> recipientList = new List<object>();
            if (recipients.Count() > 0)
            {
                JavaScriptSerializer s = new JavaScriptSerializer();
                foreach (var recipient in recipients)
                {
                    var recipientObject = new {
                        name = recipient.shortName,
                        email = recipient.email
                    };
                    recipientList.Add(recipientObject);
                }
                return s.Serialize(recipientList);
            }
            return null;
            
        }

        [ValidateInput(false)]
        public string _SendConfirmationEmail(int? reservationId, string body, string isAddOn, string[] recipients)
        {
            //Int32 id = Convert.ToInt32(reservationId);
            var reservation = db.Reservations.Include("Event").Where(r => r.reservationId == reservationId).FirstOrDefault();
            if (reservation != null)
            {
                bool addon = Convert.ToBoolean(isAddOn);
                string userEmail = Helpers.Helpers.GetEmailFromUser(User.Identity);
                JavaScriptSerializer s = new JavaScriptSerializer();

                System.Net.Mail.MailAddressCollection to = new System.Net.Mail.MailAddressCollection();
                if (recipients != null && recipients.Length > 0)
                {
                    foreach (var recipient in recipients)
                    {
                        to.Add(recipient);
                    }
                }
                else
                {
                    //to.Add(Helpers.Helpers.GetEmailFromUser(reservation.Event.organizer));

                }
                //to.Add("matt.turner@dgslaw.com");
                if (addon)
                {
                    to.Add("hospitality@dgslaw.com");
                }
                List<string> optionRecipients = new List<string>();
                foreach (var option in reservation.Options)
                {
                    foreach (var user in option.Users)
                    {
                        optionRecipients.Add(user.email);
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
                string subject = String.Join(" | ",
                    "EMS Reservation Confirmation: " + reservation.Event.name,
                    reservation.startTime.ToShortDateString(),
                    reservation.startTime.ToShortTimeString(),
                    String.Join(", ", reservation.Locations.OrderBy(rl => rl.locationId).Select(rl => rl.name).Distinct()));
                subject = addon ? "Add On: " + subject : subject;
                Exception mailResult = Helpers.Helpers.SendEmail(from, to, cc, bcc, subject, true, body);
                Result r = new Result();
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
            return string.Empty;
        }

        public ActionResult Conflict(string ids)
        {
            TempData["ReturnPath"] = Request.UrlReferrer;
            if (ids.Length == 0)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            List<Reservation> model = new List<Reservation>();
            foreach (var id in ids.Split(','))
            {
                model.Add(db.Reservations.Find(Convert.ToInt32(id)));
            }
            return View(model);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
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
