namespace Client.Pages;

public partial class QRScanner : ContentPage
{
    private static readonly char[] separator = ['|'];

    public QRScanner()
    {
        InitializeComponent();

        QRReader.Options = new ZXing.Net.Maui.BarcodeReaderOptions
        {
            Formats = ZXing.Net.Maui.BarcodeFormat.QrCode,
            AutoRotate = true,
            Multiple = true,
        };
    }

    private void QRReader_BarcodesDetected(object sender, ZXing.Net.Maui.BarcodeDetectionEventArgs e)
    {
        var first = e.Results.FirstOrDefault();

        if (first is null) return;

        Dispatcher.DispatchAsync(async () =>
        {
            string[] words = first.Value.Split(separator);

            AppShell.settings.Ip = words[0];
            AppShell.settings.Port = int.Parse(words[1]);

            await Navigation.PopModalAsync();
        });
    }
}