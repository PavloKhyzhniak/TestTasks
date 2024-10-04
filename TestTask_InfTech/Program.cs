using Microsoft.EntityFrameworkCore;
using TestTask_InfTech.DB;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

// получаем строку подключени€ из файла конфигурации
string connection = builder.Configuration.GetConnectionString("DefaultConnection");

// добавл€ем контекст ApplicationContext в качестве сервиса в приложение
builder.Services.AddDbContext<FileStorageDB>(options => options.UseNpgsql(connection));


builder.Services.AddDistributedMemoryCache();// добавл€ем IDistributedMemoryCache
builder.Services.AddSession();  // добавл€ем сервисы сессии

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();
app.UseSession();

app.UseAuthorization();

app.MapAreaControllerRoute(
    name: "Folders",
    areaName: "FileStorageArea",
    pattern: "FileStorage/{controller=Folders}/{action=Index}/{id?}");

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
