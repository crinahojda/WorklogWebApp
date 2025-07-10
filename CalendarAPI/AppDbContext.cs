using Microsoft.EntityFrameworkCore;
namespace CalendarAPI

{
    public class AppDbContext : DbContext
    {

        // Definim tabelele pentru fiecare entitate
        public DbSet<Worker> Workers { get; set; } 
        public DbSet<WorkDay> WorkDays { get; set; }
        public DbSet<WorkTime> WorkTimes { get; set; }
        public DbSet<Month> Months { get; set; }

        // Constructorul pentru configurarea conexiunii
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }
    }


}
