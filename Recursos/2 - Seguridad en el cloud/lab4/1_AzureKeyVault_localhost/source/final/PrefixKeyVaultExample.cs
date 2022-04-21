using Microsoft.Azure.KeyVault.Models;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Configuration.AzureKeyVault;

namespace Module4Lab5
{
    public class PrefixKeyVaultExample :IKeyVaultSecretManager
    {

        private readonly string _prefix;
        public PrefixKeyVaultExample(string prefix)
        {
            this._prefix = $"{prefix}-";
        }

        public bool Load(SecretItem secret)
        {
            return secret.Identifier.Name.StartsWith(this._prefix);
        }

        public string GetKey(SecretBundle secret)
        {
            return secret.SecretIdentifier.Name.Substring(this._prefix.Length)
                .Replace("--", ConfigurationPath.KeyDelimiter);
        }
    }
}