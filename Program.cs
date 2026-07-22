using System.Text;
using HMS_NewProject_Temp_Humdity.Database;
using HMS_NewProject_Temp_Humdity.Database.Interface;
using HMS_NewProject_Temp_Humdity.Middleware;
using HMS_NewProject_Temp_Humdity.Models.Config;
using HMS_NewProject_Temp_Humdity.Services;
using HMS_NewProject_Temp_Humdity.Services.Interface;
using HMS_NewProject_Temp_Humdity.Signalr;
using HMS_NewProject_Temp_Humdity.Signalr.Interface;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using MongoDB.Driver;

using Serilog;
var builder = WebApplication.CreateBuilder(args);

var port = builder.Configuration.GetValue<int>("PortService:Port");

builder.WebHost.ConfigureKestrel(options =>
{
	options.ListenAnyIP(port);

	//options.ListenAnyIP(port, listenOptions =>
	//{
	//	listenOptions.UseHttps();
	//});
});


// Add services to the container.
var mongoConnectionString = builder.Configuration.GetSection("MongoDB:ConnectionString").Value;
var mongoDatabaseName = builder.Configuration.GetSection("MongoDB:DatabaseName").Value;

var mongoClient = new MongoClient(mongoConnectionString);
var mongoDatabase = mongoClient.GetDatabase(mongoDatabaseName);

builder.Services.AddSingleton<IMongoDatabase>(mongoDatabase);
builder.Services.AddSingleton<MongodbContext>();

builder.Services.AddSingleton<IDAOUser, DAOUser>();
builder.Services.AddSingleton<IDAODevice, DAODevice>();
builder.Services.AddSingleton<IDAOLocation, DAOLocation>();
builder.Services.AddSingleton<IDAOCompany, DAOCompany>();
builder.Services.AddSingleton<IDAOCounter, DAOCounter>();


//Service
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IDeviceService, DeviceService>();
builder.Services.AddScoped<ILocationService, LocationService>();

builder.Services.AddSingleton<IHubDevice, HubDeviceMonitor>();
var appConfig = builder.Configuration.Get<clsAppConfig>();
builder.Services.AddSingleton(appConfig);

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

//Logger bằng seriLog
if (builder.Environment.IsDevelopment())
{
	builder.Host.UseSerilog((context, services, configuration) =>
	{
		configuration.ReadFrom.Configuration(context.Configuration);
	});
}
// JWT
var secretKey = builder.Configuration["Jwt:SecretKey"]!;
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
	.AddJwtBearer(options =>
	{
		options.TokenValidationParameters = new TokenValidationParameters
		{
			ValidateIssuer = true,
			ValidateAudience = true,
			ValidateLifetime = true,
			ValidateIssuerSigningKey = true,
			ValidIssuer = builder.Configuration["Jwt:Issuer"],
			ValidAudience = builder.Configuration["Jwt:Audience"],
			IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey))
		};
	});
var app = builder.Build();
app.UseMiddleware<GlobalExceptionMiddleware>();
// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
	app.UseSwagger();
	app.UseSwaggerUI();
}
app.UseAuthorization();
app.MapControllers();
app.Run();
