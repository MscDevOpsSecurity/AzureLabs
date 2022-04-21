using System;
using System.Security.Cryptography.X509Certificates;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;

namespace Module4Lab5
{
    public class Program
    {
        public static void Main(string[] args)
        {
            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureAppConfiguration(builder =>
                {
                    var root = builder.Build();
                    var vaultName = root["KeyVault:Vault"];
                    builder.AddAzureKeyVault($"https://{vaultName}.vault.azure.net/", 
                        root["KeyVault:ClientId"],
                        GetCertificate(root["KeyVault:Thumbprint"]), new PrefixKeyVaultExample("Module4Lab5"));
                })
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                });

        private static X509Certificate2 GetCertificate(string thumbprint)
        {
            var certStore = new X509Store(StoreName.My, StoreLocation.CurrentUser);
            try
            {
                certStore.Open(OpenFlags.ReadOnly);
                var certCollection = certStore.Certificates.Find(
                    X509FindType.FindByThumbprint, thumbprint, false);
                
                if (certCollection.Count == 0)
                {
                    throw new Exception("Certificate is not installed");
                }
                return certCollection[0];
            }
            finally
            {
                certStore.Close();
            }
        }
    }
}
