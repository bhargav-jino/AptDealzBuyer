using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace AptDealzBuyer.Views.MasterData
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class MenuPage : ContentPage
    {
        public MenuPage()
        {
            InitializeComponent();
        }

        //private async void Logout_Tapped(object sender, EventArgs e)
        //{
        //    var isClose = await DisplayAlert(Constraints.Logout, Constraints.AreYouSureWantLogout, Constraints.Yes, Constraints.No);
        //    if (isClose)
        //    {
        //        Settings.EmailAddress = string.Empty;
        //        Settings.UserToken = string.Empty;
        //        App.Current.MainPage = new NavigationPage(new SplashScreen.WelcomePage(true));
        //    }
        //}
    }
}