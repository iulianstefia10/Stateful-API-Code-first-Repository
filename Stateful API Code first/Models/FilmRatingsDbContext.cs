using APIfilms.Models.EntityFramework;
using Microsoft.EntityFrameworkCore;


namespace Stateful_API_Code_first.Models
{
    public class FilmRatingsDBContext : DbContext
    {
        public FilmRatingsDBContext()
        {

        }

        public FilmRatingsDBContext(DbContextOptions<FilmRatingsDBContext> options) : base(options)
        {
        }

        public virtual DbSet<Film> Films { get; set; } = null!;
        public virtual DbSet<Rating> Ratings { get; set; } = null!;
        public virtual DbSet<User> Users { get; set; } = null!;

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {

            if (!optionsBuilder.IsConfigured)
            {

                // optionsBuilder.UseNpgsql("Server=localhost;Port=5432;Database=FilmRatingsDB;User Id=postgres;Password=parola_ta;");
            }
        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Rating>()
                .HasKey(r => new { r.UserId, r.FilmId })
                .HasName("pk_not");

            modelBuilder.Entity<Rating>()
              .HasCheckConstraint("ck_not_note", "not_note between 0 and 5");

            modelBuilder.Entity<Rating>()
                .HasOne(r => r.UserNotator)
                .WithMany(u => u.UserRatings)
                .HasForeignKey(r => r.UserId)
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("fk_not_utl");

            modelBuilder.Entity<Rating>()
                .HasOne(r => r.MovieNote)
                .WithMany(f => f.NotesFilm)
                .HasForeignKey(r => r.FilmId)
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("fk_not_flm");

            
            modelBuilder.Entity<User>()
                .HasIndex(u => u.Mail)
                .IsUnique()
                .HasDatabaseName("uq_utl_mail");

           
            modelBuilder.Entity<User>()
                .Property(u => u.Country)
                .HasDefaultValue("France");

            modelBuilder.Entity<User>()
                .Property(u => u.CreationDate)
                .HasDefaultValueSql("now()");
        }

    }
}
