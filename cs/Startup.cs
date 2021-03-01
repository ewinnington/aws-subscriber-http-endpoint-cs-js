using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Models;

namespace cs
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }
        static readonly HttpClient client = new HttpClient(); 

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            app.UseRouting();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapGet("/", async context =>
                {
                    await context.Response.WriteAsync("Hello World"); 
                });

                endpoints.MapPost("/subs/", async context => await subscribe(context, false)); 
                endpoints.MapPost("/subs-raw/", async context => await subscribe(context, true)); 
            });
        }

        private async Task subscribe(HttpContext context, bool isRaw)
        {
            string body;
            using (StreamReader reader = new StreamReader(context.Request.Body, Encoding.UTF8))
            {  
                body = await reader.ReadToEndAsync();
            }

            if(isRaw)
            {
                if(body[0] == '<') 
                {
                    Console.Write(body); //full xml received
                    context.Response.StatusCode = 200; 
                    return;
                }
            }

            JsonDocument request = JsonDocument.Parse(body);

            var requestType = request.RootElement.GetProperty("Type"); 

            switch(requestType.GetString()) {
                case "SubscriptionConfirmation" : 
                    try 
                    {
                        Uri uri = new Uri( request.RootElement.GetProperty("SubscribeURL").GetString());
                        HttpResponseMessage response = await client.GetAsync(uri); 
                        response.EnsureSuccessStatusCode(); 
                        Console.WriteLine("Subscribed with success " + (isRaw ? "with raw" : "with json")); 
                        context.Response.StatusCode = 200; 
                    }
                    catch(HttpRequestException e)
                    {
                        Console.WriteLine("\nException Caught!");	
                        Console.WriteLine("Message :{0} ",e.Message);
                        context.Response.StatusCode = 403;
                    }
                    return; 
                case "UnsubscribeConfirmation" : 
                    Console.WriteLine("unsubscribed");
                    context.Response.StatusCode = 200; 
                return; 
                case "Notification" : 
                    Console.WriteLine(request.RootElement.GetProperty("Message").GetString());
                    context.Response.StatusCode = 200;  
                return; 
            }

        }
    }
}
