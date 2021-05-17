using System;
using System.Collections.Generic;

namespace FW.Entities
{
    public class Movie
    {
        public int Id { get; set; }

        public string Title { get; set; }

        public string Director { get; set; }

        public DateTime ReleaseDate { get; set; }

        public string GenreCSV { get; set; }

    }
}
