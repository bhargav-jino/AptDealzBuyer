using AptDealzBuyer.Model;
using AptDealzBuyer.Utility;
using System;
using System.Collections.Generic;
using System.Linq;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace AptDealzBuyer.Views.MainTabbedPages
{
    [XamlCompilation(XamlCompilationOptions.Compile)]

    public partial class HomeView : ContentView
    {
        #region [ Constructor ]
        public HomeView()
        {
            InitializeComponent();
            BindMenus();

            MessagingCenter.Unsubscribe<string>(this, Constraints.Str_NotificationCount);
            MessagingCenter.Subscribe<string>(this, Constraints.Str_NotificationCount, (count) =>
            {
                if (!Common.EmptyFiels(Common.NotificationCount))
                {
                    lblNotificationCount.Text = count;
                    frmNotification.IsVisible = true;
                }
                else
                {
                    frmNotification.IsVisible = false;
                    lblNotificationCount.Text = string.Empty;
                }
            });
        }
        #endregion

        #region [ Methods ]
        private void BindMenus()
        {
            try
            {
                List<HomeMenu> HomeMenus;
                HomeMenus = new List<HomeMenu>()
                {
                    new HomeMenu { MenuId = 1,  MenuImage = Constraints.Img_ActiveRequirements,   UiName="Active\nRequirements",          MenuName = Constraints.Str_Requirements         },
                    new HomeMenu { MenuId = 2,  MenuImage = Constraints.Img_PostRequirements,     UiName="Post New\nRequirement",         MenuName = Constraints.Str_PostNewRequirements  },
                    new HomeMenu { MenuId = 3,  MenuImage = Constraints.Img_PreviousRequirements, UiName = "Previous\nRequirements",      MenuName = Constraints.Str_PreviousRequirements },
                    new HomeMenu { MenuId = 4,  MenuImage = Constraints.Img_OrderHistory,         UiName = "Order\nHistory",              MenuName = Constraints.Str_Order                },
                    new HomeMenu { MenuId = 5,  MenuImage = Constraints.Img_ShippingDetails,      UiName = "Shipping\nDetails",           MenuName = Constraints.Str_ShippingDetails      },
                    new HomeMenu { MenuId = 6,  MenuImage = Constraints.Img_Notifications,        UiName = Constraints.Str_Notifications, MenuName = Constraints.Str_Notifications        },
                    new HomeMenu { MenuId = 7,  MenuImage = Constraints.Img_Profile,              UiName = Constraints.Str_Profile,       MenuName = Constraints.Str_Profile              },
                    new HomeMenu { MenuId = 8,  MenuImage = Constraints.Img_Grievances,           UiName = Constraints.Str_Grievances,    MenuName = Constraints.Str_Grievances           },
                    new HomeMenu { MenuId = 9,  MenuImage = Constraints.Img_ContactSupport,       UiName = "Contact\nSupport",            MenuName = Constraints.Str_ContactSupport       },
                    new HomeMenu { MenuId = 10, MenuImage = Constraints.Img_AboutAptDealz,        UiName = "About\nAptDealz",             MenuName = Constraints.Str_AboutAptDealz        },
                    new HomeMenu { MenuId = 11, MenuImage = Constraints.Img_TermsAndPolicies,     UiName = "Terms & Policies",            MenuName = Constraints.Str_TermsPolicies        },
                    new HomeMenu { MenuId = 12, MenuImage = Constraints.Img_FAQHelp,              UiName = "FAQ & Help",                  MenuName = Constraints.Str_FAQHelp              },
                    new HomeMenu { MenuId = 13, MenuImage = Constraints.Img_WeSupport,            UiName = "We Support",                  MenuName = Constraints.Str_WeSupport            },
                };

                flvMenus.FlowItemsSource = HomeMenus.ToList();
            }
            catch (Exception ex)
            {
                Common.DisplayErrorMessage("HomeView/BindMenus: " + ex.Message);
            }
        }
        #endregion

        #region [ Events ]
        private async void ImgMenu_Tapped(object sender, EventArgs e)
        {
            var Tab = (Image)sender;
            if (Tab.IsEnabled)
            {
                try
                {
                    Common.BindAnimation(image: ImgMenu);
                    Tab.IsEnabled = false;
                    await Navigation.PushAsync(new OtherPages.SettingsPage());
                }
                catch (Exception ex)
                {
                    Common.DisplayErrorMessage("HomeView/ImgNotification_Tapped: " + ex.Message);
                }
                finally
                {
                    Tab.IsEnabled = true;
                }
            }
        }

        private async void ImgNotification_Tapped(object sender, EventArgs e)
        {
            var Tab = (Grid)sender;
            if (Tab.IsEnabled)
            {
                try
                {
                    Tab.IsEnabled = false;
                    await Navigation.PushAsync(new DashboardPages.NotificationPage());
                }
                catch (Exception ex)
                {
                    Common.DisplayErrorMessage("HomeView/ImgNotification_Tapped: " + ex.Message);
                }
                finally
                {
                    Tab.IsEnabled = true;
                }
            }
        }

        private void ImgQuestion_Tapped(object sender, EventArgs e)
        {
            Common.MasterData.Detail = new NavigationPage(new MainTabbedPages.MainTabbedPage(Constraints.Str_FAQHelp));
        }

        private async void BtnMenu_Tapped(object sender, EventArgs e)
        {
            var Tab = (Frame)sender;
            if (Tab.IsEnabled)
            {
                try
                {
                    Tab.IsEnabled = false;
                    var menuName = Tab.BindingContext as HomeMenu;

                    if (menuName != null && menuName.MenuName == Constraints.Str_PostNewRequirements)
                    {
                        await Navigation.PushAsync(new DashboardPages.PostNewRequiremntPage());
                    }
                    else if (menuName != null && menuName.MenuName == Constraints.Str_Notifications)
                    {
                        await Navigation.PushAsync(new DashboardPages.NotificationPage());
                    }
                    else if (menuName != null && menuName.MenuName == Constraints.Str_ContactSupport)
                    {
                        await Navigation.PushAsync(new OtherPages.ContactSupportPage());
                    }
                    else if (menuName != null && menuName.MenuName == Constraints.Str_Grievances)
                    {
                        await Navigation.PushAsync(new DashboardPages.GrievancesPage());
                    }
                    else if (menuName != null && menuName.MenuName == Constraints.Str_WeSupport)
                    {
                        await Navigation.PushAsync(new OtherPages.WeSupportPage());
                    }
                    else if (menuName != null && menuName.MenuName != null)
                    {
                        Common.MasterData.Detail = new NavigationPage(new MainTabbedPages.MainTabbedPage(menuName.MenuName, isNavigate: true));
                    }
                }
                catch (Exception ex)
                {
                    Common.DisplayErrorMessage("HomeView/BtnMenu_Tapped: " + ex.Message);
                }
                finally
                {
                    Tab.IsEnabled = true;
                }
            }
        }
        #endregion
    }
}