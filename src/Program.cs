
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using src.Data;
using src.Interfaces;
using src.Middlewares;
using src.Repositories;
using src.Services;
using src.Utils;
using System.Reflection;

var builder = WebApplication.CreateBuilder(args);

// Đăng ký logging, config, services, v.v.
builder.Services.AddLogging(loggingBuilder =>
{
    loggingBuilder.AddConsole();
    loggingBuilder.AddDebug();
});
builder.Services.AddProblemDetails();
builder.Configuration.AddJsonFile("seedData.json", optional: true, reloadOnChange: true);
QuestPDF.Settings.License = QuestPDF.Infrastructure.LicenseType.Community;

// DB configuration
builder.Services.AddDbContext<ApplicationDbContext>(options =>
{
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection"));
});

// Global exception handler
builder.Services.AddExceptionHandler<GlobalExceptionHandlers>();

// Đăng ký SeedService
builder.Services.AddTransient<SeedService>();

// Nếu không chạy reset DB, đăng ký dịch vụ migration tự động
bool resetDb = args.Contains("--resetdb");
if (!resetDb)
{
    builder.Services.AddHostedService<ApplyMigrationService>();
}

// Đăng ký các repository và dịch vụ khác
builder.Services.AddScoped<IItemGroupRepository, ItemGroupRepository>();
builder.Services.AddScoped<IVariantRepository, VariantRepository>();    
builder.Services.AddScoped<IItemRepository, ItemRepository>();
builder.Services.AddScoped<IColorRepository, ColorRepository>();
builder.Services.AddScoped<IManufacturerRepository, ManufacturerRepository>();
builder.Services.AddScoped<ICustomerRepository, CustomerRepository>();
builder.Services.AddScoped<IInvoiceRepository, InvoiceRepository>();
builder.Services.AddScoped<IInventoryTransactionRepository, InventoryTransactionRepository>();
builder.Services.AddTransient<PdfInvoiceService>();
builder.Services.AddTransient<HtmlInvoiceService>();

// Cấu hình Swagger
builder.Services.AddControllers();
 builder.Services.Configure<ApiBehaviorOptions>(options =>
    {
        options.InvalidModelStateResponseFactory = context =>
        {
            // Lấy danh sách các thông báo lỗi từ ModelState
            var errors = context.ModelState
                .Where(e => e.Value.Errors.Count > 0)
                .SelectMany(e => e.Value.Errors)
                .Select(e => e.ErrorMessage)
                .ToArray();

            // Tạo đối tượng lỗi theo định dạng bạn muốn
            var errorResponse = new
            {
                data = (object)null,
                succeeded = false,
                errors = errors,
                message = "Validation errors occurred."
            };

            return new BadRequestObjectResult(errorResponse);
        };
    });
builder.Services.AddOpenApi();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "Phone API",
        Version = "v1",
        Description = "API for Phone system management"
    });
    c.OperationFilter<FileUploadOperationFilter>();
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        In = ParameterLocation.Header,
        Description = "Please enter a valid token",
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        BearerFormat = "JWT",
        Scheme = "Bearer"
    });
    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            new string[] { }
        }
    });
});

// Cấu hình CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAllOrigins",
        policy =>
        {
            policy.AllowAnyOrigin()
                  .AllowAnyMethod()
                  .AllowAnyHeader();
        });
});

var app = builder.Build();

app.UseCors("AllowAllOrigins");

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.UseSwagger();
    app.UseSwaggerUI();
}

// Nếu có tham số "--resetdb", tự động reset và seed DB
if (resetDb)
{
    using (var scope = app.Services.CreateScope())
    {
        var seedService = scope.ServiceProvider.GetRequiredService<SeedService>();
        Console.WriteLine("Resetting database and seeding data...");
        await seedService.SeedAsync(true);
    }
}

app.UseHttpsRedirection();
app.UseExceptionHandler();
app.MapControllers();
app.Run();
