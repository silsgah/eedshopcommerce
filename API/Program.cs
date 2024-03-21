using Microsoft.EntityFrameworkCore;
using Infrastructure;
using Entity.Interfaces;
using API.Helpers;
using API.Middlewear;
using Microsoft.AspNetCore.Mvc;
using API.ErrorResponse;
using Entity;
using Microsoft.AspNetCore.Identity;
using Infrastructure.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;

var builder = WebApplication.CreateBuilder(args);
var MyAllowSpecificOrigins = "_myAllowSpecificOrigins";
// Add services to the container.
builder.Services.AddCors(options =>
{
    options.AddPolicy(name: MyAllowSpecificOrigins,
                      policy =>
                      {
                          policy.AllowAnyHeader().AllowAnyMethod().AllowCredentials();
                          policy.WithOrigins("http://localhost:3000");
                      });
});
builder.Services.Configure<ApiBehaviorOptions>(options =>
{
    options.InvalidModelStateResponseFactory = actionContext =>
    {
        var errors = actionContext.ModelState
            .Where(e => e.Value.Errors.Count > 0)
            .SelectMany(x => x.Value.Errors)
            .Select(x => x.ErrorMessage).ToArray();

        var errorResponse = new ApiValidationErrorResponse
        {
            Errors = errors
        };

        return new BadRequestObjectResult(errorResponse);
    };
    //
});
builder.Services.AddIdentityCore<User>().AddRoles<IdentityRole>().AddEntityFrameworkStores<StoreContext>();
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(opt =>
                {
                    opt.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuer = false,
                        ValidateAudience = false,
                        ValidateLifetime = true,
                        ValidateIssuerSigningKey = true,
                        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["JWT:TokenKey"]))
                    };
                });
builder.Services.AddAuthorization();
builder.Services.AddScoped<PaymentService>();
builder.Services.AddScoped<TokenService>();
builder.Services.AddControllers();
builder.Services.AddScoped<ICourseRepository, CourseRepository>();
builder.Services.AddScoped<ICategoryRepository, CategoryRepository>();
builder.Services.AddScoped(typeof(IGenericRepository<>), (typeof(GenericRepository<>)));
builder.Services.AddAutoMapper(typeof(MappingProfiles));
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddDbContext<StoreContext>(x =>
{
    var env = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");

    string dbConnection;

    if (env == "Development")
    {
        // Reading value from appsettings.Development.json
        dbConnection = (builder.Configuration.GetConnectionString("DefaultConnection"));
    }
    else
    {
        // Reading Value from Heroku Environment variables.
        var connectionUrl = Environment.GetEnvironmentVariable("DATABASE_URL");

        // extracting values from the connectionURL
        connectionUrl = connectionUrl.Replace("postgres://", string.Empty);
        var pgUserPass = connectionUrl.Split("@")[0];
        var pgHostPortDb = connectionUrl.Split("@")[1];
        var pgHostPort = pgHostPortDb.Split("/")[0];
        var pgDb = pgHostPortDb.Split("/")[1];
        var pgUser = pgUserPass.Split(":")[0];
        var pgPass = pgUserPass.Split(":")[1];
        var pgHost = pgHostPort.Split(":")[0];
        var pgPort = pgHostPort.Split(":")[1];

        dbConnection = $"Server={pgHost};Port={pgPort};User Id={pgUser};Password={pgPass};Database={pgDb};SSL Mode=Require;Trust Server Certificate=true";
    }

    // Using dbConnection string either from Development or production
    x.UseNpgsql(dbConnection);
});
builder.Services.AddSwaggerGen();
//var connectionString = builder.Configuration.GetConnectionString("DefaultConn");

//builder.Services.AddDbContext<StoreContext>(x =>
//{
//    x.UseNpgsql(connectionString,
//      x => x.UseQuerySplittingBehavior(QuerySplittingBehavior.SplitQuery));
//});
//builder.Services.AddDbContext<StoreContext>(x =>
//{
//    x.UseSqlServer(connectionString,
//      x => x.UseQuerySplittingBehavior(QuerySplittingBehavior.SplitQuery));
//});
var app = builder.Build();
app.UseDefaultFiles();
app.UseStaticFiles();
using var scope = app.Services.CreateScope();
var services = scope.ServiceProvider;
var logger = services.GetRequiredService<ILogger<Program>>();
try
{
    var userManager = scope.ServiceProvider.GetRequiredService<UserManager<User>>();
    var context = services.GetRequiredService<StoreContext>();
    context.Database.Migrate();
    await StoreContextSeed.SeedAsync(context, logger, userManager);
}
catch (Exception ex)
{
    logger.LogError(ex, "Something wrong happened during migration");
}


app.UseMiddleware<ExceptionMiddleware>();
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "API v1"));
}
app.UseRouting();

app.UseCors("CorsPolicy");

app.UseStatusCodePagesWithReExecute("/redirect/{0}");
app.UseHttpsRedirection();
app.UseCors(MyAllowSpecificOrigins);
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();
app.MapFallbackToFile("/index.html");
app.MapFallbackToController("Index", "Fallback");
app.Run();
