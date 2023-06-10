using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using WebApiAuthentication.Authentication;
using WebApiAuthentication.DataAccess.Context;
using WebApiAuthentication.DataAccess.Repositories;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();

builder.Services.AddDbContext<ReviewContext>(options =>
                options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddIdentity<LibraryUser, IdentityRole>(options =>
    options.User.AllowedUserNameCharacters += " ")
           .AddEntityFrameworkStores<ReviewContext>()
           .AddDefaultTokenProviders();

builder.Services.AddScoped<IReviewRepository, SqlServerRepository>();

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

//PopulateDb();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();

//void PopulateDb()
//{
//    using var scope = app.Services.CreateScope();
    
//    using var db = scope.ServiceProvider.GetRequiredService<ReviewContext>();

//    db.BookReviews.Add(new() { Title = "Dr No", Rating = 4 });
//    db.BookReviews.Add(new() { Title = "Goldfinger", Rating = 3 });
//    db.BookReviews.Add(new() { Title = "From Russia with Love", Rating = 1 });
//    db.BookReviews.Add(new() { Title = "Moonraker", Rating = 4 });
//    db.BookReviews.Add(new() { Title = "Dr No", Rating = 5 });
//    db.BookReviews.Add(new() { Title = "Moonraker", Rating = 2 });
//    db.BookReviews.Add(new() { Title = "Dr No", Rating = 2});
//    db.BookReviews.Add(new() { Title = "From Russia with Love", Rating = 5 });
//    db.BookReviews.Add(new() { Title = "From Russia with Love", Rating = 3 });

//    db.SaveChanges();
//}