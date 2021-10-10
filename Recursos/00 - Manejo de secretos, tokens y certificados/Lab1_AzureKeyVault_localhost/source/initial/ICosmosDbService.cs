using System.Collections.Generic;
using System.Threading.Tasks;
using Module4Lab5.Models;

namespace Module4Lab5
{
    /// <summary>
    /// Database Service Interface for CosmosDB.
    /// </summary>
    public interface ICosmosDbService
    {
        Task<IEnumerable<Item>> GetItemsAsync(string query);
        Task<Item> GetItemAsync(string id);
        Task AddItemAsync(Item item);
        Task UpdateItemAsync(string id, Item item);
        Task DeleteItemAsync(string id);
    }
}