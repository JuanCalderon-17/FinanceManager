using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text.Json;
using System.Threading.Tasks;
using FinanceManager.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace FinanceManager.Pages.Account
{
    public class IndexModel : PageModel
    {
        private readonly IHttpClientFactory _httpClientFactory;

        public List<TransactionDto> Transactions { get; set; } = new List<TransactionDto>();

        // El "experto" que nos da el tel�fono (el par�metro)
        public IndexModel(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }

        // Este m�todo se ejecuta cada vez que se carga la p�gina (GET).
        public async Task<IActionResult> OnGetAsync()
        {
            // 1. Leemos el "pase de acceso" (el token) desde la cookie.
            var token = Request.Cookies["jwtToken"];

            if (string.IsNullOrEmpty(token))
            {
                // Si no hay token, significa que el usuario no ha iniciado sesi�n.
                // Lo redirigimos a la p�gina de Login.
                return RedirectToPage("/Account/Login");
            }

            // 2. Creamos nuestro "tel�fono" (HttpClient) para llamar a la API.
            var client = _httpClientFactory.CreateClient("api");

            // 3. �Paso crucial! Ponemos el token en la cabecera de la petici�n.
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            // 4. Hacemos la llamada al endpoint que nos da las transacciones.
            var response = await client.GetAsync("/api/transactions");

            if (response.IsSuccessStatusCode)
            {
                // 5. Si la API nos responde con �xito, leemos los datos.
                var responseBody = await response.Content.ReadAsStringAsync();
                Transactions = JsonSerializer.Deserialize<List<TransactionDto>>(responseBody, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
            }
            else
            {
                // Si la API nos da un error (ej: el token expir�), lo redirigimos a Login.
                return RedirectToPage("/Account/Login");
            }

            return Page();
        }
    }
}
