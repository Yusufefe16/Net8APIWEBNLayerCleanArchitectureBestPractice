﻿using System.Net;
using App.Repositories;
using App.Repositories.Products;
using App.Services.Products.Create;
using App.Services.Products.Update;
using AutoMapper;
using Microsoft.EntityFrameworkCore;

namespace App.Services.Products;

public class ProductService(IProductRepository productRepository, IUnitOfWork unitOfWork, IMapper mapper) : IProductService
{
    public async Task<ServiceResult<List<ProductDto>>> GetTopPriceProductsAsync(int count)
    {
        var products = await productRepository.GetTopPriceProductsAsync(count);

        var productDtos = mapper.Map<List<ProductDto>>(products);
        
        return new ServiceResult<List<ProductDto>>()
        {
            Data = productDtos
        };
    }

    public async Task<ServiceResult<List<ProductDto>>> GetAllListAsync()
    {
        var products = await productRepository.GetAll().ToListAsync();
        //var productsAsDto = products.Select(product => new ProductDto(product.Id, product.Name, product.Price, product.Stock)).ToList();
        
        var productsAsDto = mapper.Map<List<ProductDto>>(products);
        
        return ServiceResult<List<ProductDto>>.Success(productsAsDto);
    }

    public async Task<ServiceResult<List<ProductDto>>> GetPagedAllAsync(int pageNumber, int pageSize)
    {
        var products = await productRepository.GetAll().Skip((pageNumber - 1) * pageSize).Take(pageSize).ToListAsync();
        
        //var productsAsDto = products.Select(product => new ProductDto(product.Id, product.Name, product.Price, product.Stock)).ToList();
        
        var productsAsDto = mapper.Map<List<ProductDto>>(products);
        
        return ServiceResult<List<ProductDto>>.Success(productsAsDto);
    }

    public async Task<ServiceResult<ProductDto?>> GetByIdAsync(int id)
    {
        var product = await productRepository.GetByIdAsync(id);

        if (product is null)
        {
            return ServiceResult<ProductDto?>.Fail("Product not found", HttpStatusCode.NotFound);
        }

        //var productDto = new ProductDto(product!.Id, product.Name, product.Price, product.Stock);
        
        var productAsDto = mapper.Map<ProductDto>(product);
        
        return ServiceResult<ProductDto>.Success(productAsDto)!;
    }

    public async Task<ServiceResult<CreateProductResponse>> CreateAsync(CreateProductRequest request)
    {
        var anyProduct = await productRepository.Where(x => x.Name == request.Name).AnyAsync();

        if (anyProduct)
        {
            return ServiceResult<CreateProductResponse>.Fail("ürün ismi veritabanında bulunmaktadır.",
                HttpStatusCode.NotFound);
        }
        
        var product = mapper.Map<Product>(request);

        await productRepository.AddAsync(product);
        await unitOfWork.SaveChangesAsync();
        return ServiceResult<CreateProductResponse>.SuccessAsCreated(new CreateProductResponse(product.Id),$"api/products/{product.Id}");
    }

    public async Task<ServiceResult> UpdateAsync(int id, UpdateProductRequest request)
    {
        var product = await productRepository.GetByIdAsync(id);
        if (product is null)
        {
            return ServiceResult.Fail("Product not found", HttpStatusCode.NotFound);
        }
        
        var isProductNameExist = await productRepository.Where(x => x.Name == request.Name && x.Id != product.Id).AnyAsync();

        if (isProductNameExist)
        {
            return ServiceResult.Fail("ürün ismi veritabanında bulunmaktadır.",
                HttpStatusCode.NotFound);
        }
        
        product = mapper.Map(request, product);

        productRepository.Update(product);
        await unitOfWork.SaveChangesAsync();
        return ServiceResult.Success(HttpStatusCode.NoContent);
    }

    public async Task<ServiceResult> UpdateStockAsync(UpdateProductStockRequest.UpdateProductStockRequest request)
    {
        var product = await productRepository.GetByIdAsync(request.ProductId);
        if (product is null)
        {
            return ServiceResult.Fail("Product not found", HttpStatusCode.NotFound);
        }
        product.Stock = request.Quantity;
        
        productRepository.Update(product);
        await unitOfWork.SaveChangesAsync();
        return ServiceResult.Success(HttpStatusCode.NoContent);
    }

    public async Task<ServiceResult> DeleteAsync(int id)
    {
        var product = await productRepository.GetByIdAsync(id);
        if (product is null)
        {
            return ServiceResult.Fail("Product not found", HttpStatusCode.NotFound);
        }

        productRepository.Delete(product);
        await unitOfWork.SaveChangesAsync();
        return ServiceResult.Success(HttpStatusCode.NoContent);
    }
}