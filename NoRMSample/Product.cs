using Norm;
using Norm.BSON.DbTypes;

namespace NoRMSample {

    public class Product {

        [MongoIdentifier]
        public string Sku { get; set; }

        public string Name { get; set; }
        public string Price { get; set; }
        public string Weight { get; set; }
        public DbReference<Supplier> Supplier { get; set; }
    }

    public class Supplier {

        public ObjectId Id { get; set; }
        public string Name { get; set; }
        public string Address { get; set; }
    }
}
