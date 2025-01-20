using Microsoft.EntityFrameworkCore;
using System.Diagnostics;
using System.Numerics;

namespace TomorrowsVoices.Data
{
    public static class TomorrowsVoicesInitializer
    {
        public static void Initialize(IServiceProvider serviceProvider)
        {
            using (var context = new TomorrowsVoicesContext(
                serviceProvider.GetRequiredService<DbContextOptions<TomorrowsVoicesContext>>()))
            {
              
            }
        }
    }
}
