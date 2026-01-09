using AuthHW.Configuration;
using AuthHW.Extenssions;
using AuthHW.Middleware;
using AuthHW.Services;
using Microsoft.Extensions.Options;
using Minio;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddWebComponents();
builder.Services.AddDatabase(builder.Configuration);
builder.Services.AddAuthenticationWithJwt(builder.Configuration);
builder.Services.AddApplicationServices(); 


/*builder.Services.Configure<FileStorageOptions>(
    builder.Configuration.GetSection("Minio")
);

builder.Services.AddSingleton<IMinioClient>(sp =>
{
    var cfg = sp.GetRequiredService<IOptions<FileStorageOptions>>().Value;

    return new MinioClient()
        .WithEndpoint(cfg.Endpoint.Replace("http://", ""))
        .WithCredentials(cfg.AccessKey, cfg.SecretKey)
        .Build();
});


builder.Services.AddScoped<FileService>();*/

builder.Services.AddControllers(); 
builder.Services.AddSignalR();

builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
    {
        policy.AllowAnyHeader()
            .AllowAnyMethod()
            .AllowCredentials()
            .WithOrigins("http://localhost:3000");
    });
});

var app = builder.Build();

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseMiddleware<ExceptionHandlingMiddleware>();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.UseCors();
app.MapControllers();
app.MapGrpcService<GrpcAuthHandler>();

app.MapFallbackToFile("index.html");

await app.SetupDatabaseAsync();

app.Run();