using MLFoodAnalyzerClient.Extension;

namespace MLFoodAnalyzerClient.Pages;

public partial class DetailInformation : ContentPage
{

    private static Settings settings = AppShell.settings;

    public DetailInformation(History history)
	{
		InitializeComponent();

        settings = (Settings)Resources["settings"];

        BindingContext = this;
        detailUser.Text = $"{history.NickName}#{history.IdAccount}";
        detailFood.Text = $"{history.NameFood[0].ToString().ToUpper()}{history.NameFood[1..]} #{history.IdFood}";
        detailDate.Text = $"{history.LastUpdate}";
        oldDescription.Text = history.Old_Description;
        newDescription.Text = history.New_Description;
    }

    private void Button_Clicked(object sender, EventArgs e) => Navigation.PopAsync();
}