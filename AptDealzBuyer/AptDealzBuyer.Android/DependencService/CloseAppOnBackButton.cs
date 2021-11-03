using Android.App;
using AptDealzBuyer.Droid.DependencService;
using AptDealzBuyer.Interfaces;
using AptDealzBuyer.Utility;
using System;
using Xamarin.Forms;

[assembly: Dependency(typeof(CloseAppOnBackButton))]

namespace AptDealzBuyer.Droid.DependencService
{
    public class CloseAppOnBackButton : ICloseAppOnBackButton, IDisposable
    {
#pragma warning disable CS0618 // Type or member is obsolete
        public void CloseApp()
        {
            try
            {
                var activity = (Activity)Xamarin.Forms.Forms.Context;
                activity.FinishAffinity();
            }
            catch (Exception ex)
            {
                Common.DisplayErrorMessage("CloseAppOnBackButton: " + ex.Message);
            }
        }

        public void Dispose()
        {
            GC.Collect(GC.MaxGeneration, GCCollectionMode.Forced);
        }
#pragma warning restore CS0618 // Type or member is obsolete
    }
}