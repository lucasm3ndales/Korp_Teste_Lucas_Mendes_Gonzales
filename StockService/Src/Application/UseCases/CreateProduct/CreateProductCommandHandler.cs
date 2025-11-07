using Mapster;
using MediatR;
using StockService.Application.Common.Dtos;
using StockService.Application.Common.Exceptions;
using StockService.Application.Repositories;
using StockService.Domain.Entities;

namespace StockService.Application.UseCases.CreateProduct;

public class CreateProductCommandHandler(
    IProductRepository productRepository
) : IRequestHandler<CreateProductCommand, ApiResultDto<ProductDto>>
{
    public async Task<ApiResultDto<ProductDto>> Handle(CreateProductCommand request,
        CancellationToken cancellationToken)
    {
        var existentProduct = await productRepository.GetByCode(request.Code, cancellationToken);
        
        if (existentProduct != null)
            throw new ProductCodeAlreadyExistsException(request.Code);

        var product = new Product(
            request.Code,
            request.Description,
            request.InitialStockBalance
        );

        await productRepository.Add(product, cancellationToken);
        await productRepository.SaveChanges(cancellationToken);

        return ApiResultDto<ProductDto>.Success(
            "Produto adicionado com sucesso!",
            product.Adapt<ProductDto>()
        );
    }
}