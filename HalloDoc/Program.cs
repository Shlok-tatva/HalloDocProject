using HalloDoc_BAL.Interface;
using HalloDoc_DAL.DataContext;
using HalloDoc_BAL.Repository;
using Microsoft.AspNetCore.Authentication.Cookies;
using Rotativa.AspNetCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddDbContext<ApplicationDbContext>();
builder.Services.AddScoped<IAspnetuserRepository, AspnetuserRepository>();
builder.Services.AddScoped<IRequestClientRepository, RequestClientRepository>();
builder.Services.AddScoped<IRequestRepository, RequestRepository>();
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IRConciergeRepository, RConciergeRepository>();
builder.Services.AddScoped<IRequestConciergeRepository, RequestConciergeRepository>();
builder.Services.AddScoped<IRequestBusinessRepository, RequestBusinessRepository>();
builder.Services.AddScoped<IRBusinessRepository, RBusinessRepository>();
builder.Services.AddScoped<IRequestwisefileRepository, RequestwisefileRepository>();
builder.Services.AddScoped<IPatientFunctionRepository, PatientFunctionRepository>();
builder.Services.AddScoped<IAdminFunctionRepository, AdminFunctionRepository>();
builder.Services.AddScoped<IRequestNotesRepository, RequestNotesRepository>();
builder.Services.AddScoped<ICommonFunctionRepository, CommonFunctionRepository>();
builder.Services.AddScoped<IAdminRepository, AdminRepository>();
builder.Services.AddScoped<IJwtServices, JwtServices>();




builder.Services.AddSession(options =>
{
    options.Cookie.Name = ".HalloDoc.Session";
    options.IdleTimeout = TimeSpan.FromMinutes(30); // Adjust as needed
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

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
app.UseRotativa();
app.UseAuthorization();
app.UseAuthentication();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Patient}/{action=Index}/{id?}");

    app.Run();
