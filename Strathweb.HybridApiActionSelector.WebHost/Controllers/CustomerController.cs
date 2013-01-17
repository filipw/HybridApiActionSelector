using System.Web.Http;
using Strathweb.HybridApiActionSelector.WebHost.Models;

namespace Strathweb.HybridApiActionSelector.WebHost.Controllers
{
    public class CustomerController : ApiController
    {
        // /api/customer/
        public string Get()
        {
            return "All Customers";
        }

        // /api/customer/
        public string Post(Customer c)
        {
            return "Customer with Id: " + c.Id + " added";
        }

        // /api/customer/1
        public string GetById(int id)
        {
            return "Customer " + id;
        }

        // /api/customer/1/orders
        [HttpGet]
        public string Orders(int id)
        {
            return "All Orders of customer " + id;
        }

        // /api/customer/1/orders/3
        [HttpGet]
        public string Orders(int id, int actionid)
        {
            return "Order with id " + actionid + " of customer " + id;
        }

        // /api/customer/1/orders
        [HttpPost]
        public string Orders(int id, Order order)
        {
            return "Order with Id: " + order.Id + " of customer " + id + " added";
        }

        // /api/customer/1/orders
        [HttpGet]
        public string Shipments(int id, int actionid)
        {
            return "All shipments of Order with id " + actionid + " of customer " + id;
        }

        // /api/customer/1/orders/3/shipments
        [HttpGet]
        public string Shipments(int id, int actionid, int subactionid)
        {
            return "Shipment with Id: " + subactionid + " of order with id " + actionid + " of customer " + id;
        }

        // /api/customer/1/orders/3/shipments/1
        [HttpPost]
        public string Shipments(int id, int actionid, Shipment shipment)
        {
            return "Shipment with Id: " + shipment.Id + " of order " + actionid + " of customer " + id + " added";
        }
    }
}