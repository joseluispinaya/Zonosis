using CommunityToolkit.Maui;
using Microsoft.Extensions.Logging;

namespace Zonosis.Mobile
{
    public static class MauiProgram
    {
        public static MauiApp CreateMauiApp()
        {
            var builder = MauiApp.CreateBuilder();
            builder
                .UseMauiApp<App>()
                .ConfigureFonts(fonts =>
                {
                    fonts.AddFont("Ubuntu-Regular.ttf", "UbuntuRegular");
                    fonts.AddFont("Ubuntu-Bold.ttf", "UbuntuBold");
                })
                .UseMauiCommunityToolkit();

            //builder.Services.AddSingleton<IRepository, Repository>();
            //builder.Services.AddSingleton<HttpClient>();

#if DEBUG
            builder.Logging.AddDebug();
#endif

            RegisterAppDependencies(builder.Services);
            return builder.Build();
        }

        static void RegisterAppDependencies(IServiceCollection services)
        {
            services.AddSingleton<CommonService>();
            services.AddSingleton<IRepository, Repository>();
            services.AddTransient<AuthService>();

            services.AddTransient<LoginRegisterViewModel>()
                .AddTransient<LoginRegisterPage>();

            services.AddSingleton<HomeViewModel>()
                .AddSingleton<HomePage>();
        }
    }
}
