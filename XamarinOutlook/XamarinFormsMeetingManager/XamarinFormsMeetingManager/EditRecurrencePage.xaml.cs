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
    public partial class EditRecurrencePage : ContentPage
    {
        public Event ThisMeeting;
        public DateTime ThisMeetingDate;
        public RecurrencePatternType UpdateRecurrencePatternType = RecurrencePatternType.Weekly;
        public Dictionary<string, Microsoft.Graph.DayOfWeek> StringToDayOfWeek = new Dictionary<string, Microsoft.Graph.DayOfWeek>();
        public EditRecurrencePage(Event meetingToUpdate, DateTime meetingToUpdateDate)
        {
            InitializeComponent();
            StringToDayOfWeek.Add(SampleStrings.SundayText, Microsoft.Graph.DayOfWeek.Sunday);
            StringToDayOfWeek.Add(SampleStrings.MondayText, Microsoft.Graph.DayOfWeek.Monday);
            StringToDayOfWeek.Add(SampleStrings.TuesdayText, Microsoft.Graph.DayOfWeek.Tuesday);
            StringToDayOfWeek.Add(SampleStrings.WednesdayText, Microsoft.Graph.DayOfWeek.Wednesday);
            StringToDayOfWeek.Add(SampleStrings.ThursdayText, Microsoft.Graph.DayOfWeek.Thursday);
            StringToDayOfWeek.Add(SampleStrings.FridayText, Microsoft.Graph.DayOfWeek.Friday);
            StringToDayOfWeek.Add(SampleStrings.SaturdayText, Microsoft.Graph.DayOfWeek.Saturday);
            ThisMeeting = meetingToUpdate;
            ThisMeetingDate = meetingToUpdateDate;
            SaveButton.Text = SampleStrings.SaveButtonText;
            CancelButton.Text = SampleStrings.CancelButtonText;
            RecurrenceTypeButton.Text = SampleStrings.RecurrenceTypeText;
            StartOnLabel.Text = SampleStrings.StartOnText;
            EndOnLabel.Text = SampleStrings.EndByText;
            EndAfterLabel.Text = SampleStrings.EndAfterText;
            OccurencesLabel.Text = SampleStrings.OccurencesText;

            DaysOfWeekButton.Text = SampleStrings.SelectButtonText;
            EveryWeekLabel.Text = SampleStrings.EveryText + " ";
            WeeklyLabel.Text = SampleStrings.WeeklyLabelText + " ";
            WeeklyDayOfWeek.Text = SampleStrings.SelectDayText;
            WeeklyStack.IsVisible = true;
        }

        async void OnSaveClicked(Object sender, EventArgs args)
        {
            var newRecurrence = new PatternedRecurrence();
            Date startDate;
            Date endDate;

            var recurrenceRange = new RecurrenceRange();
            var recurrencePattern = new RecurrencePattern();
            var daysOfWeek = new List<Microsoft.Graph.DayOfWeek>();

            if (UpdateRecurrencePatternType == RecurrencePatternType.Daily)
            {
                daysOfWeek.Add(Microsoft.Graph.DayOfWeek.Monday);
                daysOfWeek.Add(Microsoft.Graph.DayOfWeek.Tuesday);
                daysOfWeek.Add(Microsoft.Graph.DayOfWeek.Wednesday);
                daysOfWeek.Add(Microsoft.Graph.DayOfWeek.Thursday);
                daysOfWeek.Add(Microsoft.Graph.DayOfWeek.Friday);
                recurrencePattern.Type = RecurrencePatternType.Daily;
                recurrencePattern.FirstDayOfWeek = Microsoft.Graph.DayOfWeek.Monday;
                recurrencePattern.DaysOfWeek = daysOfWeek;
                recurrencePattern.Interval = 1;
            }
            else if (UpdateRecurrencePatternType == RecurrencePatternType.Weekly)
            {
                daysOfWeek.Add(StringToDayOfWeek[WeeklyDayOfWeek.Text]);
                recurrencePattern.Type = RecurrencePatternType.Weekly;
                recurrencePattern.DaysOfWeek = daysOfWeek;
                recurrencePattern.Interval = Convert.ToInt32(NumberOfWeeksEditor.Text);
            }
            else if (UpdateRecurrencePatternType == RecurrencePatternType.AbsoluteMonthly)
            {
                recurrencePattern.Type = RecurrencePatternType.AbsoluteMonthly;
                recurrencePattern.DayOfMonth = Convert.ToInt32(MonthDayNumberLabel.Text);
                recurrencePattern.Interval = Convert.ToInt32(MonthIntervalLabel.Text);
            }


            if (String.IsNullOrEmpty(EndAfterOccurrencesEntry.Text))
            {
                recurrenceRange.Type = RecurrenceRangeType.EndDate;
                startDate = new Date(StartOnDatePicker.Date.Year, StartOnDatePicker.Date.Month, StartOnDatePicker.Date.Day);
                endDate = new Date(EndOnDatePicker.Date.Year, EndOnDatePicker.Date.Month, EndOnDatePicker.Date.Day);
                recurrenceRange.StartDate = startDate;
                recurrenceRange.EndDate = endDate;                
            }
            else
            {
                startDate = new Date(ThisMeetingDate.Date.Year, ThisMeetingDate.Date.Month, ThisMeetingDate.Date.Day);
                recurrenceRange.StartDate = startDate;
                recurrenceRange.Type = RecurrenceRangeType.Numbered;
                recurrenceRange.NumberOfOccurrences = Convert.ToInt32(EndAfterOccurrencesEntry.Text);
            }

            newRecurrence.Pattern = recurrencePattern;
            newRecurrence.Range = recurrenceRange;

            var eventUpdated = await CalendarHelper.UpdateEventRecurrenceAsync(ThisMeeting.Id, newRecurrence);

            if (eventUpdated)
            {
                await Navigation.PopAsync();
            }
            else
            {
                await DisplayAlert(SampleStrings.ErrorText, SampleStrings.CreateMeetingFailed, SampleStrings.OKButtonText);
            }

        }

        async void OnRecurrenceTypeClicked(Object sender, EventArgs args)
        {
            var recurrenceType = await DisplayActionSheet(SampleStrings.RecurrenceTypeChoiceText, null, null, SampleStrings.DailyText, SampleStrings.WeeklyText, SampleStrings.MonthlyText);
            if ( recurrenceType == SampleStrings.DailyText)
            {
                UpdateRecurrencePatternType = RecurrencePatternType.Daily;
                DailylLabel.Text = SampleStrings.EveryWeekdayText;
                DailylLabel.IsVisible = true;
                WeeklyStack.IsVisible = false;
                MonthlyStack.IsVisible = false;
            }
            else if ( recurrenceType == SampleStrings.WeeklyText)
            {
                UpdateRecurrencePatternType = RecurrencePatternType.Weekly;
                DaysOfWeekButton.Text = SampleStrings.SelectButtonText;
                EveryWeekLabel.Text = SampleStrings.EveryText + " ";
                WeeklyLabel.Text = SampleStrings.WeeklyLabelText + " ";
                WeeklyDayOfWeek.Text = SampleStrings.SelectDayText;
                WeeklyStack.IsVisible = true;
                DailylLabel.IsVisible = false;
                MonthlyStack.IsVisible = false;
            }
            else if ( recurrenceType == SampleStrings.MonthlyText)
            {
                UpdateRecurrencePatternType = RecurrencePatternType.AbsoluteMonthly;
                MonthlyStack.IsVisible = true;
                MonthDayLabel.Text = SampleStrings.DayText + " ";
                OfEveryLabel.Text = SampleStrings.OfEveryText + " ";
                MonthSingPluralLabel.Text = SampleStrings.MonthlyLabelText;
                DailylLabel.IsVisible = false;
                WeeklyStack.IsVisible = false;
            }
            else
            {
                UpdateRecurrencePatternType = RecurrencePatternType.AbsoluteYearly;
                DailylLabel.IsVisible = false;
                WeeklyStack.IsVisible = false;
                MonthlyStack.IsVisible = false;
            }

        }

        async void OnDayOfWeekClicked(Object sender, EventArgs args)
        {
            var dayOfWeek = await DisplayActionSheet(SampleStrings.SelectDayText, null, null, SampleStrings.SundayText, SampleStrings.MondayText, SampleStrings.TuesdayText, SampleStrings.WednesdayText, SampleStrings.ThursdayText, SampleStrings.FridayText, SampleStrings.SaturdayText);
            WeeklyDayOfWeek.Text = dayOfWeek;

        }

        void OnEndAfterOccurencesTextChanged(Object sender, TextChangedEventArgs args)
        {
            if (!String.IsNullOrEmpty(EndAfterOccurrencesEntry.Text) )
            {
                StartOnDatePicker.IsEnabled = false;
                EndOnDatePicker.IsEnabled = false;
            }
            else
            {
                StartOnDatePicker.IsEnabled = true;
                EndOnDatePicker.IsEnabled = true;
            }
        }

        async void OnCancelClicked(object sender, EventArgs args)
        {
            await Navigation.PopAsync();
        }

    }
}
