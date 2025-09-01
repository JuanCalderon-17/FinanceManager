using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Data;

namespace FinanceManager.Pages.Account
{
    public class LogoutModel : PageModel
    {
        public IActionResult OnPost()
        {
            // Clear the user's session or authentication cookies
            Response.Cookies.Delete("jwt_token");
            //return RedirectToPage("/login" again);
            return RedirectToPage("/Account/Login");
        }
    }
}
