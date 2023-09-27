using Microsoft.EntityFrameworkCore;

namespace HelloWorld.Data
{
    public class DataContextEF : DbContext
    {
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                optionsBuilder.UseSqlServer("Server=localhost;Database=DotNetCourseDatabase;TrustServerCertificate=true;Trusted_Connection=true;",
                options => options.EnableRetryOnFailure());
            }
        }
    }
}