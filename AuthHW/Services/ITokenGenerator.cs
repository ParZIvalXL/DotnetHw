namespace AuthHW.Services;

public interface ITokenGenerator
{
    string CreateToken(string username, int id, string issuer, string audience, string secretKey, int validityMinutes);
}