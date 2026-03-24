using FluentValidation;
using FluentValidation.AspNetCore;
using InventoryManagement.Application.Mapping;
using InventoryManagement.Application.Services;
using InventoryManagement.Application.Validations;
using InventoryManagement.Infrastructure;
using InventoryManagement.Infrastructure.Persistence.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Versioning;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<ApplicationDbContext>(opt => opt.UseSqlServer(connectionString));
builder.Services.AddUnitOfWorkAndRepository();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddAutoMapper(cfg =>
{
    cfg.AddProfile<MappingProfile>();
});

builder.Services.AddScoped<CategoryService, CategoryService>();
builder.Services.AddScoped<ProductService, ProductService>();
builder.Services.AddOpenApi();
builder.Services.AddFluentValidationAutoValidation();
builder.Services.AddValidatorsFromAssemblyContaining<CreateProductValidator>();
builder.Services.AddCors(options =>
{
    options.AddPolicy("OpenCorsPolicy", policy =>
    {
        policy.WithOrigins("https://localhost:7161") // بورت البلازور بتاعك
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});
//Config API Versioning 
builder.Services.AddApiVersioning(opt =>
{
    opt.DefaultApiVersion = new ApiVersion(1, 0);
    opt.AssumeDefaultVersionWhenUnspecified = true;
    opt.ApiVersionReader = new HeaderApiVersionReader("api-version"); 
});    

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}
app.UseStaticFiles();
app.UseRouting();
app.UseHttpsRedirection();
app.UseCors("OpenCorsPolicy");
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

app.Run();
