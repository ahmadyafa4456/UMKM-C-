using UMKM_C_.Data;
using UMKM_C_.IRepository.Repository;
using UMKM_C_.Models;

namespace UMKM_C_.Repository
{
    public class UnitOfWork : IUnitOfWork
    {
        private ApplicationDbContext db;
        public IBahanRepsitory Bahan { get; private set; }
        public UnitOfWork(ApplicationDbContext db)
        {
            this.db = db;
            Bahan = new BahanRepository(db);
        }

        public async Task Save()
        {
            await db.SaveChangesAsync();
        }
    }
}