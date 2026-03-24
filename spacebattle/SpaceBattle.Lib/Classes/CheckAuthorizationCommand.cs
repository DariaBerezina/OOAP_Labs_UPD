using Hwdtech;

namespace SpaceBattle.Lib;

public class CheckAuthorizationCommand : ICommand
{
    private readonly IUObject _order;

    public CheckAuthorizationCommand(IUObject order)
    {
        _order = order;
    }

    public void Execute()
    {
        var authInfo = IoC.Resolve<IAuthorizationInfo>("Adapter", typeof(IAuthorizationInfo), _order);
        var isAuthorized = IoC.Resolve<bool>("Game.Security.CheckAccess", authInfo.UserId, authInfo.GameItemId);
        if (!isAuthorized)
        {
            throw new UnauthorizedAccessException($"User {authInfo.UserId} is not authorized to control object {authInfo.GameItemId}");
        }
    }
}
