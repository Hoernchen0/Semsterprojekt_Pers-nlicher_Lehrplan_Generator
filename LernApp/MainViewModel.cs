using System.Collections.Generic;
using System.Linq;

public class MainViewModel
{
    public List<User> Users { get; set; }

    public MainViewModel()
    {
        using var db = new LernAppDbConnection();
        db.Database.EnsureCreated();
        Users = db.Users.ToList();
    }
}
