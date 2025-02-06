using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using stkTakip.Data;
using stkTakip.Forms;

namespace stkTakip
{
    internal static class Program
    {
        public static IServiceProvider ServiceProvider { get; private set; }

        /// <summary>
        ///  The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            var services = new ServiceCollection();

            services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlServer(
                    "Server=(localdb)\\mssqllocaldb;Database=StokTakip;Trusted_Connection=True;MultipleActiveResultSets=true",
                    sqlOptions => sqlOptions.EnableRetryOnFailure(
                        maxRetryCount: 5,
                        maxRetryDelay: TimeSpan.FromSeconds(30),
                        errorNumbersToAdd: null)
                ));

            services.AddTransient<MainForm>();
            services.AddTransient<CategoryForm>();
            services.AddTransient<ProductForm>();
            services.AddTransient<StockForm>();
            services.AddTransient<ReportsForm>();
            services.AddTransient<UserForm>();

            ServiceProvider = services.BuildServiceProvider();

            Application.Run(ServiceProvider.GetRequiredService<MainForm>());
        }

        private static void ConfigureServices(ServiceCollection services)
        {
            // Add DbContext
            services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlServer(
                    "Server=(localdb)\\mssqllocaldb;Database=StokTakip;Trusted_Connection=True;MultipleActiveResultSets=true",
                    sqlOptions => sqlOptions.EnableRetryOnFailure(
                        maxRetryCount: 5,
                        maxRetryDelay: TimeSpan.FromSeconds(30),
                        errorNumbersToAdd: null)
                ));

            // Add Forms
            services.AddTransient<MainForm>();
            services.AddTransient<CategoryForm>();
            services.AddTransient<ProductForm>();
            services.AddTransient<StockForm>();
            services.AddTransient<ReportsForm>();
            services.AddTransient<UserForm>();
        }
    }
}