using System;
using System.Collections.Generic;
using System.Linq;
using Norm;
using Norm.BSON;
using Norm.BSON.DbTypes;
using Norm.Configuration;
using NoRMSample;
using NUnit.Framework;

namespace Tests {

    [TestFixture]
    public class NoRMTests {

        private const string ConnectionString = "mongodb://localhost/NoRMSample";

        [SetUp]
        public void DropCollections() {
            var session = new MongoSession();
            session.DropCollection<Patient>();
            session.DropCollection<Product>();
            session.DropCollection<Article>();
            session.DropCollection("MythicalCreatures");
        }

        [Test]
        public void Demonstrate_SimplePersistance() {

            var patient = new Patient {
                FirstName = "John",
                LastName = "Doe",
                DateOfBirth = new DateTime(1980, 1, 1)
            };

            using (var db = Mongo.Create("mongodb://localhost/NoRMSample")) {

                db.GetCollection<Patient>().Save(patient);

                var fetched = db.GetCollection<Patient>()
                    .Find(new { LastName = "Doe" });

                Assert.IsNotNull(fetched.First());
            }
        }

        [Test]
        public void Demonstrate_ComplexPersistance() {

            var patient = new Patient {
                FirstName = "John",
                LastName = "Doe",
                DateOfBirth = new DateTime(1980, 1, 1),
                Prescriptions = new List<Prescription> {
                    new Prescription {
                        DatePrescribed = DateTime.Now,
                        Medication = "Asprin",
                        Dosage = "200mg",
                        NumberOfRefils = 1000
                    }                       
                },
                Visits = new List<ProviderVisit> {
                    new ProviderVisit {
                        DateOfVisit = DateTime.Now,
                        ProviderName = "Townsville Healthcare",
                        ProviderNotes = "Patient is a pain in the arse."
                    }
                }
            };

            var session = new MongoSession();
            session.Save(patient);

            var fetched = session.Query<Patient>()
                .Where(x => x.LastName == "Doe");

            Assert.IsNotNull(fetched.First());
        }

        [Test]
        public void Demonstrate_MongoIdentifierAttribute() {

            var product = new Product {
                Name = "Hammer",
                Price = "11.50",
                Sku = "a3k4j22je9",
                Weight = "4"
            };

            var session = new MongoSession();
            session.Save(product);

            var fetched = session.Query<Product>().Where(x => x.Sku == product.Sku);

            Assert.IsNotNull(fetched.First());
        }

        [Test]
        public void Demonstrate_DbReference() {

            var session = new MongoSession();

            var supplier = new Supplier { Name = "Acme" };
            session.Save(supplier);

            var product = new Product {
                Name = "Shovel",
                Price = "17.50",
                Sku = "a3k4j22je9",
                Weight = "11",
                Supplier = new DbReference<Supplier>(supplier.Id)
            };
            session.Save(product);

            var fetched = session.Query<Product>().Where(x => x.Sku == product.Sku);

            Assert.AreEqual(fetched.First().Supplier.Id, supplier.Id);

            var fetchedSupplier = fetched.First().Supplier.Fetch(() => session.GetDb());

            Assert.AreEqual(fetchedSupplier.Name, supplier.Name);
        }

        [Test]
        public void Demonstrate_Expando() {

            var creature = new Thing();
            creature["Color"] = "Green";
            creature["MythicalPower"] = "Can fly.";

            using (var db = Mongo.Create(ConnectionString)) {

                db.GetCollection<Thing>("MythicalCreatures").Save(creature);

                var fetched = db.GetCollection<Thing>("MythicalCreatures")
                    .FindOne(new { Color = "Green" });

                Assert.IsNotNull(fetched);
            }
        }

        [Test]
        public void Demonstrate_PropertyAliasing() {

            MongoConfiguration.Initialize(x => x.AddMap<ArticleMap>());

            var article = new Article {
                Author = "Mark Twain",
                Body = "Once upon a time...",
                DatePosted = DateTime.Now,
                Title = "A Conneticuit Yankee in King Arthur's Court",
                Comments = new List<Comment> {
                    new Comment { CommentersName = "Me", Body = "Some Message..." }                                 
                }
            };

            var session = new MongoSession();
            session.Save(article);

            var fetched = session.Query<Article>()
                .Where(x => x.Id == article.Id).SingleOrDefault();

            Assert.AreEqual(fetched.Title, article.Title);
        }

        [Test]
        public void Demonstrate_TypeDiscrimination() {
            
            Assert.Fail();
        }

        [Test]
        public void Demonstrate_MapReduce() {

            var events = new List<Event> {
                new Event { Tags = new List<string> { "Apple", "Orange", "Pear" } },
                new Event { Tags = new List<string> { "Apple", "Orange" } },
                new Event { Tags = new List<string> { "Orange" } },
                new Event { Tags = new List<string> { "Banana" } },
                new Event { Tags = new List<string> { "Banana", "Peach" } }
            };

            var session = new MongoSession();
            session.BulkInsert(events);

            var map = @"function () { 
                this.Tags.forEach(function(x) {
                    emit(x, { count: 1 });
                });
            }";

            var reduce = @"function(key, values) {
                var total = 0;
                for (var i = 0; i < values.length; i++) {
                    total += values[i].count;
                }
                return { Tag: key, Count: total };
            }";

            using (var db = Mongo.Create(ConnectionString)) {
                var values = db.GetCollection<Event>().MapReduce<Expando>(map, reduce);
                Assert.NotNull(values.ToList()[0].Get<Expando>("value")["Count"]);
            }
        }
    }
}
