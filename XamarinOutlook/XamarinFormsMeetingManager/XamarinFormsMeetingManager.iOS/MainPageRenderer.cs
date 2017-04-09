using Microsoft.Identity.Client;
using System;
using System.Collections.Generic;
using System.Text;
using Xamarin.Forms;
using Xamarin.Forms.Platform.iOS;
using XamarinFormsMeetingManager;
using XamarinFormsMeetingManager.iOS;

[assembly: ExportRenderer(typeof(MainPage), typeof(MainPageRenderer))]

namespace XamarinFormsMeetingManager.iOS
{
    class MainPageRenderer : PageRenderer
    {
        MainPage page;
        protected override void OnElementChanged(VisualElementChangedEventArgs e)
        {
            base.OnElementChanged(e);
            page = e.NewElement as MainPage;
        }
        public override void ViewDidLoad()
        {
            base.ViewDidLoad();
            page.platformParameters = new PlatformParameters(this);
        }
    }
}
