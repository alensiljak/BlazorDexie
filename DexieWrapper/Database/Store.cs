﻿using Nosthy.Blazor.DexieWrapper.JsInterop;
using Nosthy.Blazor.DexieWrapper.Utils;
using System.Dynamic;

namespace Nosthy.Blazor.DexieWrapper.Database
{
    public class Store<T, TKey> : Collection<T, TKey>, IStore
    {
        public string[] SchemaDefinitions { get; }
        public string PrimaryKey { get; }
        public string[] Indices { get; }

        protected override List<Command> CurrentCommands { get => new List<Command>(); }

        public Store(params string[] schemaDefinitions)
        {
            if (schemaDefinitions.Length == 0)
            {
                throw new ArgumentException("Must contain at least one element", nameof(schemaDefinitions));
            }

            SchemaDefinitions = schemaDefinitions.Select(Camelizer.ToCamelCase).ToArray();

            Indices = schemaDefinitions.Select(d => d
            .TrimStart('+')
            .TrimStart('&')
            .TrimStart('*'))
                .ToArray();

            PrimaryKey = Indices.First();   
        }


        public async Task<TKey> Add(T item)
        {
            return await Execute<TKey>("add", item);
        }

        public async Task<TKey> Add(T item, TKey? key)
        {
            return await Execute<TKey>("add", item, key);
        }

        public async Task<TKey> BulkAdd(IEnumerable<T> items)
        {
            return await Execute<TKey>("bulkAdd", items);
        }

        public async Task BulkAdd(IEnumerable<T> items, IEnumerable<TKey>? keys)
        {
            await ExecuteNonQuery("bulkAdd", items, keys);
        }

        public async Task<List<TKey>> BulkAddReturnAllKeys(IEnumerable<T> items)
        {
            return await Execute<List<TKey>>("bulkAdd", items, new { allKeys = true });
        }

        public async Task BulkDelete(IEnumerable<TKey> primaryKeys)
        {
            await ExecuteNonQuery("bulkDelete", primaryKeys);
        }

        public async Task<T?[]> BulkGet(IEnumerable<TKey> keys)
        {
            return await Execute<T[]>("bulkGet", keys);
        }

        public async Task<TKey> BulkPut(IEnumerable<T> items)
        {
            return await Execute<TKey>("bulkPut", items);
        }

        public async Task BulkPut(IEnumerable<T> items, IEnumerable<TKey>? keys)
        {
            await ExecuteNonQuery("bulkPut", items, keys);
        }

        public async Task<List<TKey>> BulkPutReturnAllKeys(IEnumerable<T> items)
        {
            return await Execute<List<TKey>>("bulkPut", items, new { allKeys = true });
        }

        public async Task Delete(TKey primaryKey)
        {
            await ExecuteNonQuery("delete", primaryKey);
        }

        public async Task<T?> Get(TKey primaryKey)
        {
            return await Execute<T?>("get", primaryKey);
        }

        public Collection<T, TKey> OrderBy(string index)
        {
            var collection = CreateNewColletion();
            collection.AddCommand("orderBy", Camelizer.ToCamelCase(index));
            return collection;
        }

        public async Task<TKey> Put(T item)
        {
            return await Execute<TKey>("put", item);
        }

        public async Task<TKey> Put(T item, TKey? key)
        {
            return await Execute<TKey>("put", item, key);
        }

        public async Task<int> Update(TKey key, Dictionary<string, object> changes)
        {
            var changesObject = new ExpandoObject();
            foreach (var change in changes)
            {
                changesObject.TryAdd(Camelizer.ToCamelCase(change.Key), change.Value);
            }

            return await Execute<int>("update", key, changesObject);
        }

        public WhereClause<T, TKey> Where(string indexOrPrimaryKey)
        {
            var collection = CreateNewColletion();
            collection.AddCommand("where", Camelizer.ToCamelCase(indexOrPrimaryKey));
            return new WhereClause<T, TKey>(collection);
        }

        public Collection<T, TKey> Where(Dictionary<string, object> criterias)
        {
            var collection = CreateNewColletion();

            var criteriaObject = new ExpandoObject();
            foreach (var criteria in criterias)
            {
                criteriaObject.TryAdd(Camelizer.ToCamelCase(criteria.Key), criteria.Value);
            }

            collection.AddCommand("where", criteriaObject);

            return collection;
        }

        public async Task Clear()
        {
            await ExecuteNonQuery("clear");
        }

        public bool HasIndex(string index)
        {
            return Indices.Contains(index); 
        }

        protected override Collection<T, TKey> CreateNewColletion()
        {
            return new Collection<T, TKey>(Db, StoreName, CommandExecuterJsInterop);
        }
    }
}
