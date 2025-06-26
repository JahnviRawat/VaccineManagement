using CO_VM.Controllers;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using NUnit.Framework;
using System.Text;

using CO_VM.Models;


using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Primitives;
using Moq;

namespace VaccineProjectTesting
{
    public class Tests
    {
        private FeedbackController _controller;
        [SetUp]
        public void Setup()
        {
        }

        [Test]
        public void Test_For_ViewUser()
        {
            var controller = new AdminController();
            var result = controller.View();
            Assert.That(result, Is.InstanceOf<ViewResult>());
        }
        
        [Test]
        public void AddUser_Post_WithValidFormData_ReturnsRedirectToIndex()
        {
            // Arrange
            var controller = new AdminController(); // Assumes parameterless constructor
            controller.ModelState.Clear(); // Ensure ModelState is valid

            var formData = new FormCollection(new Dictionary<string, Microsoft.Extensions.Primitives.StringValues>
    {
        { "FullName", "Test User" },
        { "Email", "test@example.com" },
        { "Password", "TestPassword123" },
        { "PhoneNumber", "1234567890" },
        { "AadhaarNo", "123412341234" },
        { "Username", "testuser" },
        { "Dob", "1990-01-01" },
        { "City", "TestCity" },
        { "State", "TestState" },
        { "Gender", "Male" },
        { "Address", "123 Test Street" },
        { "SecurityQuestion", "Your pet's name?" },
        { "SecurityAnswer", "Fluffy" }
    });

            // Act
            var result = controller.AddUser(formData);

            // Assert
            Assert.That(result, Is.InstanceOf<RedirectToActionResult>());
            var redirect = result as RedirectToActionResult;
            Assert.That(redirect.ActionName, Is.EqualTo("Index"));
        }

         


        [Test]
        public void UpdateUser_Post_WithValidUser_ReturnsViewResult()
        {
            // Arrange
            var controller = new AdminController(); // Assumes parameterless constructor

            var updatedUser = new User
            {
                UserId = 1,
                FullName = "Updated Name",
                Username = "updatedusername",
                Email = "updated@example.com",
                Address = "Updated Address",
                City = "Updated City",
                State = "Updated State",
                PhoneNumber = "9876543210",
                AadhaarNo = "123412341234"
            };

            // Act
            var result = controller.UpdateUser(updatedUser);

            // Assert
            Assert.That(result, Is.InstanceOf<ViewResult>());
        }

        [Test]
        [TestCase("testuser1", "testuser1@example.com", "123412341234", "Female", "1234567890", "MySecurePassword", "MySecurePassword", "Fluffy", "What is your pet's name?", true, TestName = "Register_ValidUser_ReturnsRedirect")]
        public void Register_ValidUser_ReturnsRedirect(string username, string email, string aadhaarNo, string gender, string phoneNumber, string password, string confirmPassword, string securityAnswer, string securityQuestion, bool isValid)
        {
            // Arrange
            var controller = new VaccineController();
            controller.ModelState.Clear();
            var user = new User
            {
                Username = username,
                FullName = "Test User",
                Email = email,
                AadhaarNo = aadhaarNo,
                Gender = gender,
                Dob = new DateOnly(1990, 1, 1),
                PhoneNumber = phoneNumber,
                Address = "123 Test St",
                State = "TestState",
                City = "TestCity",
                SecurityAnswer = securityAnswer,
                SecurityQuestion = securityQuestion
            };

            // Act
            IActionResult result = controller.Register(user, password, confirmPassword, securityQuestion);

            // Assert
            if (isValid)
            {
                Assert.That(controller.ModelState.IsValid, Is.True, "ModelState is invalid");
                Assert.That(result, Is.InstanceOf<RedirectToActionResult>());
                var redirect = result as RedirectToActionResult;
                Assert.That(redirect.ActionName, Is.EqualTo("Login"));
            }
            else
            {
                Assert.That(result, Is.InstanceOf<ViewResult>());
            }

        }

        [Test]
        public void PostFeedback_ValidFeedback_ReturnsOk_WithInMemoryDb()
        {
            // Arrange
            // Ensure the InMemory provider is referenced and the using directive is present
            var options = new DbContextOptionsBuilder<vaccineManagementContext>()
                .UseSqlServer("Data Source = WKSBAN36SUHTR19\\SQLEXPRESS; Initial Catalog = vaccineManagement; Integrated Security = True; Encrypt = False")
                .Options;

            using (var context = new vaccineManagementContext(options))
            {
                // Add necessary data to satisfy foreign key constraints
                context.Users.Add(new User { UserId = 19, FullName = "Janwe Doe" });
                context.Vaccines.Add(new Vaccine { VaccineId = 144, VaccineName = "TestwVaccine" });
                context.SaveChanges();

                // You may need to update FeedbackController to accept context via DI for proper testing
                var controller = new FeedbackController();
                controller.ModelState.Clear();

                var feedback = new VaccinationFeedback
                {
                    UserId = 19,
                    VaccineId = 144,
                    
                    FullName = "Janwe Doe",
                    PhoneNumber = "9989266353",
                    Feedback = "Very helpful staff!",
                    Rating = 4,
                    SubmittedAt = DateTime.Now,
                };

                // Act
                var result = controller.PostFeedback(feedback) as OkObjectResult;

                // Assert
                Assert.That(result, Is.Not.Null, "Result is null");
                Assert.That(result?.StatusCode, Is.EqualTo(200), "Status code is not 200");
                Assert.That(result?.Value?.ToString(), Does.Contain("Feedback submitted successfully"), "Feedback message not found");
            }

           


        }
    }
}