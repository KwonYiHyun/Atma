using Empen;
using Empen.Middleware;
using Empen.Service;
using Empen.Service.IService;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Serilog;
using CloudStructures;
using System.Text;
using Empen.Worker;
using ServerCore.Extensions;
using ServerCore.Middleware;

var builder = WebApplication.CreateBuilder(args);

// Blazor 제거
//builder.Services.AddRazorComponents()
//    .AddInteractiveServerComponents();
builder.Services.AddRazorPages();

builder.Services.AddMemoryCache();

// Add DB
//builder.Services.AddDbContext<MasterDbContext>(options =>
//    options.UseSqlServer(builder.Configuration.GetConnectionString("MasterConnection")));
//builder.Services.AddDbContext<PersonDbContext>(options =>
//    options.UseSqlServer(builder.Configuration.GetConnectionString("PersonConnection")));

// Add DBFactory (DataCacheService, CachePrewarmingService)
builder.Services.AddDbContextFactory<MasterDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("MasterConnection")));
builder.Services.AddDbContextFactory<PersonDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("PersonConnection")));
// API 컨트롤러가 DbContext를 요청하면 Factory에서 꺼내줌
builder.Services.AddScoped<MasterDbContext>(p =>
    p.GetRequiredService<IDbContextFactory<MasterDbContext>>().CreateDbContext());
builder.Services.AddScoped<PersonDbContext>(p =>
    p.GetRequiredService<IDbContextFactory<PersonDbContext>>().CreateDbContext());

builder.Services.AddInfrastructure(builder.Configuration);

// Add Singleton
builder.Services.AddSingleton<IMasterDataCacheService, MasterDataCacheService>();
builder.Services.AddSingleton<CacheMetricsService>();

// Add Service
builder.Services.AddScoped<IPersonDataCacheService, PersonDataCacheService>();
builder.Services.AddScoped<IGachaService, GachaService>();
builder.Services.AddScoped<IMailService, MailService>();
builder.Services.AddScoped<ILoginbonusService, LoginbonusService>();
builder.Services.AddScoped<ICharacterService, CharacterService>();
builder.Services.AddScoped<IBannerService, BannerService>();
builder.Services.AddScoped<IPersonService, PersonService>();
builder.Services.AddScoped<INoticeService, NoticeService>();
//builder.Services.AddScoped<IFriendService, FriendService>();
builder.Services.AddScoped<IProductService, ProductService>();
//builder.Services.AddScoped<IStoryService, StoryService>();
builder.Services.AddScoped<IAchievementService, AchievementService>();
builder.Services.AddScoped<IItemService, ItemService>();
//builder.Services.AddScoped<IExchangeProductService, ExchangeProductService>();
builder.Services.AddScoped<IResourceService, ResourceService>();
builder.Services.AddScoped<IRewardService, RewardService>();
builder.Services.AddScoped<IAdminService, AdminService>();

// Add Hosted Service
builder.Services.AddHostedService<CachePrewarmingService>();
builder.Services.AddHostedService<MasterDataSyncWorker>();

// Add Web API
builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.IncludeFields = true;
        options.JsonSerializerOptions.PropertyNameCaseInsensitive = true;
    });

// Add Jwt
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

        ValidIssuer = builder.Configuration["Jwt:Issuer"],
        ValidAudience = builder.Configuration["Jwt:Audience"],
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]))
    };
});

// Serilog
Log.Logger = new LoggerConfiguration()
    .ReadFrom.Configuration(builder.Configuration)
    .Enrich.FromLogContext()
    .WriteTo.Console()
    .WriteTo.File("Logs/log-.txt",
    rollingInterval: RollingInterval.Day,
    retainedFileCountLimit: 30)
    .CreateLogger();
builder.Host.UseSerilog();

var app = builder.Build();

// Add Docker DB Setting
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    var logger = services.GetRequiredService<ILogger<Program>>();

    int retryCount = 0;
    while (retryCount < 10)
    {
        try
        {
            logger.LogInformation($"[DB 접속 시도 {retryCount + 1}/10]...");

            // MasterDBContext
            var masterContext = services.GetRequiredService<MasterDbContext>();
            masterContext.Database.Migrate();

            // PersonDbContext
            var personContext = services.GetRequiredService<PersonDbContext>();
            personContext.Database.Migrate();

            logger.LogInformation("DB 및 테이블 생성 완료!");
            break;
        }
        catch (Exception ex)
        {
            retryCount++;
            logger.LogWarning($"실패: {ex.GetType().Name} - {ex.Message}");
            logger.LogWarning("3초 후 재시도...");
            System.Threading.Thread.Sleep(3000);
        }
    }
}

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    app.UseHsts();
}
else
{
    app.UseDeveloperExceptionPage();
}

app.UseMaintenanceCheck();

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

//app.UseMiddleware<RequestOneCheckMiddleware>();
app.UseRequestOneCheck();

// Blazor 제거
//app.UseAntiforgery();
app.MapStaticAssets();
//app.MapRazorComponents<App>()
//    .AddInteractiveServerRenderMode();
app.MapRazorPages();

app.MapControllers(); // Web API 컨트롤러 매핑

app.UseSerilogRequestLogging();

app.Run();
