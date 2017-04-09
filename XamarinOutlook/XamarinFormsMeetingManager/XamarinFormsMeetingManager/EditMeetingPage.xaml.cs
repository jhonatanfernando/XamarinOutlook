//Copyright (c) Microsoft. All rights reserved. Licensed under the MIT license.
//See LICENSE in the project root for license information.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Microsoft.Graph;
using XamarinFormsMeetingManager.Helpers;

namespace XamarinFormsMeetingManager
{
    public partial class EditMeetingPage : ContentPage
    {
        public Event ThisMeeting;
        public DateTime ThisMeetingDate;
        public Dictionary<string, string> UserNameToEmail = new Dictionary<string, string>();
        bool IsAllDayMeeting = false;
        ExpandableEditor Description = new ExpandableEditor();
        public EditMeetingPage(Event meetingToEdit, DateTime meetingToEditDate)
        {
            InitializeComponent();
            //Insert the Expandable editor control directly beneath its header.
            EditMeetingMainStack.Children.Insert(14, Description);
            Description.TextChanged += OnTextChanged;
            Description.HorizontalOptions = LayoutOptions.FillAndExpand;
            Description.VerticalOptions = LayoutOptions.EndAndExpand;
            Description.BackgroundColor = Color.Gray;
            Description.WidthRequest = 500;
            Description.HeightRequest = 200;
            ThisMeeting = meetingToEdit;
            ThisMeetingDate = meetingToEditDate;
            DateHeader.Text = SampleStrings.DateHeaderText;
            DatePicker.Date = meetingToEditDate.Date;
            SaveButton.Text = SampleStrings.SaveButtonText;
            CancelButton.Text = SampleStrings.CancelButtonText;
            SubjectHeader.Text = SampleStrings.SubjectHeaderText;
            Subject.Text = ThisMeeting.Subject;
            StartTimeHeader.Text = SampleStrings.StartTimeHeaderText;
            EndTimeHeader.Text = SampleStrings.EndTimeHeaderText;
            RecurrenceButton.Text = SampleStrings.RecurrenceButtonText;
            if (ThisMeeting.IsAllDay.GetValueOrDefault())
            {
                IsAllDayMeeting = true;
                StartTimePicker.IsEnabled = false;
                EndTimePicker.IsEnabled = false;
            }
            else
            {
                StartTimePicker.Time = TimeSpan.Parse(ThisMeeting.Start.DateTime);
                EndTimePicker.Time = TimeSpan.Parse(ThisMeeting.End.DateTime);
            }           
            DescriptionHeader.Text = SampleStrings.DescriptionHeaderText;
            Description.Text = ThisMeeting.Body.Content;
            LocationHeader.Text = SampleStrings.LocationHeaderText;
            Location.Text = ThisMeeting.Location.DisplayName;
            AttendeesHeader.Text = SampleStrings.AttendeesHeaderText;
            AddAttendeeButton.Text = SampleStrings.AddAttendeeButtonText;
            foreach ( var attendee in ThisMeeting.Attendees)
            {
                AttendeesBox.Text += attendee.EmailAddress.Address + ";";
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


        }

        async void OnSaveClicked(Object sender, EventArgs args)
        {
            DateTime startDateTime;
            DateTime endDateTime;

            if (IsAllDayMeeting)
            {
                startDateTime = ThisMeetingDate.Date;
                endDateTime = ThisMeetingDate.Date.AddDays(1);
            }
            else
            {
                startDateTime = (DatePicker.Date.Date + StartTimePicker.Time).ToUniversalTime();
                endDateTime = (DatePicker.Date.Date + EndTimePicker.Time).ToUniversalTime();
            }

            var eventUpdated = await CalendarHelper.UpdateEventAsync(ThisMeeting.Id, AttendeesBox.Text, Description.Text, Location.Text, Subject.Text, startDateTime, endDateTime, IsAllDayMeeting);

            if (eventUpdated)
            {
                await Navigation.PopAsync();
            }
            else
            {
                await DisplayAlert(SampleStrings.ErrorText, SampleStrings.CreateMeetingFailed, SampleStrings.OKButtonText);
            }
        }

        async void OnRecurrenceButtonClicked(object sender, EventArgs args)
        {
            await Navigation.PushAsync(new EditRecurrencePage(ThisMeeting, ThisMeetingDate));
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
