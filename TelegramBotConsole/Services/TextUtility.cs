using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TelegramBotConsole.Services
{
    public class TextUtility
    {
        public static async Task<string[]> ConvertForTranslateAsync(string textForTranslate)
        {
            return await Task.Run(() => ConvertForTranslate(textForTranslate));
        }
        public static string[] ConvertForTranslate(string textForTranslate)
        {
            string[] wordArrayForTranslate = textForTranslate.Split(new[] { ' ', '.', ',', '?', '!', ':', ';', '-', '\"', '/' },
                             StringSplitOptions.RemoveEmptyEntries);// Создание массив слов из текста пользователя, по разделителям и удаляем лишние пробелы
            return wordArrayForTranslate;
        }

        public static async Task<string[]> ConvertOriginalAsync(string textOriginal) 
        {
            string st  = await AddSpacesBeforePunctuationAsync(textOriginal);
            string[] wordArrayOriginal = st.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);// Создание массива слов включаю разделители и удаляем пробелы
            return wordArrayOriginal;

        }

        public static async Task<string> AddSpacesBeforePunctuationAsync(string stringOriginal)
        {
            return await Task.Run(() => AddSpacesBeforePunctuation(stringOriginal));
        }
        public static string AddSpacesBeforePunctuation(string stringOriginal)
        {
            var builder = new StringBuilder();
            char? previousChar = null;

            foreach (var ch in stringOriginal)
            {
                // Добавление пробела перед знаком пунктуации
                if ((Char.IsPunctuation(ch) || ch =='-')&& previousChar != ' ' )
                {
                    builder.Append(' ');
                }
                if (previousChar =='-')
                {
                    builder.Append(' ');
                }
                builder.Append(ch);
                previousChar = ch;
            }
            // Строка с пробелами между словами и знаками пунктуации
            string stringChanged = builder.ToString();
            return stringChanged;
        }
        public static async Task<string> RemoveSpacesBeforePunctuationAsync(string[] wordArray)
        {
            return await Task.Run(() => RemoveSpacesBeforePunctuation(wordArray));
        }

        public static string RemoveSpacesBeforePunctuation(string[] wordArray)
        {
            StringBuilder result = new StringBuilder(); // Строка с результатом перевода слова/фразы
            for (int i = 0; i < wordArray.Length; i++)
            {
                if (Char.IsPunctuation(wordArray[i].First()))
                {
                    result.Length--;
                }
                result.Append(wordArray[i]);
                result.Append(" ");
            }
            return result.ToString();
        }
    }
}
