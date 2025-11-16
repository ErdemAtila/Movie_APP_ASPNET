using CORE.APP.Models;
using System;
using System.ComponentModel.DataAnnotations;

namespace APP.Models
{
    public class MovieRequest : Request
    {
        [Required, StringLength(200)]
        public string Name { get; set; }

        public DateTime? ReleaseDate { get; set; }

        public decimal TotalRevenue { get; set; }

        public int? DirectorId { get; set; }

        public List<int> GenreIds { get; set; } = new List<int>();
    }
}

