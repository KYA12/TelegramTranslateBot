using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace TelegramTranslateBotMVC.Models
{
    public class Word
    {
        [Key]
        [Required]
        public string OriginalWord { get; set; }// Оригинальное слово
        [Required]
        public string TranslatedWord { get; set; }// Перевод слова
    }
}
