using ARCH.CoreLibrary.Entities;
using ARCH.CoreLibrary.Entities.CommonModels;
using ARCH.CoreLibrary.Entities.Log;
using ARCH.CoreLibrary.Utils;
using ARCH.CoreLibrary.Utils.Collections;
using ARCH.CoreLibrary.Utils.Security;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace ARCH.CoreLibrary.Repositories
{
    public class GenericRepository<C> : IGenericRepository<C>, IDisposable where C : DbContext
    {
        private readonly C _Context;

        public GenericRepository(C context)
        {
            if (_Context == null)
                _Context = context;
        }

        protected DbSet<TModel> DataModel<TModel>() where TModel : EntityBase
        {
            return _Context.Set<TModel>();
        }

        public virtual IQueryable<TModel> _GetAll<TModel>() where TModel : EntityBase
        {
            return DataModel<TModel>()
                .Where(x => x.Status == Constants.DbStatus.Active);
        }

        public virtual IQueryable<TModel> _GetAllPassive<TModel>() where TModel : EntityBase
        {
            return DataModel<TModel>()
                .Where(x => x.Status == Constants.DbStatus.Passive);
        }

        public virtual async Task<IEnumerable<TModel>> _GetAllAsync<TModel>() where TModel : EntityBase
        {
            return await DataModel<TModel>()
                .Where(x => x.Status == Constants.DbStatus.Active)
                .ToListAsync().ConfigureAwait(false);
        }

        public virtual async Task<IEnumerable<TModel>> _GetAllPassiveAsync<TModel>() where TModel : EntityBase
        {
            return await DataModel<TModel>()
                .Where(x => x.Status == Constants.DbStatus.Passive)
                .ToListAsync().ConfigureAwait(false);
        }

        public virtual IQueryable<TModel> _GetByExpression<TModel>(
            Expression<Func<TModel, bool>> filter = null,
            Func<IQueryable<TModel>, IOrderedQueryable<TModel>> orderBy = null,
            string includeProperties = "") where TModel : EntityBase
        {
            IQueryable<TModel> query = DataModel<TModel>();

            if (filter != null)
                query = query.Where(filter);

            for (int i = 0; i < includeProperties.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries).Length; i++)
            {
                var includeProperty = includeProperties.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries)[i];
                query = query.Include(includeProperty);
            }

            return orderBy != null ? orderBy(query) : query;
        }

        public virtual async Task<IEnumerable<TModel>> _GetByExpressionAsync<TModel>(
            Expression<Func<TModel, bool>> filter = null,
            Func<IQueryable<TModel>, IOrderedQueryable<TModel>> orderBy = null,
            string includeProperties = "") where TModel : EntityBase
        {
            IQueryable<TModel> query = DataModel<TModel>();

            if (filter != null)
                query = query.Where(filter);

            for (int i = 0; i < includeProperties.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries).Length; i++)
            {
                var includeProperty = includeProperties.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries)[i];
                query = query.Include(includeProperty);
            }

            var result = orderBy != null ? orderBy(query) : query;
            return await result.ToListAsync().ConfigureAwait(false);
        }

        public virtual IQueryable<TModel> _IncludeMultiple<TModel>(params Expression<Func<TModel, object>>[] includeExpressions) where TModel : EntityBase
        {
            IQueryable<TModel> query = DataModel<TModel>();
            foreach (var includeExpression in includeExpressions)
            {
                query = query.Include(includeExpression);
            }
            return query;
        }

        public virtual TModel _GetById<TModel>(object id) where TModel : EntityBase
        {
            if (id == null)
                throw new ArgumentNullException(nameof(id));
            return DataModel<TModel>().Find(id);
        }

        public virtual TModel _GetById<TModel>(int id) where TModel : EntityBase
        {
            if (id <= 0)
                throw new ArgumentOutOfRangeException(nameof(id));
            return DataModel<TModel>().Find(id);
        }

        public virtual async Task<TModel> _GetByIdAsync<TModel>(int id) where TModel : EntityBase
        {
            if (id <= 0)
                throw new ArgumentOutOfRangeException(nameof(id));
            return await DataModel<TModel>().FindAsync(id).ConfigureAwait(false);
        }

        public virtual void _Insert<TModel>(TModel entity, bool active = true) where TModel : EntityBase
        {
            if (entity == null)
                throw new ArgumentNullException(nameof(entity));
            entity.CreatedDate = DateTime.Now;
            entity.Status = active ? Constants.DbStatus.Active : Constants.DbStatus.Passive;
            DataModel<TModel>().Add(entity);
        }

        public virtual void _InsertAll<TModel>(IEnumerable<TModel> entityList, bool active = true) where TModel : EntityBase
        {
            if (entityList == null)
                throw new ArgumentNullException(nameof(entityList));

            entityList.ForEach(e =>
            {
                e.CreatedDate = DateTime.Now;
                e.Status = active ? Constants.DbStatus.Active : Constants.DbStatus.Passive;
            });

            DataModel<TModel>().AddRange(entityList);
        }

        public virtual void _Update<TModel>(TModel entity) where TModel : EntityBase
        {
            if (entity == null)
                throw new ArgumentNullException(nameof(entity));
            entity.ModifiedDate = DateTime.Now;
            DataModel<TModel>().Attach(entity);
            _Context.Entry(entity).State = EntityState.Modified;
        }

        public virtual void _UpdateAll<TModel>(IEnumerable<TModel> entityList) where TModel : EntityBase
        {
            if (entityList == null)
                throw new ArgumentNullException(nameof(entityList));
            entityList.ForEach(e => e.ModifiedDate = DateTime.Now);
            DataModel<TModel>().UpdateRange(entityList);
        }

        public virtual void _SetPassive<TModel>(TModel entity) where TModel : EntityBase
        {
            if (entity == null)
                throw new ArgumentNullException(nameof(entity));
            entity.ModifiedDate = DateTime.Now;
            entity.Status = Constants.DbStatus.Passive;
            DataModel<TModel>().Attach(entity);
            _Context.Entry(entity).State = EntityState.Modified;
        }

        public virtual bool _IsPassive<TModel>(TModel entity) where TModel : EntityBase
        {
            if (entity == null)
                throw new ArgumentNullException(nameof(entity));
            return _GetById<TModel>(entity.Id).Status == Constants.DbStatus.Passive;
        }

        public virtual bool _IsPassive<TModel>(int id) where TModel : EntityBase
        {
            if (id <= 0)
                throw new ArgumentNullException(nameof(id));
            return _GetById<TModel>(id).Status == Constants.DbStatus.Passive;
        }

        public virtual bool _IsSoftDeleted<TModel>(TModel entity) where TModel : EntityBase
        {
            if (entity == null)
                throw new ArgumentNullException(nameof(entity));
            return _GetById<TModel>(entity.Id).Status == Constants.DbStatus.Deleted;
        }

        public virtual bool _IsSoftDeleted<TModel>(int id) where TModel : EntityBase
        {
            if (id <= 0)
                throw new ArgumentNullException(nameof(id));
            return _GetById<TModel>(id).Status == Constants.DbStatus.Deleted;
        }

        public virtual void _RawDelete<TModel>(TModel entity) where TModel : EntityBase
        {
            if (entity == null)
                throw new ArgumentNullException(nameof(entity));
            if (DataModel<TModel>() == null)
                throw new ArgumentNullException("Entities");
            entity.ModifiedDate = DateTime.Now;
            if (_Context.Entry(entity).State == EntityState.Detached)
                DataModel<TModel>().Attach(entity);

            DataModel<TModel>().Remove(entity);
        }

        public virtual void _RawDelete<TModel>(object id) where TModel : EntityBase
        {
            if (id == null)
                throw new ArgumentNullException(nameof(id));
            _RawDelete(_GetById<TModel>(id));
        }

        public virtual void _RawDelete<TModel>(int id) where TModel : EntityBase
        {
            if (id <= 0)
                throw new ArgumentOutOfRangeException(nameof(id));
            _RawDelete(_GetById<TModel>(id));
        }

        public virtual void _SoftDelete<TModel>(TModel entity) where TModel : EntityBase
        {
            if (entity == null)
                throw new ArgumentNullException(nameof(entity));
            if (DataModel<TModel>() == null)
                throw new ArgumentNullException("Entities");
            entity.Status = Constants.DbStatus.Deleted;

            _Update(entity);
        }

        public virtual void _SoftDelete<TModel>(object id) where TModel : EntityBase
        {
            if (id == null)
                throw new ArgumentNullException(nameof(id));
            _SoftDelete(_GetById<TModel>(id));
        }

        public virtual void _SoftDelete<TModel>(int id) where TModel : EntityBase
        {
            if (id <= 0)
                throw new ArgumentOutOfRangeException(nameof(id));
            _SoftDelete(_GetById<TModel>(id));
        }

        public void _RawDeleteAll(string tableName)
        {
            _Context.Database.ExecuteSqlRaw($"DELETE FROM {tableName}");
        }

        public void _RawDeleteAll<TModel>(Expression<Func<TModel, bool>> match) where TModel : EntityBase
        {
            var entities = DataModel<TModel>().Where(match);
            entities.ForEach(e => e.ModifiedDate = DateTime.Now);
            DataModel<TModel>().RemoveRange(entities);
        }

        public void _SoftDeleteAll(string tableName)
        {
            _Context.Database.ExecuteSqlRaw($"UPDATE {tableName} SET Status = {Constants.DbStatus.Deleted}, ModifiedDate = {DateTime.Now}");
        }

        public void _SoftDeleteAll<TModel>(Expression<Func<TModel, bool>> match) where TModel : EntityBase
        {
            var entities = DataModel<TModel>().Where(match);
            entities.ForEach(e => e.CreatedDate = DateTime.Now);
            DataModel<TModel>().UpdateRange(entities);
        }

        //public virtual IEnumerable<TModel> _GetWithRawSql<TModel>(RawSqlString query, params object[] parameters) where TModel : EntityBase
        //{
        //    if (DataModel<TModel>() == null)
        //        throw new ArgumentNullException("Entities");
        //    return DataModel<TModel>().FromSqlRaw(query, parameters);
        //}

        public virtual IEnumerable<TModel> _GetWithRawSql<TModel>(string query, params object[] parameters) where TModel : EntityBase
        {
            if (DataModel<TModel>() == null)
                throw new ArgumentNullException("Entities");
            return DataModel<TModel>().FromSqlRaw(query, parameters);
        }

        public virtual TModel _Find<TModel>(Expression<Func<TModel, bool>> match) where TModel : EntityBase
        {
            if (DataModel<TModel>() == null)
                throw new ArgumentNullException("Entities");
            return DataModel<TModel>().SingleOrDefault(match);
        }

        public async Task<TModel> _FindAsync<TModel>(Expression<Func<TModel, bool>> match) where TModel : EntityBase
        {
            if (DataModel<TModel>() == null)
                throw new ArgumentNullException("Entities");
            return await DataModel<TModel>().SingleOrDefaultAsync(match).ConfigureAwait(false);
        }

        public IQueryable<TModel> _FindAll<TModel>(Expression<Func<TModel, bool>> match) where TModel : EntityBase
        {
            if (DataModel<TModel>() == null)
                throw new ArgumentNullException("Entities");
            return DataModel<TModel>().Where(match);
        }

        public async Task<IEnumerable<TModel>> _FindAllAsync<TModel>(Expression<Func<TModel, bool>> match) where TModel : EntityBase
        {
            if (DataModel<TModel>() == null)
                throw new ArgumentNullException("Entities");
            return await DataModel<TModel>().Where(match).ToListAsync().ConfigureAwait(false);
        }

        public bool _IsAttached<TModel>(TModel entity) where TModel : EntityBase
        {
            return DataModel<TModel>().Local.Any(e => e == entity);
        }

        public virtual bool _Exists<TModel>(object id) where TModel : EntityBase
        {
            if (id == null)
                throw new ArgumentNullException(nameof(id));
            if (DataModel<TModel>() == null)
                throw new ArgumentNullException("Entities");
            return DataModel<TModel>().Find(id) != null;
        }

        public virtual bool _Exists<TModel>(int id) where TModel : EntityBase
        {
            if (id <= 0)
                throw new ArgumentOutOfRangeException(nameof(id));
            return DataModel<TModel>().Find(id) != null;
        }

        public virtual async Task<bool> _ExistsAsync<TModel>(int id) where TModel : EntityBase
        {
            if (id <= 0)
                throw new ArgumentOutOfRangeException(nameof(id));
            return await DataModel<TModel>().FindAsync(id).ConfigureAwait(false) != null;
        }

        public virtual bool _Contains<TModel>(Expression<Func<TModel, bool>> match) where TModel : EntityBase
        {
            if (DataModel<TModel>() == null)
                throw new ArgumentNullException("Entities");
            return DataModel<TModel>().Count(match) > 0;
        }

        public virtual async Task<bool> _ContainsAsync<TModel>(Expression<Func<TModel, bool>> match) where TModel : EntityBase
        {
            return await DataModel<TModel>().CountAsync(match).ConfigureAwait(false) > 0;
        }

        public virtual int _Count<TModel>() where TModel : EntityBase
        {
            return DataModel<TModel>().Count();
        }

        public virtual async Task<int> _CountAsync<TModel>() where TModel : EntityBase
        {
            return await DataModel<TModel>().CountAsync().ConfigureAwait(false);
        }

        public void _ReSeedTable(string tableName)
        {
            _Context.Database.ExecuteSqlRaw($"DBCC CHECKIDENT('{tableName}', RESEED, 0)");
        }

        public IEnumerable<string> _GetColumns<TModel>() where TModel : EntityBase
        {
            return typeof(TModel)
                    .GetProperties()
                    .Where(e => e.Name != "Id" && !e.PropertyType.GetTypeInfo().IsGenericType)
                    .Select(e => e.Name);
        }

        public IEnumerable<string> _GetProperties<TModel>() where TModel : EntityBase
        {
            return typeof(TModel)
                    .GetProperties()
                    .Select(e => e.Name);
        }

        public int _Save()
        {
            return _Context.SaveChanges();
        }

        public async Task<int> _SaveAsync()
        {
            var affected = _Context.SaveChangesAsync();
            return await affected;
        }

        public void LogLogin(LogLevel logLevel, UserModel userModel)
        {
            var log = new LoginLog
            {
                UserId = userModel.Id,
                ProjectId = userModel.CurrentCompanyId,
                Token = userModel.Token,
                Email = userModel.Username,
                Ip = Helpers.GetIP(),
                CreatedDate = DateTime.Now,
                CreatedBy = userModel.Username,
            };

            try
            {
                DataModel<LoginLog>().Add(log);
                _Save();
            }
            catch (Exception ex)
            {
                var exceptionLog = new ExceptionLog
                {
                    Environment = nameof(ProjectEnvironment.Repository),
                    HostName = Environment.MachineName,
                    ExceptionMessage = "Login log DB'ye yazılırken hata oluştu",
                    MethodName = "Login",
                    LogLevel = logLevel.ToString(),
                    Request = JsonConvert.SerializeObject(userModel),
                    Response = "",
                    Url = "",
                    Username = userModel.Username,
                };

                logExceptionToFile(exceptionLog, ex);
            }
        }

        public void Log(LogLevel logLevel, string message, string userName, object requestModel, object responseModel, string url)
        {
            var log = new AuditLog
            {
                Environment = nameof(ProjectEnvironment.Repository),
                HostName = Environment.MachineName,
                LogLevel = logLevel.ToString(),
                MethodName = ConfigUtils.GetCurrentMethodName(2),
                Details = message,
                Request = requestModel == null ? "" : JsonConvert.SerializeObject(requestModel),
                Response = responseModel == null ? "" : JsonConvert.SerializeObject(responseModel),
                Url = url,
                CreatedDate = DateTime.Now,
                CreatedBy = userName,
                Username = userName
            };

            try
            {
                DataModel<AuditLog>().Add(log);
                _Save();
            }
            catch (Exception ex)
            {
                var exceptionLog = new ExceptionLog
                {
                    Environment = log.Environment,
                    HostName = log.HostName,
                    ExceptionMessage = "Audit log DB'ye yazılırken hata oluştu",
                    MethodName = log.MethodName,
                    LogLevel = log.LogLevel,
                    Request = log.Request,
                    Response = log.Response,
                    Url = log.Url,
                    Username = log.Username,
                };

                logExceptionToFile(exceptionLog, ex);
            }
        }

        public void Log(Dictionary<LogLevel, string> messages, string userName, object requestModel, object responseModel, string url)
        {
            if (messages?.Any() == true)
            {
                foreach (var x in messages)
                {
                    var log = new AuditLog
                    {
                        Environment = nameof(ProjectEnvironment.Repository),
                        HostName = Environment.MachineName,
                        LogLevel = x.Key.ToString(),
                        MethodName = ConfigUtils.GetCurrentMethodName(2),
                        Details = x.Value,
                        Request = requestModel == null ? "" : JsonConvert.SerializeObject(requestModel),
                        Response = responseModel == null ? "" : JsonConvert.SerializeObject(responseModel),
                        Url = url,
                        CreatedDate = DateTime.Now,
                        CreatedBy = userName,
                        Username = userName
                    };

                    DataModel<AuditLog>().Add(log);
                }
            }

            try
            {
                _Save();
            }
            catch (Exception ex)
            {
                var exceptionLog = new ExceptionLog
                {
                    Environment = nameof(ProjectEnvironment.Repository),
                    HostName = Environment.MachineName,
                    ExceptionMessage = "Audit log Bulk olarak DB'ye yazılırken hata oluştu",
                    MethodName = ConfigUtils.GetCurrentMethodName(2),
                    Request = requestModel == null ? "" : JsonConvert.SerializeObject(requestModel),
                    Response = responseModel == null ? "" : JsonConvert.SerializeObject(responseModel),
                    Url = url,
                    Username = userName,
                };

                logExceptionToFile(exceptionLog, ex);
            }
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
#if !DEBUG
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
                            this._Insert(log);
                            this._Save();

                            sendPerformanceEmail(log, "PERFORMANCE");
                        }
                        catch (Exception ex)
                        {
                            logExceptionToFile(new ExceptionLog { MethodName = log.MethodName, Username = log.Username, ExceptionMessage = "Performance logunu DB ye yazarken hata oluştu." }, ex);
                            sendPerformanceEmail(log, "PERFORMANCE - FILELOG");
                        }
                        finally
                        {
                            Dispose();
                        }
                    }
                }
#endif
            }
            catch (Exception ex)
            {
#if !DEBUG
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
                    this._Insert(log);
                    this._Save();
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
                    Dispose();
                }
#endif
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

        private bool _Disposed = false;

        protected virtual void Dispose(bool disposing)
        {
            if (!_Disposed && disposing)
                _Context.Dispose();
            _Disposed = true;
        }

        public void Dispose()
        {
            if (_Context == null) return;
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        #endregion

    }

    public enum ProjectEnvironment
    {
        Repository = 1,
        Business = 2,
        Service = 3,
        UI = 4
    }
}
