using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VentaOnline.Models
{
    public class ApplicationUser : IdentityUser
    {
        public string? Nombre  { get; set; }

        public string? Apellido1 { get; set; }
        public string? Apellido2 { get; set; }        


        [Display(Name = "Nombre Completo")]
        public string NombreCompleto
        {
            get { 
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

    }
}
