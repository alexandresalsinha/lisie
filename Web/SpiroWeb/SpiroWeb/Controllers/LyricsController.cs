using ClassLibrary1;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace SpiroWeb.Controllers
{
    public class LyricsController : Controller
    {
        [System.Web.Mvc.Authorize]
        public ActionResult Index()
        {
            List<Lyrics> _lyrics = Managers.LyricsManager.GetAll();
            if (_lyrics != null)
                return View(_lyrics);
            else
                return View();
        }

        public ActionResult Details(int id, bool darkMode = false)
        {
            ViewBag.DarkMode = darkMode;
            Lyrics _lyrics = Managers.LyricsManager.GetById(id);
            if (_lyrics != null)
                return View(_lyrics);
            else
                return View();
        }

        [System.Web.Mvc.Authorize]
        public async Task<ActionResult> CurrentPlaying(bool darkMode = false)
        {
            ViewBag.DarkMode = darkMode;
            ViewBag.Artist = Helpers.GlobalVariables.CurrentArtist;
            ViewBag.Track = Helpers.GlobalVariables.CurrentTrack;
            Lyrics _lyrics = Managers.LyricsManager.Get(Helpers.GlobalVariables.CurrentArtist, Helpers.GlobalVariables.CurrentTrack);

            if (_lyrics != null)
            {
                ViewBag.LyricsText = _lyrics.LyricsText;
            }
            else
            {
                var client = new HttpClient();
                string _url = "https://puppeteer-lyrics.herokuapp.com/getlyrics?artist=" + HttpUtility.UrlEncode(Helpers.GlobalVariables.CurrentArtist) + "&track=" + HttpUtility.UrlEncode(Helpers.GlobalVariables.CurrentTrack);

                try
                {
                    string responseLyrics = await client.GetStringAsync(_url);
                    ViewBag.LyricsText = responseLyrics;
                }
                catch (Exception ex)
                {
                    ViewBag.LyricsText = "";
                }
            }
            return View();
        }

        public async Task<ActionResult> Search(string artist, string track, bool darkMode = false)
        {
            ViewBag.DarkMode = darkMode;
            ViewBag.Artist = artist;
            ViewBag.Track = track;
            Lyrics _lyrics = Managers.LyricsManager.Get(artist, track);
            if (_lyrics != null)
                return View(_lyrics.LyricsText);
            else
            {
                var client = new HttpClient();
                string _url = "https://puppeteer-lyrics.herokuapp.com/getlyrics?artist=" + HttpUtility.UrlEncode(artist) + "&track=" + HttpUtility.UrlEncode(track);

                try
                {
                    string responseLyrics = await client.GetStringAsync(_url);
                    return View("CurrentPlaying", responseLyrics);
                }
                catch (Exception ex)
                {
                    return View("CurrentPlaying", "");
                }
            }
        }
    }
}
