var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

// HttpClient for calling the API
builder.Services.AddHttpClient("ReportingApi", client =>
{
    client.BaseAddress = new Uri("https://localhost:7159"); // API base URL
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