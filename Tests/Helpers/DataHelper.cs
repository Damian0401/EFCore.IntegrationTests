using Api.Models;
using Bogus;

namespace Tests;

public class DataHelper
{
    private static Faker<BookModel>? _bookFaker;

    public static BookModel GetBookWithoutId()
    {
        if (_bookFaker is null)
        {
            _bookFaker = new Faker<BookModel>()
                .RuleFor(b => b.Title, f => f.Lorem.Sentence())
                .RuleFor(b => b.Description, f => f.Lorem.Paragraph());
        }

        return _bookFaker.Generate();
    }

    public static IEnumerable<BookModel> GetBooksWithoutId(int count)
    {
        for (var i = 0; i < count; i++)
        {
            yield return GetBookWithoutId();
        }
    }
}
