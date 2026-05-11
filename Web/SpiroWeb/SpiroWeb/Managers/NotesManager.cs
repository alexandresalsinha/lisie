using ClassLibrary1;
using System.Linq;

namespace SpiroWeb.Managers
{
    public static class NotesManager
    {
        static private SpiroStockManagementEntities db = new SpiroStockManagementEntities();
        static public UserNotes Get(string userId)
        {
            if (userId != string.Empty)
            {
                return db.UserNotes.Where(c => c.UserId.Equals(userId)).FirstOrDefault();
            }
            return null;
        }

        static public UserNotes Save(string userId, string notes)
        {
            if (userId != string.Empty)
            {
                var _notes = db.UserNotes.Where(c => c.UserId.Equals(userId)).FirstOrDefault();
                if (_notes != null)
                {
                    _notes.Notes = notes;
                    db.SaveChanges();
                    return _notes;
                }
                else
                {
                    UserNotes _UserNotes = new UserNotes();
                    _UserNotes.UserId = userId;
                    _UserNotes.Notes = notes;
                    db.UserNotes.Add(_UserNotes);
                    db.SaveChanges();
                    return _UserNotes;
                }
                return null;
            }
            return null;
        }
    }
}