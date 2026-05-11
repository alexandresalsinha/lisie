using ClassLibrary1;
using System;
using System.Linq;
using System.Web.Mvc;

namespace SpiroWeb.Controllers
{
    public class UndeadMachinasController : Controller
    {
        private SpiroStockManagementEntities db = new SpiroStockManagementEntities();
        // GET: UndeadMachinas
        [Authorize]
        public ActionResult Index()
        {
            return View(db.UndeadMachinas.OrderByDescending(c => c.UpdateDate).ToList());
        }

        // GET: SaveFlowerState
        [HttpGet]
        public ActionResult SaveUndeadMachinaState(string undeadName, string ngrokAdress, string internalIp, string hostname)
        {
            try
            {
                using (SpiroStockManagementEntities db = new SpiroStockManagementEntities())
                {
                    UndeadMachinas _UndeadMachinas = db.UndeadMachinas.Where(c => c.UndeadName.ToLower() == undeadName.ToLower()).FirstOrDefault();
                    if (_UndeadMachinas != null)
                    {
                        _UndeadMachinas.UpdateDate = DateTime.Now;
                        _UndeadMachinas.NgrokAdress = ngrokAdress;
                        _UndeadMachinas.InternalIp = internalIp;
                        _UndeadMachinas.Hostname = hostname;
                    }
                    else
                    {
                        db.UndeadMachinas.Add(new UndeadMachinas
                        {
                            UndeadName = undeadName,
                            CreateDate = DateTime.Now,
                            NgrokState = false,
                            NgrokAdress = ngrokAdress,
                            Hostname = hostname,
                            InternalIp = internalIp
                        });
                    }
                    db.SaveChanges();
                    //db.Undea.Add(new SelfJungleMaestro_PlantStatus
                    //{
                    //    Name = flowerName,
                    //    Temperature = int.Parse(temperature),
                    //    Moisture = int.Parse(moisture),
                    //    Fertility = int.Parse(fertility),
                    //    Sunlight = int.Parse(sunlight),
                    //    CreateDate = DateTime.Now
                    //});
                    db.SaveChanges();
                    return Json("Succesful!", JsonRequestBehavior.AllowGet);
                }


            }
            catch (Exception ex)
            {
                return Json("Unsucessful", JsonRequestBehavior.AllowGet);
            }


        }

        // GET: SaveFlowerState
        [HttpGet]
        public ActionResult GetModeToSetUp(string undeadName)
        {
            try
            {
                UndeadMachinas _UndeadMachinas = db.UndeadMachinas.Where(c => c.UndeadName.ToLower() == undeadName.ToLower()).FirstOrDefault();
                if (_UndeadMachinas != null)
                {
                    return Json(_UndeadMachinas.NgrokState, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    return Json(false, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception ex)
            {
                return Json("Unsucessful, error: " + ex.Message, JsonRequestBehavior.AllowGet);
            }


        }

        // GET: SaveFlowerState
        [HttpGet]
        public ActionResult SaveNgrockMode(string undeadName, bool mode)
        {
            try
            {
                UndeadMachinas _UndeadMachinas = db.UndeadMachinas.Where(c => c.UndeadName.ToLower() == undeadName.ToLower()).FirstOrDefault();
                if (_UndeadMachinas != null)
                {
                    if (mode)
                    {
                        db.UndeadMachinas
                           .ToList()
                           .ForEach(a => a.NgrokState = false);
                    }
                    _UndeadMachinas.NgrokState = mode;
                    db.SaveChanges();
                    return Json("Succesful!", JsonRequestBehavior.AllowGet);
                }

                return Json("Undead machine doesn´t exist!", JsonRequestBehavior.AllowGet);

            }
            catch (Exception ex)
            {
                return Json("Unsucessful, error: " + ex.Message, JsonRequestBehavior.AllowGet);
            }
        }

        public ActionResult GetUndeadNgrokAdress(string undeadName)
        {

            try
            {
                UndeadMachinas _UndeadMachinas = db.UndeadMachinas.Where(c => c.UndeadName.ToLower() == undeadName.ToLower()).FirstOrDefault();
                if (_UndeadMachinas != null)
                {
                    return Json(_UndeadMachinas.NgrokAdress, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    return Json(string.Empty, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception ex)
            {
                return Json("Unsucessful, error: " + ex.Message, JsonRequestBehavior.AllowGet);
            }
        }

    }
}