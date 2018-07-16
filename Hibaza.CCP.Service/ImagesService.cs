using Hibaza.CCP.Core;
using Hibaza.CCP.Data.Infrastructure;
using Hibaza.CCP.Data.Repositories;
using Hibaza.CCP.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hibaza.CCP.Service
{
    public class ImagesService 
    {
        public async static Task<string> UpsertImageStore(string imageUrl, AppSettings appSettings)
        {
            return await Core.Helpers.WebHelper.HttpGetAsync<string>(appSettings.BaseUrls.ApiSaveImage + "api/File_SaveFromUrl?url=" + imageUrl);
        }       
    }
}
