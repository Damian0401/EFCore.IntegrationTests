using Api;
using Api.Models;
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
        var response = await _systemUnderTest.GetById(id);
        
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
        var response = await _systemUnderTest.GetById(book.Id);
        
        // Assert
        Assert.IsType<Ok<BookModel>>(response.Result);

        var okResult = (Ok<BookModel>)response.Result;
        
        Assert.NotNull(okResult.Value);
        Assert.True(TestHelper.AllPropertiesAreEqual(book, okResult.Value));
    }

    [Fact]
    public async Task GetAll_ShouldReturnEmptyEnumerable_WhenNoBooksExist()
    {
        // Arrange

        // Act
        var response = await _systemUnderTest.GetAll();
        
        // Assert
        Assert.IsType<Ok<IEnumerable<BookModel>>>(response.Result);

        var okResult = (Ok<IEnumerable<BookModel>>)response.Result;

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
        var response = await _systemUnderTest.GetAll();
        
        // Assert
        Assert.IsType<Ok<IEnumerable<BookModel>>>(response.Result);
        
        var okResult = (Ok<IEnumerable<BookModel>>)response.Result;

        Assert.NotNull(okResult.Value);
        Assert.True(TestHelper.AllElementsAreEqual(books, okResult.Value));
    }

    [Fact]
    public async Task Create_ShouldCreateBook_WhenBookIsValid()
    {
        // Arrange
        var book = DataHelper.GetBookWithoutId();

        // Act
        var response = await _systemUnderTest.Create(book);
        
        // Assert
        Assert.IsType<Created<BookModel>>(response.Result);

        var createdResult = (Created<BookModel>)response.Result;

        Assert.NotNull(createdResult.Value);
        Assert.True(TestHelper.AllPropertiesAreEqual(book, createdResult.Value, ignoreProperties: [nameof(BookModel.Id)]));
    }

    [Fact]
    public async Task Update_ShouldReturnBadRequest_WhenBookDoesNotExist()
    {
        // Arrange
        var id = -1;
        var book = DataHelper.GetBookWithoutId();

        // Act
        var response = await _systemUnderTest.Update(id, book);

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
        updatedBook.Id = book.Id;

        // Act
        var response = await _systemUnderTest.Update(book.Id, updatedBook);

        // Assert
        Assert.IsType<NoContent>(response.Result);

        Context.Entry(book).Reload();

        Assert.True(TestHelper.AllPropertiesAreEqual(updatedBook, book));
    }

    [Fact]
    public async Task Delete_ShouldReturnBadRequest_WhenBookDoesNotExist()
    {
        // Arrange
        var id = -1;

        // Act
        var response = await _systemUnderTest.Delete(id);
        
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
        var response = await _systemUnderTest.Delete(book.Id);

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
