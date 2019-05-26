using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;
using MongoDB.Driver;

namespace MongoDB.Data
{
    public interface IRepository<T> where T : IModel
    {
        #region MongoSpecific

        /// <summary>
        /// mongo collection
        /// </summary>
        IMongoCollection<T> Collection { get; }

        /// <summary>
        /// filter for collection
        /// </summary>
        FilterDefinitionBuilder<T> Filter { get; }

        List<FilterDefinition<T>> Filters { get; }

        /// <summary>
        /// projector for collection
        /// </summary>
        ProjectionDefinitionBuilder<T> Project { get; }

        List<ProjectionDefinition<T>> Projects { get; }

        /// <summary>
        /// updater for collection
        /// </summary>
        UpdateDefinitionBuilder<T> Updater { get; }

        List<UpdateDefinition<T>> Updaters { get; }

        #endregion MongoSpecific

        #region CRUD

        #region Delete

        /// <summary>
        /// delete by id
        /// </summary>
        /// <param name="id">id</param>
        bool Delete(string id);

        /// <summary>
        /// delete Model
        /// </summary>
        /// <param name="Model">Model</param>
        bool Delete(T Model);

        /// <summary>
        /// delete items with filter
        /// </summary>
        /// <param name="filter">expression filter</param>
        bool Delete(Expression<Func<T, bool>> filter);

        /// <summary>
        /// delete all documents
        /// </summary>
        bool DeleteAll();

        /// <summary>
        /// delete all documents with filter
        /// </summary>
        bool DeleteAll(Expression<Func<T, bool>> filter);

        /// <summary>
        /// delete by id
        /// </summary>
        /// <param name="id">id</param>
        Task<bool> DeleteAsync(string id);

        /// <summary>
        /// delete Model
        /// </summary>
        /// <param name="Model">Model</param>
        Task<bool> DeleteAsync(T Model);

        /// <summary>
        /// delete items with filter
        /// </summary>
        /// <param name="filter">expression filter</param>
        Task<bool> DeleteAsync(Expression<Func<T, bool>> filter);

        /// <summary>
        /// delete all documents
        /// </summary>
        Task<bool> DeleteAllAsync();

        /// <summary>
        /// delete all documents with filter
        /// </summary>
        Task<bool> DeleteAllAsync(Expression<Func<T, bool>> filter);
        #endregion Delete

        #region Find

        /// <summary>
        /// find models
        /// </summary>
        /// <param name="filter">expression filter</param>
        /// <returns>collection of Model</returns>
        IEnumerable<T> Find(Expression<Func<T, bool>> filter);

        /// <summary>
        /// find models async
        /// </summary>
        /// <param name="filter">expression filter</param>
        /// <returns>collection of Model</returns>
        Task<List<T>> FindAsync(Expression<Func<T, bool>> filter);

        /// <summary>
        /// find models with paging
        /// </summary>
        /// <param name="filter">expression filter</param>
        /// <param name="pageIndex">page index, based on 0</param>
        /// <param name="size">number of items in page</param>
        /// <returns>collection of Model</returns>
        IEnumerable<T> Find(Expression<Func<T, bool>> filter, int pageIndex, int size);

        /// <summary>
        /// find models async with paging
        /// </summary>
        /// <param name="filter">expression filter</param>
        /// <param name="pageIndex">page index, based on 0</param>
        /// <param name="size">number of items in page</param>
        /// <returns>collection of Model</returns>
        Task<List<T>> FindAsync(Expression<Func<T, bool>> filter, int pageIndex, int size);

        /// <summary>
        /// find models with paging and ordering
        /// default ordering is descending
        /// </summary>
        /// <param name="filter">expression filter</param>
        /// <param name="order">ordering parameters</param>
        /// <param name="pageIndex">page index, based on 0</param>
        /// <param name="size">number of items in page</param>
        /// <returns>collection of Model</returns>
        IEnumerable<T> Find(Expression<Func<T, bool>> filter, Expression<Func<T, object>> order, int pageIndex, int size);

        /// <summary>
        /// find models async with paging and ordering
        /// default ordering is descending
        /// </summary>
        /// <param name="filter">expression filter</param>
        /// <param name="order">ordering parameters</param>
        /// <param name="pageIndex">page index, based on 0</param>
        /// <param name="size">number of items in page</param>
        /// <returns>collection of Model</returns>
        Task<List<T>> FindAsync(Expression<Func<T, bool>> filter, Expression<Func<T, object>> order, int pageIndex, int size);

        /// <summary>
        /// find models with paging and ordering in direction
        /// </summary>
        /// <param name="filter">expression filter</param>
        /// <param name="order">ordering parameters</param>
        /// <param name="pageIndex">page index, based on 0</param>
        /// <param name="size">number of items in page</param>
        /// <param name="isDescending">ordering direction</param>
        /// <returns>collection of Model</returns>
        IEnumerable<T> Find(Expression<Func<T, bool>> filter, Expression<Func<T, object>> order, int pageIndex, int size, bool isDescending);

        /// <summary>
        /// find models async with paging and ordering in direction
        /// </summary>
        /// <param name="filter">expression filter</param>
        /// <param name="order">ordering parameters</param>
        /// <param name="pageIndex">page index, based on 0</param>
        /// <param name="size">number of items in page</param>
        /// <param name="isDescending">ordering direction</param>
        /// <returns>collection of Model</returns>
        Task<List<T>> FindAsync(Expression<Func<T, bool>> filter, Expression<Func<T, object>> order, int pageIndex, int size, bool isDescending);

        #endregion Find

        #region FindAll

        /// <summary>
        /// fetch all items in collection
        /// </summary>
        /// <returns>collection of Model</returns>
        IEnumerable<T> FindAll();

        /// <summary>
        /// fetch all items async in collection
        /// </summary>
        /// <returns>collection of Model</returns>
        Task<List<T>> FindAllAsync();

        /// <summary>
        /// fetch all items in collection with paging
        /// </summary>
        /// <param name="pageIndex">page index, based on 0</param>
        /// <param name="size">number of items in page</param>
        /// <returns>collection of Model</returns>
        IEnumerable<T> FindAll(int pageIndex, int size);

        /// <summary>
        /// fetch all items async in collection with paging
        /// </summary>
        /// <param name="pageIndex">page index, based on 0</param>
        /// <param name="size">number of items in page</param>
        /// <returns>collection of Model</returns>
        Task<List<T>> FindAllAsync(int pageIndex, int size);

        /// <summary>
        /// fetch all items in collection with paging and ordering
        /// default ordering is descending
        /// </summary>
        /// <param name="order">ordering parameters</param>
        /// <param name="pageIndex">page index, based on 0</param>
        /// <param name="size">number of items in page</param>
        /// <returns>collection of Model</returns>
        IEnumerable<T> FindAll(Expression<Func<T, object>> order, int pageIndex, int size);

        /// <summary>
        /// fetch all items async in collection with paging and ordering
        /// default ordering is descending
        /// </summary>
        /// <param name="order">ordering parameters</param>
        /// <param name="pageIndex">page index, based on 0</param>
        /// <param name="size">number of items in page</param>
        /// <returns>collection of Model</returns>
        Task<List<T>> FindAllAsync(Expression<Func<T, object>> order, int pageIndex, int size);

        /// <summary>
        /// fetch all items in collection with paging and ordering in direction
        /// </summary>
        /// <param name="order">ordering parameters</param>
        /// <param name="pageIndex">page index, based on 0</param>
        /// <param name="size">number of items in page</param>
        /// <param name="isDescending">ordering direction</param>
        /// <returns>collection of Model</returns>
        IEnumerable<T> FindAll(Expression<Func<T, object>> order, int pageIndex, int size, bool isDescending);

        /// <summary>
        /// fetch all items async in collection with paging and ordering in direction
        /// </summary>
        /// <param name="order">ordering parameters</param>
        /// <param name="pageIndex">page index, based on 0</param>
        /// <param name="size">number of items in page</param>
        /// <param name="isDescending">ordering direction</param>
        /// <returns>collection of Model</returns>
        Task<List<T>> FindAllAsync(Expression<Func<T, object>> order, int pageIndex, int size, bool isDescending);

        #endregion FindAll

        #region First

        /// <summary>
        /// get first item in collection
        /// </summary>
        /// <returns>Model of <typeparamref name="T"/></returns>
        T First();

        /// <summary>
        /// get first item async in collection
        /// </summary>
        /// <returns>Model of <typeparamref name="T"/></returns>
        Task<T> FirstAsync();

        /// <summary>
        /// get first item in query
        /// </summary>
        /// <param name="filter">expression filter</param>
        /// <returns>Model of <typeparamref name="T"/></returns>
        T First(Expression<Func<T, bool>> filter);

        /// <summary>
        /// get first item async in query
        /// </summary>
        /// <param name="filter">expression filter</param>
        /// <returns>Model of <typeparamref name="T"/></returns>
        Task<T> FirstAsync(Expression<Func<T, bool>> filter);

        /// <summary>
        /// get first item in query with order
        /// </summary>
        /// <param name="filter">expression filter</param>
        /// <param name="order">ordering parameters</param>
        /// <returns>Model of <typeparamref name="T"/></returns>
        T First(Expression<Func<T, bool>> filter, Expression<Func<T, object>> order);

        /// <summary>
        /// get first item async in query with order
        /// </summary>
        /// <param name="filter">expression filter</param>
        /// <param name="order">ordering parameters</param>
        /// <returns>Model of <typeparamref name="T"/></returns>
        Task<T> FirstAsync(Expression<Func<T, bool>> filter, Expression<Func<T, object>> order);

        /// <summary>
        /// get first item in query with order and direction
        /// </summary>
        /// <param name="filter">expression filter</param>
        /// <param name="order">ordering parameters</param>
        /// <param name="isDescending">ordering direction</param>
        /// <returns>Model of <typeparamref name="T"/></returns>
        T First(Expression<Func<T, bool>> filter, Expression<Func<T, object>> order, bool isDescending);

        /// <summary>
        /// get first item async in query with order and direction
        /// </summary>
        /// <param name="filter">expression filter</param>
        /// <param name="order">ordering parameters</param>
        /// <param name="isDescending">ordering direction</param>
        /// <returns>Model of <typeparamref name="T"/></returns>
        Task<T> FirstAsync(Expression<Func<T, bool>> filter, Expression<Func<T, object>> order, bool isDescending);

        #endregion First

        #region Get

        /// <summary>
        /// get by id
        /// </summary>
        /// <param name="id">id value</param>
        /// <returns>Model of <typeparamref name="T"/></returns>
        T Get(string id);

        /// <summary>
        /// get async by id
        /// </summary>
        /// <param name="id">id value</param>
        /// <returns>Model of <typeparamref name="T"/></returns>
        Task<T> GetAsync(string id);

        #endregion Get

        #region Insert

        /// <summary>
        /// insert Model
        /// </summary>
        /// <param name="Model">Model</param>
        void Insert(T Model);

        /// <summary>
        /// insert Model
        /// </summary>
        /// <param name="Model">Model</param>
        Task InsertAsync(T Model);

        /// <summary>
        /// insert Model collection
        /// </summary>
        /// <param name="models">collection of models</param>
        void Insert(IEnumerable<T> models);

        /// <summary>
        /// insert Model collection
        /// </summary>
        /// <param name="models">collection of models</param>
        Task InsertAsync(IEnumerable<T> models);

        #endregion Insert

        #region Last

        /// <summary>
        /// get last item in collection
        /// </summary>
        /// <returns>Model of <typeparamref name="T"/></returns>
        T Last();

        /// <summary>
        /// get last item async in collection
        /// </summary>
        /// <returns>Model of <typeparamref name="T"/></returns>
        Task<T> LastAsync();

        /// <summary>
        /// get last item in query
        /// </summary>
        /// <param name="filter">expression filter</param>
        /// <returns>Model of <typeparamref name="T"/></returns>
        T Last(Expression<Func<T, bool>> filter);

        /// <summary>
        /// get last item async in query
        /// </summary>
        /// <param name="filter">expression filter</param>
        /// <returns>Model of <typeparamref name="T"/></returns>
        Task<T> LastAsync(Expression<Func<T, bool>> filter);

        /// <summary>
        /// get last item in query with order
        /// </summary>
        /// <param name="filter">expression filter</param>
        /// <param name="order">ordering parameters</param>
        /// <returns>Model of <typeparamref name="T"/></returns>
        T Last(Expression<Func<T, bool>> filter, Expression<Func<T, object>> order);

        /// <summary>
        /// get last item async in query with order
        /// </summary>
        /// <param name="filter">expression filter</param>
        /// <param name="order">ordering parameters</param>
        /// <returns>Model of <typeparamref name="T"/></returns>
        Task<T> LastAsync(Expression<Func<T, bool>> filter, Expression<Func<T, object>> order);

        /// <summary>
        /// get last item in query with order and direction
        /// </summary>
        /// <param name="filter">expression filter</param>
        /// <param name="order">ordering parameters</param>
        /// <param name="isDescending">ordering direction</param>
        /// <returns>Model of <typeparamref name="T"/></returns>
        T Last(Expression<Func<T, bool>> filter, Expression<Func<T, object>> order, bool isDescending);

        /// <summary>
        /// get last item async in query with order and direction
        /// </summary>
        /// <param name="filter">expression filter</param>
        /// <param name="order">ordering parameters</param>
        /// <param name="isDescending">ordering direction</param>
        /// <returns>Model of <typeparamref name="T"/></returns>
        Task<T> LastAsync(Expression<Func<T, bool>> filter, Expression<Func<T, object>> order, bool isDescending);

        #endregion Last

        #region Replace

        /// <summary>
        /// replace an existing Model
        /// </summary>
        /// <param name="Model">Model</param>
        bool Replace(T Model);

        /// <summary>
        /// replace an existing Model
        /// </summary>
        /// <param name="Model">Model</param>
        Task<bool> ReplaceAsync(T Model);

        /// <summary>
        /// replace collection of models
        /// </summary>
        /// <param name="models">collection of models</param>
        void Replace(IEnumerable<T> models);

        /// <summary>
        /// replace collection of models async
        /// </summary>
        /// <param name="models">collection of models</param>
        Task ReplaceAsync(IEnumerable<T> models);

        #endregion Replace

        #region Update

        /// <summary>
        /// update an Model with updated fields
        /// </summary>
        /// <param name="id">id</param>
        /// <param name="updates">updated field(s)</param>
        /// <returns>true if successful, otherwise false</returns>
        bool Update(string id, params UpdateDefinition<T>[] updates);

        /// <summary>
        /// update an Model with updated fields
        /// </summary>
        /// <param name="Model">Model</param>
        /// <param name="updates">updated field(s)</param>
        /// <returns>true if successful, otherwise false</returns>
        bool Update(T Model, params UpdateDefinition<T>[] updates);

        /// <summary>
        /// update found models by filter with updated fields
        /// </summary>
        /// <param name="filter">collection filter</param>
        /// <param name="updates">updated field(s)</param>
        /// <returns>true if successful, otherwise false</returns>
        bool Update(FilterDefinition<T> filter, params UpdateDefinition<T>[] updates);

        /// <summary>
        /// update found models by filter with updated fields
        /// </summary>
        /// <param name="filter">collection filter</param>
        /// <param name="updates">updated field(s)</param>
        /// <returns>true if successful, otherwise false</returns>
        bool Update(Expression<Func<T, bool>> filter, params UpdateDefinition<T>[] updates);

        /// <summary>
        /// update a property field in an Model
        /// </summary>
        /// <typeparam name="TField">field type</typeparam>
        /// <param name="Model">Model</param>
        /// <param name="field">field</param>
        /// <param name="value">new value</param>
        /// <returns>true if successful, otherwise false</returns>
        bool Update<TField>(T Model, Expression<Func<T, TField>> field, TField value);

        /// <summary>
        /// update a property field in models
        /// </summary>
        /// <typeparam name="TField">field type</typeparam>
        /// <param name="filter">filter</param>
        /// <param name="field">field</param>
        /// <param name="value">new value</param>
        /// <returns>true if successful, otherwise false</returns>
        bool Update<TField>(FilterDefinition<T> filter, Expression<Func<T, TField>> field, TField value);

        /// <summary>
        /// update a property field in an Model
        /// </summary>
        /// <typeparam name="TField">field type</typeparam>
        /// <param name="Model">Model</param>
        /// <param name="field">field</param>
        /// <param name="value">new value</param>
        Task<bool> UpdateAsync<TField>(T Model, Expression<Func<T, TField>> field, TField value);

        /// <summary>
        /// update an Model with updated fields
        /// </summary>
        /// <param name="id">id</param>
        /// <param name="updates">updated field(s)</param>
        Task<bool> UpdateAsync(string id, params UpdateDefinition<T>[] updates);

        /// <summary>
        /// update an Model with updated fields
        /// </summary>
        /// <param name="Model">Model</param>
        /// <param name="updates">updated field(s)</param>
        Task<bool> UpdateAsync(T Model, params UpdateDefinition<T>[] updates);

        /// <summary>
        /// update a property field in models
        /// </summary>
        /// <typeparam name="TField">field type</typeparam>
        /// <param name="filter">filter</param>
        /// <param name="field">field</param>
        /// <param name="value">new value</param>
        Task<bool> UpdateAsync<TField>(FilterDefinition<T> filter, Expression<Func<T, TField>> field, TField value);

        /// <summary>
        /// update found models by filter with updated fields
        /// </summary>
        /// <param name="filter">collection filter</param>
        /// <param name="updates">updated field(s)</param>
        Task<bool> UpdateAsync(FilterDefinition<T> filter, params UpdateDefinition<T>[] updates);

        /// <summary>
        /// update found models by filter with updated fields
        /// </summary>
        /// <param name="filter">collection filter</param>
        /// <param name="updates">updated field(s)</param>
        Task<bool> UpdateAsync(Expression<Func<T, bool>> filter, params UpdateDefinition<T>[] updates);
        #endregion Update

        #endregion CRUD

        #region Utils
        /// <summary>
        /// validate if filter result exists
        /// </summary>
        /// <param name="filter">expression filter</param>
        /// <returns>true if exists, otherwise false</returns>
        bool Any(Expression<Func<T, bool>> filter);

        /// <summary>
        /// validate async if filter result exists
        /// </summary>
        /// <param name="filter">expression filter</param>
        /// <returns>true if exists, otherwise false</returns>
        Task<bool> AnyAsync(Expression<Func<T, bool>> filter);

        #region Count
        /// <summary>
        /// get number of filtered documents
        /// </summary>
        /// <param name="filter">expression filter</param>
        /// <returns>number of documents</returns>
        long Count(Expression<Func<T, bool>> filter);

        long EstimatedCount(EstimatedDocumentCountOptions options);

        /// <summary>
        /// get number of filtered documents
        /// </summary>
        /// <param name="filter">expression filter</param>
        /// <returns>number of documents</returns>
        Task<long> CountAsync(Expression<Func<T, bool>> filter);

        Task<long> EstimatedCountAsync(EstimatedDocumentCountOptions options);

        /// <summary>
        /// get number of documents in collection
        /// </summary>
        /// <returns>number of documents</returns>
        long Count();

        long EstimatedCount();

        /// <summary>
        /// get number of documents in collection
        /// </summary>
        /// <returns>number of documents</returns>
        Task<long> CountAsync();

        Task<long> EstimatedCountAsync();
        #endregion Count

        #endregion Utils
    }
}
