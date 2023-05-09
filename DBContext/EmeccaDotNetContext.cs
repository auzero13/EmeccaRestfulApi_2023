using EmeccaRestfulApi.Models;
using Microsoft.EntityFrameworkCore;

namespace EmeccaRestfulApi.DBContext
{
    public class EmeccaDotNetContext : DbContext
    {
        public EmeccaDotNetContext() : base()
        {
        }
        public EmeccaDotNetContext(DbContextOptions<EmeccaDotNetContext> options) : base(options)
        {
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            var configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json")
                .Build();

            var connectionString = configuration.GetConnectionString("DefaultConnection");
            optionsBuilder.UseSqlServer(connectionString);
        }

        #region Properties
        public DbSet<EmeUserBasVO> emeuser_vo { get; set; }
        public DbSet<DeleteArchiveLogVO> delete_archive_log_vo { get; set; }
        public DbSet<EmeccaObjectIdVO> emecca_objid_vo { get; set; }

        #endregion Properties

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<EmeUserBasVO>()
                .ToTable("EmeUserBas").HasKey(e => e.ObjId);
            modelBuilder.Entity<DeleteArchiveLogVO>()
                .ToTable("DeleteArchiveLog").HasKey(e => e.ObjId);
            modelBuilder.Entity<EmeccaObjectIdVO>()
               .ToTable("EmeccaObjectId").HasKey(e => e.ObjId);
        }
    }
}
