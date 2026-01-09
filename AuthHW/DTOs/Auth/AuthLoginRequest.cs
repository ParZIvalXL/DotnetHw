namespace AuthHW.DTOs;

public sealed record CredentialsRequest(string TagOrEmail, string Password, bool rememberMe);