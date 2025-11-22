namespace SomeTrade.Data.Infrastructure
{
    using Entities;
    using System;
    using System.Collections.Generic;
    using System.Linq.Expressions;
    using System.Threading.Tasks;

    public interface IData<T>
    {
        DataResult Insert(T t);
        DataResult Update(T t);
        DataResult Delete(T t);
        DataResult DeleteByKey(int id);

        T GetByKey(int id);

        List<T> GetAll();
        List<T> GetAll(string orderBy, bool isDesc = false);
        List<T> GetBy(Expression<Func<T, bool>> predicate);
        List<T> GetBy(Expression<Func<T, bool>> predicate, string orderBy, bool isDesc = false);
        List<T> GetByPage(int pageNumber, int pageCount, string orderBy = "Id", bool isDesc = false);
        List<T> GetByPage(Expression<Func<T, bool>> predicate, int pageNumber, int pageCount, string orderBy = "Id", bool isDesc = false);
        
        T FirstOrDefault(Expression<Func<T, bool>> predicate, bool asNoTracking = false);
        T FirstOrDefault(Expression<Func<T, bool>> predicate, string orderBy = "Id", bool isDesc = false, bool asNoTracking = false);
        void DetachAllEntities();

        DataResult InsertBulk(List<T> ts, bool validateAndIgnoreBefore = false);

        DataResult UpdateBulk(List<T> ts, List<string> properties, List<object> values);
        DataResult UpdateBulk(Expression<Func<T, bool>> predicate, List<string> properties, List<object> values);
        DataResult UpdateBulk(Expression<Func<T, bool>> predicate, int pageNumber, int pageCount, List<string> properties, List<object> values);
        DataResult UpdateBulk(List<T> ts, List<string> properties, List<List<object>> valueSets);

        int GetCount();
        int GetCount(Expression<Func<T, bool>> predicate);

        List<T> GetRandom(int limit);
        List<T> GetRandom(Expression<Func<T, bool>> predicate, int limit);
    }
}
