using EdFi.SampleDataGenerator.Console.Entities.Csv.EducationOrganization;
using EdFi.SampleDataGenerator.Console.Entities.Csv.EducationOrgCalendar;
using EdFi.SampleDataGenerator.Console.Entities.Csv.MasterSchedule;
using System;
using System.Collections.Generic;
using System.Linq;

namespace EdFi.SampleDataGenerator.Console.Entities.Csv
{
    public static class CsvWriteHelper
    {
        public const string DefaultDistrictId = "255901";
        public static List<string> DefaultHighschoolIds  = new List<string>() { "255901001"}; // Grades 9-12
        public static List<string> DefaultJuniorHighschoolIds = new List<string>() { "255901002", "255901003" }; // Grades 7 and 8
        public static List<string> DefaultMiddleschoolIds = new List<string>() { "255901004", "255901005" }; // Grades 4-6
        public static List<SessionTerm> DefaultSessionNames = new List<SessionTerm>() {
            new SessionTerm("2016 - 2017 Fall Semester", "uri://ed-fi.org/TermDescriptor#Fall Semester"),
            new SessionTerm("2016 - 2017 Spring Semester","uri://ed-fi.org/TermDescriptor#Spring Semester")
             };
        public static void ModifyCsvFiles(District district)
        {
            ModifyAccountabilityRating(district);
            ModifyClassPeriod(district);
            ModifyCourse(district);
            ModifyLocalEducationAgency(district);
            ModifyLocation(district);
            ModifyProgram(district);
            ModifySchool(district);
            ModifyCalendar(district);
            ModifyCalendarDate(district);
            ModifyGradingPeriod(district);
            ModifySession(district);
        }
        public static void ModifyAccountabilityRating(District district)
        {
            var records = AccountabilityRating.ReadFile();
            var resultRecords = new List<AccountabilityRating>();

            Random random = new Random();

            var ratings = records.GroupBy(x => x.Rating).Select(g => g.Key).ToArray();
            var districtRecord = records.FirstOrDefault();

            districtRecord.Rating = ratings[random.Next(0, ratings.Count())];
            districtRecord.EducationOrganizationIdentityId = district.ID;

            resultRecords.Add(districtRecord);

            foreach (var school in district.Schools)
            {
                var newSchoolRecord = records.FirstOrDefault();

                newSchoolRecord.Rating = ratings[random.Next(0, ratings.Count())];
                newSchoolRecord.EducationOrganizationIdentityId = school.ID;

                resultRecords.Add(newSchoolRecord);
            }

            AccountabilityRating.WriteFile(resultRecords);
        }

        
        public static void ModifyClassPeriod(District district)
        {
            var resultRecords = new List<ClassPeriod>();

            foreach (var school in district.Schools)
            {
                for (int i = 1; i <= 8; i++)
                {
                    resultRecords.Add(new ClassPeriod()
                    {
                        ClassPeriodName = $"0{i} - Traditional",
                        SchoolIdentityId = school.ID
                    });
                }
            }

            ClassPeriod.WriteFile(resultRecords);
        }

        public static void ModifyCourse(District district)
        {
            var records = Course.ReadFile();
            var resultRecords = new List<Course>();
            var courseOfferingResultRecords = new List<CourseOffering>();
            var sectionResultRecords = new List<Section>();

            var courseOfferingRecords = CourseOffering.ReadFile();
            var sectionRecords = Section.ReadFile();

            var gradeLevels = records
                .GroupBy(x => x.OfferedGradeLevel)
                .Select(x => new
                {
                    GradeLevel = x.Key.Split('#')[1],
                    Courses = x.GroupBy(c => new {
                        c.CompetencyLevel,
                        c.CourseCode,
                        c.CourseDescription,
                        c.CourseLevelCharacteristic,
                        c.CourseIdentificationSystem,
                        c.CourseTitle,
                        c.CouseIdentificationCode,
                        c.LearningStandardIdentityId,
                        c.AcademicSubject
                    })
                    .Select(c => new {
                        Id = $"CRSE_SchoolId_{c.Key.CourseCode}",
                        CompetencyLevel = c.Key.CompetencyLevel,
                        CourseDescription = c.Key.CourseDescription,
                        CourseCode = c.Key.CourseCode,
                        CourseLevelCharacteristic = c.Key.CourseLevelCharacteristic,
                        CourseIdentificationSystem = c.Key.CourseIdentificationSystem,
                        CourseTitle = c.Key.CourseTitle,
                        CouseIdentificationCode = c.Key.CouseIdentificationCode,
                        LearningStandardIdentityId = c.Key.LearningStandardIdentityId,
                        AcademicSubject = c.Key.AcademicSubject
                    }).ToList()
                }).ToList();

            foreach (var school in district.Schools)
            {
                foreach (var gl in school.GradeLevels)
                {
                    // Identify matching gradelevel and apply all possible courses
                    var currentGl = gl.Grade.GetGradeLevelFromDb();
                    var gradeLevel = gradeLevels.FirstOrDefault(x => x.GradeLevel.GetGradeLevelFromCsv() == currentGl); ;

                    if (gradeLevel == null) continue; // they don't exist in the current data

                    foreach (var course in gradeLevel.Courses)
                    {
                        var newCourseRecord = new Course()
                        {
                            Id = course.Id.Replace("SchoolId", school.ID),
                            CompetencyLevel = course.CompetencyLevel,
                            CourseDescription = course.CourseDescription,
                            CourseCode = course.CourseCode,
                            CourseLevelCharacteristic = course.CourseLevelCharacteristic,
                            CourseIdentificationSystem = course.CourseIdentificationSystem,
                            CourseTitle = course.CourseTitle,
                            CouseIdentificationCode = course.CouseIdentificationCode,
                            EducationOrganizationIdentityId = school.ID,
                            LearningStandardIdentityId = course.LearningStandardIdentityId,
                            OfferedGradeLevel = $"uri://ed-fi.org/GradeLevelDescriptor#{gradeLevel.GradeLevel}",
                            AcademicSubject = course.AcademicSubject
                        };

                        resultRecords.Add(newCourseRecord);

                        var courseOffering = courseOfferingRecords.FirstOrDefault(x => x.LocalCourseCode == course.CourseCode);
                        foreach (var st in DefaultSessionNames)
                        {
                            var newCourseOfferingRecord = new CourseOffering()
                            {
                                LocalCourseCode = course.CourseCode,
                                CourseIdentityCourseCode = course.CourseCode,
                                CourseLookupCourseCode = course.CourseCode,
                                CourseLookupIdentificationCode = course.CourseCode,
                                CourseIdentityEducationOrganizationIdentityId = school.ID,
                                CourseLookupIdentificationSystem = courseOffering?.CourseLookupIdentificationSystem,
                                SchoolIdentityId = school.ID,
                                SessionIdentitySchoolIdentityId = school.ID,
                                SessionIdentitySchoolYear = courseOffering?.SessionIdentitySchoolYear,
                                SessionIdentitySessionName = st.SessionName,
                                SessionLookupSchooIdentityId = school.ID,
                                SessionLookupSessionName = st.SessionName,
                                SessionLookupTerm = st.Term
                            };
                            courseOfferingResultRecords.Add(newCourseOfferingRecord);
                        }
                      

                        var section = sectionRecords.FirstOrDefault(x => x.CourseOfferingIdentityLocalCourseCode == course.CourseCode);
                        foreach (var st in DefaultSessionNames)
                        {
                            for (int i = 1; i <= 8; i++)
                            {
                                var newSection = new Section()
                                {
                                    AvailableCreditsCredits1 = section?.AvailableCreditsCredits1,
                                    ClassPeriodIdentityClassPeriodName = $"0{i} - Traditional",
                                    ClassPeriodIdentitySchoolId = school.ID,
                                    ClassPeriodIdentitySchoolIdentityId = school.ID,
                                    CourseOfferingIdentityLocalCourseCode = course.CourseCode,
                                    CourseOfferingIdentitySessionIdentitySchoolIdentityId = school.ID,
                                    CourseOfferingIdentitySessionIdentitySchoolYear = section?.CourseOfferingIdentitySessionIdentitySchoolYear,
                                    CourseOfferingIdentitySessionIdentitySessionName = st.SessionName,
                                    CourseOfferingIdentitySessionLookupSchoolIdentityId = school.ID,
                                    CourseOfferingIdentitySessionLookupSessionName = st.SessionName,
                                    CourseOfferingIdentitySessionLookupTerm = st.Term,
                                    CourseOfferingidentitySchoolIdentityId = school.ID,
                                    EducationalEnvironment = section?.EducationalEnvironment,
                                    InstructionLanguage = section?.InstructionLanguage,
                                    LocationIdentityClassroomIdentificationCode = section?.LocationIdentityClassroomIdentificationCode,
                                    LocationIdentitySchoolIdentityId = school.ID,
                                    LocationSchoolIdentityId = school.ID,
                                    ProgramIdentityEducationOrganizationIdentityId = district.ID,
                                    ProgramIdentityProgramName = section?.ProgramIdentityProgramName,
                                    ProgramIdentityProgramType = section?.ProgramIdentityProgramType,
                                    SectionIdentifier = section.SectionIdentifier.Replace(section.SectionIdentifier.Split('-')[0], $"{school.ID}_0{i}"),
                                    SequenceOfCourse = section.SequenceOfCourse
                                };

                                sectionResultRecords.Add(newSection);
                            }
                        }
                    }
                }
            }

            Course.WriteFile(resultRecords);
            CourseOffering.WriteFile(courseOfferingResultRecords);
            Section.WriteFile(sectionResultRecords);
        }


        // Question: I dont know how to modify this one...
        //public static void ModifyEducationServiceCenter(District district)
        //{
        //    var resultRecords = new List<EducationServiceCenter>();

        //    foreach (var school in district.Schools)
        //    {
        //        for (int i = 1; i <= 8; i++)
        //        {
        //            resultRecords.Add(new EducationServiceCenter()
        //            {
        //                ClassPeriodName = $"0{i} - Traditional",
        //                SchoolIdentityId = school.ID
        //            });
        //        }
        //    }

        //    ClassPeriod.WriteFile(resultRecords);
        //}

        public static void ModifyLocalEducationAgency(District district)
        {
            var resultRecords = LocalEducationAgency.ReadFile();


            foreach (var record in resultRecords)
            {
                record.City = district.City;
                record.EducationOrganizationIdentificationCode = district.ID;
                record.Id = $"LEAG_{district.ID}";
                record.LocalEducationAgencyId = district.ID;
                record.NameOfCounty = district.State;
                record.NameOfInstitution = district.Name;
                record.PostalCode = district.PostalCode;
                record.ShortNameOfInstitution = district.Name.Substring(5); // Question: Logic for this?
                record.StateOrganizationId = district.ID;
                record.TelephoneNumber = "(832) 356-8309"; // TODO: Add real data
                record.StreetNumberName = "8996 Spruce Avenue"; // TODO: Add real data
            }

            LocalEducationAgency.WriteFile(resultRecords);
        }

        public static void ModifyLocation(District district)
        {
            var records = Location.ReadFile().Where(x => x.SchoolIdentityId == "255901001").ToList();
            var resultRecords = new List<Location>();
            
            foreach (var school in district.Schools)
            {
                resultRecords.AddRange(records.Select(x => new Location
                {
                    Id = $"LOCN_{school.ID}-{x.ClassroomIdentificationCode}",
                    ClassroomIdentificationCode = x.ClassroomIdentificationCode,
                    MaximumNumberOfSeats = x.MaximumNumberOfSeats,
                    OptimalNumberOfSeats = x.OptimalNumberOfSeats,
                    SchoolIdentityId = school.ID
                }));
            }

            Location.WriteFile(resultRecords);
        }

        public static void ModifyProgram(District district)
        {
            var resultRecords = EducationOrganization.Program.ReadFile();

            foreach (var record in resultRecords)
            {
                var programId = record.ProgramId.Split('_')[1];
                record.EducationOrganizationIdentityId = district.ID;
                record.Id = $"PRGM_{district.ID}_{programId}";
                record.ProgramId = $"{district.ID}_{programId}";
            }

            EducationOrganization.Program.WriteFile(resultRecords);
        }

        public static void ModifySchool(District district)
        {
            var resultRecords = new List<EducationOrganization.School>();

            foreach (var school in district.Schools)
            {
                var defaultRecord = EducationOrganization.School.ReadFile().FirstOrDefault();

                defaultRecord.City = school.City;
                defaultRecord.EducationOrganizationIdentificationCode = school.ID;
                // defaultRecord.GradeLevel = ""; not sure how to overwrite this
                defaultRecord.Id = $"SCOL_{school.ID}";
                defaultRecord.LocalEducationAgencyIdentityId = district.ID;
                defaultRecord.NameOfCounty = school.State;
                defaultRecord.NameOfInstitution = school.Name;
                defaultRecord.PostalCode = school.PostalCode;
                defaultRecord.SchoolId = school.ID;
                defaultRecord.ShortNameOfInstitution = school.Name.Substring(5);  // Question: Logic for this?
                defaultRecord.StateOrganizationId = school.ID;
                defaultRecord.TelephoneNumber = "(832) 356-8309"; // TODO: Add real data
                defaultRecord.StreetNumberName = "8996 Spruce Avenue"; // TODO: Add real data
                // TODO: defaultRecord.WebSite add read data
                resultRecords.Add(defaultRecord);
            }

            EducationOrganization.School.WriteFile(resultRecords);
        }

        public static void ModifyCalendar(District district)
        {
            var resultRecords = new List<Calendar>();

            foreach (var school in district.Schools)
            {
                resultRecords.Add(new Calendar {
                    Id = $"CAL_{school.ID}_Item20162017",
                    CalendarCode = "Calendar Code",
                    SchoolYear = "2016-2017",
                    CalendarType = "uri://ed-fi.org/CalendarTypeDescriptor#IEP",
                    SchoolIdentityId = school.ID
                });
            }

            Calendar.WriteFile(resultRecords);
        }

        public static void ModifyCalendarDate(District district)
        {
            var records = CalendarDate.ReadFile().Where(x => x.SchoolIdentityId == "255901001");
            var resultRecords = new List<CalendarDate>();

            foreach (var school in district.Schools)
            {
                resultRecords.AddRange(records.Select(x => new CalendarDate {
                    CalendarCode  = x.CalendarCode,
                    CalendarEvent = x.CalendarEvent,
                    Date = x.Date,
                    Id = x.Id.Replace("255901001",  school.ID),
                    SchoolIdentityId = school.ID,
                    SchoolYear = x.SchoolYear
                }));
            }

            CalendarDate.WriteFile(resultRecords);
        }

        public static void ModifyGradingPeriod(District district)
        {
            var records = GradingPeriod.ReadFile().Where(x => x.SchoolIdentityId == "255901001");
            var resultRecords = new List<GradingPeriod>();

            foreach (var school in district.Schools)
            {
                resultRecords.AddRange(records.Select(x => new GradingPeriod
                {
                    BeginDate = x.BeginDate,
                    TotalInstructionalDays = x.TotalInstructionalDays,
                    PeriodSequence = x.PeriodSequence,
                    GradingPeriod1 = x.GradingPeriod1,
                    EndDate = x.EndDate,
                    Id = x.Id.Replace("255901001", school.ID),
                    SchoolIdentityId = school.ID,
                    SchoolYear = x.SchoolYear
                }));
            }

            GradingPeriod.WriteFile(resultRecords);
        }

        public static void ModifySession(District district)
        {
            var records = Session.ReadFile().Where(x => x.SchoolIdentityId == "255901001");
            var resultRecords = new List<Session>();

            foreach (var school in district.Schools)
            {
                resultRecords.AddRange(records.Select(x => new Session
                {
                    BeginDate = x.BeginDate,
                    TotalInstructionalDays = x.TotalInstructionalDays,
                    SessionName = x.SessionName,
                    Term = x.Term,
                    EndDate = x.EndDate,
                    Id = x.Id.Replace("255901001", school.ID),
                    SchoolIdentityId = school.ID,
                    SchoolYear = x.SchoolYear,
                    GradingPeriodIdentityGradingPeriod = x.GradingPeriodIdentityGradingPeriod,
                    GradingPeriodIdentityPeriodSequence = x.GradingPeriodIdentityPeriodSequence,
                    GradingPeriodIdentitySchoolYear = x.GradingPeriodIdentitySchoolYear,
                    GradingPeriodIdentitySchoolId = school.ID,
                }));
            }

            Session.WriteFile(resultRecords);
        }

        private static bool IsDistrict(this string value) => value == DefaultDistrictId;
        private static bool IsHighschool(this string value) => DefaultHighschoolIds.Contains(value);
        private static bool IsJuniorHighschool(this string value) => DefaultJuniorHighschoolIds.Contains(value);
        private static bool IsMiddleschool(this string value) => DefaultMiddleschoolIds.Contains(value);

        private static int GetGradeLevelFromDb(this string value) => Int32.Parse(value.Trim().Split(' ')[1]);
        private static int GetGradeLevelFromCsv(this string value)
        {

            switch (value)
            {
                case "Twelfth grade":
                    return 12;
                case "Eleventh grade":
                    return 11;
                case "Tenth grade":
                    return 10;
                case "Ninth grade":
                    return 9;
                case "Eighth grade":
                    return 8;
                case "Seventh grade":
                    return 7;
                case "Sixth grade":
                    return 6;
                case "Fifth grade":
                    return 5;
                case "Fourth grade":
                    return 4;
                default:
                    throw new FormatException("Unexpected Value in csv gradelevels");
            }
        }
    }

    public class SessionTerm
    {
        public SessionTerm(string session, string term)
        {
            SessionName = session;
            Term = term;
        }
        public string SessionName { get; set; }
        public string Term { get; set; }
    }
}
