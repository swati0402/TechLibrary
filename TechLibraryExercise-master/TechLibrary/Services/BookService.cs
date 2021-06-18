using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using TechLibrary.Contracts.Responses;
using TechLibrary.Data;
using TechLibrary.Domain;
using TechLibrary.Models;
using TechLibrary.ResourceParameters;

namespace TechLibrary.Services
{
    public interface IBookService
    {
        Task<List<Book>> GetBooksAsync(BooksResourceParams bookParams);
        Task<Book> GetBookByIdAsync(int bookid);
        Task<Tuple<Book, ErrorResponse>> UpdateBookByIdAsync(int bookid, BookResponse book);
        Task<Tuple<Book, ErrorResponse>> AddBookAsync(BookResponse book);
        Task<string> DeleteBookByIdAsync(int bookid);
        int TotalBooks();
    }

    public class BookService : IBookService
    {
        private readonly DataContext _dataContext;

        public BookService(DataContext dataContext)
        {
            _dataContext = dataContext;
        }

        public async Task<List<Book>> GetBooksAsync(BooksResourceParams bookParams)
        {
            var queryable = _dataContext.Books.AsQueryable();

            //if searchQuery is passed filter records for title and descr
            if (!string.IsNullOrWhiteSpace(bookParams.searchQuery))
            {
                var search = bookParams.searchQuery.Trim();
                //Return data based on pagenum/pagesize if not passed default pagenum=1 and pagesize=10
                queryable = queryable
                    .Where(a => a.Title.Contains(search) || a.ShortDescr.Contains(search));
            }

            //Return data based on pagenum/pagesize if not passed default pagenum=1 and pagesize=10
            queryable = queryable
                .OrderBy(i => i.BookId)
                .Skip((bookParams.PageNumber - 1) * bookParams.PageSize)
                .Take(bookParams.PageSize);

            return await queryable.ToListAsync();
        }

        public async Task<Book> GetBookByIdAsync(int bookid)
        {
            return await _dataContext.Books.SingleOrDefaultAsync(x => x.BookId == bookid);
        }

        //update book
        public async Task<Tuple<Book,ErrorResponse>> UpdateBookByIdAsync(int bookid, BookResponse book)
        {
            Book oldBook = await _dataContext.Books.FirstOrDefaultAsync(x => x.BookId == bookid);
            ErrorResponse err = new ErrorResponse();
            //no book found, nothing to update
            if (oldBook == null)
            {
                err.ErrorMessage = "No book exists with this bookid";
                return new Tuple<Book, ErrorResponse>(null, err);
            }
            //title is required field
            if (String.IsNullOrEmpty(book.Title))
            {
                err.ErrorMessage = "Title is required.";
                return new Tuple<Book, ErrorResponse>(null, err);
            }
            if (!string.IsNullOrWhiteSpace(book.Title))
            {
                oldBook.Title = book.Title;
            }
            if (!string.IsNullOrWhiteSpace(book.Descr)) 
            {
                oldBook.ShortDescr = book.Descr; 
            }
            if (!string.IsNullOrWhiteSpace(book.PublishedDate))
            {
                oldBook.PublishedDate = book.PublishedDate;
            }
            if (!string.IsNullOrWhiteSpace(book.ThumbnailUrl))
            {
                oldBook.ThumbnailUrl = book.ThumbnailUrl;
            }
            _dataContext.Books.Update(oldBook);
            await _dataContext.SaveChangesAsync();
            return new Tuple<Book, ErrorResponse>(oldBook,null);
        }

        //add new book
        public async Task<Tuple<Book, ErrorResponse>> AddBookAsync(BookResponse book)
        {
            Book newBook = new Book();
            ErrorResponse err = new ErrorResponse();
            //title is required field
            if (String.IsNullOrEmpty(book.Title))
            {
                err.ErrorMessage = "Title is required.";
                return new Tuple<Book, ErrorResponse>(null, err);
            }
            int book_id = await _dataContext.Books.MaxAsync(x => x.BookId);
            
            newBook.BookId = book_id + 1;
            newBook.Title = book.Title;
            newBook.ISBN = book.ISBN;
            newBook.PublishedDate = book.PublishedDate;
            newBook.ThumbnailUrl = book.ThumbnailUrl;
            newBook.ShortDescr = book.Descr;
            await _dataContext.Books.AddAsync(newBook);
            await _dataContext.SaveChangesAsync();
            return new Tuple<Book, ErrorResponse>(newBook, null);
        }

        //Total book count
        public int TotalBooks()
        {
            try 
            {
                return _dataContext.Books.Count();
            }
            catch (Exception)
            {
                return 0;
            }
            
        }
        //delete books
        public async Task<string> DeleteBookByIdAsync(int bookid)
        {
            Book oldBook = await _dataContext.Books.FirstOrDefaultAsync(x => x.BookId == bookid);
            //if no book found return 
            if (oldBook == null)
            {
                return "No book with this Id:" +bookid;
            }
             _dataContext.Books.Remove(oldBook);
            await _dataContext.SaveChangesAsync();
            return "Book Deleted successfully";
        }
    }
}
