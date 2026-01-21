using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LehrplanGenerator.Data.Repositories;
using LehrplanGenerator.Logic.Models;
using LehrplanGenerator.Logic.Utils;

namespace LehrplanGenerator.Logic.Services;

/// <summary>
/// Adapter um alte UserCredentialStore API mit neuer SQLite-basierter IUserRepository zu kombinieren
/// Ermöglicht dass bestehende ViewModels ohne Änderungen weiterarbeiten
/// ALLE Daten werden jetzt in SQLite3 gespeichert, nicht mehr in datenbank.json!
/// </summary>
public class UserCredentialStore
{
    private readonly IUserRepository _repository;

    public UserCredentialStore(IUserRepository repository)
    {
        _repository = repository;
    }

    public bool UsernameExists(string username)
    {
        // Synchrones Wrapper für async Methode
        return UsernameExistsAsync(username).GetAwaiter().GetResult();
    }

    public async Task<bool> UsernameExistsAsync(string username)
    {
        var user = await _repository.GetByUsernameAsync(username);
        return user != null;
    }

    public void Add(UserCredential user)
    {
        // Synchrones Wrapper für async Methode
        AddAsync(user).GetAwaiter().GetResult();
    }


    public async Task AddAsync(UserCredential user)
    {
        // Konvertiere UserCredential zu UserCredentialEntity für DB
        var entity = UserCredentialEntity.FromCredential(user);
        await _repository.AddAsync(entity);
    }

    public UserCredential? GetByUsername(string username)
    {
        // Synchrones Wrapper für async Methode
        return GetByUsernameAsync(username).GetAwaiter().GetResult();
    }

    public async Task<UserCredential?> GetByUsernameAsync(string username)
    {
        var entity = await _repository.GetByUsernameAsync(username);
        if (entity == null) return null;

        // Konvertiere UserCredentialEntity zu UserCredential
        return new UserCredential(
            entity.UserId,
            entity.FirstName,
            entity.LastName,
            entity.Username,
            entity.PasswordHash
        );
    }

    public UserCredential? GetById(Guid userId)
    {
        // Synchrones Wrapper für async Methode
        return GetByIdAsync(userId).GetAwaiter().GetResult();
    }

    public async Task<UserCredential?> GetByIdAsync(Guid userId)
    {
        var entity = await _repository.GetByUserIdAsync(userId);
        if (entity == null) return null;

        // Konvertiere UserCredentialEntity zu UserCredential
        return new UserCredential(
            entity.UserId,
            entity.FirstName,
            entity.LastName,
            entity.Username,
            entity.PasswordHash
        );
    }

    public void Update(UserCredential user)
    {
        // Synchrones Wrapper für async Methode
        UpdateAsync(user).GetAwaiter().GetResult();
    }

    public async Task UpdateAsync(UserCredential user)
    {
        // Konvertiere UserCredential zu UserCredentialEntity für DB
        var entity = UserCredentialEntity.FromCredential(user);
        await _repository.UpdateAsync(entity);
    }

    public IReadOnlyList<UserCredential> GetAll()
    {
        // Hinweis: Diese Methode ist deprecated - nutze async Version stattdessen
        throw new NotImplementedException("Nutze GetAllAsync stattdessen oder verwende die Repository direkt");
    }

    public void UpdateUsername(string oldUsername, string newUsername)
    {
        UpdateUsernameAsync(oldUsername, newUsername).GetAwaiter().GetResult();
    }

    public async Task UpdateUsernameAsync(string oldUsername, string newUsername)
    {
        var entity = await _repository.GetByUsernameAsync(oldUsername);
        if (entity == null)
            throw new InvalidOperationException("User not found.");

        entity.Username = newUsername;

        await _repository.UpdateAsync(entity);
    }


     public void UpdatePassword(string oldUsername, string newPassword)
    {
        UpdatePasswordAsync(oldUsername, newPassword).GetAwaiter().GetResult();
    }

    public async Task UpdatePasswordAsync(string oldUsername, string newPassword)
    {
        var entity = await _repository.GetByUsernameAsync(oldUsername);
        if (entity == null)
            throw new InvalidOperationException("User not found.");

        entity.PasswordHash = PasswordHasher.Hash(newPassword);

        await _repository.UpdateAsync(entity);
    }


}

