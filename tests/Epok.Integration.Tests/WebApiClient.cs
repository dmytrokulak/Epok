using System;
using System.Collections.Generic;
using System.Linq;
using Epok.Domain.Contacts.Entities;
using Epok.Domain.Customers;
using Epok.Domain.Customers.Entities;
using Epok.Domain.Orders.Entities;
using Epok.Presentation.Model.Customers;
using Newtonsoft.Json;
using RestSharp;

namespace Epok.Integration.Tests
{
    internal static class WebApiClient
    {
        internal const string EpokBaseUrl = "https://localhost:5001/api";

        private static readonly IRestClient _client = new RestClient(EpokBaseUrl);

        private static Guid Execute(string url, object body)
        {
            var content = Execute(url, Method.POST, body);
            var id = Guid.Parse(content.Trim('"'));
            if (id == Guid.Empty)
                throw new Exception("Empty guid returned");
            return id;
        }

        private static T Execute<T>(string url, Method method, object body = null,
            IDictionary<string, string> queryParams = null) where T : class
        {
            if (queryParams != null)
                url = queryParams.Where(param => !string.IsNullOrWhiteSpace(param.Value))
                    .Aggregate(url + "?", (current, param) => current + $"{param.Key}={param.Value}&")
                    .TrimEnd('&', '?');

            var content = Execute(url, method, body);
            return JsonConvert.DeserializeObject<T>(content);
        }

        private static string Execute(string url, Method method, object body = null)
        {
            var request = new RestRequest(url) {Method = method};
            if (body != null && (method != Method.GET || method != Method.DELETE))
                request.AddJsonBody(body);

            var response = _client.Execute(request);

            if (!response.IsSuccessful)
                throw new Exception(response.Content);

            return response.Content;
        }

        internal static class System
        {
            internal static bool Ping()
            {
                try
                {
                    Execute("system/ping", Method.GET);
                    return true;
                }
                catch (Exception)
                {
                    return false;
                }
            }
        }
        

        internal static class Customers
        {
            internal static IEnumerable<Customer> Get(string nameLike = null, CustomerType? typeExact = null,
            string countryExact = null, string provinceExact = null, string cityExact = null)
            {
                var dict = new Dictionary<string, string>
                {
                    {nameof(nameLike), nameLike},
                    {nameof(countryExact), countryExact},
                    {nameof(provinceExact), provinceExact},
                    {nameof(cityExact), cityExact}
                };
                if(typeExact.HasValue)
                    dict.Add(nameof(typeExact),((int)typeExact.Value).ToString());

                return Execute<IEnumerable<Customer>>("customers", Method.GET, queryParams: dict);
            }

            internal static Customer Get(Guid id) =>
                Execute<Customer>($"customers/{id}", Method.GET);

            internal static IEnumerable<Order> GetOrders(Guid id) =>
                Execute<IEnumerable<Order>>($"customers/{id}/orders", Method.GET);

            internal static IEnumerable<Contact> GetContacts(Guid id) =>
                Execute<IEnumerable<Contact>>($"customers/{id}/contacts", Method.GET);

            internal static Guid Post(Customer entity) =>
                Execute("customers", EpokMapper.Map<RegisterCustomerModel>(entity));

            internal static void PutCustomerType(Guid id, CustomerType type ) =>
                Execute($"customers/{id}/type", Method.PUT, new ChangeCustomerTypeModel { NewCustomerType = type });

            internal static void PutCustomerAddress(Guid id, Address entity) =>
                Execute($"customers/{id}/address", Method.PUT, EpokMapper.Map<ChangeCustomerAddressModel>(entity));

            internal static Guid PostCustomerContact(Guid id, Contact entity) =>
                Execute($"customers/{id}/contact", EpokMapper.Map<ContactModel>(entity));

            internal static void PutCustomerContact(Guid id, Guid subId, Contact entity) =>
                Execute($"customers/{id}/contact/{subId}", Method.PUT, EpokMapper.Map<ContactModel>(entity));

            internal static void PutCustomerContactAsPrimary(Guid id, Guid subId) =>
                Execute($"customers/{id}/contact/{subId}/primary", Method.PUT);

            internal static void Delete(Guid id) =>
                Execute($"customers/{id}", Method.DELETE);

            internal static void DeleteContact(Guid id, Guid subId) =>
                Execute($"customers/{id}/contact/{subId}", Method.DELETE);
        }
    }
}
