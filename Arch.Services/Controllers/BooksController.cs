using Arch.CoreLibrary.Entities;
using Arch.Data;
using Arch.Mongo.Managers;
using Arch.Mongo.Models;
using Arch.Mongo.Models.CommonModels;
using Arch.Mongo.Models.Logs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;

namespace Arch.Services.Controllers
{
    public class BooksController : BaseApiController
    {
        private readonly IGenericRepository<Books> _booksService;

        public BooksController(IGenericRepository<Books> bookService, CoreLibrary.Managers.IServiceManager serviceManager, IMemoryCache cache, CoreLibrary.Repositories.IGenericRepository<LogManagementContext> logService) : base(serviceManager, cache, logService)
        {
            _booksService = bookService;
        }

        [Authorize]
        [HttpGet]
        public async Task<IActionResult> Get()
        {
            var response = new ResponseBase<IQueryable<Books>>();
            _LogService.Run(
                CoreLibrary.Repositories.ProjectEnvironment.Service,
                "BookService",
                action: () =>
                {
                    var result = _booksService._GetAll().AsQueryable();

                    if (result.Any())
                        response = _Service.SuccessServiceResponse(result);
                    else response = _Service.WarningServiceResponse(result, "Kayıt bulunamadı!");
                },
                errorAction: (ex) => response = _Service.ErrorServiceResponse<IQueryable<Books>>(ex),
            //requestModel: request,
            responseModel: response);

            return await Api(response);
        }

        [HttpGet("{id:length(24)}")]
        public async Task<IActionResult> Get(string id)
        {
            var response = new ResponseBase<Books>();
            _LogService.Run(
                CoreLibrary.Repositories.ProjectEnvironment.Service,
                "BookService",
                action: () =>
                {
                    var result = _booksService.FindById(id);

                    if (result is not null)
                        response = _Service.SuccessServiceResponse(result);
                    else response = _Service.WarningServiceResponse(result, "Kayıt bulunamadı!");
                },
                errorAction: (ex) => response = _Service.ErrorServiceResponse<Books>(ex),
            //requestModel: request,
            responseModel: response);

            return await Api(response);
        }

        [HttpPost]
        public async Task<IActionResult> Post(Books newBook)
        {
            var response = new ResponseBase<UpsertModel>();
            var result = new UpsertModel();
            _LogService.Run(
                CoreLibrary.Repositories.ProjectEnvironment.Service,
                "BookService",
                action: () =>
                {
                    var res = _booksService.InsertOneAsync(newBook);

                    if (res.IsCompleted)
                    {
                        response = _Service.SuccessServiceResponse(result);
                    }

                    else response = _Service.WarningServiceResponse(result, "Kayıt bulunamadı!");
                },
                errorAction: (ex) => response = _Service.ErrorServiceResponse<UpsertModel>(ex),
            //requestModel: request,
            responseModel: response);

            return await Api(response);
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
