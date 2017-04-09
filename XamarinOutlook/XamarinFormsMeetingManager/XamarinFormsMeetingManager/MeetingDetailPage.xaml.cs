//Copyright (c) Microsoft. All rights reserved. Licensed under the MIT license.
//See LICENSE in the project root for license information.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Graph;
using Xamarin.Forms;
using XamarinFormsMeetingManager.Helpers;

namespace XamarinFormsMeetingManager
{
    public partial class MeetingDetailPage : ContentPage
    {
        public Event ThisMeeting;
        public DateTime ThisMeetingDate;
        public MeetingDetailPage(Event selectedMeeting, DateTime selectedMeetingDate)
        {
            InitializeComponent();
            ThisMeeting = selectedMeeting;
            ThisMeetingDate = selectedMeetingDate;
            EmailButton.Text = SampleStrings.EmailButtonText;
            RunLateButton.Text = SampleStrings.RunLateButtonText;
            EditButton.Text = SampleStrings.EditButtonText;
            Subject.Text = selectedMeeting.Subject;
            DeleteButton.Text = SampleStrings.DeleteButtonText;
            Date.Text = selectedMeetingDate.Date.ToString("D");
            if (selectedMeeting.IsAllDay.GetValueOrDefault())
            {
                Time.Text = SampleStrings.AllDayMeeting;
            }
            else
            {
                Time.Text = selectedMeeting.Start.DateTime + " - " + selectedMeeting.End.DateTime;
            }
            Location.Text = SampleStrings.LocationHeader + selectedMeeting.Location.DisplayName;
            string meetingBody = selectedMeeting.Body.Content;
            if (!String.IsNullOrEmpty(selectedMeeting.Body.ContentType.ToString()) && selectedMeeting.Body.ContentType == BodyType.Html)
            {
                meetingBody = Regex.Replace(meetingBody, @"<[^>]+>|&nbsp;", "").Trim();
            }
            Body.Text = meetingBody.Length <= 200 ? meetingBody : meetingBody.Substring(0, 200);
            var attendeesList = (List<Attendee>)selectedMeeting.Attendees;
            AttendeesView.ItemsSource = attendeesList;
            BodyHeader.Text = SampleStrings.DescriptionHeaderText;
        }

        async void OnEmailClicked(object sender, EventArgs args)
        {
            bool isReplyAll = false;
            var action = await DisplayActionSheet(SampleStrings.EmailTypePrompt, null, null, SampleStrings.ReplyAllEmailType, SampleStrings.ForwardEmailType);
            if (action == SampleStrings.ReplyAllEmailType)
            {
                isReplyAll = true;
            }

            await Navigation.PushAsync(new EmailComposePage(ThisMeeting, isReplyAll));
        }

        async void OnRunLateClicked(object sender, EventArgs args)
        {
            await Navigation.PushAsync(new EmailComposePage(ThisMeeting, true, SampleStrings.RunningLate));
        }

        async void OnEditClicked(object sender, EventArgs args)
        {
            await Navigation.PushAsync(new EditMeetingPage(ThisMeeting, ThisMeetingDate));
        }

        async void OnDeleteClicked(object sender, EventArgs args)
        {
            await CalendarHelper.DeleteEventAsync(ThisMeeting.Id);
            await Navigation.PopAsync();
        }
    }
}
