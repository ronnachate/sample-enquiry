﻿using System;
using Sample.Enquiry.Core.Entities;
using Sample.Enquiry.Core.Dtos;
using Sample.Enquiry.Api;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using Xunit;

namespace Sample.Enquiry.Tests.Integration.Web.Api
{
    public class CustomerController : IClassFixture<CustomWebApplicationFactory<Startup>>
    {
        private readonly HttpClient _client;

        public CustomerController(CustomWebApplicationFactory<Startup> factory)
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
    }
}
