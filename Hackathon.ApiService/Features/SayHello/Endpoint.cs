using FastEndpoints;
using Hackathon.ApiService.Databases.DbContexts;

namespace Hackathon.ApiService.Features.SayHello
{
    public class Request
    {
        public required string Name { get; set; } 
    }
    public class Response
    {
        public required string Message { get; set; }
    }
    public class Endpoint : Endpoint<Request, Response>
    {
        private readonly IMaindbContext _dbContext;
        public Endpoint(IMaindbContext dbContext)
        {
            _dbContext = dbContext;
        }
        public override void Configure() => Get("/api/v1/say-hello");
        public override async Task HandleAsync(Request request, CancellationToken ct)
        {
            var message = $"Hello, {request.Name}!";
            await Send.OkAsync(new Response { Message = message }, cancellation: ct);
        }
    }
}
