using System;
using Android.App;
using Android.Runtime;
using Plugin.CurrentActivity;

namespace KeenASRForms.Droid
{
    //You can specify additional application information in this attribute
    [Application]
    public class MainApplication : Application
    {
        public MainApplication(IntPtr handle, JniHandleOwnership transer)
          : base(handle, transer)
        {
        }

        public override void OnCreate()
        {
            base.OnCreate();

            //A great place to initialize Xamarin.Insights and Dependency Services!
            CrossCurrentActivity.Current.Init(this);
        }

    }
}