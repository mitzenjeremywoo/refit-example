using Refit;

var builder = WebApplication.CreateBuilder(args);
builder.Logging.AddConsole();

IHttpClientBuilder refitClientBuilder = builder.Services.AddRefitClient<IRandomUserAPI>()
  .ConfigureHttpClient(httpClient =>
  {
      httpClient.BaseAddress = new Uri("https://randomuser.me"); 
  });

//// Adding our new handler here
refitClientBuilder.AddHttpMessageHandler(serviceProvider
  => new HttpLoggingHandler(serviceProvider.GetRequiredService<ILogger<HttpLoggingHandler>>()));
refitClientBuilder.Services.AddSingleton<HttpLoggingHandler>();

// doésn't work for me 
//refitClientBuilder.AddHttpMessageHandler<HttpLoggingHandler>();

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.MapControllers();
app.UseAuthorization();

app.MapGet("/hello", async (IRandomUserAPI client) =>
{
    // no don't try this - use DI instead
    //var randomClient = RestService.For<IRandomUserAPI>("https://randomuser.me");
       
    var result = await client.GetUser();
    Console.WriteLine(result.Results[0].Gender);
    return result;

});

app.Run();

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

public class HttpLoggingHandler(ILogger<HttpLoggingHandler> logger) : DelegatingHandler
{
    protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        Guid id = Guid.NewGuid();
        HttpResponseMessage response = await base.SendAsync(request, cancellationToken).ConfigureAwait(false);
   
        logger.LogInformation("[{Id}] Request: {Request}", id, request);
        logger.LogInformation("[{Id}] Response: {Response}", id, response);
        return response;
    }
}