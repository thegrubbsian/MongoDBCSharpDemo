using System;
using System.Linq;
using System.Collections.Generic;
using Norm.BSON.DbTypes;
using NUnit.Framework;
using NoRMaticSample;

namespace Tests {

    [TestFixture]
    public class NoRMaticTests {

        private const string ConnectionString = "mongodb://localhost/NoRMSample";
        
        [SetUp]
        public void DropCollections() {
            Product.DeleteAll();
            Document.DeleteAll();
            Article.DeleteAll();
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

            Assert.Fail();
        }

        [Test]
        public void Demonstrate_GlobalQueryFilters() {

            Assert.Fail();
        }

        [Test]
        public void Demonstrate_SimpleLogging() {

            Assert.Fail();
        }

        [Test]
        public void Demonstrate_Configuration() {
            
        }
    }
}
