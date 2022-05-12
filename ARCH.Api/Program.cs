using Arch.Mongo.Managers;
using Arch.Mongo.Models;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.Configure<MongoDbSettings>(builder.Configuration.GetSection("MongoDbSettings"));

builder.Services.AddSingleton<IMongoDbSettings>(serviceProvider =>
    serviceProvider.GetRequiredService<IOptions<MongoDbSettings>>().Value);

builder.Services.AddScoped(typeof(IServiceManager<>), typeof(ServiceManager<>));


builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

//builder.Services.Configure<BookStoreDatabaseSettings>(
//    builder.Configuration.GetSection("BookStoreDatabase"));
//builder.Services.AddSingleton<BooksService>();

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                 .AddJwtBearer(options =>
                 {
                     options.TokenValidationParameters =
                         new TokenValidationParameters
                         {
                             ValidateAudience = false,
                             ValidateIssuer = false,
                             ValidateActor = false,
                             ValidateLifetime = true
                             //IssuerSigningKey = GestorOrdenadores.Service.Grpc.DotNet.Autenticacion.SecurityKey
                         };
                 });


var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
