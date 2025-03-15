
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using src.Data;
using src.Interfaces;
using src.Middlewares;
using src.Repositories;
using src.Services;
using src.Utils;
using System.Reflection; // Needed for Assembly

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddLogging(loggingBuilder =>
{
    loggingBuilder.AddConsole(); // Log to console
    loggingBuilder.AddDebug();   // Log to debug output
});
builder.Services.AddProblemDetails();
// ------------------- add json file
builder.Configuration.AddJsonFile("seedData.json", optional: true, reloadOnChange: true);
// DB configuration ----------
builder.Services.AddDbContext<ApplicationDbContext>(options =>
{
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection"));
});

// --global exception handler
builder.Services.AddExceptionHandler<GlobalExceptionHandlers>();


// --Util services like migrations 
builder.Services.AddTransient<SeedService>();
builder.Services.AddHostedService<ApplyMigrationService>();
// Add services to the container.

builder.Services.AddControllers();
builder.Services.AddOpenApi();
// dependency injection
builder.Services.AddScoped<IItemGroupRepository, ItemGroupRepository>();
builder.Services.AddScoped<IVariantRepository, VariantRepository>();    
builder.Services.AddScoped<IItemRepository, ItemRepository>();
builder.Services.AddScoped<IColorRepository, ColorRepository>();
builder.Services.AddScoped<IManufacturerRepository, ManufacturerRepository>();
builder.Services.AddScoped<ICustomerRepository, CustomerRepository>();
builder.Services.AddScoped<IInvoiceRepository, InvoiceRepository>();
builder.Services.AddScoped<IInventoryTransactionRepository, InventoryTransactionRepository>();
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

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAllOrigins",
        policy =>
        {
            policy.AllowAnyOrigin()  // Chấp nhận tất cả nguồn (nên giới hạn trong production)
                  .AllowAnyMethod()   // Chấp nhận tất cả HTTP methods (GET, POST, PUT, DELETE)
                  .AllowAnyHeader();  // Chấp nhận tất cả headers
        });
});

var app = builder.Build();

app.UseCors("AllowAllOrigins");

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.UseSwagger();
    app.UseSwaggerUI();
}


app.UseHttpsRedirection();
app.UseExceptionHandler(); // Works
app.MapControllers();
app.Run();
