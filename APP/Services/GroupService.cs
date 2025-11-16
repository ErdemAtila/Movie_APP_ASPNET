using APP.Domain;
using APP.Models;
using CORE.APP.Models;
using CORE.APP.Services;
using Microsoft.EntityFrameworkCore;

namespace APP.Services
{
    // Inherit from the generic entity service class therefore DbContext injected constructor can be automatically created
    // and entity CRUD (create, read, update, delete) methods in the base class can be invoked.
    public class GroupService : Service<Group>, IService<GroupRequest, GroupResponse>
    {
        public GroupService(DbContext db) : base(db)
        {
        }

        // base virtual Query method is overriden therefore the overriden query can be used in all other methods
        protected override IQueryable<Group> Query(bool isNoTracking = true)
        {
            return base.Query(isNoTracking) // will return Groups DbSet
                .Include(g => g.Users)
                .OrderBy(g => g.Name); // query will be ordered ascending by Name values

            // OrderBy, OrderByDescending, ThenBy and ThenByDescending methods can also be used with DbSets.
        }

        public List<GroupResponse> List()
        {
            // get the query of all Group entities then project each entity to GroupResponse object and return the list of GroupResponse objects
            return Query().Select(g => new GroupResponse // g: Group entity delegate
            {
                // assigning entity properties to the response
                Description = g.Description,
                Guid = g.Guid,
                Id = g.Id,
                Name = g.Name
            }).ToList();
        }

        // get a single Group entity by Id then project the entity to GroupResponse object and return the GroupResponse object
        public GroupResponse Item(int id)
        {
            var entity = Query().SingleOrDefault(g => g.Id == id); // g: Group entity delegate
            if (entity is null)
                return null;
            return new GroupResponse
            {
                // assigning entity properties to the response
                Description = entity.Description,
                Guid = entity.Guid,
                Id = entity.Id,
                Name = entity.Name
            };
        }

        // get a single Group entity by Id then project the entity to GroupRequest object and return the GroupRequest object
        public GroupRequest Edit(int id)
        {
            var entity = Query().SingleOrDefault(g => g.Id == id); // g: Group entity delegate
            if (entity is null)
                return null;
            return new GroupRequest
            {
                // assigning entity properties to the request
                Description = entity.Description,
                Id = entity.Id,
                Name = entity.Name
            };
        }

        // create a new Group entity from the GroupRequest object and save it to the database if a group with the same name does not exist
        public CommandResponse Create(GroupRequest request)
        {
            // g: Group entity delegate. Check if a group with the same name exists
            // (case-sensitive, request.Name without white space characters in the beginning and at the end).
            if (Query().Any(g => g.Name == request.Name.Trim()))
                return Error("Group with the same name already exists!");
            var entity = new Group
            {
                Description = request.Description?.Trim(), // remove white space characters in the beginning and at the end
                Name = request.Name?.Trim() // remove white space characters in the beginning and at the end
            };
            Create(entity); // will add the entity to the Groups DbSet and since save default parameter's value is true, will save changes to the database
            return Success("Group created successfully.", entity.Id);
        }

        // get the Group entity by Id then update the entity from the GroupRequest object and save changes to the database
        // if a group other than the current updated group with the same name does not exist
        public CommandResponse Update(GroupRequest request)
        {
            // g: Group entity delegate. Check if a group excluding the current updated group with the same name exists
            // (case-sensitive, request.Name without white space characters in the beginning and at the end).
            if (Query().Any(g => g.Id != request.Id && g.Name == request.Name.Trim()))
                return Error("Group with the same name already exists!");
            // get the Group entity by ID from the Groups table
            var entity = Query(false).SingleOrDefault(g => g.Id == request.Id); // isNoTracking is false for being tracked by EF Core to update the entity
            if (entity is null)
                return Error("Group not found!");
            // update retrieved Group entity's properties with request properties
            entity.Description = request.Description?.Trim();
            entity.Name = request.Name?.Trim();
            Update(entity); // will update the entity in the Groups DbSet and since save default parameter's value is true, will save changes to the database
            return Success("Group updated successfully.", entity.Id);
        }

        // get the Group entity by Id then delete the entity from the database
        public CommandResponse Delete(int id)
        {
            // get the Group entity by ID from the Groups table
            var entity = Query(false).SingleOrDefault(g => g.Id == id); // isNoTracking is false for being tracked by EF Core to delete the entity
            if (entity is null)
                return Error("Group not found!");

            // Way 2: recommended
            if (entity.Users.Count > 0) // if (entity.Users.Any())
                return Error("Group can't be deleted because it has relational users!");

            // delete the Group entity
            Delete(entity); // will delete the entity from the Groups DbSet and since save default parameter's value is true, will save changes to the database
            return Success("Group deleted successfully.", entity.Id);
        }
    }
}
