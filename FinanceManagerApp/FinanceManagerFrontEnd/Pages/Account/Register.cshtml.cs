using System.ComponentModel.DataAnnotations;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

// Usamos el mismo UserDto que definimos en la página de Login
// para entender la respuesta de la API.

namespace FinanceManagerFrontEnd.Pages.Account
{
    public class RegisterModel : PageModel
    {
        private readonly IHttpClientFactory _httpClientFactory;

        public RegisterModel(IHttpClientFactory httpClientFactory)
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

            [Required(ErrorMessage = "El email es obligatorio.")]
            [EmailAddress(ErrorMessage = "El formato del email no es válido.")]
            public string Email { get; set; }

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
                JsonSerializer.Serialize(new { username = Input.Username, email = Input.Email, password = Input.Password }),
                Encoding.UTF8,
                "application/json"
            );

            // Llamamos al endpoint de registro de nuestra API
            var response = await client.PostAsync("/api/account/register", jsonContent);

            if (response.IsSuccessStatusCode)
            {
                // Si el registro es exitoso, la API nos devuelve un token.
                // Lo guardamos en una cookie y lo redirigimos, ¡iniciando sesión automáticamente!
                var responseBody = await response.Content.ReadAsStringAsync();
                var userDto = JsonSerializer.Deserialize<UserDto>(responseBody, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

                var cookieOptions = new CookieOptions
                {
                    HttpOnly = true,
                    Secure = true,
                    Expires = DateTime.UtcNow.AddDays(7)
                };

                Response.Cookies.Append("jwtToken", userDto.Token, cookieOptions);

                return RedirectToPage("/Index");
            }
            else
            {
                // Si la API devuelve un error (ej: usuario ya existe), lo mostramos.
                var errorContent = await response.Content.ReadAsStringAsync();
                ErrorMessage = $"Error en el registro: {errorContent}";
                return Page();
            }
        }
    }
}
