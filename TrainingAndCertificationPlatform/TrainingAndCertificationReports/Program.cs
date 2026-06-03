var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

// HttpClient for calling the API (base URL comes from configuration so it can
// be overridden in Azure App Settings without a code change)
var apiBaseUrl = builder.Configuration["ApiBaseUrl"] ?? "https://localhost:7159";
builder.Services.AddHttpClient("ReportingApi", client =>
{
    client.BaseAddress = new Uri(apiBaseUrl);
});

// Session + HttpContextAccessor for storing token
builder.Services.AddSession();
builder.Services.AddHttpContextAccessor();

// Auth service that uses session
builder.Services.AddScoped<ApiAuthService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseSession();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Account}/{action=Login}/{id?}");

app.Run();