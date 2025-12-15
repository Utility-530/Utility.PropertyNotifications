using System;
using System.Collections.Generic;
using System.Text;
using Utility.Interfaces.Generic.Data;
using Utility.Interfaces.NonGeneric;

namespace Utility.Entities
{

    public class Person : Entity
    {

        public Person()
        {
        }


        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public string? EmailAddress { get; set; }
    }
}