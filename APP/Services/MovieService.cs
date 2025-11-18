using APP.Domain;
using APP.Models;
using CORE.APP.Models;
using CORE.APP.Services;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;

namespace APP.Services
{
    public class MovieService : Service<Movie>, IService<MovieRequest, MovieResponse>
    {
        public MovieService(DbContext db) : base(db)
        {
        }

        public List<MovieResponse> List()
        {
            return Query()
                .Include(m => m.Director)
                .Include(m => m.MovieGenres)
                    .ThenInclude(mg => mg.Genre)
                .Select(m => new MovieResponse
                {
                    // assigning entity properties to the response
                    Guid = m.Guid,
                    Id = m.Id,
                    Name = m.Name,
                    ReleaseDate = m.ReleaseDate,
                    TotalRevenue = m.TotalRevenue,

                    // assigning custom or formatted properties to the response
                    // If Movie entity's ReleaseDate value is not null, convert and assign the value with month/day/year format, otherwise assign "".
                    ReleaseDateF = m.ReleaseDate.HasValue ? m.ReleaseDate.Value.ToString("MM/dd/yyyy") : string.Empty,
                    // C: currency format, 2: 2 decimal places
                    TotalRevenueF = m.TotalRevenue.ToString("C2"),

                    DirectorName = m.Director != null ? $"{m.Director.FirstName} {m.Director.LastName}" : "",
                    GenreNames = string.Join(", ", m.MovieGenres.Select(mg => mg.Genre.Name)),
                    GenreIds = m.MovieGenres.Select(mg => mg.GenreId).ToList()
                }).ToList();
        }

        public MovieResponse Item(int id)
        {
            var entity = Query()
                .Include(m => m.Director)
                .Include(m => m.MovieGenres)
                    .ThenInclude(mg => mg.Genre)
                .SingleOrDefault(m => m.Id == id);

            if (entity is null)
                return null;

            return new MovieResponse
            {
                // assigning entity properties to the response
                Guid = entity.Guid,
                Id = entity.Id,
                Name = entity.Name,
                ReleaseDate = entity.ReleaseDate,
                TotalRevenue = entity.TotalRevenue,

                // assigning custom or formatted properties to the response
                // If Movie entity's ReleaseDate value is not null, convert and assign the value with month/day/year format, otherwise assign "".
                ReleaseDateF = entity.ReleaseDate.HasValue ? entity.ReleaseDate.Value.ToShortDateString() : string.Empty,
                // C: currency format, 2: 2 decimal places
                TotalRevenueF = entity.TotalRevenue.ToString("C2"),

                DirectorName = entity.Director != null ? $"{entity.Director.FirstName} {entity.Director.LastName}" : "",
                GenreNames = string.Join(", ", entity.MovieGenres.Select(mg => mg.Genre.Name)),
                GenreIds = entity.MovieGenres.Select(mg => mg.GenreId).ToList()
            };
        }

        public MovieRequest Edit(int id)
        {
            var entity = Query()
                .Include(m => m.MovieGenres)
                .SingleOrDefault(m => m.Id == id);
            if (entity is null)
                return null;

            return new MovieRequest
            {
                Id = entity.Id,
                Name = entity.Name,
                ReleaseDate = entity.ReleaseDate,
                TotalRevenue = entity.TotalRevenue,
                DirectorId = entity.DirectorId,
                GenreIds = entity.MovieGenres.Select(mg => mg.GenreId).ToList()
            };
        }

        public CommandResponse Create(MovieRequest request)
        {
            if (Query().Any(m => m.Name == request.Name.Trim()))
                return Error("Movie with the same name already exists!");

            var entity = new Movie
            {
                Name = request.Name.Trim(),
                ReleaseDate = request.ReleaseDate,
                TotalRevenue = request.TotalRevenue,
                DirectorId = request.DirectorId
            };

            // Save the Movie first to get its Id
            Create(entity, true);

            // Handle many-to-many relationship with genres
            if (request.GenreIds != null && request.GenreIds.Count > 0)
            {
                foreach (var genreId in request.GenreIds)
                {
                    if (genreId > 0) // Ensure valid genre ID
                    {
                        var movieGenre = new MovieGenre
                        {
                            MovieId = entity.Id,
                            GenreId = genreId
                        };
                        _db.Set<MovieGenre>().Add(movieGenre);
                    }
                }
                Save(); // Save the MovieGenre entries
            }

            return Success("Movie created successfully.", entity.Id);
        }

        public CommandResponse Update(MovieRequest request)
        {
            if (Query().Any(m => m.Id != request.Id && m.Name == request.Name.Trim()))
                return Error("Movie with the same name already exists!");

            var entity = Query(false)
                .Include(m => m.MovieGenres)
                .SingleOrDefault(m => m.Id == request.Id);
            if (entity is null)
                return Error("Movie not found!");

            entity.Name = request.Name.Trim();
            entity.ReleaseDate = request.ReleaseDate;
            entity.TotalRevenue = request.TotalRevenue;
            entity.DirectorId = request.DirectorId;

            // Update many-to-many relationship with genres
            var existingGenreIds = entity.MovieGenres.Select(mg => mg.GenreId).ToList();
            var newGenreIds = request.GenreIds ?? new List<int>();

            // Remove genres that are no longer selected
            var genresToRemove = entity.MovieGenres
                .Where(mg => !newGenreIds.Contains(mg.GenreId))
                .ToList();
            foreach (var movieGenre in genresToRemove)
            {
                _db.Set<MovieGenre>().Remove(movieGenre);
            }

            // Add new genres
            var genresToAdd = newGenreIds
                .Where(gid => !existingGenreIds.Contains(gid))
                .ToList();
            foreach (var genreId in genresToAdd)
            {
                var movieGenre = new MovieGenre
                {
                    MovieId = entity.Id,
                    GenreId = genreId
                };
                _db.Set<MovieGenre>().Add(movieGenre);
            }

            Update(entity, false);
            Save();
            return Success("Movie updated successfully.", entity.Id);
        }

        public CommandResponse Delete(int id)
        {
            var entity = Query(false)
                .Include(m => m.MovieGenres)
                .SingleOrDefault(m => m.Id == id);
            if (entity is null)
                return Error("Movie not found!");

            // Delete related MovieGenres
            var movieGenres = entity.MovieGenres.ToList();
            foreach (var movieGenre in movieGenres)
            {
                _db.Set<MovieGenre>().Remove(movieGenre);
            }

            Delete(entity);
            return Success("Movie deleted successfully.", entity.Id);
        }
    }
}

