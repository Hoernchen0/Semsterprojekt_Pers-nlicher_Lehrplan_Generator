using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;

class Program
{
    static void Main()
    {
        using var db = new LernAppDbConnection();
        db.Database.EnsureCreated();

        // CREATE
        db.Users.Add(new User { Name = "Dennis", LernzeitProTag = 67 });
        db.SaveChanges();

        // READ
        foreach (var u in db.Users)
            Console.WriteLine($"{u.Id}: {u.Name} ({u.LernzeitProTag} Min)");

        // UPDATE
        var user = db.Users.First();
        user.LernzeitProTag += 30;
        db.SaveChanges();

        // DELETE
        db.Users.Remove(user);
        db.SaveChanges();
    }
}
