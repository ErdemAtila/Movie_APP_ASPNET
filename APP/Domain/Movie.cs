using CORE.APP.Domain;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace APP.Domain
{
    public class Movie : Entity
    {
        [Required, StringLength(200)]
        public string Name { get; set; }

        public DateTime? ReleaseDate { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal TotalRevenue { get; set; }

        public int? DirectorId { get; set; }
        public Director? Director { get; set; }

        public ICollection<MovieGenre> MovieGenres { get; set; }
    }
}

