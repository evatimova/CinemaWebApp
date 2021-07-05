using CinemaWeb.Domain.DomainModels;
using CinemaWeb.Domain.DTO;
using CinemaWeb.Domain.Identity;
using ExcelDataReader;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace CinemaWeb.Web.Controllers
{
    public class AccountController : Controller
    {
        private readonly UserManager<CinemaWebApplicationUser> _userManager;
        private readonly SignInManager<CinemaWebApplicationUser> _signInManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        public AccountController(UserManager<CinemaWebApplicationUser> userManager, SignInManager<CinemaWebApplicationUser> signInManager, RoleManager<IdentityRole> roleManager)
        {
            this._userManager = userManager;
            this._signInManager = signInManager;
            this._roleManager = roleManager;
        }
        //[Authorize(Roles = "Admin")]
        public IActionResult Index()
        {
            return View();
        }

        //[Authorize(Roles = "Admin")]
        public IActionResult ImportUsers(IFormFile file)
        {
            //make a copy
            string pathToUpload = $"{Directory.GetCurrentDirectory()}\\files\\{file.FileName}";

            using (FileStream fileStream = System.IO.File.Create(pathToUpload))
            {
                file.CopyTo(fileStream);

                fileStream.Flush();
            }

            //read data from copy file

            List<UserRegisterDto> users = getAllUsersFromFile(file.FileName);

            HttpClient client = new HttpClient();

            string URL = "https://localhost:44306/api/Admin/ImportAllUsers";

            HttpContent content = new StringContent(JsonConvert.SerializeObject(users), Encoding.UTF8, "application/json");

            HttpResponseMessage response = client.PostAsync(URL, content).Result;

            var data = response.Content.ReadAsAsync<bool>().Result;

            return RedirectToAction("Index", "Order");
        }

        //[Authorize(Roles = "Admin")]
        private List<UserRegisterDto> getAllUsersFromFile(string fileName)
        {
            List<UserRegisterDto> users = new List<UserRegisterDto>();

            string filePath = $"{Directory.GetCurrentDirectory()}\\files\\{fileName}";

            System.Text.Encoding.RegisterProvider(System.Text.CodePagesEncodingProvider.Instance);

            using (var stream = System.IO.File.Open(filePath, FileMode.Open, FileAccess.Read))
            {
                using (var reader = ExcelReaderFactory.CreateReader(stream))
                {
                    while (reader.Read())
                    {
                        users.Add(new UserRegisterDto
                        {
                            Email = reader.GetValue(0).ToString(),
                            Password = reader.GetValue(1).ToString(),
                            ConfirmPassword = reader.GetValue(2).ToString(),
                            Role = reader.GetValue(3).ToString()
                        });
                    }
                }
            }

            return users;
        }

        //[Authorize(Roles = "Admin")]
        [HttpPost("[action]")]
        public bool ImportAllUsers(List<UserRegisterDto> model)
        {
            bool status = true;

            foreach (var item in model)
            {
                var userCheck = _userManager.FindByEmailAsync(item.Email).Result;
                if (userCheck == null)
                {
                    var user = new CinemaWebApplicationUser
                    {
                        UserName = item.Email,
                        NormalizedUserName = item.Email,
                        Email = item.Email,
                        EmailConfirmed = true,
                        PhoneNumberConfirmed = true,
                        UserCart = new ShoppingCart()
                    };
                    var result = _userManager.CreateAsync(user, item.Password).Result;
                    var roleAdded = _userManager.AddToRoleAsync(user, item.Role).Result;
                    status = status && result.Succeeded;
                }
                else
                {
                    continue;
                }
            }

            return status;
        }

        [HttpGet]
        public IActionResult Register()
        {
            UserRegisterDto model = new UserRegisterDto();
            return View(model);
        }

        [HttpPost, AllowAnonymous]
        public async Task<IActionResult> Register(UserRegisterDto request)
        {
            if (ModelState.IsValid)
            {
                var userCheck = await _userManager.FindByEmailAsync(request.Email);
                if (userCheck == null)
                {
                    var user = new CinemaWebApplicationUser
                    {
                        UserName = request.Email,
                        NormalizedUserName = request.Email,
                        Email = request.Email,
                        EmailConfirmed = true,
                        PhoneNumberConfirmed = true,
                        UserCart = new ShoppingCart()
                    };
                    var result = await _userManager.CreateAsync(user, request.Password);
                    if (result.Succeeded)
                    {
                        await _userManager.AddToRoleAsync(user, "User");
                        return RedirectToAction("Login");
                    }
                    else
                    {
                        if (result.Errors.Count() > 0)
                        {
                            foreach (var error in result.Errors)
                            {
                                ModelState.AddModelError("message", error.Description);
                            }
                        }
                        return View(request);
                    }
                }
                else
                {
                    ModelState.AddModelError("message", "Email already exists.");
                    return View(request);
                }
            }
            return View(request);
        }

        [HttpGet]
        [AllowAnonymous]
        public IActionResult Login()
        {
            UserLoginDto model = new UserLoginDto();
            return View(model);
        }

        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> Login(UserLoginDto model)
        {
            if (ModelState.IsValid)
            {
                var user = await _userManager.FindByEmailAsync(model.Email);
                if (user != null && !user.EmailConfirmed)
                {
                    ModelState.AddModelError("message", "Email not confirmed yet");
                    return View(model);

                }
                if (await _userManager.CheckPasswordAsync(user, model.Password) == false)
                {
                    ModelState.AddModelError("message", "Invalid credentials");
                    return View(model);

                }

                var result = await _signInManager.PasswordSignInAsync(model.Email, model.Password, model.RememberMe, true);

                if (result.Succeeded)
                { 
                    await _userManager.AddClaimAsync(user, new Claim("UserRole", "User"));
                    return RedirectToAction("Index", "Home");
                }
                else if (result.IsLockedOut)
                {
                    return View("AccountLocked");
                }
                else
                {
                    ModelState.AddModelError("message", "Invalid login attempt");
                    return View(model);
                }
            }
            return View(model);
        }

        [HttpGet]
        //[Authorize(Roles = "Admin")]
        public IActionResult CreateRole()
        {
            return View();
        }

        [HttpPost]
        //[Authorize(Roles = "Admin")]
        public async Task<IActionResult> CreateRole(CreateRoleDto model)
        {
            if (ModelState.IsValid)
            {
                IdentityRole identityRole = new IdentityRole
                {
                    Name = model.RoleName
                };

                IdentityResult result = await _roleManager.CreateAsync(identityRole);

                if (result.Succeeded)
                {
                    return RedirectToAction("Index", "Home");
                }

                foreach (IdentityError error in result.Errors)
                {
                    ModelState.AddModelError("", error.Description);
                }
            }

            return View(model);
        }

        [HttpGet]
        //[Authorize(Roles = "Admin")]
        public IActionResult AddToRole()
        {
            var model = new AddToRoleDto();
            model.Roles.Add("Admin");
            model.Roles.Add("User");
            return View(model);
        }

        [HttpPost]
        //[Authorize(Roles = "Admin")]
        public async Task<IActionResult> AddToRole(AddToRoleDto model)
        {
            try
            {
                var user = await _userManager.FindByEmailAsync(model.Email);
                await _userManager.AddToRoleAsync(user, model.SelectedRole);
                return RedirectToAction("Index", "Home");
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        [HttpGet]
        //[Authorize(Roles = "Admin")]
        public IActionResult RemoveFromRole()
        {
            var model = new RemoveFromRoleDto();
            model.Roles.Add("Admin");
            model.Roles.Add("User");
            return View(model);
        }

        [HttpPost]
        //[Authorize(Roles = "Admin")]
        public async Task<IActionResult> RemoveFromRole(RemoveFromRoleDto model)
        {
            try
            {
                var user = await _userManager.FindByEmailAsync(model.Email);
                await _userManager.RemoveFromRoleAsync(user, model.SelectedRole);
                return RedirectToAction("Index", "Home");
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        [HttpPost]
        public async Task<IActionResult> Logout(UserRegisterDto user)
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction("Login", "Account");
        }
    }
}
