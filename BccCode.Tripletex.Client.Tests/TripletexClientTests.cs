using System.Diagnostics;
using System.Formats.Asn1;
using System.Security.AccessControl;
using Task = System.Threading.Tasks.Task;

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
        public async System.Threading.Tasks.Task GetBudgetsTest()
        {
            var client = new TripletexClient(_options);
            var project = await client.GetProjectAsync(113087043, "*,projectActivities");
        }

        [Fact]
        public async System.Threading.Tasks.Task RateLimitingConcurrencyTest()
        {
            var client = new TripletexClient(_options);
            var start = new DateTime(DateTime.Today.Year, DateTime.Today.Month, 1);
            var end = start.AddMonths(1);
            var date = start;
            var tasks = new List<Task<ListResponsePosting>>();
            while (date < end)
            {
                for (int i = 0; i < 25; i++)
                {
                    // Simulate some concurrency
                    if (i % 20 != 0)
                    {
                        Debug.WriteLine("Starting async request: " + date.ToString("yyyy-MM-dd") + " " + i);
                        tasks.Add(client.GetLedgerPostingsAsync(date.ToString("yyyy-MM-dd"), date.ToString("yyyy-MM-dd")));
                    }
                    else
                    {
                        Debug.WriteLine("Starting sync request: " + date.ToString("yyyy-MM-dd") + " " + i);
                        await client.GetLedgerPostingsAsync(date.ToString("yyyy-MM-dd"), date.ToString("yyyy-MM-dd"));
                        Debug.WriteLine("Completed sync request: " + date.ToString("yyyy-MM-dd") + " " + i);
                    }
                }
                date = date.AddDays(1);
            }
            Debug.WriteLine("Awaiting async requests");
            await System.Threading.Tasks.Task.WhenAll(tasks);
            Debug.WriteLine("Completed async requests");
        }


        // [Fact]
        public async void Test1()
        {
            var client = new TripletexClient(_options);

            var ids = ((int[]?)(null)).IdString();

            var employees = await client.GetEmployeesAsync(ids, "Ola", "Normann", null, null, null, null, null, null, null, null, null, null, null, null, null, null, null);

            var accounts = await client.GetLedgerAccountsAsync("", "", null, null, null, null, null, null, null, null, null, null);

            var rates = await client.GetEmployeeHourlyCostAndRatesAsync(employees.Values[0].Id, null, null, null, null);


            var projects = await client.GetProjectsAsync("", null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null);
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