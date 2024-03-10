namespace MLFoodAnalyzerClient.Pages;

public partial class QRScanner : ContentPage
{
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
            string[] words = first.Value.Split(new char[] { '|' });

            NetworkPage.qR.Ip = words[0];
            NetworkPage.qR.Port = int.Parse(words[1]);
            NetworkPage.qR.Password = words[2];

            await Navigation.PopModalAsync();
        });
    }
}