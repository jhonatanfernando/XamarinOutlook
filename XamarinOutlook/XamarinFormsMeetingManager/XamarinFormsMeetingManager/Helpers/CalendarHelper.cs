//Copyright (c) Microsoft. All rights reserved. Licensed under the MIT license.
//See LICENSE in the project root for license information.

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Graph;

namespace XamarinFormsMeetingManager.Helpers
{
    public static class CalendarHelper
    {

        public static async Task<IUserCalendarViewCollectionPage> GetDayEventsAsync(string startDateTime, string endDateTime)
        {
            IUserCalendarViewCollectionPage events = null;

            try
            {
                var graphClient = AuthenticationHelper.GetAuthenticatedClient();

                var options = new List<Option>();          
                options.Add(new QueryOption("StartDateTime", startDateTime));
                options.Add(new QueryOption("EndDateTime", endDateTime));
                
                events = await graphClient.Me.CalendarView.Request(options).OrderBy("start/DateTime").GetAsync();
            }

            catch (ServiceException e)
            {
                Debug.WriteLine(SampleStrings.GetEventsFailed + e.Error.Message);
                return null;
            }

            return events;
        }


        public static async Task<bool> DeleteEventAsync(string eventId)
        {
            bool isDeleted = false;

            var graphClient = AuthenticationHelper.GetAuthenticatedClient();

            await graphClient.Me.Events[eventId].Request().DeleteAsync();
            isDeleted = true;

            return isDeleted;
        }

        // Creates a new event in the signed-in user's tenant.
        public static async Task<string> CreateEventAsync(DateTime startDateTime, DateTime endDateTime, string eventDescription, string eventLocation, string eventSubject, string eventAttendees, bool isAllDay)
        {
            string createdEventId = null;

            //Prepare the List of attendees
            // Prepare the recipient list
            string[] splitter = { ";" };
            var splitRecipientsString = eventAttendees.Split(splitter, StringSplitOptions.RemoveEmptyEntries);
            List<Attendee> attendeesList = new List<Attendee>();
            foreach (string attendee in splitRecipientsString)
            {
                attendeesList.Add(new Attendee { EmailAddress = new EmailAddress { Address = attendee.Trim() }, Type = AttendeeType.Required });
            }


            //Event body
            var eventBody = new ItemBody();
            eventBody.Content = eventDescription;
            eventBody.ContentType = BodyType.Text;

            //Event start and end time
            var eventStartTime = new DateTimeTimeZone();
            eventStartTime.DateTime = startDateTime.ToString("o");
            eventStartTime.TimeZone = "UTC";
            var eventEndTime = new DateTimeTimeZone();
            eventEndTime.TimeZone = "UTC";
            eventEndTime.DateTime = endDateTime.ToString("o");

            //Create an event to add to the events collection

            var location = new Location();
            location.DisplayName = eventLocation;
            var newEvent = new Event();
            newEvent.Subject = eventSubject;
            newEvent.Location = location;
            newEvent.Attendees = attendeesList;
            newEvent.Body = eventBody;
            newEvent.Start = eventStartTime;
            newEvent.End = eventEndTime;

            if ( isAllDay)
            {
                newEvent.IsAllDay = true;
            }
            
            try
            {
                var graphClient = AuthenticationHelper.GetAuthenticatedClient();
                var createdEvent = await graphClient.Me.Events.Request().AddAsync(newEvent);
                createdEventId = createdEvent.Id;

            }

            catch (ServiceException e)
            {
                Debug.WriteLine(SampleStrings.CreateMeetingFailedDebug + e.Error.Message);
                return null;
            }

            return createdEventId;

        }

        // Returns all of the users in the directory of the signed-in user's tenant. 
        public static async Task<IGraphServiceUsersCollectionPage> GetUsersAsync()
        {
            IGraphServiceUsersCollectionPage users = null;

            try
            {
                var graphClient = AuthenticationHelper.GetAuthenticatedClient();
                users = await graphClient.Users.Request().GetAsync();

                return users;

            }

            catch (ServiceException e)
            {
                Debug.WriteLine(SampleStrings.GetUsersFailedDebug + e.Error.Message);
                return null;
            }


        }


        // Updates an existing event in the signed-in user's tenant.
        public static async Task<bool> UpdateEventAsync(string eventId, string eventAttendees, string eventDescription, string eventLocation, string eventSubject, DateTime startDateTime, DateTime endDateTime, bool isAllDayMeeting)
        {
            bool eventUpdated = false;

            try
            {
                var graphClient = AuthenticationHelper.GetAuthenticatedClient();
                var eventToUpdate = new Event();

                //Prepare the List of attendees
                // Prepare the recipient list
                string[] splitter = { ";" };
                var splitRecipientsString = eventAttendees.Split(splitter, StringSplitOptions.RemoveEmptyEntries);
                List<Attendee> attendeesList = new List<Attendee>();
                foreach (string attendee in splitRecipientsString)
                {
                    attendeesList.Add(new Attendee { EmailAddress = new EmailAddress { Address = attendee.Trim() }, Type = AttendeeType.Required });
                }

                //Event body
                var eventBody = new ItemBody();
                eventBody.Content = eventDescription;
                eventBody.ContentType = BodyType.Text;

                var eventStartTime = new DateTimeTimeZone();
                var eventEndTime = new DateTimeTimeZone();
                //Event start and end time

                eventStartTime.DateTime = startDateTime.ToString("o");
                eventStartTime.TimeZone = "UTC";                   
                eventEndTime.TimeZone = "UTC";
                eventEndTime.DateTime = endDateTime.ToString("o");


                //Create an event to add to the events collection

                var location = new Location();
                location.DisplayName = eventLocation;

                eventToUpdate.Subject = eventSubject;
                eventToUpdate.Location = location;
                eventToUpdate.Attendees = attendeesList;
                eventToUpdate.Body = eventBody;
                eventToUpdate.Start = eventStartTime;
                eventToUpdate.End = eventEndTime;


                var updatedEvent = await graphClient.Me.Events[eventId].Request().UpdateAsync(eventToUpdate);


                if (updatedEvent != null)
                {
                    eventUpdated = true;
                }

            }

            catch (ServiceException e)
            {
                Debug.WriteLine(SampleStrings.UpdateMeetingFailedDebug + e.Error.Message);
                eventUpdated = false;
            }

            return eventUpdated;

        }

        // Updates the recurrence of an existing event in the signed-in user's tenant.
        public static async Task<bool> UpdateEventRecurrenceAsync(string eventId, PatternedRecurrence patternedRecurrence)
        {
            bool eventUpdated = false;

            try
            {
                var graphClient = AuthenticationHelper.GetAuthenticatedClient();
                var eventToUpdate = new Event();

                eventToUpdate.Recurrence = patternedRecurrence;


                var updatedEvent = await graphClient.Me.Events[eventId].Request().UpdateAsync(eventToUpdate);

                if (updatedEvent != null)
                {
                    eventUpdated = true;
                }

            }

            catch (ServiceException e)
            {
                Debug.WriteLine(SampleStrings.UpdateMeetingFailedDebug + e.Error.Message);
                eventUpdated = false;
            }

            return eventUpdated;

        }

        //Converts DateTime strings to local time and then strips them for display.
        public static void ConvertEventDates(IUserCalendarViewCollectionPage events)
        {
            foreach (var eventToConvert in events)
            {
                //Convert Start datetime
                if (eventToConvert.IsAllDay.GetValueOrDefault())
                {
                    eventToConvert.Start.DateTime = SampleStrings.AllDayMeeting;
                }
                else
                {
                    eventToConvert.Start.DateTime = StripDateTimeForDisplay(eventToConvert.Start.DateTime);
                    eventToConvert.End.DateTime = StripDateTimeForDisplay(eventToConvert.End.DateTime);
                }


            }

        }


        //Converts UTC times returned from service to local time.
        //Works around the fact that we can't get consistent, normalized time zone
        //strings across all platforms in Xamarin.
        public static DateTime ConvertDateTimeString(string dateTime)
        {
            string[] dateElements = dateTime.Split('/');
            var month = Convert.ToInt32(dateElements[0]);
            var day = Convert.ToInt32(dateElements[1]);
            var splitDateFromTime = dateElements[2].Split(' ');
            var year = Convert.ToInt32(splitDateFromTime[0]);
            var timeElements = splitDateFromTime[1].Split(':');
            var hour = Convert.ToInt32(timeElements[0]);
            var minutes = Convert.ToInt32(timeElements[1]);
            var seconds = Convert.ToInt32(timeElements[2]);

            var dateTimeToConvert = new DateTime(year, month, day, hour, minutes, seconds);
            var convertedDateTime = dateTimeToConvert.ToLocalTime();
            return convertedDateTime;


        }

        //Strings the DateTime objects into displayable strings.
        public static string StripDateTimeForDisplay(string dateTime)
        {
            var convertedDateTime = ConvertDateTimeString(dateTime);
            var timeString = convertedDateTime.TimeOfDay.ToString("hh\\:mm");

            return timeString;


        }

    }
}
