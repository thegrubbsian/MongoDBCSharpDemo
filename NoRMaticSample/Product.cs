using Norm.BSON.DbTypes;
using NoRMatic;

namespace NoRMaticSample {

    public class Product : NoRMaticModel<Product> {

        public string Sku { get; set; }
        public string Name { get; set; }
        public string Price { get; set; }
        public string Weight { get; set; }
        public DbReference<Supplier> Supplier { get; set; }
    }

    public class Supplier : NoRMaticModel<Supplier> {

        public string Name { get; set; }
        public string Address { get; set; }
    }
}
