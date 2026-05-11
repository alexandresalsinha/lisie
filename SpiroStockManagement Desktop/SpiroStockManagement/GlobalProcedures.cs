using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.Windows.Forms;
using System.Xml.Linq;
using System.Xml.Serialization;
using System.Xml;
using System.Text.RegularExpressions;
using System.Collections;
using System.Security.Cryptography;
using System.Security;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Security;
using System.Security.Cryptography;
using System.Text;


namespace SpiroStockManagement
{

    public class ListViewColumnSorter : IComparer
    {
        /// <summary>
        /// Specifies the column to be sorted
        /// </summary>
        private int ColumnToSort;
        /// <summary>
        /// Specifies the order in which to sort (i.e. 'Ascending').
        /// </summary>
        private SortOrder OrderOfSort;
        /// <summary>
        /// Case insensitive comparer object
        /// </summary>
        private CaseInsensitiveComparer ObjectCompare;

        /// <summary>
        /// Class constructor.  Initializes various elements
        /// </summary>
        public ListViewColumnSorter()
        {
            // Initialize the column to '0'
            ColumnToSort = 0;

            // Initialize the sort order to 'none'
            OrderOfSort = SortOrder.None;

            // Initialize the CaseInsensitiveComparer object
            ObjectCompare = new CaseInsensitiveComparer();
        }

        /// <summary>
        /// This method is inherited from the IComparer interface.  It compares the two objects passed using a case insensitive comparison.
        /// </summary>
        /// <param name="x">First object to be compared</param>
        /// <param name="y">Second object to be compared</param>
        /// <returns>The result of the comparison. "0" if equal, negative if 'x' is less than 'y' and positive if 'x' is greater than 'y'</returns>
        public int Compare(object x, object y)
        {
            int compareResult;
            ListViewItem listviewX, listviewY;

            // Cast the objects to be compared to ListViewItem objects
            listviewX = (ListViewItem)x;
            listviewY = (ListViewItem)y;

            // Compare the two items
            compareResult = ObjectCompare.Compare(listviewX.SubItems[ColumnToSort].Text, listviewY.SubItems[ColumnToSort].Text);

            // Calculate correct return value based on object comparison
            if (OrderOfSort == SortOrder.Ascending)
            {
                // Ascending sort is selected, return normal result of compare operation
                return compareResult;
            }
            else if (OrderOfSort == SortOrder.Descending)
            {
                // Descending sort is selected, return negative result of compare operation
                return (-compareResult);
            }
            else
            {
                // Return '0' to indicate they are equal
                return 0;
            }
        }

        /// <summary>
        /// Gets or sets the number of the column to which to apply the sorting operation (Defaults to '0').
        /// </summary>
        public int SortColumn
        {
            set
            {
                ColumnToSort = value;
            }
            get
            {
                return ColumnToSort;
            }
        }

        /// <summary>
        /// Gets or sets the order of sorting to apply (for example, 'Ascending' or 'Descending').
        /// </summary>
        public SortOrder Order
        {
            set
            {
                OrderOfSort = value;
            }
            get
            {
                return OrderOfSort;
            }
        }

    }

    public static class GlobalProcedures
    {
        public static Image DownloadImage(string _URL)
        {
            Image _tmpImage = null;

            try
            {
                // Open a connection
                System.Net.HttpWebRequest _HttpWebRequest = (System.Net.HttpWebRequest)System.Net.HttpWebRequest.Create(_URL);

                _HttpWebRequest.AllowWriteStreamBuffering = true;

                // You can also specify additional header values like the user agent or the referer: (Optional)
                _HttpWebRequest.UserAgent = "Mozilla/4.0 (compatible; MSIE 7.0; Windows NT 5.1)";
                _HttpWebRequest.Referer = "http://www.google.com/";

                // set timeout for 20 seconds (Optional)
                _HttpWebRequest.Timeout = 20000;

                // Request response:
                System.Net.WebResponse _WebResponse = _HttpWebRequest.GetResponse();

                // Open data stream:
                System.IO.Stream _WebStream = _WebResponse.GetResponseStream();

                // convert webstream to image
                _tmpImage = Image.FromStream(_WebStream);

                // Cleanup
                _WebResponse.Close();
                _WebResponse.Close();
            }
            catch (Exception _Exception)
            {
                // Error
                Console.WriteLine("Exception caught in process: {0}", _Exception.ToString());
                return null;
            }

            return _tmpImage;
        }

        public static IEnumerable<string> GetDecimalFromString(string text)
        {
            var doubleArray = Regex.Split(text, @"[^0-9\.]+")
    .Where(c => c != "." && c.Trim() != "");

            var _doubleArray = Regex.Split(text, @"^[0-9]([\.\,][0-9]{1,3})?$");
            

            //text.SelectstringArray.Select(s => decimal.Parse(s));
            return (IEnumerable<string>)doubleArray;
        }
        //static class XmlSerializerExtension
        //{
        //    public static XElement SerializeAsXElement(this XmlSerializer xs, object o)
        //    {
        //        XDocument d = new XDocument();
        //        using (XmlWriter w = d.CreateWriter()) xs.Serialize(w, o);
        //        XElement e = d.Root;
        //        e.Remove();
        //        return e;
        //    }
        //}

        public static Image ThumbnailImage(string imageFilename, int width, int heigh)
        {
            Image image = Image.FromFile(imageFilename);

            int srcWidth = image.Width;
            int srcHeight = image.Height;
            int thumbHeight = (srcHeight / srcWidth) * width;
            Bitmap bmp = new Bitmap(width, thumbHeight);

            System.Drawing.Graphics gr = System.Drawing.Graphics.FromImage(bmp);
            gr.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;
            gr.CompositingQuality = System.Drawing.Drawing2D.CompositingQuality.HighQuality;
            gr.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.High;

            System.Drawing.Rectangle rectDestination = new System.Drawing.Rectangle(0, 0, width, thumbHeight);
            gr.DrawImage(image, rectDestination, 0, 0, srcWidth, srcHeight, GraphicsUnit.Pixel);

            return image;
        }
        public static Image ThumbnailImage2(string imageFilename, int width, int height)
        {
            Image tempImage = Image.FromFile(imageFilename);

            int dw = tempImage.Width;
            int dh = tempImage.Height;
            int tw = width;
            int th = height;
            double zw = (tw / (double)dw);
            double zh = (th / (double)dh);
            double z = (zw <= zh) ? zw : zh;
            dw = (int)(dw * z);
            dh = (int)(dh * z);

            Bitmap m_Image = new Bitmap(dw, dh);
            Graphics g = Graphics.FromImage(m_Image);
            g.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
            g.DrawImage(tempImage, 0, 0, dw, dh);

            return tempImage;// do not forget to dispose and don't wait for GC
        }

        public static Size GetThumbnailSize(Image original, int maxPixels)
        {
            // Maximum size of any dimension.
            //const int maxPixels = 40;

            // Width and height.
            int originalWidth = original.Width;
            int originalHeight = original.Height;

            // Compute best factor to scale entire image based on larger dimension.
            double factor;
            if (originalWidth > originalHeight)
            {
                factor = (double)maxPixels / originalWidth;
            }
            else
            {
                factor = (double)maxPixels / originalHeight;
            }

            // Return thumbnail size.
            return new Size((int)(originalWidth * factor), (int)(originalHeight * factor));
        }

        static public Image ImageFromFileNoLock(string fileName)
        {
            // Image.FromFile will hold a lock on the file until the image is destroyed,
            // so using a FileStream to load the file will release lock before returning image
            // because the stream gets closed...
            System.IO.FileStream fs = new System.IO.FileStream(fileName, System.IO.FileMode.Open, System.IO.FileAccess.Read);

            // Get image from stream...
            Image img = Image.FromStream(fs);

            // Close/release the file...
            fs.Close();

            // return the image...
            return img;
        }

        static public float GetVariableWeightByKgOfString(string text)
        {
            string variableWeightByKgString = text;
            variableWeightByKgString = variableWeightByKgString.Substring(0, variableWeightByKgString.IndexOf('/'));
            variableWeightByKgString = variableWeightByKgString.Trim();
            try
            {
                return float.Parse(variableWeightByKgString);
            }
            catch (Exception)
            {
                return -1;
            }
        }

        static public Point GetControlLocationInRelationToForm(Control control)
        {
            return control.FindForm().PointToClient(
    control.Parent.PointToScreen(control.Location));
        }

        static public Int32 FractionalPart(float n)
        {
            string s = n.ToString("#.#########", System.Globalization.CultureInfo.InvariantCulture);
            return Int32.Parse(s.Substring(s.IndexOf(".") + 1)).ToString().Length;
        }


        static public string EncodeTo64(string toEncode)
        {
            byte[] toEncodeAsBytes
                  = System.Text.ASCIIEncoding.ASCII.GetBytes(toEncode);
            string returnValue
                  = System.Convert.ToBase64String(toEncodeAsBytes);
            return returnValue;
        }

        static public string DecodeFrom64(string encodedData)
        {
            byte[] encodedDataAsBytes
                = System.Convert.FromBase64String(encodedData);
            string returnValue =
               System.Text.ASCIIEncoding.ASCII.GetString(encodedDataAsBytes);
            return returnValue;
        }

        static public string base64Encode(string data)
        {
            try
            {
                byte[] encData_byte = new byte[data.Length];
                encData_byte = System.Text.Encoding.UTF8.GetBytes(data);
                string encodedData = Convert.ToBase64String(encData_byte);
                return encodedData;
            }
            catch (Exception e)
            {
                throw new Exception("Error in base64Encode" + e.Message);
            }
        }

        static public string base64Decode(string data)
        {
            try
            {
                System.Text.UTF8Encoding encoder = new System.Text.UTF8Encoding();
                System.Text.Decoder utf8Decode = encoder.GetDecoder();

                byte[] todecode_byte = Convert.FromBase64String(data);
                int charCount = utf8Decode.GetCharCount(todecode_byte, 0, todecode_byte.Length);
                char[] decoded_char = new char[charCount];
                utf8Decode.GetChars(todecode_byte, 0, todecode_byte.Length, decoded_char, 0);
                string result = new String(decoded_char);
                return result;
            }
            catch (Exception e)
            {
                throw new Exception("Error in base64Decode" + e.Message);
            }
        }
    }
}
