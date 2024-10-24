// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
#nullable disable

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Text.Encodings.Web;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Logging;
using VentaOnline.Models;
using VentaOnline.Utilities;

namespace VentaOnline.Areas.Identity.Pages.Account
{
    public class RegisterModel : PageModel
    {
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IUserStore<ApplicationUser> _userStore;
        private readonly IUserEmailStore<ApplicationUser> _emailStore;
        private readonly ILogger<RegisterModel> _logger;
        private readonly IEmailSender _emailSender;
        private readonly RoleManager<IdentityRole> _roleManager;

        public RegisterModel(
            UserManager<ApplicationUser> userManager,
            IUserStore<ApplicationUser> userStore,
            SignInManager<ApplicationUser> signInManager,
            ILogger<RegisterModel> logger,
            IEmailSender emailSender,
            RoleManager<IdentityRole> roleManager)
        {
            _userManager = userManager;
            _userStore = userStore;
            _emailStore = GetEmailStore();
            _signInManager = signInManager;
            _logger = logger;
            _emailSender = emailSender;
            _roleManager = roleManager;
        }

        /// <summary>
        ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        [BindProperty]
        public InputModel Input { get; set; }

        /// <summary>
        ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        public string ReturnUrl { get; set; }

        /// <summary>
        ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        public IList<AuthenticationScheme> ExternalLogins { get; set; }

        /// <summary>
        ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        public class InputModel
        {
            /// <summary>
            ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
            ///     directly from your code. This API may change or be removed in future releases.
            /// </summary>
            [Required(ErrorMessage = "El Email es obligatorio")]
            [EmailAddress(ErrorMessage = "El campo Email no es una dirección de correo electrónico válida.")]
            [Display(Name = "Email")]
            public string Email { get; set; }

            /// <summary>
            ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
            ///     directly from your code. This API may change or be removed in future releases.
            /// </summary>
            [Required(ErrorMessage = "La Contraseña es obligatoria")]
            [StringLength(100, ErrorMessage = "La {0} debe tener al menos {2} carácteres y como máximo {1} carácteres.", MinimumLength = 6)]
            [DataType(DataType.Password)]
            [Display(Name = "Contraseña")]
            public string Password { get; set; }

            /// <summary>
            ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
            ///     directly from your code. This API may change or be removed in future releases.
            /// </summary>
            [DataType(DataType.Password)]
            [Display(Name = "Confirmar Contraseña")]
            [Compare("Password", ErrorMessage = "Las contraseñas no son iguales.")]
            public string ConfirmPassword { get; set; }

            //Campos personalizados de cajeros 
            public string? Nombre { get; set; }

            public string? Apellido1 { get; set; }
            public string? Apellido2 { get; set; }

            [Display(Name = "Nombre Completo")]
            public string NombreCompleto
            {
                get
                {
                    if (Apellido2 == null)
                    {
                        return Nombre + " " + Apellido1 + " " + Apellido2;
                    }
                    else
                    {
                        return Nombre + " " + Apellido1;

                    };


                }
            }
            [Display(Name = "Teléfono")]
            public string PhoneNumber { get; set; }
        }


        public async Task OnGetAsync(string returnUrl = null)
        {
            ReturnUrl = returnUrl;
            ExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList();
        }

        public async Task<IActionResult> OnPostAsync(string returnUrl = null)
        {
            returnUrl ??= Url.Content("~/");
            ExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList();
            if (ModelState.IsValid)
            {
                var user = CreateUser();

                await _userStore.SetUserNameAsync(user, Input.Email, CancellationToken.None);
                await _emailStore.SetEmailAsync(user, Input.Email, CancellationToken.None);



                string message = "";

                try { 
                var result = await _userManager.CreateAsync(user, Input.Password);

                if (result.Succeeded)
                {
                        //Aquí validamos si los roles existen sino se crean
                        if (!await _roleManager.RoleExistsAsync(CNT.Administrador))
                        {
                            await _roleManager.CreateAsync(new IdentityRole(CNT.Administrador));
                            await _roleManager.CreateAsync(new IdentityRole(CNT.Cliente));                            
                        }

                        //Obtenemos el rol seleccionado
                        string rol = Request.Form["rol"].ToString();

                        //Validamos si el rol seleccionado es Admin y si lo es lo agregamos
                        if (rol == CNT.Administrador)
                        {
                            await _userManager.AddToRoleAsync(user, CNT.Administrador);

                        }
                        else
                        {                           
                            await _userManager.AddToRoleAsync(user, CNT.Cliente);                            
                           
                        }


                   _logger.LogInformation("El usuario ha creado una nueva cuenta con contraseña.");

                    //var userId = await _userManager.GetUserIdAsync(user);
                    //var code = await _userManager.GenerateEmailConfirmationTokenAsync(user);
                    //code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));
                    //var callbackUrl = Url.Page(
                    //    "/Account/ConfirmEmail",
                    //    pageHandler: null,
                    //    values: new { area = "Identity", userId = userId, code = code, returnUrl = returnUrl },
                    //    protocol: Request.Scheme);

                    //await _emailSender.SendEmailAsync(Input.Email, "Confirme su email",
                    //    $"Por favor, confirme su cuenta haciendo click <a href='{HtmlEncoder.Default.Encode(callbackUrl)}'>en el link</a>.");

                    if (_userManager.Options.SignIn.RequireConfirmedAccount)
                    {
                        return RedirectToPage("RegisterConfirmation", new { email = Input.Email, returnUrl = returnUrl });
                    }
                    else
                    {
                        await _signInManager.SignInAsync(user, isPersistent: false);
                        return LocalRedirect(returnUrl);
                    }
                }
                foreach (var error in result.Errors)
                {

                    if (error.Description.Contains("least one non alphanumeric character"))
                    {
                        message = "Las contraseñas deben tener al menos un carácter no alfanumérico.";
                    }

                    else if (error.Description.Contains("least one lowercase ('a'-'z')"))
                    {
                        message = "Las contraseñas deben tener al menos una minúscula('a' - 'z').";
                    }

                    else if (error.Description.Contains("least one uppercase ('A'-'Z')."))
                    {
                        message = "Las contraseñas deben tener al menos una mayúscula ('A'-'Z').";
                    }

                    else if (error.Description.Contains("already taken"))
                    {
                        message = "El Email ya se encuentra en uso.";
                    }


                    else
                    {
                        message = error.Description;
                    }
                    ModelState.AddModelError(string.Empty, message);
                }
                }

                catch (Exception ex)
                {


                    if (ex.InnerException != null &&
                       ex.InnerException != null &&
                       ex.InnerException.Message.Contains("EmailIndex"))
                    {
                        message = "El Email ingresado ya se encuentra registrado.";
                    }
                    else
                    {
                        message = "Contacte con el administrador >> Error: " + ex.Message;
                    }

                    ModelState.AddModelError(string.Empty, message);
                }
            }



            // If we got this far, something failed, redisplay form
            return Page();
        }

        private ApplicationUser CreateUser()
        {
            try
            {
                return Activator.CreateInstance<ApplicationUser>();
            }
            catch
            {
                throw new InvalidOperationException($"Can't create an instance of '{nameof(ApplicationUser)}'. " +
                    $"Ensure that '{nameof(ApplicationUser)}' is not an abstract class and has a parameterless constructor, or alternatively " +
                    $"override the register page in /Areas/Identity/Pages/Account/Register.cshtml");
            }
        }

        private IUserEmailStore<ApplicationUser> GetEmailStore()
        {
            if (!_userManager.SupportsUserEmail)
            {
                throw new NotSupportedException("The default UI requires a user store with email support.");
            }
            return (IUserEmailStore<ApplicationUser>)_userStore;
        }
    }
}
