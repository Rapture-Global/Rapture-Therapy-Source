using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Rapture.Therapy.DataAccess.RaptureTherapy.Entities
{
    [Table("RaptureTherapyDatabaseVersions")]
    public class RaptureTherapyDatabaseVersionEntity
    {
        [Key]
        public int DatabaseVersionId { get; set; }

        public int Major { get; set; }

        public int Minor { get; set; }

        public int Patch { get; set; }

        public string Build { get; set; }

        public string Description { get; set; }

        public DateTime CreatedDateTimeUtc { get; set; }

        public DateTime? LastUpdatedDateTimeUtc { get; set; }
    }
}
