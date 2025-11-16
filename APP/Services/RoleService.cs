using APP.Domain;
using APP.Models;
using CORE.APP.Models;
using CORE.APP.Services;
using Microsoft.EntityFrameworkCore;

namespace APP.Services
{
    // Inherit from the generic entity service class therefore DbContext injected constructor can be automatically created
    // and entity CRUD (create, read, update, delete) methods in the base class can be invoked.
    public class RoleService : Service<Role>, IService<RoleRequest, RoleResponse>
    {
        public RoleService(DbContext db) : base(db)
        {
        }

        // base virtual Query method is overriden therefore the overriden query can be used in all other methods
        protected override IQueryable<Role> Query(bool isNoTracking = true)
        {
            return base.Query(isNoTracking) // will return Roles DbSet
                .OrderBy(r => r.Name); // query will be ordered ascending by Name values

            // OrderBy, OrderByDescending, ThenBy and ThenByDescending methods can also be used with DbSets.
        }

        public List<RoleResponse> List()
        {
            // get the query of all Role entities then project each entity to RoleResponse object and return the list of RoleResponse objects
            return Query().Select(r => new RoleResponse // r: Role entity delegate
            {
                // assigning entity properties to the response
                Guid = r.Guid,
                Id = r.Id,
                Name = r.Name
            }).ToList();
        }

        // get a single Role entity by Id then project the entity to RoleResponse object and return the RoleResponse object
        public RoleResponse Item(int id)
        {
            var entity = Query().SingleOrDefault(r => r.Id == id); // r: Role entity delegate
            if (entity is null)
                return null;
            return new RoleResponse
            {
                // assigning entity properties to the response
                Guid = entity.Guid,
                Id = entity.Id,
                Name = entity.Name
            };
        }

        // get a single Role entity by Id then project the entity to RoleRequest object and return the RoleRequest object
        public RoleRequest Edit(int id)
        {
            var entity = Query().SingleOrDefault(r => r.Id == id); // r: Role entity delegate
            if (entity is null)
                return null;
            return new RoleRequest
            {
                // assigning entity properties to the request
                Id = entity.Id,
                Name = entity.Name
            };
        }

        // create a new Role entity from the RoleRequest object and save it to the database if a role with the same name does not exist
        public CommandResponse Create(RoleRequest request)
        {
            // r: Role entity delegate. Check if a role with the same name exists
            // (case-sensitive, request.Name without white space characters in the beginning and at the end).
            if (Query().Any(r => r.Name == request.Name.Trim()))
                return Error("Role with the same name already exists!");
            var entity = new Role
            {
                Name = request.Name?.Trim() // remove white space characters in the beginning and at the end
            };
            Create(entity); // will add the entity to the Roles DbSet and since save default parameter's value is true, will save changes to the database
            return Success("Role created successfully.", entity.Id);
        }

        // get the Role entity by Id then update the entity from the RoleRequest object and save changes to the database
        // if a role other than the current updated role with the same name does not exist
        public CommandResponse Update(RoleRequest request)
        {
            // r: Role entity delegate. Check if a role excluding the current updated role with the same name exists
            // (case-sensitive, request.Name without white space characters in the beginning and at the end).
            if (Query().Any(r => r.Id != request.Id && r.Name == request.Name.Trim()))
                return Error("Role with the same name already exists!");
            // get the Role entity by ID from the Roles table
            var entity = Query(false).SingleOrDefault(r => r.Id == request.Id); // isNoTracking is false for being tracked by EF Core to update the entity
            if (entity is null)
                return Error("Role not found!");
            // update retrieved Role entity's properties with request properties
            entity.Name = request.Name?.Trim();
            Update(entity); // will update the entity in the Roles DbSet and since save default parameter's value is true, will save changes to the database
            return Success("Role updated successfully.", entity.Id);
        }

        // get the Role entity by Id then delete the entity from the database
        public CommandResponse Delete(int id)
        {
            // get the Role entity by ID from the Roles table
            var entity = Query(false).SingleOrDefault(r => r.Id == id); // isNoTracking is false for being tracked by EF Core to delete the entity
            if (entity is null)
                return Error("Role not found!");
            // delete the Role entity
            Delete(entity); // will delete the entity from the Roles DbSet and since save default parameter's value is true, will save changes to the database
            return Success("Role deleted successfully.", entity.Id);
        }
    }
}
