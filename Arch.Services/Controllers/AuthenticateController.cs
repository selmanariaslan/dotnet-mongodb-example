using Arch.CoreLibrary.Entities;
using Arch.CoreLibrary.Utils.Security;
using Arch.Mongo.Managers;
using Arch.Mongo.Models;
using Arch.Mongo.Models.Logs;
using Arch.Services.Models.Requests;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace Arch.Services.Controllers
{
    public class AuthenticateController : BaseApiController
    {
        private readonly IGenericRepository<Books> _booksService;
        public AuthenticateController(IGenericRepository<Books> bookService, IServiceManager serviceManager, IMemoryCache cache, IGenericRepository<PerformanceLog> logService) : base(serviceManager, cache, logService)
        {
            _booksService = bookService;
        }

        [HttpPost]
        [Route("login")]
        public async Task<IActionResult> Login([FromBody] LoginModel request)
        {
            var response = new ResponseBase<User>();
            _booksService.Run(CoreLibrary.Repositories.ProjectEnvironment.Service,
                "", action: () =>
                {
                    var user = GetUser(request.Username);
                    if (user != null && CheckPassword(user.Id, request.Password))
                    {
                        var userRole = GetRole(user.Id);
                        user.Role = userRole;

                        var token = TokenUtils.GenerateJwtTokenNew(user.Id.ToString(), userRole: userRole);
                        user.Token = token;
                        response = _Service.SuccessServiceResponse(user);
                        response.Status = ServiceResponseStatuses.Success;
                    }
                    else
                    {
                        _Service.WarningServiceResponse<User>("Kullanıcı adı veya parola yanlış!");
                        response.Status = ServiceResponseStatuses.Warning;
                    }
                }, errorAction: (ex) => response = _Service.ErrorServiceResponse<User>(ex),
            requestModel: request,
            responseModel: response);
            return Api(response);
        }

        private User GetUser(string username)
        {
            List<User> users = new List<User>();
            users.Add(new User { Id = 1, Username = "selman", Password = "123" });
            users.Add(new User { Id = 2, Username = "ali", Password = "345" });
            users.Add(new User { Id = 3, Username = "veli", Password = "567" });
            var user = users.Find(x => x.Username == username);
            if (!(user is null))
                return new User { Id = user.Id, Username = user.Username };
            else return new User();
        }

        private bool CheckPassword(int userId, string password)
        {
            List<User> users = new List<User>();
            users.Add(new User { Id = 1, Username = "selman", Password = "123", Role = "SuperAdmin" });
            users.Add(new User { Id = 2, Username = "ali", Password = "345", Role = "Admin" });
            users.Add(new User { Id = 3, Username = "veli", Password = "567", Role = "User" });
            var user = users.Find(x => x.Id == userId && x.Password == password);

            if (!(user is null))
                return true;
            else return false;
        }

        private string GetRole(int userId)
        {
            List<User> users = new List<User>();
            users.Add(new User { Id = 1, Username = "selman", Password = "123", Role = "SuperAdmin" });
            users.Add(new User { Id = 2, Username = "ali", Password = "345", Role = "Admin" });
            users.Add(new User { Id = 3, Username = "veli", Password = "567", Role = "User" });
            var user = users.Find(x => x.Id == userId);
            if (!(user is null))
                return user.Role;
            else return "";
        }
    }

    public class User
    {
        public int Id { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public string Role { get; set; }
        public string Token { get; set; }
    }
}
