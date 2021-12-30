using Eadent.Common.DataAccess.Repositories;
using Rapture.Therapy.DataAccess.RaptureTherapy.Databases;
using Rapture.Therapy.DataAccess.RaptureTherapy.Entities;

namespace Rapture.Therapy.DataAccess.RaptureTherapy.Repositories
{
    public class RaptureTherapyDatabaseVersionsRepository : BaseRepository<IRaptureTherapyDatabase, RaptureTherapyDatabaseVersionEntity, int>, IRaptureTherapyDatabaseVersionsRepository
    {
        public RaptureTherapyDatabaseVersionsRepository(IRaptureTherapyDatabase database) : base(database)
        {
        }
    }
}
