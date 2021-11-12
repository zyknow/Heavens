namespace Heavens.Application.AuthorizeApp.Dtos;

public class LoginInput
{
    public string Account { get; set; }
    public string Passwd { get; set; }
    public bool KeepAlive { get; set; } = false;
    public LoginType LoginType { get; set; } = LoginType.AccountPassword;
    public LoginClientType LoginClientType { get; set; } = LoginClientType.Browser;

    public string Value { get; set; }
}
