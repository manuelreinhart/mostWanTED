using Consul;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Common.Tools
{
    public class ServiceDiscovery
    {
        public static readonly ServiceDiscovery Singleton = new ServiceDiscovery();

        private ConsulClient ConsulClient;


        private ServiceDiscovery()
        {
            var consulConfig = new ConsulClientConfiguration
            {
                Address = new Uri("http://consul.westeurope.azurecontainer.io:8500/"),
                Datacenter = "dc1"
            };

            this.ConsulClient = new ConsulClient(consulConfig);

        }    

        public async Task<Uri> GetServiceUrlByTag(string tag)
        {
            try
            {
                var service = await this.GetServiceByTagAsync(tag);
                if (service == null)
                    return null;

                var uriBuilder = new UriBuilder(service.ServiceAddress);
                uriBuilder.Port = service.ServicePort;
                uriBuilder.Scheme = "https";

                return uriBuilder.Uri;
            }
            catch (Exception)
            {
                return new Uri("http://localhost");
            }
        }

        public async Task<CatalogService> GetServiceByTagAsync(string tag)
        {          
            var service = await this.ConsulClient.Catalog.Service(tag);
            var services = service.Response;
            return services.FirstOrDefault();    
        }

        public async Task RegisterAppServiceAsService(string name)
        {
            try
            {
                var website = Environment.GetEnvironmentVariable("WEBSITE_SITE_NAME");
                var urlString = $"https://{website}.azurewebsites.net";
                var uri = new Uri(urlString);
                await this.RegisterService(website, name, uri.Host, uri.Port);
            }
            catch (Exception)
            {
                Console.WriteLine("Cannot register service");
            }
            
        }

        public async Task RegisterService(string id, string name, string address, int port, string[] tags = null)
        {
            var service = new AgentServiceRegistration();
            service.ID = id;
            service.Name = name;
            service.Address = address;            
            service.Port = port;
            service.Tags = tags;                    

            await this.ConsulClient.Agent.ServiceRegister(service);            
        }

        public async Task<string> GetConfigValueByKey(string key)
        {
            var res = await this.ConsulClient.KV.Get(key);
            var value = System.Text.Encoding.Default.GetString(res.Response.Value);

            return value;
        }


    }
}
