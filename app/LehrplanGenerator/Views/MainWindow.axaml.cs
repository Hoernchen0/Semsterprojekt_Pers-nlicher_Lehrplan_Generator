using Avalonia.Controls;
using LehrplanGenerator.Logic.Models;
using System;
using System.Collections.Generic;

namespace LehrplanGenerator.Views;

public partial class LegacyMainWindow : Window
{
    public LegacyMainWindow()
    {
        InitializeComponent();


        //Testmodel 
        var user = new UserModel("TestUser");
        Console.WriteLine(user);
    }
}
