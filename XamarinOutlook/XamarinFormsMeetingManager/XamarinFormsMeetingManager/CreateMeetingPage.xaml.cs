//Copyright (c) Microsoft. All rights reserved. Licensed under the MIT license.
//See LICENSE in the project root for license information.

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Microsoft.Graph;
using XamarinFormsMeetingManager.Helpers;

namespace XamarinFormsMeetingManager
{
    public partial class CreateMeetingPage : ContentPage
    {

        public Event NewMeeting;
        public DateTime SelectedDate = new DateTime();
        public Dictionary<string, string> UserNameToEmail = new Dictionary<string, string>();
        bool IsAllDayMeeting = false;
        ExpandableEditor Description = new ExpandableEditor();

        public CreateMeetingPage(DateTime selectedDate)
        {
            InitializeComponent();
            //Insert the Expandable editor control directly beneath its header.
            CreateMeetingMainStack.Children.Insert(16, Description);
            Description.TextChanged += OnTextChanged;
            Description.HorizontalOptions = LayoutOptions.FillAndExpand;
            Description.VerticalOptions = LayoutOptions.EndAndExpand;
            Description.BackgroundColor = Color.Gray;
            Description.WidthRequest = 500;
            Description.HeightRequest = 200;
            SelectedDate = selectedDate;
            DateHeader.Text = SampleStrings.DateHeaderText;
            DateText.Text = SelectedDate.Date.Date.ToString("D");
            SaveButton.Text = SampleStrings.SaveButtonText;
            CancelButton.Text = SampleStrings.CancelButtonText;
            SubjectHeader.Text = SampleStrings.SubjectHeaderText;
            StartTimeHeader.Text = SampleStrings.StartTimeHeaderText;
            EndTimeHeader.Text = SampleStrings.EndTimeHeaderText;
            DescriptionHeader.Text = SampleStrings.DescriptionHeaderText;
            LocationHeader.Text = SampleStrings.LocationHeaderText;
            DescriptionHeader.Text = SampleStrings.DescriptionHeaderText;
            AttendeesHeader.Text = SampleStrings.AttendeesHeaderText;
            AddAttendeeButton.Text = SampleStrings.AddAttendeeButtonText;
            AllDaySwitchText.Text = SampleStrings.AllDayMeeting;
            AttendeesBox.Text = "";
        }

        async void OnSaveClicked(Object sender, EventArgs args)
        {
            DateTime startDateTime;
            DateTime endDateTime;

            if (IsAllDayMeeting)
            {
                startDateTime = SelectedDate.Date;
                endDateTime = SelectedDate.Date.AddDays(1);
            }
            else
            {
                startDateTime = (SelectedDate + StartTimePicker.Time).ToUniversalTime();
                endDateTime = (SelectedDate + EndTimePicker.Time).ToUniversalTime();
            }

            var eventId = await CalendarHelper.CreateEventAsync(startDateTime, endDateTime, Description.Text, Location.Text, Subject.Text, AttendeesBox.Text, IsAllDayMeeting);

            if (eventId != null)
            {
                await Navigation.PopAsync();
            }
            else
            {
                await DisplayAlert(SampleStrings.ErrorText, SampleStrings.CreateMeetingFailed, SampleStrings.OKButtonText);
            }
        }

        async void OnAddAttendeeButtonClicked(object sender, EventArgs args)
        {
            var users = await CalendarHelper.GetUsersAsync();
            var userNameToEmail = new Dictionary<string, string>();
            foreach (User user in users)
            {
                if (!user.DisplayName.Contains("Conf Room") && !userNameToEmail.ContainsKey(user.DisplayName))
                {
                    userNameToEmail.Add(user.DisplayName, user.UserPrincipalName);
                }
            }



            var selectedAttendee = await DisplayActionSheet(SampleStrings.AddAttendeePrompt, null, null, userNameToEmail.Keys.ToArray());

            AttendeesBox.Text += userNameToEmail[selectedAttendee] + ";";
            AttendeesHeader.IsVisible = true;


        }

        void OnAllDayChanged(object sender, ToggledEventArgs args)
        {
            if (args.Value)
            {
                IsAllDayMeeting = true;
                StartTimePicker.IsEnabled = false;
                EndTimePicker.IsEnabled = false;
            }
            else
            {
                IsAllDayMeeting = false;
                StartTimePicker.IsEnabled = true;
                EndTimePicker.IsEnabled = true;
            }
        }

        async void OnCancelClicked(object sender, EventArgs args)
        {
            await Navigation.PopAsync();
        }

        private void OnTextChanged(Object sender, TextChangedEventArgs e)
        {
            Description.InvalidateLayout();
        }


    }
}
