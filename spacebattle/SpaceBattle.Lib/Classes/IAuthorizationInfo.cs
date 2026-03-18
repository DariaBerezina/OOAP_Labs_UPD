namespace SpaceBattle.Lib;

public interface IAuthorizationInfo
{
    string UserId { get; }
    string GameItemId { get; }
}
