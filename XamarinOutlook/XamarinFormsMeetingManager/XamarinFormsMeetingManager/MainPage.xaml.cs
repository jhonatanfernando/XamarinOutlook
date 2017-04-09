//Copyright (c) Microsoft. All rights reserved. Licensed under the MIT license.
//See LICENSE in the project root for license information.

using System;
using System.Diagnostics;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Microsoft.Graph;
using Microsoft.Identity.Client;
using XamarinFormsMeetingManager.Helpers;

namespace XamarinFormsMeetingManager
{
    public partial class MainPage : ContentPage
    {

        public IPlatformParameters platformParameters { get; set; }

        public MainPage()
        {
            InitializeComponent();
        }

        protected override void OnAppearing()
        {
            App.IdentityClientApp.PlatformParameters = platformParameters;

            // Developer code - if you haven't registered the app yet, we warn you. 
            if (App.ClientID == "")
            {
                Debug.WriteLine(SampleStrings.RegisterWarning);
            }
            else if (MeetingView.ItemsSource == null)
            {
                DateSelectHeader.Text = SampleStrings.DateSelectHeader;
                MeetingView.Header = SampleStrings.MeetingViewHeader;
                SignoutButton.Text = SampleStrings.SignoutButtonText;
                CreateButton.Text = SampleStrings.CreateButtonText;
                ConnectButton.Text = SampleStrings.ConnectButtonText;
                RefreshButton.Text = SampleStrings.RefreshButtonText;
            }

        }

        async void OnDateSelected(object sender, DateChangedEventArgs args)
        {
            var startDate = args.NewDate.Date.ToUniversalTime();
            var endDate = args.NewDate.Date.ToUniversalTime().AddDays(1).AddTicks(-1);
            var events = await CalendarHelper.GetDayEventsAsync(startDate.ToString("o"), endDate.ToString("o"));
            CalendarHelper.ConvertEventDates(events);
            var selectedDateEvents = new List<Event>(events);

            if (selectedDateEvents.Count > 0)
            {
                MeetingView.ItemsSource = selectedDateEvents;
            }
            else
            {
                // Apparent bug in Xamarin Forms: Setting the ItemsSource property to an empty list throws an exception.
                var emptyArray = new List<string>();
                emptyArray.Add("");
                MeetingView.ItemsSource = emptyArray;
            }

        }

        async void OnItemSelected(object sender, SelectedItemChangedEventArgs args)
        {
            var selectedMeeting = (Event)args.SelectedItem;
            await Navigation.PushAsync(new MeetingDetailPage(selectedMeeting, DatePicker.Date));
        }

        async void OnCreateClicked(object sender, EventArgs args)
        {
            var selectedDate = DatePicker.Date.ToUniversalTime();
            await Navigation.PushAsync(new CreateMeetingPage(selectedDate));
        }

        async void OnConnectClicked(object sender, EventArgs args)
        {
            var graphClient = AuthenticationHelper.GetAuthenticatedClient();
            var startDate = DateTime.Today.Date.ToUniversalTime();
            var endDate = DateTime.Today.Date.ToUniversalTime().AddDays(1).AddTicks(-1);
            var events = await CalendarHelper.GetDayEventsAsync(startDate.ToString("o"), endDate.ToString("o"));

            CalendarHelper.ConvertEventDates(events);

            var selectedDateEvents = new List<Event>(events);


            MeetingView.ItemsSource = selectedDateEvents;
            CreateButton.IsEnabled = true;
            RefreshButton.IsEnabled = true;
            DateSelectHeader.IsVisible = true;
            DatePicker.IsEnabled = true;
            MeetingView.IsVisible = true;
            MeetingView.IsEnabled = true;
            ConnectButton.IsEnabled = false;
        }

        async void OnRefreshClicked(object sender, EventArgs args)
        {
            var graphClient = AuthenticationHelper.GetAuthenticatedClient();

            //Since it's not possible to get standard, normalized time zone strings across all platforms in Xamarin,
            //we retrieve dates in UTC time and then convert them to local time.
            var startDate = DatePicker.Date.Date.ToUniversalTime();
            var endDate = DatePicker.Date.Date.ToUniversalTime().AddDays(1).AddTicks(-1);
            var events = await CalendarHelper.GetDayEventsAsync(startDate.ToString("o"), endDate.ToString("o"));

            CalendarHelper.ConvertEventDates(events);
            var selectedDateEvents = new List<Event>(events);


            MeetingView.ItemsSource = selectedDateEvents;

        }

        void OnSignoutClicked(object sender, EventArgs e)
        {
            AuthenticationHelper.SignOut();
            CreateButton.IsEnabled = false;
            RefreshButton.IsEnabled = false;
            DateSelectHeader.IsVisible = false;
            DatePicker.IsEnabled = false;
            MeetingView.IsEnabled = false;
            ConnectButton.IsEnabled = true;
        }
    }
}
