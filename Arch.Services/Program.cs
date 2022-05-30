using Arch.CoreLibrary.Utils.Security;
using Arch.Mongo.Managers;
using Arch.Mongo.Models;
using Arch.Services.Bootstrappers;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Text;

var builder = WebApplication.CreateBuilder(args);
ConfigurationManager configuration = builder.Configuration;

// Add services to the container.
builder.Services.Configure<MongoDbSettings>(builder.Configuration.GetSection("BookStore"));
builder.Services.AddScoped<IMongoDbSettings>(serviceProvider =>
    serviceProvider.GetRequiredService<IOptions<MongoDbSettings>>().Value);

builder.Services.Configure<MongoDbSettings>(builder.Configuration.GetSection("LogManagement"));
builder.Services.AddScoped<IMongoDbSettings>(serviceProvider =>
    serviceProvider.GetRequiredService<IOptions<MongoDbSettings>>().Value);


builder.Services.AddScoped(typeof(IGenericRepository<>), typeof(GenericRepository<>));
builder.Services.AddScoped(typeof(IServiceManager), typeof(ServiceManager));
builder.Services.AddScoped(typeof(IMemoryCache), typeof(MemoryCache));


builder.Services.AddControllers().AddJsonOptions(options =>
options.JsonSerializerOptions.PropertyNamingPolicy = null
); ;
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "Demo API", Version = "v1" });
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        In = ParameterLocation.Header,
        Description = "Please enter a valid token",
        Name = "Authorization",
        Type = SecuritySchemeType.ApiKey,
        BearerFormat = "JWT",
        Scheme = "Bearer"
    });
    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type=ReferenceType.SecurityScheme,
                    Id="Bearer"
                }
            },
            new string[]{}
        }
    });
});
// Adding Jwt Bearer
//Auth.AddJwtConfig(builder.Services, configuration);

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                 .AddJwtBearer(options =>
                 {
                     var secretKey = new CryptoUtils().DecryptString(configuration["Application:Secret"]);
                     var key = Encoding.ASCII.GetBytes(secretKey);
                     options.TokenValidationParameters =
                         new TokenValidationParameters
                         {
                             ValidateIssuer = true,
                             ValidateAudience = true,
                             ValidateLifetime = true,
                             ValidAudience = configuration["Application:Audience"],
                             ValidIssuer = configuration["Application:Issuer"],
                             IssuerSigningKey = new SymmetricSecurityKey(key)
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

// Authentication & Authorization
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
//app.Run(async context =>
//{
//    context.Response.StatusCode = 404;
//    await context.Response.WriteAsync($"ALONET |{Configuration["Application:Name"]}|").ConfigureAwait(false);
//});
