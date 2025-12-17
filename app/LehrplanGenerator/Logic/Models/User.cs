using System;

namespace LehrplanGenerator.Logic.Models;

public record User(
    Guid UserId,
    string FirstName,
    string LastName
)
{
    public string DisplayName => $"{FirstName} {LastName}";
}
