using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using FW.Entities;
using FreewheelAssessment.Models;
using FreewheelAssessment.Helpers;

namespace FreewheelAssessment.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MoviesController : ControllerBase
    {
        private readonly MovieDbContext _context;

        public MoviesController(MovieDbContext context)
        {
            _context = context;
        }



        [HttpGet]
        [Route("Search")]
        //GET: api/Movies/Search
        public ActionResult<IEnumerable<object>> GetMovies([FromQuery] FilterModel searchParams)
        {
            var title = searchParams.title;
            var year = searchParams.year;
            var  genre = searchParams.genre;

            if (string.IsNullOrWhiteSpace(title) && year == null && string.IsNullOrWhiteSpace(genre))
                return BadRequest();
            var filteredMovies = _context.Movies.AsEnumerable();
            filteredMovies = string.IsNullOrWhiteSpace(title) ? filteredMovies : filteredMovies.Where(x => x.Title.IndexOf(title,StringComparison.CurrentCultureIgnoreCase)>=0).ToList().AsEnumerable();

            if (!string.IsNullOrWhiteSpace(genre)) {
                foreach (var g in genre.Split(','))
                {
                    filteredMovies = filteredMovies.Where(x => x.GenreCSV.Split(',',StringSplitOptions.RemoveEmptyEntries).Contains(g, StringComparer.CurrentCultureIgnoreCase));
                }
            }

            if(year != null)
            {
                filteredMovies = filteredMovies.Where(x => x.ReleaseDate.Year == year.Value);
            }

            return (from fm in filteredMovies
            select new { fm.Title, fm.RunningTime, fm.Director, Genre = fm.GenreCSV, ReleaseDate = fm.ReleaseDate.ToString("D"), AverageRating = fm.GetAverageRatingForMovie(_context) }).ToList();

        }




        // GET: api/Movies/TopRated
        [HttpGet]
        [Route("TopRated")]
        public async Task<ActionResult<object>> GetTopRatedMovies()
        {
            var moviesWithRatings =
               await _context.Ratings.GroupBy(x => new { x.MovieId })
                .Select(x => new { x.Key.MovieId, AvgRating = x.Average(x => x.Rating).RoundtoMid5() }).ToListAsync();
                ;
            return  (from mr in moviesWithRatings
                     join fm in _context.Movies on mr.MovieId equals fm.Id
                     select new { fm.Title, fm.RunningTime, fm.Director, Genre = fm.GenreCSV, ReleaseDate = fm.ReleaseDate.ToString("D"), AverageRating = mr.AvgRating.RoundtoMid5() }).OrderByDescending(x=>x.AverageRating).ThenBy(x=>x.Title).Take(5).ToList();

        }


        // GET: api/Movies/TopRated/5
        [HttpGet]
        [Route("TopRated/{userName}")]
        public ActionResult<object> GetTopRatedMovies(string userName)
        {
            var user = _context.Users.FirstOrDefault(x => x.UserName.ToLower() == userName.ToLower());
            if (user == null){
                return NotFound();
            };
            var moviesWithRatings =
                _context.Ratings.Where(x => x.UserId == user.Id).GroupBy(x => new { x.MovieId })
                .Select(x => new { x.Key.MovieId, AvgRating = x.Average(x => x.Rating).RoundtoMid5() }).ToList();
                ;
            return (from mr in moviesWithRatings
                          join fm in _context.Movies on mr.MovieId equals fm.Id
                          select new { fm.Title, fm.RunningTime, fm.Director, Genre = fm.GenreCSV, ReleaseDate = fm.ReleaseDate.ToString("D"), AverageRating = mr.AvgRating.RoundtoMid5() }).OrderByDescending(x => x.AverageRating).ThenBy(x => x.Title).Take(5).ToList();

        }







        // GET: api/Movies
        [HttpGet]
        public async Task<ActionResult<IEnumerable<object>>> GetMovies()
        {
            return  await (from fm in  _context.Movies
                    select new { fm.Title, fm.RunningTime, fm.Director, Genre = fm.GenreCSV, ReleaseDate = fm.ReleaseDate.ToString("D"), AverageRating = fm.GetAverageRatingForMovie(_context) })
                    .ToListAsync();

            ;
        }

        // GET: api/Movies/5
        [HttpGet("{id}")]
        public async Task<ActionResult<object>> GetMovie(int id)
        {
            var fm = await _context.Movies.FindAsync(id);

            if (fm == null)
            {
                return NotFound();
            }

            return new { fm.Title, fm.RunningTime, fm.Director, Genre = fm.GenreCSV, ReleaseDate = fm.ReleaseDate.ToString("D"), AverageRating = fm.GetAverageRatingForMovie(_context) };
        }



        // POST: api/Movies
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        [Route("UpdateRating")]
        public async Task<ActionResult> PostMovie(string movie,string user,int rating)
        {
            
            var movieEntity = _context.Movies.FirstOrDefault(x=>x.Title.ToLower()==movie.ToLower());
            var userEntity =  _context.Users.FirstOrDefault(x=>x.UserName.ToLower()==user.ToLower());
            if(userEntity ==null || movieEntity == null)
            {
                return NotFound();
            }
            if (rating < 1 && rating > 5)
                return BadRequest();

            var userRating = _context.Ratings.FirstOrDefault(x => x.UserId == userEntity.Id && x.MovieId == movieEntity.Id);

            if(userRating!=null)
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
            catch (DbUpdateConcurrencyException)
            {
                //Log
                return BadRequest();
            }
            return Ok();
        }
    }
}
