using Arch.CoreLibrary.Repositories;
using Arch.Mongo.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Arch.Mongo.Managers
{
    public interface IGenericRepository<TDocument> where TDocument : EntityBase
    {
        IQueryable<TDocument> _GetAll();
        IQueryable<TDocument> FilterBy(
            Expression<Func<TDocument, bool>> filterExpression);

        IQueryable<TProjected> FilterBy<TProjected>(
            Expression<Func<TDocument, bool>> filterExpression,
            Expression<Func<TDocument, TProjected>> projectionExpression);


        TDocument FindOne(Expression<Func<TDocument, bool>> filterExpression);

        Task<TDocument> FindOneAsync(Expression<Func<TDocument, bool>> filterExpression);

        TDocument FindById(string id);

        Task<TDocument> FindByIdAsync(string id);

        void InsertOne(TDocument document);

        Task InsertOneAsync(TDocument document);

        void InsertMany(ICollection<TDocument> documents);

        Task InsertManyAsync(ICollection<TDocument> documents);

        void ReplaceOne(TDocument document);

        Task ReplaceOneAsync(TDocument document);

        void DeleteOne(Expression<Func<TDocument, bool>> filterExpression);

        Task DeleteOneAsync(Expression<Func<TDocument, bool>> filterExpression);

        void DeleteById(string id);

        Task DeleteByIdAsync(string id);

        void DeleteMany(Expression<Func<TDocument, bool>> filterExpression);

        Task DeleteManyAsync(Expression<Func<TDocument, bool>> filterExpression);

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

    }
}
