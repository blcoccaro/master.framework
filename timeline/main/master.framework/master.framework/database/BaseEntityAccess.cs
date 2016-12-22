using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace master.framework.database
{
    /// <summary>
    /// Data Access Layer BaseClass
    /// </summary>
    /// <typeparam name="TContext">Context of Application</typeparam>
    public abstract class BaseEntityAccess<TContext> : IDisposable
        where TContext : System.Data.Entity.DbContext
    {
        public dto.TimeStampKeeper TimestampKeeper = new dto.TimeStampKeeper();

        #region Variables
        /// <summary>
        /// Context Variable
        /// </summary>
        public TContext db = null;
        #endregion

        #region Constructors
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="setInitializerNull">Set Initializer Null (Default: true)</param>
        public BaseEntityAccess(bool setInitializerNull = true)
        {
            if (setInitializerNull)
            {
                System.Data.Entity.Database.SetInitializer<TContext>(null);
            }
            MappingEngine.Map();
            db = Activator.CreateInstance<TContext>();
        }
        #endregion

        #region Disposes
        /// <summary>
        /// Dispose
        /// </summary>
        public void Dispose()
        {
            try
            {
                /*if (this.scope != null)
                {
                    this.scope.Dispose();
                    this.scope = null;
                }*/
                if (db != null)
                {
                    try
                    {
                        if (db.Database != null && db.Database.Connection != null && db.Database.Connection.State == System.Data.ConnectionState.Open)
                        {
                            db.Database.Connection.Close();
                        }
                    }
                    catch (Exception)
                    {
                    }
                }
                if (db != null)
                {
                    db.Dispose();
                    db = null;
                }
            }
            catch (Exception)
            {

            }
        }

        #endregion

        #region Methods to save
        /// <summary>
        /// Method to save
        /// </summary>
        /// <typeparam name="TDTO">Type of DTO</typeparam>
        /// <typeparam name="TModel">Type of Model</typeparam>
        /// <param name="request">object to save</param>
        /// <param name="isAdd">Is to Add the object?</param>
        /// <param name="predicate">Where to search</param>
        /// <returns>Object Saved as DTO</returns>
        public TDTO Save<TDTO, TModel>(TDTO request, bool isAdd = true, System.Linq.Expressions.Expression<Func<TModel, bool>> predicate = null, bool automaticSaveChanges = true)
            where TDTO : class
            where TModel : class
        {
            TModel obj = null;

            if (!isAdd)
            {
                obj = db.Set<TModel>().FirstOrDefault(predicate);
                db.Entry<TModel>(obj).CurrentValues
                    .SetValues(Map<TDTO, TModel>(request));
            }
            else
            {
                obj = Map<TDTO, TModel>(request);
            }

            if (isAdd) { db.Set<TModel>().Add(obj); }
            if (automaticSaveChanges) { db.SaveChanges(); }
            return Map<TModel, TDTO>(obj);
        }

        /// <summary>
        /// Method to save
        /// </summary>
        /// <typeparam name="TClass">Type of Class</typeparam>
        /// <param name="request">object to save</param>
        /// <param name="isAdd">Is to Add the object?</param>
        /// <param name="predicate">Where to search</param>
        /// <returns>Object Saved as DTO</returns>
        public TClass Save<TClass>(TClass request, bool isAdd = true, System.Linq.Expressions.Expression<Func<TClass, bool>> predicate = null)
            where TClass : class
        {
            return Save<TClass, TClass>(request, isAdd, predicate);
        }
        public int SaveChanges()
        {
            return db.SaveChanges();
        }
        #endregion

        #region Methods to search
        /// <summary>
        /// Method to search
        /// </summary>
        /// <typeparam name="TDTO">Type of DTO</typeparam>
        /// <typeparam name="TModel">Type of Model</typeparam>
        /// <param name="predicate">Where to Search</param>
        /// <returns>List of Objects founded as DTO</returns>
        public List<TDTO> Where<TDTO, TModel>(System.Linq.Expressions.Expression<Func<TModel, bool>> predicate)
            where TDTO : class
            where TModel : class
        {
            var query = db.Set<TModel>().Where(predicate);

            return MapList<TModel, TDTO>(query.ToList());
        }
        /// <summary>
        /// Method to search
        /// </summary>
        /// <typeparam name="TClass">Type of Class</typeparam>
        /// <param name="predicate">Where to Search</param>
        /// <returns>List of Objects founded as Class</returns>
        public List<TClass> Where<TClass>(System.Linq.Expressions.Expression<Func<TClass, bool>> predicate)
            where TClass : class
        {
            return Where<TClass, TClass>(predicate);
        }


        /// <summary>
        /// Method to get the first or default
        /// </summary>
        /// <typeparam name="TDTO">Type of DTO</typeparam>
        /// <typeparam name="TModel">Type of Model</typeparam>
        /// <param name="predicate">Where to Search</param>
        /// <returns>Object founded as DTO</returns>
        public TDTO First<TDTO, TModel>(System.Linq.Expressions.Expression<Func<TModel, bool>> predicate)
            where TDTO : class
            where TModel : class
        {
            return Map<TModel, TDTO>(db.Set<TModel>().FirstOrDefault(predicate));
        }
        /// <summary>
        /// Method to get the first or default
        /// </summary>
        /// <typeparam name="TClass">Type of TClass</typeparam>
        /// <typeparam name="TModel">Type of Model</typeparam>
        /// <param name="predicate">Where to Search</param>
        /// <returns>Object founded as TClass</returns>
        public TClass First<TClass>(System.Linq.Expressions.Expression<Func<TClass, bool>> predicate)
            where TClass : class
        {
            return First<TClass, TClass>(predicate);
        }


        /// <summary>
        /// Method to check if exist
        /// </summary>
        /// <typeparam name="TModel">Type of Model</typeparam>
        /// <param name="predicate">Where to Search</param>
        /// <returns>If where exist</returns>
        public bool Exists<TModel>(System.Linq.Expressions.Expression<Func<TModel, bool>> predicate)
            where TModel : class
        {
            return db.Set<TModel>().Any(predicate);
        }
        #endregion

        #region Methods to delete
        /// <summary>
        /// Method to delete
        /// </summary>
        /// <typeparam name="TModel">Type of Model</typeparam>
        /// <param name="predicate">Where to search</param>
        public void Delete<TModel>(System.Linq.Expressions.Expression<Func<TModel, bool>> predicate)
            where TModel : class
        {
            List<TModel> lst = null;
            lst = db.Set<TModel>().Where(predicate).ToList();
            if (lst.Count > 0)
            {
                foreach (var item in lst)
                {
                    if (item != null)
                    {
                        db.Set<TModel>().Remove(item);
                    }
                }
            }
            db.SaveChanges();
        }
        #endregion

        #region Methods to search (with pagination)
        /// <summary>
        /// Method to get a grid (with pagination)
        /// </summary>
        /// <typeparam name="TDTO">Type of DTO</typeparam>
        /// <typeparam name="TModel">Type of Model</typeparam>
        /// <param name="where">Where clause</param>
        /// <param name="predicateAux">Where to use by system</param>
        /// <param name="page">Current Page</param>
        /// <param name="pageSize">Page Size</param>
        /// <param name="sort">Property to order</param>
        /// <param name="sortdir">Order direction</param>
        /// <returns></returns>
        public dto.ResponseGrid<TDTO> GetListSimple<TDTO, TModel>(Expression<Func<TModel, bool>> where = null, IQueryable<TModel> startQuery = null, System.Linq.Expressions.Expression<Func<TModel, bool>> predicateInsideAux = null, int page = 0, int pageSize = 10, string sort = "", bool count = true, bool onlyCount = false)
            where TDTO : class
            where TModel : class
        {
            TimestampKeeper.ClearNative();
            TimestampKeeper.AddStartNative("GetLisSimple", DateTime.Now);
            int totalrows = 0;
            dto.ResponseGrid<TDTO> ret = new dto.ResponseGrid<TDTO>();

            #region FROM
            TimestampKeeper.AddStartNative("GetLisSimple.CreateQuery", DateTime.Now);
            IQueryable<TModel> query = null;
            if (startQuery != null)
            {
                query = startQuery;
            }
            else
            {
                query = db.Set<TModel>()
                        .AsQueryable();
            }
            TimestampKeeper.AddEndCustom("GetLisSimple.CreateQuery", DateTime.Now);
            #endregion

            #region WHERE
            TimestampKeeper.AddStartNative("GetLisSimple.CreateWhere", DateTime.Now);
            if (where != null)
            {
                if (predicateInsideAux != null)
                {
                    query = query.Where(predicateInsideAux).AsQueryable();
                }

                query = query.Where(where).AsQueryable();
            }
            TimestampKeeper.AddEndCustom("GetLisSimple.CreateWhere", DateTime.Now);
            #endregion

            #region COUNT
            TimestampKeeper.AddStartNative("GetLisSimple.TotalRows", DateTime.Now);
            if (count || onlyCount)
            {
                totalrows = query.Count();
                ret.TotalRows = totalrows;
            }
            if (onlyCount)
            {
                return ret;
            }
            TimestampKeeper.AddEndNative("GetLisSimple.TotalRows", DateTime.Now);
            #endregion

            #region ORDER BY
            TimestampKeeper.AddStartNative("GetLisSimple.CreateOrderBy", DateTime.Now);
            query = query.OrderBy(sort).AsQueryable();
            TimestampKeeper.AddEndCustom("GetLisSimple.CreateOrderBy", DateTime.Now);
            #endregion

            #region PAGING
            TimestampKeeper.AddStartNative("GetLisSimple.SetPaging", DateTime.Now);
            query = query.SkipTake(page, pageSize);
            TimestampKeeper.AddEndCustom("GetLisSimple.SetPaging", DateTime.Now);
            #endregion

            #region SELECT
            TimestampKeeper.AddStartNative("GetLisSimple.List", DateTime.Now);
            ret.List = MapList<TModel, TDTO>(query.ToList());
            TimestampKeeper.AddEndNative("GetLisSimple.List", DateTime.Now);
            #endregion

            #region COUNT
            if (!count)
            {
                totalrows = ret.List.Count;
                ret.TotalRows = totalrows;
            }
            #endregion

            TimestampKeeper.AddEndNative("GetLisSimple", DateTime.Now);
            ret.Timestamp = TimestampKeeper;
            return ret;
        }

        /// <summary>
        /// Method to get a grid (with pagination)
        /// </summary>
        /// <typeparam name="TClass">Type of TClass</typeparam>
        /// <param name="where">Where clause</param>
        /// <param name="predicateAux">Where to use by system</param>
        /// <param name="page">Current Page</param>
        /// <param name="pageSize">Page Size</param>
        /// <param name="sort">Property to order</param>
        /// <param name="sortdir">Order direction</param>
        /// <returns></returns>
        public dto.ResponseGrid<TClass> GetListSimple<TClass>(Expression<Func<TClass, bool>> where = null, IQueryable<TClass> startQuery = null, System.Linq.Expressions.Expression<Func<TClass, bool>> predicateInsideAux = null, int page = 0, int pageSize = 10, string sort = "Id ASC", bool count = true, bool onlyCount = false)
            where TClass : class
        {
            return GetListSimple<TClass, TClass>(where, startQuery, predicateInsideAux, page, pageSize, sort, count);
        }
        #endregion

        #region Methods to transform
        /// <summary>
        /// Transform method
        /// </summary>
        /// <typeparam name="TSource">Object type of source</typeparam>
        /// <typeparam name="TDestination">Object type of destination</typeparam>
        /// <param name="source">Object to transform</param>
        /// <returns>Object transformed to destination type</returns>
        public TDestination Map<TSource, TDestination>(TSource source)
            where TSource : class
            where TDestination : class
        {
            //MappingEngine.Mapper<TSource, TDestination>();
            return AutoMapper.Mapper.Map<TSource, TDestination>(source);
        }

        /// <summary>
        /// Transform method (List)
        /// </summary>
        /// <typeparam name="TSource">Object type of source</typeparam>
        /// <typeparam name="TDestination">Object type of destination</typeparam>
        /// <param name="source">Object to transform</param>
        /// <returns>Object transformed to destination type (List)</returns>
        public List<TDestination> MapList<TSource, TDestination>(List<TSource> source)
            where TSource : class
            where TDestination : class
        {
            //MappingEngine.Mapper<TSource, TDestination>();
            return AutoMapper.Mapper.Map<List<TSource>, List<TDestination>>(source);
        }
        #endregion
    }
}
