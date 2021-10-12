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
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Configuration;
using Final_0._0._3.ViewModel;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Final_0._0._3.Controllers
{
    
    [Route("api/[controller]")]
    [ApiController]
    public class DownloadController : ControllerBase
    {
        private readonly UserManager<User> _userManager;
        private readonly SignInManager<User> _signInManager;
        private readonly IConfiguration _configuration;
        ApplicationContext _context;
        public DownloadController(UserManager<User> userManager, SignInManager<User> signInManager, ApplicationContext context)
        {
            _context = context;
            _userManager = userManager;
            _signInManager = signInManager;
        }
        [HttpGet]
        [Route("filedownload")]
        public async Task<IActionResult> Download([FromBody] FileModel model)
        {
            var model2 = _context.Files.Where(p => p.UserLogin == model.UserLogin && p.Id==model.Id).FirstOrDefault();
            
            if (model2 != null)
            {
                var userId = model.Id.ToString();
                var UserFile = from m in _context.Files select m;

                if (!String.IsNullOrEmpty(userId.ToString()))
                {
                    UserFile = UserFile.Where(s => s.UserLogin.Contains(userId));
                }

                var memory = new MemoryStream();
                using (FileStream stream = new FileStream(model2.Path, FileMode.Open))
                {
                    await stream.CopyToAsync(memory);
                }
                memory.Position = 0;
                var exp = Path.GetExtension(model2.Path).ToLowerInvariant();
                return File(memory, GetMimeTypes()[exp], Path.GetFileName(model2.Name));

            }

            return NotFound();
        }
        
        private Dictionary<string, string> GetMimeTypes()
        {
            return new Dictionary<string, string>
            {
                {".txt","text/plan" },
                {".pdf","application/pdf" },
                {".doc","application/vnd.ms-word" },
                {".docx","application/vnd.ms-word"},
                {".xls","application/vnd.ms-excel" },
                {".xlsx","application/vnd.ms-excel" },
                {".png","image/png" },
                {".jpg","image/jpeg" },
                {".jpeg","image/jpeg" },
                {".csv","text/csv" }
            };
        }
       
        [HttpPost]
        [Route("Auth")]
        public async Task<IActionResult> AuthAsync([FromBody] LoginViewModel model)
        {
            var userId = model.Email;
            var UserFile = from m in _context.Files select m;

            if (!String.IsNullOrEmpty(userId))
            {
                UserFile = UserFile.Where(s => s.UserLogin.Contains(userId));
            }
            

                var result =
                    await _signInManager.PasswordSignInAsync(model.Email, 
                    model.Password, model.RememberMe, false);
                if (result.Succeeded)
                {
                    return Ok(UserFile.ToList());
                }
                
                return Unauthorized();
            
            
        }
    }

}
