using APIfilms.Models.EntityFramework;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Stateful_API_Code_first.Models;
using Stateful_API_Code_first.Models.Repository;

public class UserManager : IDataRepository<User>
{
    readonly FilmRatingsDBContext filmsDbContext;

    public UserManager(FilmRatingsDBContext context)
    {
        filmsDbContext = context;
    }

    public async Task<ActionResult<IEnumerable<User>>> GetAllAsync()
    {
        return await filmsDbContext.Users.ToListAsync();
    }

    public async Task<ActionResult<User>> GetByIdAsync(int id)
    {
        return await filmsDbContext.Users.FirstOrDefaultAsync(u => u.UserId == id);
    }

    public async Task<ActionResult<User>> GetByStringAsync(string mail)
    {
        return await filmsDbContext.Users.FirstOrDefaultAsync(u => u.Mail.ToUpper() == mail.ToUpper());
    }

    public async Task AddAsync(User entity)
    {
        await filmsDbContext.Users.AddAsync(entity);
        await filmsDbContext.SaveChangesAsync();
    }

    public async Task UpdateAsync(User user, User entity)
    {
        filmsDbContext.Entry(user).State = EntityState.Modified;

        user.UserId = entity.UserId;
        user.Mail = entity.Mail;
        // Be sure to add any other required properties for your User model here

        await filmsDbContext.SaveChangesAsync();
    }

    public async Task DeleteAsync(User user)
    {
        filmsDbContext.Users.Remove(user);
        await filmsDbContext.SaveChangesAsync();
    }
}