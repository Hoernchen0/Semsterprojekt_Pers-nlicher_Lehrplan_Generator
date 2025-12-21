using System;
using LehrplanGenerator.Logic.Utils;
using LehrplanGenerator.Logic.Models;

namespace LehrplanGenerator.Logic.Services;

public class AuthService
{
    private readonly UserCredentialStore _store;

    public AuthService(UserCredentialStore store)
    {
        _store = store;
    }
    public bool Register(string firstName, string lastName, string username, string password)
    {
        if (_store.UsernameExists(username))
            return false;

        var user = new UserCredential(
            Guid.NewGuid(),
            firstName,
            lastName,
            username,
            PasswordHasher.Hash(password)
        );

        _store.Add(user);
        return true;
    }
    public UserCredential? Login(string username, string password)
    {
        var cred = _store.GetByUsername(username);
        if (cred == null) return null;

        var hashed = PasswordHasher.Hash(password);
        return hashed == cred.PasswordHash ? cred : null;
    }
}
