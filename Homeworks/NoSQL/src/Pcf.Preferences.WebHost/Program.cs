using StackExchange.Redis;
using Pcf.Preferences.Core.Abstractions.Repositories;
using Pcf.Preferences.DataAccess.Repositories;
using Pcf.Preferences.Core.Domain;
using Pcf.Preferences.DataAccess;
using Microsoft.EntityFrameworkCore;
using Pcf.Preferences.DataAccess.Data;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddSingleton<IConnectionMultiplexer>(sp =>
{
    var configuration = builder.Configuration.GetSection("Redis")["ConnectionString"];
    return ConnectionMultiplexer.Connect(configuration);
});

builder.Services.AddScoped<IRepository<Preference>>(sp =>
{
    var efRepo = ActivatorUtilities.CreateInstance<EfRepository<Preference>>(sp);
    var redis = sp.GetRequiredService<IConnectionMultiplexer>();
    return new RedisCachePreferencesRepository(efRepo, redis);
});


builder.Services.AddDbContext<DataContext>(x =>
{
    x.UseNpgsql(builder.Configuration.GetConnectionString("PromocodeFactoryReceivingFromPartnerDb"));
    x.UseSnakeCaseNamingConvention();
    x.UseLazyLoadingProxies();
});

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<DataContext>();
    new EfDbInitializer(context).InitializeDb();
}

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();
app.Run();


