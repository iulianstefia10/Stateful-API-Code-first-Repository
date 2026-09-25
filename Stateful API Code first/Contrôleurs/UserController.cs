using APIfilms.Models.EntityFramework;
using Microsoft.AspNetCore.Mvc;
using Stateful_API_Code_first.Models;
using Stateful_API_Code_first.Models.Repository;

namespace Stateful_API_Code_first.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UtilisateursController : ControllerBase
    {
        private readonly IDataRepository<User> dataRepository;

        public UtilisateursController(IDataRepository<User> dataRepo)
        {
            dataRepository = dataRepo;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<User>>> GetUsers()
        {
            // Added await and Async suffix
            return await dataRepository.GetAllAsync();
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<User>> GetUtilisateurById(int id)
        {
            // Added await and Async suffix
            var user = await dataRepository.GetByIdAsync(id);

            if (user.Value == null)
            {
                return NotFound();
            }

            return user;
        }

        [HttpGet("GetByEmail/{email}")]
        public async Task<ActionResult<User>> GetUserByEmail(string email)
        {
            // Added await and Async suffix
            var user = await dataRepository.GetByStringAsync(email);

            if (user.Value == null)
            {
                return NotFound();
            }

            return user;
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> PutUser(int id, User user)
        {
            if (id != user.UserId)
            {
                return BadRequest();
            }

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            // Added await and Async suffix
            var userToUpdate = await dataRepository.GetByIdAsync(id);

            if (userToUpdate.Value == null)
            {
                return NotFound();
            }
            else
            {
                // Added await and Async suffix
                await dataRepository.UpdateAsync(userToUpdate.Value, user);
                return NoContent();
            }
        }

        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<User>> PostUser(User user)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            // Added await and Async suffix
            await dataRepository.AddAsync(user);

            return CreatedAtAction("GetUtilisateurById", new { id = user.UserId }, user);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteUser(int id)
        {
            // Added await and Async suffix
            var user = await dataRepository.GetByIdAsync(id);

            if (user.Value == null)
            {
                return NotFound();
            }

            // Added await and Async suffix
            await dataRepository.DeleteAsync(user.Value);

            return NoContent();
        }
    }
}