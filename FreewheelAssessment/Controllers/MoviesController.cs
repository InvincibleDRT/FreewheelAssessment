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
using FW.Services;

namespace FreewheelAssessment.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MoviesController : ControllerBase
    {
        private readonly IMovieService _service;

        public MoviesController(IMovieService movieService)
        {
            _service = movieService;
        }



        [HttpGet]
        [Route("Search")]
        //GET: api/Movies/Search
        public ActionResult GetMovies([FromQuery] FilterModel searchParams)
        {

            if (!searchParams.IsValid)
                return BadRequest();

            return Ok(_service.GetMovies(searchParams));
        }




        // GET: api/Movies/TopRated
        [HttpGet]
        [Route("TopRated")]
        public async Task<ActionResult> GetTopRatedMovies()
        {
            return Ok(await _service.GetTopRatedMovies());
        }


        // GET: api/Movies/TopRated/5
        [HttpGet]
        [Route("TopRated/{userName}")]
        public ActionResult GetTopRatedMovies(string userName)
        {
            var user = _service.GetUser(userName);
            if (user == null){
                return NotFound();
            };
            return Ok(_service.GetTopRatedMovies(user));
        }







        // GET: api/Movies
        [HttpGet]
        public async Task<ActionResult> GetMovies()
        {
            return Ok(await _service.GetMovies());
        }

        // GET: api/Movies/5
        [HttpGet("{name}")]
        public ActionResult GetMovie(string name)
        {
            var fm = _service.GetMovie(name);

            if (fm == null)
            {
                return NotFound();
            }
            return Ok(_service.FormatMovie(fm));
        }


        [HttpPost]
        [Route("UpdateRating")]
        public async Task<ActionResult> UpdateRating(string movie,string user,int rating)
        {

            var movieEntity = _service.GetMovie(movie);
            var userEntity = _service.GetUser(user);
            if(userEntity ==null || movieEntity == null)
            {
                return NotFound();
            }
            if (rating < 1 && rating > 5)
                return BadRequest();

            try
            {
                await _service.UpdateRating(movieEntity, userEntity, rating);
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
