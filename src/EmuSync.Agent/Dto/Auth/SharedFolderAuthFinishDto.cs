using FluentValidation;
using System.Text.Json.Serialization;

namespace EmuSync.Agent.Dto.Auth;

public class SharedFolderAuthFinishDto
{
    [JsonPropertyName("path")]
    public string Path { get; set; }

    [JsonPropertyName("username")]
    public string? Username { get; set; }

    [JsonPropertyName("password")]
    public string? Password { get; set; }
}


public class SharedFolderAuthFinishDtoValidator : AbstractValidator<SharedFolderAuthFinishDto>
{
    public SharedFolderAuthFinishDtoValidator()
    {
        RuleFor(x => x.Path).NotEmpty();

        When(x => !string.IsNullOrEmpty(x.Username) || !string.IsNullOrEmpty(x.Password), () =>
        {
            RuleFor(x => x.Username).NotEmpty().WithMessage("Username is required when password is provided");
            RuleFor(x => x.Password).NotEmpty().WithMessage("Password is required when username is provided");
        });
    }
}