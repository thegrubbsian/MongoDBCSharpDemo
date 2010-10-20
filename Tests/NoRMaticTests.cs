using System;
using System.Linq;
using System.Collections.Generic;
using Norm.BSON.DbTypes;
using NoRMatic;
using NUnit.Framework;
using NoRMaticSample;

namespace Tests {

    [TestFixture]
    public class NoRMaticTests {

        [SetUp]
        public void DropCollections() {
            Product.DeleteAll();
            Document.DeleteAll();
            Article.DeleteAll();
            Subscriber.DeleteAll();
        }

        [Test]
        public void Demonstrate_ActiveRecordStyleAPI() {

            var product = new Product {
                Name = "DVD Player",
                Price = "89.95",
                Sku = "d9ejr3jj3e"
            };

            product.Save();

            var fetched = Product.GetById(product.Id);
            Assert.AreEqual(product.Sku, fetched.Sku);

            product.Delete();

            var reFetched = Product.GetById(product.Id);
            Assert.IsNull(reFetched);
        }

        [Test]
        public void Demonstrate_Versioning() {

            Document.EnableVersioning();

            var document = new Document {
                Author = "Bill Clinton",
                Content = "The quick brown fox jumped over the...",
                Title = "Font Sample"
            };
            document.Save();

            document.Content = "And now we've changed it...";
            document.Save();

            var fetched = Document.GetById(document.Id);
            Assert.AreEqual(2, fetched.GetVersions().Count());
        }

        [Test]
        public void Demonstrate_SoftDelete() {

            Patient.EnableSoftDelete();

            var patient = new Patient {
                FirstName = "John",
                LastName = "Doe",
                DateOfBirth = new DateTime(1980, 1, 1)
            };

            patient.Save();
            patient.Delete();

            var fetched = Patient.FindOne(x => x.Id == patient.Id, includeDeleted: true);
            Assert.IsTrue(fetched.IsDeleted);
        }

        [Test]
        public void Demonstrate_Validation() {

            var article = new Article {
                Body = "Once upon a time...",
                DatePosted = DateTime.Now,
                Title = "A",
                Comments = new List<Comment> {
                    new Comment { CommentersName = "Me", Body = "Some Message..." }                                 
                }
            };
            article.Save();

            var fetched = Article.GetById(article.Id);
            Assert.IsNull(fetched);

            Assert.AreEqual(2, article.Errors.Count);
        }

        [Test]
        public void Demonstrate_DbReferenceFetch() {

            var supplier = new Supplier { Name = "Acme" };
            supplier.Save();

            var product = new Product {
                Name = "Shovel",
                Price = "17.50",
                Sku = "a3k4j22je9",
                Weight = "11",
                Supplier = new DbReference<Supplier>(supplier.Id)
            };
            product.Save();

            var fetched = Product.GetById(product.Id);

            Assert.AreEqual(supplier.Id, fetched.GetRef(x => x.Supplier).Id);
        }

        [Test]
        public void Demonstrate_SaveAndDeleteEventHooks() {

            const int projectNumber = 73;
            Document.AddBeforeSaveBehavior(x => {
                x.ProjectNumber = projectNumber;
                return true;
            });

            var document = new Document {
                Author = "Jim Beam",
                Content = "Tired of making stuff up",
                Title = "This is a title"
            };
            document.Save();

            var fetched = Document.GetById(document.Id);
            Assert.AreEqual(fetched.ProjectNumber, projectNumber);
        }

        [Test]
        public void Demonstrate_GlobalQueryFilters() {

            Document.AddQueryBehavior(x => x.ProjectNumber == 73);

            var document = new Document {
                ProjectNumber = 123,
                Author = "Jim Beam",
                Content = "Tired of making stuff up",
                Title = "This is a title"
            };
            document.Save();

            var fetched = Document.Find(x => x.Author == "Jim Beem");
            Assert.AreEqual(0, fetched.Count());
        }

        [Test]
        public void Demonstrate_SimpleLogging() {

            var log = new List<string>();
            NoRMaticConfig.SetLogListener(x => log.Add(x));

            var article = new Article {
                Author = "Eddie Bauer",
                Body = "One, two, three, go"
            };
            article.Save();

            var fetched = Article.All();

            Assert.AreEqual(0, log.Count);
        }

        [Test]
        public void Demonstrate_Configuration() {

            NoRMaticConfig.Initialize();

            var subscriberA = new Subscriber {
                FirstName = "Steve",
                LastName = "Carrell",
                City = "Austin, TX"
            };
            subscriberA.Save();

            var subscriberB = new Subscriber {
                FirstName = "Steve",
                LastName = "Carrell",
                City = "Charlotte"
            };
            subscriberB.Save();

            var fetched = Subscriber.Find(x => x.LastName == "Carrell");
            Assert.AreEqual(1, fetched.Count());
        }
    }
}
