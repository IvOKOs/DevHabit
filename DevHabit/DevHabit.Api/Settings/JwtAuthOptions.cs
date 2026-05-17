namespace DevHabit.Api.Settings;

public sealed class JwtAuthOptions
{
    public string Issuer { get; init; }// who created the token(only trusted sources)
    public string Audience { get; init; }// who the token is for(only trusted sources)
    public string Key { get; init; } // for signing jwt
    public int ExpirationInMinutes { get; init; }
    public int RefreshTokenExpirationDays { get; init; }
}
