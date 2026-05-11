using ClassLibrary1;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SpiroWeb.Managers
{
    public static class LyricsManager
    {
        static public Lyrics Get(string artist, string track)
        {
            using (SpiroStockManagementEntities db = new SpiroStockManagementEntities())
            {
                if (artist != string.Empty && track != string.Empty)
                {
                    return db.Lyrics.Where(c => c.Artist.Equals(artist.ToLower()) && c.Track.ToLower().Equals(track.ToLower())).FirstOrDefault();
                }
                return null;
            }
        }

        static public Lyrics GetById(int id)
        {
            using (SpiroStockManagementEntities db = new SpiroStockManagementEntities())
            {
                return db.Lyrics.Where(c => c.Id == id).FirstOrDefault();
            }
        }

        static public List<Lyrics> GetAll()
        {
            using (SpiroStockManagementEntities db = new SpiroStockManagementEntities())
            {
                return db.Lyrics.OrderBy(c => c.Artist).ToList();
            }
        }

        static public Lyrics Save(string artist, string track, string lyrics)
        {
            using (SpiroStockManagementEntities db = new SpiroStockManagementEntities())
            {
                try
                {
                    if (artist != string.Empty && track != string.Empty && lyrics != string.Empty)
                    {
                        var _lyrics = db.Lyrics.Where(c => c.Artist.Equals(artist.ToLower()) && c.Track.ToLower().Equals(track.ToLower())).FirstOrDefault();
                        if (_lyrics != null)
                        {
                            _lyrics.LyricsText = lyrics;
                            db.SaveChanges();
                            return _lyrics;
                        }
                        else
                        {
                            Lyrics _newLyrics = new Lyrics();
                            _newLyrics.Artist = artist;
                            _newLyrics.Track = track;
                            _newLyrics.LyricsText = lyrics;
                            _newLyrics.DateCreated = DateTime.Now;
                            db.Lyrics.Add(_newLyrics);
                            db.SaveChanges();
                            return _newLyrics;
                        }
                    }
                    return null;
                }
                catch (Exception ex)
                {
                    string error = ex.Message;
                    return null;
                }
            }
        }
    }
}