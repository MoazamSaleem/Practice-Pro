using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

public class Register
{
    public int id { get; set; }
    public string email { get; set; }
    public string password { get; set; }
    public string confirmPassword { get; set; }
}
