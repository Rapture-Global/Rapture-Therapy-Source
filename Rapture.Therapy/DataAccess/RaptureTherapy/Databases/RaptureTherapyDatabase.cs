using Eadent.Common.DataAccess.EntityFramework.Databases;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Rapture.Therapy.Configuration;
using Rapture.Therapy.DataAccess.RaptureTherapy.Entities;

namespace Rapture.Therapy.DataAccess.RaptureTherapy.Databases
{
    public class RaptureTherapyDatabase : BaseDatabase, IRaptureTherapyDatabase
    {
        // Attributes/Properties.
        private RaptureTherapySettings RaptureTherapySettings{ get; }

        // Database Tables.
        public virtual DbSet<RaptureTherapyDatabaseVersionEntity> RaptureTherapyDatabaseVersions { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.HasDefaultSchema(RaptureTherapySettings.Database.DatabaseSchema);

            base.OnModelCreating(modelBuilder);
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            base.OnConfiguring(optionsBuilder);
        }

        public RaptureTherapyDatabase(IConfiguration configuration, DbContextOptions<RaptureTherapyDatabase> options) : base(options)
        {
            RaptureTherapySettings = configuration.GetSection(RaptureTherapySettings.SectionName).Get<RaptureTherapySettings>();

            DatabaseName = RaptureTherapySettings.Database.DatabaseName;
            DatabaseSchema = RaptureTherapySettings.Database.DatabaseSchema;
        }
    }
}
