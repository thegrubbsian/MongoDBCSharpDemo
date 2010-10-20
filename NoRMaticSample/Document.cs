using NoRMatic;

namespace NoRMaticSample {

    public class Document : NoRMaticModel<Document> {

        public int ProjectNumber { get; set; }
        public string Title { get; set; }
        public string Content { get; set; }
        public string Author { get; set; }
    }
}
