using CO_VM.Models;
using Microsoft.AspNetCore.Mvc;
using PdfSharp.Drawing;
using PdfSharp.Pdf;
using System.Net;

namespace CO_VM.Controllers
{
    public class CertificateController : Controller
    {
        vaccineManagementContext vm = new vaccineManagementContext();
        public IActionResult Index()
        {
            return View();
        }

        private void CreateTextFile(int id, string date)
        {
            //int id = Convert.ToInt32(f["id"]);
            var booking = vm.Bookings.FirstOrDefault(t => t.Status == "Booked" && t.BookId == id);

            if (booking == null)
            {
                ViewBag.Message = "Booking not found.";
                return;
            }

            dynamic users;
            if (booking.FamilyId == null)
            {
                // Get from User table
                users = vm.Users.FirstOrDefault(u => u.UserId == booking.UserId);
            }
            else
            {
                // Get from Family table
                users = vm.Families.FirstOrDefault(f => f.FamilyId == booking.FamilyId);
            }
            var vaccines = (from t in vm.Vaccines where t.VaccineId == booking.VaccineId select t).FirstOrDefault();
            DateTime certificateDate = DateTime.Parse(date);

            var folderPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "Certificates");
            if (!Directory.Exists(folderPath))
                Directory.CreateDirectory(folderPath);
            var filePath = Path.Combine(folderPath, $"{id}_certificate.pdf");
            PdfDocument document = new PdfDocument();
            document.Info.Title = "Vaccination Certificate";
            PdfPage page = document.AddPage();
            XGraphics gfx = XGraphics.FromPdfPage(page);
            // Font
            XFont headerFont = new XFont("Arial", 14, XFontStyle.Bold);
            XFont titleFont = new XFont("Arial", 18, XFontStyle.Bold);
            XFont regularFont = new XFont("Arial", 12, XFontStyle.Regular);
            int top = 40;
            // Emblem
            var emblemPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "emblem.jfif");
        
            if (System.IO.File.Exists(emblemPath))
            {
                XImage emblem = XImage.FromFile(emblemPath);
                double emblemWidth = 50;
                double emblemHeight = emblem.PixelHeight * emblemWidth / emblem.PixelWidth;
                gfx.DrawImage(emblem, (page.Width - emblemWidth) / 2, top, emblemWidth, emblemHeight);
                top += (int)emblemHeight + 10;
            }
            // Government Heading
            gfx.DrawString("Ministry of Health & Family Welfare", headerFont, XBrushes.Black, new XRect(0, top, page.Width, 20), XStringFormats.TopCenter);
            top += 20;
            gfx.DrawString("Government of India", headerFont, XBrushes.Black, new XRect(0, top, page.Width, 20), XStringFormats.TopCenter);
            top += 30;
            gfx.DrawString("Issued by Ministry of Health & Family Welfare, Govt. of India",
             new XFont("Arial", 11, XFontStyle.Italic), XBrushes.DarkGray,
             new XRect(0, top, page.Width, 20), XStringFormats.TopCenter);
            top += 40;

            // Certificate Title
            gfx.DrawString("Certificate for Vaccination", titleFont, XBrushes.DarkBlue, new XRect(0, top, page.Width, 30), XStringFormats.TopCenter);
            top += 50;

            // Section Heading: Beneficiary Details
            double leftMargin = 60;
            gfx.DrawString("Beneficiary Details",
                new XFont("Arial", 14, XFontStyle.Bold),
            XBrushes.DarkBlue,
                new XRect(leftMargin, top, page.Width - leftMargin, 25),
                XStringFormats.TopLeft);
            top += 35;

            // Beneficiary Details
            gfx.DrawString($" Beneficiary Name   :  {users.FullName}", regularFont, XBrushes.Black, 100, top); top += 25;
            gfx.DrawString($" Gender             :  {users.Gender}", regularFont, XBrushes.Black, 100, top); top += 25;
            gfx.DrawString($" DateOfBirth         :   {users.Dob}", regularFont, XBrushes.Black, 100, top); top += 25;
            gfx.DrawString($"Aadhaar Verified     :    XXXX XXXX {users.AadhaarNo.Substring(users.AadhaarNo.Length - 4)}", regularFont, XBrushes.Black, 100, top); top += 25;
            gfx.DrawString($"Certificate Date     :   {certificateDate:dd MMMM yyyy}", regularFont, XBrushes.Black, 100, top); top += 40;

            // Vaccination Details
            // Section Heading: Vaccine Details
            double leftMarginforvac = 60;
            gfx.DrawString("Vaccine Details",
                new XFont("Arial", 14, XFontStyle.Bold),
            XBrushes.DarkBlue,
                new XRect(leftMarginforvac, top, page.Width - leftMarginforvac, 25),
                XStringFormats.TopLeft);
            top += 35;
            gfx.DrawString($" Vaccine        :       {vaccines.VaccineName}", regularFont, XBrushes.Black, 100, top); top += 25;
            gfx.DrawString($" Manufacturer   :       {vaccines.Manufacturer}", regularFont, XBrushes.Black, 100, top); top += 25;
            gfx.DrawString($" Dose Number    :    {booking.DoseNumber}", regularFont, XBrushes.Black, 100, top); top += 25;
            gfx.DrawString(" Issuer Name    :       Government Of India", regularFont, XBrushes.Black, 100, top); top += 40;
            var imagePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "modiji.jfif");
            if (System.IO.File.Exists(imagePath))
            {
                XImage image = XImage.FromFile(imagePath);

                // Set full width
                double targetWidth = page.Width;
                // Set a smaller custom height (e.g., 100 points)
                double targetHeight = 200; // You can adjust this value to your preference
                // Position the image at the very bottom
                double x = 0;
                double y = page.Height - targetHeight;
                gfx.DrawImage(image, x, y, targetWidth, targetHeight);
            }
            document.Save(filePath);
            ViewBag.Message = "Certificate created successfully!";
        }


        [HttpGet]
        public IActionResult GenerateAllCertificates()
        {
            return View();
        }
        [HttpPost]
        public IActionResult GenerateAllCertificates(IFormCollection f)
        {
            var date = f["certificateDate"];
            // Get all bookings where status is "Completed"
            var completedBookings = vm.Bookings.Where(b => b.Status == "Booked").ToList();
            foreach (var booking in completedBookings)
            {
                // Call your helper for each completed booking
                CreateTextFile(booking.BookId, date);
            }
            ViewBag.Message = "Certificates generated for all completed bookings!";
            return RedirectToAction("Certificate");
        }



        [HttpGet]
        public IActionResult Certificate()
        {
            // Display the form for uploading the certificate
            return View();
        }
        [HttpPost]
        public IActionResult Certificate(IFormCollection form)
        {
            int id = Convert.ToInt32(form["id"]);
            var certificatetable = (from t in vm.VaccineCertificates
                                    where t.BookId == id
                                    select t).FirstOrDefault();
            if (certificatetable == null)
            {
                var Booking = (from Booking t in vm.Bookings
                               where t.Status == "Booked" && t.BookId == id
                               select t).FirstOrDefault();
                int bookId = Booking.BookId;
                VaccineCertificate certificate = new VaccineCertificate();
                int bid = bookId;
                certificate.BookId = bid;
                var file = form.Files["t2"];
                if (file != null && file.Length > 0)
                {
                    using (var stream = new MemoryStream())
                    {
                        file.CopyTo(stream);
                        certificate.Certificate = stream.ToArray();
                    }
                }
                else
                {
                    ViewBag.Message = "Certificate file is required.";
                    return View();
                }
                vm.VaccineCertificates.Add(certificate);
                vm.SaveChanges();
                ViewBag.Message = "Certificate uploaded successfully!";
                Booking.Status = "Completed";
                vm.SaveChanges();
                return View(certificate);
            }
            else
            {
                ViewBag.found = "Already has a certificate.";
            }
            return View();
        }

        [HttpGet]

        public IActionResult DownloadCertificate(int bookId)

        {


            var certificate = vm.VaccineCertificates.FirstOrDefault(c => c.BookId == bookId );

            if (certificate == null || certificate.Certificate == null)

            {

                return NotFound("Certificate not found.");

            }

            string fileName = $"{bookId}_certificate.pdf";

            return File(certificate.Certificate, "application/pdf", fileName);

        }

        [HttpGet]

        public IActionResult Carti()

        {

            string UserId = HttpContext.Session.GetString("UserId");

            var users = vm.Users.FirstOrDefault(u => u.UserId == Convert.ToInt32(UserId));

            if (users == null)

            {

                return NotFound("User not found.");

            }

            int user_bookid = users.UserId;

            var Booking = (from t in vm.Bookings

                           where t.UserId == user_bookid && t.Status=="Completed"
                           select t).ToList();
            if (Booking == null)
            {
                return NotFound("Booking not found.");
            }
            return View(Booking);
        }

        [HttpGet]
        public IActionResult Status()
        {
            return View();
        }
        [HttpPost]
        public IActionResult Status(IFormCollection f)
        {
            int id = Convert.ToInt32(f["BookId"]);
            var res = (from t in vm.Bookings where t.BookId == id select t).FirstOrDefault();
            res.DoseNumber = Convert.ToInt32(f["DoseNumber"]);
            res.VaccinatedOn = DateOnly.Parse(f["VaccinatedOn"]);
            vm.Bookings.Update(res);
            vm.SaveChanges();
            ViewBag.Status = "User Status Updated Successfully";
            return RedirectToAction("GenerateAllCertificates");
        }




    }
}