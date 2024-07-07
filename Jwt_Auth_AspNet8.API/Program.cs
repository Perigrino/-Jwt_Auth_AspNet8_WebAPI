using System.Text;
using System.Text.Json.Serialization;
using Jwt_Auth_AspNet8.API;
using Jwt_Auth_AspNet8.Application.Data;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Http.Json;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddApplication();
builder.Services.AddDbContext<AppDbContext>(options => options.UseNpgsql(builder.Configuration.GetConnectionString("Default")));
builder.Services.AddControllers().AddNewtonsoftJson(options => options.SerializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore); 
builder.Services.Configure<JsonOptions>(options => { options.SerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles; });

builder.Services.AddAuthorization();
//builder.Services.AddIdentityApiEndpoints<IdentityUser>().AddEntityFrameworkStores<AppDbContext>();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();
// Add Identity
builder.Services
    .AddIdentity<IdentityUser, IdentityRole>()
    .AddEntityFrameworkStores<AppDbContext>()
    .AddDefaultTokenProviders();

// Config Identity
builder.Services.Configure<IdentityOptions>(options =>
{
    options.Password.RequiredLength = 8;
    options.Password.RequireDigit = false;
    options.Password.RequireLowercase = false;
    options.Password.RequireNonAlphanumeric = false;
    options.Password.RequireUppercase = false;
    options.Password.RequiredUniqueChars = 0;
    options.SignIn.RequireConfirmedEmail = false;
});
    

// Add Authentication and JwtBearer
builder.Services
    .AddAuthentication(options =>
    {
        options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
        options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
        options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    })
    .AddJwtBearer(options =>
    {
        options.SaveToken = true;
        options.RequireHttpsMetadata = false;
        options.TokenValidationParameters = new Microsoft.IdentityModel.Tokens.TokenValidationParameters()
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidIssuer = builder.Configuration["JWT: ValidIssuer"],
            ValidAudience = builder.Configuration["JWT: ValidAudience"],
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["JWT: SecretKey"]))
        };
    });


// Configure the HTTP request pipeline.
// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.MapIdentityApi<IdentityUser>();
app.UseHttpsRedirection();
app.UseRouting();
app.UseAuthentication();
app.UseEndpoints(endpoints => { endpoints.MapControllers(); });

app.Run();