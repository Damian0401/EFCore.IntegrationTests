using Api;
using Api.Dtos;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Tests.Helpers;
using Tests.IntegrationTests.Utils;

namespace Tests.IntegrationTests;

public class BooksIntegrationTests : BaseIntegrationTest
{
    private readonly IBookService _systemUnderTest;

    public BooksIntegrationTests(IntegrationTestWebAppFactory<Program, DataContext> factory) 
        : base(factory)
    {
        _systemUnderTest = Scope.ServiceProvider
            .GetRequiredService<IBookService>();
    }

    [Fact]
    public async Task GetById_ShouldReturnNotFound_WhenBookDoesNotExist()
    {
        // Arrange
        var id = -1;

        // Act
        var response = await _systemUnderTest.GetByIdAsync(id);
        
        // Assert
        Assert.IsType<NotFound>(response.Result);
    }

    [Fact]
    public async Task GetById_ShouldReturnBook_WhenBookExists()
    {
        // Arrange
        var book = DataHelper.GetBookWithoutId();

        await Context.Books.AddAsync(book);
        await Context.SaveChangesAsync();

        // Act
        var response = await _systemUnderTest.GetByIdAsync(book.Id);
        
        // Assert
        Assert.IsType<Ok<GetBookDetailsDto>>(response.Result);

        var okResult = (Ok<GetBookDetailsDto>)response.Result;
        
        Assert.NotNull(okResult.Value);
        Assert.True(TestHelper.AllPropertiesAreEqual(book, okResult.Value));
    }

    [Fact]
    public async Task GetAll_ShouldReturnEmptyEnumerable_WhenNoBooksExist()
    {
        // Arrange

        // Act
        var response = await _systemUnderTest.GetAllAsync();
        
        // Assert
        Assert.IsType<Ok<IEnumerable<GetBookDto>>>(response.Result);

        var okResult = (Ok<IEnumerable<GetBookDto>>)response.Result;

        Assert.NotNull(okResult.Value);
        Assert.Empty(okResult.Value);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(5)]
    [InlineData(10)]
    public async Task GetAll_ShouldReturnEnumerableOfBooks_WhenBooksExist(int bookNumber)
    {
        // Arrange
        var books = DataHelper.GetBooksWithoutId(bookNumber);

        await Context.Books.AddRangeAsync(books);
        await Context.SaveChangesAsync();

        // Act
        var response = await _systemUnderTest.GetAllAsync();
        
        // Assert
        Assert.IsType<Ok<IEnumerable<GetBookDto>>>(response.Result);
        
        var okResult = (Ok<IEnumerable<GetBookDto>>)response.Result;

        Assert.NotNull(okResult.Value);
        Assert.True(TestHelper.AllElementsAreEqual(books, okResult.Value));
    }

    [Fact]
    public async Task Create_ShouldCreateBook_WhenBookIsValid()
    {
        // Arrange
        var book = DataHelper.GetBookWithoutId();
        var dto = new CreateBookDto
        {
            Title = book.Title,
            Description = book.Description
        };

        // Act
        var response = await _systemUnderTest.CreateAsync(dto);
        
        // Assert
        Assert.IsType<Created<CreatedBookDto>>(response.Result);

        var createdResult = (Created<CreatedBookDto>)response.Result;

        Assert.NotNull(createdResult.Value);
        Assert.True(TestHelper.AllPropertiesAreEqual(dto, createdResult.Value));
    }

    [Fact]
    public async Task Update_ShouldReturnBadRequest_WhenBookDoesNotExist()
    {
        // Arrange
        var id = -1;
        var book = DataHelper.GetBookWithoutId();
        var dto = new UpdateBookDto
        {
            Title = book.Title,
            Description = book.Description
        };

        // Act
        var response = await _systemUnderTest.UpdateAsync(id, dto);

        // Assert
        Assert.IsType<BadRequest>(response.Result);
    }

    [Fact]
    public async Task Update_ShouldUpdateBook_WhenBookExists()
    {
        // Arrange
        var book = DataHelper.GetBookWithoutId();

        await Context.Books.AddAsync(book);
        await Context.SaveChangesAsync();

        var updatedBook = DataHelper.GetBookWithoutId();
        var dto = new UpdateBookDto
        {
            Title = updatedBook.Title,
            Description = updatedBook.Description
        };

        // Act
        var response = await _systemUnderTest.UpdateAsync(book.Id, dto);

        // Assert
        Assert.IsType<NoContent>(response.Result);

        Context.Entry(book).Reload();

        Assert.True(TestHelper.AllPropertiesAreEqual(dto, book));
    }

    [Fact]
    public async Task Delete_ShouldReturnBadRequest_WhenBookDoesNotExist()
    {
        // Arrange
        var id = -1;

        // Act
        var response = await _systemUnderTest.DeleteAsync(id);
        
        // Assert
        Assert.IsType<BadRequest>(response.Result);
    }

    [Fact]
    public async Task Delete_ShouldDeleteBook_WhenBookExists()
    {
        // Arrange
        var book = DataHelper.GetBookWithoutId();

        await Context.Books.AddAsync(book);
        await Context.SaveChangesAsync();

        // Act
        var response = await _systemUnderTest.DeleteAsync(book.Id);

        // Assert
        Assert.IsType<NoContent>(response.Result);

        Context.Entry(book).Reload();
        var bookExists = await Context.Books.AnyAsync(b => b.Id == book.Id);

        Assert.False(bookExists);
    }

    public override void Dispose()
    {
        Context.Books.RemoveRange(Context.Books);
        Context.SaveChanges();

        base.Dispose();
    }
}
