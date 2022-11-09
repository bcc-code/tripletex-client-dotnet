//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Net.Http;
//using System.Runtime.InteropServices;
//using System.Text;
//using System.Threading.Tasks;

//namespace BccCode.Tripletex.Client
//{



//        public partial class TripletexService
//        {

//            // 1. Get or Create Customer

//            // 2. Get Voucher

//            // 3. Get Products

//            // 4. Add / Update Product

//            // 5. Update Product Stock Level

//            // 6. Add / Update Voucher

//            // 7. Cancel Voucher

//            private readonly TripletexOptions _options;
//            private readonly IHttpClientFactory _clientFactory;

//            public TripletexService(TripletexOptions options, IHttpClientFactory clientFactory)
//            {
//                _options = options;
//                _clientFactory = clientFactory;
//            }

//            private static (string Token, DateTime Expiry) _sessionToken;
//            private static object _sessionTokenLock = new object();

//            public string GetSessionToken()
//            {
//                lock (_sessionTokenLock)
//                {
//                    if (_sessionToken.Token == null || _sessionToken.Expiry <= DateTime.Today)
//                    {
//                        var httpClient = _clientFactory.CreateClient();
//                        httpClient.BaseAddress = new Uri(_options.ApiBasePath);
//                        var tripletexClient = new Client(httpClient);
//                        var expiry = DateTime.Today.AddDays(30);
//                        var response = tripletexClient.TokenSessionCreate_createAsync(_options.ConsumerToken, _options.EmployeeToken, expiry.ToString("yyyy-MM-dd")).Result;
//                        var token = response?.Value?.Token;
//                        if (token != null)
//                        {
//                            _sessionToken = (token, expiry);
//                        }
//                    }
//                }
//                return _sessionToken.Token;
//            }

//            private Client _client;
//            public Client TripletexClient
//            {
//                get
//                {
//                    if (_client == null)
//                    {
//                        var sessionToken = GetSessionToken();
//                        var httpClient = _clientFactory.CreateClient();
//                        httpClient.BaseAddress = new Uri(_options.ApiBasePath);
//                        httpClient.DefaultRequestHeaders.Add("Authorization", $"Basic {Convert.ToBase64String(Encoding.UTF8.GetBytes($"0:{sessionToken}"))}");
//                        _client = new Client(httpClient);
//                    }
//                    return _client;
//                }
//            }



//            public Task<Account> GetAccountAsync(int accountNumber)
//            {
//                var cacheKey = Cache.GenerateKey<Account>(accountNumber.ToString());
//                return Cache.GetCachedAsync(cacheKey, TimeSpan.FromHours(24), async () =>
//                {
//                    return (await TripletexClient.LedgerAccount_searchAsync(
//                        id: null,
//                        number: accountNumber.ToString(),
//                        isBankAccount: null,
//                        isInactive: null,
//                        isApplicableForSupplierInvoice: null,
//                        from: null,
//                        count: null,
//                        sorting: null,
//                        fields: null
//                        )).Values.FirstOrDefault();
//                });
//            }

//            public Task<VatType> GetVatTypeAsync(int vatTypeNumber)
//            {
//                var cacheKey = Cache.GenerateKey<VatType>(vatTypeNumber.ToString());
//                return Cache.GetCachedAsync(cacheKey, TimeSpan.FromHours(24), async () =>
//                {
//                    return (await TripletexClient.LedgerVatType_searchAsync(
//                        id: null,
//                        number: vatTypeNumber.ToString(),
//                        from: null,
//                        count: null,
//                        sorting: null,
//                        fields: null
//                        )).Values.FirstOrDefault();
//                });
//            }

//            public Task<Currency> GetCurrencyAsync(string currencyCode)
//            {
//                var cacheKey = Cache.GenerateKey<Currency>(currencyCode);
//                return Cache.GetCachedAsync(cacheKey, TimeSpan.FromHours(24), async () =>
//                {
//                    return (await TripletexClient.Currency_searchAsync(
//                        id: null,
//                        code: currencyCode,
//                        from: null,
//                        count: null,
//                        sorting: null,
//                        fields: null)).Values.FirstOrDefault();
//                });
//            }

//            public Task<Department> GetDepartmentAsync(int departmentNumber)
//            {
//                var cacheKey = Cache.GenerateKey<Department>(departmentNumber.ToString());
//                return Cache.GetCachedAsync(cacheKey, TimeSpan.FromHours(24), async () =>
//                {
//                    return (await TripletexClient.Department_searchAsync(
//                        id: null,
//                        name: null,
//                        departmentNumber: departmentNumber.ToString(),
//                        departmentManagerId: null,
//                        from: null,
//                        count: null,
//                        sorting: null,
//                        fields: null
//                        )).Values.FirstOrDefault();
//                });
//            }


//            public Task<ProductUnit> GetProductUnit(string commonCode)
//            {
//                var cacheKey = Cache.GenerateKey<ProductUnit>(commonCode.ToString());
//                return Cache.GetCachedAsync(cacheKey, TimeSpan.FromHours(24), async () =>
//                {
//                    return (await TripletexClient.ProductUnit_searchAsync(
//                        id: null,
//                        name: null,
//                        nameShort: null,
//                        commonCode: commonCode,
//                        from: null,
//                        count: null,
//                        sorting: null,
//                        fields: null)).Values.FirstOrDefault();
//                });
//            }

//            public async Task<Customer> GetCustomerByCustomerNumberAsync(string customerNumber)
//            {
//                var result = await TripletexClient.Customer_searchAsync(
//                    id: null,
//                    customerAccountNumber: customerNumber,
//                    organizationNumber: null,
//                    email: null,
//                    invoiceEmail: null,
//                    isInactive: null,
//                    accountManagerId: null,
//                    from: null,
//                    count: null,
//                    sorting: null,
//                    fields: "*,category1,category2,category3");
//                if (result != null && result.Count > 0)
//                {
//                    return result.Values.FirstOrDefault();
//                }
//                return null;
//            }

//            public async Task<TripletexClient.Customer> CreateCustomerAsync(Customer customer)
//            {
//                var result = await TripletexClient.Customer_postAsync(customer);
//                return result.Value;
//            }

//            public async Task<TripletexClient.Product> GetProductByProductNumberAsync(string productNumber)
//            {
//                return (await TripletexClient.Product_searchAsync(
//                    number: null,
//                    productNumber: new List<string> { productNumber },
//                    name: null,
//                    ean: null,
//                    isInactive: null,
//                    isStockItem: null,
//                    currencyId: null,
//                    vatTypeId: null,
//                    productUnitId: null,
//                    departmentId: null,
//                    accountId: null,
//                    costExcludingVatCurrencyFrom: null,
//                    costExcludingVatCurrencyTo: null,
//                    priceExcludingVatCurrencyFrom: null,
//                    priceExcludingVatCurrencyTo: null,
//                    priceIncludingVatCurrencyFrom: null,
//                    priceIncludingVatCurrencyTo: null,
//                    from: null,
//                    count: null,
//                    sorting: null,
//                    fields: null)).Values.FirstOrDefault();
//            }

//            public async Task<List<Order>> GetSubscriptionOrdersAsync()
//            {

//                var result = await TripletexClient.Order_searchAsync(
//                    id: null,
//                    number: null,
//                    customerId: null,
//                    orderDateFrom: "2000-01-01",
//                    orderDateTo: "2050-01-01",
//                    isClosed: null,
//                    isSubscription: true,
//                    from: null,
//                    count: 10000,
//                    sorting: null,
//                    fields: "*,customer(*,postalAddress(country)),orderLines");
//                return result.Values.ToList();
//            }

//            public async Task<Order> GetOrderAsync(string orderNumber)
//            {

//                var result = await TripletexClient.Order_searchAsync(
//                    id: null,
//                    number: orderNumber,
//                    customerId: null,
//                    orderDateFrom: "2000-01-01",
//                    orderDateTo: "2050-01-01",
//                    isClosed: null,
//                    isSubscription: null,
//                    from: null,
//                    count: 1,
//                    sorting: null,
//                    fields: "*,customer,orderLines");
//                return result.Values.FirstOrDefault();
//            }

//            public async Task<Order> UpdateOrderAsync(Order order)
//            {
//                return (await TripletexClient.Order_putAsync(order.Id.Value, order)).Value;
//            }

//            public async Task<Order> CreateOrderAsync(Order order)
//            {
//                return (await TripletexClient.Order_postAsync(order)).Value;
//            }

//            public async Task<List<OrderLine>> AddOrderLinesAsync(List<OrderLine> orderLines)
//            {
//                return (await TripletexClient.OrderOrderlineList_postListAsync(orderLines)).Values.ToList();
//            }

//            public async System.Threading.Tasks.Task RemoveOrderLineAsync(OrderLine orderLine)
//            {
//                await TripletexClient.OrderOrderline_deleteAsync(orderLine.Id.Value);
//            }


//            public async Task<TripletexClient.Product> CreateProductAsync(TripletexClient.Product product)
//            {
//                var result = await TripletexClient.Product_postAsync(product);
//                return result.Value;
//            }

//            public async Task<TripletexClient.Product> UpdateProductAsync(TripletexClient.Product product)
//            {
//                var result = await TripletexClient.Product_putAsync(product.Id.Value, product);
//                return result.Value;
//            }

//            public Task<CustomerCategory> GetCustomerCategoryAsync(TripletexCategoryType type, int number)
//            {
//                var cacheKey = Cache.GenerateKey<CustomerCategory>(type.ToString(), number.ToString());
//                return Cache.GetCachedAsync(cacheKey, TimeSpan.FromHours(1), async () =>
//                {
//                    var customerCategory = await TripletexClient.CustomerCategory_searchAsync(
//                        id: null,
//                        name: null,
//                        number: number.ToString(),
//                        description: null,
//                        type: ((int)type).ToString(),
//                        from: null,
//                        count: null,
//                        sorting: null,
//                        fields: null);
//                    return customerCategory.Values.FirstOrDefault();
//                });
//            }

//            public Task<CustomerCategory> GetShopifyCustomerCategoryAsync()
//            {
//                return GetCustomerCategoryAsync(TripletexCategoryType.Category1, (int)SsfTripletexCategory1.ShopifyKunder);
//            }

//            public Task<CustomerCategory> GetVicCustomerCategoryAsync()
//            {
//                return GetCustomerCategoryAsync(TripletexCategoryType.Category1, (int)SsfTripletexCategory1.BCCMedlem);
//            }


//            public Task<List<TripletexClient.Country>> GetCountriesAsync()
//            {
//                var cacheKey = Cache.GenerateKey<TripletexClient.Country>();
//                return Cache.GetCachedAsync(cacheKey, TimeSpan.FromHours(24), async () => {
//                    return (await TripletexClient.Country_searchAsync(
//                        id: null,
//                        code: null,
//                        from: null,
//                        count: null,
//                        sorting: null,
//                        fields: null)).Values.ToList();
//                });
//            }

//            public async Task<TripletexClient.Country> GetCountryByCodeAsync(string countryCode)
//            {
//                var countries = await GetCountriesAsync();
//                return countries.FirstOrDefault(c => c.IsoAlpha2Code.Equals(countryCode, StringComparison.OrdinalIgnoreCase));
//            }

//            public async Task<TripletexClient.Customer> GetCustomerByCustomerEmailAsync(string email)
//            {
//                var result = await TripletexClient.Customer_searchAsync(
//                    id: null,
//                    customerAccountNumber: null,
//                    organizationNumber: null,
//                    email: email,
//                    invoiceEmail: null,
//                    isInactive: null,
//                    accountManagerId: null,
//                    from: null,
//                    count: null,
//                    sorting: null,
//                    fields: "*,category1,category2,category3");
//                if (result != null && result.Count > 0)
//                {
//                    return result.Values.FirstOrDefault();
//                }
//                return null;
//            }

//            public async Task<Voucher> GetVoucherByDescriptionAsync(DateTime voucherDate, string descriptionPrefix)
//            {
//                var dateFrom = voucherDate.ToString("yyyy-MM-dd");
//                var dateTo = voucherDate.AddDays(1).ToString("yyyy-MM-dd");
//                var result = (await TripletexClient.LedgerVoucher_searchAsync(
//                    id: null,
//                    number: null,
//                    numberFrom: null,
//                    numberTo: null,
//                    typeId: null,
//                    dateFrom: dateFrom,
//                    dateTo: dateTo,
//                    from: null,
//                    count: null,
//                    sorting: null,
//                    fields: "*,postings"))
//                    .Values.FirstOrDefault(v => v.Description?.StartsWith(descriptionPrefix) ?? false);
//                return result;
//            }

//            public async Task<Voucher> AddVoucherAsync(Voucher voucher, bool sendToLedger = false)
//            {
//                var result = await TripletexClient.LedgerVoucher_postAsync(sendToLedger, voucher);
//                return result?.Value;
//            }

//            public async Task<Voucher> UpdateVoucherAsync(Voucher voucher, bool sendToLedger = false)
//            {
//                var result = await TripletexClient.LedgerVoucher_putAsync(sendToLedger, voucher.Id.Value, voucher);
//                return result?.Value;
//            }

//            public async Task<Voucher> ReverseVoucherAsync(Voucher voucher, DateTime reverseDate)
//            {
//                var result = await TripletexClient.LedgerVoucherReverse_reverseAsync(voucher.Id.Value, reverseDate.ToString("yyyy-MM-dd"));
//                return result?.Value;
//            }

//            public Task<Project> GetProjectByNameAsync(string projectName, string fields = null)
//            {
//                var cacheKey = Cache.GenerateKey<Project>(projectName);
//                return Cache.GetCachedAsync(cacheKey, TimeSpan.FromHours(24), async () => {
//                    return (await TripletexClient.Project_searchAsync(
//                        id: null,
//                        name: projectName,
//                        number: null,
//                        isOffer: null,
//                        projectManagerId: null,
//                        employeeInProjectId: null,
//                        departmentId: null,
//                        startDateFrom: null,
//                        startDateTo: null,
//                        endDateFrom: null,
//                        endDateTo: null,
//                        isClosed: null,
//                        customerId: null,
//                        externalAccountsNumber: null,
//                        from: null,
//                        count: null,
//                        sorting: null,
//                        fields: fields
//                        )).Values.FirstOrDefault();
//                });
//            }

//            public async Task<Project> CreateProjectAsync(Project project)
//            {
//                var result = await TripletexClient.Project_postAsync(project);
//                return result.Value;
//            }


//            public Task<VoucherType> GetVoucherTypeByNameAsync(string voucherTypeName)
//            {
//                var cacheKey = Cache.GenerateKey<Project>(voucherTypeName);
//                return Cache.GetCachedAsync(cacheKey, TimeSpan.FromHours(24), async () => {
//                    return (await TripletexClient.LedgerVoucherType_searchAsync(
//                        name: voucherTypeName,
//                        from: null,
//                        count: null,
//                        sorting: null,
//                        fields: null)).Values.FirstOrDefault();
//                });
//            }

//        }

//        public static class TripletexExtensions
//        {
//            public static Account Identifier(this Account item)
//            {
//                return item != null ? new Account { Id = item.Id } : null;
//            }

//            public static VoucherType Identifier(this VoucherType item)
//            {
//                return item != null ? new VoucherType { Id = item.Id } : null;
//            }

//            public static Account IdentifierWithName(this Account item)
//            {
//                return item != null ? new Account { Id = item.Id, Name = item.Name } : null;
//            }

//            public static VatType Identifier(this VatType item)
//            {
//                return item != null ? new VatType { Id = item.Id } : null;
//            }

//            public static ProductUnit Identifier(this ProductUnit item)
//            {
//                return item != null ? new ProductUnit { Id = item.Id } : null;
//            }


//            public static Customer Identifier(this Customer item)
//            {
//                return item != null ? new Customer { Id = item.Id } : null;
//            }

//            public static Customer IdentifierWithName(this Customer item)
//            {
//                return item != null ? new Customer { Id = item.Id, Name = item.Name } : null;
//            }

//            public static Product Identifier(this Product item)
//            {
//                return item != null ? new Product { Id = item.Id } : null;
//            }

//            public static Currency Identifier(this Currency item)
//            {
//                return item != null ? new Currency { Id = item.Id } : null;
//            }

//            public static Project Identifier(this Project item)
//            {
//                return item != null ? new Project { Id = item.Id } : null;
//            }

//            public static Department Identifier(this Department item)
//            {
//                return item != null ? new Department { Id = item.Id } : null;
//            }

//            public static Department IdentifierWithName(this Department item)
//            {
//                return item != null ? new Department { Id = item.Id, Name = item.Name } : null;
//            }

//            public static CustomerCategory Identifier(this CustomerCategory item)
//            {
//                return item != null ? new CustomerCategory { Id = item.Id } : null;
//            }

//            public static Country Identifier(this Country item)
//            {
//                return item != null ? new Country { Id = item.Id } : null;
//            }
//        }
    

//}
