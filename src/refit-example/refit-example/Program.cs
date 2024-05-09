// See https://aka.ms/new-console-template for more information
using Refit;

var gitHubApi = RestService.For<IRandomUserAPI>("https://randomuser.me");
var result = await gitHubApi.GetUser();
Console.WriteLine(result.Results[0].Gender);

public interface IRandomUserAPI
{
    [Get("/api")]
    Task<User> GetUser();

    [Get("/api/")]
    Task<User> GetNationality(string nat);
}

public class User
{
    public List<UserInfo> Results { get; set; }
}

public class UserInfo
{
    public string Gender { get; set; }
    public UserName Name { get; set; }
}

public class UserName
{
    public string Title { get; set; }
    public string First { get; set; }
    public string Last { get; set; }
}