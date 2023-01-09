using System.Collections.Generic;
using System.Linq;
using ASP.NETCoreWebApplication.Scrappers;
using Microsoft.Extensions.Primitives;

namespace ASP.NETCoreWebApplication.Controllers
{
    using FluentValidation;

    public class HousingItemsValidator: AbstractValidator<Dictionary<string, string>> {
        public HousingItemsValidator() {
            RuleFor(x => x["priceMin"]).Must(ConvertsToIntNullable);
            RuleFor(x => x["priceMax"]).Must(ConvertsToIntNullable);
            RuleFor(x => x["floorsMin"]).Must(ConvertsToInt).NotEmpty();
            RuleFor(x => x["floorsMax"]).Must(ConvertsToInt).NotEmpty();
            RuleFor(x => x["roomsMin"]).Must(ConvertsToInt).NotEmpty();
            RuleFor(x => x["roomsMax"]).Must(ConvertsToInt).NotEmpty();
            RuleFor(x => x["areaMin"]).Must(ConvertsToInt).NotEmpty();
            RuleFor(x => x["areaMax"]).Must(ConvertsToInt).NotEmpty();
            RuleFor(x => x["dataSources"]).Must(OneOfDataSources).NotEmpty();
            RuleFor(x => x["propertyType"]).Must(OneOfHousingTypes).NotEmpty();
        }

        public static bool ConvertsToInt(string value) {
            int result;
            return int.TryParse(value, out result);
        }

        private bool OneOfDataSources(string value) {
            var dataSources = value.Split(",");
            string[] availableDataSources = DataSources.Values();
            return dataSources.All(dataSource => availableDataSources.Contains(dataSource));
        }

        private bool OneOfHousingTypes(string value) {
            return HousingType.Values().Contains(value);
        }

        private bool ConvertsToIntNullable(string value) {
            if ((value) == StringValues.Empty) {
                return true;
            }
            int result;
            return int.TryParse(value, out result);
        }
    }
}