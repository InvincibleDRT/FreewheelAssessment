using FreewheelAssessment.Models;
using FW.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FW.Services
{
    public interface IMovieService
    {
        IEnumerable<object> GetMovies(FilterModel searchParams);

        Task<object> GetTopRatedMovies();
        object GetTopRatedMovies(User userName);

        User GetUser(string userName);
        Movie GetMovie(string movie);

        Task UpdateRating(Movie movie, User user, int rating);

        Task<object> GetMovies();

        object FormatMovie(Movie movie);
    }
}
