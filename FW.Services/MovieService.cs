using FreewheelAssessment.Helpers;
using FreewheelAssessment.Models;
using FW.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FW.Services
{
    public class MovieService : IMovieService
    {
        private readonly MovieDbContext _context;
        public MovieService(MovieDbContext movieDbContext)
        {
            _context = movieDbContext;
        }
        public IEnumerable<object> GetMovies(FilterModel searchParams)
        {

            var title = searchParams.title;
            var year = searchParams.year;
            var genre = searchParams.genre;

            var filteredMovies = _context.Movies.AsEnumerable();
            filteredMovies = string.IsNullOrWhiteSpace(title) ? filteredMovies
                : filteredMovies.Where(x => x.Title.IndexOf(title, StringComparison.CurrentCultureIgnoreCase) >= 0).ToList().AsEnumerable();

            if (!string.IsNullOrWhiteSpace(genre))
            {
                foreach (var g in genre.Split(','))
                {
                    filteredMovies = filteredMovies.Where(x => x.GenreCSV.Split(',', StringSplitOptions.RemoveEmptyEntries).Contains(g, StringComparer.CurrentCultureIgnoreCase));
                }
            }

            if (year != null)
            {
                filteredMovies = filteredMovies.Where(x => x.ReleaseDate.Year == year.Value);
            }

            return (from fm in filteredMovies
                    select new { fm.Title, fm.RunningTime, fm.Director, Genre = fm.GenreCSV, ReleaseDate = fm.ReleaseDate.ToString("D"), AverageRating = fm.GetAverageRatingForMovie(_context) })
                    .ToList();

        }

        public async Task<object> GetTopRatedMovies()
        {
            var moviesWithRatings =   
                await _context.Ratings
                .GroupBy(x => new { x.MovieId })
                .Select(x => new { x.Key.MovieId, AvgRating = x.Average(x => x.Rating).RoundtoMid5() })
                .ToListAsync();    
            ;
            return (from mr in moviesWithRatings
                    join fm in _context.Movies on mr.MovieId equals fm.Id
                    select new 
                    { fm.Title, fm.RunningTime, fm.Director, Genre = fm.GenreCSV, ReleaseDate = fm.ReleaseDate.ToString("D"), AverageRating = mr.AvgRating.RoundtoMid5()})
                    .OrderByDescending(x => x.AverageRating)
                    .ThenBy(x => x.Title)
                    .Take(5)
                    .ToList();

        }


        public object GetTopRatedMovies(User user)
        {
            var moviesWithRatings =
                 _context.Ratings.Where(x => x.UserId == user.Id).GroupBy(x => new { x.MovieId })
                 .Select(x => new { x.Key.MovieId, AvgRating = x.Average(x => x.Rating).RoundtoMid5() }).ToList();
            ;
            return (from mr in moviesWithRatings
                    join fm in _context.Movies on mr.MovieId equals fm.Id
                    select new { fm.Title, fm.RunningTime, fm.Director, Genre = fm.GenreCSV, ReleaseDate = fm.ReleaseDate.ToString("D"), AverageRating = mr.AvgRating.RoundtoMid5() }).OrderByDescending(x => x.AverageRating).ThenBy(x => x.Title).Take(5).ToList();


        }


        public async Task UpdateRating(Movie movieEntity, User userEntity, int rating)
        {
            var userRating = _context.Ratings.FirstOrDefault(x => x.UserId == userEntity.Id && x.MovieId == movieEntity.Id);

            if (userRating != null)
                _context.Entry(userRating).State = EntityState.Modified;

            else
            {
                _context.Ratings.Add(new UserRating { MovieId = movieEntity.Id, UserId = userEntity.Id });
            }
            userRating.Rating = rating;



            try
            {
                await _context.SaveChangesAsync();
            }
            catch
            {
                //log

                throw;
            }
        }


        public async Task<object> GetMovies()
        {
            return await(from fm in _context.Movies
                         select new 
                         { fm.Title, fm.RunningTime, fm.Director, Genre = fm.GenreCSV, ReleaseDate = fm.ReleaseDate.ToString("D"), AverageRating = fm.GetAverageRatingForMovie(_context) })
                        .ToListAsync();
        }


        public User GetUser(string userName)
        {
            return _context.Users.FirstOrDefault(x => x.UserName.ToLower() == userName.ToLower());
        }


        public Movie GetMovie(string movie)
        {
            return _context.Movies.FirstOrDefault(x => x.Title.ToLower() == movie.ToLower());
        }

        public object FormatMovie(Movie movie)
        {

            return new { movie.Title, movie.RunningTime, movie.Director, Genre = movie.GenreCSV, ReleaseDate = movie.ReleaseDate.ToString("D"), AverageRating = movie.GetAverageRatingForMovie(_context) };

        }
    }
}
