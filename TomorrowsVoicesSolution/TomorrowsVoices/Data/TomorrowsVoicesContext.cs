using Microsoft.EntityFrameworkCore;
using TomorrowsVoices.Models;

namespace TomorrowsVoices.Data
{
    public class TomorrowsVoicesContext : DbContext
    {
        public TomorrowsVoicesContext(DbContextOptions<TomorrowsVoicesContext> options)
            : base(options) 
        {

        }

        DbSet<Director> Manager { get; set; }


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
        }
    }
}
