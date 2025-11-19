using APP.Domain;
using APP.Models;
using CORE.APP.Models;
using CORE.APP.Services;
using Microsoft.EntityFrameworkCore;

namespace APP.Services
{
    // Inherit from the generic entity service class therefore DbContext injected constructor can be automatically created
    // and entity CRUD (create, read, update, delete) methods in the base class can be invoked.
    public class DirectorService : Service<Director>, IService<DirectorRequest, DirectorResponse>
    {
        public DirectorService(DbContext db) : base(db)
        {
        }

        // base virtual Query method is overriden therefore the overriden query can be used in all other methods
        protected override IQueryable<Director> Query(bool isNoTracking = true)
        {
            return base.Query(isNoTracking) // will return Directors DbSet
                .Include(d => d.Movies)
                .OrderBy(d => d.LastName) // query will be ordered ascending by LastName values
                .ThenBy(d => d.FirstName); // after LastName ordering, query will be ordered ascending by FirstName values

            // OrderBy, OrderByDescending, ThenBy and ThenByDescending methods can also be used with DbSets.
        }

        public List<DirectorResponse> List()
        {
            // get the query of all Director entities then project each entity to DirectorResponse object and return the list of DirectorResponse objects
            return Query().Select(d => new DirectorResponse // d: Director entity delegate
            {
                // assigning entity properties to the response
                Guid = d.Guid,
                Id = d.Id,
                FirstName = d.FirstName,
                LastName = d.LastName,
                IsRetired = d.IsRetired,

                // assigning custom or formatted properties to the response
                IsRetiredF = d.IsRetired ? "Yes" : "No",
                MovieCount = d.Movies.Count,
                MovieNames = d.Movies.Any()
                    ? string.Join(", ", d.Movies.Select(m => m.Name))
                    : string.Empty
            }).ToList();
        }

        // get a single Director entity by Id then project the entity to DirectorResponse object and return the DirectorResponse object
        public DirectorResponse Item(int id)
        {
            var entity = Query().SingleOrDefault(d => d.Id == id); // d: Director entity delegate
            if (entity is null)
                return null;
            return new DirectorResponse
            {
                // assigning entity properties to the response
                Guid = entity.Guid,
                Id = entity.Id,
                FirstName = entity.FirstName,
                LastName = entity.LastName,
                IsRetired = entity.IsRetired,

                // assigning custom or formatted properties to the response
                IsRetiredF = entity.IsRetired ? "Yes" : "No",
                MovieCount = entity.Movies.Count,
                MovieNames = entity.Movies.Any()
                    ? string.Join(", ", entity.Movies.Select(m => m.Name))
                    : string.Empty
            };
        }

        // get a single Director entity by Id then project the entity to DirectorRequest object and return the DirectorRequest object
        public DirectorRequest Edit(int id)
        {
            var entity = Query().SingleOrDefault(d => d.Id == id); // d: Director entity delegate
            if (entity is null)
                return null;
            return new DirectorRequest
            {
                // assigning entity properties to the request
                Id = entity.Id,
                FirstName = entity.FirstName,
                LastName = entity.LastName,
                IsRetired = entity.IsRetired
            };
        }

        // create a new Director entity from the DirectorRequest object and save it to the database if a director with the same first and last name does not exist
        public CommandResponse Create(DirectorRequest request)
        {
            // d: Director entity delegate. Check if a director with the same first and last name exists
            // (case-sensitive, request.FirstName and request.LastName without white space characters in the beginning and at the end).
            if (Query().Any(d => d.FirstName == request.FirstName.Trim() && d.LastName == request.LastName.Trim()))
                return Error("Director with the same name already exists!");
            var entity = new Director
            {
                FirstName = request.FirstName?.Trim(), // remove white space characters in the beginning and at the end
                LastName = request.LastName?.Trim(), // remove white space characters in the beginning and at the end
                IsRetired = request.IsRetired
            };
            Create(entity); // will add the entity to the Directors DbSet and since save default parameter's value is true, will save changes to the database
            return Success("Director created successfully.", entity.Id);
        }

        // get the Director entity by Id then update the entity from the DirectorRequest object and save changes to the database
        // if a director other than the current updated director with the same first and last name does not exist
        public CommandResponse Update(DirectorRequest request)
        {
            // d: Director entity delegate. Check if a director excluding the current updated director with the same first and last name exists
            // (case-sensitive, request.FirstName and request.LastName without white space characters in the beginning and at the end).
            if (Query().Any(d => d.Id != request.Id && d.FirstName == request.FirstName.Trim() && d.LastName == request.LastName.Trim()))
                return Error("Director with the same name already exists!");
            // get the Director entity by ID from the Directors table
            var entity = Query(false).SingleOrDefault(d => d.Id == request.Id); // isNoTracking is false for being tracked by EF Core to update the entity
            if (entity is null)
                return Error("Director not found!");
            // update retrieved Director entity's properties with request properties
            entity.FirstName = request.FirstName?.Trim();
            entity.LastName = request.LastName?.Trim();
            entity.IsRetired = request.IsRetired;
            Update(entity); // will update the entity in the Directors DbSet and since save default parameter's value is true, will save changes to the database
            return Success("Director updated successfully.", entity.Id);
        }

        // get the Director entity by Id then delete the entity from the database
        public CommandResponse Delete(int id)
        {
            // get the Director entity by ID from the Directors table
            var entity = Query(false).SingleOrDefault(d => d.Id == id); // isNoTracking is false for being tracked by EF Core to delete the entity
            if (entity is null)
                return Error("Director not found!");

            // Way 2: recommended
            if (entity.Movies.Count > 0) // if (entity.Movies.Any())
                return Error("Director can't be deleted because it has relational movies!");

            // delete the Director entity
            Delete(entity); // will delete the entity from the Directors DbSet and since save default parameter's value is true, will save changes to the database
            return Success("Director deleted successfully.", entity.Id);
        }
    }
}

