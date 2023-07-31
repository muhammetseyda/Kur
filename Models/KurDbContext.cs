using Microsoft.EntityFrameworkCore;
namespace Kur.Models
{
    public class KurDbContext : DbContext
    {
        public KurDbContext(DbContextOptions<KurDbContext> options) : base (options)
        {

        }

        public DbSet<TcmbKur> TcmbKurlar { get; set; }

        public async Task<bool> SaveTcmbKurAsync(TcmbKur tcmbKur)
        {
            try
            {
                TcmbKurlar.Add(tcmbKur);
                await SaveChangesAsync();
                return true;
            }
            catch
            {
                return false;
            }
        }
    }


}
