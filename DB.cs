using System.Data.Entity;
using Server;

namespace курсач3сервер
{
    public class DBContext : DbContext
    {
        
        public DBContext()
            : base("name=DB")
        {
        }

        public DbSet<User> Users { get; set; }

        public DbSet<Admin> Admins { get; set; }
        public DbSet<Location> Locations { get; set; }
        public DbSet<CustomsControlPoint> CustomsControlPoints { get; set; }
        public DbSet<Vote> votes { get; set; }
        public DbSet<CustomControlPointNewTime> newPoint { get; set; }
    }
}