﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace Hibaza.CCP.Data.Infrastructure
{
    public interface ISession : IDisposable
    {
        void CommitChanges();
        void Delete<T>(Expression<Func<T, bool>> expression) where T : class;
        void Delete<T>(T item) where T : class;
        void DeleteAll<T>() where T : class;
        T Single<T>(Expression<Func<T, bool>> expression) where T : class;
        System.Linq.IQueryable<T> All<T>() where T : class;
        void Add<T>(T item) where T : class;
        void Add<T>(IEnumerable<T> items) where T : class;
        void Save<T>(T item) where T : class;
        void Update<T>(T mItem, T vItem) where T : class;
    }
}
