using FluentValidation;
using PriceComparisonMVC.Models.Seller;

namespace PriceComparisonMVC.Infrastructure.Validation.Seller
{
    public class SellerRequestCreateModelValidator : AbstractValidator<SellerRequestCreateModel>
    {
        public SellerRequestCreateModelValidator()
        {
            RuleFor(x => x.StoreName)
               //.NotEmpty().WithMessage("Назва магазину є обов'язковою")
               .MaximumLength(255).WithMessage("Назва магазину не може містити більше 255 символів");

            RuleFor(x => x.WebsiteUrl)
                .NotEmpty().WithMessage("URL сайту є обов'язковим")
                .MaximumLength(2083).WithMessage("URL сайту не може містити більше 2083 символів")
                .Must(uri => Uri.TryCreate(uri, UriKind.Absolute, out _))
                .WithMessage("URL сайту має бути коректною адресою");

            RuleFor(x => x.ContactPerson)
                .NotEmpty().WithMessage("Контактна особа є обов'язковою")
                .MaximumLength(100).WithMessage("Контактна особа не може містити більше 100 символів");

            RuleFor(x => x.ContactPhone)
                .NotEmpty().WithMessage("Контактний телефон є обов'язковим")
                .MaximumLength(20).WithMessage("Контактний телефон не може містити більше 20 символів")
                .Matches(@"^\+?[0-9\s\-\(\)]+$").WithMessage("Контактний телефон має бути коректним номером");

            RuleFor(x => x.StoreComment)
                .MaximumLength(1000).WithMessage("Коментар до магазину не може містити більше 1000 символів")
                .When(x => x.StoreComment != null);
        }
    }
}
