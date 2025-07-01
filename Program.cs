using HuynhNgocTien_SE18B01_A02.Models;
using HuynhNgocTien_SE18B01_A02.Repositories;
using HuynhNgocTien_SE18B01_A02.Repositories.Implement;
using HuynhNgocTien_SE18B01_A02.Services;
using HuynhNgocTien_SE18B01_A02.Services.Implement;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorPages();
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});
// Configure DbContext
builder.Services.AddDbContext<FunewsManagementContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// Register repositories
builder.Services.AddScoped<ISystemAccountRepository, SystemAccountRepository>();
builder.Services.AddScoped<INewsArticleRepository, NewsArticleRepository>();
builder.Services.AddScoped<ICategoryRepository, CategoryRepository>();
builder.Services.AddScoped<ITagRepository, TagRepository>();

// Register services
builder.Services.AddScoped<ISystemAccountService, SystemAccountService>();
builder.Services.AddScoped<INewsArticleService, NewsArticleService>();
builder.Services.AddScoped<ICategoryService, CategoryService>();

// Register DbInitializer
builder.Services.AddScoped<DbInitializer>();

var app = builder.Build();
// Initialize database and seed data
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    try
    {
        var context = services.GetRequiredService<FunewsManagementContext>();
        var initializer = services.GetRequiredService<DbInitializer>();
        await initializer.InitializeAsync();

        // Seed default admin if it doesn't exist
        var defaultAdmin = builder.Configuration.GetSection("DefaultAdmin");
        var adminEmail = defaultAdmin["Email"];
        var logger = services.GetRequiredService<ILogger<Program>>();
        
        logger.LogInformation("Checking for DefaultAdmin with email: {Email}", adminEmail);
        
        if (!await context.SystemAccounts.AnyAsync(x => x.AccountEmail == adminEmail))
        {
            logger.LogInformation("DefaultAdmin not found, creating new admin account");
            try 
            {
                var maxId = await context.SystemAccounts.MaxAsync(x => (int?)x.AccountId) ?? 0;
                logger.LogInformation("Current max AccountId: {MaxId}", maxId);
                
                var adminAccount = new SystemAccount
                {
                    AccountId = (short)(maxId + 1),
                    AccountEmail = adminEmail,
                    AccountPassword = defaultAdmin["Password"],
                    AccountRole = short.Parse(defaultAdmin["Role"] ?? "3"),
                    AccountName = "System Administrator"
                };

                await context.SystemAccounts.AddAsync(adminAccount);
                await context.SaveChangesAsync();
                logger.LogInformation("DefaultAdmin created successfully with ID: {Id}", adminAccount.AccountId);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error creating DefaultAdmin account");
                throw;
            }
        }
        else
        {
            logger.LogInformation("DefaultAdmin already exists");
        }
    }
    catch (Exception ex)
    {
        var logger = services.GetRequiredService<ILogger<Program>>();
        logger.LogError(ex, "An error occurred while initializing the database.");
    }
}
// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();
app.UseSession();

app.UseAuthorization();

app.MapRazorPages();

app.Run();
