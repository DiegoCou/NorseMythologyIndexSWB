
using System.Globalization;
using System.Net.Http.Json;
using System.Text.Json;
using NorseMythologyIndex.Models;


using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using NorseMythologyIndex.Models;

namespace NorseMythologyIndex.Services
{
    public class MythologyService
    {
        private readonly HttpClient _httpClient;
        private Dictionary<string, MythologyItem>? _cachedItems;

        public MythologyService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<Dictionary<string, MythologyItem>> GetAllItemsAsync()
        {
            if (_cachedItems != null)
                return _cachedItems;

            try
            {
                var options = new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                };

                var mythologyIndex = await _httpClient.GetFromJsonAsync<MythologyIndex>(
                    "/Data/NorseMythologyIndex.json", options);

                if (mythologyIndex != null)
                {
                    _cachedItems = mythologyIndex.GetItems();
                }
                else
                {
                    _cachedItems = new Dictionary<string, MythologyItem>();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error loading mythology data: {ex.Message}");
                _cachedItems = new Dictionary<string, MythologyItem>();
            }

            return _cachedItems;
        }

        // Obtener personajes por tipo (Aesir, Jötunn, etc.)
        public async Task<Dictionary<string, MythologyItem>> GetItemsByTypeAsync(string type)
        {
            var allItems = await GetAllItemsAsync();
            return allItems
                .Where(c => c.Value.Tags.Any(t => 
                    t.Equals(type, StringComparison.OrdinalIgnoreCase)))
                .ToDictionary(c => c.Key, c => c.Value);
        }

        // Buscar personajes por nombre
        public async Task<Dictionary<string, MythologyItem>> SearchItemsAsync(string searchTerm)
        {
            var allItems = await GetAllItemsAsync();
            return allItems
                .Where(c => c.Key.Contains(searchTerm, StringComparison.OrdinalIgnoreCase))
                .ToDictionary(c => c.Key, c => c.Value);
        }

        // Obtener un personaje específico
        public async Task<(string Name, MythologyItem? Item)> GetItemAsync(string name)
        {
            var allItems = await GetAllItemsAsync();
            var Item = allItems.FirstOrDefault(c => 
                c.Key.Equals(name, StringComparison.OrdinalIgnoreCase));
            
            return (Item.Key, Item.Value);
        }

        // Obtener todos los tipos/tags únicos
        public async Task<List<string>> GetAllTagsAsync()
        {
            var allItems = await GetAllItemsAsync();
            return allItems
                .SelectMany(c => c.Value.Tags)
                .Distinct()
                .OrderBy(t => t)
                .ToList();
        }

        // Obtener personajes por género
        public async Task<Dictionary<string, MythologyItem>> GetItemsByGenderAsync(string gender)
        {
            var allItems = await GetAllItemsAsync();
            return allItems
                .Where(c => c.Value.Tags.Contains(gender, StringComparer.OrdinalIgnoreCase))
                .ToDictionary(c => c.Key, c => c.Value);
        }
        // Obtener lista de Tags
        public IEnumerable<String> GetTagList(Dictionary<string, MythologyItem> items, string type)
        {
            List<String> tagList = [];

            if (items == null)
            {
                return tagList;
            }
            
            foreach(var item in items)
            {
                foreach (var tag in item.Value.Tags)
                {
                    if(tag.ToLower() != type.ToLower())
                        tagList.Add(tag);
                }
            }
            return tagList.Distinct().AsEnumerable();
        }
        // Contar personajes por tipo
        public async Task<Dictionary<string, int>> GetItemCountByTypeAsync()
        {
            var allItems = await GetAllItemsAsync();
            var counts = new Dictionary<string, int>();
            
            foreach (var Item in allItems)
            {
                foreach (var tag in Item.Value.Tags)
                {
                    if (counts.ContainsKey(tag))
                        counts[tag]++;
                    else
                        counts[tag] = 1;
                }
            }
            
            return counts.OrderByDescending(x => x.Value)
                .ToDictionary(x => x.Key, x => x.Value);
        }
        
        public static string RemoveSpecialCharacters(string str)
        {
            string normalized = str.Normalize(NormalizationForm.FormD);
            var builder = new StringBuilder();

            foreach (char ch in normalized)
            {
                if (CharUnicodeInfo.GetUnicodeCategory(ch) != UnicodeCategory.NonSpacingMark)
                {
                    builder.Append(ch);
                }
            }
            return builder.ToString().Normalize(NormalizationForm.FormC);
        }
        public static string GetReferenceWithoutTranslation(string str)
        {
            if (!str.Contains("("))
                return str;
            str = str.Remove(str.IndexOf("(", StringComparison.Ordinal));
            return str;
        }
        
        public IEnumerable<IGrouping<char, KeyValuePair<string, MythologyItem>>> GetGroupedItems(Dictionary<string, MythologyItem> items)
        {
            return items
                .Where(kvp => !string.IsNullOrEmpty(kvp.Key))
                .GroupBy(kvp => char.ToUpper(kvp.Key[0]))
                .OrderBy(g => g.Key);
        }
    }
    
}
