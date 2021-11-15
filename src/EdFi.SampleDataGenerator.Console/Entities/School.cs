using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EdFi.SampleDataGenerator.Console.Entities
{
    public class School
    {
        public School()
        {
            GradeLevels = new List<GradeLevel>();
        }
        public string ID { get; set; }
        public string Name { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public string StateAbr { get; set; }
        public string PostalCode { get; set; }
        public string AreaCode { get; set; }
        public string Level { get; set; }
        public string LeaID { get; set; }
        public List<GradeLevel> GradeLevels { get; set; }
    }
}
