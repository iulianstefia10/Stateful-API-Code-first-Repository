using APIfilms.Models.EntityFramework;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Stateful_API_Code_first.Controllers;
using Stateful_API_Code_first.Models;
using Stateful_API_Code_first.Models.Repository;
using System;
using System.Linq;

namespace Stateful_API_Code_first_Tests
{
    [TestClass]
    public class UtilisateursControllerTests
    {
        private UtilisateursController controller;
        private FilmRatingsDBContext context;
        private IDataRepository<User> dataRepository;

        [TestInitialize]
        public void Setup()
        {
            var builder = new DbContextOptionsBuilder<FilmRatingsDBContext>();
            builder.UseNpgsql("Server=localhost;Port=5432;Database=FilmRatingsDB;User Id=postgres;Password=parola_ta;");

            context = new FilmRatingsDBContext(builder.Options);
            dataRepository = new UserManager(context);
            controller = new UtilisateursController(dataRepository);
        }

        

        [TestMethod]
        public void GetUsers_ReturnsAllUsers_OK()
        {
            var result = controller.GetUsers().Result;
            var resultList = result.Value?.ToList();
            var dbList = context.Users.ToList();

            Assert.IsNotNull(resultList);
            Assert.AreEqual(dbList.Count, resultList.Count, "Numărul de utilizatori returnați diferă de cel din BD.");
        }

        [TestMethod]
        public void GetUtilisateurById_IdExists_ReturnsUser()
        {
            var firstUser = context.Users.FirstOrDefault();
            if (firstUser == null) Assert.Inconclusive("Nu există utilizatori în BD.");

            var result = controller.GetUtilisateurById(firstUser.UserId).Result;

            Assert.IsNotNull(result.Value);
            Assert.AreEqual(firstUser.UserId, result.Value.UserId);
        }

        [TestMethod]
        public void GetUtilisateurById_IdNotExists_ReturnsNotFound()
        {
            var result = controller.GetUtilisateurById(-1).Result;
            Assert.IsInstanceOfType(result.Result, typeof(NotFoundResult));
        }

        [TestMethod]
        public void GetUserByEmail_EmailExists_ReturnsUser()
        {
            var firstUser = context.Users.FirstOrDefault();
            if (firstUser == null) Assert.Inconclusive("Nu există utilizatori în BD.");

            var result = controller.GetUserByEmail(firstUser.Mail).Result;

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

            var result = controller.PostUser(userAtester).Result;

            var userRecupere = context.Users.FirstOrDefault(u => u.Mail.ToUpper() == userAtester.Mail.ToUpper());
            Assert.IsNotNull(userRecupere);
            Assert.AreEqual(userAtester.Mail, userRecupere.Mail);
        }

        [TestMethod]
        public void PostUtilisateur_InvalidModel_ReturnsBadRequest()
        {
            User invalidUser = new User() { Mobile = "1" };
            controller.ModelState.AddModelError("Mobile", "The mobile number must contain 10 digits");

            var result = controller.PostUser(invalidUser).Result;
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

            context.Users.Add(tempUser);
            context.SaveChanges();

            int createdId = tempUser.UserId;

            var result = controller.DeleteUser(createdId).Result;
            Assert.IsInstanceOfType(result, typeof(NoContentResult));

            var userInDb = context.Users.FirstOrDefault(u => u.UserId == createdId);
            Assert.IsNull(userInDb);
        }

        

        [TestMethod]
        public void PostUtilisateur_ModelValidated_CreationOK_AvecMoq()
        {
            var mockRepository = new Mock<IDataRepository<User>>();
            var userController = new UtilisateursController(mockRepository.Object);

            User user = new User
            {
                Name = "FISH",
                FirstName = "Pascal",
                Mobile = "1",
                Mail = "poisson@gmail.com",
                Pwd = "Toto12345678!",
                Street = "Chemin de Bellevue",
                PostalCode = "74940",
                City = "Annecy-le-Vieux",
                Country = "France"
            };

            var actionResult = userController.PostUser(user).Result;

            Assert.IsInstanceOfType(actionResult, typeof(ActionResult<User>), "Not an ActionResult<User>");
            Assert.IsInstanceOfType(actionResult.Result, typeof(CreatedAtActionResult), "Not a CreatedAtActionResult");

            var result = actionResult.Result as CreatedAtActionResult;
            Assert.IsInstanceOfType(result.Value, typeof(User), "Not a User");

            user.UserId = ((User)result.Value).UserId;
            Assert.AreEqual(user, (User)result.Value, "Users not identical");
        }

        [TestMethod]
        public void GetUtilisateurById_ExistingIdPassed_ReturnsRightItem_AvecMoq()
        {
            User user = new User
            {
                UserId = 1,
                Name = "Calida",
                FirstName = "Lilley",
                Mobile = "0653930778",
                Mail = "clilleymd@last.fm",
                Pwd = "Toto12345678!",
                Street = "Impasse des bergeronnettes",
                PostalCode = "74200",
                City = "Allinges",
                Country = "France"
            };

            var mockRepository = new Mock<IDataRepository<User>>();
            mockRepository.Setup(x => x.GetByIdAsync(1).Result).Returns(user);
            var userController = new UtilisateursController(mockRepository.Object);

            var actionResult = userController.GetUtilisateurById(1).Result;

            Assert.IsNotNull(actionResult);
            Assert.IsNotNull(actionResult.Value);
            Assert.AreEqual(user, actionResult.Value as User);
        }

        [TestMethod]
        public void GetUtilisateurById_UnknownIdPassed_ReturnsNotFoundResult_AvecMoq()
        {
            var mockRepository = new Mock<IDataRepository<User>>();
            var userController = new UtilisateursController(mockRepository.Object);

            var actionResult = userController.GetUtilisateurById(0).Result;

            Assert.IsInstanceOfType(actionResult.Result, typeof(NotFoundResult));
        }

        [TestMethod]
        public void GetUserByEmail_ExistingEmailPassed_ReturnsRightItem_AvecMoq()
        {
            User user = new User
            {
                UserId = 1,
                Mail = "clilleymd@last.fm"
            };

            var mockRepository = new Mock<IDataRepository<User>>();
            mockRepository.Setup(x => x.GetByStringAsync("clilleymd@last.fm").Result).Returns(user);
            var userController = new UtilisateursController(mockRepository.Object);

            var actionResult = userController.GetUserByEmail("clilleymd@last.fm").Result;

            Assert.IsNotNull(actionResult);
            Assert.IsNotNull(actionResult.Value);
            Assert.AreEqual(user, actionResult.Value as User);
        }

        [TestMethod]
        public void GetUserByEmail_UnknownEmailPassed_ReturnsNotFoundResult_AvecMoq()
        {
            var mockRepository = new Mock<IDataRepository<User>>();
            var userController = new UtilisateursController(mockRepository.Object);

            var actionResult = userController.GetUserByEmail("inconnu@test.com").Result;

            Assert.IsInstanceOfType(actionResult.Result, typeof(NotFoundResult));
        }

        [TestMethod]
        public void DeleteUtilisateurTest_AvecMoq()
        {
            User user = new User
            {
                UserId = 1,
                Name = "Calida",
                FirstName = "Lilley"
            };

            var mockRepository = new Mock<IDataRepository<User>>();
            
            mockRepository.Setup(x => x.GetByIdAsync(1).Result).Returns(user);
            var userController = new UtilisateursController(mockRepository.Object);

            var actionResult = userController.DeleteUser(1).Result;

          
            Assert.IsInstanceOfType(actionResult, typeof(NoContentResult), "Not a NoContentResult");
        }

        [TestMethod]
        public void PutUtilisateur_ModelValidated_UpdateOK_AvecMoq()
        {
           
            User userToUpdate = new User { UserId = 1, Name = "Ancien Nom", Mail = "old@test.com" };
            User updatedUser = new User { UserId = 1, Name = "Nouveau Nom", Mail = "new@test.com" };

            var mockRepository = new Mock<IDataRepository<User>>();
           
            mockRepository.Setup(x => x.GetByIdAsync(1).Result).Returns(userToUpdate);

            var userController = new UtilisateursController(mockRepository.Object);

            
            var actionResult = userController.PutUser(1, updatedUser).Result;

            
            Assert.IsInstanceOfType(actionResult, typeof(NoContentResult), "Not a NoContentResult");
        }
    }
}