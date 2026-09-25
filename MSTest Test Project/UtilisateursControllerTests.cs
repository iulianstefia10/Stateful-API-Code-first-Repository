using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Stateful_API_Code_first.Controllers;
using Stateful_API_Code_first.Models;
using APIfilms.Models.EntityFramework;

namespace Stateful_API_Code_first_Tests
{
    [TestClass]
    public class UtilisateursControllerTests
    {
        private FilmRatingsDBContext _context;
        private UtilisateursController _controller;

        [TestInitialize]
        public void Setup()
        {
            // ATENȚIE: Pune parola ta reală pentru PostgreSQL aici!
            var builder = new DbContextOptionsBuilder<FilmRatingsDBContext>();
            builder.UseNpgsql("Server=localhost;Port=5432;Database=FilmRatingsDB;User Id=postgres;Password=parola_ta;");

            _context = new FilmRatingsDBContext(builder.Options);
            _controller = new UtilisateursController(_context);
        }

        

        [TestMethod]
        public void GetUsers_ReturnsAllUsers_OK()
        {
            var result = _controller.GetUsers().Result;
            var resultList = result.Value?.ToList();
            var dbList = _context.Users.ToList();

            Assert.IsNotNull(resultList);
            Assert.AreEqual(dbList.Count, resultList.Count, "Numărul de utilizatori returnați diferă de cel din BD.");
        }

        [TestMethod]
        public void GetUtilisateurById_IdExists_ReturnsUser()
        {
            var firstUser = _context.Users.FirstOrDefault();
            if (firstUser == null) Assert.Inconclusive("Nu există utilizatori în BD.");

            var result = _controller.GetUtilisateurById(firstUser.UserId).Result;

            Assert.IsNotNull(result.Value);
            Assert.AreEqual(firstUser.UserId, result.Value.UserId);
        }

        [TestMethod]
        public void GetUtilisateurById_IdNotExists_ReturnsNotFound()
        {
            var result = _controller.GetUtilisateurById(-1).Result;
            Assert.IsInstanceOfType(result.Result, typeof(NotFoundResult));
        }

        [TestMethod]
        public void GetUserByEmail_EmailExists_ReturnsUser()
        {
            var firstUser = _context.Users.FirstOrDefault();
            if (firstUser == null) Assert.Inconclusive("Nu există utilizatori în BD.");

            var result = _controller.GetUserByEmail(firstUser.Mail).Result;

            Assert.IsNotNull(result.Value);
            Assert.AreEqual(firstUser.Mail, result.Value.Mail);
        }

      

        [TestMethod]
        public void PostUtilisateur_ModelValidated_CreationOK()
        {
            int num = new Random().Next(1, 1000000);

            User userAtester = new User()
            {
                Name = "Test",
                FirstName = "Unu",
                Mail = "test" + num + "@gmail.com",
                Pwd = "Pass!",
                CreationDate = DateTime.Now
            };

            var result = _controller.PostUser(userAtester).Result;

            var userRecupere = _context.Users.FirstOrDefault(u => u.Mail.ToUpper() == userAtester.Mail.ToUpper());
            Assert.IsNotNull(userRecupere);
            Assert.AreEqual(userAtester.Mail, userRecupere.Mail);
        }

        [TestMethod]
        public void PostUtilisateur_InvalidModel_ReturnsBadRequest()
        {
            User invalidUser = new User() { Mobile = "1" };

          
            _controller.ModelState.AddModelError("Mobile", "The mobile number must contain 10 digits");

            var result = _controller.PostUser(invalidUser).Result;
            Assert.IsInstanceOfType(result.Result, typeof(BadRequestObjectResult));
        }

   

        [TestMethod]
        public void DeleteUser_DeletesFromDatabase_ReturnsNoContent()
        {
            User tempUser = new User()
            {
                Name = "De Sters",
                Mail = "delete" + new Random().Next(1, 100000) + "@test.com",
                Pwd = "Pass"
            };

            _context.Users.Add(tempUser);
            _context.SaveChanges();

            int createdId = tempUser.UserId;

            var result = _controller.DeleteUser(createdId).Result;
            Assert.IsInstanceOfType(result, typeof(NoContentResult));

            var userInDb = _context.Users.FirstOrDefault(u => u.UserId == createdId);
            Assert.IsNull(userInDb);
        }
    }
}