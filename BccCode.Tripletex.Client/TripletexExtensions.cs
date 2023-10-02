using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BccCode.Tripletex.Client
{
    public static class TripletexExtensions
    {
        public static string DateString(this DateTime item) => item.ToString("yyyy-MM-dd", System.Globalization.CultureInfo.InvariantCulture);

        public static string? IdString(this IEnumerable<int> ids) => ids != null ? (ids.Any() ? ids.Select(i => i.ToString()).Aggregate((c,n) => $"{c},{n}") : string.Empty) : null;

        public static Account? Identifier(this Account item)
        {
            return item != null ? new Account { Id = item.Id } : null;
        }

        public static VoucherType? Identifier(this VoucherType item)
        {
            return item != null ? new VoucherType { Id = item.Id } : null;
        }

        public static Account? IdentifierWithName(this Account item)
        {
            return item != null ? new Account { Id = item.Id, Name = item.Name } : null;
        }

        public static VatType? Identifier(this VatType item)
        {
            return item != null ? new VatType { Id = item.Id } : null;
        }

        public static ProductUnit? Identifier(this ProductUnit item)
        {
            return item != null ? new ProductUnit { Id = item.Id } : null;
        }


        public static Customer? Identifier(this Customer item)
        {
            return item != null ? new Customer { Id = item.Id } : null;
        }

        public static Customer? IdentifierWithName(this Customer item)
        {
            return item != null ? new Customer { Id = item.Id, Name = item.Name } : null;
        }

        public static Product? Identifier(this Product item)
        {
            return item != null ? new Product { Id = item.Id } : null;
        }

        public static Currency? Identifier(this Currency item)
        {
            return item != null ? new Currency { Id = item.Id } : null;
        }

        public static Project? Identifier(this Project item)
        {
            return item != null ? new Project { Id = item.Id, ProjectManager = null } : null;
        }

        public static Department? Identifier(this Department item)
        {
            return item != null ? new Department { Id = item.Id, DepartmentManager = null } : null;
        }

        public static Department? IdentifierWithName(this Department item)
        {
            return item != null ? new Department { Id = item.Id, Name = item.Name } : null;
        }

        public static CustomerCategory? Identifier(this CustomerCategory item)
        {
            return item != null ? new CustomerCategory { Id = item.Id } : null;
        }

        public static Country? Identifier(this Country item)
        {
            return item != null ? new Country { Id = item.Id } : null;
        }
    }
}
