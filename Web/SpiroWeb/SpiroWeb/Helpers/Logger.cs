using System;

namespace SpiroWeb.Helpers
{
    public static class Logger
    {
        static public string Filename = "logs.txt";
        static public string FolderPath = string.Empty;
        static public void Debug(string data)
        {
            string[] str = new string[] { DateTime.Now.ToString() + " DEBUG " + data };
            if (System.IO.Directory.Exists(FolderPath))
                System.IO.File.WriteAllLines(FolderPath + "\\" + Filename, str);
        }

        static public void Debug(string data, string filename)
        {
            string[] str = new string[] { DateTime.Now.ToString() + " DEBUG " + data };
            if (System.IO.Directory.Exists(FolderPath))
                System.IO.File.AppendAllLines(FolderPath + "\\" + filename, str);
        }
    }
}