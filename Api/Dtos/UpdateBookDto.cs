namespace Api;

public record class UpdateBookDto
{
    public required string Title { get; init; } = default!;
    public required string Description { get; init; } = default!;
}
