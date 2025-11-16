using CORE.APP.Domain;
using Microsoft.EntityFrameworkCore;

namespace CORE.APP.Services
{
    public abstract class Service<TEntity> : ServiceBase, IDisposable 
        where TEntity : Entity, new()
    {
        protected readonly DbContext _db;

        protected Service(DbContext db)
        {
            _db = db;
        }

        protected virtual IQueryable<TEntity> Query(bool isNoTracking = true)
        {
            return isNoTracking ? _db.Set<TEntity>().AsNoTracking() : _db.Set<TEntity>();
        }

        protected void Create(TEntity entity, bool save = true)
        {
            entity.Guid = Guid.NewGuid().ToString();
            _db.Set<TEntity>().Add(entity);
            if (save)
                Save();
        }

        protected void Update(TEntity entity, bool save = true)
        {
            _db.Set<TEntity>().Update(entity);
            if (save)
                Save();
        }

        protected void Delete(TEntity entity, bool save = true)
        {
            _db.Set<TEntity>().Remove(entity);
            if (save)
                Save();
        }

        protected virtual int Save() => _db.SaveChanges();

        public void Dispose()
        {
            _db.Dispose();
            GC.SuppressFinalize(this);
        }
    }
}
