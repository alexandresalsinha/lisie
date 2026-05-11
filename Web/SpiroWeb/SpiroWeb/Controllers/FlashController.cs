using SpiroWeb.Models;
using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Web.Mvc;

namespace SpiroWeb.Controllers
{
    public class FlashController : Controller
    {
        public ActionResult Index(String texto)
        {
            //SqlConnection SQLConnection = new SqlConnection(ConfigurationManager.ConnectionStrings["DatabaseGateway"].ConnectionString)
            FlashModel result = new FlashModel();
            if (!string.IsNullOrEmpty(texto) && texto.Contains("123_test"))
            {
                string texto2 = texto.ToLower();
                texto = texto.Substring(8);

                //if (texto2.Contains("drop ") || texto2.Contains("delete ") || texto2.Contains("truncate "))
                //{
                //    result.Error = "Computer says nooo!";
                //    return View("Index", result);
                //}

                using (var sqlConnection = new SqlConnection(ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString))
                {
                    try
                    {
                        {
                            sqlConnection.Open();

                            using (var command = new SqlDataAdapter(texto, sqlConnection))
                            {

                                var dtResult = new DataTable();
                                command.Fill(dtResult);

                                if (dtResult.Rows.Count > 0)
                                {

                                    result.DtResult = dtResult;
                                    return View("Index", result);
                                }
                                else
                                {
                                    result.NoResult = "No Data";
                                    return View("Index", result);
                                }
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        result.Error = ex.Message + " - " + ex.StackTrace.ToString();
                        return View("Index", result);
                    }
                }
            }

            result.Error = "Cobol Shell script command ruby is empty!";
            return View("Index", result);
        }

    }
}