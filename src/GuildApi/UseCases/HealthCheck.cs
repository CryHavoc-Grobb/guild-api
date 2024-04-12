using Ardalis.ApiEndpoints;
using Microsoft.AspNetCore.Mvc;

namespace GuildApi.UseCases;

public class HealthCheck : EndpointBaseSync
    .WithoutRequest
    .WithActionResult
{
    [HttpGet("/")]
    public override ActionResult Handle()
    {
        return Ok();
    }
}