using System;
using System.IO;
using System.Threading.Tasks;
using PaintShopIMS.Repositories;

namespace PaintShopIMS
{
    public static class TestDb
    {
        public static async Task SeedAndTestAsync()
        {
            try
            {
                var userRepo = new UserRepository();
                var user = await userRepo.GetByUsernameAsync("admin");
                
                string debugOutput = "";
                if (user == null)
                {
                    debugOutput = "USER IS NULL!";
                }
                else
                {
                    bool verify = BCrypt.Net.BCrypt.Verify("admin123", user.PasswordHash);
                    debugOutput = $"User found. ID={user.UserId}, Hash={user.PasswordHash}, Verify='admin123' -> {verify}";
                }
                File.WriteAllText("login_debug.txt", debugOutput);
            }
            catch (Exception ex)
            {
                File.WriteAllText("login_debug.txt", $"EXCEPTION: {ex.Message} | {ex.StackTrace}");
            }
        }
    }
}
