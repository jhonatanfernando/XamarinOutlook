//Copyright (c) Microsoft. All rights reserved. Licensed under the MIT license.
//See LICENSE in the project root for license information.

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

using Xamarin.Forms;
using Microsoft.Graph;
using XamarinFormsMeetingManager.Helpers;

namespace XamarinFormsMeetingManager
{
    public partial class EmailComposePage : ContentPage
    {
        Event MeetingToEmail;
        ExpandableEditor BodyBox = new ExpandableEditor();
        public EmailComposePage(Event replyToMeeting, bool isReplyAll, string body = "")
        {
            InitializeComponent();
            MainStack.Children.Add(BodyBox);
            BodyBox.HorizontalOptions = LayoutOptions.FillAndExpand;
            BodyBox.VerticalOptions = LayoutOptions.EndAndExpand;
            BodyBox.BackgroundColor = Color.Gray;
            BodyBox.WidthRequest = 500;
            BodyBox.HeightRequest = 200;
            BodyBox.TextChanged += OnTextChanged;
            MeetingToEmail = replyToMeeting;
            SubjectBox.Text = MeetingToEmail.Subject;
            SubjectHeader.Text = SampleStrings.SubjectHeaderText;
            RecipientsHeader.Text = SampleStrings.RecipientsText;
            DescriptionHeader.Text = SampleStrings.DescriptionHeaderText;
            BodyBox.Text = body + Regex.Replace(MeetingToEmail.Body.Content, @"<[^>]+>|&nbsp;", "").Trim();
            if ( !isReplyAll)
            {
                RecipientsBox.IsVisible = true;
                RecipientsHeader.IsVisible = true;
            }

        }

        public async void OnEmailClicked(object sender, EventArgs args)
        {
            bool emailSent = false;

            if (!RecipientsBox.IsVisible)
            {
                emailSent = await EmailHelper.SendReplyAllMessageAsync(BodyBox.Text, MeetingToEmail.Subject, MeetingToEmail.IsOrganizer);
            }
            else
            {
                if (String.IsNullOrEmpty(RecipientsBox.Text))
                {
                    await DisplayAlert(SampleStrings.ErrorText, SampleStrings.EmptyRecipientsListMessage, SampleStrings.OKButtonText);
                }
                else
                {
                    emailSent = await EmailHelper.SendForwardMessageAsync(BodyBox.Text, MeetingToEmail.Subject, RecipientsBox.Text, MeetingToEmail.IsOrganizer);
                }
            }

            if (emailSent)
            {
                await Navigation.PopAsync();
            }
            else
            {
                await DisplayAlert(SampleStrings.ErrorText, SampleStrings.SendMessageFailedUser, SampleStrings.OKButtonText);
            }
 
        }

        private void OnTextChanged(Object sender, TextChangedEventArgs e)
        {
            BodyBox.InvalidateLayout();
        }

    }
}
