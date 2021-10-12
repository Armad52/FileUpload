using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace Final_0._0._3.ViewModel
{
    public class RegisterViewModel
    {
        [Required]
        
        public string Email { get; set; }
        
        [Required]
        
        public string Phone { get; set; }

        [Required]
        [DataType(DataType.Password)]
        
        public string Password { get; set; }

        [Required]
        [Compare("Password", ErrorMessage = "Пароли не совпадают")]
        [DataType(DataType.Password)]
        
        public string PasswordConfirm { get; set; }
    }
}
