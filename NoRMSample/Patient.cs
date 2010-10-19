using System;
using System.Collections.Generic;
using Norm;
using Norm.Attributes;

namespace NoRMSample {

    public class Patient {

        public ObjectId Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public DateTime DateOfBirth { get; set; }

        private List<Prescription> _prescriptions = new List<Prescription>();
        public List<Prescription> Prescriptions {
            get { return _prescriptions; }
            set { _prescriptions = value; }
        }

        private List<ProviderVisit> _visits = new List<ProviderVisit>();
        public List<ProviderVisit> Visits {
            get { return _visits; }
            set { _visits = value; }
        }

        [MongoIgnore]
        public int Age {
            get { return Convert.ToInt32((DateTime.Now - DateOfBirth).TotalDays / 365); }
        }
    }

    public class Prescription {

        public string Medication { get; set; }
        public string Dosage { get; set; }
        public DateTime DatePrescribed { get; set; }
        public int NumberOfRefils { get; set; }
    }

    public class ProviderVisit {

        public DateTime DateOfVisit { get; set; }
        public string ProviderName { get; set; }
        public string ProviderNotes { get; set; }
    }
}
