using System.Collections.Generic;
using Norm;

namespace NoRMSample {

    public class Event {

        public ObjectId Id { get; set; }
        
        private List<string> _tags = new List<string>();
        public List<string> Tags {
            get { return _tags; }
            set { _tags = value; }
        }
    }
}
