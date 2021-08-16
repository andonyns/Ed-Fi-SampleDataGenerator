using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EdFi.SampleDataGenerator.Console.Entities
{
    public class GradeLevel
    {
        public GradeLevel()
        {
            Ethnicities = new List<Ethnicity>();
        }
        public string SchoolID { get; set; }
        public string Grade { get; set; }
        public List<Ethnicity> Ethnicities { get; set; }
        public long TotalStudents { get; set; }
    }
}
