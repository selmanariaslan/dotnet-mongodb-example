using Arch.CoreLibrary.Entities.Log;
using Arch.Mongo.Models;
using Arch.CoreLibrary;
using Arch.CoreLibrary.Repositories;
using Arch.CoreLibrary.Utils;
using Arch.CoreLibrary.Utils.Security;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using MongoDB.Bson;
using MongoDB.Driver;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Arch.Mongo.Managers
{
    public class GenericRepository<TDocument> : IGenericRepository<TDocument> where TDocument : EntityBase
    {
        private readonly IMongoCollection<TDocument> _collection;
        public GenericRepository(IMongoDbSettings settings)
        {
            var database = new MongoClient(settings.ConnectionString).GetDatabase(settings.DatabaseName);
            _collection = database.GetCollection<TDocument>((typeof(TDocument)).Name);
        }

        public virtual IQueryable<TDocument> _GetAll()
        {
            return _collection.AsQueryable();
        }

        public virtual IQueryable<TDocument> FilterBy(
            Expression<Func<TDocument, bool>> filterExpression)
        {
            var resp= _collection.Find(filterExpression);
            return resp.ToEnumerable().AsQueryable();
        }

        public virtual IQueryable<TProjected> FilterBy<TProjected>(
            Expression<Func<TDocument, bool>> filterExpression,
            Expression<Func<TDocument, TProjected>> projectionExpression)
        {
            var resp=_collection.Find(filterExpression).Project(projectionExpression);
            return resp.ToEnumerable().AsQueryable();
        }

        public virtual TDocument FindOne(Expression<Func<TDocument, bool>> filterExpression)
        {
            return _collection.Find(filterExpression).FirstOrDefault();
        }

        public virtual Task<TDocument> FindOneAsync(Expression<Func<TDocument, bool>> filterExpression)
        {
            return Task.Run(() => _collection.Find(filterExpression).FirstOrDefaultAsync());
        }

        public virtual TDocument FindById(string id)
        {
            var objectId = new ObjectId(id);
            var filter = Builders<TDocument>.Filter.Eq(doc => doc.Id, id);
            return _collection.Find(filter).SingleOrDefault();
        }

        public virtual Task<TDocument> FindByIdAsync(string id)
        {
            return Task.Run(() =>
            {
                var objectId = new ObjectId(id);
                var filter = Builders<TDocument>.Filter.Eq(doc => doc.Id, id);
                return _collection.Find(filter).SingleOrDefaultAsync();
            });
        }


        public virtual void InsertOne(TDocument document)
        {
            _collection.InsertOne(document);
        }

        public virtual Task InsertOneAsync(TDocument document)
        {
            return Task.Run(() => _collection.InsertOneAsync(document));
        }

        public void InsertMany(ICollection<TDocument> documents)
        {
            _collection.InsertMany(documents);
        }


        public virtual async Task InsertManyAsync(ICollection<TDocument> documents)
        {
            await _collection.InsertManyAsync(documents);
        }

        public void ReplaceOne(TDocument document)
        {
            var filter = Builders<TDocument>.Filter.Eq(doc => doc.Id, document.Id);
            _collection.FindOneAndReplace(filter, document);
        }

        public virtual async Task ReplaceOneAsync(TDocument document)
        {
            var filter = Builders<TDocument>.Filter.Eq(doc => doc.Id, document.Id);
            await _collection.FindOneAndReplaceAsync(filter, document);
        }

        public void DeleteOne(Expression<Func<TDocument, bool>> filterExpression)
        {
            _collection.FindOneAndDelete(filterExpression);
        }

        public Task DeleteOneAsync(Expression<Func<TDocument, bool>> filterExpression)
        {
            return Task.Run(() => _collection.FindOneAndDeleteAsync(filterExpression));
        }

        public void DeleteById(string id)
        {
            var objectId = new ObjectId(id);
            var filter = Builders<TDocument>.Filter.Eq(doc => doc.Id, id);
            _collection.FindOneAndDelete(filter);
        }

        public Task DeleteByIdAsync(string id)
        {
            return Task.Run(() =>
            {
                var objectId = new ObjectId(id);
                var filter = Builders<TDocument>.Filter.Eq(doc => doc.Id, id);
                _collection.FindOneAndDeleteAsync(filter);
            });
        }

        public void DeleteMany(Expression<Func<TDocument, bool>> filterExpression)
        {
            _collection.DeleteMany(filterExpression);
        }

        public Task DeleteManyAsync(Expression<Func<TDocument, bool>> filterExpression)
        {
            return Task.Run(() => _collection.DeleteManyAsync(filterExpression));
        }


        public void Run(
       ProjectEnvironment projectEnvironment,
       string username,
       Action action,
       long maximumMilseconds = 10000,
       Action<Exception> errorAction = null,
       string currentUrl = null,
       object requestModel = null,
       object responseModel = null,
       Action finallyAction = null)
        {
            var watch = System.Diagnostics.Stopwatch.StartNew();

            try
            {
                action();
                //#if !DEBUG
                /// Performance Log DB
                if (watch.IsRunning)
                {
                    watch.Stop();

                    if (watch?.ElapsedMilliseconds > maximumMilseconds)
                    {
                        var log = new PerformanceLog
                        {
                            HostName = Environment.MachineName,
                            MethodName = ConfigUtils.GetCurrentMethodName(2),
                            ElapsedMilisecond = watch.ElapsedMilliseconds,
                            Url = currentUrl,
                            Request = requestModel == null ? "" : JsonConvert.SerializeObject(requestModel),
                            Response = responseModel == null ? "" : JsonConvert.SerializeObject(responseModel),
                            Details = null,
                            Username = username,
                            CreatedBy = username
                        };

                        try
                        {
                            //this.InsertOne(log);
                            //this._Save();

                            //sendPerformanceEmail(log, "PERFORMANCE");
                        }
                        catch (Exception ex)
                        {
                            logExceptionToFile(new ExceptionLog { MethodName = log.MethodName, Username = log.Username, ExceptionMessage = "Performance logunu DB ye yazarken hata oluştu." }, ex);
                            //sendPerformanceEmail(log, "PERFORMANCE - FILELOG");
                        }
                        finally
                        {
                            //Dispose();
                        }
                    }
                }
                //#endif
            }
            catch (Exception ex)
            {
                //#if !DEBUG
                /// Error Log DB
                var log = new ExceptionLog
                {
                    Environment = projectEnvironment.ToString(),
                    HostName = Environment.MachineName,
                    LogLevel = Constants.ErrorMessageTypes.Error.ToUpper(),
                    MethodName = ConfigUtils.GetCurrentMethodName(2),
                    ExceptionMessage = ex?.Message,
                    StackTrace = ex?.StackTrace,
                    Url = currentUrl,
                    Request = requestModel == null ? "" : JsonConvert.SerializeObject(requestModel),
                    Response = responseModel == null ? "" : JsonConvert.SerializeObject(responseModel),
                    Details = ex?.InnerException?.Message,
                    Username = username,
                    CreatedBy = username
                };

                try
                {
                    //this.InsertOneAsync(log);
                    //this._Save();
                    ServiceManager.ExceptionLogId = log.Id;

                    sendExceptionEmail(log, "HATA");
                }
                catch (Exception lastEx)
                {
                    logExceptionToFile(log, lastEx);
                    sendExceptionEmail(log, "HATA - FILELOG");
                }
                finally
                {
                    //Dispose();
                }
                //#endif
                errorAction?.Invoke(ex);
            }
            finally
            {
                if (watch.IsRunning)
                {
                    watch.Stop();
                }

                finallyAction?.Invoke();
            }
        }

        private void sendExceptionEmail(ExceptionLog log, string subject)
        {
            Helpers.SendMail(
                        new System.Net.NetworkCredential(
                            ConfigUtils.GetConfigurationRoot().GetSection("Logging").GetSection("EmailConfig").GetSection("NetworkCredentials")["Username"],
                           new CryptoUtils().DecryptString(ConfigUtils.GetConfigurationRoot().GetSection("Logging").GetSection("EmailConfig").GetSection("NetworkCredentials")["Password"])),
                        new System.Net.Mail.MailAddress(
                            ConfigUtils.GetConfigurationRoot().GetSection("Logging").GetSection("EmailConfig").GetSection("NetworkCredentials")["Username"],
                            ConfigUtils.GetConfigurationRoot().GetSection("Logging").GetSection("EmailConfig").GetSection("MailAddress")["DisplayName"]),
                        ConfigUtils.GetConfigurationRoot().GetSection("Logging").GetSection("EmailConfig")["Host"],
                        ConfigUtils.GetConfigurationRoot().GetSection("Logging").GetSection("EmailConfig")["To"].Split(';').Where(x => !(x is null)).ToList(),
                        ConfigUtils.GetConfigurationRoot().GetSection("Logging").GetSection("EmailConfig")["Cc"].Split(';').Where(x => !(x is null)).ToList(),
                        ConfigUtils.GetConfigurationRoot().GetSection("Logging").GetSection("EmailConfig")["Bcc"].Split(';').Where(x => !(x is null)).ToList(),
                        $"{subject} - {log.MethodName}",
                        $@"
<table cellpadding='5' cellspacing='5'>
    <tbody>
        <tr>
            <th>ID</th>
            <th>:</th>
            <td>{log.Id}</td>
        </tr>
        <tr>
            <th>PROJECT</th>
            <th>:</th>
            <td>{log.MethodName.Split('.').FirstOrDefault()}</td>
        </tr>
        <tr>
            <th>ENVIRONMENT</th>
            <th>:</th>
            <td>{log.Environment}</td>
        </tr>
        <tr>
            <th>METHOD</th>
            <th>:</th>
            <td>{log.MethodName.Split('.').LastOrDefault()}</td>
        </tr>
        <tr>
            <th>MESSAGE</th>
            <th>:</th>
            <td>{log.ExceptionMessage}</td>
        </tr>
        <tr>
            <th>URL</th>
            <th>:</th>
            <td>{log.Url}</td>
        </tr>
        <tr>
            <th>REQUEST</th>
            <th>:</th>
            <td>{log.Request}</td>
        </tr>
        <tr>
            <th>RESPONSE</th>
            <th>:</th>
            <td>{log.Response}</td>
        </tr>
        <tr>
            <th>STACK TRACE</th>
            <th>:</th>
            <td>{log.StackTrace}</td>
        </tr>
        <tr>
            <th>USER</th>
            <th>:</th>
            <td>{log.Username}</td>
        </tr>
    </tbody>
</table>"
);
        }

        private void sendPerformanceEmail(PerformanceLog log, string subject)
        {
            Helpers.SendMail(
                        new System.Net.NetworkCredential(
                            ConfigUtils.GetConfigurationRoot().GetSection("Logging").GetSection("EmailConfig").GetSection("NetworkCredentials")["Username"],
                            new CryptoUtils().DecryptString(ConfigUtils.GetConfigurationRoot().GetSection("Logging").GetSection("EmailConfig").GetSection("NetworkCredentials")["Password"])),
                        new System.Net.Mail.MailAddress(
                            ConfigUtils.GetConfigurationRoot().GetSection("Logging").GetSection("EmailConfig").GetSection("NetworkCredentials")["Username"],
                            ConfigUtils.GetConfigurationRoot().GetSection("Logging").GetSection("EmailConfig").GetSection("MailAddress")["DisplayName"]),
                        ConfigUtils.GetConfigurationRoot().GetSection("Logging").GetSection("EmailConfig")["Host"],
                        ConfigUtils.GetConfigurationRoot().GetSection("Logging").GetSection("EmailConfig")["To"].Split(';').Where(x => !(x is null)).ToList(),
                        ConfigUtils.GetConfigurationRoot().GetSection("Logging").GetSection("EmailConfig")["Cc"].Split(';').Where(x => !(x is null)).ToList(),
                        ConfigUtils.GetConfigurationRoot().GetSection("Logging").GetSection("EmailConfig")["Bcc"].Split(';').Where(x => !(x is null)).ToList(),
                                $"{subject} - {log.MethodName}",
                                 $@"
<table cellpadding='5' cellspacing='5'>
    <tbody>
        <tr>
            <th>ID</th>
            <th>:</th>
            <td>{log.Id}</td>
        </tr>
        <tr>
            <th>ELAPSED MILISECOND</th>
            <th>:</th>
            <td>{log.ElapsedMilisecond}</td>
        </tr>
        <tr>
            <th>PROJECT</th>
            <th>:</th>
            <td>{log.MethodName.Split('.').FirstOrDefault()}</td>
        </tr>
        <tr>
            <th>METHOD</th>
            <th>:</th>
            <td>{log.MethodName.Split('.').LastOrDefault()}</td>
        </tr>
        <tr>
            <th>URL</th>
            <th>:</th>
            <td>{log.Url}</td>
        </tr>
        <tr>
            <th>REQUEST</th>
            <th>:</th>
            <td>{log.Request}</td>
        </tr>
        <tr>
            <th>RESPONSE</th>
            <th>:</th>
            <td>{log.Response}</td>
        </tr>
        <tr>
            <th>USER</th>
            <th>:</th>
            <td>{log.Username}</td>
        </tr>
    </tbody>
</table>"
);
        }

        private static void logExceptionToFile(ExceptionLog log, Exception lastException = null)
        {
            var logText = new StringBuilder();
            logText
                .Append("DATE : ").AppendLine(DateTime.Now.ToLongDateString())
                .Append("HOST : ").AppendLine(Environment.MachineName)
                .Append("URL : ").AppendLine(log.Url)
                .Append("METHOD : ").AppendLine(log.MethodName)
                .Append("REQUEST : ").AppendLine(log.Request)
                .Append("RESPONSE : ").AppendLine(log.Response)
                .Append("MESSAGE : ").AppendLine(log.ExceptionMessage)
                .Append("STACKTRACE : ").AppendLine(log.StackTrace)
                .Append("USER : ").AppendLine(log.Username)
                .AppendLine(Environment.NewLine);

            if (lastException != null)
            {
                logText
                    .Append("MESSAGE - LogDbEx : ").AppendLine(lastException.Message)
                    .Append("STACKTARCE - LogDbEx : ").AppendLine(lastException.StackTrace)
                    .AppendLine(Environment.NewLine);
            }

            logText
                .AppendLine("-------------------------------------------------------------------------------------------")
                .AppendLine(Environment.NewLine);

            string folder = "Logs";
            bool exists = Directory.Exists(folder.ToApplicationPath());
            if (!exists)
            {
                Directory.CreateDirectory(folder.ToApplicationPath());
            }

            string month = DateTime.Now.Month.ToString().Length == 1 ? $"0{DateTime.Now.Month}" : DateTime.Now.Month.ToString();
            string day = DateTime.Now.Day.ToString().Length == 1 ? $"0{DateTime.Now.Day}" : DateTime.Now.Day.ToString();

            string filePath = $"Logs/{log.Environment}Logs_{DateTime.Now.Year}{month}{day}.txt".ToApplicationPath();
            using (StreamWriter writer = new StreamWriter(filePath, true))
            {
                writer.WriteLine(logText.ToString());
            }
        }

        #region Dispose

        //private bool _Disposed = false;

        //protected virtual void Dispose(bool disposing)
        //{
        //    if (!_Disposed && disposing)
        //        _collection.Dispose();
        //    _Disposed = true;
        //}

        //public void Dispose()
        //{
        //    if (_collection == null) return;
        //    Dispose(true);
        //    GC.SuppressFinalize(this);
        //}

        #endregion
    }
}
