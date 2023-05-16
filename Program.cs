using com.emecca.service;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.AspNetCore.Cors;
using EmeccaRestfulApi.Utils;
using EmeccaRestfulApi.DBContext;
using Microsoft.EntityFrameworkCore;
using EmeccaRestfulApi.Services.Interfaces;
using EmeccaRestfulApi.Services.Implementations;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Logging.AddLog4Net("CNF/log4net.config");
builder.Services.AddControllers();
builder.Services.AddScoped<IMenuService, MenuServiceImpl>();
builder.Services.AddScoped<IEmeccaDeletePacsApplyService, EmeccaDeletePacsApplyServiceImpl>();
builder.Services.AddScoped<IDeleteArchiveLogService, DeleteArchiveLogServiceImpl>();
builder.Services.AddScoped<IObjidService, ObjidServiceImpl>();
builder.Services.AddScoped<IUserBasService, UserBasServiceImpl>();

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddCors(options =>
                        {
                            options.AddPolicy("AllowAllOrigins",
                                builder =>
                                {
                                    builder.AllowAnyOrigin()
                                        .AllowAnyHeader()
                                        .AllowAnyMethod();
                                });
                        });
builder.Services.AddSingleton<EmeccaObjectIdGenerator>();
builder.Services.AddDbContext<EmeccaDotNetContext>(options => options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")), ServiceLifetime.Scoped);
builder.Services.AddDbContext<DCMASPEAFContext>(options => options.UseSqlServer(builder.Configuration.GetConnectionString("AppConnection")), ServiceLifetime.Scoped);
builder.Services.AddHttpClient();

var app = builder.Build();

// Configure the HTTP request pipeline.
//if (app.Environment.IsDevelopment())
//{
    app.UseSwagger();
    app.UseSwaggerUI();
//}

// 中間層設定
//app.UseMiddleware<OAuth2Middleware>();   

//app.UseHttpsRedirection();
app.UseCors("AllowAllOrigins");
app.UseAuthorization();
app.UseRouting();
app.MapControllers();

app.Run();
