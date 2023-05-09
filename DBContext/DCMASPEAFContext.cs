using EmeccaRestfulApi.Models;
using Microsoft.EntityFrameworkCore;

namespace EmeccaRestfulApi.DBContext
{
    public class DCMASPEAFContext: DbContext
    {
        public DCMASPEAFContext() : base()
        {
        }
        public DCMASPEAFContext(DbContextOptions<DCMASPEAFContext> options) : base(options)
        {
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            var configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json")
                .Build();

            var connectionString = configuration.GetConnectionString("AppConnection");
            optionsBuilder.UseSqlServer(connectionString);
        }

        #region Properties
        public DbSet<STUDYINFOVO> study_info_vo { get; set; }
        public DbSet<STORAGEINFOVO> storage_info_vo { get; set; }
        #endregion Properties
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<STUDYINFOVO>()
                .ToTable("STUDY_INFO").HasNoKey();
            modelBuilder.Entity<STORAGEINFOVO>()
                .ToTable("STORAGE_INFO").HasNoKey();
        }
    }
}
