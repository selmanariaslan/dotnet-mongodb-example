using Arch.Mongo.Managers;
using Arch.Mongo.Models;
using Microsoft.AspNetCore.Mvc;

namespace ARCH.Services.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class BooksController : ControllerBase
    {
        private readonly IServiceManager<Books> _booksService;

        public BooksController(IServiceManager<Books> bookService)
        {
            _booksService = bookService;
        }

        //private readonly BooksService _booksService;

        //public BooksController(BooksService booksService) =>
        //    _booksService = booksService;

        //[HttpGet]
        //public async Task<IQueryable<Book>> Get() =>
        //    await _booksService.AsQueryable();
        [HttpGet]
        public async Task<List<Books>> Get()
        {
            var books=_booksService.AsQueryable();
            return books.ToList();
        }

        [HttpGet("{id:length(24)}")]
        public async Task<IActionResult> Get(string id)
        {
            var book = _booksService.FindById(id);

            if (book is null)
            {
                return NotFound();  
            }

            return Ok(book);
        }

        [HttpPost]
        public async Task<IActionResult> Post(Books newBook)
        {
            await _booksService.InsertOneAsync(newBook);

            return CreatedAtAction(nameof(Get), new { id = newBook.Id }, newBook);
        }

        [HttpPut("{id:length(24)}")]
        public async Task<IActionResult> Update(/*string id, */Books updatedBook)
        {
            //var book = await _booksService.GetAsync(id);

            //if (book is null)
            //{
            //    return NotFound();
            //}

            //updatedBook.Id = book.Id;

            await _booksService.ReplaceOneAsync(updatedBook);//UpdateAsync(id, updatedBook);

            return NoContent();
        }

        [HttpDelete("{id:length(24)}")]
        public async Task<IActionResult> Delete(string id)
        {
            //var book = await _booksService.GetAsync(id);

            //if (book is null)
            //{
            //    return NotFound();
            //}

            await _booksService.DeleteByIdAsync(id);

            return NoContent();
        }
    }
}
