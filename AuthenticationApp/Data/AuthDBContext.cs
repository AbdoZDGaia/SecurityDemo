using Microsoft.EntityFrameworkCore;

namespace AuthenticationApp.Data
{
    public class AuthDBContext : DbContext
    {
        public AuthDBContext(DbContextOptions<AuthDBContext> options) : base(options)
        {
        }

        public DbSet<AppUser> AppUsers { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<AppUser>(entity =>
            {
                entity.HasKey(e=>e.UserId);
                entity.Property(e => e.UserId);
                entity.Property(e => e.Provider).HasMaxLength(250);
                entity.Property(e => e.NameIdentifier).HasMaxLength(500);
                entity.Property(e => e.UserName).HasMaxLength(250);
                entity.Property(e => e.Password).HasMaxLength(250);
                entity.Property(e => e.Email).HasMaxLength(250);
                entity.Property(e => e.FirstName).HasMaxLength(250);
                entity.Property(e => e.LastName).HasMaxLength(250);
                entity.Property(e => e.Mobile).HasMaxLength(250);
                entity.Property(e => e.Roles).HasMaxLength(1000);

                entity.HasData(new AppUser
                {
                    Provider = "Cookies",
                    UserId = 1,
                    UserName = "AbdoZ",
                    Password = "P@ssw0rd",
                    Email = "oldtimeprogrammer@yahoo.com",
                    FirstName = "Abdulrahman",
                    LastName = "Seliem",
                    Mobile = "+201111012606",
                    Roles = "Admin,User"
                });
            });


        }
    }
}
