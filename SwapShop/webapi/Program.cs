using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Text;
using webapi.Data;
using webapi.Models;
using webapi.Repositories;
using webapi.Services.Authentication;



var builder = WebApplication.CreateBuilder(args);
AddAuthentication();
AddIdentity();
ConfigureSwagger();

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<ITokenService, TokenService>();
builder.Services.AddTransient<IProduct, ProductService>();
builder.Services.AddTransient<IUser, UserService>();
builder.Services.AddDbContext<DataContext>();




/*builder.Services.AddDbContext<DataContext>(options =>
{
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"));
});*/


var app = builder.Build();
AddRoles();
AddAdmin();

// Configure CORS
app.UseCors(builder =>
{
    builder.AllowAnyOrigin(); // You can replace this with specific origins
    builder.AllowAnyHeader();
    builder.AllowAnyMethod();
});
// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();

void AddAuthentication()
{

    builder.Services
        .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
        .AddJwtBearer(options =>
        {
            options.TokenValidationParameters = new TokenValidationParameters()
            {
                ClockSkew = TimeSpan.Zero,
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateLifetime = true,
                ValidateIssuerSigningKey = true,
                ValidIssuer = "apiWithAuthBackend",
                ValidAudience = "apiWithAuthBackend",
                IssuerSigningKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes("!SomethingSecret!")
                ),
            };
        });
}
void AddIdentity()
{
    builder.Services
     .AddIdentityCore<IdentityUser>(options =>
     {
         options.SignIn.RequireConfirmedAccount = false;
         options.User.RequireUniqueEmail = true;
         options.Password.RequireDigit = false;
         options.Password.RequiredLength = 6;
         options.Password.RequireNonAlphanumeric = false;
         options.Password.RequireUppercase = false;
         options.Password.RequireLowercase = false;
     })
     .AddRoles<IdentityRole>()
     .AddEntityFrameworkStores<DataContext>();
}
void ConfigureSwagger()
{
    builder.Services.AddSwaggerGen(option =>
    {
        option.SwaggerDoc("v1", new OpenApiInfo { Title = "Demo API", Version = "v1" });
        option.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
        {
            In = ParameterLocation.Header,
            Description = "Please enter a valid token",
            Name = "Authorization",
            Type = SecuritySchemeType.Http,
            BearerFormat = "JWT",
            Scheme = "Bearer"
        });
        option.AddSecurityRequirement(new OpenApiSecurityRequirement
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


}
void AddRoles()
{
    using var scope = app.Services.CreateScope(); // RoleManager is a scoped service, therefore we need a scope instance to access it
    var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();

    var tAdmin = CreateAdminRole(roleManager);
    tAdmin.Wait();

    var tUser = CreateUserRole(roleManager);
    tUser.Wait();
}

async Task CreateAdminRole(RoleManager<IdentityRole> roleManager)
{
    await roleManager.CreateAsync(new IdentityRole("Admin")); //The role string should better be stored as a constant or a value in appsettings
}

async Task CreateUserRole(RoleManager<IdentityRole> roleManager)
{
    await roleManager.CreateAsync(new IdentityRole("User")); //The role string should better be stored as a constant or a value in appsettings
}
void AddAdmin()
{
    var tAdmin = CreateAdminIfNotExists();
    tAdmin.Wait();
}

async Task CreateAdminIfNotExists()
{
    using var scope = app.Services.CreateScope();
    var userManager = scope.ServiceProvider.GetRequiredService<UserManager<IdentityUser>>();
    var adminInDb = await userManager.FindByEmailAsync("admin@admin1.com");
    if (adminInDb == null)
    {
        var admin = new IdentityUser { UserName = "admin1", Email = "admin@admin1.com" };
        var adminCreated = await userManager.CreateAsync(admin, "admin1234");

        if (adminCreated.Succeeded)
        {
            await userManager.AddToRoleAsync(admin, "Admin");
            var customUser = new User
            {
                UserName = admin.UserName,
                Email = admin.Email,
                IdentityUserId = admin.Id
            };
            using var dbContext = scope.ServiceProvider.GetRequiredService<DataContext>();
            dbContext.Users.Add(customUser);
            await dbContext.SaveChangesAsync();

        }
    }


}

public partial class Program { };