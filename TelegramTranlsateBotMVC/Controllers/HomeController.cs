using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using TelegramTranslateBotMVC.Models;

namespace TelegramTranslateBotMVC.Controllers
{
    public class HomeController : Controller
    {
        readonly ILogger<HomeController> logger;
        private DatabaseContext context;
        public HomeController(DatabaseContext con, ILogger<HomeController> _logger)
        {
            context = con;
            logger = _logger;
            if (!con.Words.Any())
            {
                con.Words.Add(new Word { OriginalWord = "птица", TranslatedWord = "bird" });
                con.SaveChanges();
            }
        }

        // Get /home
        [HttpGet]
        public async Task<IActionResult> Index()
        {
            try
            {
                var words = await context.Words.ToListAsync();
                return View(words);
            }
            catch (Exception ex)
            {
                logger.LogError($"Error in method Index /home: {ex.Message}");
                return NotFound();
            }

        }

        [HttpGet]
        public IActionResult AddWord()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> AddWord(IFormCollection form)
        {
            try
            {
                Word word = new Word { OriginalWord = form["OriginalWord"], TranslatedWord = form["TranslatedWord"] };
                await context.Words.AddAsync(word);
                await context.SaveChangesAsync();
                return Redirect("Index");
            }
            catch (Exception ex)
            {
                logger.LogError($"Error in method AddWord /home: {ex.Message}");
                return View();
            }

        }
        [HttpGet]
        public async Task<IActionResult> EditWord(string originalWord)
        {
            try
            {
                Word word = await context.Words.FindAsync(originalWord);
                return View(word);
            }
            catch (Exception ex)
            {
                logger.LogError($"Error in method EditWord /home/{originalWord}: {ex.Message}");
                return View();
            }

        }
        [HttpPost]
        public async Task<IActionResult> EditWord(IFormCollection form)
        {
            try
            {
                Word word = await context.Words.FindAsync(form["OriginalWord"]);
                word.TranslatedWord = form["TranslatedWord"];
                context.Words.Update(word);
                await context.SaveChangesAsync();
                return Redirect("Index");
            }
            catch (Exception ex)
            {
                logger.LogError($"Error in method EditWord /home/5: {ex.Message}");
                return View();
            }

        }

        [HttpGet]
        public async Task<IActionResult> DeleteWord(string originalWord)
        {
            try
            {
                Word word = await context.Words.FindAsync(originalWord);
                return View(word);
            }
            catch (Exception ex)
            {
                logger.LogError($"Error in method DeleteWord /home/{originalWord}: {ex.Message}");
                return View();
            }
        }

        [HttpPost]
        [ActionName("DeleteWord")]
        public async Task <IActionResult> Delete(string originalWord)
        {
            try
            {
                Word word = await context.Words.FindAsync(originalWord);
                context.Words.Remove(word);
                await context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                logger.LogError($"Error in method Delete /home/{originalWord}: {ex.Message}");
                return NotFound();
            }
        }
    }
}

