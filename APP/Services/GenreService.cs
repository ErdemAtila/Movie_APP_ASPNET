using APP.Domain;
using APP.Models;
using CORE.APP.Models;
using CORE.APP.Services;
using Microsoft.EntityFrameworkCore;

namespace APP.Services
{
    // Inherit from the generic entity service class therefore DbContext injected constructor can be automatically created
    // and entity CRUD (create, read, update, delete) methods in the base class can be invoked.
    public class GenreService : Service<Genre>, IService<GenreRequest, GenreResponse>
    {
        public GenreService(DbContext db) : base(db)
        {
        }

        // base virtual Query method is overriden therefore the overriden query can be used in all other methods
        protected override IQueryable<Genre> Query(bool isNoTracking = true)
        {
            return base.Query(isNoTracking) // will return Genres DbSet
                .OrderBy(g => g.Name); // query will be ordered ascending by Name values

            // OrderBy, OrderByDescending, ThenBy and ThenByDescending methods can also be used with DbSets.
        }

        public List<GenreResponse> List()
        {
            // get the query of all Genre entities then project each entity to GenreResponse object and return the list of GenreResponse objects
            return Query().Select(g => new GenreResponse // g: Genre entity delegate
            {
                // assigning entity properties to the response
                Guid = g.Guid,
                Id = g.Id,
                Name = g.Name
            }).ToList();
        }

        // get a single Genre entity by Id then project the entity to GenreResponse object and return the GenreResponse object
        public GenreResponse Item(int id)
        {
            var entity = Query().SingleOrDefault(g => g.Id == id); // g: Genre entity delegate
            if (entity is null)
                return null;
            return new GenreResponse
            {
                // assigning entity properties to the response
                Guid = entity.Guid,
                Id = entity.Id,
                Name = entity.Name
            };
        }

        // get a single Genre entity by Id then project the entity to GenreRequest object and return the GenreRequest object
        public GenreRequest Edit(int id)
        {
            var entity = Query().SingleOrDefault(g => g.Id == id); // g: Genre entity delegate
            if (entity is null)
                return null;
            return new GenreRequest
            {
                // assigning entity properties to the request
                Id = entity.Id,
                Name = entity.Name
            };
        }

        // create a new Genre entity from the GenreRequest object and save it to the database if a genre with the same name does not exist
        public CommandResponse Create(GenreRequest request)
        {
            // g: Genre entity delegate. Check if a genre with the same name exists
            // (case-sensitive, request.Name without white space characters in the beginning and at the end).
            if (Query().Any(g => g.Name == request.Name.Trim()))
                return Error("Genre with the same name already exists!");
            var entity = new Genre
            {
                Name = request.Name?.Trim() // remove white space characters in the beginning and at the end
            };
            Create(entity); // will add the entity to the Genres DbSet and since save default parameter's value is true, will save changes to the database
            return Success("Genre created successfully.", entity.Id);
        }

        // get the Genre entity by Id then update the entity from the GenreRequest object and save changes to the database
        // if a genre other than the current updated genre with the same name does not exist
        public CommandResponse Update(GenreRequest request)
        {
            // g: Genre entity delegate. Check if a genre excluding the current updated genre with the same name exists
            // (case-sensitive, request.Name without white space characters in the beginning and at the end).
            if (Query().Any(g => g.Id != request.Id && g.Name == request.Name.Trim()))
                return Error("Genre with the same name already exists!");
            // get the Genre entity by ID from the Genres table
            var entity = Query(false).SingleOrDefault(g => g.Id == request.Id); // isNoTracking is false for being tracked by EF Core to update the entity
            if (entity is null)
                return Error("Genre not found!");
            // update retrieved Genre entity's properties with request properties
            entity.Name = request.Name?.Trim();
            Update(entity); // will update the entity in the Genres DbSet and since save default parameter's value is true, will save changes to the database
            return Success("Genre updated successfully.", entity.Id);
        }

        // get the Genre entity by Id then delete the entity from the database
        public CommandResponse Delete(int id)
        {
            // get the Genre entity by ID from the Genres table
            var entity = Query(false).SingleOrDefault(g => g.Id == id); // isNoTracking is false for being tracked by EF Core to delete the entity
            if (entity is null)
                return Error("Genre not found!");
            // delete the Genre entity
            Delete(entity); // will delete the entity from the Genres DbSet and since save default parameter's value is true, will save changes to the database
            return Success("Genre deleted successfully.", entity.Id);
        }
    }
}

