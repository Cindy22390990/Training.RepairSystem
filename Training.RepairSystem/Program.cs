using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using NLog;
using NLog.Web;
using System.Text;
using Training.RepairSystem.Data;
using Training.RepairSystem.Models;
using Training.RepairSystem.Repositories;
using Training.RepairSystem.Repositories.Interfaces;
using Training.RepairSystem.Services;
using Training.RepairSystem.Services.Interfaces;


namespace Training.RepairSystem
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var logger = NLog.LogManager
                .Setup()
                .LoadConfigurationFromFile("nlog.config")
                .GetCurrentClassLogger();
            var builder = WebApplication.CreateBuilder(args);
            //加入 NLog
            builder.Logging.ClearProviders();

            builder.Host.UseNLog();
            // 加入 Controller 支援。
            builder.Services.AddControllers();
            // 加入 API Endpoint 探索服務。
            builder.Services.AddEndpointsApiExplorer();
            // 加入 Swagger 文件產生器。
            builder.Services.AddSwaggerGen(options =>
            {
                options.CustomSchemaIds(type => type.FullName);
                options.AddSecurityDefinition(
                    "Bearer",
                    new Microsoft.OpenApi.OpenApiSecurityScheme
                    {
                        Name = "Authorization",
                        Type = Microsoft.OpenApi.SecuritySchemeType.Http,
                        Scheme = "bearer",
                        BearerFormat = "JWT",
                        In = Microsoft.OpenApi.ParameterLocation.Header,
                        Description =
                            "輸入 JWT Token"
                    });


                options.AddSecurityRequirement(
               document =>
                {
                   return new Microsoft.OpenApi.OpenApiSecurityRequirement
                   {
                        {
                            new Microsoft.OpenApi.OpenApiSecuritySchemeReference(
                                "Bearer",
                                document
                            ),
                            new List<string>()
                        }
                   };
               });
            });
            // Database
            builder.Services.AddDbContext<AppDbContext>(options =>
            {
                options.UseSqlServer(
                    builder.Configuration.GetConnectionString("DefaultConnection")
                );
            });
            // 註冊資料庫服務。
            builder.Services
                .AddAuthentication(
                    JwtBearerDefaults.AuthenticationScheme
                )
                // 加入身份驗證功能。
                .AddJwtBearer(options =>
                {
                    // 設定 JWT 驗證規則。
                    options.TokenValidationParameters =
                        new TokenValidationParameters
                        {
                            // 驗證 Token 發行者。
                            ValidateIssuer = true,
                            // 驗證 Token 使用對象。
                            ValidateAudience = true,
                            // 驗證 Token 是否過期。
                            ValidateLifetime = true,
                            // 驗證 Token 簽章。
                            ValidateIssuerSigningKey = true,
                            // 取得允許的 Token 發行者。
                            ValidIssuer =
                                builder.Configuration["Jwt:Issuer"],
                            // 取得允許的 Token 使用者。
                            ValidAudience =
                                builder.Configuration["Jwt:Audience"],
                            // 建立 JWT 驗證金鑰。
                            IssuerSigningKey =
                                new SymmetricSecurityKey(
                                    Encoding.UTF8.GetBytes(
                                        builder.Configuration["Jwt:Key"]!
                                    )
                                )
                        };
                });

            // Services
            // 註冊帳號服務。
            builder.Services.AddScoped<IAccountService, AccountService>();
            // 註冊 TokenService。
            builder.Services.AddScoped<ITokenService, TokenService>();
            builder.Services.AddScoped<IRepairTicketRepository, RepairTicketRepository>();
            builder.Services.AddScoped<IRepairTicketService, RepairTicketService>();
            builder.Services.AddScoped<IPhotoService, PhotoService>();
            builder.Services.AddScoped<IPythonImageService,PythonImageService>();
            // 加入 OpenAPI 文件功能。
            builder.Services.AddOpenApi();
            // 根據前面設定建立 Web Application。
            var app = builder.Build();
            app.UseExceptionHandler("/error");
            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();

                app.UseSwaggerUI();
            }
            app.UseHttpsRedirection();
            app.UseStaticFiles();
            // 強制 HTTP 轉 HTTPS。
            app.UseAuthentication();
            // ⭐讀取 JWT Token。
            app.UseAuthorization();
            // 將 Controller 對應到 API 路由。
            app.MapControllers();
            // 啟動 Web API。
            app.Run();
        }
    }
}
