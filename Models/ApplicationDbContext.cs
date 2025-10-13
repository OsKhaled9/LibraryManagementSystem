using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Readify_Library.Settings;
using System.Reflection.Emit;

namespace Readify_Library.Models
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {

        }

        public DbSet<Category> Categories { get; set; }
        public DbSet<Book> Books { get; set; }
        public DbSet<Borrowing> Borrowings { get; set; }
        public DbSet<UserType> UsersTypes { get; set; }
        public DbSet<Notification> Notifications { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            // Enum Mapping: User TYpe -> Type Name
            builder.Entity<UserType>()
                   .Property(t => t.TypeName)
                   .HasConversion<string>(); // يخزن Enum كـ string في الجدول


            // Enum Mapping: Borrowing -> Status
            builder.Entity<Borrowing>()
                   .Property(t => t.Status)
                   .HasConversion<string>(); // يخزن Enum كـ string في الجدول


            // Seeding Data
            builder.Entity<UserType>().HasData(
               new UserType
               {
                   Id = 1,
                   TypeName = enTypesNames.Normal,
                   ExtraBooks = 0,
                   ExtraDays = 0,
                   ExtraPenalty = 0.00m
               },
               new UserType
               {
                   Id = 2,
                   TypeName = enTypesNames.Premium,
                   ExtraBooks = 2,
                   ExtraDays = 7,
                   ExtraPenalty = 10.00m
               },
               new UserType
               {
                   Id = 3,
                   TypeName = enTypesNames.VIP,
                   ExtraBooks = 5,
                   ExtraDays = 10,
                   ExtraPenalty = 5.00m
               }
            );

            builder.Entity<IdentityRole>().HasData
            (
                new IdentityRole
                {
                    Id = Guid.NewGuid().ToString(),
                    Name = SystemRoles.Admin,
                    NormalizedName = SystemRoles.Admin.ToUpper(),
                },
                new IdentityRole
                {
                    Id = Guid.NewGuid().ToString(),
                    Name = SystemRoles.User,
                    NormalizedName= SystemRoles.User.ToUpper(),
                }
            );
        }
    }
}
