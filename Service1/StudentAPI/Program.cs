using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using StudentAPI;
using StudentAPI.Implementation;
using StudentAPI.Implementations;
using StudentAPI.Interfaces;
using StudentAPI.Model;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.


builder.Services.AddControllers();
builder.Services.AddCors();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddScoped<IStudentService, StudentService>();
builder.Services.AddScoped<IKafkaConsumer<string, string>, KafkaConsumer<string, string>>();

builder.Services.AddDbContext<DBContext>(Options => {
    Options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"));
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseCors(p => p.AllowAnyHeader().AllowAnyMethod().WithOrigins());
app.UseAuthorization();

app.MapControllers();

app.Run();
