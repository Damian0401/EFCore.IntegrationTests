using Microsoft.EntityFrameworkCore;
using Api;
using Api.Models;
using Microsoft.AspNetCore.Mvc;

var builder = WebApplication.CreateBuilder(args);

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
ArgumentException.ThrowIfNullOrWhiteSpace(connectionString);

builder.Services.AddDbContext<DataContext>(options =>
{
    options.UseSqlServer(connectionString);
});

builder.Services.AddScoped<IBookService, BookService>();

var app = builder.Build();

app.MapGet("/books", async (
    [FromServices] IBookService service) => await service.GetAll());

app.MapGet("/books/{id}", async (
    [FromServices] IBookService service, 
    int id) => await service.GetById(id));

app.MapPost("/books", async (
    [FromServices] IBookService service, 
    BookModel book) => await service.Create(book));

app.MapPut("/books/{id}", async (
    [FromServices] IBookService service, 
    int id, 
    BookModel book) => await service.Update(id, book));

app.MapDelete("/books/{id}", async (
    [FromServices] IBookService service, 
    int id) => await service.Delete(id));

app.Run();

public partial class Program { }