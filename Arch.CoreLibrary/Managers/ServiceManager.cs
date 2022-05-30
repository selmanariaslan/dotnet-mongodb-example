using Arch.CoreLibrary;
using Arch.CoreLibrary.Entities;
using Arch.CoreLibrary.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Json;
using System.Text;
using System.Threading.Tasks;

namespace Arch.CoreLibrary.Managers
{
    public interface IServiceManager
    {
        HttpClient Client { get; set; }

        HttpResponseMessage DeleteResponse(string url);
        ResponseBase<T> ErrorServiceResponse<T>(string userMessage = "İşleminiz gerçekleştirilmeye çalışılırken bir hata alındı.");
        ResponseBase<T> ErrorServiceResponse<T>(Exception ex, string userMessage = "İşleminiz gerçekleştirilmeye çalışılırken bir hata alındı.");
        HttpResponseMessage GetResponse(string url);
        HttpResponseMessage PostResponse(string url, object model);
        HttpResponseMessage PutResponse(string url, object model);
        ResponseBase<T> ServiceResponse<T>(T data, ServiceResponseStatuses status, Dictionary<string, string> messages, string userMessage);
        ResponseBase<T> SuccessServiceResponse<T>(T data, string userMessage = "İşleminiz başarıyla gerçekleşti.");
        ResponseBase<T> WarningServiceResponse<T>(string userMessage = "İşleminiz gerçşekleştirilemedi, Lütfen birazdan tekrar deneyin.");
        ResponseBase<T> WarningServiceResponse<T>(T data, string userMessage = "İşleminiz gerçşekleştirilemedi, Lütfen birazdan tekrar deneyin.");
    }

    public class ServiceManager : IServiceManager
    {
        public HttpClient Client { get; set; }
        public static int? ExceptionLogId = null;

        public ServiceManager(string serviceUrl = "Test")
        {
            // #if !DEBUG
            //             serviceUrl = "Live";
            // #endif
            if (Client == null)
            {
                var root = ConfigUtils.GetConfigurationRoot();

                Client = new HttpClient
                {
                    BaseAddress = new Uri(root.GetSection("Application").GetSection("Service")[serviceUrl])
                };
            }
        }

        public HttpResponseMessage GetResponse(string url)
        {
            return Client.GetAsync(url).Result;
        }

        public HttpResponseMessage PutResponse(string url, object model)
        {
            return Client.PutAsJsonAsync(url, model).Result;
        }

        public HttpResponseMessage PostResponse(string url, object model)
        {
            return Client.PostAsJsonAsync(url, model).Result;
        }

        public HttpResponseMessage DeleteResponse(string url)
        {
            return Client.DeleteAsync(url).Result;
        }


        #region Service Response

        public virtual ResponseBase<T> SuccessServiceResponse<T>(T data, string userMessage = Constants.DefaultUserMessagesTR.Success)
        {
            Dictionary<string, string> messages = null;
            if (!(userMessage is null))
            {
                messages = new Dictionary<string, string> { { Constants.ErrorMessageTypes.UserMessage, userMessage } };
            }

            return new ResponseBase<T> { Data = data, Status = ServiceResponseStatuses.Success, Messages = messages };
        }

        public virtual ResponseBase<T> WarningServiceResponse<T>(string userMessage = Constants.DefaultUserMessagesTR.Warning)
        {
            if (ExceptionLogId != null) userMessage = $"{userMessage}<br />Hata Kodu: {ExceptionLogId}";
            Dictionary<string, string> messages = new Dictionary<string, string> { { Constants.ErrorMessageTypes.UserMessage, userMessage } };

            return new ResponseBase<T> { Data = default, Status = ServiceResponseStatuses.Warning, Messages = messages };
        }

        public virtual ResponseBase<T> WarningServiceResponse<T>(T data, string userMessage = Constants.DefaultUserMessagesTR.Warning)
        {
            if (ExceptionLogId != null) userMessage = $"{userMessage}<br />Hata Kodu: {ExceptionLogId}";
            Dictionary<string, string> messages = new Dictionary<string, string> { { Constants.ErrorMessageTypes.UserMessage, userMessage } };

            return new ResponseBase<T> { Data = data, Status = ServiceResponseStatuses.Warning, Messages = messages };
        }

        public virtual ResponseBase<T> ErrorServiceResponse<T>(Exception ex, string userMessage = Constants.DefaultUserMessagesTR.Error)
        {
            Dictionary<string, string> messages = null;
            if (ex != null)
            {
                messages = new Dictionary<string, string> { { Constants.ErrorMessageTypes.BusinessError, ex.Message } };
                if (ExceptionLogId != null)
                {
                    messages.Add(Constants.ErrorMessageTypes.ExceptionLogId, ExceptionLogId.ToString());
                }
            }

            if (ExceptionLogId != null) userMessage = $"{userMessage}<br />Hata Kodu: {ExceptionLogId}";
            (messages ?? (messages = new Dictionary<string, string>())).Add(Constants.ErrorMessageTypes.UserMessage, userMessage);

            return new ResponseBase<T> { Data = default, Status = ServiceResponseStatuses.Error, Messages = messages };
        }

        public virtual ResponseBase<T> ErrorServiceResponse<T>(string userMessage = Constants.DefaultUserMessagesTR.Error)
        {
            if (ExceptionLogId != null) userMessage = $"{userMessage}<br />Hata Kodu: {ExceptionLogId}";
            Dictionary<string, string> messages = new Dictionary<string, string> { { Constants.ErrorMessageTypes.UserMessage, userMessage } };

            return new ResponseBase<T> { Data = default, Status = ServiceResponseStatuses.Error, Messages = messages };
        }

        public virtual ResponseBase<T> ServiceResponse<T>(T data, ServiceResponseStatuses status, Dictionary<string, string> messages, string userMessage)
        {
            if (userMessage != null)
            {
                if (ExceptionLogId != null) userMessage = $"{userMessage}<br />Hata Kodu: {ExceptionLogId}";
                (messages ?? (messages = new Dictionary<string, string>())).Add(Constants.ErrorMessageTypes.UserMessage, userMessage);
            }

            if (ExceptionLogId != null)
            {
                (messages ?? (messages = new Dictionary<string, string>())).Add(Constants.ErrorMessageTypes.ExceptionLogId, ExceptionLogId.ToString());
            }

            return new ResponseBase<T> { Data = data, Status = status, Messages = messages };
        }

        #endregion Service Response
    }
}