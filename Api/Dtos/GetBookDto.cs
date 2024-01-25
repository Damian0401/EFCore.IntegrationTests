namespace Api.Dtos;

public record class GetBookDto
{
    public required int Id { get; init; }
    public required string Title { get; init; } = default!;
    public required string Description { get; init; } = default!;
}
