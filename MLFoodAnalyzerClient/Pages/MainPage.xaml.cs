using MLFoodAnalyzerClient.Extension;

namespace MLFoodAnalyzerClient.Pages;

public partial class MainPage : ContentPage
{
    public LocalizationResourceManager LocalizationResourceManager => LocalizationResourceManager.Instance;
    private AlertService? alert;
    private Connection? connection;

    private const string logo = """
                          ``
          `.              `ys
          +h+             +yh-
          yyh:           .hyys
         .hyyh.          oyyyh`
         /yyyyy`        .hyydy/
         syyhhy+        oyyhsys
         hyyyoyh.      .hyyy:hh`
        .hyyyy:ho      +yyys-yh-
        :hyyyh-oh.    `hyyyo-oy/
        /yyyyh-:h+    -hyyh/-oy+
        +yyyyh:-yy    +yyyh--oyo
        +yyyyh/-sh.   syyyh--oyo
        +yyyyh/-oy/  `hyyyy--syo
        +yyyyh/-+y+  `hyyys--yy+
        :yyyyh/-+ys  .hyyyo-:hy:
        .hyyyh+-+ys  .hyyyo-oyh`
        `yyyyyo-oyy  .hyyy+-yyy
         +yyyys-syy  `hyyh/oyy/
         .hyyyh-hyy  `hyyh/hyh
          oyyyhshys   yyyhyyy+
          oyyyhshys   yyyhyyy+
           /hyyyyyo`.-oyyyyh/
           `syyyyyyyhyyyyyyho.
            .hyyyyhNdyyyyyyymh/`
        """;
    private bool IsFlag = true;
    bool IsPolicyRead;

    readonly Random rnd = new();
    readonly int minValue = 5;
    readonly int maxValue = 25;

    public MainPage()
    {
        IsPolicyRead = Preferences.Get("IsPolicyRead", true);
        if (IsPolicyRead) GoToPolicy();

        InitializeComponent();

        InfoLabel.FontSize = AppShell.settings.FSize + 5;
    }

    private async void GoToPolicy() => await Navigation.PushModalAsync(new PolicyPage());

    private async void SendText_Tapped(object sender, EventArgs e)
    {
        IsPolicyRead = Preferences.Get("IsPolicyRead", true);
        if (IsPolicyRead)
        {
            GoToPolicy();
            return;
        }

        string text = QueryEditor.Text;
        QueryEditor.Text = string.Empty;
        if (string.IsNullOrWhiteSpace(text) || !IsFlag) return;

        IsFlag = false;
        SendTextButton.IsInProgress = SendPictureButton.IsInProgress = true;

        if (text[0].Equals('/'))
        {
            await Command(text[1..]);
            SendTextButton.IsInProgress = SendPictureButton.IsInProgress = false;
            IsFlag = true;
            return;
        }

        ResultEditor.Text += LocalizationResourceManager["AttachedAText"].ToString();

        connection ??= new();
        string results = await connection.SendText(text) ?? string.Empty;

        await Display(results);
    }

    private async void SendPicture_Tapped(object sender, EventArgs e)
    {
        IsPolicyRead = Preferences.Get("IsPolicyRead", true);
        if (IsPolicyRead)
        {
            GoToPolicy();
            return;
        }

        if (!IsFlag) return;

        string path = await GetPicturePath();
        if (string.IsNullOrWhiteSpace(path)) return;

        IsFlag = false;
        SendTextButton.IsInProgress = SendPictureButton.IsInProgress = true;

        ResultEditor.Text += LocalizationResourceManager["AttachedAPicture"].ToString();
        
        connection ??= new();
        string results = await connection.SendPicture(path) ?? string.Empty;

        await Display(results);
    }

    private async Task Display(string results)
    {
        ResultEditor.Text += LocalizationResourceManager["Server"].ToString();
        foreach (var result in results)
        {
            ResultEditor.Text += result;
            await Task.Delay(rnd.Next(minValue, maxValue));
        }

        SendTextButton.IsInProgress = SendPictureButton.IsInProgress = false;
        IsFlag = true;
    }

    private void QueryEditor_Changed(object sender, TextChangedEventArgs e)
    {
        if (SendPictureButton is null || SendTextButton is null) return;

        SendPictureButton.IsVisible = string.IsNullOrWhiteSpace(QueryEditor.Text);
        SendTextButton.IsVisible = !string.IsNullOrWhiteSpace(QueryEditor.Text);
    }

    private async Task<string> GetPicturePath()
    {
#if ANDROID || IOS
        alert ??= new();
        bool result = await alert.DisplayMessage(LocalizationResourceManager["SelectAnAction"].ToString(), 
            LocalizationResourceManager["TakeAPicture"].ToString(), LocalizationResourceManager["Gallery"].ToString());
        string res = (result) ? await GetPicture() : await GetMedia();
        return res;
#else
        string res = await GetMedia();
        return res;
#endif
    }

    private async Task<string> GetPicture() => await GetPathToImage(await MediaPicker.Default.CapturePhotoAsync());

    private async Task<string> GetMedia() => await GetPathToImage(await MediaPicker.Default.PickPhotoAsync());

    private async Task<string> GetPathToImage(FileResult? myPhoto)
    {
        if (myPhoto is null) return string.Empty;

        string localFilePath = Path.Combine(FileSystem.CacheDirectory, myPhoto.FileName);
        using Stream sourceStream = await myPhoto.OpenReadAsync();
        using FileStream localFileStream = File.OpenWrite(localFilePath);
        await sourceStream.CopyToAsync(localFileStream);
        localFileStream.Close();

        return localFilePath;
    }

    private async Task Command(string command)
    {
        switch (command)
        {
            case "clear":
                ResultEditor.Text = string.Empty;
                break;
            case "Kamchatka":
                ResultEditor.FontFamily = "LogoFont";
                ResultEditor.Text = logo;
                ResultEditor.TextType = TextType.Text;
                await Task.Delay(5000);
                ResultEditor.FontFamily = "RegularFont";
                ResultEditor.Text = string.Empty;
                ResultEditor.TextType = TextType.Html;
                break;
            default:
                break;
        }
    }
}





/*
                  ``
  `.              `ys
  +h+             +yh-
  yyh:           .hyys
 .hyyh.          oyyyh`
 /yyyyy`        .hyydy/
 syyhhy+        oyyhsys
 hyyyoyh.      .hyyy:hh`
.hyyyy:ho      +yyys-yh-
:hyyyh-oh.    `hyyyo-oy/
/yyyyh-:h+    -hyyh/-oy+
+yyyyh:-yy    +yyyh--oyo
+yyyyh/-sh.   syyyh--oyo
+yyyyh/-oy/  `hyyyy--syo
+yyyyh/-+y+  `hyyys--yy+
:yyyyh/-+ys  .hyyyo-:hy:
.hyyyh+-+ys  .hyyyo-oyh`
`yyyyyo-oyy  .hyyy+-yyy
 +yyyys-syy  `hyyh/oyy/
 .hyyyh-hyy  `hyyh/hyh
  oyyyhshys   yyyhyyy+
  oyyyhshys   yyyhyyy+
   /hyyyyyo`.-oyyyyh/
   `syyyyyyyhyyyyyyho.
    .hyyyyhNdyyyyyyymh/`
*/