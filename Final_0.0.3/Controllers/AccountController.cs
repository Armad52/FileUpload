using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Final_0._0._3.Models;
using Final_0._0._3.ViewModel;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace Final_0._0._3.Controllers
{
    public class AccountController : Controller
    {
        [Obsolete]
        private IHostingEnvironment Environment;
        private readonly ILogger<AccountController> _logger;
        ApplicationContext _context;
        IWebHostEnvironment _appEnvironment;
        const string server = "https://localhost:44384/";
        private readonly UserManager<User> _userManager;
        private readonly SignInManager<User> _signInManager;

        [Obsolete]
        public AccountController(UserManager<User> userManager, SignInManager<User> signInManager, IHostingEnvironment _environment, ILogger<HomeController> logger, ApplicationContext context, IWebHostEnvironment appEnvironment)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            Environment = _environment;
            _context = context;
            _appEnvironment = appEnvironment;
            
        }

        
        [HttpPost]
        public async Task<IActionResult> AddFile(IFormFile uploadedFile)
        {
            if (uploadedFile != null)
            {
                var userId = this.User.FindFirstValue("Email");
                var uploadDirecotroy = @"files\";
                var uploadPath = Path.Combine(_appEnvironment.WebRootPath, uploadDirecotroy);

                if (!Directory.Exists(uploadPath))
                    Directory.CreateDirectory(uploadPath);

                var fileName = Guid.NewGuid() + Path.GetExtension(uploadedFile.FileName);
                var filePath = Path.Combine(uploadPath, fileName);


                // сохраняем файл в папку Files в каталоге wwwroot
                using (var fileStream = new FileStream(filePath, FileMode.Create))
                {

                    await uploadedFile.CopyToAsync(fileStream);
                }

                FileModel model = new FileModel();
                FileModel file = new FileModel
                {
                    Name = uploadedFile.FileName,
                    Path = filePath,
                    UserLogin = User.Identity.Name,
                    Date = DateTime.Now
                };
                _context.Files.Add(file);
                _context.SaveChanges();
                int id = _context.Files.First(p => p.Name == file.Name && p.Path == file.Path).Id;
                filePath = server + _context.Files.First(p => p.Id == id).Path;
                
            }

            return RedirectToAction("import");
        }

        [HttpGet]
        public ActionResult Delete(int Id)
        {
            var file = _context.Files.Where(p => p.Id == Id).FirstOrDefault();
            _context.Remove(file);
            _context.SaveChanges();
            return RedirectToAction("account");

        }
        [HttpGet]
        public FileResult DownloadFile(string Filename)
        {
            string file = Filename;
            byte[] data = System.IO.File.ReadAllBytes(file);
            return File(data, "text/plain"); // could have specified the downloaded file name again here
        }
        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            if (ModelState.IsValid)
            {
                User user = new User { Email = model.Email, UserName = model.Email};
                // добавляем пользователя
                var result = await _userManager.CreateAsync(user, model.Password);
                if (result.Succeeded)
                {
                    // установка куки
                    await _signInManager.SignInAsync(user, false);
                    return RedirectToAction("import", "Account");
                }
                else
                {
                    foreach (var error in result.Errors)
                    {
                        ModelState.AddModelError(string.Empty, error.Description);
                    }
                }
            }
            return View(model);
        }
        [HttpGet]
        public IActionResult Login(string returnUrl = null)
        {
            if (_signInManager.IsSignedIn(User))
            {
                return RedirectToAction("import", "Account");
                
            }
            else 
            {
                return View(new LoginViewModel { ReturnUrl = returnUrl });
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (ModelState.IsValid)
            {
                var result =
                    await _signInManager.PasswordSignInAsync(model.Email, model.Password, model.RememberMe, false);
                if (result.Succeeded)
                {
                    // проверяем, принадлежит ли URL приложению
                    if (!string.IsNullOrEmpty(model.ReturnUrl) && Url.IsLocalUrl(model.ReturnUrl))
                    {
                        return Redirect(model.ReturnUrl);
                    }
                    else
                    {
                        return RedirectToAction("import", "Account");
                    }
                }
                else
                {
                    ModelState.AddModelError("", "Неправильный логин и (или) пароль");
                }
            }
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout()
        {
            // удаляем аутентификационные куки
            await _signInManager.SignOutAsync();
            return RedirectToAction("Index", "Home");
        }
        public IActionResult test()
        {
            return View();
        }
        public IActionResult account()
        {
            var userId = User.Identity.Name;
            var UserFile = from m in _context.Files select m;

            if (!String.IsNullOrEmpty(userId))
            {
                UserFile = UserFile.Where(s => s.UserLogin.Contains(userId));
            }
            return View(UserFile.ToList());
        }
        public IActionResult fillups()
        {
            return View();
        }
        public IActionResult history()
        {
            var userId = User.Identity.Name;
            var UserFile = from m in _context.Files select m;

            if (!String.IsNullOrEmpty(userId))
            {
                UserFile = UserFile.Where(s => s.UserLogin.Contains(userId));
            }
            return View(UserFile.ToList());
        }
        public IActionResult import()
        {
            return View();
        }
        public IActionResult settings()
        {
            return View();
        }
    }
}
