using System;

namespace LehrplanGenerator.Logic.Models;

public record UserCredential(
    Guid UserId,
    string FirstName,
    string LastName,
    string Username,
    string PasswordHash
);