using CItyInfo.API;
using CItyInfo.API.DbContexts;
using CItyInfo.API.Services;
using Microsoft.AspNetCore.StaticFiles;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Reflection;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers(options =>
{
    options.ReturnHttpNotAcceptable = true;
})
.AddNewtonsoftJson()
.AddXmlDataContractSerializerFormatters();

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(setupAction =>
{
    //Optional - Only used for adding custom comments to the generated documentation
    var xmlCommentsFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
    var xmlCommentsFullPath = Path.Combine(AppContext.BaseDirectory, xmlCommentsFile);
    setupAction.IncludeXmlComments(xmlCommentsFullPath);

    //Optional - Config authentication in the Swagger UI
    setupAction.AddSecurityDefinition("CityInfoApiBearerAuth", new Microsoft.OpenApi.Models.OpenApiSecurityScheme()
    {
        Type = Microsoft.OpenApi.Models.SecuritySchemeType.Http,
        Scheme = "Bearer",
        Description = "Input a valid token to access this API"
    });
    setupAction.AddSecurityRequirement(new Microsoft.OpenApi.Models.OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "CityInfoApiBearerAuth"
                }
            }, new List<string>()
        }
    });
});

// Config FileExtensionContentTypeProvider 
builder.Services.AddSingleton<FileExtensionContentTypeProvider>();

// Add new Service
builder.Services.AddTransient<IMailService, LocalMailService>();
builder.Services.AddSingleton<CitiesDataStore>();
builder.Services.AddDbContext<CityInfoContext>(dbContextOptions => dbContextOptions.UseSqlite(
    builder.Configuration["ConnectionStrings:CityInfoDBConnctionsString"]));
builder.Services.AddScoped<ICityInfoRespository, CityInfoRespository>();

// Config Automapper
builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());

// Config Auth Bearer
builder.Services.AddAuthentication("Bearer").AddJwtBearer(options =>
{
    options.TokenValidationParameters = new()
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = builder.Configuration["Authentication:Issuer"],
        ValidAudience = builder.Configuration["Authentication:Audience"],
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(builder.Configuration["Authentication:SecretForKey"]))
    };
});

// Config Authorization Polices
builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("MustBeFromParis", policy =>
    {
        policy.RequireAuthenticatedUser();
        policy.RequireClaim("city", "Paris");
    });
});

// Config versionning
builder.Services.AddApiVersioning(setupAction =>
{
    setupAction.AssumeDefaultVersionWhenUnspecified = true;
    setupAction.DefaultApiVersion = new Microsoft.AspNetCore.Mvc.ApiVersion(1, 0);
    setupAction.ReportApiVersions = true;
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseRouting();

app.UseAuthentication();

app.UseAuthorization();

app.UseEndpoints(endpoints => endpoints.MapControllers());

app.Run();
