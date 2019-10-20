using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TelegramBotConsole.Models;

namespace TelegramBotConsole.Services
{
    public class DatabaseService
    {
        public static async Task<string> GetValueFromDBAsync(string wordToCheck, Serilog.ILogger logger, 
            DictionaryContext context, string fromLanguage, string toLanguage)
        {
            try
            {
                return await Task.Run(() => GetValueFromDB(wordToCheck, context, fromLanguage, toLanguage, logger));
            }
            catch (Exception ex)
            {
                logger.Error($"Error is: {ex.Message}");
                return null;
            }
        }
        public static string GetValueFromDB(string word, DictionaryContext db, string fromLanguage, 
            string toLanguage, Serilog.ILogger logger)
        {
            /* Если слово для перевода начинается с большой буквы,
             * то переводится слово на английский и выводится перевод с большой буквы */
            List<Word> words = db.Words.ToList();
            if (char.IsUpper(word[0]))
            {
                foreach (var w in words)
                {
                    if (string.Equals(w.OriginalWord, word, StringComparison.OrdinalIgnoreCase))
                    {
                        return w.TranslatedWord.First().ToString().ToUpper() + w.TranslatedWord.Substring(1);
                    }
                }

                string lowWord = word.First().ToString().ToLower() + word.Substring(1);
                Word upperWord = new Word(lowWord, Translate.GoogleTranslate(lowWord, logger, 
                    fromLanguage, toLanguage).Result);
                string s = upperWord.TranslatedWord;
                db.Words.Add(upperWord);// Добавление английского слова в базу данных, если его там нет
                try
                {
                    db.SaveChanges();
                }
                catch (Exception ex)
                {
                    logger.Error($"Error is: {ex.Message}");
                }
                return upperWord.TranslatedWord.First().ToString().ToUpper() + upperWord.TranslatedWord.Substring(1);
            };

            /* Если слово для перевода начинается с маленькой буквы,
             * то переводится слово на английский и выводится перевод с маленькой буквы */
            foreach (var w in words)
            {
                if (string.Equals(w.OriginalWord, word))
                {
                    return w.TranslatedWord;
                }
            }

            Word newWord = new Word(word, Translate.GoogleTranslate(word, logger, fromLanguage, toLanguage).Result);
            db.Words.Add(newWord);
            try
            {
                db.SaveChanges();
            }
            catch (Exception ex)
            {
                logger.Error($"Error is: {ex.Message}");
            }
            return newWord.TranslatedWord;
        }

    }
}
