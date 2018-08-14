using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using USER_DETAIL;
using DataAccessLayer;
using Mywebsite.Models;

namespace Mywebsite.Controllers
{
    public class LogInController : Controller
    {
        // GET: LogIn
        public ActionResult LogIn()
        {
            USER_MASTER objUSER_MASTER = new USER_MASTER();
            return View(objUSER_MASTER);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult LoginSubmit(USER_MASTER objUSER_MASTER)
        {
            USER_MASTERs objUSER_MASTERs = new USER_MASTERs();

            string str = objUSER_MASTER.EMAIL_ID;
            string email_id = "";
            string mobile_no = "";
            string decryptPass = "";
            String Msg = "";
            try
            {
                if (string.IsNullOrEmpty(objUSER_MASTER.EMAIL_ID))
                    Msg = "Please enter EmailId/Mobile number";
                if (string.IsNullOrEmpty(objUSER_MASTER.PASSWORD))
                    Msg = "Please Enter the password";
                if (Msg == "")
                {
                    if (str.Contains("@"))
                        email_id = objUSER_MASTER.EMAIL_ID;
                    else
                        mobile_no = objUSER_MASTER.EMAIL_ID;

                    objUSER_MASTERs.LogInUser(email_id, mobile_no, "");
                    decryptPass = CryptoEngine.Decrypt(objUSER_MASTERs[0].PASSWORD, "sblw-3hn8-sqoy19");

                    if (objUSER_MASTERs.Count > 0)
                    {
                        if (objUSER_MASTERs[0].ACTIVE == "Y" && objUSER_MASTER.PASSWORD == decryptPass)
                        {
                            return View("~/Views/Home/Index.cshtml");
                        }
                    }

                    else
                        ViewBag.Message = "Invalid UserId/Password";
                }
                else
                    ViewBag.Message = Msg;
            }
            catch (Exception ex)
            {
                ViewBag.Message = ex.Message;
            }
            return View("~/Views/LogIn/LogIn.cshtml", objUSER_MASTER);
        }

        public ActionResult SignUp()
        {
            USER_MASTER objUSER_MASTER = new USER_MASTER();
            return View(objUSER_MASTER);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult SignUpSubmit(USER_MASTER objUSER_MASTER)
        {
            string Msg = "";
            try
            {
                if (objUSER_MASTER.PASSWORD != objUSER_MASTER.CONFIRM_PASSWORD)
                {
                    Msg = "Password is not matched";
                }
                else
                {
                    string pass = "";
                    pass = CryptoEngine.Encrypt(objUSER_MASTER.PASSWORD, "sblw-3hn8-sqoy19");
                    objUSER_MASTER.PASSWORD = pass;
                    Msg = objUSER_MASTER.SaveData();
                }
            }
            catch (Exception ex)
            {
                ViewBag.Message = ex.Message;
            }
            ViewBag.Message = Msg;
            return View("~/Views/LogIn/SignUp.cshtml", objUSER_MASTER);
        }
    }
}