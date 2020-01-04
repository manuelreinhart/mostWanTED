using Microsoft.Azure.KeyVault;
using Microsoft.Azure.KeyVault.Models;
using Microsoft.Azure.Services.AppAuthentication;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Common.Tools
{
    public class KeyVault
    {
        public static readonly KeyVault Singleton = new KeyVault();
                
        private KeyVaultClient KeyVaultClient;

        private KeyVault()
        {
            AzureServiceTokenProvider azureServiceTokenProvider = new AzureServiceTokenProvider();
            this.KeyVaultClient = new KeyVaultClient(new KeyVaultClient.AuthenticationCallback(azureServiceTokenProvider.KeyVaultTokenCallback));
        }

        public async Task<string> GetSecretByKey(string key)
        {
            var keyVaultUri = await ServiceDiscovery.Singleton.GetConfigValueByKey("KeyVaultUri");
            
            try
            {                
                //var secret = await this.KeyVaultClient.GetSecretAsync("https://iegkeyvaultmar.vault.azure.net/secrets/myPassword");
                var secret = await this.KeyVaultClient.GetSecretAsync(keyVaultUri, key);
                return secret.Value;
            }            
            catch (Exception)
            {
                return null;
            }

        }




    }
}
