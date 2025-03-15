namespace CleanTib.Application.Demo;

public class SearchProductsRequest : PaginationFilter, ICommand<PaginationResponse<ProductDto>>
{ }


public class SearchProductsRequestHandler : ICommandHandler<SearchProductsRequest, PaginationResponse<ProductDto>>
{
    private readonly IReadRepository<Product> _repo;

    public SearchProductsRequestHandler(IReadRepository<Product> repo)
    {
        _repo = repo;
    }

    public async Task<PaginationResponse<ProductDto>> Handle(SearchProductsRequest request, CancellationToken cancellationToken)
    {
        var spec = new ProductsBySearchRequestSpec(request);

        return await _repo.PaginatedListAsync(spec, request.PageNumber, request.PageSize, cancellationToken);
    }
}

public class ProductsBySearchRequestSpec : EntitiesByPaginationFilterSpec<Product, ProductDto>
{
    public ProductsBySearchRequestSpec(SearchProductsRequest request)
        : base(request) =>
        Query.OrderBy(c => c.Name, !request.HasOrderBy());
}

public class ProductDto : IDto
{
    public string? Name { get; set; }
    public string? Description { get; set; }
    public BrandDto? Brand { get; set; }
}
