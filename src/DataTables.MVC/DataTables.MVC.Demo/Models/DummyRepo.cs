using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DataTables.MVC.Demo.Models
{

    public class DummyRepo
    {
        private static List<PersonModel> _Persons;
        public IEnumerable<PersonModel> Persons
        {
            get { return _Persons; }
        }

        private DummyRepo()
        {
            _Persons = new List<PersonModel> {
                new PersonModel{  Id = 1, FirstName = "Person", LastName = "One", DateOfBirth = DateTime.Now.AddYears(-20)},
                new PersonModel{  Id = 2, FirstName = "Person", LastName = "Two", DateOfBirth = DateTime.Now.AddYears(-24)}
            };
        }

        private static DummyRepo _Instance;
        public static DummyRepo Instance {
            get {
                _Instance = _Instance ?? new DummyRepo();
                return _Instance;
            }
        }
    }
}