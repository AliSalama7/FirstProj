using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Movies.Domain.Constants;
using MoviesApp.Models;
using MoviesApp.Models.IdentityViewModels;
using System.Data;
using System.Security.Claims;

namespace MoviesApp.Controllers
{
    [Authorize(Roles = "Moderator")]
    public class RolesController : Controller
    {
        private readonly RoleManager<IdentityRole> _roleManager;
        public RolesController(RoleManager<IdentityRole> roleManager)
        {
            _roleManager = roleManager;
        }
    
        public async Task<IActionResult> Index()
        {
            var roles = await    _roleManager.Roles.ToListAsync();
            return View(roles);
        }

        public async Task<IActionResult> Add(RoleFormViewModel model)
        {
            if (!ModelState.IsValid)
                return View("Index", await  _roleManager.Roles.ToListAsync());
            if(await  _roleManager.RoleExistsAsync(model.Name))
            {
                ModelState.AddModelError("Name", "Role Is Exists");
                return View("Index", await  _roleManager.Roles.ToListAsync());
            }
            await _roleManager.CreateAsync(new IdentityRole(model.Name.Trim()));
            return RedirectToAction(nameof(Index));
        }
        public async Task<IActionResult> ManagePermissions(string roleid)
        {
            var role = await _roleManager.FindByIdAsync(roleid);
            if (role == null)
                return NotFound();
            var permissions = _roleManager.GetClaimsAsync(role).Result.Select(r => r.Value).ToList();
            var allPermissions = Permissions.GenerateAllPermissions().Select(m => new CheckBoxViewModel { Name = m }).ToList();
            foreach (var permission in allPermissions)
            {
                if (permissions.Any(c => c == permission.Name))
                    permission.IsChecked = true;
            }
            var viewModel = new GenericOne2ManyviewModel
            {
                Id = roleid,
                Name = role.Name,
                Items = allPermissions
            };
            return View(viewModel);
        }
        [HttpPost]
        [AutoValidateAntiforgeryToken]
        public async Task <IActionResult> ManagePermissions(GenericOne2ManyviewModel model)
        {
            var role = await _roleManager.FindByIdAsync(model.Id);
            if(role == null)
                return NotFound();
            var roleClaims = await _roleManager.GetClaimsAsync(role);
            foreach (var claim in roleClaims)
                await _roleManager.RemoveClaimAsync(role, claim);
            var selectedClaims = model.Items.Where(c => c.IsChecked).ToList();
            foreach (var claim in selectedClaims)
                await _roleManager.AddClaimAsync(role, new Claim("Permission", claim.Name));
            return RedirectToAction(nameof(Index));

        }
        public async Task<IActionResult> Delete(string id)
        {
            var role = await _roleManager.FindByIdAsync(id);   
            if (role == null) 
                return NotFound();
           var result = await _roleManager.DeleteAsync(role);
            if (!result.Succeeded)
                throw new Exception();

            return Ok();
        }
    }
}
