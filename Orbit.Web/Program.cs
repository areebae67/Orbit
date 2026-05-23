using Orbit.Infrastructure.DependencyInjection;
using Orbit.Infrastructure.Pdf;

var builder = WebApplication.CreateBuilder(args);

//
// 🔧 SERVICES
//

// MVC + API Controllers
builder.Services.AddControllersWithViews();
builder.Services.AddControllers(); // IMPORTANT for API endpoints

// Swagger (API documentation)
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Clean Architecture DI
builder.Services.AddInfrastructure(builder.Configuration);

// PDF service
builder.Services.AddScoped<PdfExtractorService>();

var app = builder.Build();

//
// 🌐 HTTP PIPELINE
//

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
else
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

//
// 📌 ROUTING
//

// API routes
app.MapControllers();

// MVC routes (views)
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}"
);
app.MapGet("/test", () => "API is working");
app.Run();