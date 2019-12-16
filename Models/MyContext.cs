using Microsoft.EntityFrameworkCore;

namespace bug_tracker.Models
{
    public class MyContext : DbContext
    {
        public MyContext(DbContextOptions options) : base(options) { }
        public DbSet<User> Users { get; set; }

    }
}