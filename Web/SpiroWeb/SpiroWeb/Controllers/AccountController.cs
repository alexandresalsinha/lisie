using ClassLibrary1;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security;
using SpiroWeb.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace SpiroWeb.Controllers
{
    [Authorize]
    public class AccountController : Controller
    {
        private ApplicationSignInManager _signInManager;
        private ApplicationUserManager _userManager;

        private SpiroStockManagementEntities db = new SpiroStockManagementEntities();

        public AccountController()
        {
        }

        public AccountController(ApplicationUserManager userManager, ApplicationSignInManager signInManager)
        {
            UserManager = userManager;
            SignInManager = signInManager;
        }

        public ApplicationSignInManager SignInManager
        {
            get
            {
                return _signInManager ?? HttpContext.GetOwinContext().Get<ApplicationSignInManager>();
            }
            private set
            {
                _signInManager = value;
            }
        }

        public ApplicationUserManager UserManager
        {
            get
            {
                return _userManager ?? HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>();
            }
            private set
            {
                _userManager = value;
            }
        }

        //
        // GET: /Account/Login
        [AllowAnonymous]
        public ActionResult Login(string returnUrl)
        {
            ViewBag.ReturnUrl = returnUrl;
            return View();
        }

        //
        // POST: /Account/Login
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Login(LoginViewModel model, string returnUrl)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            // Require the user to have a confirmed email before they can log on.
            var user = await UserManager.FindByNameAsync(model.Email);
            //if (user != null)
            //{
            //    if (!await UserManager.IsEmailConfirmedAsync(user.Id))
            //    {
            //        string callbackUrl = await SendEmailConfirmationTokenAsync(user.Id, "Confirm your account-Resend");

            //        ViewBag.VerifyAccount = "You must have a confirmed email to log on. Check your email";

            //        return View("Login");
            //    }
            //}

            // This doesn't count login failures towards account lockout
            // To enable password failures to trigger account lockout, change to shouldLockout: true
            var result = await SignInManager.PasswordSignInAsync(model.Email, model.Password, model.RememberMe, shouldLockout: false);
            switch (result)
            {
                case SignInStatus.Success:
                    var claims = new List<Claim>();
                    claims.Add(new Claim(ClaimTypes.Name, model.Email.Substring(0, model.Email.IndexOf("@"))));
                    claims.Add(new Claim(ClaimTypes.Email, model.Email));
                    var id = new ClaimsIdentity(claims,
                                                DefaultAuthenticationTypes.ApplicationCookie);

                    var ctx = Request.GetOwinContext();
                    var authenticationManager = ctx.Authentication;
                    authenticationManager.SignIn(id);
                    return RedirectToLocal(returnUrl);
                case SignInStatus.LockedOut:
                    return View("Lockout");
                case SignInStatus.RequiresVerification:
                    return RedirectToAction("SendCode", new { ReturnUrl = returnUrl, RememberMe = model.RememberMe });
                case SignInStatus.Failure:
                default:
                    ModelState.AddModelError("", "Invalid login attempt.");
                    return View(model);
            }
        }

        //
        // POST: /Account/Login
        //[HttpPost]
        [AllowAnonymous]
        [AllowCrossSiteJsonAttribute]
        public async Task<JsonResult> LoginAndroid(string UserEmail, string UserPassword)
        {
            if (UserEmail != null && UserPassword != null && !string.IsNullOrEmpty(UserEmail) && !string.IsNullOrEmpty(UserPassword))
            {
                // Require the user to have a confirmed email before they can log on.
                var user = await UserManager.FindByNameAsync(UserEmail);
                if (user != null)
                {
                    if (!await UserManager.IsEmailConfirmedAsync(user.Id))
                    {
                        string callbackUrl = await SendEmailConfirmationTokenAsync(user.Id, UserEmail, "Confirm your account-Resend");

                        ViewBag.VerifyAccount = "You must have a confirmed email to log on. Check your email";
                        return Json(new Tuple<bool, string>(false, "Email ainda não confirmado. Vai ao teu email e confirma a conta"), JsonRequestBehavior.AllowGet);

                    }
                }

                // This doesn't count login failures towards account lockout
                // To enable password failures to trigger account lockout, change to shouldLockout: true
                var result = await SignInManager.PasswordSignInAsync(UserEmail, UserPassword, true, shouldLockout: false);
                switch (result)
                {
                    case SignInStatus.Success:
                        var claims = new List<Claim>();
                        claims.Add(new Claim(ClaimTypes.Name, UserEmail.Substring(0, UserEmail.IndexOf("@"))));
                        claims.Add(new Claim(ClaimTypes.Email, UserEmail));
                        var id = new ClaimsIdentity(claims,
                                                    DefaultAuthenticationTypes.ApplicationCookie);

                        var ctx = Request.GetOwinContext();
                        var authenticationManager = ctx.Authentication;
                        authenticationManager.SignIn(id);
                        //return new Tuple<bool, string>(true, user.Id.ToString());
                        return Json(new Tuple<bool, string>(true, user.Id.ToString()), JsonRequestBehavior.AllowGet);
                    case SignInStatus.LockedOut:
                    //return View("Lockout");
                    case SignInStatus.RequiresVerification:
                    // return RedirectToAction("SendCode", new { ReturnUrl = returnUrl, RememberMe = model.RememberMe });
                    case SignInStatus.Failure:
                    default:
                        ModelState.AddModelError("", "Invalid login attempt.");
                        return Json(new Tuple<bool, string>(false, string.Empty), JsonRequestBehavior.AllowGet);
                }
            }
            return Json(new Tuple<bool, string>(false, string.Empty), JsonRequestBehavior.AllowGet);
            //return new Tuple<bool, string>(false, string.Empty);
        }


        //NEW ONES
        [AllowAnonymous]
        public async Task<JsonResult> LoginUser(string email, string password)
        {
            if (email != null && password != null && !string.IsNullOrEmpty(email) && !string.IsNullOrEmpty(password))
            {
                // Require the user to have a confirmed email before they can log on.
                var user = await UserManager.FindByNameAsync(email);
                if (user != null)
                {
                    if (!await UserManager.IsEmailConfirmedAsync(user.Id))
                    {
                        string callbackUrl = await SendEmailConfirmationTokenAsync(user.Id, email, "Confirm your account-Resend");

                        ViewBag.VerifyAccount = "You must have a confirmed email to log on. Check your email";
                        return Json(new Tuple<bool, string>(false, "Email ainda não confirmado. Vai ao teu email e confirma a conta"), JsonRequestBehavior.AllowGet);

                    }
                }

                // This doesn't count login failures towards account lockout
                // To enable password failures to trigger account lockout, change to shouldLockout: true
                var result = await SignInManager.PasswordSignInAsync(email, password, true, shouldLockout: false);
                switch (result)
                {
                    case SignInStatus.Success:
                        var claims = new List<Claim>();
                        claims.Add(new Claim(ClaimTypes.Name, email.Substring(0, email.IndexOf("@"))));
                        claims.Add(new Claim(ClaimTypes.Email, email));
                        var id = new ClaimsIdentity(claims,
                                                    DefaultAuthenticationTypes.ApplicationCookie);

                        var ctx = Request.GetOwinContext();
                        var authenticationManager = ctx.Authentication;
                        authenticationManager.SignIn(id);
                        //return new Tuple<bool, string>(true, user.Id.ToString());
                        return Json(new Tuple<bool, string>(true, user.Id.ToString()), JsonRequestBehavior.AllowGet);
                    case SignInStatus.LockedOut:
                    //return View("Lockout");
                    case SignInStatus.RequiresVerification:
                    // return RedirectToAction("SendCode", new { ReturnUrl = returnUrl, RememberMe = model.RememberMe });
                    case SignInStatus.Failure:
                    default:
                        ModelState.AddModelError("", "Invalid login attempt.");
                        return Json(new Tuple<bool, string>(false, string.Empty), JsonRequestBehavior.AllowGet);
                }
            }
            return Json(new Tuple<bool, string>(false, string.Empty), JsonRequestBehavior.AllowGet);
            //return new Tuple<bool, string>(false, string.Empty);
        }
        [AllowAnonymous]
        [AllowCrossSiteJson]
        public async Task<JsonResult> LoginGoogle(string email, string token, string name)
        {
            if (email != null && token != null && !string.IsNullOrEmpty(email) && !string.IsNullOrEmpty(token))
            {
                // Require the user to have a confirmed email before they can log on.
                var user = await UserManager.FindByNameAsync(email);

                if (user == null) //if user doesn´t exist , register and then login
                {
                    var _registerSuccess = await RegisterUserFromOauth(email, "lisie!2021@magic.");
                    if (_registerSuccess.Item1) //sucess
                    {
                        user = await UserManager.FindByNameAsync(email);
                        return Json(new Tuple<bool, string>(true, user.Id), JsonRequestBehavior.AllowGet);
                    }
                    else
                        return Json(new Tuple<bool, string>(false, "error registering new user"), JsonRequestBehavior.AllowGet);

                }
                else
                    return Json(new Tuple<bool, string>(true, user.Id), JsonRequestBehavior.AllowGet);
                //In google authenticaton, not required email confirmation
                //if (user != null)
                //{
                //    if (!await UserManager.IsEmailConfirmedAsync(user.Id))
                //    {
                //        string callbackUrl = await SendEmailConfirmationTokenAsync(user.Id, email, "Confirm your account-Resend");

                //        ViewBag.VerifyAccount = "You must have a confirmed email to log on. Check your email";
                //        return Json(new Tuple<bool, string>(false, "Email ainda não confirmado. Vai ao teu email e confirma a conta"), JsonRequestBehavior.AllowGet);

                //    }
                //}

                //generate password
                string password = "lisie!2021@magic.";

                //TODO - security - confirm token


                // This doesn't count login failures towards account lockout
                // To enable password failures to trigger account lockout, change to shouldLockout: true
                var result = await SignInManager.PasswordSignInAsync(email, password, true, shouldLockout: false);
                switch (result)
                {
                    case SignInStatus.Success:
                        var claims = new List<Claim>();
                        claims.Add(new Claim(ClaimTypes.Name, email.Substring(0, email.IndexOf("@"))));
                        claims.Add(new Claim(ClaimTypes.Email, email));
                        var id = new ClaimsIdentity(claims,
                                                    DefaultAuthenticationTypes.ApplicationCookie);

                        var ctx = Request.GetOwinContext();
                        var authenticationManager = ctx.Authentication;
                        authenticationManager.SignIn(id);
                        //return new Tuple<bool, string>(true, user.Id.ToString());
                        return Json(new Tuple<bool, string>(true, user.Id.ToString()), JsonRequestBehavior.AllowGet);
                    case SignInStatus.LockedOut:
                    //return View("Lockout");
                    case SignInStatus.RequiresVerification:
                    // return RedirectToAction("SendCode", new { ReturnUrl = returnUrl, RememberMe = model.RememberMe });
                    case SignInStatus.Failure:
                    default:
                        ModelState.AddModelError("", "Invalid login attempt.");
                        return Json(new Tuple<bool, string>(false, string.Empty), JsonRequestBehavior.AllowGet);
                }
            }
            return Json(new Tuple<bool, string>(false, string.Empty), JsonRequestBehavior.AllowGet);
            //return new Tuple<bool, string>(false, string.Empty);
        }
        [AllowAnonymous]
        [AllowCrossSiteJsonAttribute]
        public async Task<JsonResult> LoginFacebook(string email, string token, string name)
        {
            if (email != null && token != null && !string.IsNullOrEmpty(email) && !string.IsNullOrEmpty(token))
            {
                // Require the user to have a confirmed email before they can log on.
                var user = await UserManager.FindByNameAsync(email);
                if (user == null) //if user doesn´t exist , register and then login
                {
                    var _registerSuccess = await RegisterUserFromOauth(email, "lisie!2021@magic.");
                    if (_registerSuccess.Item1) //sucess
                    {
                        user = await UserManager.FindByNameAsync(email);
                        return Json(new Tuple<bool, string>(true, user.Id), JsonRequestBehavior.AllowGet);
                    }
                    else
                        return Json(new Tuple<bool, string>(false, "error registering new user"), JsonRequestBehavior.AllowGet);

                }
                else
                    return Json(new Tuple<bool, string>(true, user.Id), JsonRequestBehavior.AllowGet);
                //In facebook login 
                //if (user != null)
                //{
                //    if (!await UserManager.IsEmailConfirmedAsync(user.Id))
                //    {
                //        string callbackUrl = await SendEmailConfirmationTokenAsync(user.Id, email, "Confirm your account-Resend");

                //        ViewBag.VerifyAccount = "You must have a confirmed email to log on. Check your email";
                //        return Json(new Tuple<bool, string>(false, "Email ainda não confirmado. Vai ao teu email e confirma a conta"), JsonRequestBehavior.AllowGet);

                //    }
                //}

                //generate password
                string password = "lisie!2021@magic.";

                //TODO - confirm token

                // This doesn't count login failures towards account lockout
                // To enable password failures to trigger account lockout, change to shouldLockout: true
                var result = await SignInManager.PasswordSignInAsync(email, password, true, shouldLockout: false);
                switch (result)
                {
                    case SignInStatus.Success:
                        var claims = new List<Claim>();
                        claims.Add(new Claim(ClaimTypes.Name, email.Substring(0, email.IndexOf("@"))));
                        claims.Add(new Claim(ClaimTypes.Email, email));
                        var id = new ClaimsIdentity(claims,
                                                    DefaultAuthenticationTypes.ApplicationCookie);

                        var ctx = Request.GetOwinContext();
                        var authenticationManager = ctx.Authentication;
                        authenticationManager.SignIn(id);
                        //return new Tuple<bool, string>(true, user.Id.ToString());
                        return Json(new Tuple<bool, string>(true, user.Id.ToString()), JsonRequestBehavior.AllowGet);
                    case SignInStatus.LockedOut:
                    //return View("Lockout");
                    case SignInStatus.RequiresVerification:
                    // return RedirectToAction("SendCode", new { ReturnUrl = returnUrl, RememberMe = model.RememberMe });
                    case SignInStatus.Failure:
                    default:
                        ModelState.AddModelError("", "Invalid login attempt.");
                        return Json(new Tuple<bool, string>(false, string.Empty), JsonRequestBehavior.AllowGet);
                }
            }
            return Json(new Tuple<bool, string>(false, string.Empty), JsonRequestBehavior.AllowGet);
            //return new Tuple<bool, string>(false, string.Empty);
        }

        [AllowAnonymous]
        [AllowCrossSiteJsonAttribute]
        public async Task<JsonResult> LoginApple(string email, string appleUserId)
        {
            if (email != null && !string.IsNullOrEmpty(email) && appleUserId != null && !string.IsNullOrEmpty(appleUserId))
            {
                // Require the user to have a confirmed email before they can log on.
                var user = await UserManager.FindByNameAsync(email);
                if (user == null) //if user doesn´t exist , register and then login
                {
                    var _registerSuccess = await RegisterUserFromOauth(email, "lisie!2021@magic.");
                    if (_registerSuccess.Item1) //sucess
                    {
                        user = await UserManager.FindByNameAsync(email);
                        //save also apple user Id to another table - UsersApple
                        var _appleUserCreated = SpiroWeb.Managers.UsersManager.AddAppleUser(appleUserId, email);
                        if (_appleUserCreated != null)
                        {
                            return Json(new Tuple<bool, string>(true, user.Id), JsonRequestBehavior.AllowGet);

                        }
                        else
                        {
                            return Json(new Tuple<bool, string>(false, "error registering new apple user"), JsonRequestBehavior.AllowGet);
                        }
                    }
                    else
                        return Json(new Tuple<bool, string>(false, "error registering new user"), JsonRequestBehavior.AllowGet);

                }
                else
                    return Json(new Tuple<bool, string>(true, user.Id), JsonRequestBehavior.AllowGet);
            }
            if (appleUserId != null && !string.IsNullOrEmpty(appleUserId))
            {
                //get user apple email by user apple id
                var _appleEmailByAppleUserId = string.Empty;

                var _AppleUser = SpiroWeb.Managers.UsersManager.GetAppleUser(appleUserId);
                if (_AppleUser != null)
                {
                    _appleEmailByAppleUserId = _AppleUser.Email;
                }

                if (!string.IsNullOrEmpty(_appleEmailByAppleUserId))
                {
                    var user = await UserManager.FindByNameAsync(_appleEmailByAppleUserId);
                    if (user != null) //if user doesn´t exist , register and then login
                    {
                        return Json(new Tuple<bool, string>(true, user.Id), JsonRequestBehavior.AllowGet);
                    }
                    else
                    {
                        return Json(new Tuple<bool, string>(false, "error logging in apple user"), JsonRequestBehavior.AllowGet);
                    }
                }
            }
            return Json(new Tuple<bool, string>(false, string.Empty), JsonRequestBehavior.AllowGet);
        }

        [AllowAnonymous]
        [AllowCrossSiteJsonAttribute]
        public JsonResult GetLoginApple(string appleUserId)
        {
            try
            {
                if (appleUserId != null && !string.IsNullOrEmpty(appleUserId))
                {
                    // Require the user to have a confirmed email before they can log on.
                    var user = Managers.UsersManager.GetAppleUser(appleUserId);
                    if (user != null) //if user doesn´t exist , register and then login
                    {
                        return Json(new Tuple<bool, string>(true, user.Email), JsonRequestBehavior.AllowGet);
                    }
                    else
                        return Json(new Tuple<bool, string>(false, "Apple user not found"), JsonRequestBehavior.AllowGet);
                }
                return Json(new Tuple<bool, string>(false, string.Empty), JsonRequestBehavior.AllowGet);

            }
            catch (Exception ex)
            {
                return Json(new Tuple<bool, string>(false, ex.Message), JsonRequestBehavior.AllowGet);
            }
        }


        private readonly Random _random = new Random();
        public string RandomString(int size, bool lowerCase = false)
        {
            var builder = new StringBuilder(size);

            // Unicode/ASCII Letters are divided into two blocks
            // (Letters 65–90 / 97–122):
            // The first group containing the uppercase letters and
            // the second group containing the lowercase.  

            // char is a single Unicode character  
            char offset = lowerCase ? 'a' : 'A';
            const int lettersOffset = 26; // A...Z or a..z: length=26  

            for (var i = 0; i < size; i++)
            {
                var @char = (char)_random.Next(offset, offset + lettersOffset);
                builder.Append(@char);
            }

            return lowerCase ? builder.ToString().ToLower() : builder.ToString();
        }

        [AllowAnonymous]
        [AllowCrossSiteJsonAttribute]
        public async Task<JsonResult> LoginAnonymous(string id)
        {
            if (id != null && !string.IsNullOrEmpty(id))
            {
                // Require the user to have a confirmed email before they can log on.
                var user = await UserManager.FindByIdAsync(id);
                if (user == null) //if user doesn´t exist , register and then login
                {
                    //generate email 
                    int _totalUsers = Managers.UsersManager.GetTotal();
                    string _generatedEmail = (_totalUsers + 1) + RandomString(10, true) + "@temp.lisie";


                    var _registerSuccess = await RegisterUserFromOauth(_generatedEmail, "lisie!2021@magic.");
                    if (_registerSuccess.Item1) //sucess
                    {
                        user = await UserManager.FindByNameAsync(_generatedEmail);
                        return Json(new Tuple<bool, string>(true, user.Id), JsonRequestBehavior.AllowGet);
                    }
                    else
                        return Json(new Tuple<bool, string>(false, "error registering new user"), JsonRequestBehavior.AllowGet);

                }
                else
                    return Json(new Tuple<bool, string>(true, user.Id), JsonRequestBehavior.AllowGet);
            }
            else
            {
                int _totalUsers = Managers.UsersManager.GetTotal();
                string _generatedEmail = (_totalUsers + 1) + RandomString(10, true) + "@temp.lisie";


                var _registerSuccess = await RegisterUserFromOauth(_generatedEmail, "lisie!2021@magic.");
                if (_registerSuccess.Item1) //sucess
                {
                    var _user = await UserManager.FindByNameAsync(_generatedEmail);
                    return Json(new Tuple<bool, string>(true, _user.Id), JsonRequestBehavior.AllowGet);
                }
                else
                    return Json(new Tuple<bool, string>(false, "error registering new user"), JsonRequestBehavior.AllowGet);
            }
            //return Json(new Tuple<bool, string>(false, string.Empty), JsonRequestBehavior.AllowGet);
        }


        //
        // GET: /Account/VerifyCode
        [AllowAnonymous]
        public async Task<ActionResult> VerifyCode(string provider, string returnUrl, bool rememberMe)
        {
            // Require that the user has already logged in via username/password or external login
            if (!await SignInManager.HasBeenVerifiedAsync())
            {
                return View("Error");
            }
            return View(new VerifyCodeViewModel { Provider = provider, ReturnUrl = returnUrl, RememberMe = rememberMe });
        }

        //
        // POST: /Account/VerifyCode
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> VerifyCode(VerifyCodeViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            // The following code protects for brute force attacks against the two factor codes. 
            // If a user enters incorrect codes for a specified amount of time then the user account 
            // will be locked out for a specified amount of time. 
            // You can configure the account lockout settings in IdentityConfig
            var result = await SignInManager.TwoFactorSignInAsync(model.Provider, model.Code, isPersistent: model.RememberMe, rememberBrowser: model.RememberBrowser);
            switch (result)
            {
                case SignInStatus.Success:
                    return RedirectToLocal(model.ReturnUrl);
                case SignInStatus.LockedOut:
                    return View("Lockout");
                case SignInStatus.Failure:
                default:
                    ModelState.AddModelError("", "Invalid code.");
                    return View(model);
            }
        }

        //
        // GET: /Account/Register
        [AllowAnonymous]
        public ActionResult Register()
        {
            return View();
        }

        //
        // POST: /Account/Register
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Register(RegisterViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = new ApplicationUser { UserName = model.Email, Email = model.Email };
                var result = await UserManager.CreateAsync(user, model.Password);
                if (result.Succeeded)
                {
                    //await SignInManager.SignInAsync(user, isPersistent:false, rememberBrowser:false);
                    //update AspNetUser CareateDate
                    var _newUser = db.AspNetUsers.Where(c => c.Id == user.Id).FirstOrDefault();
                    if (_newUser != null)
                    {
                        _newUser.CreateDate = DateTime.Now;
                        db.SaveChanges();
                    }
                    // For more information on how to enable account confirmation and password reset please visit http://go.microsoft.com/fwlink/?LinkID=320771
                    // Send an email with this link
                    string code = await UserManager.GenerateEmailConfirmationTokenAsync(user.Id);
                    string callbackUrl = await SendEmailConfirmationTokenAsync(user.Id, model.Email, "Confirm your account");
                    await UserManager.SendEmailAsync(user.Id, "Confirm your account", "Please confirm your account by clicking <a href=\"" + callbackUrl + "\">here</a>");


                    ViewBag.Message = "Check your email and confirm your account, you must be confirmed "
                        + "before you can log in.";
                    return View("Info");
                    //return RedirectToAction("Index", "Home");
                }
                AddErrors(result);
            }

            // If we got this far, something failed, redisplay form
            return View(model);
        }

        [HttpPost]
        [AllowAnonymous]
        [AllowCrossSiteJson]
        public async Task<ActionResult> RegisterAndroid(RegisterViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = new ApplicationUser { UserName = model.Email, Email = model.Email };
                var result = await UserManager.CreateAsync(user, model.Password);
                if (result.Succeeded)
                {
                    //await SignInManager.SignInAsync(user, isPersistent:false, rememberBrowser:false);

                    // For more information on how to enable account confirmation and password reset please visit http://go.microsoft.com/fwlink/?LinkID=320771
                    // Send an email with this link

                    //TODO - UNCOMMENT CONFIRMATION EMAIL
                    string code = await UserManager.GenerateEmailConfirmationTokenAsync(user.Id);
                    var callbackUrl = Url.Action("ConfirmEmail", "Account", new { userId = user.Id, code = code }, protocol: Request.Url.Scheme);
                    //await UserManager.SendEmailAsync(user.Id, "Confirm your account", "Please confirm your account by clicking "Please confirm your account by clicking <a href=\"" + callbackUrl + "\">here</a>");

                    Helpers.Email.Send("lisie@lisie.app", model.Email, "Lisie - Confirmação de conta", "A Lisie dá-te as boas vindas! 🥕 <br> Carrega neste <a href=\"" + callbackUrl + " \">link</a> para ativares a tua conta!");


                    //ViewBag.Message = "Check your email and confirm your account, you must be confirmed "
                    //    + "before you can log in.";

                    //Update User CreateDate
                    AspNetUsers _userCreated = db.AspNetUsers.Where(c => c.Id.Equals(user.Id)).FirstOrDefault();
                    if (_userCreated != null)
                    {
                        _userCreated.CreateDate = DateTime.Now;
                        db.Entry(_userCreated).State = System.Data.Entity.EntityState.Modified;
                        db.SaveChanges();

                        //warn myself of new user registered
                        Helpers.FirebaseAndroid.SendNotification("9ff8224f-17cf-49fb-b555-05779a13eb40", "newUserRegistered:Novo utilizador registado " + user.Email + " Total - " + db.AspNetUsers.Count().ToString());
                        //increase ScoreBoard with SignalR
                        var _totalUsers = db.AspNetUsers.Count();
                        Microsoft.AspNet.SignalR.StockTicker.StockTicker.Instance.BroadcastNewUser("9ff8224f-17cf-49fb-b555-05779a13eb40", _totalUsers);
                    }

                    return Json(new Tuple<bool, string>(true, user.Id.ToString()), JsonRequestBehavior.AllowGet);
                }
                else
                {
                    if (result.Errors != null && result.Errors.Count() > 1)
                        return Json(new Tuple<bool, string>(false, ((string[])result.Errors)[1]), JsonRequestBehavior.AllowGet);
                    else
                        return Json(new Tuple<bool, string>(false, "Error"), JsonRequestBehavior.AllowGet);
                }

                //AddErrors(result);
            }

            // If we got this far, something failed, redisplay form
            return Json(new Tuple<bool, string>(false, ""), JsonRequestBehavior.AllowGet);
        }

        [AllowAnonymous]
        public async Task<ActionResult> RegisterUser(string email, string password)
        {
            if (ModelState.IsValid)
            {
                var user = new ApplicationUser { UserName = email, Email = email };
                var result = await UserManager.CreateAsync(user, password);
                if (result.Succeeded)
                {
                    //await SignInManager.SignInAsync(user, isPersistent:false, rememberBrowser:false);

                    // For more information on how to enable account confirmation and password reset please visit http://go.microsoft.com/fwlink/?LinkID=320771
                    // Send an email with this link

                    //TODO - UNCOMMENT CONFIRMATION EMAIL
                    string code = await UserManager.GenerateEmailConfirmationTokenAsync(user.Id);
                    var callbackUrl = Url.Action("ConfirmEmail", "Account", new { userId = user.Id, code = code }, protocol: Request.Url.Scheme);
                    //await UserManager.SendEmailAsync(user.Id, "Confirm your account", "Please confirm your account by clicking "Please confirm your account by clicking <a href=\"" + callbackUrl + "\">here</a>");

                    Helpers.Email.Send("lisie@lisie.app", email, "Lisie - Confirmação de conta", "A Lisie dá-te as boas vindas! 🥕 <br> Carrega neste <a href=\"" + callbackUrl + " \">link</a> para ativares a tua conta!");


                    //ViewBag.Message = "Check your email and confirm your account, you must be confirmed "
                    //    + "before you can log in.";

                    //Update User CreateDate
                    AspNetUsers _userCreated = db.AspNetUsers.Where(c => c.Id.Equals(user.Id)).FirstOrDefault();
                    if (_userCreated != null)
                    {
                        _userCreated.CreateDate = DateTime.Now;
                        db.Entry(_userCreated).State = System.Data.Entity.EntityState.Modified;
                        db.SaveChanges();

                        //warn myself of new user registered
                        Helpers.FirebaseAndroid.SendNotification("9ff8224f-17cf-49fb-b555-05779a13eb40", "newUserRegistered:Novo utilizador registado " + user.Email + " Total - " + db.AspNetUsers.Count().ToString());
                    }

                    return Json(new Tuple<bool, string>(true, user.Id.ToString()), JsonRequestBehavior.AllowGet);
                }
                else
                {
                    if (result.Errors != null && result.Errors.Count() > 1)
                        return Json(new Tuple<bool, string>(false, ((string[])result.Errors)[1]), JsonRequestBehavior.AllowGet);
                    else
                        return Json(new Tuple<bool, string>(false, "Error"), JsonRequestBehavior.AllowGet);
                }

                //AddErrors(result);
            }

            // If we got this far, something failed, redisplay form
            return Json(new Tuple<bool, string>(false, "Error"), JsonRequestBehavior.AllowGet);
        }


        public async Task<Tuple<bool, string>> RegisterUserFromOauth(string userEmail, string password)
        {
            var user = new ApplicationUser { UserName = userEmail, Email = userEmail };
            var result = await UserManager.CreateAsync(user, password);
            if (result.Succeeded)
            {
                //await SignInManager.SignInAsync(user, isPersistent:false, rememberBrowser:false);

                // For more information on how to enable account confirmation and password reset please visit http://go.microsoft.com/fwlink/?LinkID=320771
                // Send an email with this link

                //TODO - UNCOMMENT CONFIRMATION EMAIL
                //string code = await UserManager.GenerateEmailConfirmationTokenAsync(user.Id);
                //var callbackUrl = Url.Action("ConfirmEmail", "Account", new { userId = user.Id, code = code }, protocol: Request.Url.Scheme);
                ////await UserManager.SendEmailAsync(user.Id, "Confirm your account", "Please confirm your account by clicking "Please confirm your account by clicking <a href=\"" + callbackUrl + "\">here</a>");

                //Helpers.Email.Send("accounts@lisie.app", model.Email, "Lisie - Confirmação de conta", "Para confirmares a tua conta na Lisie, abre este link <a href=\"" + callbackUrl + "\">aqui</a>");


                //ViewBag.Message = "Check your email and confirm your account, you must be confirmed "
                //    + "before you can log in.";

                //Update User CreateDate
                AspNetUsers _userCreated = db.AspNetUsers.Where(c => c.Id.Equals(user.Id)).FirstOrDefault();
                if (_userCreated != null)
                {
                    _userCreated.CreateDate = DateTime.Now;
                    _userCreated.EmailConfirmed = true; //if from Oauth confirm email not necessary
                    db.Entry(_userCreated).State = System.Data.Entity.EntityState.Modified;
                    db.SaveChanges();

                    //warn myself of new user registered
                    Helpers.FirebaseAndroid.SendNotification("9ff8224f-17cf-49fb-b555-05779a13eb40", "newUserRegistered:Novo utilizador registado " + user.Email + " Total - " + db.AspNetUsers.Count().ToString());
                }

                return new Tuple<bool, string>(true, user.Id.ToString());
            }
            else
            {
                if (result.Errors != null && result.Errors.Count() > 1)
                    return new Tuple<bool, string>(false, ((string[])result.Errors)[1]);
                else
                    return new Tuple<bool, string>(false, "Error");
            }
        }
        //
        // GET: /Account/ConfirmEmail
        [AllowAnonymous]
        public async Task<ActionResult> ConfirmEmail(string userId, string code)
        {
            if (userId == null || code == null)
            {
                return View("Error");
            }
            var result = await UserManager.ConfirmEmailAsync(userId, code);
            if (result.Succeeded)
            {
                if (Request.Browser.IsMobileDevice)
                {
                    return Redirect("com.lisie.org://Confirmed");
                }
                else
                {
                    return View(result.Succeeded ? "ConfirmEmail" : "Error");
                }
            }
            else
            {
                return View("Error");

            }
        }

        //
        // GET: /Account/ForgotPassword
        [AllowAnonymous]
        public ActionResult ForgotPassword()
        {
            return View();
        }

        //
        // POST: /Account/ForgotPassword
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> ForgotPassword(ForgotPasswordViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = await UserManager.FindByNameAsync(model.Email);
                //if (user == null || !(await UserManager.IsEmailConfirmedAsync(user.Id)))
                //{
                //    // Don't reveal that the user does not exist or is not confirmed
                //    return View("ForgotPasswordConfirmation");
                //}

                if (user != null)
                {
                    // For more information on how to enable account confirmation and password reset please visit http://go.microsoft.com/fwlink/?LinkID=320771
                    // Send an email with this link
                    string code = await UserManager.GeneratePasswordResetTokenAsync(user.Id);
                    var callbackUrl = Url.Action("ResetPassword", "Account", new { userId = user.Id, code = code }, protocol: Request.Url.Scheme);
                    //await UserManager.SendEmailAsync(user.Id, "Reset Password", "Please reset your password by clicking <a href=\"" + callbackUrl + "\">here</a>");

                    //user new way to send email
                    Helpers.Email.Send("lisie@lisie.app", model.Email, "Lisie - Recuperar Password", "Carrega neste <a href=\"" + callbackUrl + "\">link</a> para alterares a tua password! 🥑");

                    return RedirectToAction("ForgotPasswordConfirmation", "Account");
                }
                return View("ForgotPasswordConfirmation");
            }

            // If we got this far, something failed, redisplay form
            return View(model);
        }

        [HttpPost]
        [AllowAnonymous]
        [AllowCrossSiteJson]
        public async Task<ActionResult> ForgotPasswordAndroid(ForgotPasswordViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = await UserManager.FindByNameAsync(model.Email);
                if (user != null)
                {
                    string code = await UserManager.GeneratePasswordResetTokenAsync(user.Id);
                    var callbackUrl = Url.Action("ResetPassword", "Account", new { userId = user.Id, code = code, email = user.Email }, protocol: Request.Url.Scheme);
                    //await UserManager.SendEmailAsync(user.Id, "Reset Password", "Please reset your password by clicking <a href=\"" + callbackUrl + "\">here</a>");
                    //await UserManager.SendEmailAsync(user.Id, "Lisie - Recuperar Password", "Para fazeres reset à tua password clicka no link " + callbackUrl);
                    //user new way to send email
                    Helpers.Email.Send("lisie@lisie.app", model.Email, "Lisie - Recuperar Password", "Carrega neste <a href=\"" + callbackUrl + "\">link</a> para alterares a tua password! 🥑");
                    return Json(new Tuple<bool, string>(true, user.Id.ToString()), JsonRequestBehavior.AllowGet);
                }
                return Json(new Tuple<bool, string>(false, "Utilizador com esse email não existe"), JsonRequestBehavior.AllowGet);
            }

            return Json(new Tuple<bool, string>(false, string.Empty), JsonRequestBehavior.AllowGet);
        }

        //
        // GET: /Account/ForgotPasswordConfirmation
        [AllowAnonymous]
        public ActionResult ForgotPasswordConfirmation()
        {
            return View();
        }

        //
        // GET: /Account/ResetPassword
        [AllowAnonymous]
        public ActionResult ResetPassword(string code, string email = "")
        {
            if (Request.Browser.IsMobileDevice)
            {
                return Redirect("com.lisie.org://ResetPassword?email=" + email);
            }
            return code == null ? View("Error") : View();
        }

        //
        // POST: /Account/ResetPassword
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> ResetPassword(ResetPasswordViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }
            var user = await UserManager.FindByNameAsync(model.Email);
            if (user == null)
            {
                // Don't reveal that the user does not exist
                return RedirectToAction("ResetPasswordConfirmation", "Account");
            }
            var result = await UserManager.ResetPasswordAsync(user.Id, model.Code, model.Password);
            if (result.Succeeded)
            {
                return RedirectToAction("ResetPasswordConfirmation", "Account");
            }
            AddErrors(result);
            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        //[ValidateAntiForgeryToken]
        [AllowCrossSiteJson]
        public async Task<ActionResult> ResetPasswordMobile(ResetPasswordViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return Json(new Tuple<bool, string>(false, ""), JsonRequestBehavior.AllowGet);
            }
            var user = await UserManager.FindByNameAsync(model.Email);
            if (user == null)
            {
                return Json(new Tuple<bool, string>(false, ""), JsonRequestBehavior.AllowGet);
            }
            var token = await UserManager.GeneratePasswordResetTokenAsync(user.Id);
            var result = await UserManager.ResetPasswordAsync(user.Id, token, model.Password);
            if (result.Succeeded)
            {
                return Json(new Tuple<bool, string>(true, user.Id), JsonRequestBehavior.AllowGet);
            }
            return Json(new Tuple<bool, string>(false, ""), JsonRequestBehavior.AllowGet);
        }

        //
        // GET: /Account/ResetPasswordConfirmation
        [AllowAnonymous]
        public ActionResult ResetPasswordConfirmation()
        {
            return View();
        }

        //
        // POST: /Account/ExternalLogin
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public ActionResult ExternalLogin(string provider, string returnUrl)
        {
            // Request a redirect to the external login provider
            return new ChallengeResult(provider, Url.Action("ExternalLoginCallback", "Account", new { ReturnUrl = returnUrl }));
        }

        //
        // GET: /Account/SendCode
        [AllowAnonymous]
        public async Task<ActionResult> SendCode(string returnUrl, bool rememberMe)
        {
            var userId = await SignInManager.GetVerifiedUserIdAsync();
            if (userId == null)
            {
                return View("Error");
            }
            var userFactors = await UserManager.GetValidTwoFactorProvidersAsync(userId);
            var factorOptions = userFactors.Select(purpose => new SelectListItem { Text = purpose, Value = purpose }).ToList();
            return View(new SendCodeViewModel { Providers = factorOptions, ReturnUrl = returnUrl, RememberMe = rememberMe });
        }

        //
        // POST: /Account/SendCode
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> SendCode(SendCodeViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View();
            }

            // Generate the token and send it
            if (!await SignInManager.SendTwoFactorCodeAsync(model.SelectedProvider))
            {
                return View("Error");
            }
            return RedirectToAction("VerifyCode", new { Provider = model.SelectedProvider, ReturnUrl = model.ReturnUrl, RememberMe = model.RememberMe });
        }

        //
        // GET: /Account/ExternalLoginCallback
        [AllowAnonymous]
        public async Task<ActionResult> ExternalLoginCallback(string returnUrl)
        {
            var loginInfo = await AuthenticationManager.GetExternalLoginInfoAsync();
            if (loginInfo == null)
            {
                return RedirectToAction("Login");
            }

            // Sign in the user with this external login provider if the user already has a login
            var result = await SignInManager.ExternalSignInAsync(loginInfo, isPersistent: false);
            switch (result)
            {
                case SignInStatus.Success:
                    return RedirectToLocal(returnUrl);
                case SignInStatus.LockedOut:
                    return View("Lockout");
                case SignInStatus.RequiresVerification:
                    return RedirectToAction("SendCode", new { ReturnUrl = returnUrl, RememberMe = false });
                case SignInStatus.Failure:
                default:
                    // If the user does not have an account, then prompt the user to create an account
                    ViewBag.ReturnUrl = returnUrl;
                    ViewBag.LoginProvider = loginInfo.Login.LoginProvider;
                    return View("ExternalLoginConfirmation", new ExternalLoginConfirmationViewModel { Email = loginInfo.Email });
            }
        }

        //
        // POST: /Account/ExternalLoginConfirmation
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> ExternalLoginConfirmation(ExternalLoginConfirmationViewModel model, string returnUrl)
        {
            if (User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Index", "Manage");
            }

            if (ModelState.IsValid)
            {
                // Get the information about the user from the external login provider
                var info = await AuthenticationManager.GetExternalLoginInfoAsync();
                if (info == null)
                {
                    return View("ExternalLoginFailure");
                }
                var user = new ApplicationUser { UserName = model.Email, Email = model.Email };
                var result = await UserManager.CreateAsync(user);
                if (result.Succeeded)
                {
                    result = await UserManager.AddLoginAsync(user.Id, info.Login);
                    if (result.Succeeded)
                    {
                        await SignInManager.SignInAsync(user, isPersistent: false, rememberBrowser: false);
                        return RedirectToLocal(returnUrl);
                    }
                }
                AddErrors(result);
            }

            ViewBag.ReturnUrl = returnUrl;
            return View(model);
        }

        //
        // POST: /Account/LogOff
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult LogOff()
        {
            AuthenticationManager.SignOut();

            //cookie removal
            var ctx = Request.GetOwinContext();
            var authenticationManager = ctx.Authentication;
            authenticationManager.SignOut();

            return RedirectToAction("Index", "Home");
        }

        //
        // GET: /Account/ExternalLoginFailure
        [AllowAnonymous]
        public ActionResult ExternalLoginFailure()
        {
            return View();
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (_userManager != null)
                {
                    _userManager.Dispose();
                    _userManager = null;
                }

                if (_signInManager != null)
                {
                    _signInManager.Dispose();
                    _signInManager = null;
                }
            }

            base.Dispose(disposing);
        }

        private async Task<string> SendEmailConfirmationTokenAsync(string userID, string userEmail, string subject)
        {
            string code = await UserManager.GenerateEmailConfirmationTokenAsync(userID);
            var callbackUrl = Url.Action("ConfirmEmail", "Account", new { userId = userID, code = code }, protocol: Request.Url.Scheme);

            Helpers.Email.Send("lisie@lisie.app", userEmail, "Lisie - Confirmação de conta", "A Lisie dá-te as boas vindas! 🥕 <br> Carrega neste <a href=\"" + callbackUrl + " \">link</a> para ativares a tua conta!");

            //await UserManager.SendEmailAsync(userID, subject, "Please confirm your account by clicking <a href=\"" + callbackUrl + "\">here</a>");

            return callbackUrl;
        }

        #region Helpers
        // Used for XSRF protection when adding external logins
        private const string XsrfKey = "XsrfId";

        private IAuthenticationManager AuthenticationManager
        {
            get
            {
                return HttpContext.GetOwinContext().Authentication;
            }
        }

        private void AddErrors(IdentityResult result)
        {
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError("", error);
            }
        }

        private ActionResult RedirectToLocal(string returnUrl)
        {
            if (Url.IsLocalUrl(returnUrl))
            {
                return Redirect(returnUrl);
            }
            return RedirectToAction("Index", "Home");
        }

        internal class ChallengeResult : HttpUnauthorizedResult
        {
            public ChallengeResult(string provider, string redirectUri)
                : this(provider, redirectUri, null)
            {
            }

            public ChallengeResult(string provider, string redirectUri, string userId)
            {
                LoginProvider = provider;
                RedirectUri = redirectUri;
                UserId = userId;
            }

            public string LoginProvider { get; set; }
            public string RedirectUri { get; set; }
            public string UserId { get; set; }

            public override void ExecuteResult(ControllerContext context)
            {
                var properties = new AuthenticationProperties { RedirectUri = RedirectUri };
                if (UserId != null)
                {
                    properties.Dictionary[XsrfKey] = UserId;
                }
                context.HttpContext.GetOwinContext().Authentication.Challenge(properties, LoginProvider);
            }
        }
        #endregion
    }
}