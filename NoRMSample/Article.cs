using System;
using System.Collections.Generic;
using Norm;
using Norm.Configuration;

namespace NoRMSample {

    public class Article {

        public ObjectId Id { get; set; }
        public string Title { get; set; }
        public string Body { get; set; }
        public string Author { get; set; }
        public DateTime DatePosted { get; set; }

        private List<Comment> _comments = new List<Comment>();
        public List<Comment> Comments {
            get { return _comments; }
            set { _comments = value; }
        }
    }

    public class Comment {

        public DateTime DateAdded { get; set; }
        public string CommentersName { get; set; }
        public string Body { get; set; }
    }

    public class ArticleMap : MongoConfigurationMap {
        
        public ArticleMap() {
            For<Article>(x => {
                x.ForProperty(p => p.Title).UseAlias("tl");
                x.ForProperty(p => p.Body).UseAlias("bd");
                x.ForProperty(p => p.Author).UseAlias("au");
                x.ForProperty(p => p.DatePosted).UseAlias("dt");
                x.ForProperty(p => p.Comments).UseAlias("ct");
            });
            
            For<Comment>(x => {
                x.ForProperty(p => p.DateAdded).UseAlias("dt");
                x.ForProperty(p => p.Body).UseAlias("bd");
                x.ForProperty(p => p.CommentersName).UseAlias("nm");
            });
        }
    }
}
