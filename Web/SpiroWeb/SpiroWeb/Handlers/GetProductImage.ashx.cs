using ClassLibrary1;
using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Net;
using System.Web;

namespace SpiroWeb.Handlers
{
    /// <summary>
    /// Summary description for GetProductImage
    /// </summary>
    public class GetProductImage : IHttpHandler
    {
        private SpiroStockManagementEntities db = new SpiroStockManagementEntities();

        public void ProcessRequest(HttpContext context)
        {
            try
            {
                //if (context.Request.QueryString["productId"] != null && context.Request.QueryString["width"] != null && context.Request.QueryString["height"] != null)
                if (context.Request.QueryString["productId"] != null)
                {
                    string _productId = context.Request.QueryString["productId"];
                    //int _width = int.Parse(context.Request.QueryString["width"].ToString());
                    //int _height = int.Parse(context.Request.QueryString["height"].ToString());
                    string _imageUrl = HttpContext.Current.Server.MapPath("~/App_Data/ProductsPictures/" + _productId + ".jpg");

                    if (System.IO.File.Exists(_imageUrl))
                    {
                        byte[] _imageFromDisk = GetImageInBytes(_imageUrl);
                        context.Response.OutputStream.Write(_imageFromDisk, 0, _imageFromDisk.Length);
                        context.Response.ContentType = "image/JPEG";
                        return;
                    }
                    else
                    {
                        Products products = db.Products.Find(int.Parse(_productId));
                        if (products != null)
                        {
                            if (products.Picture != null)
                            {
                                //See if is to resize
                                if (context.Request.QueryString["width"] != null && context.Request.QueryString["height"] != null)
                                {
                                    int _width = int.Parse(context.Request.QueryString["width"]);
                                    int _height = int.Parse(context.Request.QueryString["height"]);
                                    Image _resizedImage = SpiroWeb.Helpers.Imager.PutOnWhiteCanvas(SpiroWeb.Helpers.Imager.byteArrayToImage(products.Picture), _width, _height);
                                    byte[] _resizedImageBytes = SpiroWeb.Helpers.Imager.imageToByteArray(_resizedImage);
                                    context.Response.OutputStream.Write(_resizedImageBytes, 0, _resizedImageBytes.Length);
                                    context.Response.ContentType = "image/JPEG";
                                    return;
                                }
                                SaveImageToDisk(products.Picture, HttpContext.Current.Server.MapPath("~/App_Data/ProductsPictures/" + _productId + ".jpg"));
                                context.Response.OutputStream.Write(products.Picture, 0, products.Picture.Length);
                                context.Response.ContentType = "image/JPEG";
                                return;
                            }
                        }
                    }
                }

                if (context.Request.QueryString["url"] != null)
                {
                    string _imageUrl = context.Request.QueryString["url"];

                    string _AppDataPath = System.Web.Hosting.HostingEnvironment.MapPath("~/App_Data/tempimg.temp");
                    WebClient _client = new WebClient();
                    _client.DownloadFile(new Uri(_imageUrl), _AppDataPath);
                    byte[] _imageInBase64 = Helpers.ManageImage.GetBase64OfImagePath(_AppDataPath);

                    if (context.Request.QueryString["width"] != null && context.Request.QueryString["height"] != null)
                    {
                        int _width = int.Parse(context.Request.QueryString["width"]);
                        int _height = int.Parse(context.Request.QueryString["height"]);
                        Image _resizedImage = SpiroWeb.Helpers.Imager.Resize(SpiroWeb.Helpers.Imager.byteArrayToImage(_imageInBase64), _width, _height, false);
                        byte[] _resizedImageBytes = SpiroWeb.Helpers.Imager.imageToByteArray(_resizedImage);
                        context.Response.OutputStream.Write(_resizedImageBytes, 0, _resizedImageBytes.Length);
                        context.Response.ContentType = "image/JPEG";
                        return;
                    }
                    else
                    {
                        context.Response.OutputStream.Write(_imageInBase64, 0, _imageInBase64.Length);
                        context.Response.ContentType = "image/JPEG";
                        return;
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