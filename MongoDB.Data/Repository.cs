using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using System.Net.Sockets;
using System.IO;
using Microsoft.Extensions.Configuration;
using MongoDB.Driver;
using Polly;

namespace MongoDB.Data
{
    public abstract class Repository<T> : IRepository<T> where T : IModel
    {
        #region MongoSpecific

        /// <summary>
        /// mongo collection
        /// </summary>
        public IMongoCollection<T> Collection
        {
            get; private set;
        }

        /// <summary>
        /// filter for collection
        /// </summary>
        public FilterDefinitionBuilder<T> Filter
        {
            get
            {
                return Builders<T>.Filter;
            }
        }

        /// <summary>
        /// filters for collection
        /// </summary>
        public List<FilterDefinition<T>> Filters
        {
            get
            {
                return new List<FilterDefinition<T>>();
            }
        }

        /// <summary>
        /// projector for collection
        /// </summary>
        public ProjectionDefinitionBuilder<T> Project
        {
            get
            {
                return Builders<T>.Projection;
            }
        }

        /// <summary>
        /// projectors for collection
        /// </summary>
        public List<ProjectionDefinition<T>> Projects
        {
            get
            {
                return new List<ProjectionDefinition<T>>();
            }
        }

        /// <summary>
        /// updater for collection
        /// </summary>
        public UpdateDefinitionBuilder<T> Updater
        {
            get
            {
                return Builders<T>.Update;
            }
        }

        /// <summary>
        /// updaters for collection
        /// </summary>
        public List<UpdateDefinition<T>> Updaters
        {
            get
            {
                return new List<UpdateDefinition<T>>();
            }
        }

        /// <summary>
        /// updater for collection
        /// </summary>
        public IndexKeysDefinitionBuilder<T> IndexKeys
        {
            get
            {
                return Builders<T>.IndexKeys;
            }
        }

        /// <summary>
        /// where you need to define a connectionString with the name of repository
        /// </summary>
        /// <param name="config">config interface to read default settings</param>
        public Repository(IConfiguration config)
        {
            Collection = DbContext<T>.GetCollection(config);
        }

        /// <summary>
        /// where collection name will be name of the repository
        /// </summary>
        /// <param name="connectionString">connection string</param>
        public Repository(string connectionString)
        {
            Collection = DbContext<T>.GetCollectionFromConnectionString(connectionString);
        }

        /// <summary>
        /// with custom settings
        /// </summary>
        /// <param name="connectionString">connection string</param>
        /// <param name="collectionName">collection name</param>
        public Repository(string connectionString, string collectionName)
        {
            Collection = DbContext<T>.GetCollectionFromConnectionString(connectionString, collectionName);
        }

        private IFindFluent<T, T> Query(Expression<Func<T, bool>> filter)
        {
            return Collection.Find(filter);
        }

        private IFindFluent<T, T> Query(FilterDefinition<T> filter)
        {
            return Collection.Find(filter);
        }

        private IFindFluent<T, T> Query()
        {
            return Collection.Find(Filter.Empty);
        }

        private IFindFluent<T, T> Query(Expression<Func<T, bool>> filter, Expression<Func<T, object>> order, int pageIndex, int size, bool isDescending)
        {
            var query = Query(filter).Skip(pageIndex * size).Limit(size);
            return (isDescending ? query.SortByDescending(order) : query.SortBy(order));
        }

        private IFindFluent<T, T> Query(FilterDefinition<T> filter, Expression<Func<T, object>> order, int pageIndex, int size, bool isDescending)
        {
            var query = Query(filter).Skip(pageIndex * size).Limit(size);
            return (isDescending ? query.SortByDescending(order) : query.SortBy(order));
        }

        private IFindFluent<T, T> Query(Expression<Func<T, object>> order, int pageIndex, int size, bool isDescending)
        {
            var query = Query().Skip(pageIndex * size).Limit(size);
            return (isDescending ? query.SortByDescending(order) : query.SortBy(order));
        }
        #endregion MongoSpecific

        #region CRUD

        #region Delete

        /// <summary>
        /// delete model
        /// </summary>
        /// <param name="model">model</param>
        public bool Delete(T model)
        {
            return Delete(model.Id);
        }

        /// <summary>
        /// delete model async
        /// </summary>
        /// <param name="model">model</param>
        public Task<bool> DeleteAsync(T model)
        {
            return DeleteAsync(model.Id);
        }

        /// <summary>
        /// delete by id
        /// </summary>
        /// <param name="id">id</param>
        public virtual bool Delete(string id)
        {
            return Retry(() =>
            {
                return Collection.DeleteOne(i => i.Id.Equals(id)).IsAcknowledged;
            });
        }

        /// <summary>
        /// delete by id
        /// </summary>
        /// <param name="id">id</param>
        public virtual Task<bool> DeleteAsync(string id)
        {
            return Retry(async () =>
            {
                var result = await Collection.DeleteOneAsync(i => i.Id.Equals(id));
                return result.IsAcknowledged;
            });
        }

        /// <summary>
        /// delete items with filter
        /// </summary>
        /// <param name="filter">expression filter</param>
        public bool Delete(Expression<Func<T, bool>> filter)
        {
            return Retry(() =>
            {
                return Collection.DeleteMany(filter).IsAcknowledged;
            });
        }

        /// <summary>
        /// delete items with filter
        /// </summary>
        /// <param name="filter">expression filter</param>
        public Task<bool> DeleteAsync(Expression<Func<T, bool>> filter)
        {
            return Retry(async () =>
            {
                var result = await Collection.DeleteManyAsync(filter);
                return result.IsAcknowledged;
            });
        }

        /// <summary>
        /// delete all documents
        /// </summary>
        public virtual bool DeleteAll()
        {
            return Retry(() =>
            {
                return Collection.DeleteMany(Filter.Empty).IsAcknowledged;
            });
        }

        /// <summary>
        /// delete all documents async
        /// </summary>
        public virtual Task<bool> DeleteAllAsync()
        {
            return Retry(async () =>
            {
                var result = await Collection.DeleteManyAsync(Filter.Empty);
                return result.IsAcknowledged;
            });
        }

        /// <summary>
        /// delete all documents with filter
        /// </summary>
        public virtual bool DeleteAll(Expression<Func<T, bool>> filter)
        {
            return Retry(() =>
            {
                return Collection.DeleteMany(filter).IsAcknowledged;
            });
        }

        /// <summary>
        /// delete all documents with filter
        /// </summary>
        public virtual Task<bool> DeleteAllAsync(Expression<Func<T, bool>> filter)
        {
            return Retry(async () =>
            {
                var result = await Collection.DeleteManyAsync(filter);
                return result.IsAcknowledged;
            });
        }
        #endregion Delete

        #region Find

        /// <summary>
        /// find models
        /// </summary>
        /// <param name="filter">expression filter</param>
        /// <returns>collection of model</returns>
        public virtual IEnumerable<T> Find(Expression<Func<T, bool>> filter)
        {
            return Query(filter).ToEnumerable();
        }

        public virtual IEnumerable<T> Find(FilterDefinition<T> filter)
        {
            return Query(filter).ToEnumerable();
        }

        /// <summary>
        /// find models async
        /// </summary>
        /// <param name="filter">expression filter</param>
        /// <returns>collection of model</returns>
        public virtual Task<List<T>> FindAsync(Expression<Func<T, bool>> filter)
        {
            return Retry(() =>
            {
                return Query(filter).ToListAsync();
            });
        }

        public virtual Task<List<T>> FindAsync(FilterDefinition<T> filter)
        {
            return Retry(() =>
            {
                return Query(filter).ToListAsync();
            });
        }

        /// <summary>
        /// find models with paging
        /// </summary>
        /// <param name="filter">expression filter</param>
        /// <param name="pageIndex">page index, based on 0</param>
        /// <param name="size">number of items in page</param>
        /// <returns>collection of model</returns>
        public IEnumerable<T> Find(Expression<Func<T, bool>> filter, int pageIndex, int size)
        {
            return Find(filter, i => i.Id, pageIndex, size);
        }

        public IEnumerable<T> Find(FilterDefinition<T> filter, int pageIndex, int size)
        {
            return Find(filter, i => i.Id, pageIndex, size);
        }

        /// <summary>
        /// find models async with paging
        /// </summary>
        /// <param name="filter">expression filter</param>
        /// <param name="pageIndex">page index, based on 0</param>
        /// <param name="size">number of items in page</param>
        /// <returns>collection of model</returns>
        public Task<List<T>> FindAsync(Expression<Func<T, bool>> filter, int pageIndex, int size)
        {
            return FindAsync(filter, i => i.Id, pageIndex, size);
        }

        public Task<List<T>> FindAsync(FilterDefinition<T> filter, int pageIndex, int size)
        {
            return FindAsync(filter, i => i.Id, pageIndex, size);
        }

        /// <summary>
        /// find models with paging and ordering
        /// default ordering is descending
        /// </summary>
        /// <param name="filter">expression filter</param>
        /// <param name="order">ordering parameters</param>
        /// <param name="pageIndex">page index, based on 0</param>
        /// <param name="size">number of items in page</param>
        /// <returns>collection of model</returns>
        public IEnumerable<T> Find(Expression<Func<T, bool>> filter, Expression<Func<T, object>> order, int pageIndex, int size)
        {
            return Find(filter, order, pageIndex, size, true);
        }

        public IEnumerable<T> Find(FilterDefinition<T> filter, Expression<Func<T, object>> order, int pageIndex, int size)
        {
            return Find(filter, order, pageIndex, size, true);
        }

        /// <summary>
        /// find models async with paging and ordering
        /// default ordering is descending
        /// </summary>
        /// <param name="filter">expression filter</param>
        /// <param name="order">ordering parameters</param>
        /// <param name="pageIndex">page index, based on 0</param>
        /// <param name="size">number of items in page</param>
        /// <returns>collection of model</returns>
        public Task<List<T>> FindAsync(Expression<Func<T, bool>> filter, Expression<Func<T, object>> order, int pageIndex, int size)
        {
            return FindAsync(filter, order, pageIndex, size, true);
        }

        public Task<List<T>> FindAsync(FilterDefinition<T> filter, Expression<Func<T, object>> order, int pageIndex, int size)
        {
            return FindAsync(filter, order, pageIndex, size, true);
        }

        /// <summary>
        /// find models with paging and ordering in direction
        /// </summary>
        /// <param name="filter">expression filter</param>
        /// <param name="order">ordering parameters</param>
        /// <param name="pageIndex">page index, based on 0</param>
        /// <param name="size">number of items in page</param>
        /// <param name="isDescending">ordering direction</param>
        /// <returns>collection of model</returns>
        public virtual IEnumerable<T> Find(Expression<Func<T, bool>> filter, Expression<Func<T, object>> order, int pageIndex, int size, bool isDescending)
        {
            return Retry(() =>
            {
                return Query(filter, order, pageIndex, size, isDescending).ToEnumerable();
            });
        }

        public virtual IEnumerable<T> Find(FilterDefinition<T> filter, Expression<Func<T, object>> order, int pageIndex, int size, bool isDescending)
        {
            return Retry(() =>
            {
                return Query(filter, order, pageIndex, size, isDescending).ToEnumerable();
            });
        }

        /// <summary>
        /// find models async with paging and ordering in direction
        /// </summary>
        /// <param name="filter">expression filter</param>
        /// <param name="order">ordering parameters</param>
        /// <param name="pageIndex">page index, based on 0</param>
        /// <param name="size">number of items in page</param>
        /// <param name="isDescending">ordering direction</param>
        /// <returns>collection of model</returns>
        public virtual Task<List<T>> FindAsync(FilterDefinition<T> filter, Expression<Func<T, object>> order, int pageIndex, int size, bool isDescending)
        {
            return Retry(() =>
            {
                return Query(filter, order, pageIndex, size, isDescending).ToListAsync();
            });
        }

        public virtual Task<List<T>> FindAsync(Expression<Func<T, bool>> filter, Expression<Func<T, object>> order, int pageIndex, int size, bool isDescending)
        {
            return Retry(() =>
            {
                return Query(filter, order, pageIndex, size, isDescending).ToListAsync();
            });
        }

        #endregion Find

        #region FindAll

        /// <summary>
        /// fetch all items in collection
        /// </summary>
        /// <returns>collection of model</returns>
        public virtual IEnumerable<T> FindAll()
        {
            return Retry(() =>
            {
                return Query().ToEnumerable();
            });
        }

        /// <summary>
        /// fetch all items async in collection
        /// </summary>
        /// <returns>collection of model</returns>
        public virtual Task<List<T>> FindAllAsync()
        {
            return Retry(async () =>
            {
                return await Query().ToListAsync();
            });
        }

        /// <summary>
        /// fetch all items in collection with paging
        /// </summary>
        /// <param name="pageIndex">page index, based on 0</param>
        /// <param name="size">number of items in page</param>
        /// <returns>collection of model</returns>
        public IEnumerable<T> FindAll(int pageIndex, int size)
        {
            return FindAll(i => i.Id, pageIndex, size);
        }

        /// <summary>
        /// fetch all items async in collection with paging
        /// </summary>
        /// <param name="pageIndex">page index, based on 0</param>
        /// <param name="size">number of items in page</param>
        /// <returns>collection of model</returns>
        public Task<List<T>> FindAllAsync(int pageIndex, int size)
        {
            return FindAllAsync(i => i.Id, pageIndex, size);
        }

        /// <summary>
        /// fetch all items in collection with paging and ordering
        /// default ordering is descending
        /// </summary>
        /// <param name="order">ordering parameters</param>
        /// <param name="pageIndex">page index, based on 0</param>
        /// <param name="size">number of items in page</param>
        /// <returns>collection of model</returns>
        public IEnumerable<T> FindAll(Expression<Func<T, object>> order, int pageIndex, int size)
        {
            return FindAll(order, pageIndex, size, true);
        }

        /// <summary>
        /// fetch all items async in collection with paging and ordering
        /// default ordering is descending
        /// </summary>
        /// <param name="order">ordering parameters</param>
        /// <param name="pageIndex">page index, based on 0</param>
        /// <param name="size">number of items in page</param>
        /// <returns>collection of model</returns>
        public Task<List<T>> FindAllAsync(Expression<Func<T, object>> order, int pageIndex, int size)
        {
            return FindAllAsync(order, pageIndex, size, true);
        }

        /// <summary>
        /// fetch all items in collection with paging and ordering in direction
        /// </summary>
        /// <param name="order">ordering parameters</param>
        /// <param name="pageIndex">page index, based on 0</param>
        /// <param name="size">number of items in page</param>
        /// <param name="isDescending">ordering direction</param>
        /// <returns>collection of model</returns>
        public virtual IEnumerable<T> FindAll(Expression<Func<T, object>> order, int pageIndex, int size, bool isDescending)
        {
            return Retry(() =>
            {
                return Query(order, pageIndex, size, isDescending).ToEnumerable();
            });
        }

        /// <summary>
        /// fetch all items async in collection with paging and ordering in direction
        /// </summary>
        /// <param name="order">ordering parameters</param>
        /// <param name="pageIndex">page index, based on 0</param>
        /// <param name="size">number of items in page</param>
        /// <param name="isDescending">ordering direction</param>
        /// <returns>collection of model</returns>
        public virtual Task<List<T>> FindAllAsync(Expression<Func<T, object>> order, int pageIndex, int size, bool isDescending)
        {
            return Retry(async () =>
            {
                return await Query(order, pageIndex, size, isDescending).ToListAsync();
            });
        }

        #endregion FindAll

        #region First

        /// <summary>
        /// get first item in collection
        /// </summary>
        /// <returns>model of <typeparamref name="T"/></returns>
        public T First()
        {
            return Retry(() =>
            {
                return Query(i => i.Id, 0, 1, false).FirstOrDefault();
            });
        }

        /// <summary>
        /// get first item async in collection
        /// </summary>
        /// <returns>model of <typeparamref name="T"/></returns>
        public Task<T> FirstAsync()
        {
            return Retry(() =>
            {
                return Query(i => i.Id, 0, 1, false).FirstOrDefaultAsync();
            });
        }

        /// <summary>
        /// get first item in query
        /// </summary>
        /// <param name="filter">expression filter</param>
        /// <returns>model of <typeparamref name="T"/></returns>
        public T First(Expression<Func<T, bool>> filter)
        {
            return First(filter, i => i.Id);
        }

        /// <summary>
        /// get first item async in query
        /// </summary>
        /// <param name="filter">expression filter</param>
        /// <returns>model of <typeparamref name="T"/></returns>
        public Task<T> FirstAsync(Expression<Func<T, bool>> filter)
        {
            return FirstAsync(filter, i => i.Id);
        }

        /// <summary>
        /// get first item in query with order
        /// </summary>
        /// <param name="filter">expression filter</param>
        /// <param name="order">ordering parameters</param>
        /// <returns>model of <typeparamref name="T"/></returns>
        public T First(Expression<Func<T, bool>> filter, Expression<Func<T, object>> order)
        {
            return First(filter, order, false);
        }

        /// <summary>
        /// get first item async in query with order
        /// </summary>
        /// <param name="filter">expression filter</param>
        /// <param name="order">ordering parameters</param>
        /// <returns>model of <typeparamref name="T"/></returns>
        public Task<T> FirstAsync(Expression<Func<T, bool>> filter, Expression<Func<T, object>> order)
        {
            return FirstAsync(filter, order, false);
        }

        /// <summary>
        /// get first item in query with order and direction
        /// </summary>
        /// <param name="filter">expression filter</param>
        /// <param name="order">ordering parameters</param>
        /// <param name="isDescending">ordering direction</param>
        /// <returns>model of <typeparamref name="T"/></returns>
        public T First(Expression<Func<T, bool>> filter, Expression<Func<T, object>> order, bool isDescending)
        {
            return Retry(() =>
            {
                return Query(filter, order, 0, 1, isDescending).FirstOrDefault();
            });
        }

        /// <summary>
        /// get first item async in query with order and direction
        /// </summary>
        /// <param name="filter">expression filter</param>
        /// <param name="order">ordering parameters</param>
        /// <param name="isDescending">ordering direction</param>
        /// <returns>model of <typeparamref name="T"/></returns>
        public Task<T> FirstAsync(Expression<Func<T, bool>> filter, Expression<Func<T, object>> order, bool isDescending)
        {
            return Retry(() =>
            {
                return Query(filter, order, 0, 1, isDescending).FirstOrDefaultAsync();
            });
        }

        #endregion First

        #region Get

        /// <summary>
        /// get by id
        /// </summary>
        /// <param name="id">id value</param>
        /// <returns>model of <typeparamref name="T"/></returns>
        public virtual T Get(string id)
        {
            return Retry(() =>
            {
                return Query(i => i.Id.Equals(id)).FirstOrDefault();
            });
        }

        /// <summary>
        /// get async by id
        /// </summary>
        /// <param name="id">id value</param>
        /// <returns>model of <typeparamref name="T"/></returns>
        public virtual Task<T> GetAsync(string id)
        {
            return Retry(() =>
            {
                return Query(i => i.Id.Equals(id)).FirstOrDefaultAsync();
            });
        }

        #endregion Get

        #region Insert

        /// <summary>
        /// insert model
        /// </summary>
        /// <param name="model">model</param>
        public virtual void Insert(T model)
        {
            Retry(() =>
            {
                Collection.InsertOne(model);
                return true;
            });
        }

        /// <summary>
        /// insert model
        /// </summary>
        /// <param name="model">model</param>
        public virtual Task InsertAsync(T model)
        {
            return Retry(() =>
            {
                return Collection.InsertOneAsync(model);
            });
        }

        /// <summary>
        /// insert model collection
        /// </summary>
        /// <param name="models">collection of models</param>
        public virtual void Insert(IEnumerable<T> models)
        {
            Retry(() =>
            {
                Collection.InsertMany(models);
                return true;
            });
        }

        /// <summary>
        /// insert model collection
        /// </summary>
        /// <param name="models">collection of models</param>
        public virtual Task InsertAsync(IEnumerable<T> models)
        {
            return Retry(() =>
            {
                return Collection.InsertManyAsync(models);
            });
        }
        #endregion Insert

        #region Last

        /// <summary>
        /// get first item in collection
        /// </summary>
        /// <returns>model of <typeparamref name="T"/></returns>
        public T Last()
        {
            return Query(i => i.Id, 0, 1, true).FirstOrDefault();
        }

        /// <summary>
        /// get first item async in collection
        /// </summary>
        /// <returns>model of <typeparamref name="T"/></returns>
        public Task<T> LastAsync()
        {
            return Query(i => i.Id, 0, 1, true).FirstOrDefaultAsync();
        }

        /// <summary>
        /// get last item in query
        /// </summary>
        /// <param name="filter">expression filter</param>
        /// <returns>model of <typeparamref name="T"/></returns>
        public T Last(Expression<Func<T, bool>> filter)
        {
            return Last(filter, i => i.Id);
        }

        /// <summary>
        /// get last item async in query
        /// </summary>
        /// <param name="filter">expression filter</param>
        /// <returns>model of <typeparamref name="T"/></returns>
        public Task<T> LastAsync(Expression<Func<T, bool>> filter)
        {
            return LastAsync(filter, i => i.Id);
        }

        /// <summary>
        /// get last item in query with order
        /// </summary>
        /// <param name="filter">expression filter</param>
        /// <param name="order">ordering parameters</param>
        /// <returns>model of <typeparamref name="T"/></returns>
        public T Last(Expression<Func<T, bool>> filter, Expression<Func<T, object>> order)
        {
            return Last(filter, order, false);
        }

        /// <summary>
        /// get last item async in query with order
        /// </summary>
        /// <param name="filter">expression filter</param>
        /// <param name="order">ordering parameters</param>
        /// <returns>model of <typeparamref name="T"/></returns>
        public Task<T> LastAsync(Expression<Func<T, bool>> filter, Expression<Func<T, object>> order)
        {
            return LastAsync(filter, order, false);
        }

        /// <summary>
        /// get last item in query with order and direction
        /// </summary>
        /// <param name="filter">expression filter</param>
        /// <param name="order">ordering parameters</param>
        /// <param name="isDescending">ordering direction</param>
        /// <returns>model of <typeparamref name="T"/></returns>
        public T Last(Expression<Func<T, bool>> filter, Expression<Func<T, object>> order, bool isDescending)
        {
            return First(filter, order, !isDescending);
        }

        /// <summary>
        /// get last item in query with order and direction
        /// </summary>
        /// <param name="filter">expression filter</param>
        /// <param name="order">ordering parameters</param>
        /// <param name="isDescending">ordering direction</param>
        /// <returns>model of <typeparamref name="T"/></returns>
        public Task<T> LastAsync(Expression<Func<T, bool>> filter, Expression<Func<T, object>> order, bool isDescending)
        {
            return FirstAsync(filter, order, !isDescending);
        }

        #endregion Last

        #region Replace

        /// <summary>
        /// replace an existing model
        /// </summary>
        /// <param name="model">model</param>
        public virtual bool Replace(T model)
        {
            return Retry(() =>
            {
                return Collection.ReplaceOne(i => i.Id.Equals(model.Id), model).IsAcknowledged;
            });
        }

        /// <summary>
        /// replace an existing model
        /// </summary>
        /// <param name="model">model</param>
        public virtual Task<bool> ReplaceAsync(T model)
        {
            return Retry(async () =>
            {
                var result = await Collection.ReplaceOneAsync(i => i.Id.Equals(model.Id), model);
                return result.IsAcknowledged;
            });
        }

        /// <summary>
        /// replace collection of models
        /// </summary>
        /// <param name="models">collection of models</param>
        public void Replace(IEnumerable<T> models)
        {
            foreach (T model in models)
            {
                Replace(model);
            }
        }

        /// <summary>
        /// replace collection of models
        /// </summary>
        /// <param name="models">collection of models</param>
        public async Task ReplaceAsync(IEnumerable<T> models)
        {
            foreach (T model in models)
            {
                await ReplaceAsync(model);
            }
        }

        #endregion Replace

        #region Update

        /// <summary>
        /// update a property field in an model
        /// </summary>
        /// <typeparam name="TField">field type</typeparam>
        /// <param name="model">model</param>
        /// <param name="field">field</param>
        /// <param name="value">new value</param>
        /// <returns>true if successful, otherwise false</returns>
        public bool Update<TField>(T model, Expression<Func<T, TField>> field, TField value)
        {
            return Update(model, Updater.Set(field, value));
        }

        /// <summary>
        /// update a property field in an model
        /// </summary>
        /// <typeparam name="TField">field type</typeparam>
        /// <param name="model">model</param>
        /// <param name="field">field</param>
        /// <param name="value">new value</param>
        public Task<bool> UpdateAsync<TField>(T model, Expression<Func<T, TField>> field, TField value)
        {
            return UpdateAsync(model, Updater.Set(field, value));
        }

        /// <summary>
        /// update an model with updated fields
        /// </summary>
        /// <param name="id">id</param>
        /// <param name="updates">updated field(s)</param>
        /// <returns>true if successful, otherwise false</returns>
        public virtual bool Update(string id, params UpdateDefinition<T>[] updates)
        {
            return Update(Filter.Eq(i => i.Id, id), updates);
        }

        /// <summary>
        /// update an model with updated fields
        /// </summary>
        /// <param name="id">id</param>
        /// <param name="updates">updated field(s)</param>
        public virtual Task<bool> UpdateAsync(string id, params UpdateDefinition<T>[] updates)
        {
            return UpdateAsync(Filter.Eq(i => i.Id, id), updates);
        }

        /// <summary>
        /// update an model with updated fields
        /// </summary>
        /// <param name="model">model</param>
        /// <param name="updates">updated field(s)</param>
        /// <returns>true if successful, otherwise false</returns>
        public virtual bool Update(T model, params UpdateDefinition<T>[] updates)
        {
            return Update(model.Id, updates);
        }

        /// <summary>
        /// update an model with updated fields
        /// </summary>
        /// <param name="model">model</param>
        /// <param name="updates">updated field(s)</param>
        public virtual Task<bool> UpdateAsync(T model, params UpdateDefinition<T>[] updates)
        {
            return UpdateAsync(model.Id, updates);
        }

        /// <summary>
        /// update a property field in models
        /// </summary>
        /// <typeparam name="TField">field type</typeparam>
        /// <param name="filter">filter</param>
        /// <param name="field">field</param>
        /// <param name="value">new value</param>
        /// <returns>true if successful, otherwise false</returns>
        public bool Update<TField>(FilterDefinition<T> filter, Expression<Func<T, TField>> field, TField value)
        {
            return Update(filter, Updater.Set(field, value));
        }

        /// <summary>
        /// update a property field in models
        /// </summary>
        /// <typeparam name="TField">field type</typeparam>
        /// <param name="filter">filter</param>
        /// <param name="field">field</param>
        /// <param name="value">new value</param>
        public Task<bool> UpdateAsync<TField>(FilterDefinition<T> filter, Expression<Func<T, TField>> field, TField value)
        {
            return UpdateAsync(filter, Updater.Set(field, value));
        }

        /// <summary>
        /// update found models by filter with updated fields
        /// </summary>
        /// <param name="filter">collection filter</param>
        /// <param name="updates">updated field(s)</param>
        /// <returns>true if successful, otherwise false</returns>
        public bool Update(FilterDefinition<T> filter, params UpdateDefinition<T>[] updates)
        {
            return Retry(() =>
            {
                var update = Updater.Combine(updates).CurrentDate(i => i.ModifiedAt);
                return Collection.UpdateMany(filter, update.CurrentDate(i => i.ModifiedAt)).IsAcknowledged;
            });
        }

        /// <summary>
        /// update found models by filter with updated fields
        /// </summary>
        /// <param name="filter">collection filter</param>
        /// <param name="updates">updated field(s)</param>
        public Task<bool> UpdateAsync(FilterDefinition<T> filter, params UpdateDefinition<T>[] updates)
        {
            return Retry(async () =>
            {
                var update = Updater.Combine(updates).CurrentDate(i => i.ModifiedAt);
                var result = await Collection.UpdateManyAsync(filter, update.CurrentDate(i => i.ModifiedAt));
                return result.IsAcknowledged;
            });
        }

        /// <summary>
        /// update found models by filter with updated fields
        /// </summary>
        /// <param name="filter">collection filter</param>
        /// <param name="updates">updated field(s)</param>
        /// <returns>true if successful, otherwise false</returns>
        public bool Update(Expression<Func<T, bool>> filter, params UpdateDefinition<T>[] updates)
        {
            return Retry(() =>
            {
                var update = Updater.Combine(updates).CurrentDate(i => i.ModifiedAt);
                return Collection.UpdateMany(filter, update).IsAcknowledged;
            });
        }

        /// <summary>
        /// update found models by filter with updated fields
        /// </summary>
        /// <param name="filter">collection filter</param>
        /// <param name="updates">updated field(s)</param>
        public Task<bool> UpdateAsync(Expression<Func<T, bool>> filter, params UpdateDefinition<T>[] updates)
        {
            return Retry(async () =>
            {
                var update = Updater.Combine(updates).CurrentDate(i => i.ModifiedAt);
                var result = await Collection.UpdateManyAsync(filter, update);
                return result.IsAcknowledged;
            });
        }

        #endregion Update

        #endregion CRUD

        #region Utils

        /// <summary>
        /// validate if filter result exists
        /// </summary>
        /// <param name="filter">expression filter</param>
        /// <returns>true if exists, otherwise false</returns>
        public bool Any(Expression<Func<T, bool>> filter)
        {
            return Retry(() =>
            {
                return Count(filter) > 0;
            });
        }

        /// <summary>
        /// validate async if filter result exists
        /// </summary>
        /// <param name="filter">expression filter</param>
        /// <returns>true if exists, otherwise false</returns>
        public Task<bool> AnyAsync(Expression<Func<T, bool>> filter)
        {
            return Retry(async () =>
            {
                return await CountAsync(filter) > 0;
            });
        }

        #region Count
        /// <summary>
        /// get number of filtered documents
        /// </summary>
        /// <param name="filter">expression filter</param>
        /// <returns>number of documents</returns>
        public long Count(Expression<Func<T, bool>> filter)
        {
            return Retry(() =>
            {
                return Collection.CountDocuments(filter);
            });
        }

        public long EstimatedCount(EstimatedDocumentCountOptions options)
        {
            return Retry(() =>
            {
                return Collection.EstimatedDocumentCount(options);
            });
        }

        /// <summary>
        /// get number of filtered documents
        /// </summary>
        /// <param name="filter">expression filter</param>
        /// <returns>number of documents</returns>
        public Task<long> CountAsync(Expression<Func<T, bool>> filter)
        {
            return Retry(() =>
            {
                return Collection.CountDocumentsAsync(filter);
            });
        }

        public Task<long> EstimatedCountAsync(EstimatedDocumentCountOptions options)
        {
            return Retry(() =>
            {
                return Collection.EstimatedDocumentCountAsync(options);
            });
        }

        /// <summary>
        /// get number of documents in collection
        /// </summary>
        /// <returns>number of documents</returns>
        public long Count()
        {
            return Retry(() =>
            {
                return Collection.CountDocuments(Filter.Empty);
            });
        }

        public long EstimatedCount()
        {
            return Retry(() =>
            {
                return Collection.EstimatedDocumentCount();
            });
        }

        /// <summary>
        /// get number of documents in collection
        /// </summary>
        /// <returns>number of documents</returns>
        public Task<long> CountAsync()
        {
            return Retry(() =>
            {
                return Collection.CountDocumentsAsync(Filter.Empty);
            });
        }

        public Task<long> EstimatedCountAsync()
        {
            return Retry(() =>
            {
                return Collection.EstimatedDocumentCountAsync();
            });
        }
        #endregion Count

        #region Indexing
        public string CreateIndex(IndexKeysDefinition<T> keys)
        {
            return Retry(() =>
            {
                return Collection.Indexes.CreateOne(new CreateIndexModel<T>(keys));
            });
        }

        public Task<string> CreateIndexAsync(IndexKeysDefinition<T> keys)
        {
            return Retry(() =>
            {
                return Collection.Indexes.CreateOneAsync(new CreateIndexModel<T>(keys));
            });
        }

        public IEnumerable<string> CreateIndexMany(params IndexKeysDefinition<T>[] keys)
        {
            return Retry(() =>
            {
                var models = new List<CreateIndexModel<T>>();
                foreach(var item in keys)
                    models.Add(new CreateIndexModel<T>(item));

                return Collection.Indexes.CreateMany(models);
            });
        }

        public Task<IEnumerable<string>> CreateIndexManyAsync(params IndexKeysDefinition<T>[] keys)
        {
            return Retry(() =>
            {
                var models = new List<CreateIndexModel<T>>();
                foreach (var item in keys)
                    models.Add(new CreateIndexModel<T>(item));

                return Collection.Indexes.CreateManyAsync(models);
            });
        }

        public void DropIndex(string name)
        {
            Retry(() =>
            {
                Collection.Indexes.DropOne(name);

                return true;
            });
        }

        public Task DropIndexAsync(string name)
        {
            return Retry(() =>
            {
                return Collection.Indexes.DropOneAsync(name);
            });
        }

        public void DropIndexAll()
        {
            Retry(() =>
            {
                Collection.Indexes.DropAll();

                return true;
            });
        }

        public Task DropIndexAllAsync()
        {
            return Retry(() =>
            {
                return Collection.Indexes.DropAllAsync();
            });
        }
        #endregion Indexing

        #endregion Utils

        #region RetryPolicy
        /// <summary>
        /// retry operation for three times if IOException occurs
        /// </summary>
        /// <typeparam name="TResult">return type</typeparam>
        /// <param name="action">action</param>
        /// <returns>action result</returns>
        /// <example>
        /// return Retry(() => 
        /// { 
        ///     do_something;
        ///     return something;
        /// });
        /// </example>
        protected virtual TResult Retry<TResult>(Func<TResult> action)
        {
            return Policy
                .Handle<MongoConnectionException>(i => i.InnerException.GetType() == typeof(IOException) ||
                                                       i.InnerException.GetType() == typeof(SocketException))
                .Retry(3)
                .Execute(action);
        }
        #endregion
    }
}
