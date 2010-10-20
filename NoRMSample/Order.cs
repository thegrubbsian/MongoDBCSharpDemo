using Norm;

namespace NoRMSample {

    public class Order {
        public ObjectId Id { get; set; }
        public int Quantity { get; set; }
        public OrderItem Item { get; set; }
    }

    [MongoDiscriminated]
    public abstract class OrderItem {
        public ObjectId Id { get; set; }
        public string Supplier { get; set; }
        public string Sku { get; set; }
    }

    [MongoDiscriminated]
    public class Book : OrderItem {
        public string Author { get; set; }
        public string Title { get; set; }
    }

    [MongoDiscriminated]
    public class Chair : OrderItem {
        public string Maker { get; set; }
        public int NumberOfLegs { get; set; }
    }
}
