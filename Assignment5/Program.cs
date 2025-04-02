using Assignment5.Interfaces;
using Assignment5.Repository;
using Assignment5.Service;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllersWithViews();
builder.Services.AddMemoryCache();
builder.Services.AddScoped<IPersonService, PersonService>();
builder.Services.AddSingleton<IPersonData, PersonData>();

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Person}/{action=Index}/{id?}"
);

app.Run();