using ParkyWeb.Repository.IRepository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using Newtonsoft.Json;
using System.Runtime.InteropServices;

namespace ParkyWeb.Repository
{
    public class Repository<T> : IRepository<T> where T : class
    {
        private readonly IHttpClientFactory _clientFactory;


        public Repository(IHttpClientFactory clientFactory)
        {
            this._clientFactory = clientFactory;                   
        }


        public async Task<bool> CreateAsync(string url, T objectToCreate)
        {
            var request = new HttpRequestMessage(HttpMethod.Post, url);
            if(objectToCreate != null)
            {
                /*request.Content = new StringContent(JsonConvert
                    .SerializeObject(objectToCreate),
                    Encoding.UTF8, "application/json");*/

                request.Content = new StringContent(JsonConvert
                    .SerializeObject(objectToCreate, Formatting.Indented),
                    Encoding.UTF8, "application/json");
            }
            else 
            {
                return false;
            }

            var client = this._clientFactory.CreateClient();
            HttpResponseMessage response = await client.SendAsync(request);

            return response.StatusCode == System.Net.HttpStatusCode.Created? true : false;
        }

        public async Task<bool> DeleteAsync(string url, int id)
        {
            var request = new HttpRequestMessage(HttpMethod.Delete, url+id);

            var client = this._clientFactory.CreateClient();

            HttpResponseMessage response = await client.SendAsync(request);

            /*if(response.StatusCode == System.Net.HttpStatusCode.NoContent)
            {
                return true;
            }

            return false;*/

            return response.StatusCode == System.Net.HttpStatusCode.NoContent? true : false;
        }

        
        public async Task<IEnumerable<T>> GetAllAsync(string url)
        {
            var request = new HttpRequestMessage(HttpMethod.Get, url);

            var client = this._clientFactory.CreateClient();

            HttpResponseMessage response = await client.SendAsync(request);

            if (response.StatusCode == System.Net.HttpStatusCode.OK)
            {
                var jsonString = await response.Content.ReadAsStringAsync();

                return JsonConvert.DeserializeObject<IEnumerable<T>>(jsonString);
            }

            return null;
        }

        public async Task<T> GetAsync(string url, int Id)
        {
            var request = new HttpRequestMessage(HttpMethod.Get, url+Id);

            var client = this._clientFactory.CreateClient();

            HttpResponseMessage response = await client.SendAsync(request);

            if (response.StatusCode == System.Net.HttpStatusCode.OK)
            {
                var jsonString = await response.Content.ReadAsStringAsync();
                return JsonConvert.DeserializeObject<T>(jsonString);
            }

            return null;
        }

        public async Task<bool> UpdateAsync(string url, T objectToUpdate)
        {
            var request = new HttpRequestMessage(HttpMethod.Patch, url);
            if (objectToUpdate != null)
            {
                request.Content = new StringContent(JsonConvert
                    .SerializeObject(objectToUpdate, Formatting.Indented),
                    Encoding.UTF8, "application/json");             

                /*request.Content = new StringContent(JsonConvert
                    .SerializeObject(objectToUpdate),
                    Encoding.UTF8, "application/json");*/
            }
            else
            {
                return false;
            }

            var client = this._clientFactory.CreateClient();
            HttpResponseMessage response = await client.SendAsync(request);

            return response.StatusCode == System.Net.HttpStatusCode.NoContent ? true : false;
        }
    }
}
