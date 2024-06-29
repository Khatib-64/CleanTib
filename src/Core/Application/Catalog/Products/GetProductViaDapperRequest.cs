namespace CleanTib.Application.Catalog.Products;

public class GetProductViaDapperRequest : IRequest<ProductDto>
{
    public Guid Id { get; set; }

    public GetProductViaDapperRequest(Guid id) => Id = id;
}

public class GetProductViaDapperRequestHandler : IRequestHandler<GetProductViaDapperRequest, ProductDto>
{
    private readonly IRepository<Product> _repository;
    private readonly IStringLocalizer _t;

    public GetProductViaDapperRequestHandler(IRepository<Product> repository, IStringLocalizer<GetProductViaDapperRequestHandler> localizer) =>
        (_repository, _t) = (repository, localizer);

    public async Task<ProductDto> Handle(GetProductViaDapperRequest request, CancellationToken cancellationToken)
    {
        var product = await _repository.GetByIdAsync(request.Id, cancellationToken)
            ?? throw new NotFoundException(_t["Product {0} Not Found.", request.Id]);

        return new ProductDto
        {
            Id = product.Id,
            BrandId = product.BrandId,
            BrandName = string.Empty,
            Description = product.Description,
            ImagePath = product.ImagePath,
            Name = product.Name,
            Rate = product.Rate
        };
    }
}