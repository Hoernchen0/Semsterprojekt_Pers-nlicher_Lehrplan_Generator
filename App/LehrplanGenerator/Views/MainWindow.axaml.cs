using Avalonia.Controls;
using LehrplanGenerator.Logic.Models;
using System;
using System.Collections.Generic;

namespace LehrplanGenerator.Views;

public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();


        //Testmodel 
        var user = new UserModel("TestUser");
        Console.WriteLine(user);
    }
}
