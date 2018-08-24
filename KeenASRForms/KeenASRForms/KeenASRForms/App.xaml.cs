using Autofac;
using KeenASRForms.Interfaces;
using Plugin.DeviceInfo;
using System;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

[assembly: XamlCompilation (XamlCompilationOptions.Compile)]
namespace KeenASRForms
{
	public partial class App : Application
    {
        private static IContainer _container;
        public static IASR asr;
        public static bool fResult { get; set; }

        public App ()
		{
			InitializeComponent();

			MainPage = new MainPage();
		}

        public static bool Init(AppSetup appSetup)
        {
            _container = appSetup.CreateContainer();
            SetUpASR();
            return true;
        }


        private static void SetUpASR()
        {
            bool cont = true;
            if (CrossDeviceInfo.Current.Platform == Plugin.DeviceInfo.Abstractions.Platform.iOS)
            {
                var version = new Version(CrossDeviceInfo.Current.Version);
                var v11 = new Version("11.0");
                var vres = version.CompareTo(v11);

                if (vres > 0)
                    cont = true;
                else
                    cont = false;
            }
            if (cont)
            {
                string[] phrases = new string[]
            {"ZERO", "O", "ONE", "TWO", "THREE", "FOUR", "FIVE", "SIX", "SEVEN", "EIGHT", "NINE",};

                asr = _container.Resolve<IASR>();
                fResult = asr.initialize("librispeech-nnet2-en-us");
                fResult = asr.createDecodingGraph("numbers", phrases);
                fResult = asr.prepareForListening("numbers");

                asr.SetCreateAudioRecordings(true);

                // set float values to the desired timeout
                asr.SetVADParameter(0, 600f);
                asr.SetVADParameter(1, 600f);
                asr.SetVADParameter(2, 600f);
                asr.SetVADParameter(3, 600f);
            }
        }
    }
}
