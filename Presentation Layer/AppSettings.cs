using System.IO;
using System.Text.Json;

namespace Presentation_Layer
{
    public class AppSettings
    {
        public string Theme { get; set; } = "Light";

        private static string FilePath => "AppData/settings.json";

        public static AppSettings Load()
        {
            if (!File.Exists(FilePath))
            {
                var defaultSettings = new AppSettings();
                File.WriteAllText(FilePath, JsonSerializer.Serialize(defaultSettings));
                return defaultSettings;
            }

            var json = File.ReadAllText(FilePath);
            return JsonSerializer.Deserialize<AppSettings>(json);
        }

        public void Save()
        {
            var json = JsonSerializer.Serialize(this);
            File.WriteAllText(FilePath, json);
        }
    }
}
