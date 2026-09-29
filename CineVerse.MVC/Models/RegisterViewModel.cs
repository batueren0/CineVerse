namespace CineVerse.MVC.Models;

public sealed record RegisterViewModel(
    string FirstName,
    string LastName,
    string UserName,
    DateOnly DoB,
    string Email,
    string Password
    );
