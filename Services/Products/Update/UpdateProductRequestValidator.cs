﻿using FluentValidation;

namespace App.Services.Products.Update;

public class UpdateProductRequestValidator: AbstractValidator<UpdateProductRequest>
{
    public UpdateProductRequestValidator()
    {
        
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("ürün ismi gereklidir.")
            .Length(3, 10).WithMessage("ürün ismi  3 ile 10 karakter arasında olmalıdır.");

        // price validation
        RuleFor(x => x.Price)
            .GreaterThan(0).WithMessage("ürün fiyatı 0'dan büyük olmalıdır.");
        
        //stock inclusiveBetween validation
        RuleFor(x => x.Stock)
            .InclusiveBetween(1, 100000).WithMessage("stok adedi 1 ile 100000 arasında olmalıdır");
    }   
}