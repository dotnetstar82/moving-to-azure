using AutoMapper;
using Microsoft.EntityFrameworkCore;
using MovingToAzure.Data;
using MovingToAzure.Models;
using MovingToAzure.Services;
using StackExchange.Redis;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddTransient<IProfileRepository, ProfileRepository>();
builder.Services.AddSingleton<IProfileHelper, ProfileHelper>();

builder.Services.AddAutoMapper(typeof(Program));

builder.Services.AddAntiforgery();
builder.Services.AddControllersWithViews();

var appSettings = new AppSettings();
builder.Configuration.GetSection("AppSettings").Bind(appSettings);
builder.Services.AddSingleton<AppSettings>(appSettings);

builder.Services.AddDbContext<SqlDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("MSSQL")));
builder.Services.AddTransient<ISqlDbContext, SqlDbContext>();

builder.Services.AddSingleton<IImageRepository>(new ImageRepository(
    connectionString: builder.Configuration.GetConnectionString("Blob"),
    containerName: appSettings.BlobContainerName
));

builder.Services.AddSingleton<IConnectionMultiplexer>(ConnectionMultiplexer.Connect(builder.Configuration.GetConnectionString("Redis")));
builder.Services.AddSingleton<ICacheRepository, CacheRepository>();

var app = builder.Build();

// Validate AutoMapper, TODO: move to unit test
IMapper mapper = app.Services.GetRequiredService<IMapper>();
mapper.ConfigurationProvider.AssertConfigurationIsValid();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}
else
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();
app.MapDefaultControllerRoute();

app.Run();
