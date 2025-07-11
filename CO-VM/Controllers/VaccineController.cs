﻿using CO_VM.Models;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CO_VM.Controllers
{
    public class VaccineController : Controller
    {
        vaccineManagementContext vm = new vaccineManagementContext();
        //vaccineManagementContext vm;
        private readonly ILogger<VaccineController> logger;
        public VaccineController(ILogger<VaccineController> logger)

        {
            this.logger = logger;
            //vm = con;

        }

        //public VaccineController()
        //{
        //}

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult UserDashboard()
        {
            try
            {
                int userId = Convert.ToInt32(HttpContext.Session.GetString("UserId"));
                var user = vm.Users.FirstOrDefault(u => u.UserId == userId);
                // Eager load Relation for each family member
                var family = vm.Families
                    .Include(f => f.Relation)
                    .Where(f => f.UserId == userId)
                    .ToList();

                var familyIds = family.Select(f => f.FamilyId).ToList();

                var bookings = vm.Bookings
                    .Where(b => b.UserId == userId || (b.FamilyId != null && familyIds.Contains(b.FamilyId.Value)))
                    .OrderByDescending(b => b.BookingDate)
                    .ToList();

                foreach (var booking in bookings)
                {
                    booking.Family = booking.Family ?? family.FirstOrDefault(f => f.FamilyId == booking.FamilyId);
                    booking.Vaccine = booking.Vaccine ?? vm.Vaccines.FirstOrDefault(v => v.VaccineId == booking.VaccineId);
                    booking.Slot = booking.Slot ?? vm.Slots.FirstOrDefault(s => s.SlotId == booking.SlotId);
                }

                ViewBag.User = user;
                ViewBag.Family = family;
                ViewBag.BookingCount = bookings.Count;
                ViewBag.Bookings = bookings;

                return View();
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "An error occurred in UserDashboard.");
                return RedirectToAction("Error");
            }
        }

        [HttpPost]
        public IActionResult DeleteFamily(int id)
        {
            try
            {
                var family = vm.Families.FirstOrDefault(f => f.FamilyId == id);
                if (family != null)
                {
                    vm.Families.Remove(family);
                    vm.SaveChangesAsync();
                }

                return RedirectToAction("UserDashboard");
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "An error occurred in Delete Family.");
                return RedirectToAction("Error");

            }

        }


        // Show the Add Family Member form
        [HttpGet]
        public IActionResult Family()
        {
            try
            {
                var model = new Family();
                ViewBag.Relations = vm.Relations.ToList();
                return View(model);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "An error occurred in Family.");
                return RedirectToAction("Error");

            }
        }
      
        [HttpPost]
        public IActionResult Family(Family model)
        {
            try
            {
                if (ModelState.IsValid)
                {

                    int userId = Convert.ToInt32(HttpContext.Session.GetString("UserId"));
                    model.UserId = userId;


                    vm.Families.Add(model);
                    vm.SaveChangesAsync();


                    return RedirectToAction("UserDashboard");
                }

                return View(model);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "An error occurred in Family.");
                return RedirectToAction("Error");

            }

        }

        [HttpGet]
        public IActionResult Vaccines()
        {
            try
            {
                if (HttpContext.Session.GetString("UserId") == null)
                {
                    return RedirectToAction("Login", "Vaccine");
                }

                Response.Headers["Cache-Control"] = "no-store, no-cache, must-revalidate";
                Response.Headers["Pragma"] = "no-cache";
                Response.Headers["Expires"] = "-1";

                var vaccines = vm.Vaccines.ToList();
                var states = vm.Centres.Select(c => c.State).Distinct().ToList();
                var cities = vm.Centres.Select(c => c.City).Distinct().ToList();
                var centres = vm.Centres.Select(c => c.CentreName).Distinct().ToList();

                ViewBag.VaccineNames = vaccines.Select(v => v.VaccineName).Distinct().ToList();
                ViewBag.States = states;
                ViewBag.Cities = cities;
                ViewBag.Centres = centres;

                return View(vaccines);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "An error occurred in Family.");
                return RedirectToAction("Error");
            }
        }


        [HttpPost]
        public IActionResult Vaccines(IFormCollection f)
        {
            string city = f["city"];
            string state = f["state"];
            string centreSearch = f["centres"];
            string vaccinename = f["vaccinename"];

            // Find all matching centres (by city, state, and centre name)
            var centres = vm.Centres
                .Where(c =>
                    (string.IsNullOrEmpty(city) || c.City.Contains(city)) &&
                    (string.IsNullOrEmpty(state) || c.State.Contains(state)) &&
                    (string.IsNullOrEmpty(centreSearch) || c.CentreName.Contains(centreSearch)))
                .Select(c => c.CentreId)
                .ToList();

            if (centres.Count == 0)
            {
                ViewBag.no = "No city/state/centre available";
                // Set ViewBag for dropdowns
                SetDropdownViewBags();
                return View(new List<Vaccine>());
            }

            // Find all vaccine-centre mappings for those centres
            var vaccineCentreIds = vm.VaccineCentres
                .Where(vc => centres.Contains(vc.CentreId))
                .Select(vc => vc.VaccineId)
                .Distinct()
                .ToList();

            if (vaccineCentreIds.Count == 0)
            {
                ViewBag.no = "No vaccine center available for this city/state/centre";
                SetDropdownViewBags();
                return View(new List<Vaccine>());
            }

            // Find all vaccines for those mappings and filter by vaccine name if provided
            var vaccines = vm.Vaccines
                .Where(v => vaccineCentreIds.Contains(v.VaccineId) &&
                    (string.IsNullOrEmpty(vaccinename) || v.VaccineName.Contains(vaccinename)))
                .ToList();

            if (vaccines.Count == 0)
            {
                ViewBag.no = "No vaccines available for this city/state/centre/vaccine name";
            }

            // Set ViewBag for dropdowns
            SetDropdownViewBags();

            return View(vaccines);

            // Local function to set dropdown ViewBags
            void SetDropdownViewBags()
            {
                ViewBag.VaccineNames = vm.Vaccines.Select(v => v.VaccineName).Distinct().ToList();
                ViewBag.States = vm.Centres.Select(c => c.State).Distinct().ToList();
                ViewBag.Cities = vm.Centres.Select(c => c.City).Distinct().ToList();
                ViewBag.Centres = vm.Centres.Select(c => c.CentreName).Distinct().ToList();
            }
        }



        //[HttpPost]
        //public IActionResult Vaccines(IFormCollection f)
        //{

        //    string city = f["city"];
        //    string state = f["state"];
        //    var centres = (from t in ob.Centres where t.City.Contains(city) && t.State.Contains(state) select t).FirstOrDefault();
        //    if (centres != null)
        //    {
        //        var Vaccinecentre = ob.VaccineCentres.FirstOrDefault(v => v.CentreId == centres.CentreId);
        //        if (Vaccinecentre != null)
        //        {
        //            var vaccine = (from t in ob.Vaccines where t.VaccineId == Vaccinecentre.VaccineId select t).ToList();
        //            return View(vaccine);
        //        }
        //        else
        //        {
        //            ViewBag.no = "No vaccine center available for this city/state";
        //            return View();
        //        }
        //    }
        //    else
        //    {
        //        ViewBag.no = "No city available";
        //        return View();
        //    }

        //try
        //{
        //    // Start with all vaccines and join to centres
        //    var vaccinesQuery = from v in ob.Vaccines
        //                        join vc in ob.VaccineCentres on v.VaccineId equals vc.VaccineId
        //                        join c in ob.Centres on vc.CentreId equals c.CentreId
        //                        select new { Vaccine = v, Centre = c };

        //    var result = vaccinesQuery.Where(s => s.Vaccine.VaccineName.Contains(search) || s.Centre.State.Contains(search));


        //    return View(result.ToList());

        //    //bool hasSearch = !string.IsNullOrWhiteSpace(search);
        //    //bool hasState = !string.IsNullOrWhiteSpace(state);
        //    //bool hasCentre = !string.IsNullOrWhiteSpace(centre);

        //    //if (hasSearch)
        //    //{
        //    //    string searchLower = search.ToLower();
        //    //    vaccinesQuery = vaccinesQuery.Where(x =>
        //    //        (x.Vaccine.VaccineName != null && x.Vaccine.VaccineName.ToLower().Contains(searchLower)) ||
        //    //        (x.Vaccine.Description != null && x.Vaccine.Description.ToLower().Contains(searchLower)) ||
        //    //        (x.Vaccine.Manufacturer != null && x.Vaccine.Manufacturer.ToLower().Contains(searchLower))
        //    //    );
        //    //}

        //    //if (hasState)
        //    //{
        //    //    string stateLower = state.ToLower();
        //    //    vaccinesQuery = vaccinesQuery.Where(x =>
        //    //        x.Centre.State != null && x.Centre.State.ToLower().Contains(stateLower)
        //    //    );
        //    //}

        //    //if (hasCentre)
        //    //{
        //    //    string centreLower = centre.ToLower();
        //    //    vaccinesQuery = vaccinesQuery.Where(x =>
        //    //        x.Centre.CentreName != null && x.Centre.CentreName.ToLower().Contains(centreLower)
        //    //    );
        //    //}

        //    //List<Vaccine> result;
        //    //if (!hasSearch && !hasState && !hasCentre)
        //    //{
        //    //    result = ob.Vaccines.ToList();
        //    //}
        //    //else
        //    //{
        //    //    // ToList() here to force query execution before Distinct()
        //    //    result = vaccinesQuery
        //    //        .Select(x => x.Vaccine)
        //    //        .Distinct()
        //    //        .ToList();
        //    //}


        //}
        //catch (Exception ex)
        //{
        //    logger.LogError(ex, "An error occurred in VaccineSearch.");
        //    ViewBag.Error = "Search failed. Please try again.";
        //    return View("Vaccines", new List<Vaccine>());
        //}
        //}



        [HttpGet]
        public IActionResult Login()
        {
            try
            {
                var captchaCode = GenerateCaptchaCode(7);
                HttpContext.Session.SetString("CaptchaCode", captchaCode);
                return View();
            }catch(Exception ex)
            {

                logger.LogError(ex, "An error occurred in Login (GET).");
                return RedirectToAction("Error");
            }

        }


        [HttpPost]
        public IActionResult Login(string Username, string Password, string Captcha)
        {
            try
            {
                // Validate CAPTCHA 
                if (!IsCaptchaValid(Captcha))
                {
                    ViewBag.CaptchaError = "Invalid CAPTCHA.";
                    return View();
                }
                var sessionCaptcha = HttpContext.Session.GetString("CaptchaCode") ?? "";
                if (!string.Equals(Captcha, sessionCaptcha, StringComparison.OrdinalIgnoreCase))
                {
                    ModelState.AddModelError("Captcha", "Invalid captcha code. Please try again.");
                }
                var passwordBytes = System.Text.Encoding.UTF8.GetBytes(Password);
                var user = vm.Users.FirstOrDefault(u => u.Username == Username && u.Password.SequenceEqual(passwordBytes));

                var newCaptchaCode = GenerateCaptchaCode(5);
                HttpContext.Session.SetString("CaptchaCode", newCaptchaCode);
                ViewBag.CaptchaCode = newCaptchaCode;

                if (user != null)
                {
                    HttpContext.Session.SetString("UserId", user.UserId.ToString());
                    ViewBag.a = "Login Successful";
                    return RedirectToAction("Index");
                }
                else
                {
                    return RedirectToAction("Login");
                }
            }

            catch (Exception ex)
            {
                logger.LogError(ex, "An error occurred in Login (POST).");
                ViewBag.CaptchaError = "Login failed.";
                return View();
            }

        }

        // Method to validate CAPTCHA 
        private bool IsCaptchaValid(string captcha)
        {
            // Implement your CAPTCHA validation logic here 
            // For example, compare with a stored value or use a CAPTCHA service 
            return captcha == HttpContext.Session.GetString("CaptchaCode");
        }


        [HttpGet]
        public IActionResult Captcha()
        {
            try
            {
                // Generate CAPTCHA code and store it in session 
                var captchaCode = GenerateCaptchaCode(7);
                HttpContext.Session.SetString("CaptchaCode", captchaCode);
                return Content(captchaCode);
            }
            catch (Exception ex)
            {

                logger.LogError(ex, "An error occurred in Login (GET).");
                return RedirectToAction("Error");
            }

        }

        // Method to generate CAPTCHA code 
        private string GenerateCaptchaCode(int length)
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            var random = new Random();
            return new string(Enumerable.Repeat(chars, 6)
                .Select(s => s[random.Next(s.Length)]).ToArray());
        }

        [HttpGet]
        public IActionResult ForgotPassword()
        {
            try
            {
                return View();
            }
            catch (Exception ex)
            {

                logger.LogError(ex, "An error occurred in ForgotPassword (GET).");
                return RedirectToAction("Error");
            }

        }


        [HttpPost]
        public IActionResult ForgotPassword(string Username, string SecurityQuestion, string SecurityAnswer, string NewPassword)
        {
            try
            {
                var user = vm.Users.FirstOrDefault(u => u.Username == Username);

                if (user == null)
                {
                    ViewBag.Error = "Username not found.";
                    return View();
                }

                // Show question pre-filled 
                ViewBag.Username = Username;
                ViewBag.SecurityQuestion = user.SecurityQuestion;
                if (string.IsNullOrEmpty(SecurityAnswer))
                {
                    return View();
                }

                ViewBag.SecurityAnswer = SecurityAnswer;
                ViewBag.NewPassword = NewPassword;
                // Validate security answer 
                if (user.SecurityAnswer != SecurityAnswer)
                {
                    ViewBag.Error = "Incorrect security answer.";
                    return View();
                }

                if (!string.IsNullOrWhiteSpace(NewPassword))
                {
                    var bytes = System.Text.Encoding.UTF8.GetBytes(NewPassword);
                    user.Password = bytes;
                    vm.SaveChangesAsync();
                    TempData["Success"] = "Password set successfully!";
                    return RedirectToAction("Login");
                }
                // If answer is right but password is missing, prompt 
                ViewBag.AnswerVerified = true;
                return View();
            }

            catch (Exception ex)
            {
                logger.LogError(ex, "An error occurred in ForgotPassword (POST).");
                ViewBag.Error = "Password reset failed.";
                return View();
            }

        }

        [HttpGet]
        public IActionResult ResetPassword()
        {
            try
            {
                return View();
            }
            catch (Exception ex)
            {

                logger.LogError(ex, "An error occurred in ResetPassword(GET).");
                return RedirectToAction("Error");
            }

        }

        [HttpPost]
        public IActionResult ResetPassword(string Username, string OldPassword, string NewPassword, string ConfirmPassword)
        {
            try
            {
                var user = vm.Users.FirstOrDefault(u => u.Username == Username);

                if (user == null)
                {
                    ViewBag.Error = "User not found.";
                    return View();
                }

                // Compare   old password 
                var oldpassword = System.Text.Encoding.UTF8.GetBytes(OldPassword);
                if (user.Password.SequenceEqual(oldpassword))
                {
                    ViewBag.Error = "Old password is incorrect.";
                    return View();
                }

                if (NewPassword != ConfirmPassword)
                {
                    ViewBag.Error = "New password and confirm password do not match.";
                    return View();
                }
                var newpassword = System.Text.Encoding.UTF8.GetBytes(NewPassword);

                // Update password 
                user.Password = newpassword;
                vm.SaveChangesAsync();

                TempData["Success"] = "Password Reset successfully!";
                return RedirectToAction("Login");
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "An error occurred in ResetPassword (POST).");
                ViewBag.Error = "Password reset failed.";
                return View();
            }

        }

        [HttpGet]
        public IActionResult Register()
        {
            try
            {
                return View();
            }
            catch (Exception ex)
            {

                logger.LogError(ex, "An error occurred in Login (GET).");
                return RedirectToAction("Error");
            }

        }

        [HttpPost]
        public IActionResult Register(User u, string password, string ConfirmPassword, string securityQuestion)
        {
            try
            {
                // Validate password
                if (string.IsNullOrEmpty(password))
                {
                    ModelState.AddModelError("Password", "Password is required.");
                    return View(u);
                }

                // Check if passwords match
                if (password != ConfirmPassword)
                {
                    ModelState.AddModelError("ConfirmPassword", "Password and Confirm Password do not match.");
                    return View(u);
                }
                //validation for dob
                if (u.Dob >= DateOnly.FromDateTime(DateTime.Today))
                {
                    ModelState.AddModelError("Dob", "Date of Birth must be less than today's date.");
                    return View(u);
                }


                u.SecurityQuestion = securityQuestion;
                u.Password = System.Text.Encoding.UTF8.GetBytes(password);

                try
                {
                    vm.Users.Add(u);
                    int i = vm.SaveChanges();

                    if (i != 0)
                    {
                        ViewBag.data = "Registration successfully";
                        return RedirectToAction("Login");
                    }
                }
                catch (Exception ex)
                {
                    ViewBag.data = "Registration Failed";
                }
                return View(u);
            }
            catch (Exception ex)
            {
               // logger.LogError(ex, "An error occurred in Register.");
                ViewBag.data = "Registration Failed";
                return View("Register");
            }
        }


        [HttpPost]
        public IActionResult CancelBooking(int familyId)
        {
            try
            {
                int userId = Convert.ToInt32(HttpContext.Session.GetString("UserId"));

                var booking = vm.Bookings
                    .Where(b => b.FamilyId == familyId && b.Status == "Booked")
                    .OrderByDescending(b => b.BookingDate)
                    .FirstOrDefault();

                if (booking != null)
                {
                    booking.Status = "Cancelled";

                    var vaccine = vm.Vaccines.FirstOrDefault(v => v.VaccineId == booking.VaccineId);
                    if (vaccine != null && vaccine.Stock.HasValue)
                    {
                        vaccine.Stock += 1;
                        vm.SaveChangesAsync();
                    }
                }


                vm.SaveChangesAsync();
                return RedirectToAction("UserDashboard");
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "An error occurred in Cancel Booking.");
                ViewBag.data = "Registration Failed";
                return View("UserDashboard");
            }

        }

        [HttpPost]
        public IActionResult CancelBookingForUser()
        {
            try
            {
                string userIdStr = HttpContext.Session.GetString("UserId");
                if (string.IsNullOrEmpty(userIdStr))
                {
                    return RedirectToAction("Login");
                }
                int userId = Convert.ToInt32(userIdStr);

                var booking = vm.Bookings
                    .Where(b => b.UserId == userId && b.FamilyId == null && b.Status == "Booked")
                    .OrderByDescending(b => b.BookingDate)
                    .FirstOrDefault();

                if (booking != null)
                {
                    booking.Status = "Cancelled";

                    var vaccine = vm.Vaccines.FirstOrDefault(v => v.VaccineId == booking.VaccineId);
                    if (vaccine != null && vaccine.Stock.HasValue)
                    {
                        vaccine.Stock += 1;
                    }
                    vm.SaveChangesAsync();
                }

                return RedirectToAction("UserDashboard");
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "An error occurred in Cancel Booking For User.");
                ViewBag.data = "Registration Failed";
                return View("UserDashboard");
            }

        }


        [HttpGet]
        public IActionResult BookSlot(int familyId)
        {
            try
            {
                var family = vm.Families.FirstOrDefault(f => f.FamilyId == familyId);
                if (family == null) return NotFound();

                ViewBag.Family = family;
                ViewBag.Vaccines = vm.Vaccines.ToList();
                ViewBag.Centres = vm.Centres.ToList();
                return View();
            }
            catch (Exception ex)
            {

                logger.LogError(ex, "An error occurred in Login (GET).");
                return RedirectToAction("Error");
            }

        }


        //[HttpPost]
        //public IActionResult BookSlot(int familyId, int vaccineId, int centreId, DateTime slotDate, TimeSpan slotTime)
        //{
        //    try
        //    {
        //        int userId = Convert.ToInt32(HttpContext.Session.GetString("UserId"));

        //        var vaccine1 = vm.Vaccines.FirstOrDefault(v => v.VaccineId == vaccineId);
        //        if (vaccine1 == null || !vaccine1.Stock.HasValue || vaccine1.Stock.Value <= 0)
        //        {
        //            ViewBag.Family = vm.Families.FirstOrDefault(f => f.FamilyId == familyId);
        //            ViewBag.Vaccines = vm.Vaccines.ToList();
        //            ViewBag.Centres = vm.Centres.ToList();
        //            ViewBag.Error = "Selected vaccine is out of stock.";
        //            return View();
        //        }

        //        var slot = vm.Slots.FirstOrDefault(s =>
        //            s.VaccineId == vaccineId &&
        //            s.CentreId == centreId &&
        //            s.SlotDate == DateOnly.FromDateTime(slotDate) &&
        //            s.SlotTime == TimeOnly.FromTimeSpan(slotTime) &&
        //            s.FamilyId == familyId);

        //        if (slot == null)
        //        {
        //            slot = new Slot
        //            {
        //                VaccineId = vaccineId,
        //                UserId = userId,
        //                CentreId = centreId,
        //                SlotDate = DateOnly.FromDateTime(slotDate),
        //                SlotTime = TimeOnly.FromTimeSpan(slotTime),
        //                FamilyId = familyId
        //            };
        //            vm.Slots.Add(slot);
        //            vm.SaveChanges();
        //        }

        //        var booking = vm.Bookings
        //            .Where(b => b.FamilyId == familyId && b.Status == "Cancelled")
        //            .OrderByDescending(b => b.BookingDate)
        //            .FirstOrDefault();

        //        if (booking != null)
        //        {
        //            booking.SlotId = slot.SlotId;
        //            booking.VaccineId = vaccineId;
        //            booking.BookingDate = DateTime.Now;
        //            booking.VaccinatedOn = DateOnly.FromDateTime(slotDate);
        //            booking.Status = "Booked";
        //            booking.PaymentMode = "Online";
        //        }
        //        else
        //        {
        //            booking = new Booking
        //            {
        //                UserId = userId,
        //                FamilyId = familyId,
        //                SlotId = slot.SlotId,
        //                VaccineId = vaccineId,
        //                BookingDate = DateTime.Now,
        //                Status = "Booked",
        //                PaymentMode = "Online"
        //            };
        //            vm.Bookings.Add(booking);
        //        }

        //        var vaccine = vm.Vaccines.FirstOrDefault(v => v.VaccineId == vaccineId);
        //        if (vaccine != null && vaccine.Stock.HasValue && vaccine.Stock.Value > 0)
        //        {
        //            vaccine.Stock -= 1;
        //        }

        //        vm.SaveChanges();
        //        return RedirectToAction("UserDashboard");
        //    }
        //    catch (Exception ex)
        //    {
        //        logger.LogError(ex, "An error occurred in Booking Slot.");
        //        ViewBag.data = "Booking failed.";
        //        // Repopulate dropdowns for the view
        //        ViewBag.Family = vm.Families.FirstOrDefault(f => f.FamilyId == familyId);
        //        ViewBag.Vaccines = vm.Vaccines.ToList();
        //        ViewBag.Centres = vm.Centres.ToList();
        //        return View();
        //    }
        //}

        [HttpPost]
        public IActionResult BookSlot(int familyId, int vaccineId, int centreId, DateTime slotDate, TimeSpan slotTime)
        {
            try
            {
                int userId = Convert.ToInt32(HttpContext.Session.GetString("UserId"));

                // Check if this family member already has a booking for this vaccine that is not cancelled
                var existingBooking = vm.Bookings
                    .FirstOrDefault(b => b.FamilyId == familyId && b.VaccineId == vaccineId && b.Status == "Booked");

                if (existingBooking != null)
                {
                    ViewBag.Family = vm.Families.FirstOrDefault(f => f.FamilyId == familyId);
                    ViewBag.Vaccines = vm.Vaccines.ToList();
                    ViewBag.Centres = vm.Centres.ToList();
                    ViewBag.Error = "This family member already has a booking for this vaccine.";
                    return View();
                }

                var vaccine1 = vm.Vaccines.FirstOrDefault(v => v.VaccineId == vaccineId);
                if (vaccine1 == null || !vaccine1.Stock.HasValue || vaccine1.Stock.Value <= 0)
                {
                    ViewBag.Family = vm.Families.FirstOrDefault(f => f.FamilyId == familyId);
                    ViewBag.Vaccines = vm.Vaccines.ToList();
                    ViewBag.Centres = vm.Centres.ToList();
                    ViewBag.Error = "Selected vaccine is out of stock.";
                    return View();
                }

                var slot = vm.Slots.FirstOrDefault(s =>
                    s.VaccineId == vaccineId &&
                    s.CentreId == centreId &&
                    s.SlotDate == DateOnly.FromDateTime(slotDate) &&
                    s.SlotTime == TimeOnly.FromTimeSpan(slotTime) &&
                    s.FamilyId == familyId);

                if (slot == null)
                {
                    slot = new Slot
                    {
                        VaccineId = vaccineId,
                        UserId = userId,
                        CentreId = centreId,
                        SlotDate = DateOnly.FromDateTime(slotDate),
                        SlotTime = TimeOnly.FromTimeSpan(slotTime),
                        FamilyId = familyId
                    };
                    vm.Slots.Add(slot);
                    vm.SaveChanges();
                }

                var cancelledBooking = vm.Bookings
                    .Where(b => b.FamilyId == familyId && b.Status == "Cancelled")
                    .OrderByDescending(b => b.BookingDate)
                    .FirstOrDefault();

                if (cancelledBooking != null)
                {
                    cancelledBooking.SlotId = slot.SlotId;
                    cancelledBooking.VaccineId = vaccineId;
                    cancelledBooking.BookingDate = DateTime.Now;
                    cancelledBooking.VaccinatedOn = DateOnly.FromDateTime(slotDate);
                    cancelledBooking.Status = "Booked";
                    cancelledBooking.PaymentMode = "Online";
                }
                else
                {
                    var booking = new Booking
                    {
                        UserId = userId,
                        FamilyId = familyId,
                        SlotId = slot.SlotId,
                        VaccineId = vaccineId,
                        BookingDate = DateTime.Now,
                        Status = "Booked",
                        PaymentMode = "Online"
                    };
                    vm.Bookings.Add(booking);
                }

                if (vaccine1.Stock.HasValue && vaccine1.Stock.Value > 0)
                {
                    vaccine1.Stock -= 1;
                }

                vm.SaveChanges();
                return RedirectToAction("UserDashboard");
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "An error occurred in Booking Slot.");
                ViewBag.data = "Booking failed.";
                // Repopulate dropdowns for the view
                ViewBag.Family = vm.Families.FirstOrDefault(f => f.FamilyId == familyId);
                ViewBag.Vaccines = vm.Vaccines.ToList();
                ViewBag.Centres = vm.Centres.ToList();
                return View();
            }
        }

        [HttpGet]
        public IActionResult BookSlotForUser(int? vaccineId = null)
        {
            try
            {
                int userId = Convert.ToInt32(HttpContext.Session.GetString("UserId"));
                var vaccine = vm.Vaccines.FirstOrDefault(v => v.VaccineId == vaccineId);
                if (vaccine == null || !vaccine.Stock.HasValue || vaccine.Stock.Value <= 0)
                {
                    // Repopulate dropdowns and show error
                    ViewBag.User = vm.Users.FirstOrDefault(u => u.UserId == userId);
                    ViewBag.Vaccines = vm.Vaccines.ToList();
                    ViewBag.Centres = vm.Centres.ToList();
                    ViewBag.SelectedVaccineId = vaccineId;
                    ViewBag.Error = "Selected vaccine is out of stock.";
                    return View();
                }

                var user = vm.Users.FirstOrDefault(u => u.UserId == userId);


                ViewBag.User = user;
                ViewBag.Vaccines = vm.Vaccines.ToList();
                ViewBag.Centres = vm.Centres.ToList();
                ViewBag.SelectedVaccineId = vaccineId;
                return View();
            }
            catch (Exception ex)
            {

                logger.LogError(ex, "An error occurred in BookSlotForUser.");
                return RedirectToAction("Error");
            }
        }


        //[HttpPost]
        //public IActionResult BookSlotForUser(int vaccineId, int centreId, DateTime slotDate, TimeSpan slotTime)
        //{
        //    try
        //    {
        //        string userIdStr = HttpContext.Session.GetString("UserId");
        //        if (string.IsNullOrEmpty(userIdStr))
        //        {
        //            return RedirectToAction("Login");
        //        }
        //        int userId = Convert.ToInt32(userIdStr);

        //        var vaccine = vm.Vaccines.FirstOrDefault(v => v.VaccineId == vaccineId);
        //        if (vaccine == null || !vaccine.Stock.HasValue || vaccine.Stock.Value <= 0)
        //        {
        //            ViewBag.Error = "Selected vaccine is out of stock.";
        //            ViewBag.User = vm.Users.FirstOrDefault(u => u.UserId == userId);
        //            ViewBag.Vaccines = vm.Vaccines.ToList();
        //            ViewBag.Centres = vm.Centres.ToList();
        //            ViewBag.SelectedVaccineId = vaccineId;
        //            return View();
        //        }

        //        var slot = vm.Slots.FirstOrDefault(s =>
        //            s.VaccineId == vaccineId &&
        //            s.CentreId == centreId &&
        //            s.SlotDate == DateOnly.FromDateTime(slotDate) &&
        //            s.SlotTime == TimeOnly.FromTimeSpan(slotTime) &&
        //            s.UserId == userId &&
        //            s.FamilyId == null);

        //        if (slot == null)
        //        {
        //            slot = new Slot
        //            {
        //                VaccineId = vaccineId,
        //                UserId = userId,
        //                CentreId = centreId,
        //                SlotDate = DateOnly.FromDateTime(slotDate),
        //                SlotTime = TimeOnly.FromTimeSpan(slotTime),
        //                FamilyId = null
        //            };
        //            vm.Slots.Add(slot);
        //            vm.SaveChanges();
        //        }

        //        var booking = vm.Bookings
        //            .Where(b => b.UserId == userId && b.FamilyId == null && b.Status == "Cancelled")
        //            .OrderByDescending(b => b.BookingDate)
        //            .FirstOrDefault();

        //        if (booking != null)
        //        {
        //            booking.SlotId = slot.SlotId;
        //            booking.VaccineId = vaccineId;
        //            booking.BookingDate = DateTime.Now;
        //            booking.Status = "Booked";
        //            booking.PaymentMode = "Online";
        //        }
        //        else
        //        {
        //            booking = new Booking
        //            {
        //                UserId = userId,
        //                FamilyId = null,
        //                SlotId = slot.SlotId,
        //                VaccineId = vaccineId,
        //                BookingDate = DateTime.Now,
        //                Status = "Booked",
        //                PaymentMode = "Online"
        //            };
        //            vm.Bookings.Add(booking);
        //        }

        //        if (vaccine.Stock.HasValue && vaccine.Stock.Value > 0)
        //        {
        //            vaccine.Stock -= 1;
        //        }
        //        vm.SaveChanges();

        //        return RedirectToAction("UserDashboard");
        //    }
        //    catch (Exception ex)
        //    {
        //        logger.LogError(ex, "An error occurred in Booking Slot.");
        //        ViewBag.data = "Booking failed.";
        //        return View();
        //    }
        //}

        [HttpPost]
        public IActionResult BookSlotForUser(int vaccineId, int centreId, DateTime slotDate, TimeSpan slotTime)
        {
            try
            {
                string userIdStr = HttpContext.Session.GetString("UserId");
                if (string.IsNullOrEmpty(userIdStr))
                {
                    return RedirectToAction("Login");
                }
                int userId = Convert.ToInt32(userIdStr);

                // Prevent duplicate booking for the same vaccine
                var existingBooking = vm.Bookings
                    .FirstOrDefault(b => b.UserId == userId && b.FamilyId == null && b.VaccineId == vaccineId && b.Status == "Booked");

                if (existingBooking != null)
                {
                    ViewBag.Error = "You have already booked this vaccine.";
                    ViewBag.User = vm.Users.FirstOrDefault(u => u.UserId == userId);
                    ViewBag.Vaccines = vm.Vaccines.ToList();
                    ViewBag.Centres = vm.Centres.ToList();
                    ViewBag.SelectedVaccineId = vaccineId;
                    return View();
                }

                var vaccine = vm.Vaccines.FirstOrDefault(v => v.VaccineId == vaccineId);
                if (vaccine == null || !vaccine.Stock.HasValue || vaccine.Stock.Value <= 0)
                {
                    ViewBag.Error = "Selected vaccine is out of stock.";
                    ViewBag.User = vm.Users.FirstOrDefault(u => u.UserId == userId);
                    ViewBag.Vaccines = vm.Vaccines.ToList();
                    ViewBag.Centres = vm.Centres.ToList();
                    ViewBag.SelectedVaccineId = vaccineId;
                    return View();
                }

                var slot = vm.Slots.FirstOrDefault(s =>
                    s.VaccineId == vaccineId &&
                    s.CentreId == centreId &&
                    s.SlotDate == DateOnly.FromDateTime(slotDate) &&
                    s.SlotTime == TimeOnly.FromTimeSpan(slotTime) &&
                    s.UserId == userId &&
                    s.FamilyId == null);

                if (slot == null)
                {
                    slot = new Slot
                    {
                        VaccineId = vaccineId,
                        UserId = userId,
                        CentreId = centreId,
                        SlotDate = DateOnly.FromDateTime(slotDate),
                        SlotTime = TimeOnly.FromTimeSpan(slotTime),
                        FamilyId = null
                    };
                    vm.Slots.Add(slot);
                    vm.SaveChanges();
                }

                var booking = vm.Bookings
                    .Where(b => b.UserId == userId && b.FamilyId == null && b.Status == "Cancelled")
                    .OrderByDescending(b => b.BookingDate)
                    .FirstOrDefault();

                if (booking != null)
                {
                    booking.SlotId = slot.SlotId;
                    booking.VaccineId = vaccineId;
                    booking.BookingDate = DateTime.Now;
                    booking.Status = "Booked";
                    booking.PaymentMode = "Online";
                }
                else
                {
                    booking = new Booking
                    {
                        UserId = userId,
                        FamilyId = null,
                        SlotId = slot.SlotId,
                        VaccineId = vaccineId,
                        BookingDate = DateTime.Now,
                        Status = "Booked",
                        PaymentMode = "Online"
                    };
                    vm.Bookings.Add(booking);
                }

                if (vaccine.Stock.HasValue && vaccine.Stock.Value > 0)
                {
                    vaccine.Stock -= 1;
                }
                vm.SaveChanges();

                return RedirectToAction("UserDashboard");
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "An error occurred in Booking Slot.");
                ViewBag.data = "Booking failed.";
                return View();
            }
        }

        public IActionResult Error()
        {
            var exceptionFeature = HttpContext.Features.Get<IExceptionHandlerFeature>();
            if (exceptionFeature != null)
            {
                // Log the exception details 
                logger.LogError(exceptionFeature.Error, "An error occurred.");
            }
            return View("Error");
        }

        //[HttpGet]
        public IActionResult ProjectHome()
        {
            return View();
        }

        [HttpGet]
        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            Response.Headers["Cache-Control"] = "no-cache, no-store, must-revalidate";
            Response.Headers["Pragma"] = "no-cache";
            Response.Headers["Expires"] = "-1";

            return RedirectToAction("ProjectHome", "Vaccine");
        }

        [HttpGet]
        public IActionResult Feedback()
        {
            return View();
        }

        [HttpGet]
        public IActionResult History()
        {
            var UserId = HttpContext.Session.GetString("UserId");
            var user = vm.Users.FirstOrDefault(u => u.UserId == Convert.ToInt32(UserId));
            if (user == null)
            {
                return NotFound("User not found.");
            }
            var userhistory = (from t in vm.Bookings
                               where t.UserId == user.UserId && t.FamilyId == null
                               select t).ToList();
            ViewBag.UserHistory = userhistory;

            var familyhistory = (from t in vm.Families
                                 where t.UserId == user.UserId && t.FamilyId != null
                                 select t).ToList();
            ViewBag.FamilyHistory = familyhistory;

            return View();
        }



    }

    public class ForgotPasswordViewModel
    {
        public string SecurityAnswer { get; internal set; }
        public object NewPassword { get; internal set; }
        public object ConfirmPassword { get; internal set; }
        public string Username { get; internal set; }
        public String SecurityQuestion { get; internal set; }
    }
}





