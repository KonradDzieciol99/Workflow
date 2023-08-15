using API.Aggregator;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddWebAPIServices(builder.Configuration);

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

//app.UseHttpsRedirection();
app.UseCors("allowAny");
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();
app.Run();
