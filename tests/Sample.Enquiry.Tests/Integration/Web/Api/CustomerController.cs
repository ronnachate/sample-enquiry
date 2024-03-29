﻿using System;
using Sample.Enquiry.Core.Dtos;
using Sample.Enquiry.Api;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Globalization;

using System.Threading.Tasks;
using Xunit;

namespace Sample.Enquiry.Tests.Integration.Web.Api
{
    public class CustomerControllerTest : IClassFixture<CustomWebApplicationFactory<Startup>>
    {
        private readonly HttpClient _client;

        public CustomerControllerTest(CustomWebApplicationFactory<Startup> factory)
        {
            _client = factory.CreateClient();
        }

        [Fact]
        public async Task Get_All_Customer_Success()
        {
            var response = await _client.GetAsync("/api/customers");
            response.EnsureSuccessStatusCode();
            var stringResponse = await response.Content.ReadAsStringAsync();
            var result = JsonConvert.DeserializeObject<IEnumerable<CustomerDto>>(stringResponse).ToList();

            Assert.Equal(3, result.Count());
            Assert.Equal(1, result.Count(a => a.Name == "Customer without transaction"));
            Assert.Equal(1, result.Count(a => a.Name == "Customer with 1 transaction"));
            Assert.Equal(1, result.Count(a => a.Name == "Customer with 2 transactions"));
        }
        [Fact]
        public async Task Get_Customer_By_Id_Success()
        {
            ulong id = 123456;
            var response = await _client.GetAsync("/api/customer/" + id.ToString());
            response.EnsureSuccessStatusCode();
            var stringResponse = await response.Content.ReadAsStringAsync();
            var result = JsonConvert.DeserializeObject<CustomerDto>(stringResponse);
            Assert.NotNull(result);
            Assert.Equal(id, result.Id);
            Assert.Equal("Customer without transaction", result.Name);
        }
        [Fact]
        public async Task Get_Customer_By_Id_Params_Success()
        {
            ulong id = 123456;
            var response = await _client.GetAsync("/api/customer?CustomerId=" + id.ToString());
            response.EnsureSuccessStatusCode();
            var stringResponse = await response.Content.ReadAsStringAsync();
            var result = JsonConvert.DeserializeObject<CustomerDto>(stringResponse);
            Assert.NotNull(result);
            Assert.Equal(id, result.Id);
            Assert.Equal("Customer without transaction", result.Name);
        }
        [Fact]
        public async Task Get_Customer_By_Email_Params_Success()
        {
            string email = "user2@domain.com";
            var response = await _client.GetAsync("/api/customer?Email=" + email);
            response.EnsureSuccessStatusCode();
            var stringResponse = await response.Content.ReadAsStringAsync();
            var result = JsonConvert.DeserializeObject<CustomerDto>(stringResponse);
            Assert.NotNull(result);
            Assert.Equal(email, result.Email);
            Assert.Equal("Customer with 1 transaction", result.Name);
        }
        [Fact]
        public async Task Get_Customer_By_Id_Params_Notfound()
        {
            ulong id = 123456789;
            var response = await _client.GetAsync("/api/customer?CustomerId=" + id.ToString());
            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        }
        [Fact]
        public async Task Get_Customer_By_EMAIL_Params_Notfound()
        {
            string email = "notfound@domain.com";
            var response = await _client.GetAsync("/api/customer?Email=" + email);
            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        }

        [Fact]
        public async Task Get_Customer_NO_Params_BadRequest()
        {
            var response = await _client.GetAsync("/api/customer");
            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
            var stringResponse = await response.Content.ReadAsStringAsync();
            Assert.Matches("No inquiry criteria", stringResponse);
        }
        [Fact]
        public async Task Get_Customer_Invalid_Id_Params_BadRequest()
        {
            ulong id = 1234567890123;
            var response = await _client.GetAsync("/api/customer?CustomerId=" + id.ToString());
            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
            var stringResponse = await response.Content.ReadAsStringAsync();
            Assert.Matches("Invalid Customer Id", stringResponse);
        }
        [Fact]
        public async Task Get_Customer_Invalid_Email_Params_BadRequest()
        {
            string email = "badrequest";
            var response = await _client.GetAsync("/api/customer?Email=" + email);
            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
            var stringResponse = await response.Content.ReadAsStringAsync();
            Assert.Matches("Invalid Email", stringResponse);
        }
        [Fact]
        public async Task Get_Customer_Without_Transaction_Success()
        {
            ulong id = 123456;
            var response = await _client.GetAsync("/api/customer/" + id.ToString());
            response.EnsureSuccessStatusCode();
            var stringResponse = await response.Content.ReadAsStringAsync();
            var result = JsonConvert.DeserializeObject<CustomerDto>(stringResponse);
            Assert.NotNull(result);
            Assert.Equal(id, result.Id);
            Assert.Equal("Customer without transaction", result.Name);
        }
        [Fact]
        public async Task Get_Customer_With_1_Transaction_Success()
        {
            ulong id = 234567;
            var response = await _client.GetAsync("/api/customer/" + id.ToString());
            response.EnsureSuccessStatusCode();
            var stringResponse = await response.Content.ReadAsStringAsync();
            var result = JsonConvert.DeserializeObject<CustomerDto>(stringResponse);
            Assert.NotNull(result);
            Assert.Equal(id, result.Id);
            Assert.Equal("Customer with 1 transaction", result.Name);
            Assert.Single(result.Transactions);
        }
        [Fact]
        public async Task Get_Customer_With_2_Transaction_Success()
        {
            ulong id = 345678;
            var response = await _client.GetAsync("/api/customer/" + id.ToString());
            response.EnsureSuccessStatusCode();
            var stringResponse = await response.Content.ReadAsStringAsync();
            var result = JsonConvert.DeserializeObject<CustomerDto>(stringResponse);
            Assert.NotNull(result);
            Assert.Equal(id, result.Id);
            Assert.Equal("Customer with 2 transactions", result.Name);
            Assert.Equal(2, result.Transactions.Count);
        }
        [Fact]
        public async Task GetCorrect_Transaction_Date_Format()
        {
            ulong id = 234567;
            var response = await _client.GetAsync("/api/customer/" + id.ToString());
            response.EnsureSuccessStatusCode();
            var stringResponse = await response.Content.ReadAsStringAsync();
            var result = JsonConvert.DeserializeObject<CustomerDto>(stringResponse);
            Assert.NotNull(result);
            Assert.Equal(id, result.Id);
            Assert.Equal("Customer with 1 transaction", result.Name);
            Assert.Single(result.Transactions);
            var transaction = Assert.IsType<TransactionDto>(result.Transactions.First());
            var transactionDate = DateTime.ParseExact(transaction.TransactionDate, "dd/MM/yy HH:mm", CultureInfo.InvariantCulture);
            Assert.IsType<DateTime>(transactionDate);
        }
        [Fact]
        public async Task GetCorrect_Transaction_Amount_Format()
        {
            ulong id = 234567;
            var response = await _client.GetAsync("/api/customer/" + id.ToString());
            response.EnsureSuccessStatusCode();
            var stringResponse = await response.Content.ReadAsStringAsync();
            var result = JsonConvert.DeserializeObject<CustomerDto>(stringResponse);
            Assert.NotNull(result);
            Assert.Equal(id, result.Id);
            Assert.Equal("Customer with 1 transaction", result.Name);
            Assert.Single(result.Transactions);
            var transaction = Assert.IsType<TransactionDto>(result.Transactions.First());
            Assert.Matches("^[0-9]*(\\.[0-9]{1,2})?$", transaction.Amount);
        }
    }
}
