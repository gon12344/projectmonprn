using System;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using WpfApp1.Data;
using WpfApp1.Models;

namespace WpfApp1.Services
{
    public class AuthService
    {
        private readonly DBContext _context;
        public User? CurrentUser { get; private set; }

        public AuthService(DBContext context)
        {
            _context = context;
        }

        public async Task<bool> LoginAsync(string username, string password)
        {
            try
            {
                var hash = ComputeSha256(password);
                var user = await _context.Users
                    .Include(u => u.Role)
                    .FirstOrDefaultAsync(u => u.Username == username && u.PasswordHash == hash && u.IsActive);
                   
                CurrentUser = user;
                  return user != null;
             }
            catch
            {
     return false;
    }
        }

        public void Logout()
        {
      CurrentUser = null;
        }

        public static string ComputeSha256(string input)
     {
  using var sha = SHA256.Create();
     var bytes = sha.ComputeHash(Encoding.UTF8.GetBytes(input));
         var sb = new StringBuilder();
 foreach (var b in bytes)
  {
                sb.Append(b.ToString("x2"));
        }
      return sb.ToString();
        }
    }
}
