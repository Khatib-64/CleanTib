namespace CleanTib.Application.Demo;

public class SearchBrandsRequest : PaginationFilter, ICommand<PaginationResponse<BrandDto>>
{ }


public class SearchBrandsRequestHandler : ICommandHandler<SearchBrandsRequest, PaginationResponse<BrandDto>>
{
    private readonly IReadRepository<Brand> _repo;

    public SearchBrandsRequestHandler(IReadRepository<Brand> repo)
    {
        _repo = repo;
    }

    public async Task<PaginationResponse<BrandDto>> Handle(SearchBrandsRequest request, CancellationToken cancellationToken)
    {
        var spec = new BrandsBySearchRequestSpec(request);

        return await _repo.PaginatedListAsync(spec, request.PageNumber, request.PageSize, cancellationToken);
    }
}

public class BrandsBySearchRequestSpec : EntitiesByPaginationFilterSpec<Brand, BrandDto>
{
    public BrandsBySearchRequestSpec(SearchBrandsRequest request)
        : base(request) =>
        Query.OrderBy(c => c.Name, !request.HasOrderBy());
}

public class BrandDto : IDto
{
    public string? Name { get; set; }
    public string? Description { get; set; }
}
