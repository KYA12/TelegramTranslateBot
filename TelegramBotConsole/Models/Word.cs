using System.ComponentModel.DataAnnotations;

namespace TelegramBotConsole.Models
{
    public class Word
    {
        [Key]
        public string OriginalWord { get; set; }// Оригинальное слово
        public string TranslatedWord { get; set; }// Перевод слова
        public Word(string originalWord, string translatedWord)
        {
            OriginalWord = originalWord;
            TranslatedWord = translatedWord;
        }
    }
}
