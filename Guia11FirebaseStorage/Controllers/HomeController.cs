using Firebase.Auth;
using Firebase.Storage;
using Guia11FirebaseStorage.Models;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace Guia11FirebaseStorage.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
        [HttpPost]
        public async Task<ActionResult> SubirArchivo(IFormFile archivo)
        {
            
            Stream archivoASubir = archivo.OpenReadStream();
            string email = "laura.tejada@catolica.edu.sv";
            string clave = "apolo23";
            string ruta = "guia11firebasestorage.appspot.com";
            string api_key = "AIzaSyD-wqbNIaTNQ0LKkfWVmxJxdcivMdbJvXc";

            var auth = new FirebaseAuthProvider(new FirebaseConfig(api_key));
            var autenticarFireBase = await auth.SignInWithEmailAndPasswordAsync(email, clave);
            var cancellation = new CancellationTokenSource();
            var tokenUser = autenticarFireBase.FirebaseToken;

            var tareaCargarArchivo = new FirebaseStorage(ruta,
                                       new FirebaseStorageOptions
                                       {
                                           AuthTokenAsyncFactory = () => Task.FromResult(tokenUser),
                                           ThrowOnCancel = true
                                       }
                                       ).Child("Archivos")
                                       .Child(archivo.FileName)
                                       .PutAsync(archivoASubir, cancellation.Token);

            var urlArchivoCargado = await tareaCargarArchivo;



            return RedirectToAction("VerImagen", new {urlImagen = urlArchivoCargado});
        }

        [HttpGet]
        public ActionResult VerImagen(string urlImagen)
        {
            return View((object)urlImagen);
        }
    }
}
