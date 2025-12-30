using OpenFinance.Customers.CrossCutting;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddCrossCutting(builder.Configuration);

var app = builder.Build();
app.UseHttpsRedirection();
app.Run();