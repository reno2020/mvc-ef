using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.UI;
using WikEasyUIMvc;

namespace WikEasyUIMvc.Controllers
{ 
    public class UserController : Controller
    {
        private WikTestEntities db = new WikTestEntities();

        ///参照博客园一哥们进行了界面更换及代码改进
        // GET: /User/

        public ViewResult Index()
        {
            return View();
        }

        //
        // GET: /User/Create

        public ActionResult Create()
        {
            return View();
        } 

        //
        // POST: /User/Create

        [HttpPost]
        public JsonResult Create(TUser tuser)
        {
            JsonResult json=new JsonResult();
			json.Data=true;
            try{
                db.TUser.AddObject(tuser);
                db.SaveChanges();
            }
            catch(Exception ee)
            {
                json.Data=ee.Message;
            }
            
            return json;
        }
        
        //编辑的 jhl
        // GET: /User/Edit/5
        [OutputCache(Location = OutputCacheLocation.None)]
        public ActionResult Edit(int id)
        {
            TUser tuser = db.TUser.Single(t => t.ID == id);
            return View(tuser);
        }

        //
        // POST: /User/Edit/5

        [HttpPost]
        [OutputCache(Location = OutputCacheLocation.None)]
        public ActionResult Edit(TUser tuser)
        {
            JsonResult json=new JsonResult();
            try
            {
                db.TUser.Attach(tuser);
                db.ObjectStateManager.ChangeObjectState(tuser, EntityState.Modified);
                db.SaveChanges();
            }
            catch(Exception ee)
            {
                json.Data=ee.Message;
                return json;
            }
            json.Data=true;
            return json;
        }


        //
        // POST: /User/Delete/5

        [HttpPost, ActionName("Delete")]
        public JsonResult DeleteConfirmed(string id)
        {
            string[] tid = id.Split(',');

            JsonResult json = new JsonResult();
            json.Data = true;
            for (int i = 0; i < tid.Length; i++)
            {
                try
                {
                    string aa = tid[i];
                    int idi = Convert.ToInt32(aa);
                    TUser tuser = db.TUser.Single(t => t.ID == idi);
                    db.TUser.DeleteObject(tuser);
                    db.SaveChanges();
                }
                catch (Exception ee)
                {
                    //json.Data = ee.Message;
                }
            }
            return json;
        }

        public JsonResult List(int page,int rows)
        {

            string strWhere = "";
            string searchType = Request.Form["search_type"] != "" ? Request.Form["search_type"] : string.Empty;
            string searchValue = Request.Form["search_value"] != "" ? Request.Form["search_value"] : string.Empty;

            if (searchType != null && searchValue != null)
            {
                var q = from e in db.TUser
                        where e.UserName.Contains(searchValue)
                        orderby e.ID
                        select new
                        {
                            ID = e.ID,
                            UserName = e.UserName,
                            RealName = e.RealName,
                            UserPwd = e.UserPwd,
                            Role = e.Role,
                            IsAdmin = e.IsAdmin,
                            Email = e.Email
                        };
                var result = q.Skip((page - 1) * rows).Take(rows).ToList();
                Dictionary<string, object> json = new Dictionary<string, object>();
                json.Add("total", q.ToList().Count);
                json.Add("rows", result);
                return Json(json, JsonRequestBehavior.AllowGet);
            }
            else
            {
                var q = from e in db.TUser
                        orderby e.ID
                        select new
                        {
                            ID = e.ID,
                            UserName = e.UserName,
                            RealName = e.RealName,
                            UserPwd = e.UserPwd,
                            Role = e.Role,
                            IsAdmin = e.IsAdmin,
                            Email = e.Email
                        };
                var result = q.Skip((page - 1) * rows).Take(rows).ToList();
                Dictionary<string, object> json = new Dictionary<string, object>();
                json.Add("total", q.ToList().Count);
                json.Add("rows", result);
                return Json(json, JsonRequestBehavior.AllowGet);
            }
					
			
        }
		
		protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}
