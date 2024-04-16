using Client.Extension;

namespace Client.Pages;

public partial class DetailInformation : ContentPage
{
    public DetailInformation(History history)
	{
		InitializeComponent();

        BindingContext = this;
        detailUser.Text = $"{history.NickName} #{history.IdAccount}";
        detailFood.Text = $"{history.NameFood[0].ToString().ToUpper()}{history.NameFood[1..]} #{history.IdFood}";
        detailDate.Text = $"{history.LastUpdate}";
        oldDescription.Text = history.Old_Description;
        newDescription.Text = history.New_Description;
    }

    private void Button_Clicked(object sender, EventArgs e) => Navigation.PopAsync();
}