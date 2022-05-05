using Eadent.Common.DataAccess.EntityFramework.Databases;
using Microsoft.EntityFrameworkCore;
using Rapture.Therapy.Configuration;
using Rapture.Therapy.DataAccess.RaptureTherapy.Entities;

namespace Rapture.Therapy.DataAccess.RaptureTherapy.Databases
{
    public class RaptureTherapyDatabase : BaseDatabase, IRaptureTherapyDatabase
    {
        // Database Tables.
        public virtual DbSet<RaptureTherapyDatabaseVersionEntity> RaptureTherapyDatabaseVersions { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.HasDefaultSchema(RaptureTherapySettings.Instance.Database.DatabaseSchema);

            base.OnModelCreating(modelBuilder);
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            base.OnConfiguring(optionsBuilder);
        }

        public RaptureTherapyDatabase(DbContextOptions<RaptureTherapyDatabase> options) : base(options)
        {
            DatabaseName = RaptureTherapySettings.Instance.Database.DatabaseName;
            DatabaseSchema = RaptureTherapySettings.Instance.Database.DatabaseSchema;
        }
    }
}
