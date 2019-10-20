using GoogleTranslateFreeApi;
using System;
using System.Threading.Tasks;

namespace TelegramBotConsole.Services
{
    public class Translate
    {
        /* Метод переводит слова/фразы с одного языка на другой и возвращает перевод */
        public static async Task<string> GoogleTranslate(string s, Serilog.ILogger logger, string fromLanguage, string toLanguage)
        {
            if (s.Length > 0)
            {
                var translator = new GoogleTranslator();
                Language from = GoogleTranslator.GetLanguageByName(fromLanguage);
                Language to = GoogleTranslator.GetLanguageByName(toLanguage);
                try
                {
                    TranslationResult result = await translator.TranslateLiteAsync(s, from, to);
                    return result.MergedTranslation;
                }
                catch (Exception ex)
                {
                    logger.Error($"Error is: {ex.Message}");
                }
                return null;
            }
            string str = "";
            return str;
        }
    }
}

