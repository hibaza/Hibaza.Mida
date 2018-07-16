using Firebase.Storage;
using Hangfire;
using Hibaza.CCP.Core;
using Hibaza.CCP.Domain.Entities;
using Hibaza.CCP.Service.Facebook;
using Hibaza.CCP.Service.SQL;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Hibaza.CCP.Service.Shop
{
    public interface IProductService
    {
        Task<List<string>> GetStoreList();
        Task<List<string>> GetProductList(string storeId);
    }

    public class ProductService : IProductService
    {
        private readonly ILoggingService _logService;
        private readonly string _apiUrl = "http://api.baza.vn";


        public ProductService(ILoggingService logService)
        {
            _logService = logService;
        }

       public async Task<List<string>> GetStoreList()
        {
            throw new NotImplementedException();
        }

        public async Task<List<string>> GetProductList(string storeId)
        {
            throw new NotImplementedException();
        }
    }
}