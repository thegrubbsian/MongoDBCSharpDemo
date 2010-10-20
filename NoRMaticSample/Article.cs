using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using NoRMatic;

namespace NoRMaticSample {

    public class Article : NoRMaticModel<Article> {

        [Required, StringLength(50, MinimumLength = 5)]
        public string Title { get; set; }

        [Required]
        public string Body { get; set; }

        [Required]
        public string Author { get; set; }

        public DateTime DatePosted { get; set; }

        private List<Comment> _comments = new List<Comment>();

        [ValidateChild]
        public List<Comment> Comments {
            get { return _comments; }
            set { _comments = value; }
        }
    }

    public class Comment {

        public DateTime DateAdded { get; set; }

        [Required]
        public string CommentersName { get; set; }

        [Required]
        public string Body { get; set; }
    }
}
