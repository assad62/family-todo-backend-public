using System.Text;
using API.Helpers;
using API.Hubs;
using API.Middleware;
using AwsS3.Services;
using CorePush.Google;
using Entity;
using Entity.Interfaces;
using Infrastructure;
using Infrastructure.Repository;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Cors.Infrastructure;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using PushNotification.Models;
using PushNotification.Services;

namespace API
{
    public class Startup
    {
        private readonly IConfiguration _config;
        public Startup(IConfiguration config)
        {
            _config = config;
        }


        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {

            services.AddControllers().AddNewtonsoftJson();
            services.AddHttpClient();
            services.AddAutoMapper(typeof(ApplicationMapper));
            services.AddSignalR();
            services.AddTransient<ITaskRepository, UserTaskRepository>();
            services.AddScoped<IStorageService,StorageService>();
            services.AddTransient<IAccountRepository, AccountRepository>();
            services.AddTransient<IFamilyRepository, FamilyRepository>();
            services.TryAddSingleton<IHttpContextAccessor, HttpContextAccessor>();
            services.AddTransient<INotificationService, NotificationService>();
            services.AddHttpClient<FcmSender>();
            
            services.AddOptions();
            
            //push
            var appSettingsSection = _config.GetSection("FcmNotification");
            services.Configure<FcmNotificationSetting>(appSettingsSection);

            
            // Or you can also register as follows
            services.AddHttpContextAccessor();

            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "WebAPIv5", Version = "v1" });
            });
            services.AddDbContext<AppDataContext>(x =>
            {

                x.UseSqlite(_config.GetConnectionString("DefaultConnection"));

            });

            services.AddIdentity<ApplicationUser, IdentityRole>()
                .AddEntityFrameworkStores<AppDataContext>().AddDefaultTokenProviders();


            services.AddAuthentication(option =>
            {
                option.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                option.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                option.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
            }).AddJwtBearer(option =>
                {
                    option.SaveToken = true;
                    option.RequireHttpsMetadata = false;
                    option.TokenValidationParameters = new Microsoft.IdentityModel.Tokens.TokenValidationParameters()
                    {
                        ValidateIssuer = true,
                        ValidateAudience = true,
                        ValidAudience = _config["JWT:ValidAudience"],
                        ValidIssuer = _config["JWT:ValidIssuer"],
                        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["JWT:Secret"]))
                    };
                });

            services.AddCors(options => options.AddPolicy("CorsPolicy",
            builder =>
            {
                builder.AllowAnyHeader()
                       .AllowAnyMethod()
                       .SetIsOriginAllowed((host) => true)
                       .AllowCredentials();
            }));

        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {

            if (env.IsDevelopment())
            {
                //app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "WebAPIv5 v1"));
            }

            app.UseMiddleware<ExceptionMiddleware>();
            //app.UseHttpsRedirection();

            app.UseRouting();
            app.UseCors("CorsPolicy");

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
                endpoints.MapHub<UserTaskHub>("/usertask");
                endpoints.MapHub<NotificationHub>("/notificationHub");
            });
        }
    }
}