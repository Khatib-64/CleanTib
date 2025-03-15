using CleanTib.Application.Demo;

namespace CleanTib.Host.Controllers.Demo;

public class DemoController : VersionNeutralApiController
{
    [HttpPost]
    [OpenApiOperation("Request an access token using credentials.", "")]
    public async Task<PaginationResponse<BrandDto>> DemoCommand(SearchBrandsRequest recommandquest, CancellationToken cancellationToken)
    {
        return await Mediator.Send(recommandquest);
    }

    [HttpPost("search")]
    [OpenApiOperation("Request an access token using credentials.", "")]
    public async Task<PaginationResponse<BrandDto>> SearchCommand(SearchBrandsRequest recommandquest, CancellationToken cancellationToken)
    {
        return await Mediator.Send(recommandquest);
    }

    [HttpPost("search-prod")]
    [OpenApiOperation("Request an access token using credentials.", "")]
    public async Task<PaginationResponse<ProductDto>> SearchProdCommand(SearchProductsRequest recommandquest, CancellationToken cancellationToken)
    {
        return await Mediator.Send(recommandquest);
    }
}
