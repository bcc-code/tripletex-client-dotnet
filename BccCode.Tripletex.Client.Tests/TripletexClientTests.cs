using System.Security.AccessControl;

namespace BccCode.Tripletex.Client.Tests
{
    public class TripletexClientTests
    {
        TripletexClientOptions _options = default!;
        public TripletexClientTests() 
        {
            _options = ConfigHelper.GetOptions();
        }


        [Fact]
        public async void Test1()
        {
            var client = new TripletexClient(_options);

            var ids = ((int[]?)(null)).IdString();

            var employees = await client.GetEmployeesAsync(ids, "Ola", "Normann", null, null, null, null, null, null, null, null, null, null, null, null, null, null, null);

            var accounts = await client.GetLedgerAccountsAsync("", "", null, null, null, null, null, null, null, null, null, null);

            var rates = await client.GetEmployeeHourlyCostAndRatesAsync(employees.Values[0].Id, null, null, null, null);


            var projects = await client.GetProjectsAsync("", null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null);
            //var timeregistrations = await client.GetTimesheetEntriesAsync("", null, null, null, DateTime.Today.AddMonths(-3).ToString("yyyy-MM-dd"), DateTime.Today.ToString("yyyy-MM-dd"), null, null, null, null, null);

            var bilag = await client.AddLedgerVoucherAsync(false, new Voucher
            {
                Date = DateTime.Today.DateString(),
                Description = "SOME TEST",
                Postings = new List<Posting>
                {
                    new Posting
                    {
                        Row = 1,
                        Date= DateTime.Today.DateString(),
                        Account = accounts.Values.FirstOrDefault(a => a.Number ==  3200)!.Identifier(),
                        Description = "TEST",
                        Amount = 100,
                        AmountGross = 100,
                        AmountGrossCurrency= 100,
                        VatType = (await client.GetLedgerVatTypeAsync(6,"")).Value,
                        SystemGenerated = false
                    },
                    new Posting
                    {
                        Row = 2,
                        Date= DateTime.Today.DateString(),
                        Account = accounts.Values.FirstOrDefault(a => a.Number >=  1300)!.Identifier(),
                         VatType = (await client.GetLedgerVatTypeAsync(0,"")).Value,
                        Description = "TEST",
                        Amount = -100,
                        AmountGross = -100,
                        AmountGrossCurrency= -100,
                        SystemGenerated = false
                    }
                }

            });

           // client.AddOrderOrderlineAsync
        }
    }
}