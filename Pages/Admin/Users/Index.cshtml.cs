using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Cloud9_2.Models;
using System.ComponentModel.DataAnnotations;

namespace Cloud9_2.Pages.Admin.Users
{
    [Authorize(Roles = "SuperAdmin, Admin")]
    public class IndexModel : PageModel
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        public IndexModel(
            UserManager<ApplicationUser> userManager,
            RoleManager<IdentityRole> roleManager)
        {
            _userManager = userManager;
            _roleManager = roleManager;
        }

        public IList<UserViewModel> Users { get; set; } = new List<UserViewModel>();

        public async Task OnGetAsync()
        {
            Users = await _userManager.Users
                .Select(u => new UserViewModel
                {
                    Id = u.Id,
                    UserName = u.UserName,
                    Email = u.Email,
                    PhoneNumber = u.PhoneNumber,
                    LockoutEnabled = u.LockoutEnabled,
                    EmailConfirmed = u.EmailConfirmed,
                    PhoneNumberConfirmed = u.PhoneNumberConfirmed,
                    AccessFailedCount = u.AccessFailedCount,
                    TwoFactorEnabled = u.TwoFactorEnabled,
                    Disabled = u.Disabled.GetValueOrDefault(false)
                })
                .ToListAsync();
        }

[ValidateAntiForgeryToken]
public async Task<IActionResult> OnPostAddUser(UserDto dto)
{
    Console.WriteLine("OnPostAddUser HIT");

    try
    {
        if (dto == null)
        {
            return new JsonResult(new
            {
                success = false,
                message = "A kérés adatai hiányoznak."
            })
            {
                StatusCode = 400
            };
        }

        if (!ModelState.IsValid)
        {
            var errors = ModelState
                .Where(x => x.Value.Errors.Count > 0)
                .SelectMany(x => x.Value.Errors.Select(e => new
                {
                    field = x.Key,
                    message = e.ErrorMessage
                }))
                .ToList();

            return new JsonResult(new
            {
                success = false,
                message = "Validációs hiba.",
                errors
            })
            {
                StatusCode = 400
            };
        }

        var existingUserByName = await _userManager.FindByNameAsync(dto.UserName);
        if (existingUserByName != null)
        {
            return new JsonResult(new
            {
                success = false,
                message = "Ez a felhasználónév már létezik."
            })
            {
                StatusCode = 400
            };
        }

        var existingUserByEmail = await _userManager.FindByEmailAsync(dto.Email);
        if (existingUserByEmail != null)
        {
            return new JsonResult(new
            {
                success = false,
                message = "Ez az email cím már használatban van."
            })
            {
                StatusCode = 400
            };
        }

        var user = new ApplicationUser
        {
            UserName = dto.UserName,
            Email = dto.Email,
            PhoneNumber = string.IsNullOrWhiteSpace(dto.PhoneNumber) ? null : dto.PhoneNumber,
            MustChangePassword = false
        };

        var result = await _userManager.CreateAsync(user, dto.Password);

        if (!result.Succeeded)
        {
            var errors = result.Errors.Select(e => new
            {
                field = e.Code,
                message = e.Description
            }).ToList();

            return new JsonResult(new
            {
                success = false,
                message = "A felhasználó létrehozása sikertelen.",
                errors
            })
            {
                StatusCode = 400
            };
        }

        return new JsonResult(new
        {
            success = true,
            message = "Felhasználó sikeresen létrehozva.",
            user = new
            {
                id = user.Id,
                userName = user.UserName,
                email = user.Email,
                phoneNumber = user.PhoneNumber,
                accessFailedCount = user.AccessFailedCount,
                disabled = user.Disabled
            }
        });
    }
    catch (Exception ex)
    {
        Console.WriteLine($"Exception in OnPostAddUser: {ex}");

        return new JsonResult(new
        {
            success = false,
            message = $"Szerver hiba: {ex.Message}"
        })
        {
            StatusCode = 500
        };
    }
}

        [ValidateAntiForgeryToken]
        public async Task<IActionResult> OnPostDeleteUserAsync([FromBody] DeleteUserDto dto)
        {
            if (string.IsNullOrEmpty(dto?.UserId))
            {
                return new JsonResult(new { success = false, message = "User ID is required." })
                {
                    StatusCode = 400
                };
            }

            var user = await _userManager.FindByIdAsync(dto.UserId);
            if (user == null)
            {
                return new JsonResult(new { success = false, message = "User not found." })
                {
                    StatusCode = 404
                };
            }

            var result = await _userManager.DeleteAsync(user);
            if (result.Succeeded)
            {
                return new JsonResult(new
                {
                    success = true,
                    message = $"User {user.UserName} deleted successfully."
                });
            }

            return new JsonResult(new
            {
                success = false,
                message = "Error deleting user: " + string.Join(", ", result.Errors.Select(e => e.Description))
            })
            {
                StatusCode = 400
            };
        }

        public async Task<IActionResult> OnPostForcePasswordChangeAsync(string id)
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user == null)
            {
                return new JsonResult(new
                {
                    success = false,
                    message = "User not found."
                })
                {
                    StatusCode = 404
                };
            }

            user.MustChangePassword = true;
            var result = await _userManager.UpdateAsync(user);

            if (!result.Succeeded)
            {
                var errors = result.Errors.Select(e => new
                {
                    field = "",
                    message = e.Description
                }).ToList();

                return new JsonResult(new
                {
                    success = false,
                    message = "Update failed.",
                    errors
                })
                {
                    StatusCode = 400
                };
            }

            return new JsonResult(new
            {
                success = true,
                message = $"User {user.UserName} must now change their password."
            });
        }

        public async Task<IActionResult> OnPostEditUserAsync([FromBody] EditUserDto dto)
        {
            if (dto == null || string.IsNullOrEmpty(dto.Id))
            {
                return new JsonResult(new
                {
                    success = false,
                    message = "Invalid user data."
                })
                {
                    StatusCode = 400
                };
            }

            var user = await _userManager.FindByIdAsync(dto.Id);
            if (user == null)
            {
                return new JsonResult(new
                {
                    success = false,
                    message = "User not found."
                })
                {
                    StatusCode = 404
                };
            }

            user.UserName = dto.UserName;
            user.Email = dto.Email;
            user.PhoneNumber = dto.PhoneNumber;
            user.AccessFailedCount = dto.AccessFailedCount;
            user.Disabled = dto.Disabled;

            var result = await _userManager.UpdateAsync(user);
            if (!result.Succeeded)
            {
                var errors = result.Errors.Select(e => new
                {
                    field = e.Code,
                    message = e.Description
                }).ToList();

                return new JsonResult(new
                {
                    success = false,
                    message = "A felhasználó módosítása sikertelen.",
                    errors
                })
                {
                    StatusCode = 400
                };
            }

            return new JsonResult(new
            {
                success = true,
                user = new
                {
                    Id = user.Id,
                    UserName = user.UserName,
                    Email = user.Email,
                    PhoneNumber = user.PhoneNumber,
                    AccessFailedCount = user.AccessFailedCount,
                    Disabled = user.Disabled
                }
            });
        }

[ValidateAntiForgeryToken]
public async Task<IActionResult> OnPostResetPassword(ResetPasswordDto dto)
{
    try
    {
        if (string.IsNullOrEmpty(dto?.UserId))
        {
            return new JsonResult(new
            {
                success = false,
                message = "User ID is required."
            })
            {
                StatusCode = 400
            };
        }

        var user = await _userManager.FindByIdAsync(dto.UserId);
        if (user == null)
        {
            return new JsonResult(new
            {
                success = false,
                message = "User not found."
            })
            {
                StatusCode = 404
            };
        }

        string today = DateTime.UtcNow.ToString("yyyyMMdd");
        string newPassword = $"{user.UserName}{today}C92";

        var token = await _userManager.GeneratePasswordResetTokenAsync(user);
        var resetResult = await _userManager.ResetPasswordAsync(user, token, newPassword);

        if (!resetResult.Succeeded)
        {
            return new JsonResult(new
            {
                success = false,
                message = "Error resetting password: " + string.Join(", ", resetResult.Errors.Select(e => e.Description))
            })
            {
                StatusCode = 400
            };
        }

        user.MustChangePassword = true;
        var updateResult = await _userManager.UpdateAsync(user);

        if (!updateResult.Succeeded)
        {
            return new JsonResult(new
            {
                success = false,
                message = "Error setting password change requirement: " + string.Join(", ", updateResult.Errors.Select(e => e.Description))
            })
            {
                StatusCode = 400
            };
        }

        return new JsonResult(new
        {
            success = true,
            message = $"Password reset to {newPassword} for {user.UserName}. User must change it on next login."
        });
    }
    catch (Exception ex)
    {
        Console.WriteLine($"Exception in OnPostResetPassword: {ex}");
        return new JsonResult(new
        {
            success = false,
            message = $"Server error: {ex.Message}"
        })
        {
            StatusCode = 500
        };
    }
}
    }

    public class EditUserDto
    {
        public string Id { get; set; } = string.Empty;
        public string UserName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string? PhoneNumber { get; set; }
        public int AccessFailedCount { get; set; }
        public bool Disabled { get; set; }
    }

    public class UserViewModel
    {
        public string Id { get; set; } = string.Empty;
        public string UserName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string? PhoneNumber { get; set; }
        public bool LockoutEnabled { get; set; }
        public bool EmailConfirmed { get; set; }
        public bool PhoneNumberConfirmed { get; set; }
        public bool TwoFactorEnabled { get; set; }
        public DateTime LockoutEnd { get; set; }
        public int AccessFailedCount { get; set; }
        public bool MustChangePassword { get; set; }
        public bool Disabled { get; set; }
    }

    public class UserDto
    {
        [Required(ErrorMessage = "Username is required")]
        public string UserName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Email is required")]
        [EmailAddress(ErrorMessage = "Invalid email format")]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "Password is required")]
        [StringLength(100, MinimumLength = 6, ErrorMessage = "Password must be at least 6 characters")]
        public string Password { get; set; } = string.Empty;

        [Phone(ErrorMessage = "Invalid phone number format")]
        public string? PhoneNumber { get; set; }
    }

    public class DeleteUserDto
    {
        public string UserId { get; set; } = string.Empty;
    }

    public class ResetPasswordDto
    {
        public string UserId { get; set; } = string.Empty;
    }
}