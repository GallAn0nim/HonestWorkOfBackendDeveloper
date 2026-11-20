using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

namespace HonestWork.Api.Features.Cv;

public class CvDocument(CvProfile profile, CvLabels labels, CvPageOptions pageOptions) : IDocument
{
    private const char NonBreakingSpace = ' ';

    static CvDocument()
    {
        QuestPDF.Settings.License = LicenseType.Community;
    }

    public DocumentMetadata GetMetadata()
    {
        return new() { Title = $"{profile.FullName} - Curriculum Vitae" };
    }

    public void Compose(IDocumentContainer container)
    {
        container.Page(page =>
        {
            page.Size(new PageSize(pageOptions.WidthMm, pageOptions.HeightMm, Unit.Millimetre));
            page.Margin(pageOptions.Margin);
            page.DefaultTextStyle(style => style.FontSize(pageOptions.FontSize));
            page.Content().Column(column =>
            {
                column.Spacing(16);
                column.Item().Element(Header);

                if (profile.Experiences.Count > 0)
                {
                    column.Item().Element(container => Section(container, labels.Experience, Experiences));
                }

                if (profile.Educations.Count > 0)
                {
                    column.Item().Element(container => Section(container, labels.Education, Educations));
                }

                column.Item().ShowEntire().Row(row =>
                {
                    row.Spacing(24);

                    if (profile.Skills.Count > 0)
                    {
                        row.RelativeItem(3).Element(container => Section(container, labels.Skills, Skills));
                    }

                    if (profile.Languages.Count > 0)
                    {
                        row.RelativeItem().Element(container => Section(container, labels.Languages, Languages));
                    }
                });
            });
        });
    }

    private void Header(IContainer container)
    {
        container.Column(column =>
        {
            column.Spacing(4);
            column.Item().Text(profile.FullName).FontSize(26).SemiBold();
            column.Item().Text(profile.Title).FontSize(13).FontColor(Colors.Grey.Darken2);
            column.Item().Text(text =>
            {
                text.Hyperlink(profile.Email, $"mailto:{profile.Email}");
                text.Span("  ·  ");
                text.Hyperlink(WebsiteLabel(), profile.WebsiteUrl);
                text.Span("  ·  ");
                text.Span(profile.WorkPreference);
            });
        });
    }

    private void Skills(IContainer container)
    {
        container.Column(column =>
        {
            column.Spacing(2);

            foreach (var category in profile.Skills.GroupBy(skill => skill.Category))
            {
                column.Item().Text(text =>
                {
                    text.Span($"{category.Key}: ").SemiBold();
                    text.Span(string.Join(" · ", category.Select(skill => skill.Name.Replace(' ', NonBreakingSpace))));
                });
            }
        });
    }

    private void Experiences(IContainer container)
    {
        container.Column(column =>
        {
            column.Spacing(12);

            foreach (var company in profile.Experiences.GroupBy(experience => experience.Company))
            {
                var end = company.Any(experience => experience.EndDate is null) ? null : company.Max(experience => experience.EndDate);
                var summary = company.Select(experience => experience.CompanySummary).FirstOrDefault(text => text is not null);

                column.Item().Column(entry =>
                {
                    entry.Spacing(8);
                    entry.Item().Column(header =>
                    {
                        header.Spacing(2);
                        header.Item().Text(text =>
                        {
                            text.Span(company.Key).FontSize(12).SemiBold();
                            text.Span($"   {Period(company.Min(experience => experience.StartDate), end)}").FontColor(Colors.Grey.Darken1);
                        });

                        if (summary is not null)
                        {
                            header.Item().Text(summary).Italic().FontColor(Colors.Grey.Darken3);
                        }
                    });

                    foreach (var experience in company)
                    {
                        entry.Item().Element(container => Role(container, experience));
                    }
                });
            }
        });
    }

    private void Role(IContainer container, CvExperience experience)
    {
        container.Column(column =>
        {
            column.Spacing(3);
            column.Item().Text(text =>
            {
                text.Span(experience.Role).SemiBold();
                text.Span($"   {Period(experience.StartDate, experience.EndDate)}").FontColor(Colors.Grey.Darken1);
            });

            foreach (var highlight in experience.Highlights)
            {
                column.Item().PaddingLeft(4).Row(row =>
                {
                    row.ConstantItem(12).Text("•");
                    row.RelativeItem().Text(highlight);
                });
            }

            if (experience.Technologies.Length > 0)
            {
                column.Item().PaddingLeft(4).Text($"{labels.Technologies}: {string.Join(" · ", experience.Technologies)}").FontColor(Colors.Grey.Darken3);
            }
        });
    }

    private void Educations(IContainer container)
    {
        container.Column(column =>
        {
            column.Spacing(12);

            foreach (var education in profile.Educations)
            {
                column.Item().Column(entry =>
                {
                    entry.Item().Text(text =>
                    {
                        text.Span(education.Institution).SemiBold();
                        text.Span($"   {Period(education.StartDate, education.EndDate)}").FontColor(Colors.Grey.Darken1);
                    });
                    entry.Item().Text(string.Join(" · ", new[] { education.Degree, education.Field }.Where(part => part is not null)));

                    if (education.Thesis is not null)
                    {
                        entry.Item().Text($"{labels.Thesis}: {education.Thesis}");
                    }
                });
            }
        });
    }

    private void Languages(IContainer container)
    {
        container.Column(column =>
        {
            column.Spacing(4);

            foreach (var language in profile.Languages)
            {
                column.Item().Text(text =>
                {
                    text.Line(language.Name).SemiBold();
                    text.Span(language.Level).FontColor(Colors.Grey.Darken2);
                });
            }
        });
    }

    private string WebsiteLabel()
    {
        var url = new Uri(profile.WebsiteUrl);

        return Uri.UnescapeDataString($"{url.Host.Replace("www.", "")}{url.AbsolutePath.TrimEnd('/')}");
    }

    private string Period(DateOnly start, DateOnly? end)
    {
        return $"{start.Year} – {end?.Year.ToString() ?? labels.Present}";
    }

    private static void Section(IContainer container, string title, Action<IContainer> body)
    {
        container.EnsureSpace(60).Column(column =>
        {
            column.Spacing(6);
            column.Item().Text(title.ToUpperInvariant()).FontSize(11).SemiBold().LetterSpacing(0.08f);
            column.Item().LineHorizontal(0.5f).LineColor(Colors.Grey.Medium);
            column.Item().Element(body);
        });
    }
}
