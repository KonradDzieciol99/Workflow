using IdentityDuende;
using IdentityDuende.Infrastructure.DataAccess;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddWebAPIServices(builder.Configuration);

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
    app.UseSwagger();
    app.UseSwaggerUI();
    await ApplyMigration();
}

//app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
app.UseIdentityServer();
app.UseAuthentication();
app.UseAuthorization();
app.MapRazorPages().RequireAuthorization();
app.MapControllers();
app.Run();

async Task ApplyMigration()
{
    using (var scope = app.Services.CreateScope())
    {
        var initialiser = scope.ServiceProvider.GetRequiredService<SeedData>();
        await initialiser.InitialiseAsync();
        await initialiser.SeedAsync();
    }
    return;
}