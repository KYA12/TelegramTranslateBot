using System;
using System.Collections.Generic;
using Telegram.Bot;
using TelegramBotConsole.Models;
using Telegram.Bot.Args;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;
using System.Text;
using Microsoft.Extensions.Configuration;
using System.IO;
using Serilog;
using TelegramBotConsole.Services;

namespace TelegramBotConsole
{
    public class Program
    {
        public static TelegramBotClient bot; // Телеграм бот
        private static string curCommand = ""; //  Текущая команда
        private static readonly DictionaryContext db = new DictionaryContext();
        /* Вывод списка доступных пользователю команд */
        private static string fromLanguage;
        private static string toLanguage;
        private const string commands = @"
Usage:
/dictionary - translate your word/sentence from Russian to English
/show - show existing commands
/break - stop a last command";
        public static void Main(string[] args)
        {
            Log.Logger = new LoggerConfiguration()
               .MinimumLevel.Debug()
               .WriteTo.Console()
               .WriteTo.File("logs\\loggerConsole.txt", rollingInterval: RollingInterval.Day)
               .CreateLogger();
            var builder = new ConfigurationBuilder()
               .SetBasePath(Directory.GetCurrentDirectory())
               .AddJsonFile("config.json");
            var configuration = builder.Build();
            string key = configuration["Key"];
            toLanguage = configuration["ToLanguage"];
            fromLanguage = configuration["FromLanguage"];
            bot = new TelegramBotClient(key);// Создание телеграм бота с токеном
            var me = bot.GetMeAsync().Result;
            Console.WriteLine($"Hello, World! I am user {me.Id} and my name is {me.FirstName}.");
            Console.WriteLine("Press any key to exit");
            bot.OnMessage += BotCommandsSwitchingAndSending;
            bot.OnCallbackQuery += BotOnCallbackQueryDictionary;
            bot.StartReceiving();
            Console.ReadLine();
            bot.StopReceiving();
            db.Dispose();
        }
        private static async void BotCommandsSwitchingAndSending(object sender, MessageEventArgs e)
        {
            switch (e.Message.Text.Split()[0])
            {
                case "/dictionary":
                    try
                    {
                        await bot.SendChatActionAsync(e.Message.Chat.Id, ChatAction.Typing);
                    }
                    catch (Exception ex)
                    {
                        Log.Error($"Error is: {ex.Message}");
                    }
                    var keyboard = new InlineKeyboardMarkup(new[] { InlineKeyboardButton.WithCallbackData("Translate word/sentence")});
                    try
                    {
                        await bot.SendTextMessageAsync(e.Message.Chat.Id, "Choose", replyMarkup: keyboard);
                    }
                    catch (Exception ex)
                    {
                        Log.Error($"Error is: {ex.Message}");
                    }
                    break;
                case "/show":
                    try
                    {
                        await bot.SendTextMessageAsync(e.Message.Chat.Id, commands);
                    }
                    catch (Exception ex)
                    {
                        Log.Error($"Error is: {ex.Message}");
                    }
                    break;
                case "/break":
                    if (curCommand == "Translate word/sentence")
                        curCommand = "";
                    return;
            }
            if (curCommand == "Translate word/sentence")
            {
                if (e.Message.Text == null)
                {
                    await bot.SendTextMessageAsync(e.Message.Chat.Id, $"Your word/sentence is empty!");
                }
                else
                {
                    Dictionary<string, string> dictTranslated = new Dictionary<string, string>();// Словарь перевода слов с русского на английский. Dictionary(RusWord, EngWord)
                    string[] wordArrayForTranslate = TextUtility.ConvertForTranslate(e.Message.Text);
                    string[] wordArrayOriginal = TextUtility.ConvertOriginal(e.Message.Text);

                    foreach (var word in wordArrayForTranslate)
                        if (word != "")
                        {
                            /*  Переводим слово если его нет в базе данных 
                             *  и добавляем в базу данных, или берем перевод из базы данных */
                            string st = await DatabaseService.GetValueFromDBAsync(word, Log.Logger, db, fromLanguage,
                                toLanguage);
                            string testString;
                            if (!dictTranslated.TryGetValue(word, out testString))// Проверяем есть ли слово с переводом в словаре 
                            {
                                dictTranslated.Add(word, st);//Если слова в словаре нет, добавляем его в словарь
                            }
                        }

                    for (int j = 0; j < wordArrayOriginal.Length; j++)
                    {
                        string value;
                        if (dictTranslated.TryGetValue(wordArrayOriginal[j], out value))// Проверяем если перевод слова для оригинального текста в словаре
                        {
                            wordArrayOriginal[j] = value;// Если перевод слова есть, заменяем русское слово на английский перевод в оригинальном тексте
                        }
                    }

                    /* Удаление пробела перед каждым знаком пунктуации и добавление слова в строку*/
                    StringBuilder result = TextUtility.RemoveSpacesBeforePunctuationAndAddWord(wordArrayOriginal);

                    await bot.SendTextMessageAsync(e.Message.Chat.Id, $"Your word/sentence translation is: " +
                        $"{result.ToString()}");
                }
            }
        }
        private static async void BotOnCallbackQueryDictionary(object sender, CallbackQueryEventArgs e)
        {
            switch (e.CallbackQuery.Data)
            {
                case "Translate word/sentence":
                    curCommand = "Translate word/sentence";
                    try
                    {
                        await bot.SendTextMessageAsync(e.CallbackQuery.Message.Chat.Id, "Input word/sentence");
                    }
                    catch (Exception ex)
                    {
                        Log.Error($"Error is: {ex.Message}");
                    }
                    break;
            }
        }
    }
}
      
      

