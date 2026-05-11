using ClassLibrary1;
using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Web;

namespace SpiroWeb.Handlers
{
    /// <summary>
    /// Summary description for GetProductImage
    /// </summary>
    public class GetRecipeImage : IHttpHandler
    {
        private SpiroStockManagementEntities db = new SpiroStockManagementEntities();

        public void ProcessRequest(HttpContext context)
        {
            try
            {
                //if (context.Request.QueryString["productId"] != null && context.Request.QueryString["width"] != null && context.Request.QueryString["height"] != null)
                if (context.Request.QueryString["recipeId"] != null)
                {
                    string _recipeId = context.Request.QueryString["recipeId"];
                    //int _width = int.Parse(context.Request.QueryString["width"].ToString());
                    //int _height = int.Parse(context.Request.QueryString["height"].ToString());
                    string _imageUrl = HttpContext.Current.Server.MapPath("~/App_Data/ProductsPictures/recepy_" + _recipeId + ".jpg");

                    if (System.IO.File.Exists(_imageUrl))
                    {
                        byte[] _imageFromDisk = GetImageInBytes(_imageUrl);
                        context.Response.OutputStream.Write(_imageFromDisk, 0, _imageFromDisk.Length);
                        context.Response.ContentType = "image/JPEG";
                        return;
                    }
                    else
                    {
                        Recepies _recepies = db.Recepies.Find(int.Parse(_recipeId));

                        if (_recepies != null)
                        {
                            if (_recepies.Picture != null)
                            {
                                SaveImageToDisk(_recepies.Picture, _imageUrl);
                                context.Response.OutputStream.Write(_recepies.Picture, 0, _recepies.Picture.Length);
                                context.Response.ContentType = "image/JPEG";
                                return;
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                context.Response.OutputStream.Write(null, 0, 0);
                context.Response.ContentType = "image/JPEG";
            }

        }

        public bool SaveImageToDisk(byte[] image, string imageFilepath)
        {
            try
            {
                using (Image _image = Image.FromStream(new MemoryStream(image)))
                {
                    _image.Save(imageFilepath, ImageFormat.Jpeg);
                    return true;
                }
            }
            catch (Exception)
            {
                return false;
            }
        }

        public byte[] GetImageInBytes(string imageFilepath)
        {
            if (System.IO.File.Exists(imageFilepath))
            {
                Image _img = Image.FromFile(imageFilepath);
                ImageConverter _imageConverter = new ImageConverter();
                byte[] xByte = (byte[])_imageConverter.ConvertTo(_img, typeof(byte[]));
                return xByte;
            }
            else
                return null;
        }

        public bool IsReusable
        {
            get
            {
                return false;
            }
        }
    }
}