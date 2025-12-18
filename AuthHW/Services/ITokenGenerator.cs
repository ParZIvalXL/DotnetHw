namespace AuthHW.Services;

public interface ITokenGenerator
{
    string CreateToken(string username, string issuer, string audience, string secretKey, int validityMinutes);
}