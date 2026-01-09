namespace AuthHW.DTOs;

public sealed record RegisterCredentialsRequest(
    string Username, 
    string Tag, 
    string Email, 
    string Password
    );