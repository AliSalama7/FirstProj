using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Movies.Domain.Interfaces;
using Movies.Domain.Models;
using MoviesApp.Models;
using MoviesApp.Models.IdentityViewModels;
using System.Data;


namespace MoviesApp.Controllers
{
    [Authorize(Roles = "Moderator")]
    public class UsersController : Controller
    {
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly UserManager<ApplicationUser> _userManager;

        public UsersController( UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            _userManager = userManager;
            _roleManager = roleManager;
        }
        public async Task<IActionResult> Index()
        {
            var users = await _userManager.Users.Select(user => new UserViewModel
            {
                Id = user.Id,
                FirstName = user.FirstName,
                LastName = user.LastName,
                UserName = user.UserName,
                Email = user.Email,
                Roles = null
            }).ToListAsync();
            foreach (var user in users)
            {
                user.Roles = await _userManager.GetRolesAsync(await _userManager.FindByIdAsync(user.Id));
            }
            return View(users);
        }
        public async Task<IActionResult> Add()
        {
            var roles = await _roleManager.Roles.Select(r => new CheckBoxViewModel
            {
                Name = r.Name
            }).ToListAsync();

            var viewModel = new UserFormViewModel
            {
                Roles = roles
            };
            return View(viewModel);
        }
        [HttpPost]
        [AutoValidateAntiforgeryToken]
        public async Task<IActionResult> Add(UserFormViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);
            if (!model.Roles.Any(r => r.IsChecked))
            {
                ModelState.AddModelError("Roles", "Please Select At Least One Role");
                return View(model);
            }
            if (await _userManager.FindByEmailAsync(model.Email) != null)
            {
                ModelState.AddModelError("Email", "Emai Is Already Exists");
                return View(model);
            }
            if (await _userManager.FindByNameAsync(model.UserName) != null)
            {
                ModelState.AddModelError("UserName", "UserName Is Already Exists");
                return View(model);
            }
            var user = new ApplicationUser()
            {
                FirstName = model.FirstName,
                LastName = model.LastName,
                Email = model.Email,
                UserName = new System.Net.Mail.MailAddress(model.Email).User
            };
            var result = await _userManager.CreateAsync(user, model.Password);
            if (!result.Succeeded)
            {
                foreach (var errror in result.Errors)
                    ModelState.AddModelError("Roles", errror.Description);
                return View(model);
            }
            await _userManager.AddToRolesAsync(user, model.Roles.Where(r => r.IsChecked).Select(r => r.Name));
            return RedirectToAction(nameof(Index));
        }
        public async Task<IActionResult> Edit(string userid)
        {
            var user = await _userManager.FindByIdAsync(userid);

            if (user == null)
                return NotFound();
            var viewModel = new UserEditFormViewModel
            {
                Id = user.Id,
                FirstName = user.FirstName,
                LastName = user.LastName,
                UserName = user.UserName,
                Email = user.Email
             };
           
            return View(viewModel);
        }

        [HttpPost]
        [AutoValidateAntiforgeryToken]
        public async Task<IActionResult> Edit(UserEditFormViewModel model)
        {
            if(!ModelState.IsValid)
                return View(model);

            var user = await _userManager.FindByIdAsync (model.Id);

            if(user == null)
                return NotFound();
            var userwithsameemail = await _userManager.FindByEmailAsync(model.Email);
            if (userwithsameemail != null && userwithsameemail.Id != model.Id)
            {
                ModelState.AddModelError("Email", "This Email Is Assigned To Another User");
                return View(model);
            }
            var userwithsameusername = await _userManager.FindByNameAsync(model.UserName);
            if (userwithsameusername != null && userwithsameusername.Id != model.Id)
            {
                ModelState.AddModelError("Email", "This UserNamel Is Assigned To Another User");
                return View(model);
            }
           
            user.FirstName = model.FirstName;
            user.LastName = model.LastName;
            user.Email = model.Email;
            user.UserName = model.UserName;

            await _userManager.UpdateAsync(user);
            return RedirectToAction(nameof(Index));
        }
        public async Task<IActionResult> ManageRoles(string userid)
        {
            var user = await _userManager.FindByIdAsync(userid);

            if(user == null) 
                return NotFound();

            var roles  = await _roleManager.Roles.ToListAsync();
            var viewModel = new GenericOne2ManyviewModel
            {
                Id = user.Id,
                Name = user.UserName,
                Items = roles.Select(role => new CheckBoxViewModel
                {
                    Name = role.Name,
                    IsChecked = _userManager.IsInRoleAsync(user, role.Name).Result
                }).ToList()
            };
            return View(viewModel);
        }
        [HttpPost]
        [AutoValidateAntiforgeryToken]
        public async Task<IActionResult> ManageRoles(GenericOne2ManyviewModel model)
        {
            var user = await _userManager.FindByIdAsync(model.Id);

            if (user == null)
                return NotFound();

            var userRoles = await _userManager.GetRolesAsync(user);
            foreach(var role in model.Items)
            {
                if (userRoles.Any(r => r == role.Name) && !role.IsChecked)
                   await _userManager.RemoveFromRoleAsync(user, role.Name);
                if (!userRoles.Any(r => r == role.Name) && role.IsChecked)
                    await _userManager.AddToRoleAsync(user, role.Name);
            }
            return RedirectToAction(nameof(Index));
        }
        
    }
}
