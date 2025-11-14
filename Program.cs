using Microsoft.AspNetCore.Identity;

var builder = WebApplication.CreateBuilder(args);

// Register MVC controllers
builder.Services.AddControllers();

// Register your DbContext (use same connection string as OnConfiguring)
builder.Services.AddDbContext<LogiTrackContext>();

//builder.Services.AddIdentity<ApplicationUser, IdentityUser>().AddEntityFrameworkStores<LogiTrackContext>();
builder.Services.AddIdentity<ApplicationUser, IdentityRole>()
    .AddEntityFrameworkStores<LogiTrackContext>();

builder.Services.AddMemoryCache();


// Optional: swagger for development
//builder.Services.AddEndpointsApiExplorer();
//builder.Services.AddSwaggerGen();

var app = builder.Build();

// Ensure database exists
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<LogiTrackContext>();
    db.Database.EnsureCreated();
}

if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
    //app.UseSwagger();
    //app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseRouting();
app.UseAuthorization();
app.MapControllers();

app.Run();