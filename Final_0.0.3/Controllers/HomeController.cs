using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Final_0._0._3.Models;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using System.IO;

using System.Drawing;
using System.Drawing.Imaging;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using System.Security.Claims;

namespace Final_0._0._3.Controllers
{
    public class HomeController : Controller
    {
        [Obsolete]
        private IHostingEnvironment Environment;
        private readonly ILogger<HomeController> _logger;
        ApplicationContext _context;
        IWebHostEnvironment _appEnvironment;
        const string server = "https://localhost:44384/";

        [Obsolete]
        public HomeController(IHostingEnvironment _environment,ILogger<HomeController> logger, ApplicationContext context, IWebHostEnvironment appEnvironment)
        {
            Environment = _environment;
            _context = context;
            _appEnvironment = appEnvironment;
            _logger = logger;
        }

        public IActionResult Index()
        {
            return View();
        }
        
        
        public IActionResult File2()
        {
            
           var userId = User.Identity.Name;
           var UserFile = from m in _context.Files select m;

            if (!String.IsNullOrEmpty(userId))
            {
                UserFile = UserFile.Where(s => s.UserLogin.Contains(userId));
            }
            return View(UserFile.ToList());
        }
        

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
