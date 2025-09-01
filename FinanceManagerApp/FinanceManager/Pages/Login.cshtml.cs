using System.ComponentModel.DataAnnotations;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

// Necesitamos un "molde" para entender la respuesta de la API
public class UserDto
{
    public string Username { get; set; }
    public string Token { get; set; }
}

namespace FinanceManager.Pages.Account
{
    public class LoginModel : PageModel
    {
        // El "experto" que nos da acceso al teléfono para llamar a la API
        private readonly IHttpClientFactory _httpClientFactory;

        public LoginModel(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }

        // El "portapapeles" donde se guardarán los datos del formulario
        [BindProperty]
        public InputModel Input { get; set; }

        // Propiedad para mostrar mensajes de error en la página
        [TempData]
        public string ErrorMessage { get; set; }

        // La definición de nuestro "portapapeles"
        public class InputModel
        {
            [Required(ErrorMessage = "El nombre de usuario es obligatorio.")]
            public string Username { get; set; }

            [Required(ErrorMessage = "La contraseña es obligatoria.")]
            [DataType(DataType.Password)]
            public string Password { get; set; }
        }

        public void OnGet() { }

        // El método que se ejecuta cuando el usuario envía el formulario
        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page(); // Si el formulario está mal, lo mostramos de nuevo
            }

            // 1. Tomamos el teléfono
            var client = _httpClientFactory.CreateClient("api");

            // 2. Preparamos el mensaje en formato JSON
            var jsonContent = new StringContent(
                JsonSerializer.Serialize(new { username = Input.Username, password = Input.Password }),
                Encoding.UTF8,
                "application/json"
            );

            // 3. Llamamos a la API
            var response = await client.PostAsync("/api/account/login", jsonContent);

            // 4. Analizamos la respuesta
            if (response.IsSuccessStatusCode)
            {
                // Si fue exitoso, leemos el token
                var responseBody = await response.Content.ReadAsStringAsync();
                var userDto = JsonSerializer.Deserialize<UserDto>(responseBody, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

                // Y lo guardamos en una cookie segura
                var cookieOptions = new CookieOptions
                {
                    HttpOnly = true,
                    Secure = true,
                    Expires = DateTime.UtcNow.AddDays(7)
                };
                Response.Cookies.Append("jwtToken", userDto.Token, cookieOptions);

                // Finalmente, enviamos al usuario a la página principal
                return RedirectToPage("/Index");
            }
            else
            {
                // Si falló, mostramos un error
                ErrorMessage = "Usuario o contraseña incorrectos.";
                return Page();
            }
        }
    }
}
