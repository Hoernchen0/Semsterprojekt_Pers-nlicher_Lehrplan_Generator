using System;

namespace LehrplanGenerator.Logic.Models;

public record UserCredential(
    Guid UserId,
    string Username,
    string PasswordHash
);
