using APP.Domain;
using APP.Models;
using CORE.APP.Models;
using CORE.APP.Services;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;

namespace APP.Services
{
    public class UserService : Service<User>, IService<UserRequest, UserResponse>
    {
        public UserService(DbContext db) : base(db)
        {
        }

        public List<UserResponse> List()
        {
            return Query()
                .Include(u => u.Group)
                .Select(u => new UserResponse
                {
                    // assigning entity properties to the response
                    Guid = u.Guid,
                    Id = u.Id,
                    UserName = u.UserName,
                    FirstName = u.FirstName,
                    LastName = u.LastName,
                    Gender = u.Gender,
                    BirthDate = u.BirthDate,
                    RegistrationDate = u.RegistrationDate,
                    Score = u.Score,
                    IsActive = u.IsActive,
                    Address = u.Address,

                    // assigning custom or formatted properties to the response
                    // If User entity's BirthDate value is not null, convert and assign the value with month/day/year format, otherwise assign "".
                    BirthDateF = u.BirthDate.HasValue ? u.BirthDate.Value.ToString("MM/dd/yyyy") : string.Empty,
                    // If User entity's RegistrationDate value is not null, convert and assign the value with month/day/year format, otherwise assign "".
                    RegistrationDateF = u.RegistrationDate.ToString("MM/dd/yyyy"),

                    GroupName = u.Group != null ? u.Group.Name : ""
                }).ToList();
        }

        public UserResponse Item(int id)
        {
            var entity = Query()
                .Include(u => u.Group)
                .SingleOrDefault(u => u.Id == id);

            if (entity is null)
                return null;

            return new UserResponse
            {
                // assigning entity properties to the response
                Guid = entity.Guid,
                Id = entity.Id,
                UserName = entity.UserName,
                FirstName = entity.FirstName,
                LastName = entity.LastName,
                Gender = entity.Gender,
                BirthDate = entity.BirthDate,
                RegistrationDate = entity.RegistrationDate,
                Score = entity.Score,
                IsActive = entity.IsActive,
                Address = entity.Address,

                // assigning custom or formatted properties to the response
                // If User entity's BirthDate value is not null, convert and assign the value with month/day/year format, otherwise assign "".
                BirthDateF = entity.BirthDate.HasValue ? entity.BirthDate.Value.ToShortDateString() : string.Empty,
                // If User entity's RegistrationDate value is not null, convert and assign the value with month/day/year format, otherwise assign "".
                RegistrationDateF = entity.RegistrationDate.ToShortDateString(),

                GroupName = entity.Group != null ? entity.Group.Name : ""
            };
        }

        public UserRequest Edit(int id)
        {
            var entity = Query().SingleOrDefault(u => u.Id == id);
            if (entity is null)
                return null;

            return new UserRequest
            {
                Id = entity.Id,
                UserName = entity.UserName,
                FirstName = entity.FirstName,
                LastName = entity.LastName,
                Gender = entity.Gender,
                BirthDate = entity.BirthDate,
                Score = entity.Score,
                IsActive = entity.IsActive,
                Address = entity.Address,
                CountryId = entity.CountryId,
                CityId = entity.CityId,
                GroupId = entity.GroupId
            };
        }

        public CommandResponse Create(UserRequest request)
        {
            if (Query().Any(u => u.UserName == request.UserName.Trim()))
                return Error("User with the same username already exists!");

            var entity = new User
            {
                UserName = request.UserName.Trim(),
                Password = request.Password, // In a real app, hash this!
                FirstName = request.FirstName?.Trim(),
                LastName = request.LastName?.Trim(),
                Gender = request.Gender,
                BirthDate = request.BirthDate,
                Score = request.Score,
                IsActive = request.IsActive,
                Address = request.Address?.Trim(),
                CountryId = request.CountryId,
                CityId = request.CityId,
                GroupId = request.GroupId,
                RegistrationDate = System.DateTime.UtcNow
            };

            Create(entity);
            return Success("User created successfully.", entity.Id);
        }

        public CommandResponse Update(UserRequest request)
        {
            if (Query().Any(u => u.Id != request.Id && u.UserName == request.UserName.Trim()))
                return Error("User with the same username already exists!");

            var entity = Query().SingleOrDefault(u => u.Id == request.Id);
            if (entity is null)
                return Error("User not found!");

            entity.UserName = request.UserName.Trim();
            if (!string.IsNullOrWhiteSpace(request.Password))
            {
                entity.Password = request.Password; // In a real app, hash this!
            }
            entity.FirstName = request.FirstName?.Trim();
            entity.LastName = request.LastName?.Trim();
            entity.Gender = request.Gender;
            entity.BirthDate = request.BirthDate;
            entity.Score = request.Score;
            entity.IsActive = request.IsActive;
            entity.Address = request.Address?.Trim();
            entity.CountryId = request.CountryId;
            entity.CityId = request.CityId;
            entity.GroupId = request.GroupId;

            Update(entity);
            return Success("User updated successfully.", entity.Id);
        }

        public CommandResponse Delete(int id)
        {
            // Also need to handle deleting related UserRoles
            var userRoles = _db.Set<UserRole>().Where(ur => ur.UserId == id);
            _db.Set<UserRole>().RemoveRange(userRoles);

            var entity = Query(false).SingleOrDefault(u => u.Id == id);
            if (entity is null)
                return Error("User not found!");

            Delete(entity);
            return Success("User deleted successfully.", entity.Id);
        }
    }
}
