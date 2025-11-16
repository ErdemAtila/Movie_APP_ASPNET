using CORE.APP.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace APP.Models
{
    public class MovieResponse : Response
    {
        public string Name { get; set; }

        [DisplayName("Release Date")]
        public DateTime? ReleaseDate { get; set; }

        [DisplayName("Total Revenue")]
        public decimal TotalRevenue { get; set; }

        [DisplayName("Director")]
        public string DirectorName { get; set; }

        [DisplayName("Genres")]
        public string GenreNames { get; set; }

        public List<int> GenreIds { get; set; } = new List<int>();
    }
}

