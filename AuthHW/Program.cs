using AuthHW.Extenssions;
using AuthHW.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddWebComponents();
builder.Services.AddDatabase(builder.Configuration);
builder.Services.AddAuthenticationWithJwt(builder.Configuration);
builder.Services.AddApplicationServices();

var app = builder.Build();

app.UseDevelopmentComponents();
app.UseSecurityComponents();
app.MapControllers();
app.MapGrpcService<GrpcAuthHandler>();

await app.SetupDatabaseAsync();
app.Run();