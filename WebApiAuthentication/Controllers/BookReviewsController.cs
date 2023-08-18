using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using WebApiAuthentication.DataAccess.Entities;
using WebApiAuthentication.DataAccess.Repositories;

namespace WebApiAuthentication.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class BookReviewsController : ControllerBase
    {
        private readonly IReviewRepository _reviewRepository;

        private static bool ValidateReview(BookReview bookReview)
        {
            return !string.IsNullOrWhiteSpace(bookReview.Title)
                && bookReview.Rating >= 1
                && bookReview.Rating <= 5;
        }

        public BookReviewsController(IReviewRepository reviewRepository)
        {
            _reviewRepository = reviewRepository;
        }

        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(IEnumerable<BookReview>))]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public ActionResult<IEnumerable<BookReview>> Get()
        {
            return Ok(_reviewRepository.AllReviews);
        }

        [HttpGet("{id:int}")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(BookReview))]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public ActionResult<BookReview> Get(int id)
        {
            var result = _reviewRepository.AllReviews.SingleOrDefault(r => r.Id == id);

            if (result == null)
                return NotFound();
            else
                return Ok(result);
        }

        [HttpGet("summary")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(IEnumerable<BookReview>))]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public ActionResult<IEnumerable<BookReview>> Summary()
        {
            var summaries = _reviewRepository.AllReviews.GroupBy(r => r.Title).Select(g =>
                new BookReview
                {
                    Title = g.Key,
                    Rating = Math.Round(g.Average(r => r.Rating), 2)
                }).ToList();

            int id = 1;

            summaries.ForEach(r => r.Id = id++);
                
            return Ok(summaries);
        }

        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created, Type = typeof(int))]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status422UnprocessableEntity)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public ActionResult<int> Post([FromBody] BookReview review)
        {
            if (!ValidateReview(review))
                return UnprocessableEntity();

            _reviewRepository.Create(review);
            _reviewRepository.SaveChanges();

            return CreatedAtAction(nameof(Get), new { id = review.Id }, review.Id);
        }

        [HttpPut("{id:int}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status422UnprocessableEntity)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public IActionResult Put(int id, [FromBody] BookReview review)
        {
            if (!ValidateReview(review))
                return UnprocessableEntity();

            var result = _reviewRepository.AllReviews.SingleOrDefault(r => r.Id == id);

            if (result == null)
                return NotFound();
            else
            {
                result.Rating = review.Rating;
                result.Title = review.Title;

                _reviewRepository.SaveChanges();

                return Ok();
            }
        }

        [HttpDelete("{id:int}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public IActionResult Delete(int id)
        {
            var result = _reviewRepository.AllReviews.SingleOrDefault(r => r.Id == id);

            if (result == null)
                return NotFound();
            else
            {
                _reviewRepository.Remove(result);
                _reviewRepository.SaveChanges();

                return Ok();
            }
        }
    }
}
