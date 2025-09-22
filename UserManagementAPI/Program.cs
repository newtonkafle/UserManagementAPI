using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.EntityFrameworkCore;
using UserManagementAPI;
using UserManagementAPI.DTO;
using UserManagementAPI.Services;
using Serilog;
var builder = WebApplication.CreateBuilder(args);

// configuring logging using serilog


//configuring the sqlite database
var conn = builder.Configuration.GetConnectionString("DefaultConnection");

// configuring the logging service to log in json file.
builder.Logging.ClearProviders();
builder.Host.UseSerilog((context, services, configuration) =>
{
    configuration.Enrich.FromLogContext()
        .WriteTo.File(
            path: "logs/log.json",
            rollingInterval: RollingInterval.Day,
            formatter: new Serilog.Formatting.Json.JsonFormatter());
});
builder.Services.AddDbContext<AppDbContext>(options =>
{
    options.UseSqlite(conn);
});
// injecting the use service use DI
builder.Services.AddScoped<UserService>();
var app = builder.Build();

//global exception handling middleware
app.UseExceptionHandler(errorApp =>
{
    errorApp.Run(async context =>
    {
        context.Response.StatusCode = StatusCodes.Status500InternalServerError;
        context.Response.ContentType = "application/problem+json";

        var exceptionHandlerFeature =
            context.Features.Get<IExceptionHandlerFeature>();
        var exception = exceptionHandlerFeature?.Error;

        var problem = new
        {
            Title = "An unexpected error occured",
            Detail = exception?.Message,
            TraceId = context.TraceIdentifier,
        };
        await context.Response.WriteAsJsonAsync(problem);
    });
});

// adding the logging middleware
app.Use(async (context, next) =>
{
    var logger = app.Logger;

    logger.LogInformation("Request: {method} {path} {@headers} {@query}",
        context.Request.Method,
        context.Request.Path,
        context.Request.Headers,
        context.Request.Query);

    await next();
    
    logger.LogInformation("Response: {statusCode}",  context.Response.StatusCode);
});

app.MapGet("/", () =>
{
});

// endpoint to get all the users
app.MapGet("/users", (UserService userService) => userService.GetUsers());

//endpoint to get specific user
app.MapGet("/user/{id:int}", async (int id, UserService userService) =>
{
    var result = await  userService.GetUser(id);
    return !result.Success ? Results.Problem(statusCode:404, detail:result.ErrorMessage)
        : Results.Ok(result.Data);
});

// endpoint to add user to the certain department. 
app.MapPost("/user", async (UserAddDto userAddDto, UserService userService) =>
{

    var result = await userService.AddUser(userAddDto);
    return !result.Success
        ? Results.Problem(statusCode: 404, detail: result.ErrorMessage)
        : Results.Ok(result.Data);
}).AddEndpointFilter(async (context, next) =>
{
    var dto = context.GetArgument<UserAddDto>(0);
    var validationContext = new ValidationContext(dto);
    var validationResults = new List<ValidationResult>();
    bool isValid = Validator.TryValidateObject(dto, validationContext, validationResults);

    if (!isValid)
    {
        var errors = validationResults
            .GroupBy(r => r.MemberNames.FirstOrDefault() ?? "")
            .ToDictionary(
                g => g.Key,
                g => g.Select(r => r.ErrorMessage ?? "").ToArray()
            );

        return Results.ValidationProblem(errors);
    }

    return await next(context);
});

//endpoint to update the user details.
app.MapPut("/user/{id:int}", async (int id, UserUpdateDto userUpdateDto, UserService userService) =>
{
    var result = await userService.UpdateUser(id, userUpdateDto);
    return result.Success
        ? Results.Ok(result.Data)
        : Results.Problem(statusCode: 404, detail: result.ErrorMessage);
}).AddEndpointFilter(async (context, next) =>
{
    var dto = context.GetArgument<UserUpdateDto>(1);
    var validationContext = new ValidationContext(dto);
    var validationResults = new List<ValidationResult>();
    bool isValid = Validator.TryValidateObject(dto, validationContext, validationResults);

    if (!isValid)
    {
        var errors = validationResults
            .GroupBy(r => r.MemberNames.FirstOrDefault() ?? "")
            .ToDictionary(
                g => g.Key,
                g => g.Select(r => r.ErrorMessage ?? "").ToArray());

        return Results.ValidationProblem(errors);
    }

    return await next(context);

});

app.MapDelete("/user/{id:int}", async (int id, UserService userService) =>
{
    var result = await userService.DeleteUser(id);
    return result.Success
        ? Results.Ok(result.Data)
        : Results.Problem(statusCode: 400, detail: result.ErrorMessage);
});


app.Run();