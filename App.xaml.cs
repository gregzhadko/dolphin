using System;
using System.Threading;
using System.Windows;
using GalaSoft.MvvmLight.Threading;

namespace Timeline
{

    public partial class App
    {
        static App()
        {
            DispatcherHelper.Initialize();
        }

        //show splash screen before launching app - made splash.png a resource as I'm then allowed to set how long it will show
        [STAThread]
        public static void Main()
        {
            SplashScreen splashScreen = new SplashScreen("resources/splash.png");
            splashScreen.Show(true);
            Thread.Sleep(3333);
            
            var application = new App();
            application.InitializeComponent();
            application.Run();
        }
    }
}