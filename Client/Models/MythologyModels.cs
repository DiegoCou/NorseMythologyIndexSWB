using System.Text.Json.Serialization;


using System.Text.Json.Serialization;

namespace NorseMythologyIndex.Models
{
    public class MythologyIndex
    {
        [JsonExtensionData]
        public Dictionary<string, System.Text.Json.JsonElement>? ExtensionData { get; set; }

        // Método helper para obtener los personajes parseados
        public Dictionary<string, MythologyItem> GetItems()
        {
            var Items = new Dictionary<string, MythologyItem>();
            
            if (ExtensionData == null) return Items;

            foreach (var kvp in ExtensionData)
            {
                try
                {
                    var Item = System.Text.Json.JsonSerializer.Deserialize<MythologyItem>(
                        kvp.Value.GetRawText(),
                        new System.Text.Json.JsonSerializerOptions 
                        { 
                            PropertyNameCaseInsensitive = true 
                        });
                    
                    if (Item != null)
                    {
                        Items[kvp.Key] = Item;
                    }
                }
                catch
                {
                    // Si falla la deserialización, continua con el siguiente
                    continue;
                }
            }
            
            return Items;
        }
    }

    public class MythologyItem
    {
        [JsonPropertyName("tags")]
        public List<string> Tags { get; set; } = new();

        [JsonPropertyName("attestations")]
        public Dictionary<string, Source> Attestations { get; set; } = new();
    }

    public class Source
    {
        [JsonPropertyName("link")]
        public string? Link { get; set; }

        [JsonExtensionData]
        public Dictionary<string, System.Text.Json.JsonElement>? ExtensionData { get; set; }

        // Método helper para obtener las referencias parseadas
        public Dictionary<string, Reference> GetReferences()
        {
            var references = new Dictionary<string, Reference>();
            
            if (ExtensionData == null) return references;

            foreach (var kvp in ExtensionData)
            {
                try
                {
                    var reference = System.Text.Json.JsonSerializer.Deserialize<Reference>(
                        kvp.Value.GetRawText(),
                        new System.Text.Json.JsonSerializerOptions 
                        { 
                            PropertyNameCaseInsensitive = true 
                        });
                    
                    if (reference != null)
                    {
                        references[kvp.Key] = reference;
                    }
                }
                catch
                {
                    continue;
                }
            }
            
            return references;
        }
    }

    public class Reference
    {
        [JsonPropertyName("link")]
        public string? Link { get; set; }

        [JsonPropertyName("text")]
        public string? Text { get; set; }
        
        [JsonExtensionData]
        public Dictionary<string, System.Text.Json.JsonElement>? SubReferences { get; set; }
        
        // Método helper para obtener las referencias parseadas
        public Dictionary<string, Content> GetContents()
        {
            var Contents = new Dictionary<string, Content>();
            
            if (SubReferences == null) return Contents;

            foreach (var kvp in SubReferences)
            {
                try
                {
                    var Content = System.Text.Json.JsonSerializer.Deserialize<Content>(
                        kvp.Value.GetRawText(),
                        new System.Text.Json.JsonSerializerOptions 
                        { 
                            PropertyNameCaseInsensitive = true 
                        });
                    
                    if (Content != null)
                    {
                        Contents[kvp.Key] = Content;
                    }
                }
                catch
                {
                    continue;
                }
            }
            
            return Contents;
        }
        
    }

    public class Content
    {
        [JsonPropertyName("link")]
        public string? Link { get; set; }

        [JsonPropertyName("quote")]
        public string? Text { get; set; }
        
        [JsonPropertyName("explanation")]
        public string? Explanation { get; set; }
    }
}