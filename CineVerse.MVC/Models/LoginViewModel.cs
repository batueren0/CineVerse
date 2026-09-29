namespace CineVerse.MVC.Models;

public sealed record LoginViewModel(
    string Email,
    string Password,
    bool RememberMe
    );
