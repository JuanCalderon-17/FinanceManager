using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization.Infrastructure;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.ComponentModel.DataAnnotations;
using System.IdentityModel.Tokens.Jwt;
using System.Net.Http;
using System.Security.Claims;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

// Molde para entender la respuesta de la API
public class UserDto
{
    public string Username { get; set; }
    public string Token { get; set; }
}

namespace FinanceManager.Pages.Account
{
    public class LoginModel : PageModel
    {
        private readonly IHttpClientFactory _httpClientFactory;

        public LoginModel(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }

        [BindProperty]
        public InputModel Input { get; set; }

        [TempData]
        public string ErrorMessage { get; set; }

        public class InputModel
        {
            [Required(ErrorMessage = "El nombre de usuario es obligatorio.")]
            public string Username { get; set; }

            [Required(ErrorMessage = "La contraseña es obligatoria.")]
            [DataType(DataType.Password)]
            public string Password { get; set; }
        }

        public void OnGet() { }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            var client = _httpClientFactory.CreateClient("api");

            var jsonContent = new StringContent(
                JsonSerializer.Serialize(new { username = Input.Username, password = Input.Password }),
                Encoding.UTF8,
                "application/json"
            );

            var response = await client.PostAsync("/api/account/login", jsonContent);

            if (response.IsSuccessStatusCode)
            {
                var responseBody = await response.Content.ReadAsStringAsync();
                var userDto = JsonSerializer.Deserialize<UserDto>(responseBody, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

                // Guardamos el "pasaporte" (Token JWT) en su propia cookie segura.
                Response.Cookies.Append("jwtToken", userDto.Token, new CookieOptions { HttpOnly = true, Secure = true });

                // Leemos la información del pasaporte para crear el "pase de visitante".
                var handler = new JwtSecurityTokenHandler();
                var token = handler.ReadJwtToken(userDto.Token);
                var claims = token.Claims;

                var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                var principal = new ClaimsPrincipal(identity);

                // "Iniciamos sesión" en el frontend, creando la cookie de autenticación (el pase de visitante).
                await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal);

                return RedirectToPage("/Index");
            }
            else
            {
                ErrorMessage = "Usuario o contraseña incorrectos.";
                return Page();
            }
        }
    }
}