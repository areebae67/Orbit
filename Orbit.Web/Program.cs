using Orbit.Infrastructure.DependencyInjection;
using Orbit.Application.DependencyInjection;
using Orbit.Infrastructure.Pdf;
using Orbit.Application.Services.Parsing;

var builder = WebApplication.CreateBuilder(args);

//
// 🔧 SERVICES
//

// MVC + API Controllers
builder.Services.AddControllersWithViews();
builder.Services.AddControllers();

// Swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Clean Architecture DI
builder.Services.AddApplication();
builder.Services.AddInfrastructure(builder.Configuration);

// CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll",
        policy =>
        {
            policy
                .AllowAnyOrigin()
                .AllowAnyHeader()
                .AllowAnyMethod();
        });
});

// PDF Service
builder.Services.AddScoped<PdfExtractorService>();

//
// 🚀 BUILD APP
//

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

app.UseHttpsRedirection();

app.UseStaticFiles();

app.UseRouting();

// ✅ CORS MUST BE HERE
app.UseCors("AllowAll");

app.UseAuthorization();

//
// 📌 ROUTES
//

// API Controllers
app.MapControllers();

// MVC Views
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}"
);

// Test Endpoint
app.MapGet("/test", () => "API is working");

app.Run();