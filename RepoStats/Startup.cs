using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.SpaServices.ReactDevelopmentServer;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Clients.ActiveDirectory;
using Microsoft.Rest;
using System.Data.SqlClient;
using System.IO;
using Microsoft.Extensions.Azure;
using Azure.Storage.Queues;
using Azure.Storage.Blobs;
using Azure.Core.Extensions;
using System;

namespace RepoStats
    {
    public class ConnectionConfig
        {
        public string StorageConnectionString { get; set; }
        public string DbConnectionString { get; set; }
        }

    public class Startup
        {
        public Startup(IConfiguration configuration)
            {
            Configuration = configuration;
            string dbConnectionString = configuration.GetSection("ConnectionStrings").GetValue<string>("DbConnectionString");

            SetupDatabase(dbConnectionString);
            }

        //private static void GetPowerBIAccessToken()
        //    {
        //    // For app only authentication, we need the specific tenant id in the authority url
        //    var tenantSpecificURL = AuthorityUrl.Replace("common", Tenant);
        //    var authenticationContext = new AuthenticationContext(tenantSpecificURL);

        //    // Authentication using app credentials
        //    var credential = new ClientCredential(ApplicationId, ApplicationSecret);
        //    authenticationResult = await authenticationContext.AcquireTokenAsync(ResourceUrl, credential);

        //    var m_tokenCredentials = new TokenCredentials(authenticationResult.AccessToken, "Bearer");

        //    }

        private static void SetupDatabase(string connectionString)
            {
            using (var reader = new StreamReader("./Schema.sql"))
            using (SqlConnection connection = new SqlConnection(connectionString))
                {
                string sqlContent = reader.ReadToEnd();
                SqlCommand command = new SqlCommand(sqlContent, connection);

                connection.Open();
                int test = command.ExecuteNonQuery();
                }
            }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
            {
            services.AddOptions();
            services.Configure<ConnectionConfig>(Configuration.GetSection("ConnectionStrings"));

            services.AddControllersWithViews();

            // In production, the React files will be served from this directory
            services.AddSpaStaticFiles(configuration =>
            {
                configuration.RootPath = "ClientApp/build";
            });
            services.AddAzureClients(builder =>
            {
                builder.AddBlobServiceClient(Configuration["ConnectionStrings:StorageConnectionString:blob"], preferMsi: true);
                builder.AddQueueServiceClient(Configuration["ConnectionStrings:StorageConnectionString:queue"], preferMsi: true);
            });
            }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
            {
            if (env.IsDevelopment())
                {
                app.UseDeveloperExceptionPage();
                }
            else
                {
                app.UseExceptionHandler("/Error");
                }

            app.UseStaticFiles();
            app.UseSpaStaticFiles();

            app.UseRouting();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller}/{action=Index}/{id?}");
            });

            app.UseSpa(spa =>
            {
                spa.Options.SourcePath = "ClientApp";

                if (env.IsDevelopment())
                    {
                    spa.UseReactDevelopmentServer(npmScript: "start");
                    }
            });
            }
        }
    internal static class StartupExtensions
        {
        public static IAzureClientBuilder<BlobServiceClient, BlobClientOptions> AddBlobServiceClient(this AzureClientFactoryBuilder builder, string serviceUriOrConnectionString, bool preferMsi)
            {
            if (preferMsi && Uri.TryCreate(serviceUriOrConnectionString, UriKind.Absolute, out Uri serviceUri))
                {
                return builder.AddBlobServiceClient(serviceUri);
                }
            else
                {
                return builder.AddBlobServiceClient(serviceUriOrConnectionString);
                }
            }
        public static IAzureClientBuilder<QueueServiceClient, QueueClientOptions> AddQueueServiceClient(this AzureClientFactoryBuilder builder, string serviceUriOrConnectionString, bool preferMsi)
            {
            if (preferMsi && Uri.TryCreate(serviceUriOrConnectionString, UriKind.Absolute, out Uri serviceUri))
                {
                return builder.AddQueueServiceClient(serviceUri);
                }
            else
                {
                return builder.AddQueueServiceClient(serviceUriOrConnectionString);
                }
            }
        }
    }
