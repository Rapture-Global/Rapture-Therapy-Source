using Eadent.Common.DataAccess.EntityFramework.Repositories;
using Rapture.Therapy.DataAccess.RaptureTherapy.Entities;

namespace Rapture.Therapy.DataAccess.RaptureTherapy.Repositories
{
    public interface IRaptureTherapyDatabaseVersionsRepository : IBaseRepository<RaptureTherapyDatabaseVersionEntity, int>
    {
    }
}
