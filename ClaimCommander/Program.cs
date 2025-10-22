using Microsoft.EntityFrameworkCore;
using ClaimCommander.Data; // Ensure this matches your namespace for DbContext and Initializer

var builder = WebApplication.CreateBuilder(args);

// --- Database Configuration ---
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(connectionString));

// --- Add services to the container ---
builder.Services.AddControllersWithViews();

// --- Add Session Services ---
builder.Services.AddDistributedMemoryCache(); // Required for session state
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30); // How long the session is active
    options.Cookie.HttpOnly = true; // Makes the cookie inaccessible to client-side script
    options.Cookie.IsEssential = true; // Required for GDPR compliance
});


var app = builder.Build();

// --- Seed the database ---
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    try // Added try-catch for safety during initialization
    {
        var context = services.GetRequiredService<ApplicationDbContext>();
        DbInitializer.Initialize(context); // Run your seeding logic
    }
    catch (Exception ex)
    {
        var logger = services.GetRequiredService<ILogger<Program>>();
        logger.LogError(ex, "An error occurred seeding the DB.");
    }
}

// --- Configure the HTTP request pipeline ---
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles(); // Serve static files like CSS, JS, images from wwwroot

app.UseRouting();

app.UseAuthorization();

app.UseSession(); // Enable session middleware

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Account}/{action=Login}/{id?}"); // Keep login as default

app.Run();