using CleanTib.Application.Catalog.Products;

namespace CleanTib.Application.Catalog.Brands;

public class DeleteBrandRequest(Guid id) : IRequest<Result<Guid>>
{
    public Guid Id { get; set; } = id;
}

public class DeleteBrandRequestHandler : IRequestHandler<DeleteBrandRequest, Result<Guid>>
{
    // Add Domain Events automatically by using IRepositoryWithEvents
    private readonly IRepositoryWithEvents<Brand> _brandRepo;
    private readonly IReadRepository<Product> _productRepo;
    private readonly IStringLocalizer _t;

    public DeleteBrandRequestHandler(IRepositoryWithEvents<Brand> brandRepo, IReadRepository<Product> productRepo, IStringLocalizer<DeleteBrandRequestHandler> localizer) =>
        (_brandRepo, _productRepo, _t) = (brandRepo, productRepo, localizer);

    public async Task<Result<Guid>> Handle(DeleteBrandRequest request, CancellationToken cancellationToken)
    {
        if (await _productRepo.AnyAsync(new ProductsByBrandSpec(request.Id), cancellationToken))
        {
            throw new ConflictException(_t["Brand cannot be deleted as it's being used."]);
        }

        var brand = await _brandRepo.GetByIdAsync(request.Id, cancellationToken);

        _ = brand ?? throw new NotFoundException(_t["Brand {0} Not Found."]);

        await _brandRepo.DeleteAsync(brand, cancellationToken);

        return Result<Guid>.Success(request.Id);
    }
}