using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace Cloud9_2.Pages.Admin.Roles
{
    [Authorize(Roles = "SuperAdmin,Admin")]
    public class IndexModel : PageModel
    {
        private readonly RoleManager<IdentityRole> _roleManager;

        public IndexModel(RoleManager<IdentityRole> roleManager)
        {
            _roleManager = roleManager;
        }

        public IList<IdentityRole> Roles { get; set; } = new List<IdentityRole>();

        public async Task OnGetAsync()
        {
            Roles = await _roleManager.Roles
                .OrderBy(r => r.Name)
                .ToListAsync();
        }

        public async Task<IActionResult> OnPostAddRoleAsync(RoleDto dto)
        {
            if (!ModelState.IsValid || string.IsNullOrWhiteSpace(dto?.RoleName))
            {
                return new BadRequestObjectResult(new { message = "A role neve kötelező." });
            }

            var roleName = dto.RoleName.Trim();
            var normalizedName = _roleManager.NormalizeKey(roleName);

            var exists = await _roleManager.Roles
                .AnyAsync(r => r.NormalizedName == normalizedName);

            if (exists)
            {
                return new BadRequestObjectResult(new { message = "Már létezik ilyen szerepkör." });
            }

            var role = new IdentityRole(roleName);
            var result = await _roleManager.CreateAsync(role);

            if (!result.Succeeded)
            {
                return new BadRequestObjectResult(new
                {
                    message = "A szerepkör létrehozása sikertelen.",
                    errors = result.Errors.Select(e => e.Description)
                });
            }

            return new JsonResult(new
            {
                success = true,
                role = new
                {
                    id = role.Id,
                    name = role.Name,
                    normalizedName = role.NormalizedName ?? string.Empty
                }
            });
        }

        public async Task<IActionResult> OnPostUpdateRoleAsync(RoleUpdateDto dto)
        {
            if (!ModelState.IsValid || string.IsNullOrWhiteSpace(dto?.Id) || string.IsNullOrWhiteSpace(dto?.Name))
            {
                return new BadRequestObjectResult(new { message = "Érvénytelen adatok." });
            }

            var role = await _roleManager.FindByIdAsync(dto.Id);
            if (role == null)
            {
                return new NotFoundObjectResult(new { message = "A szerepkör nem található." });
            }

            var newName = dto.Name.Trim();
            var normalizedName = _roleManager.NormalizeKey(newName);

            var duplicateExists = await _roleManager.Roles
                .AnyAsync(r => r.Id != dto.Id && r.NormalizedName == normalizedName);

            if (duplicateExists)
            {
                return new BadRequestObjectResult(new { message = "Már létezik ilyen nevű szerepkör." });
            }

            role.Name = newName;
            role.NormalizedName = normalizedName;

            var result = await _roleManager.UpdateAsync(role);

            if (!result.Succeeded)
            {
                return new BadRequestObjectResult(new
                {
                    message = "A szerepkör módosítása sikertelen.",
                    errors = result.Errors.Select(e => e.Description)
                });
            }

            return new JsonResult(new
            {
                success = true,
                role = new
                {
                    id = role.Id,
                    name = role.Name,
                    normalizedName = role.NormalizedName ?? string.Empty
                }
            });
        }

        public async Task<IActionResult> OnPostDeleteRoleAsync(string id)
        {
            if (string.IsNullOrWhiteSpace(id))
            {
                return new BadRequestObjectResult(new { message = "Hiányzó azonosító." });
            }

            var role = await _roleManager.FindByIdAsync(id);
            if (role == null)
            {
                return new NotFoundObjectResult(new { message = "A szerepkör nem található." });
            }

            var result = await _roleManager.DeleteAsync(role);

            if (!result.Succeeded)
            {
                return new BadRequestObjectResult(new
                {
                    message = "A szerepkör törlése sikertelen.",
                    errors = result.Errors.Select(e => e.Description)
                });
            }

            return new JsonResult(new
            {
                success = true,
                deletedId = id
            });
        }

        public class RoleDto
        {
            [Required]
            public string RoleName { get; set; } = string.Empty;
        }

        public class RoleUpdateDto
        {
            [Required]
            public string Id { get; set; } = string.Empty;

            [Required]
            public string Name { get; set; } = string.Empty;
        }
    }
}