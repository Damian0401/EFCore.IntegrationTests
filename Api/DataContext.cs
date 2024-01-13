using Api.Models;
using Microsoft.EntityFrameworkCore;

namespace Api;

public class DataContext : DbContext
{
    public DataContext(DbContextOptions<DataContext> options) : base(options)
    {
        
    }

    public DbSet<BookModel> Books { get; set; } = default!;
}
