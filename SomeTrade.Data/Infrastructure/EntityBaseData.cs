namespace SomeTrade.Data.Infrastructure
{
    using Infrastructure;
    using Infrastructure.Entities;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.Extensions.DependencyInjection;
    using SomeTrade.Model.Core;
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.Linq;
    using System.Linq.Expressions;

    public class EntityBaseData<T> : IData<T> where T : ModelBase
    {
        protected DbContext _context;
        SomeTrade.Infrastructure.Interfaces.ILogger<object> logger;
        private readonly IServiceScopeFactory _serviceScopeFactory;

        public EntityBaseData(SomeTrade.Infrastructure.Interfaces.ILogger<object> logger
            , IServiceScopeFactory serviceScopeFactory)
        {
            this.logger = logger;
            _serviceScopeFactory = serviceScopeFactory;
        }

        protected virtual void BeforeUpdate() { }
        protected virtual void AfterUpdate() { }
        protected virtual void BeforeInsert(T t) { }
        protected virtual void AfterInsert() { }
        protected virtual void BeforeDelete() { }
        protected virtual void AfterDelete() { }

        public static string InsertSucceedLogTemplate = "{0} Insert İşlemi Sorunsuz Tamamlandı";
        public static string InsertFailedLogTemplate = "{0} Insert İşlemi Sırasında Hata Oldu {1}";
        public static string UpdateSucceedLogTemplate = "{0} Update İşlemi Sorunsuz Tamamlandı";
        public static string UpdateFailedLogTemplate = "{0} Update İşlemi Sırasında Hata Oldu {1}";
        public static string UpdateNotFoundLogTemplate = "{0} Update İşlemi Sırasında Kayıt Bulunamadı";
        public static string UpdateBulkSucceedLogTemplate = "{0} Update Bulk İşlemi Sorunsuz Tamamlandı, {1} Kayıt Guncellendi";
        public static string UpdateBulkFailedLogTemplate = "{0} Update Bulk İşlemi Sırasında Hata Oldu {1}";
        public static string InsertBulkSucceedLogTemplate = "{0} Insert Bulk İşlemi Tamamlandı {1}";
        public static string InsertBulkFailedLogTemplate = "{0} Insert Bulk İşlemi Sırasında Hata Oldu {1}";
        public static string DeleteBulkSucceedLogTemplate = "{0} Delete Bulk İşlemi Sorunsuz Tamamlandı, {1} Kayıt Silindi";
        public static string DeleteSucceedLogTemplate = "{0} Delete İşlemi Sorunsuz Tamamlandı";
        public static string DeleteFailedLogTemplate = "{0} Delete İşlemi Sırasında Hata Oldu {1}";
        public static string DetachedAllLogTemplate = "Detach İşlemi Yapıldı...";

        #region IData Implementation

        public void DetachAllEntities()
        {
            using (IServiceScope scope = _serviceScopeFactory.CreateScope())
            {
                _context = scope.ServiceProvider.GetService<DataContext>();

                var entries = _context.ChangeTracker.Entries()
                .Where(e => e.State != EntityState.Detached)
                .ToList();

                foreach (var entry in entries)
                {
                    if (entry.Entity != null)
                    {
                        entry.State = EntityState.Detached;
                    }
                }

                logger.Verbose(DetachedAllLogTemplate);
            }
        }

        /// <summary>
        /// Insert new Entity
        /// </summary>
        /// <param name="t"></param>
        /// <returns></returns>
        public DataResult Insert(T t)
        {
            using (IServiceScope scope = _serviceScopeFactory.CreateScope())
            {
                _context = scope.ServiceProvider.GetService<DataContext>();

                try
                {
                    BeforeInsert(t);
                    _context.Set<T>().Add(t);
                    AfterInsert();

                    _context.SaveChanges();

                    logger.Verbose(string.Format(InsertSucceedLogTemplate, typeof(T)));

                    return new DataResult(true, "");
                }
                catch (Exception exc)
                {
                    logger.Error(string.Format(InsertFailedLogTemplate, typeof(T), exc.Message));

                    return new DataResult(false, exc.Message +
                        exc.InnerException == null ? "" : "(" + exc.InnerException + ")"
                    );
                }
            }
        }

        public DataResult InsertBulk(List<T> ts, bool validateAndIgnoreBefore = false)
        {
            using (IServiceScope scope = _serviceScopeFactory.CreateScope())
            {
                if (ts.Count == 0)
                    return new DataResult(true, "");

                _context = scope.ServiceProvider.GetService<DataContext>();

                try
                {
                    foreach (var item in ts)
                    {
                        if (validateAndIgnoreBefore && typeof(IValidatableObject).IsAssignableFrom(item.GetType()))
                        {
                            var results = new List<ValidationResult>();
                            bool isValid = Validator.TryValidateObject(item, new ValidationContext(item, null, null), results, true);

                            if (!isValid)
                            {
                                var resultFirst = results[0];

                                logger.Warning($"{typeof(T)} Valid olmadıgı için atladım:{resultFirst.MemberNames.ToList()[0]} / {resultFirst.ErrorMessage}");
                                continue;
                            }
                        }

                        _context.Set<T>().Add(item);
                    }

                    _context.SaveChanges();

                    logger.Verbose(string.Format(InsertBulkSucceedLogTemplate, typeof(T), ts.Count));

                    return new DataResult(true, "");
                }
                catch (Exception exc)
                {
                    logger.Error(string.Format(InsertBulkFailedLogTemplate, typeof(T), exc.Message));

                    return new DataResult(false, exc.Message);
                }
            }
        }

        /// <summary>
        /// Update Entity
        /// </summary>
        /// <param name="t"></param>
        /// <returns></returns>
        public DataResult Update(T t)
        {
            using (IServiceScope scope = _serviceScopeFactory.CreateScope())
            {
                try
                {
                    _context = scope.ServiceProvider.GetService<DataContext>();

                    int updateId = t.Id;

                    T aModel = _context.Set<T>().Where(x => x.Id == updateId).FirstOrDefault();

                    if (aModel == null)
                    {
                        logger.Verbose(string.Format(UpdateNotFoundLogTemplate, typeof(T)));
                        return new DataResult(false, "Güncelleme yapılacak kayıt bulunamıyor");
                    }

                    BeforeUpdate();

                    foreach (var propertyInfo in typeof(T).GetProperties())
                    {
                        var hasIgnore = Attribute.IsDefined(propertyInfo, typeof(IgnoredAttribute));
                        if (hasIgnore)
                            continue;

                        propertyInfo.SetValue(aModel, propertyInfo.GetValue(t, null), null);
                    }

                    AfterUpdate();

                    _context.SaveChanges();

                    logger.Verbose(string.Format(UpdateSucceedLogTemplate, typeof(T)));
                    return new DataResult(true, "");
                }
                catch (Exception exc)
                {
                    logger.Error(string.Format(UpdateFailedLogTemplate, typeof(T), exc.Message));

                    return new DataResult(false, exc.Message +
                        exc.InnerException == null ? "" : "(" + exc.InnerException + ")"
                    );
                }
            }
        }

        public DataResult UpdateBulk(List<T> ts, List<string> properties, List<object> values)
        {
            using (IServiceScope scope = _serviceScopeFactory.CreateScope())
            {
                _context = scope.ServiceProvider.GetService<DataContext>();

                var typeOfT = typeof(T);
                foreach (var item in ts)
                {
                    T aModel = _context.Set<T>().Where(x => x.Id == item.Id).FirstOrDefault();
                    if (aModel == null)
                        continue;

                    foreach (var prName in properties)
                    {
                        for (int i = 0; i < properties.Count; i++)
                            typeOfT.GetProperty(properties[i]).SetValue(item, values[i], null);
                    }
                }

                _context.SaveChanges();

                logger.Verbose(string.Format(UpdateBulkSucceedLogTemplate, typeof(T), ts.Count));

                return new DataResult(true, "");
            }
        }

        public DataResult UpdateBulk(Expression<Func<T, bool>> predicate, List<string> properties, List<object> values)
        {
            using (IServiceScope scope = _serviceScopeFactory.CreateScope())
            {
                _context = scope.ServiceProvider.GetService<DataContext>();

                var list = _context.Set<T>().Where(predicate);
                var typeOfT = typeof(T);
                foreach (var item in list)
                {
                    foreach (var prName in properties)
                    {
                        for (int i = 0; i < properties.Count; i++)
                            typeOfT.GetProperty(properties[i]).SetValue(item, values[i], null);
                    }
                }

                _context.SaveChanges();

                logger.Verbose(string.Format(UpdateBulkSucceedLogTemplate, typeof(T), list.Count()));

                return new DataResult(true, "");
            }
        }

        public DataResult UpdateBulk(Expression<Func<T, bool>> predicate, int pageNumber, int pageCount, List<string> properties, List<object> values)
        {
            using (IServiceScope scope = _serviceScopeFactory.CreateScope())
            {
                _context = scope.ServiceProvider.GetService<DataContext>();

                var list = _context.Set<T>().OrderBy("Id").Where(predicate).Skip((pageNumber - 1) * pageCount).Take(pageCount);
                var typeOfT = typeof(T);
                foreach (var item in list)
                {
                    for (int i = 0; i < properties.Count; i++)
                        typeOfT.GetProperty(properties[i]).SetValue(item, values[i], null);
                }

                _context.SaveChanges();

                logger.Verbose(string.Format(UpdateBulkSucceedLogTemplate, typeof(T), list.Count()));

                return new DataResult(true, "");
            }
        }

        public DataResult UpdateBulk(List<T> ts, List<string> properties, List<List<object>> valueSets)
        {
            using (IServiceScope scope = _serviceScopeFactory.CreateScope())
            {
                try
                {
                    _context = scope.ServiceProvider.GetService<DataContext>();

                    var typeOfT = typeof(T);
                    for (int j = 0; j < ts.Count; j++)
                    {
                        var item = ts[j];
                        var values = valueSets[j];

                        T aModel = _context.Set<T>().Where(x => x.Id == item.Id).FirstOrDefault();
                        if (aModel == null)
                            continue;

                        for (int i = 0; i < properties.Count; i++)
                        {
                            typeOfT.GetProperty(properties[i]).SetValue(item, values[i], null);
                        }
                    }

                    _context.SaveChanges();

                    logger.Verbose(string.Format(UpdateBulkSucceedLogTemplate, typeof(T), ts.Count));

                    return new DataResult(true, "");
                }
                catch (Exception exc)
                {
                    logger.Error(string.Format(UpdateBulkFailedLogTemplate, typeof(T), exc.Message));

                    return new DataResult(false, exc.Message +
                        exc.InnerException == null ? "" : "(" + exc.InnerException + ")"
                    );
                }
            }
        }

        /// <summary>
        /// Delete Entity
        /// </summary>
        /// <param name="t"></param>
        /// <returns></returns>
        public DataResult Delete(T t)
        {
            return DeleteByKey(t.Id);
        }

        /// <summary>
        /// Delete Entity by Key
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public DataResult DeleteByKey(int id)
        {
            using (IServiceScope scope = _serviceScopeFactory.CreateScope())
            {
                _context = scope.ServiceProvider.GetService<DataContext>();

                try
                {
                    T aModel = _context.Set<T>().Where(x => x.Id == id).FirstOrDefault();

                    if (aModel == null)
                        return new DataResult(false, "Silinecek kayıt bulunamıyor");

                    BeforeDelete();
                    _context.Set<T>().Remove(aModel);
                    AfterDelete();
                    _context.SaveChanges();

                    logger.Verbose(string.Format(DeleteSucceedLogTemplate, typeof(T)));

                    return new DataResult(true, "");
                }
                catch (Exception exc)
                {
                    logger.Error(string.Format(DeleteFailedLogTemplate, typeof(T), exc.Message));

                    return new DataResult(false, exc.Message +
                        exc.InnerException == null ? "" : "(" + exc.InnerException + ")"
                    );
                }
            }
        }

        public DataResult DeleteBulk(Expression<Func<T, bool>> predicate)
        {
            using (IServiceScope scope = _serviceScopeFactory.CreateScope())
            {
                _context = scope.ServiceProvider.GetService<DataContext>();

                var list = _context.Set<T>().Where(predicate);
                foreach (var item in list)
                {
                    _context.Set<T>().Remove(item);
                }

                _context.SaveChanges();

                logger.Verbose(string.Format(DeleteBulkSucceedLogTemplate, typeof(T), list.Count()));

                return new DataResult(true, "");
            }
        }

        /// <summary>
        /// Get Entity By Key
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public T GetByKey(int id)
        {
            using (IServiceScope scope = _serviceScopeFactory.CreateScope())
            {
                _context = scope.ServiceProvider.GetService<DataContext>();

                try
                {
                    T aModel = _context.Set<T>().Where(x => x.Id == id).FirstOrDefault();
                    return aModel;
                }
                catch (Exception exc)
                {
                    return null;
                }
            }
        }

        /// <summary>
        /// Get All Entities
        /// </summary>
        /// <returns></returns>
        public List<T> GetAll()
        {
            using (IServiceScope scope = _serviceScopeFactory.CreateScope())
            {
                _context = scope.ServiceProvider.GetService<DataContext>();

                try
                {
                    return _context.Set<T>().ToList();
                }
                catch (Exception exc)
                {
                    return null;
                }
            }
        }

        /// <summary>
        /// Get All Entities
        /// </summary>
        /// <param name="orderBy">property name to order</param>
        /// <returns></returns>
        public List<T> GetAll(string orderBy, bool isDesc = false)
        {
            using (IServiceScope scope = _serviceScopeFactory.CreateScope())
            {
                _context = scope.ServiceProvider.GetService<DataContext>();

                return isDesc
                ? _context.Set<T>().OrderByDescending(orderBy).ToList()
                : _context.Set<T>().OrderBy(orderBy).ToList();
            }
        }

        /// <summary>
        /// Find an Entity
        /// </summary>
        /// <param name="predicate"></param>
        /// <returns></returns>
        public List<T> GetBy(Expression<Func<T, bool>> predicate)
        {
            using (IServiceScope scope = _serviceScopeFactory.CreateScope())
            {
                _context = scope.ServiceProvider.GetService<DataContext>();

                try
                {
                    return _context.Set<T>()
                        .Where(predicate)
                        .ToList();
                }
                catch (Exception exc)
                {
                    return null;
                }
            }
        }

        /// <summary>
        /// Find an Entity
        /// </summary>
        /// <param name="predicate"></param>
        /// <param name="orderBy">order by property name</param>
        /// <returns></returns>
        public List<T> GetBy(Expression<Func<T, bool>> predicate, string orderBy, bool isDesc = false)
        {
            using (IServiceScope scope = _serviceScopeFactory.CreateScope())
            {
                _context = scope.ServiceProvider.GetService<DataContext>();

                try
                {
                    return isDesc
                        ? _context.Set<T>().Where(predicate).OrderByDescending(orderBy).ToList()
                        : _context.Set<T>().Where(predicate).OrderBy(orderBy).ToList();
                }
                catch (Exception exc)
                {
                    return null;
                }
            }
        }

        /// <summary>
        /// Get Entities by Page
        /// </summary>
        /// <param name="pageNumber"></param>
        /// <param name="pageCount"></param>
        /// <returns></returns>
        public List<T> GetByPage(int pageNumber, int pageCount, string orderBy = "Id", bool isDesc = false)
        {
            using (IServiceScope scope = _serviceScopeFactory.CreateScope())
            {
                _context = scope.ServiceProvider.GetService<DataContext>();

                try
                {
                    return isDesc
                        ? _context.Set<T>().OrderByDescending(orderBy).Skip((pageNumber - 1) * pageCount).Take(pageCount).ToList()
                        : _context.Set<T>().OrderBy(orderBy).Skip((pageNumber - 1) * pageCount).Take(pageCount).ToList();
                }
                catch (Exception exc)
                {
                    return null;
                }
            }
        }

        /// <summary>
        /// Find and Get Entities by Page
        /// </summary>
        /// <param name="pageNumber"></param>
        /// <param name="pageCount"></param>
        /// <returns></returns>
        public List<T> GetByPage(Expression<Func<T, bool>> predicate, int pageNumber, int pageCount, string orderBy = "Id", bool isDesc = false)
        {
            using (IServiceScope scope = _serviceScopeFactory.CreateScope())
            {
                _context = scope.ServiceProvider.GetService<DataContext>();

                try
                {
                    return isDesc
                       ? _context.Set<T>().OrderByDescending(orderBy).Where(predicate).Skip((pageNumber - 1) * pageCount).Take(pageCount).ToList()
                       : _context.Set<T>().OrderBy(orderBy).Where(predicate).Skip((pageNumber - 1) * pageCount).Take(pageCount).ToList();
                }
                catch (Exception exc)
                {
                    return null;
                }
            }
        }

        public T FirstOrDefault(Expression<Func<T, bool>> predicate, bool asNoTracking)
        {
            using (IServiceScope scope = _serviceScopeFactory.CreateScope())
            {
                _context = scope.ServiceProvider.GetService<DataContext>();

                try
                {
                    var query = _context.Set<T>().AsQueryable();

                    if (asNoTracking)
                        query = query.AsNoTracking();

                    return query.Where(predicate).Take(1).FirstOrDefault();
                }
                catch (Exception exc)
                {
                    return null;
                }
            }
        }

        public T FirstOrDefault(Expression<Func<T, bool>> predicate, string orderBy = "Id", bool isDesc = false, bool asNoTracking = false)
        {
            using (IServiceScope scope = _serviceScopeFactory.CreateScope())
            {
                _context = scope.ServiceProvider.GetService<DataContext>();

                try
                {
                    var query = _context.Set<T>().AsQueryable();

                    if (asNoTracking)
                        query = query.AsNoTracking();

                    query = isDesc
                        ? query.OrderByDescending(orderBy)
                        : query.OrderBy(orderBy);

                    return query.Where(predicate).FirstOrDefault();
                }
                catch (Exception exc)
                {
                    return null;
                }
            }
        }

        /// <summary>
        /// Get Count of Entities
        /// </summary>
        /// <returns></returns>
        public int GetCount()
        {
            using (IServiceScope scope = _serviceScopeFactory.CreateScope())
            {
                _context = scope.ServiceProvider.GetService<DataContext>();

                return _context.Set<T>().Select(x => x.Id).Count();
            }
        }

        /// <summary>
        /// Find and Get Count of Entities
        /// </summary>
        /// <param name="predicate"></param>
        /// <returns></returns>
        public int GetCount(Expression<Func<T, bool>> predicate)
        {
            using (IServiceScope scope = _serviceScopeFactory.CreateScope())
            {
                _context = scope.ServiceProvider.GetService<DataContext>();

                return _context.Set<T>()
                .Where(predicate)
                .Count();
            }
        }

        /// <summary>
        /// Get random records 
        /// </summary>
        /// <param name="limit"></param>
        /// <returns></returns>
        public List<T> GetRandom(int limit)
        {
            using (IServiceScope scope = _serviceScopeFactory.CreateScope())
            {
                _context = scope.ServiceProvider.GetService<DataContext>();

                if (limit <= 0)
                    return new List<T>();

                return _context.Set<T>().OrderBy(x => Guid.NewGuid()).Take(limit).ToList();
            }
        }

        public List<T> GetRandom(Expression<Func<T, bool>> predicate, int limit)
        {
            using (IServiceScope scope = _serviceScopeFactory.CreateScope())
            {
                _context = scope.ServiceProvider.GetService<DataContext>();

                if (limit <= 0)
                    return new List<T>();

                return _context.Set<T>().Where(predicate).OrderBy(x => Guid.NewGuid()).Take(limit).ToList();
            }
        }

        #endregion
    }
}
