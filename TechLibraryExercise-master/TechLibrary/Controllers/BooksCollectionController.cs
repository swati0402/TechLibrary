using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Threading.Tasks;
using AutoMapper;
using TechLibrary.Models;
using TechLibrary.Services;
using TechLibrary.ResourceParameters;
using System;

namespace TechLibrary.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class BooksCollectionController : Controller
    {
        private readonly ILogger<BooksCollectionController> _logger;
        private readonly IBookService _bookService;
        private readonly IMapper _mapper;

        public BooksCollectionController(ILogger<BooksCollectionController> logger, IBookService bookService, IMapper mapper)
        {
            _logger = logger;
            _bookService = bookService;
            _mapper = mapper;
        }

        //[HttpGet({"ids"})]
        //public ActionResult BookCollection([FromRoute] IEnumerable<int> ids)
        //{

        //}
        //add new book collection
        [HttpPost]
        public async Task<IActionResult> Post([FromBody] IEnumerable<BookResponse> books)
        {
            _logger.LogInformation($"Add new book collection");
            try
            {
                foreach (var book in books)
                {
                    var newbook = await _bookService.AddBookAsync(book);

                    if (newbook.Item2 != null)
                    {
                        return BadRequest(newbook.Item2.ErrorMessage);
                    }
                    var bookResponse = _mapper.Map<BookResponse>(newbook.Item1);
                    book.BookId = newbook.Item1.BookId;
                }
                //return CreatedAtRoute("GetBook", new { id = bookResponse.BookId }, bookResponse);
                return Ok(books);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message.ToString());
            }
        }
    }
}
