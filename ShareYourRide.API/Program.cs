using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Caching.StackExchangeRedis;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using StackExchange.Redis;
using Microsoft.IdentityModel.Tokens;
using ShareYourRide.Infrastructure.Data;
using ShareYourRide.Infrastructure.Identity;
using ShareYourRide.Infrastructure.Repositories.Interfaces;
using ShareYourRide.Infrastructure.Services.Interfaces;
using ShareYourRide.Infrastructure.Services.Implementations;

namespace ShareYourRide.API
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            builder.Services.AddControllers();

            builder.Services.AddDbContext<AppDbContext>(opt =>
            opt.UseSqlServer(
                builder.Configuration.GetConnectionString("DefaultConnection"),
                sqlOptions => sqlOptions.EnableRetryOnFailure(
                    maxRetryCount: 5,
                    maxRetryDelay: TimeSpan.FromSeconds(10),
                    errorNumbersToAdd: null)
            ));

            builder.Services.AddCors(options =>
            {
                options.AddPolicy("AllowTester", policy =>
                    policy.AllowAnyOrigin()
                          .AllowAnyMethod()
                          .AllowAnyHeader()
                          .WithExposedHeaders("X-Session-Id"));
            });

            builder.Services.AddIdentity<ApplicationUser, IdentityRole<Guid>>(options =>
            {
                options.Password.RequiredLength = 6;
                options.Password.RequireNonAlphanumeric = false;
                options.User.RequireUniqueEmail = true;
            })
                .AddEntityFrameworkStores<AppDbContext>()
                .AddDefaultTokenProviders();

            // JWT Authentication
            var jwtSection = builder.Configuration.GetSection("Jwt");
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
                        ValidIssuer = jwtSection["Issuer"],
                        ValidAudience = jwtSection["Audience"],
                        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSection["Key"]!))
                    };
                });

            builder.Services.AddScoped<IUnitOfWork, ShareYourRide.Infrastructure.Repositories.Implementations.UnitOfWork>();
            builder.Services.AddScoped<ITokenService, TokenService>();
            builder.Services.AddScoped<IAuthService, AuthService>();
            builder.Services.AddScoped<ISmsSender, ConsoleSmsSender>();
            builder.Services.AddScoped<IEmailSender, ConsoleEmailSender>();
            builder.Services.AddScoped<IStopService, ShareYourRide.Infrastructure.Services.Implementations.StopService>();
            builder.Services.AddScoped<ITrajectoryService, ShareYourRide.Infrastructure.Services.Implementations.TrajectoryService>();
            builder.Services.AddScoped<IAdminService, ShareYourRide.Infrastructure.Services.Implementations.AdminService>();
            builder.Services.AddScoped<IRideApplicationService, ShareYourRide.Infrastructure.Services.Implementations.RideApplicationService>();
            builder.Services.AddScoped<IWalletService, ShareYourRide.Infrastructure.Services.Implementations.WalletService>();
            builder.Services.AddScoped<IFileStorageService, AzureBlobStorageService>();
            builder.Services.AddScoped<IEmailSender, ShareYourRide.Infrastructure.Services.Implementations.SmtpEmailSender>();
            builder.Services.AddScoped<IUserService, ShareYourRide.Infrastructure.Services.Implementations.UserService>();
            builder.Services.AddScoped<IVehicleCatalogService, ShareYourRide.Infrastructure.Services.Implementations.VehicleCatalogService>();
            builder.Services.AddScoped<IReviewService, ShareYourRide.Infrastructure.Services.Implementations.ReviewService>();
            builder.Services.AddScoped<IChatService, ShareYourRide.Infrastructure.Services.Implementations.ChatService>();

            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen(options =>
            {
                options.AddSecurityDefinition("Bearer", new Microsoft.OpenApi.Models.OpenApiSecurityScheme
                {
                    Name = "Authorization",
                    Type = Microsoft.OpenApi.Models.SecuritySchemeType.Http,
                    Scheme = "Bearer",
                    BearerFormat = "JWT",
                    In = Microsoft.OpenApi.Models.ParameterLocation.Header,
                    Description = "JWT token-i belə daxil et: Bearer {token}"
                });

                options.AddSecurityRequirement(new Microsoft.OpenApi.Models.OpenApiSecurityRequirement
                {
                    {
                        new Microsoft.OpenApi.Models.OpenApiSecurityScheme
                        {
                            Reference = new Microsoft.OpenApi.Models.OpenApiReference
                            {
                                Type = Microsoft.OpenApi.Models.ReferenceType.SecurityScheme,
                                Id = "Bearer"
                            }
                        },
                        Array.Empty<string>()
                    }
                });
            });

            var redisConnection = builder.Configuration.GetConnectionString("Redis");
            if (string.IsNullOrWhiteSpace(redisConnection))
            {
                builder.Services.AddDistributedMemoryCache();
            }
            else
            {
                var redisOptions = ConfigurationOptions.Parse(redisConnection);
                redisOptions.AbortOnConnectFail = false;
                redisOptions.ConnectTimeout = 5000;
                redisOptions.SyncTimeout = 5000;
                redisOptions.ConnectRetry = 2;

                var redisPassword = builder.Configuration["Redis:Password"];
                if (!string.IsNullOrWhiteSpace(redisPassword))
                    redisOptions.Password = redisPassword;

                var redisUser = builder.Configuration["Redis:User"];
                if (!string.IsNullOrWhiteSpace(redisUser))
                    redisOptions.User = redisUser;

                builder.Services.AddSingleton<IDistributedCache>(sp =>
                {
                    var redis = new RedisCache(Options.Create(new RedisCacheOptions
                    {
                        ConfigurationOptions = redisOptions,
                        InstanceName = "SYR_"
                    }));
                    var memory = new MemoryDistributedCache(Options.Create(new MemoryDistributedCacheOptions()));
                    return new ResilientDistributedCache(
                        redis, memory, sp.GetRequiredService<ILogger<ResilientDistributedCache>>());
                });
            }

            var app = builder.Build();


            // Configure the HTTP request pipeline.
            app.UseSwagger();
            app.UseSwaggerUI();

            app.UseCors("AllowTester");
            app.UseStaticFiles();
            app.UseHttpsRedirection();
            app.UseAuthentication();
            app.UseAuthorization();
            app.MapControllers();
            app.MapGet("/", () => Results.Redirect("/swagger")).ExcludeFromDescription();


            using (var scope = app.Services.CreateScope())
            {
                var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole<Guid>>>();
                await ShareYourRide.Infrastructure.Data.Seed.RoleSeeder.SeedRolesAsync(roleManager);

                var userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();
                var unitOfWork = scope.ServiceProvider.GetRequiredService<IUnitOfWork>();
                await ShareYourRide.Infrastructure.Data.Seed.AdminSeeder.SeedAdminAsync(userManager, unitOfWork, app.Configuration);

                var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
                await ShareYourRide.Infrastructure.Data.Seed.StopSeeder.SeedStopAsync(dbContext);

                await ShareYourRide.Infrastructure.Data.Seed.VehicleCatalogSeeder.SeedAsync(dbContext);
            }

            app.Run();
        }
    }
}