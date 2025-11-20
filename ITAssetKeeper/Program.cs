using ITAssetKeeper.Data;
using ITAssetKeeper.Infrastructure.Identity;
using ITAssetKeeper.Models.Entities;
using ITAssetKeeper.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// DB接続処理
builder.Services.AddDbContext<ITAssetKeeperDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection") ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.")));

// Identityサービスの登録、エラー文の日本語変換処理も追加
builder.Services.AddIdentity<ApplicationUser, IdentityRole>()
    .AddErrorDescriber<JapaneseIdentityErrorDescriber>()
    .AddEntityFrameworkStores<ITAssetKeeperDbContext>()
    .AddDefaultTokenProviders();

// ユーザーID: ロックアウトの設定
builder.Services.Configure<IdentityOptions>(options =>
{
    // ロックアウト時間:15分
    // 失敗回数:5回まで
    // 新規ユーザーにもこれらの設定を適用
    options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(15);
    options.Lockout.MaxFailedAccessAttempts = 5;
    options.Lockout.AllowedForNewUsers = true;
});

// パスワードの設定
builder.Services.Configure<IdentityOptions>(options =>
{
    options.Password.RequireDigit = true;           // 0-9 の数値が必要
    options.Password.RequireLowercase = true;       // 小文字が必要
    options.Password.RequireNonAlphanumeric = true; // 英数字以外の文字が必要
    options.Password.RequireUppercase = true;       // 大文字が必要
    options.Password.RequiredLength = 8;            // 文字数の最低限の長さ(8文字)
    options.Password.RequiredUniqueChars = 4;       // 最低4種類の異なる文字が必要
});

// Serviceを登録
builder.Services.AddScoped<IDashboardService, DashboardService>();
builder.Services.AddScoped<IDeviceService, DeviceService>();
builder.Services.AddScoped<IDeviceHistoryService, DeviceHistoryService>();
builder.Services.AddScoped<IDeviceDiffService, DeviceDiffService>();
builder.Services.AddScoped<IDeviceHistorySequenceService, DeviceHistorySequenceService>();
builder.Services.AddScoped<IDeviceSequenceService, DeviceSequenceService>();

// Add services to the container.
builder.Services.AddControllersWithViews();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    //pattern: "{controller=Home}/{action=Index}/{id?}");
    pattern: "{controller=Account}/{action=Login}/{id?}");

// Seed Adminの作成
// サービススコープ内で実行する
using (var scope = app.Services.CreateScope())
{
    // Seed処理に必要なサービスを取得し、Seedメソッドを呼び出す
    var services = scope.ServiceProvider;
    var userManager = services.GetRequiredService<UserManager<ApplicationUser>>();
    var roleManager = services.GetRequiredService<RoleManager<IdentityRole>>();
    await DbInitializer.SeedAsync(userManager, roleManager);
}

// ManagementId,HistoryId採番の開始位置を調整
using (var scope = app.Services.CreateScope())
{
    var deviceService = scope.ServiceProvider.GetRequiredService<IDeviceService>();
    var historyService = scope.ServiceProvider.GetRequiredService<IDeviceHistoryService>();

    await deviceService.SyncDeviceSequenceAsync();
    await historyService.SyncHistorySequenceAsync();
}

app.Run();
