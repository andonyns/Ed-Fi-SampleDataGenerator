using CsvHelper.Configuration;
using System.Collections.Generic;

namespace EdFi.SampleDataGenerator.Console.Entities.Csv.EducationOrganization
{
    public class EducationServiceCenter
    {
        public string Id { get; set; }
        public string NameOfInstitution { get; set; }
        public string ShortNameOfInstitution { get; set; }
        public string EducationOrganizationCategory { get; set; }
        public string OperationalStatus { get; set; }
        public string StateOrganizationId { get; set; }
        public string WebSite { get; set; }
        public string EducationServiceCenterId { get; set; }
        public string EducationOrganizationIdentificationCode { get; set; }
        public string EducationOrganizationIdentificationSystem { get; set; }
        public string AddressType { get; set; }
        public string StreetNumberName { get; set; }
        public string BuildingSiteNumber { get; set; }
        public string ApartmentRoomSuiteNumber { get; set; }
        public string City { get; set; }
        public string StateAbbreviation { get; set; }
        public string NameOfCounty { get; set; }
        public string PostalCode { get; set; }
        public string CountyFIPSCode { get; set; }
        public string InstitutionTelephoneNumberType { get; set; }
        public string TelephoneNumber { get; set; }
        public string StateEducationAgencyId { get; set; }
        public string StateEducationAgencyLink { get; set; }
        public string StateEducationAgencyIdentityId { get; set; }

        public static List<EducationServiceCenter> ReadFile()
        {
            string path = $"{CsvHelper.BasePath}{CsvHelper.EducationServiceCenterPath}";
            return CsvHelper.MapCsvToEntity<EducationServiceCenter, EducationSericeCenterMap>(path);
        }

        public static void WriteFile(List<EducationServiceCenter> records)
        {
            string path = $"{CsvHelper.BasePath}{CsvHelper.EducationServiceCenterPath}";
            CsvHelper.WriteCsv<EducationServiceCenter, EducationSericeCenterMap>(path, records);
        }
    }

    public class EducationSericeCenterMap : CsvClassMap<EducationServiceCenter>
    {
        public EducationSericeCenterMap()
        {
            Map(m => m.Id).Name("id");
            Map(m => m.NameOfInstitution).Name("NameOfInstitution");
            Map(m => m.ShortNameOfInstitution).Name("ShortNameOfInstitution");
            Map(m => m.EducationOrganizationCategory).Name("EducationOrganizationCategory");
            Map(m => m.OperationalStatus).Name("OperationalStatus");
            Map(m => m.StateOrganizationId).Name("StateOrganizationId");
            Map(m => m.WebSite).Name("WebSite");
            Map(m => m.EducationServiceCenterId).Name("EducationServiceCenterId");
            Map(m => m.EducationOrganizationIdentificationCode).Name("EducationOrganizationIdentificationCode.IdentificationCode");
            Map(m => m.EducationOrganizationIdentificationSystem).Name("EducationOrganizationIdentificationCode.EducationOrganizationIdentificationSystem");
            Map(m => m.AddressType).Name("Address.AddressType");
            Map(m => m.StreetNumberName).Name("Address.StreetNumberName");
            Map(m => m.BuildingSiteNumber).Name("Address.BuildingSiteNumber");
            Map(m => m.ApartmentRoomSuiteNumber).Name("Address.ApartmentRoomSuiteNumber");
            Map(m => m.City).Name("Address.City");
            Map(m => m.StateAbbreviation).Name("Address.StateAbbreviation");
            Map(m => m.NameOfCounty).Name("Address.NameOfCounty");
            Map(m => m.PostalCode).Name("Address.PostalCode");
            Map(m => m.CountyFIPSCode).Name("Address.CountyFIPSCode");
            Map(m => m.InstitutionTelephoneNumberType).Name("InstitutionTelephone.InstitutionTelephoneNumberType");
            Map(m => m.TelephoneNumber).Name("InstitutionTelephone.TelephoneNumber");
            Map(m => m.StateEducationAgencyId).Name("StateEducationAgencyReference.id");
            Map(m => m.StateEducationAgencyLink).Name("StateEducationAgencyReference.ref");
            Map(m => m.StateEducationAgencyIdentityId).Name("StateEducationAgencyReference.StateEducationAgencyIdentity.StateEducationAgencyId");
        }
    }
}
