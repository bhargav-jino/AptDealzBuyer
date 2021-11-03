using Acr.UserDialogs;
using AptDealzBuyer.API;
using AptDealzBuyer.Model.Reponse;
using AptDealzBuyer.Utility;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using Xamarin.Essentials;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace AptDealzBuyer.Views.OtherPages
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class ContactSupportPage : ContentPage, INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        #region [ Objects ]
        SupportChatAPI supportChatAPI;
        private string tempCount;

        private List<ChatSupport> _mMessageList;
        public List<ChatSupport> mMessageList
        {
            get { return _mMessageList; }
            set
            {
                _mMessageList = value;
                OnPropertyChanged("mMessageList");
            }
        }
        #endregion

        #region [ Constructor ]
        public ContactSupportPage()
        {
            try
            {
                InitializeComponent();
                supportChatAPI = new SupportChatAPI();
                mMessageList = new List<ChatSupport>();

                Binding binding = new Binding("mMessageList", mode: BindingMode.TwoWay, source: this);
                lstChar.SetBinding(ListView.ItemsSourceProperty, binding);

                if (DeviceInfo.Platform == DevicePlatform.Android)
                    txtMessage.Keyboard = Keyboard.Create(KeyboardFlags.CapitalizeWord);

                MessagingCenter.Unsubscribe<string>(this, Constraints.Str_NotificationCount);
                MessagingCenter.Subscribe<string>(this, Constraints.Str_NotificationCount, (count) =>
                {
                    if (!Common.EmptyFiels(Common.NotificationCount))
                    {
                        lblNotificationCount.Text = count;
                        frmNotification.IsVisible = true;

                        tempCount = Common.NotificationCount;
                    }
                    else
                    {
                        frmNotification.IsVisible = false;
                        lblNotificationCount.Text = string.Empty;
                    }
                });

                Common.TempNotificationCount = tempCount;

                var backgroundWorker = new BackgroundWorker();
                backgroundWorker.DoWork += async delegate
                 {
                     await GetMessages();

                     if (App.chatStoppableTimer != null)
                     {
                         App.chatStoppableTimer.Stop();
                         App.chatStoppableTimer = null;
                     }

                     if (App.chatStoppableTimer == null)
                     {
                         App.chatStoppableTimer = new StoppableTimer(TimeSpan.FromSeconds(1), async () =>
                         {
                             if (Common.PreviousNotificationCount != Common.NotificationCount)
                             {
                                 Common.PreviousNotificationCount = Common.NotificationCount;
                                 await GetMessages();
                             }
                         });
                     }
                     App.chatStoppableTimer.Start();
                 };
                backgroundWorker.RunWorkerAsync();
            }
            catch (Exception ex)
            {
                Common.DisplayErrorMessage("ContactSupportPage/Constructor: " + ex.Message);
            }
        }
        #endregion

        #region [ Methods ]
        public void Dispose()
        {
            GC.Collect();
            GC.SuppressFinalize(this);
        }

        protected override void OnDisappearing()
        {
            base.OnDisappearing();
            Dispose();

            if (App.chatStoppableTimer != null)
            {
                App.chatStoppableTimer.Stop();
                App.chatStoppableTimer = null;
            }
        }

        protected async override void OnAppearing()
        {
            base.OnAppearing();
            UserDialogs.Instance.ShowLoading(Constraints.Loading);
            await GetMessages();
            UserDialogs.Instance.HideLoading();
        }

        private async Task GetMessages()
        {
            try
            {
                var mResponse = await supportChatAPI.GetAllMyChat();
                if (mResponse != null && mResponse.Succeeded)
                {
                    JArray result = (JArray)mResponse.Data;
                    if (result != null)
                    {
                        //txtMessage.Text = string.Empty;
                        mMessageList = result.ToObject<List<ChatSupport>>();
                        if (mMessageList != null && mMessageList.Count > 0)
                        {
                            foreach (var message in mMessageList)
                            {
                                if (!message.IsMessageFromSupportTeam)
                                {
                                    //User Data
                                    message.IsContact = message.IsMessageFromSupportTeam;
                                    message.IsUser = true;
                                }
                                if (message.ChatMessageFromUserProfileImage == "")
                                {
                                    if (message.IsMessageFromSupportTeam)
                                    {
                                        message.ChatMessageFromUserProfileImage = Constraints.Img_Contact;
                                    }
                                    else
                                    {
                                        message.ChatMessageFromUserProfileImage = Constraints.Img_UserAccount;
                                    }
                                }
                            }
                            lstChar.IsVisible = true;
                            lblNoRecord.IsVisible = false;
                            //lstChar.ItemsSource = mMessageList.ToList();

                            var mMessage = mMessageList.LastOrDefault();
                            if (mMessage != null)
                                lstChar.ScrollTo(mMessage, ScrollToPosition.End, false);
                        }
                        else
                        {
                            lstChar.IsVisible = false;
                            lblNoRecord.IsVisible = true;
                        }
                    }
                }
                else
                {
                    lstChar.IsVisible = false;
                    lblNoRecord.IsVisible = true;
                    if (mResponse != null && !Common.EmptyFiels(mResponse.Message))
                        lblNoRecord.Text = mResponse.Message;
                    else
                        lblNoRecord.Text = Constraints.Something_Wrong;
                }
            }
            catch (Exception ex)
            {
                Common.DisplayErrorMessage("ContactSupportPage/GetMessages: " + ex.Message);
            }
        }

        private async Task SentMessage()
        {
            try
            {
                if (!Common.EmptyFiels(txtMessage.Text))
                {
                    var mResponse = await supportChatAPI.SendChatSupportMessage(txtMessage.Text);
                    if (mResponse != null && mResponse.Succeeded)
                    {
                        txtMessage.Text = string.Empty;
                        await GetMessages();
                    }
                    else
                    {
                        lstChar.IsVisible = false;
                        lblNoRecord.IsVisible = true;
                        if (mResponse != null && !Common.EmptyFiels(mResponse.Message))
                            lblNoRecord.Text = mResponse.Message;
                        else
                            lblNoRecord.Text = Constraints.Something_Wrong;
                    }
                }
            }
            catch (Exception ex)
            {
                Common.DisplayErrorMessage("ContactSupportPage/SentMessage: " + ex.Message);
            }
        }
        #endregion

        #region [ Events ]
        private void ImgMenu_Tapped(object sender, EventArgs e)
        {
            Common.BindAnimation(image: ImgMenu);
            //Common.OpenMenu();
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
                    Common.DisplayErrorMessage("ContactSupportPage/ImgNotification_Tapped: " + ex.Message);
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

        private async void ImgBack_Tapped(object sender, EventArgs e)
        {
            Common.BindAnimation(imageButton: ImgBack);
            await Navigation.PopAsync();
        }

        private void BtnLogo_Clicked(object sender, EventArgs e)
        {
            Common.MasterData.Detail = new NavigationPage(new MainTabbedPages.MainTabbedPage(Constraints.Str_Home));
        }

        private async void BtnSend_Clicked(object sender, EventArgs e)
        {
            var Tab = (ImageButton)sender;
            if (Tab.IsEnabled)
            {
                try
                {
                    Tab.IsEnabled = false;
                    Common.BindAnimation(imageButton: BtnSend);
                    await SentMessage();
                }
                catch (Exception ex)
                {
                    Common.DisplayErrorMessage("ContactSupportPage/BtnSend_Clicked: " + ex.Message);
                }
                finally
                {
                    Tab.IsEnabled = true;
                }
            }
        }

        private void lstChar_ItemTapped(object sender, ItemTappedEventArgs e)
        {
            lstChar.SelectedItem = null;
        }

        private async void lstChar_Refreshing(object sender, EventArgs e)
        {
            try
            {
                lstChar.IsRefreshing = true;
                mMessageList.Clear();
                await GetMessages();
                lstChar.IsRefreshing = false;
            }
            catch (Exception ex)
            {
                Common.DisplayErrorMessage("ContactSupportPage/lstChar_Refreshing: " + ex.Message);
            }
        }
        #endregion
    }
}