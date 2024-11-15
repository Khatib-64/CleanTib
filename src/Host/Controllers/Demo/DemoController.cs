using CleanTib.Application.Demo;

namespace CleanTib.Host.Controllers.Demo;

public class DemoController : VersionNeutralApiController
{
    [HttpPost]
    [OpenApiOperation("Request an access token using credentials.", "")]
    public async Task<string> DemoCommand(DemooCommand recommandquest, CancellationToken cancellationToken)
    {
        return await Mediator.Send(recommandquest);
    }
}
