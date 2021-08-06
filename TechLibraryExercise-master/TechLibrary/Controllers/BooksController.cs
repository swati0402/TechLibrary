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
    public class BooksController : Controller
    {
        private readonly ILogger<BooksController> _logger;
        private readonly IBookService _bookService;
        private readonly IMapper _mapper;

        public BooksController(ILogger<BooksController> logger, IBookService bookService, IMapper mapper)
        {
            _logger = logger;
            _bookService = bookService;
            _mapper = mapper;
        }

        //get books based on page size and page num. if not passed send page 1 and pagesize 10
        [HttpGet]
        [HttpHead]
        public async Task<IActionResult> GetAll([FromQuery] BooksResourceParams bookParams)
        {
            _logger.LogInformation("Get all books based on pagenum/pagesize");
            try
            {
                var books = await _bookService.GetBooksAsync(bookParams);

                var bookResponse = _mapper.Map<List<BookResponse>>(books);

                return Ok(bookResponse);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message.ToString());
            }
        }

        //get book by book id
        [HttpGet("{id}",  Name="GetBook")]
        public async Task<IActionResult> GetById(int id)
        {
            _logger.LogInformation($"Get book by id {id}");
            try
            {
                var book = await _bookService.GetBookByIdAsync(id);

                var bookResponse = _mapper.Map<BookResponse>(book);

                return Ok(bookResponse);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message.ToString());
            }
        }

        //update book based on book id
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateById(int id, [FromBody] BookResponse book)
        {
            _logger.LogInformation($"Update book by id {id}");
            var updatedbook = await _bookService.UpdateBookByIdAsync(id, book);
            try
            {
                if (updatedbook.Item2 != null)
                {
                    return BadRequest(updatedbook.Item2.ErrorMessage);
                }
                var bookResponse = _mapper.Map<BookResponse>(updatedbook.Item1);
                return Ok(bookResponse);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message.ToString());
            }
        }

        //add new book
        [HttpPost]
        public async Task<IActionResult> Post([FromBody] BookResponse book)
        {
            _logger.LogInformation($"Add new book");
            var newbook = await _bookService.AddBookAsync(book);
            try
            {
                if (newbook.Item2 != null)
                {
                    return BadRequest(newbook.Item2.ErrorMessage);
                }
                var bookResponse = _mapper.Map<BookResponse>(newbook.Item1);
                return CreatedAtRoute("GetBook", new { id = bookResponse.BookId }, bookResponse);
                //return Ok(bookResponse);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message.ToString());
            }
        }

        //delete book based on id
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteById(int id)
        {
            _logger.LogInformation($"Delete book by id {id}");
            try 
            { 
                var book = await _bookService.DeleteBookByIdAsync(id);
                return Ok(book);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message.ToString());
            }
        }

        //get total books from DB
        [HttpGet]
        [Route("total")]
        public int GetCount()
        {
            _logger.LogInformation("Get total book count");
            return _bookService.TotalBooks();
        }
    }
}
