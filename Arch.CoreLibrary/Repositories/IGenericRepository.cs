using Arch.CoreLibrary.Entities;
using Arch.CoreLibrary.Entities.CommonModels;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Arch.CoreLibrary.Repositories
{
    public interface IGenericRepository<C> where C : DbContext
    {
        IQueryable<TModel> _GetAll<TModel>() where TModel : EntityBase;

        Task<IEnumerable<TModel>> _GetAllAsync<TModel>() where TModel : EntityBase;

        IQueryable<TModel> _GetAllPassive<TModel>() where TModel : EntityBase;

        Task<IEnumerable<TModel>> _GetAllPassiveAsync<TModel>() where TModel : EntityBase;

        IQueryable<TModel> _GetByExpression<TModel>(Expression<Func<TModel, bool>> filter = null, Func<IQueryable<TModel>, IOrderedQueryable<TModel>> orderBy = null, string includeProperties = "") where TModel : EntityBase;

        Task<IEnumerable<TModel>> _GetByExpressionAsync<TModel>(Expression<Func<TModel, bool>> filter = null, Func<IQueryable<TModel>, IOrderedQueryable<TModel>> orderBy = null, string includeProperties = "") where TModel : EntityBase;

        IQueryable<TModel> _IncludeMultiple<TModel>(params Expression<Func<TModel, object>>[] includeExpressions) where TModel : EntityBase;

        TModel _GetById<TModel>(object id) where TModel : EntityBase;

        TModel _GetById<TModel>(int id) where TModel : EntityBase;

        Task<TModel> _GetByIdAsync<TModel>(int id) where TModel : EntityBase;

        void _Insert<TModel>(TModel entity, bool active = true) where TModel : EntityBase;

        void _InsertAll<TModel>(IEnumerable<TModel> entityList, bool active = true) where TModel : EntityBase;

        void _Update<TModel>(TModel entity) where TModel : EntityBase;

        void _UpdateAll<TModel>(IEnumerable<TModel> entityList) where TModel : EntityBase;

        void _SetPassive<TModel>(TModel entity) where TModel : EntityBase;

        bool _IsPassive<TModel>(TModel entity) where TModel : EntityBase;

        bool _IsPassive<TModel>(int id) where TModel : EntityBase;

        bool _IsSoftDeleted<TModel>(TModel entity) where TModel : EntityBase;

        bool _IsSoftDeleted<TModel>(int id) where TModel : EntityBase;

        void _RawDelete<TModel>(TModel entity) where TModel : EntityBase;

        void _RawDelete<TModel>(object id) where TModel : EntityBase;

        void _RawDelete<TModel>(int id) where TModel : EntityBase;

        void _SoftDelete<TModel>(TModel entity) where TModel : EntityBase;

        void _SoftDelete<TModel>(object id) where TModel : EntityBase;

        void _SoftDelete<TModel>(int id) where TModel : EntityBase;

        void _RawDeleteAll(string tableName);

        void _RawDeleteAll<TModel>(Expression<Func<TModel, bool>> match) where TModel : EntityBase;

        void _SoftDeleteAll(string tableName);

        void _SoftDeleteAll<TModel>(Expression<Func<TModel, bool>> match) where TModel : EntityBase;

        //IEnumerable<TModel> _GetWithRawSql<TModel>(Microsoft.EntityFrameworkCore.RawSqlString query, params object[] parameters) where TModel : EntityBase;

        IEnumerable<TModel> _GetWithRawSql<TModel>(string query, params object[] parameters) where TModel : EntityBase;

        TModel _Find<TModel>(Expression<Func<TModel, bool>> match) where TModel : EntityBase;

        Task<TModel> _FindAsync<TModel>(Expression<Func<TModel, bool>> match) where TModel : EntityBase;

        IQueryable<TModel> _FindAll<TModel>(Expression<Func<TModel, bool>> match) where TModel : EntityBase;

        Task<IEnumerable<TModel>> _FindAllAsync<TModel>(Expression<Func<TModel, bool>> match) where TModel : EntityBase;

        bool _IsAttached<TModel>(TModel entity) where TModel : EntityBase;

        bool _Exists<TModel>(object id) where TModel : EntityBase;

        bool _Exists<TModel>(int id) where TModel : EntityBase;

        Task<bool> _ExistsAsync<TModel>(int id) where TModel : EntityBase;

        bool _Contains<TModel>(Expression<Func<TModel, bool>> match) where TModel : EntityBase;

        Task<bool> _ContainsAsync<TModel>(Expression<Func<TModel, bool>> match) where TModel : EntityBase;

        int _Count<TModel>() where TModel : EntityBase;

        Task<int> _CountAsync<TModel>() where TModel : EntityBase;

        void _ReSeedTable(string tableName);

        IEnumerable<string> _GetColumns<TModel>() where TModel : EntityBase;

        IEnumerable<string> _GetProperties<TModel>() where TModel : EntityBase;

        int _Save();

        Task<int> _SaveAsync();

        void Dispose();

        void Run(
         ProjectEnvironment projectEnvironment,
         string username,
         Action action,
         long maximumMilseconds = 10000,
         Action<Exception> errorAction = null,
         string currentUrl = null,
         object requestModel = null,
         object responseModel = null,
         Action finallyAction = null);

        void LogLogin(LogLevel logLevel, UserModel userModel);

        void Log(Dictionary<LogLevel, string> messages, string userName, object requestModel = null, object responseModel = null, string url = "");

        void Log(LogLevel logLevel, string message, string userName, object requestModel = null, object responseModel = null, string url = "");
    }
}
