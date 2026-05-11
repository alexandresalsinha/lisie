using ClassLibrary1;
using SpiroWeb.Helpers;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;

namespace SpiroWeb.Controllers.api
{
    public class LyricsApiController : ApiController
    {
        public HttpResponseMessage Get(string artist, string track)
        {
            //Managers.InteractionsManager.Add(userId, "api/Lyrics/Get", userId);

            if (artist != string.Empty && track != string.Empty)
            {
                Lyrics _lyrics = Managers.LyricsManager.Get(artist, track);
                if (_lyrics != null)
                    return Request.CreateResponse(HttpStatusCode.OK, _lyrics);
                return Request.CreateResponse(HttpStatusCode.NotFound);

            }
            else
                return Request.CreateResponse(HttpStatusCode.BadRequest);
        }

        public HttpResponseMessage GetAll()
        {
            List<Lyrics> _lyrics = Managers.LyricsManager.GetAll();
            if (_lyrics != null)
                return Request.CreateResponse(HttpStatusCode.OK, _lyrics);
            return Request.CreateResponse(HttpStatusCode.NotFound);
        }

        // POST: api/Notes/Post
        [HttpGet]
        [HttpPost]
        public HttpResponseMessage Post([FromBody] Models.LyricsPostModel model)
        {
            if (model != null)
            {
                Lyrics _lyrics = Managers.LyricsManager.Save(model.Artist, model.Track, model.LyricsText);
                if (_lyrics != null)
                    return Request.CreateResponse(HttpStatusCode.OK, _lyrics);
                return Request.CreateResponse(HttpStatusCode.NotFound);

            }
            else
                return Request.CreateResponse(HttpStatusCode.BadRequest);
        }

        [HttpGet]
        [HttpPost]
        public HttpResponseMessage UpdateCurrentPlaying(string artist, string track)
        {
            if (!string.IsNullOrEmpty(artist) && !string.IsNullOrEmpty(track))
            {
                GlobalVariables.CurrentArtist = artist;
                GlobalVariables.CurrentTrack = track;
                Microsoft.AspNet.SignalR.StockTicker.StockTicker.Instance.BroadcastNewPlayingSong("9ff8224f-17cf-49fb-b555-05779a13eb40");
                return Request.CreateResponse(HttpStatusCode.OK, "Updated");
            }
            else
                return Request.CreateResponse(HttpStatusCode.BadRequest);
        }

        [HttpGet]
        [HttpPost]
        public HttpResponseMessage UpdateCurrentPlayingFromDesktop(string artist, string track) //In this method, the song may be repeate  (loop 10s)
        {
            if (!string.IsNullOrEmpty(artist) && !string.IsNullOrEmpty(track) &&
                artist != GlobalVariables.CurrentArtist &&
                track != GlobalVariables.CurrentTrack)
            {
                GlobalVariables.CurrentArtist = artist;
                GlobalVariables.CurrentTrack = track;
                Microsoft.AspNet.SignalR.StockTicker.StockTicker.Instance.BroadcastNewPlayingSong("9ff8224f-17cf-49fb-b555-05779a13eb40");
                return Request.CreateResponse(HttpStatusCode.OK, "Updated");
            }
            else
                return Request.CreateResponse(HttpStatusCode.BadRequest);
        }

        public async Task<HttpResponseMessage> GetPlayingLyrics()
        {
            if (string.IsNullOrEmpty(GlobalVariables.CurrentArtist) || string.IsNullOrEmpty(GlobalVariables.CurrentTrack))
            {
                return Request.CreateResponse(HttpStatusCode.BadRequest, "Current artist and track not set");
            }

            var client = new HttpClient();
            string _url = "https://puppeteer-lyrics.herokuapp.com/getlyrics?artist=" + HttpUtility.UrlEncode(GlobalVariables.CurrentArtist) + "&track=" + HttpUtility.UrlEncode(GlobalVariables.CurrentTrack);

            try
            {
                string responseLyrics = await client.GetStringAsync(_url);
                string _rspFinal = "<style>* {font-size: 30pt;font-family: Arial;}body{background-color: black;color: white;}.column{float: left;width: 33%;}.row:after {content: '';display: table;clear: both;}</style>";

                _rspFinal += "<div class='row'><div class='column'>";
                string[] _lines = responseLyrics.Split(new string[] { "<br>" }, StringSplitOptions.None);

                int _index = 0;
                foreach (var _line in _lines)
                {
                    if (_index == 0 && string.IsNullOrEmpty(_line))
                        continue;

                    _rspFinal += _line + "<br>";
                    if (_index == 25)
                    {
                        _rspFinal += "</div><div class='column'>";
                    }
                    if (_index == 50)
                    {
                        _rspFinal += "</div><div class='column'>";
                    }
                    _index++;
                }
                _rspFinal += "</div></div>";
                //responseLyrics = responseLyrics.Insert(0, "<style>* {font-size: 30pt;font-family: Arial;}.column{float: left;width: 50%;}.row:after {content: '';display: table;clear: both;}</style>");
                var response = new HttpResponseMessage();
                response.Content = new StringContent(_rspFinal);
                response.Content.Headers.ContentType = new MediaTypeHeaderValue("text/html");
                response.StatusCode = HttpStatusCode.OK;
                return response;
            }
            catch (Exception ex)
            {
                return Request.CreateResponse(HttpStatusCode.InternalServerError, ex.Message);
            }

        }

        public async Task<HttpResponseMessage> GetPlayingLyricsV2()
        {
            if (string.IsNullOrEmpty(GlobalVariables.CurrentArtist) || string.IsNullOrEmpty(GlobalVariables.CurrentTrack))
            {
                return Request.CreateResponse(HttpStatusCode.BadRequest, "Current artist and track not set");
            }

            var client = new HttpClient();
            string _url = "https://puppeteer-lyrics.herokuapp.com/getlyrics?artist=" + HttpUtility.UrlEncode(GlobalVariables.CurrentArtist) + "&track=" + HttpUtility.UrlEncode(GlobalVariables.CurrentTrack);

            try
            {
                string responseLyrics = await client.GetStringAsync(_url);
                string _rspFinal = "<style>* {font-size: 30pt;font-family: Arial;}body{background-color: black;color: white;}.column{float: left;width: 33%;}.row{height: 100%;column-count: 4;}</style>";

                _rspFinal += "<div class='row'>";
                string[] _lines = responseLyrics.Split(new string[] { "<br>" }, StringSplitOptions.None);

                int _index = 0;
                foreach (var _line in _lines)
                {
                    if (_index == 0 && string.IsNullOrEmpty(_line))
                        continue;

                    _rspFinal += _line + "<br>";
                    //if (_index == 25)
                    //{
                    //    _rspFinal += "</div><div class='column'>";
                    //}
                    //if (_index == 50)
                    //{
                    //    _rspFinal += "</div><div class='column'>";
                    //}
                    _index++;
                }
                //_rspFinal += "</div></div>";
                _rspFinal += "</div>";
                //responseLyrics = responseLyrics.Insert(0, "<style>* {font-size: 30pt;font-family: Arial;}.column{float: left;width: 50%;}.row:after {content: '';display: table;clear: both;}</style>");
                var response = new HttpResponseMessage();
                response.Content = new StringContent(_rspFinal);
                response.Content.Headers.ContentType = new MediaTypeHeaderValue("text/html");
                response.StatusCode = HttpStatusCode.OK;
                return response;
            }
            catch (Exception ex)
            {
                return Request.CreateResponse(HttpStatusCode.InternalServerError, ex.Message);
            }

        }
    }
}
