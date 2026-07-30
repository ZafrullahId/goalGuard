using goalGuard.Contracts;

namespace goalGuard.Endpoints;

public static class TransferEndpoints
{
    public static void MapTransferEndpoints(this WebApplication app)
    {
        var group = app.MapGroup("/api/transfer").WithTags("Transfer");

        //group.MapPost("/evaluate", (TransferEvaluateRequest request) =>
        //{
        //    return Results.StatusCode(501);
        //})
        //.WithSummary("Evaluate Transfer")
        //.WithDescription("Returns a verdict + plain-language trade-off message.");

        //group.MapPost("/confirm", (TransferConfirmRequest request) =>
        //{
        //    return Results.StatusCode(501);
        //})
        //.WithSummary("Confirm Transfer")
        //.WithDescription("Actually executes the withdrawal/exchange via BMONI.");
    }
}
