using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FW.Entities
{
    public class MovieDbContext : DbContext
    {
        public MovieDbContext(DbContextOptions options) :
          base(options)
        {
            Database.EnsureCreated();
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            var movies = new List<Movie>() {
                                        new Movie { Id = 1, Title = "The Shawshank Redemption",GenreCSV= "Crime,Drama", ReleaseDate = new DateTime(1972, 3, 24) },
                                        new Movie { Id = 2, Title = "The Godfather",GenreCSV= "Crime,Drama", ReleaseDate = new DateTime(1972, 3, 24)  },
                                        new Movie { Id = 3, Title = "The Godfather: Part II",GenreCSV= "Crime,Drama", ReleaseDate = new DateTime(1974, 12, 18) },
                                        new Movie { Id = 4, Title = "The Dark Knight",GenreCSV= "Comic,SciFi,Superhero,DC,Action", ReleaseDate = new DateTime(2008, 7, 18) },
                                        new Movie { Id = 5, Title = "12 Angry Men",GenreCSV= "Crime,Drama", ReleaseDate = new DateTime(1957, 4, 10) },
                                        new Movie { Id = 6, Title = "Avengers: Endgame",GenreCSV= "Comic,SciFi,Superhero,Marvel", ReleaseDate = new DateTime(2019, 4, 26) },
                                    };

            var users = new List<User>()
                                    {new User { Id = 1, UserName = "InvDRT" }, new User { UserName = "Karthik", Id = 2 }, new User { Id = 3, UserName = "Chetan" }, new User { Id = 4, UserName = "Naveen" } }; 
            
            modelBuilder.Entity<Movie>().HasData(movies);
            modelBuilder.Entity<User>().HasData(users);

            var random = new Random();
            int pk = 1;
            var ratings = (from m in movies
                           from u in users
                           select 
                            new UserRating { Id = pk++, MovieId = m.Id, UserId = u.Id, Rating = random.Next(1,6) });

            modelBuilder.Entity<UserRating>().HasData(ratings);

            base.OnModelCreating(modelBuilder);
        }

        public DbSet<Movie> Movies { get; set; }

        public DbSet<User> Users { get; set; }

        public DbSet<UserRating> Ratings { get; set; }


    }
}
