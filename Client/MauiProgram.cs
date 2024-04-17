using Microsoft.Extensions.Logging;
using ZXing.Net.Maui.Controls;

namespace Client;

public static class MauiProgram
{
    public static MauiApp CreateMauiApp()
    {
        var builder = MauiApp.CreateBuilder();
        builder
            .UseMauiApp<App>()
            .ConfigureFonts(fonts =>
            {
                fonts.AddFont("Carlito-Bold.ttf", "BoldFont");
                fonts.AddFont("Carlito-Regular.ttf", "RegularFont");
                fonts.AddFont("Cascade-Code.ttf", "LogoFont");
            })
            .UseBarcodeReader();

#if DEBUG
        builder.Logging.AddDebug();
#endif
        return builder.Build();
    }
}
