namespace Api.Dtos;

public record class CreateBookDto
{
    public required string Title { get; init; } = default!;
    public required string Description { get; init; } = default!;
}
