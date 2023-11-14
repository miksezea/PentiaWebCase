using Microsoft.AspNetCore.Mvc;
using PentiaWebCase.Models;
using System.Globalization;
using System.Net.Http.Headers;

namespace PentiaWebCase.Controllers
{
    public class SalesPersonController : Controller
    {
        private const string api = "https://azurecandidatetestapi.azurewebsites.net/api/v1.0/";
        private readonly string baseUrl = api;

        public ActionResult Index()
        {
            List<SalesPerson> salesPersons = GetAllSalesPersons();

            if (salesPersons != null)
            {
                return View(salesPersons);
            }
            else
            {
                return NotFound();
            }
        }

        public ActionResult Details(int id)
        {
            var salesPerson = GetSalesPersonById(id);

            if (salesPerson != null)
            {
                return View(salesPerson);
            }
            else
            {
                return NotFound();
            }

        }

        public ActionResult OrderHistory()
        {
            List<MonthlyOrders> monthlyOrders = GetOrdersPerMonth();

            if (monthlyOrders != null)
            {
                return View(monthlyOrders);
            }
            else
            {
                return NotFound();
            }
        }

        // Functions
        private List<SalesPerson> GetAllSalesPersons()
        {
            IEnumerable<SalesPerson> salesPersons;
            IEnumerable<Orderline> orderlines;

            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(baseUrl);
                client.DefaultRequestHeaders.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                client.DefaultRequestHeaders.Add("ApiKey", "test1234");

                var sPResponseTask = client.GetAsync("SalesPersons");
                sPResponseTask.Wait();
                var sPResult = sPResponseTask.Result;

                if (sPResult.IsSuccessStatusCode)
                {
                    var OlResponseTask = client.GetAsync("OrderLines");
                    OlResponseTask.Wait();
                    var OlResult = OlResponseTask.Result;

                    if (OlResult.IsSuccessStatusCode)
                    {
                        var readSpTask = sPResult.Content.ReadAsAsync<IList<SalesPerson>>();
                        readSpTask.Wait();
                        salesPersons = readSpTask.Result;

                        var readOlTask = OlResult.Content.ReadAsAsync<IList<Orderline>>();
                        readOlTask.Wait();
                        orderlines = readOlTask.Result;

                        foreach (var person in salesPersons)
                        {
                            person.Orderlines = orderlines.Where(x => x.SalesPersonId == person.Id).ToList();
                        }
                    }
                    else
                    {
                        var readSpTask = sPResult.Content.ReadAsAsync<IList<SalesPerson>>();
                        readSpTask.Wait();
                        salesPersons = readSpTask.Result;

                        Console.WriteLine($"Failed to recieve Orderlines. Server returned {OlResult.StatusCode}");
                    }
                }
                else
                {
                    throw new InvalidOperationException($"Failed to retrieve SalesPersons. Server returned {sPResult.StatusCode}");
                }
            }
            return salesPersons.ToList();
        }


        private SalesPerson GetSalesPersonById(int id)
        {
            SalesPerson salesPerson;

            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(baseUrl);
                client.DefaultRequestHeaders.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                client.DefaultRequestHeaders.Add("ApiKey", "test1234");

                var responseTask = client.GetAsync("SalesPersons");
                responseTask.Wait();
                var result = responseTask.Result;
                if (result.IsSuccessStatusCode)
                {
                    var readTask = result.Content.ReadAsAsync<IList<SalesPerson>>();
                    readTask.Wait();
                    var foundPersons = readTask.Result;

                    salesPerson = foundPersons.FirstOrDefault(p => p.Id == id);

                    if (salesPerson == null)
                    {
                        throw new InvalidOperationException($"Salesperson with ID {id} not found.");
                    }

                    salesPerson.Orderlines = GetOrdersBySalesPerson(id, client);
                }
                else
                {
                    throw new InvalidOperationException($"Failed to retrieve SalesPersons. Server returned {result.StatusCode}");
                }
            }
            return salesPerson;
        }

        private List<Orderline> GetOrdersBySalesPerson(int id, HttpClient client)
        {
            IEnumerable<Orderline> orderlines;

            var responseTask = client.GetAsync("OrderLines");
            responseTask.Wait();
            var result = responseTask.Result;

            if (result.IsSuccessStatusCode)
            {
                var readTask = result.Content.ReadAsAsync<IList<Orderline>>();
                readTask.Wait();
                orderlines = readTask.Result;
            }
            else
            {
                throw new InvalidOperationException($"Failed to retrieve Orderlines. Server returned {result.StatusCode}");
            }
            return orderlines.Where(x => x.SalesPersonId == id).ToList();
        }

        public List<MonthlyOrders> GetOrdersPerMonth()
        {
            IEnumerable<Orderline> orderlines;

            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(baseUrl);
                client.DefaultRequestHeaders.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                client.DefaultRequestHeaders.Add("ApiKey", "test1234");

                var responseTask = client.GetAsync("OrderLines");
                responseTask.Wait();
                var result = responseTask.Result;

                if (result.IsSuccessStatusCode)
                {
                    var readTask = result.Content.ReadAsAsync<IList<Orderline>>();
                    readTask.Wait();
                    orderlines = readTask.Result;
                }
                else
                {
                    throw new InvalidOperationException($"Failed to retrieve Orderlines. Server returned {result.StatusCode}");
                }
            }

            List<MonthlyOrders> monthlyOrders = new();

            foreach (var orderline in orderlines)
            {
                DateTime orderDateTime = DateTime.Parse(orderline.OrderDate);
                string year = orderDateTime.ToString("yyyy");
                string month = orderDateTime.ToString("MMMM", new CultureInfo("en-GB"));

                MonthlyOrders monthWithOrder = new(year, month, 1);

                if (monthlyOrders.Exists(x => x.Year == year && x.Month == month))
                {
                    var index = monthlyOrders.FindIndex(x => x.Year == year && x.Month == month);
                    monthlyOrders[index].OrderCount++;
                }
                else
                {
                    monthlyOrders.Add(monthWithOrder);
                }
            }
            return monthlyOrders;
        }
    }
}
