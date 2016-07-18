/**************************************************************************
*
* NAME        : LoginController.cs
*
* VERSION     : 1.0.0
*
* DATE        : 17-Jan-2016
*
* DESCRIPTION : LoginController
*
* MODIFICATION HISTORY
* Name             Date         Description
* ===============  ===========  =======================================
* Wells Cheung     17-Jan-2016  Initial Version
*
**************************************************************************/
using CoolPrivilegeControlVM.CommonVM;
using CoolPrivilegeControlVM.EntityVM;
using CoolPrivilegeControlVM.SessionMaintenance;
using CoolUtilities.MultiLingual;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.ComponentModel.Composition.Hosting;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Web.Mvc;
using System.Web.Script.Serialization;
using UI_Infrastructure;
using UI_Infrastructure.ComController;
using UI_Infrastructure.CommonFilter.AuthorizationFilter;
using UI_Infrastructure.CommonFilter.TracerActionFilter;
using UI_Infrastructure.ViewModels;
using System.ServiceModel;
using CoolPrivilegeControlVM.WCFVM;
using CoolPrivilegeControlVM.WEBVM;
using CoolPrivilegeControl.Models;
using UI_Infrastructure.CustomHtmlHelper;
using CoolPrivilegeControlVM.WCFVM.LoginUserSerVM;
using CoolPrivilegeControlSerClient;


namespace CoolPrivilegeControl.Areas.AccessControl.Controllers
{
    [PartCreationPolicy(System.ComponentModel.Composition.CreationPolicy.NonShared)]
    [Export(typeof(IController))]
    [ExportMetadata("ControllerName", "Login")]
    [ExportMetadata("Order", 1)]
    public class LoginController : CommonController, IController
    {
        [Import(typeof(LoginUserMgtHelper))]
        public Lazy<LoginUserMgtHelper> loginUserMgtHelper
        { get; set; }

        [Import(typeof(AuthHisMgtHelper))]
        public Lazy<AuthHisMgtHelper> authHisMgtHelper
        { get; set; }


        [ImportingConstructor]
        public LoginController(CompositionContainer container)
        {
            this.postOffice = new PostOffice();
        }

        public void initLanguageComboBox(string str_Language = "en")
        {
            List<SelectListItem> _list = new List<SelectListItem>();
            _list.Add(new SelectListItem()
            {
                Value = "",
                Text = MultilingualHelper.GetStringFromResource("PleaseSelect")
            });
            _list.Add(new SelectListItem()
            {
                Value = "en",
                Text = "English"
            });
            _list.Add(new SelectListItem()
            {
                Value = "cn",
                Text = "简体中文"
            });
            _list.Add(new SelectListItem()
            {
                Value = "tw",
                Text = "繁体中文"
            });
            _list.Add(new SelectListItem()
            {
                Value = "esve",
                Text = "Español - Venezuela"
            });

            ViewBag.Langs = new SelectList((IEnumerable<SelectListItem>)_list, "Value", "Text", str_Language);
        }

        [UnAuthorization]
        // GET: AccessControl/Login
        public ActionResult Index(string str_Language = "en")
        {
            ClearSelectionCriteriaFromViewData();

            LanguageKey temp = LanguageKey.en;

            Enum.TryParse<LanguageKey>(str_Language, out temp);

            TempData[StaticContent.LanguageKey] = temp;

            Thread.CurrentThread.CurrentCulture =
                Thread.CurrentThread.CurrentUICulture = new System.Globalization.CultureInfo(LangPack.GetLanugage(temp));

            if (TempData[ActionMessageKey] != null)
            {
                ViewBag.ActionMessage = TempData[ActionMessageKey];
            }

            initLanguageComboBox(str_Language);

            return View();
        }

        [UnAuthorization]
        // GET: AccessControl/Login/Reset
        public ActionResult Reset()
        {

            if (UserGuid.HasValue)
            {
                return View();
            }
            else
            {
                string str_MsgBoxTitle = MultilingualHelper.GetStringFromResource("LoginScreentTitle");

                string str_E011 = MultilingualHelper.GetStringFromResource("E011");

                MsgInfo errorMsgInfo = new MsgInfo();
                errorMsgInfo.MsgTitle = str_MsgBoxTitle;
                errorMsgInfo.MsgDesc = str_E011;
                errorMsgInfo.MsgType = MessageType.ValidationError;
                ViewBag.ActionMessage = errorMsgInfo;
                TempData[ActionMessageKey] = errorMsgInfo;
                return View("Index");
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [UnAuthorization]
        public ActionResult Reset(FormCollection collection)
        {
            string str_Error = "";
            string str_MsgBoxTitle = MultilingualHelper.GetStringFromResource("Login_Reset");

            List<string> strList_Error = new List<string>();

            if (UserGuid.HasValue)
            {
                LoginUserVM entity_LUVM = new LoginUserVM();
                entity_LUVM.ID = this.UserGuid.Value;

                WebCommonHelper webCommonHelper = new WebCommonHelper();

                LUSerEditResult entity_LUSerEditResult = new LUSerEditResult();

                webCommonHelper.CallWCFHelper(this, (entity_WCFAuthInfoVM) =>
                {
                    entity_LUSerEditResult = loginUserMgtHelper.Value.GetEntityByID(entity_WCFAuthInfoVM, entity_LUVM.ID.ToString());
                });

                if (entity_LUSerEditResult.StrList_Error.Count > 0)
                {
                    foreach (var item in entity_LUSerEditResult.StrList_Error)
                    {
                        strList_Error.Add(item);
                    }
                }

                if (strList_Error.Count > 0)
                {
                    str_Error = string.Join("<br/>", strList_Error.ToArray());
                }
                else
                {
                    entity_LUVM = entity_LUSerEditResult.Entity_LoginUserVM;

                    typeof(LoginUserVM).GetProperties(BindingFlags.SetProperty | BindingFlags.Public | BindingFlags.Instance).ToList().ForEach(current =>
                    {
                        if (collection.AllKeys.Contains(current.Name))
                        {
                            current.SetValue(entity_LUVM, collection[current.Name], null);
                        }
                    });

                    ClientSessionInfo entity_ClientSessionInfo = null;
                    if (ViewData.ContainsKey(Bootstrapper.UserClientSessionKey.ToString()))
                    {
                        entity_ClientSessionInfo = (ClientSessionInfo)ViewData[Bootstrapper.UserClientSessionKey.ToString()];
                    }

                    WCFReturnResult entity_Return = new WCFReturnResult();

                    //webCommonHelper.CallWCFHelper<ILoginUserMgtSer>(this, this.HttpContext, postOffice.LoginUserMgtSerPath, (entity_ILoginUserMgtSer, entity_WCFAuthInfoVM) =>
                    //{
                    //    entity_Return = entity_ILoginUserMgtSer.ResetPwd(entity_WCFAuthInfoVM, entity_LUVM);
                    //});

                    webCommonHelper.CallWCFHelper(this, (entity_WCFAuthInfoVM) =>
                    {
                        entity_Return = loginUserMgtHelper.Value.ResetPwd(entity_WCFAuthInfoVM, entity_LUVM);
                    });

                    if (entity_Return.StrList_Error.Count > 0)
                    {
                        foreach (var item in entity_Return.StrList_Error)
                        {
                            strList_Error.Add(item);
                        }
                        str_Error = string.Join("<br/>", entity_Return.StrList_Error.ToArray());
                    }
                    else
                        if (entity_Return.IsSuccess)
                        {
                            return Redirect("/AccessControl/FManage");
                        }
                }
            }

            string str_E011 = MultilingualHelper.GetStringFromResource("E011");

            MsgInfo errorMsgInfo = new MsgInfo();
            errorMsgInfo.MsgTitle = str_MsgBoxTitle;
            errorMsgInfo.MsgDesc = str_E011;
            errorMsgInfo.MsgType = MessageType.ValidationError;
            ViewBag.ActionMessage = errorMsgInfo;
            TempData[ActionMessageKey] = errorMsgInfo;
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [UnAuthorization]
        public ActionResult Index(LoginUserVM loginUserVM)
        {
            //Message Box Title -- When Error occured, Message Box would be showed.
            string str_MsgBoxTitle = MultilingualHelper.GetStringFromResource("LoginScreentTitle");

            ClearSelectionCriteriaFromViewData();

            #region [ Add data into language combo box ]
            if (TempData.Keys.Contains(StaticContent.LanguageKey))
            {
                LanguageKey temp = (LanguageKey)(TempData[StaticContent.LanguageKey]);

                initLanguageComboBox(temp.ToString());
            }
            else
            {
                initLanguageComboBox();
            }
            #endregion

            LUSerLoginResult entity_LUSerLoginResult = new LUSerLoginResult();
            //Define and Create channel factory in order to call the service
            entity_LUSerLoginResult = loginUserMgtHelper.Value.Login(loginUserVM, TempData[StaticContent.LanguageKey].ToString(), UserHostAddress, UserHostName);

            if (entity_LUSerLoginResult != null && entity_LUSerLoginResult.StrList_Error.Count == 0)
            {
                //Clear the cache.
                TempData[ActionMessageKey] = ViewBag.ActionMessage = null;

                //Save login user's authorized info to the cache.
                MVCSessionMgt.SaveServerSideSession(entity_LUSerLoginResult.Entity_SessionWUserInfo, StaticContent.SystemInfoInst.GetSessionTimeOutSeconds());

                //Save service authorized key and client side session key.
                ClientSessionInfo entity_ClientSessionInfo = new ClientSessionInfo();
                if (entity_LUSerLoginResult.Entity_SessionWUserInfo != null)
                {
                    entity_ClientSessionInfo.MVCSessionKey = entity_LUSerLoginResult.Entity_SessionWUserInfo.SessionKey;
                    entity_ClientSessionInfo.ServiceAuthorizedKey = entity_LUSerLoginResult.Str_ServerToken;

                    Entity_ClientSessionInfo = entity_ClientSessionInfo;

                    TempData[Bootstrapper.UserClientSessionKey.ToString()] =
                    ViewData[Bootstrapper.UserClientSessionKey.ToString()] = entity_ClientSessionInfo;
                }

                //Check the password is expire or not.
                if (entity_LUSerLoginResult.IsPWDExpire)
                {
                    return Redirect("/AccessControl/Login/Reset");
                }
                else
                {
                    return Redirect("/Home/Index");
                }
            }

            //Output error. 
            MsgInfo errorMsgInfo = new MsgInfo();
            errorMsgInfo.MsgTitle = str_MsgBoxTitle;
            //Retrieve all error message.
            errorMsgInfo.MsgDesc = string.Join("<br/>", entity_LUSerLoginResult.StrList_Error.ToArray());
            errorMsgInfo.MsgType = MessageType.ValidationError;
            ViewBag.ActionMessage = errorMsgInfo;
            TempData[ActionMessageKey] = errorMsgInfo;
            return View(loginUserVM);
        }

        [HttpPost]
        [UnAuthorization]
        [UnTracerAction]
        public ActionResult GetCurrentDatetimeAndTotalCount()
        {
            JavaScriptSerializer javaScriptSerializer = new JavaScriptSerializer();
            CurrentInfoVM entity_CurrentInfoVM = new CurrentInfoVM();
            SystemInfoVM temp_SystemInfoVM = (SystemInfoVM)StaticContent.SystemInfoInst;
            entity_CurrentInfoVM.CurrDateTime = DateTime.Now.ToString(temp_SystemInfoVM.DateFormat + " " + temp_SystemInfoVM.TimeFormat);

            //int int_Counter = authorizedHistoryRespo.GetTotalAuthorizationCount(OperationType.L);

            WebCommonHelper webCommonHelper = new WebCommonHelper();

            webCommonHelper.CallWCFHelper(this, (entity_WCFAuthInfoVM) =>
            {
                int int_Counter = authHisMgtHelper.Value.GetTotalAuthorizationCount(entity_WCFAuthInfoVM);

                entity_CurrentInfoVM.TotalLoginCount = int_Counter.ToString();
            });

            return Json(javaScriptSerializer.Serialize(entity_CurrentInfoVM));
        }

        [UnAuthorization]
        public ActionResult Logout()
        {
            #region [ Add data into language combo box ]
            if (TempData.Keys.Contains(StaticContent.LanguageKey))
            {
                LanguageKey temp = (LanguageKey)(TempData[StaticContent.LanguageKey]);

                initLanguageComboBox(temp.ToString());
            }
            else
            {
                initLanguageComboBox();
            }
            #endregion

            string str_IpAddr = this.HttpContext.Request.UserHostAddress;
            string str_HostName = this.HttpContext.Request.UserHostName;

            string str_MsgBoxTitle = MultilingualHelper.GetStringFromResource("LoginScreentTitle");

            string strError = MultilingualHelper.GetStringFromResource("I003");

            string str_SaveAuthorizedHistory_Error = "";

            if (ViewData.ContainsKey(Bootstrapper.UserClientSessionKey.ToString()))
            {
                ClientSessionInfo entity_ClientSessionInfo = null;
                if (ViewData.ContainsKey(Bootstrapper.UserClientSessionKey.ToString()))
                {
                    entity_ClientSessionInfo = (ClientSessionInfo)ViewData[Bootstrapper.UserClientSessionKey.ToString()];
                }

                if (entity_ClientSessionInfo != null)
                {
                    BaseSession entity_BaseSession = entity_SessionWUserInfo;

                    if (entity_BaseSession != null)
                    {
                        WebCommonHelper webCommonHelper = new WebCommonHelper();

                        webCommonHelper.CallWCFHelper(this, (entity_WCFAuthInfoVM) =>
                        {
                            WCFReturnResult entity = loginUserMgtHelper.Value.Logout(entity_WCFAuthInfoVM);

                            if (!entity.IsSuccess)
                            {
                                str_SaveAuthorizedHistory_Error = string.Join("<br/>", entity.StrList_Error.ToArray());
                            }
                            else
                            {
                                MVCSessionMgt.RemoveServerSideSession(entity_BaseSession);
                            }
                        });
                    }

                    if (ViewData.ContainsKey(Bootstrapper.UserClientSessionKey.ToString()))
                    {
                        ViewData.Remove(Bootstrapper.UserClientSessionKey.ToString());
                    }

                    if (TempData.ContainsKey(Bootstrapper.UserClientSessionKey.ToString()))
                    {
                        TempData.Remove(Bootstrapper.UserClientSessionKey.ToString());
                    }
                }
            }

            if (!string.IsNullOrWhiteSpace(str_SaveAuthorizedHistory_Error))
            {
                MsgInfo errorMsgInfo = new MsgInfo();
                errorMsgInfo.MsgTitle = str_MsgBoxTitle;
                errorMsgInfo.MsgDesc = str_SaveAuthorizedHistory_Error;
                errorMsgInfo.MsgType = MessageType.ValidationError;
                ViewBag.ActionMessage = errorMsgInfo;
                TempData[ActionMessageKey] = errorMsgInfo;
            }
            else
            {
                MsgInfo errorMsgInfo = new MsgInfo();
                errorMsgInfo.MsgTitle = str_MsgBoxTitle;
                errorMsgInfo.MsgDesc = strError;
                errorMsgInfo.MsgType = MessageType.Success;
                ViewBag.ActionMessage = errorMsgInfo;
                TempData[ActionMessageKey] = errorMsgInfo;
            }
            return View("Index");
        }
    }
}
