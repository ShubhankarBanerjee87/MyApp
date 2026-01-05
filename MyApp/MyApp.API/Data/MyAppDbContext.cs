using Microsoft.EntityFrameworkCore;

namespace MyApp.API.Data
{
    public class MyAppDbContext : DbContext
    {
        public MyAppDbContext(DbContextOptions options) : base(options)
        {

        }
    }
}
