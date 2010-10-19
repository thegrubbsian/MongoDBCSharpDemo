using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Norm;
using Norm.BSON;
using Norm.Collections;
using Norm.Linq;

namespace NoRMSample {

    public class MongoSession {

        private readonly string _connectionString;

        public MongoSession() {
            _connectionString = "mongodb://127.0.0.1/NoRMSample?strict=false";
        }

        public IMongo GetDb() {
            return Mongo.Create(_connectionString);
        }

        public void Delete<T>(T item) where T : class, new() {
            using (var db = Mongo.Create(_connectionString)) {
                db.Database.GetCollection<T>().Delete(item);
            }
        }

        public void DropCollection<T>() where T : class, new() {
            DropCollection(typeof (T).Name);
        }

        public void DropCollection(string collectionName) {
            using (var db = Mongo.Create(_connectionString)) {
                db.Database.DropCollection(collectionName);
            }
        }

        public T Single<T>(Expression<Func<T, bool>> expression) where T : class, new() {
            T retval;
            using (var db = Mongo.Create(_connectionString)) {
                retval = db.GetCollection<T>().AsQueryable()
                    .Where(expression).SingleOrDefault();
            }
            return retval;
        }

        public IQueryable<T> Query<T>() where T : class, new() {
            using (var db = Mongo.Create(_connectionString)) {
                return db.GetCollection<T>().AsQueryable();
            }
        }

        public void BulkInsert<T>(IEnumerable<T> items) where T : class, new() {
            using (var db = Mongo.Create(_connectionString)) {
                db.GetCollection<T>().Insert(items);
            }
        }

        public void Save<T>(T item) where T : class, new() {
            using (var db = Mongo.Create(_connectionString)) {
                db.GetCollection<T>().Save(item);
            }
        }

        public T MapReduce<T>(string map, string reduce) {
            T result;
            using (var db = Mongo.Create(_connectionString)) {
                var mr = db.Database.CreateMapReduce();
                var response =
                    mr.Execute(new MapReduceOptions(typeof(T).Name) {
                        Map = map,
                        Reduce = reduce
                    });
                var coll = (MongoCollection<MapReduceResult<T>>) response.GetCollection<MapReduceResult<T>>();
                var r = coll.Find().FirstOrDefault();
                result = r.Value;
            }
            return result;
        }
    }
}
