using Client.Services.Interfaces;
using Client.Services;
using Client.UI;

namespace Client
{
    internal class Program
    {
        static void Main(string[] args)
        {
            const string URI = "https://localhost:7078/api";
            IAuthService authService = new AuthService(URI);
            IProductService productService = new ProductService(URI);
            Menu menu = new Menu(authService, productService);
            menu.Show();
        }
    }
}
