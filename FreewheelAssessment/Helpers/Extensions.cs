using FW.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FreewheelAssessment.Helpers
{
    public static class Extensions
    {
        public static double GetAverageRatingForMovie(this Movie movie,MovieDbContext _context)
        {
            var avg =  _context.Ratings.Where(x => x.MovieId == movie.Id).Average(x=>x.Rating);
            return Math.Round(avg * 2, MidpointRounding.AwayFromZero) / 2;
        }

        public static double RoundtoMid5(this double avg)
        {
            return Math.Round(avg * 2, MidpointRounding.AwayFromZero) / 2;
        }
    }
}
