using System;
using LehrplanGenerator.Logic.Utils;
using LehrplanGenerator.Logic.Models;
using UserAlias = LehrplanGenerator.Logic.Models.User;

namespace LehrplanGenerator.Logic.Services;

public class AuthService
{
    private readonly UserCredentialStore _store;

    public UserAlias? CurrentUser { get; private set; }

    //public AuthService()
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
        CurrentUser = new UserAlias(user.UserId, user.FirstName, user.LastName);
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
