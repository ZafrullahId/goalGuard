using goalGuard.Data;
using Microsoft.EntityFrameworkCore;

namespace goalGuard.Endpoints;

public static class HealthEndpoints
{
    public static void MapHealthEndpoints(this WebApplication app)
    {
        var group = app.MapGroup("/api/health").WithTags("Health");

        group.MapGet("/db", async (GoalGuardDbContext dbContext) =>
        {
            var canConnect = await dbContext.Database.CanConnectAsync();
            return Results.Ok(new { CanConnect = canConnect });
        })
        .WithSummary("Check DB Connection")
        .WithDescription("Verifies if the database connection works end-to-end.");
    }
}
