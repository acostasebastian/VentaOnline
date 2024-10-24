using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using VentaOnline.Data;
using VentaOnline.Models;
using VentaOnline.Utilities;

namespace VentaOnline.DataAccess.Data.Initialiser
{
    public class InicializadorBD : IInicializadorBD
    {
        private readonly ApplicationDbContext _db;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly IConfiguration _config;

        public InicializadorBD(ApplicationDbContext db, UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager, IConfiguration config) 
        {
            _db = db;
            _userManager = userManager;
            _roleManager = roleManager;
            _config = config;
        }

        public void Inicializar()
        {
            try
            {
                //Se verifica si hay migraciones pendientes. Y si las hay, se ejecutan
                if (_db.Database.GetPendingMigrations().Count() > 0)
                {
                    _db.Database.Migrate();
                }

            }
            catch (Exception)
            {

                throw;
            }

            //si se encuentra algun rol Administrador, se accede
            if (_db.Roles.Any(ro => ro.Name == CNT.Administrador)) return;

            //Creacion de roles
            _roleManager.CreateAsync(new IdentityRole(CNT.Administrador)).GetAwaiter().GetResult();
            _roleManager.CreateAsync(new IdentityRole(CNT.Cliente)).GetAwaiter().GetResult();

            //Creación del usuario inicial
            _userManager.CreateAsync(new ApplicationUser
            {
                UserName = _config["EmailSettings:EmailUsuario"],
                Email = _config["EmailSettings:EmailUsuario"],
                EmailConfirmed = true,
            }, _config["EmailSettings:PasswordUsuario"]).GetAwaiter().GetResult();


            ApplicationUser usuario = (ApplicationUser)_db.Users.Where(us => us.Email == _config["EmailSettings:EmailUsuario"]).FirstOrDefault();
            _userManager.AddToRoleAsync(usuario, CNT.Administrador).GetAwaiter().GetResult();


            //Creación del usuario inicial
            _userManager.CreateAsync(new ApplicationUser
            {
                UserName = _config["EmailSettings:EmailDesarrollador"],
                Email = _config["EmailSettings:EmailDesarrollador"],
                EmailConfirmed = true,
            }, _config["EmailSettings:PasswordDesarrollador"]).GetAwaiter().GetResult();


            ApplicationUser usuarioAdmin = (ApplicationUser)_db.Users.Where(us => us.Email == _config["EmailSettings:EmailDesarrollador"]).FirstOrDefault();
            _userManager.AddToRoleAsync(usuarioAdmin, CNT.Administrador).GetAwaiter().GetResult();



         



        }
    }
}
