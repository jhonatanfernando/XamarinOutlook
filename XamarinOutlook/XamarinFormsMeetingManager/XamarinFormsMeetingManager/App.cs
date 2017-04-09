//Copyright (c) Microsoft. All rights reserved. Licensed under the MIT license.
//See LICENSE in the project root for license information.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Xamarin.Forms;
using Microsoft.Identity.Client;

namespace XamarinFormsMeetingManager
{
    public class App : Application
    {

        public static PublicClientApplication IdentityClientApp = null;
        public static string ClientID = "fb20c5e7-e17d-439a-8c07-cdfe7f95c41c";
        public static string[] Scopes = {
                        "https://graph.microsoft.com/Mail.Send",
                        "https://graph.microsoft.com/Calendars.ReadWrite",
                        "https://graph.microsoft.com/Mail.ReadWrite"
                    };

        public App()
        {
            // The root page of your application
            IdentityClientApp = new PublicClientApplication(ClientID);
            MainPage = new NavigationPage(new MainPage());
        }

        protected override void OnStart()
        {
            // Handle when your app starts
        }

        protected override void OnSleep()
        {
            // Handle when your app sleeps
        }

        protected override void OnResume()
        {
            // Handle when your app resumes
        }
    }
}
