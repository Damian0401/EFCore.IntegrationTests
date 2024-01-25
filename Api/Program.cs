using Microsoft.EntityFrameworkCore;
using Api;
using Microsoft.AspNetCore.Mvc;
using Api.Dtos;

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
    [FromServices] IBookService service) => await service.GetAllAsync());

app.MapGet("/books/{id}", async (
    [FromServices] IBookService service, 
    int id) => await service.GetByIdAsync(id));

app.MapPost("/books", async (
    [FromServices] IBookService service, 
    CreateBookDto dto) => await service.CreateAsync(dto));

app.MapPut("/books/{id}", async (
    [FromServices] IBookService service, 
    int id, 
    UpdateBookDto dto) => await service.UpdateAsync(id, dto));

app.MapDelete("/books/{id}", async (
    [FromServices] IBookService service, 
    int id) => await service.DeleteAsync(id));

app.Run();

public partial class Program { }