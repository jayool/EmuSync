using FluentValidation;
using System.Text.Json.Serialization;

namespace EmuSync.Agent.Dto.Game;

public record QuickAddRequestBodyDto
{
    [JsonPropertyName("games")]
    public List<QuickAddGameDto> Games { get; set; }
}

public class QuickAddRequestBodyDtoValidator : AbstractValidator<QuickAddRequestBodyDto>
{
    public QuickAddRequestBodyDtoValidator()
    {
        RuleFor(x => x.Games).NotEmpty();
        RuleForEach(x=> x.Games).SetValidator(new QuickAddGameDtoValidator());
    }
}

public record QuickAddGameDto
{
    [JsonPropertyName("existingGameId")]
    public string? ExistingGameId { get; set; }

    [JsonPropertyName("path")]
    public string Path { get; set; }

    //when a new game is being created
    [JsonPropertyName("gameName")]
    public string? GameName { get; set; }

    [JsonPropertyName("autoSync")]
    public bool AutoSync { get; set; }

    [JsonPropertyName("maximumLocalGameBackups")]
    public int? MaximumLocalGameBackups { get; set; }
}

public class QuickAddGameDtoValidator : AbstractValidator<QuickAddGameDto>
{
    public QuickAddGameDtoValidator()
    {
        RuleFor(x => x.GameName).MaximumLength(255);
        RuleFor(x => x.MaximumLocalGameBackups).GreaterThan(-1).When(x => x.MaximumLocalGameBackups != null);

        // ===== EXISTING GAME =====
        When(x => x.ExistingGameId != null, () =>
        {
            RuleFor(x => x.GameName)
                .Must(string.IsNullOrWhiteSpace)
                .WithMessage("Game name must be empty when an existing game is selected.");
        });

        // ===== NEW GAME =====
        When(x => x.ExistingGameId == null, () =>
        {
            RuleFor(x => x.GameName)
                .NotEmpty()
                .WithMessage("Game name is required when creating a new game.");
        });
    }
}