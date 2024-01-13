using Api.Models;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;

namespace Api;

public interface IBookService 
{
    Task<Results<Ok<IEnumerable<BookModel>>, BadRequest>> GetAll();
    Task<Results<Ok<BookModel>, NotFound, BadRequest>> GetById(int id);
    Task<Results<Created<BookModel>, BadRequest>> Create(BookModel book);
    Task<Results<NoContent, BadRequest>> Update(int id, BookModel book);
    Task<Results<NoContent, BadRequest>> Delete(int id);
}

internal class BookService : IBookService
{
    private readonly DataContext _context;

    public BookService(DataContext context)
    {
        _context = context;
    }

    public async Task<Results<Created<BookModel>, BadRequest>> Create(BookModel book)
    {
        await _context.Books.AddAsync(book);
        await _context.SaveChangesAsync();
        return TypedResults.Created($"/books/{book.Id}", book);
    }

    public async Task<Results<NoContent, BadRequest>> Delete(int id)
    {
        if (await _context.Books.FindAsync(id) is not BookModel book)
        {
            return TypedResults.BadRequest();
        }

        _context.Books.Remove(book);
        await _context.SaveChangesAsync();
        return TypedResults.NoContent();
    }

    public async Task<Results<Ok<IEnumerable<BookModel>>, BadRequest>> GetAll()
    {
        var books = await _context.Books.ToListAsync();
        return TypedResults.Ok(books.AsEnumerable());
    }

    public async Task<Results<Ok<BookModel>, NotFound, BadRequest>> GetById(int id)
    {
        if (await _context.Books.FindAsync(id) is not BookModel book)
        {
            return TypedResults.NotFound();
        }

        return TypedResults.Ok(book);
    }

    public async Task<Results<NoContent, BadRequest>> Update(int id, BookModel book)
    {
        if (await _context.Books.FindAsync(id) is not BookModel bookToUpdate)
        {
            return TypedResults.BadRequest();
        }

        bookToUpdate.Title = book.Title;
        bookToUpdate.Description = book.Description;

        await _context.SaveChangesAsync();
        return TypedResults.NoContent();
    }
}
