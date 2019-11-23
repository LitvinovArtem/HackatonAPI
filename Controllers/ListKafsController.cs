using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using WebDekAPI.Auth;
using WebDekAPI.Hubs;
using WebDekAPI.Share;

namespace WebDekAPI.Controllers
{
    [Produces("application/json")]
    [Route("api/ListKafs")]
    public class ListKafsController : Controller
    {
        private DataBaseContext db;

		public ListKafsController(DataBaseContext context)
        {
			this.hubContext = hubContext;
		}

        [HttpGet]
        public IActionResult Get(int? userID)
        {
            
            return Json(new RespondView(
                    new
                    {
                        dbName = db.Database.GetDbConnection().Database,
                        listKafs = "",
                        defaultKaf
                    },

                    RequestState.Success, "Список кафедр"));
        }
    }
}