using System.Text;
using HonestWork.Api.Features.Cv;
using QuestPDF.Fluent;

namespace HonestWork.Api.Tests;

public class CvDocumentTests
{
    [Fact]
    public void GeneratePdf_ReturnsPdfBytes()
    {
        var profile = new CvProfile
        {
            LanguageCode = "en",
            FullName = "Oskar Gruszczyński",
            Title = "Team Lead | Senior .NET Developer",
            Email = "oskar@example.com",
            WebsiteUrl = "https://example.com",
            WorkPreference = "Remote preferred",
            Skills = [new CvSkill { Category = "Languages & Frameworks", Name = "C#" }],
            Experiences = [new CvExperience { Company = "Acme", Role = "Developer", StartDate = new DateOnly(2020, 1, 1), Highlights = ["Built APIs"], Technologies = [".NET", "AWS"] }],
            Educations = [new CvEducation { Institution = "University", Degree = "Bachelor of Engineering", Thesis = "AI", StartDate = new DateOnly(2013, 1, 1), EndDate = new DateOnly(2017, 1, 1) }],
            Languages = [new CvLanguage { Name = "Polish", Level = "Native" }],
        };
        var labels = new CvLabels { Skills = "Skills", Experience = "Experience", Education = "Education", Languages = "Languages", Thesis = "Thesis", Technologies = "Tech", Present = "present" };
        var page = new CvPageOptions { WidthMm = 210, HeightMm = 297, Margin = 40, FontSize = 10.5f };

        var pdf = new CvDocument(profile, labels, page).GeneratePdf();

        Assert.StartsWith("%PDF", Encoding.ASCII.GetString(pdf, 0, 4));
    }
}
