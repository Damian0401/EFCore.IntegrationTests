using Api.Dtos;
using Api.Models;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;

namespace Api;

public interface IBookService 
{
    Task<Results<Ok<IEnumerable<GetBookDto>>, BadRequest>> GetAllAsync(CancellationToken ct = default);
    Task<Results<Ok<GetBookDetailsDto>, NotFound, BadRequest>> GetByIdAsync(int id, CancellationToken ct = default);
    Task<Results<Created<CreatedBookDto>, BadRequest>> CreateAsync(CreateBookDto book, CancellationToken ct = default);
    Task<Results<NoContent, BadRequest>> UpdateAsync(int id, UpdateBookDto book, CancellationToken ct = default);
    Task<Results<NoContent, BadRequest>> DeleteAsync(int id, CancellationToken ct = default);
}

internal class BookService : IBookService
{
    private readonly DataContext _context;

    public BookService(DataContext context)
    {
        _context = context;
    }

    public async Task<Results<Created<CreatedBookDto>, BadRequest>> CreateAsync(
        CreateBookDto book, 
        CancellationToken ct = default)
    {
        var bookModel = new BookModel
        {
            Title = book.Title,
            Description = book.Description
        };

        await _context.Books.AddAsync(bookModel);
        await _context.SaveChangesAsync(ct);

        var createdBook = new CreatedBookDto
        {
            Id = bookModel.Id,
            Title = bookModel.Title,
            Description = bookModel.Description
        };

        return TypedResults.Created($"/books/{createdBook.Id}", createdBook);
    }

    public async Task<Results<NoContent, BadRequest>> DeleteAsync(
        int id, 
        CancellationToken ct = default)
    {
        if (await _context.Books.FindAsync(id) is not BookModel book)
        {
            return TypedResults.BadRequest();
        }

        _context.Books.Remove(book);
        await _context.SaveChangesAsync(ct);
        return TypedResults.NoContent();
    }

    public async Task<Results<Ok<IEnumerable<GetBookDto>>, BadRequest>> GetAllAsync(
        CancellationToken ct = default)
    {
        var books = await _context.Books
            .Select(book => new GetBookDto
            {
                Id = book.Id,
                Title = book.Title,
                Description = book.Description
            })
            .ToListAsync(ct);

        return TypedResults.Ok(books.AsEnumerable());
    }

    public async Task<Results<Ok<GetBookDetailsDto>, NotFound, BadRequest>> GetByIdAsync(
        int id, 
        CancellationToken ct = default)
    {
        var book = await _context.Books
            .Select(book => new GetBookDetailsDto
            {
                Id = book.Id,
                Title = book.Title,
                Description = book.Description
            })
            .FirstOrDefaultAsync(book => book.Id == id, ct);

        if (book is null)
        {
            return TypedResults.NotFound();
        }

        return TypedResults.Ok(book);
    }

    public async Task<Results<NoContent, BadRequest>> UpdateAsync(
        int id, 
        UpdateBookDto book, 
        CancellationToken ct = default)
    {
        if (await _context.Books.FindAsync(id, ct) is not BookModel bookToUpdate)
        {
            return TypedResults.BadRequest();
        }

        bookToUpdate.Title = book.Title;
        bookToUpdate.Description = book.Description;

        await _context.SaveChangesAsync(ct);
        return TypedResults.NoContent();
    }
}
