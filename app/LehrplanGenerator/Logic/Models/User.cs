using System;

namespace LehrplanGenerator.Logic.Models;

public record User(
    Guid UserId,
    string Name
);