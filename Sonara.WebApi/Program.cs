using Hangfire;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Sonara.CoreLayer;
using Sonara.CoreLayer.Entities;
using Sonara.CoreLayer.Interfaces;
using Sonara.DataAccessLayer.Context;
using Sonara.DataAccessLayer.Repositories.Implementations;
using Sonara.DataAccessLayer.Repositories.Interfaces;
using Sonara.WebApi;
using Sonara.WebApi.BackgroundJobs;
using Sonara.WebApi.Services;
using System.Security.Claims;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddScoped<IGenericDal<Artist>, GenericDal<Artist>>(); 
builder.Services.AddScoped<ISongDal, SongDal>();
builder.Services.AddScoped<IArtistDal, ArtistDal>();
builder.Services.AddScoped<IAlbumDal, AlbumDal>();
builder.Services.AddScoped<IUserMembershipDal, UserMembershipDal>();
builder.Services.AddScoped<IMembershipPlanDal, MembershipPlanDal>();
builder.Services.AddScoped<ITokenService, TokenService>();
builder.Services.AddScoped<IDeviceSessionDal, DeviceSessionDal>();
builder.Services.AddScoped<IDashboardDal, DashboardDal>();
builder.Services.AddScoped<IPlaylistDal, PlaylistDal>();
builder.Services.AddScoped<IPlaybackHistoryDal, PlaybackHistoryDal>();
builder.Services.AddScoped<IBlobStorageService, BlobStorageService>();
builder.Services.AddDbContext<SonaraDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));


builder.Services.AddIdentity<ApplicationUser, IdentityRole>(options =>
{
    options.Password.RequiredLength = 6;
    options.Password.RequireNonAlphanumeric = false;
    options.Lockout.MaxFailedAccessAttempts = 5;
    options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(10);
    options.Lockout.AllowedForNewUsers = true;
})
.AddEntityFrameworkStores<SonaraDbContext>()
.AddDefaultTokenProviders();


builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,

        ValidIssuer = builder.Configuration["JwtSettings:Issuer"],
        ValidAudience = builder.Configuration["JwtSettings:Audience"],
        
        IssuerSigningKey = new SymmetricSecurityKey(
            Encoding.UTF8.GetBytes(builder.Configuration["JwtSettings:Key"]!)),

        RoleClaimType = ClaimTypes.Role
    };

    options.Events = new JwtBearerEvents
    {
        OnAuthenticationFailed = context =>
        {
            Console.WriteLine("AUTH ERROR");
            Console.WriteLine(context.Exception.ToString());
            return Task.CompletedTask;
        }
    };
});

builder.Services.AddAutoMapper(cfg => { }, typeof(Program).Assembly);

builder.Services.AddControllers();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowWebUI", policy =>
    {
        policy.WithOrigins("https://localhost:PORT") 
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});
builder.Services.AddHangfire(configuration =>
{
    configuration.UseSqlServerStorage(builder.Configuration.GetConnectionString("DefaultConnection"));
});
builder.Services.AddHangfireServer(); 

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHangfireDashboard("/hangfire");
app.UseHttpsRedirection();
app.UseCors("AllowWebUI");
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

RecurringJob.AddOrUpdate<MembershipExpirationJob>(
    "membership-expiration-check",
    job => job.RunAsync(),
    Cron.Daily(3)
);
using (var scope = app.Services.CreateScope())
{
    await SeedData.SeedAsync(scope.ServiceProvider);
}
app.Run();