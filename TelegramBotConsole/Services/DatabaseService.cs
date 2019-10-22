using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TelegramBotConsole.Models;

namespace TelegramBotConsole.Services
{
    public class DatabaseService
    {
        public static async Task<string>  GetValueFromDBAsync(string wordToCheck, Serilog.ILogger logger,
            string fromLanguage, string toLanguage)
        {
            /* Если слово для перевода начинается с большой буквы,
             * то переводится слово на английский и выводится перевод с большой буквы */
            List<Word> words;
            using (DictionaryContext con = new DictionaryContext())
            {
                words = await con.Words.ToListAsync();
            }
            if (char.IsUpper(wordToCheck[0]))
            {
                var isUpper = words.AsParallel().Where(w => string.Equals(w.OriginalWord, wordToCheck, StringComparison.OrdinalIgnoreCase));
               
                if (isUpper != null)
                {
                    foreach (Word w in isUpper) return w.TranslatedWord.First().ToString().ToUpper() + w.TranslatedWord.Substring(1);
                }
                string lowWord = wordToCheck.First().ToString().ToLower() + wordToCheck.Substring(1);
                Word upperWord = new Word(lowWord, await Translate.GoogleTranslateAsync(lowWord, logger, 
                    fromLanguage, toLanguage));
                using (DictionaryContext con = new DictionaryContext())
                {
                    if (con.Words.FindAsync(upperWord.OriginalWord) == null)
                    {
                        try
                        {
                            await con.Words.AddAsync(upperWord);// Добавление английского слова в базу данных, если его там нет
                            await con.SaveChangesAsync();
                        }
 
                        catch (DbUpdateConcurrencyException ex)
                        {
                            logger.Error($"Error is: {ex.Message}");
                        }
                    
                    }
                }
                return upperWord.TranslatedWord.First().ToString().ToUpper() + upperWord.TranslatedWord.Substring(1);
            };

            /* Если слово для перевода начинается с маленькой буквы,
             * то переводится слово на английский и выводится перевод с маленькой буквы */

            var word = words.AsParallel().Where(w => string.Equals(w.OriginalWord, wordToCheck));

            if (word != null)
            {
                foreach (Word w in word) return w.TranslatedWord;
            }

            Word newWord = new Word(wordToCheck, await Translate.GoogleTranslateAsync(wordToCheck, logger, fromLanguage, toLanguage));
            using (DictionaryContext con = new DictionaryContext())
            {
                try
                {
                    await con.Words.AddAsync(newWord);
                    await con.SaveChangesAsync();
                }
                catch (Exception ex)
                {
                    logger.Error($"Error is: {ex.Message}");
                }
            }
            return newWord.TranslatedWord;
        }

    }
}
