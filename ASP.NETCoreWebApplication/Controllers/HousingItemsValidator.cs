using System.Collections.Generic;
using Microsoft.Extensions.Primitives;

namespace ASP.NETCoreWebApplication.Controllers
{
    using FluentValidation;

    public class HousingItemsValidator: AbstractValidator<Dictionary<string, string>> {
        public HousingItemsValidator() {
            RuleFor(x => x["priceMin"]).Must(convertsToIntNullable);
            RuleFor(x => x["priceMax"]).Must(convertsToIntNullable);
            RuleFor(x => x["floorsMin"]).Must(convertsToInt).NotEmpty();
            RuleFor(x => x["floorsMax"]).Must(convertsToInt).NotEmpty();
            RuleFor(x => x["roomsMin"]).Must(convertsToInt).NotEmpty();
            RuleFor(x => x["roomsMax"]).Must(convertsToInt).NotEmpty();
        }

        public bool convertsToInt(string value) {
            int result;
            return int.TryParse(value, out result);
        }

        public bool convertsToIntNullable(string value) {
            if ((value) == StringValues.Empty) {
                return true;
            }
            int result;
            return int.TryParse(value, out result);
        }
    }
}