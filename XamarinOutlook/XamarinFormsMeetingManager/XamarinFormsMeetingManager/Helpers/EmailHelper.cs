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
    public static class EmailHelper
    {
        public static async Task<bool> SendReplyAllMessageAsync(string body, string subject, bool? isOrganizer)
        {
            bool emailSent = false;

            try
            {
                var graphClient = AuthenticationHelper.GetAuthenticatedClient();
                // Get meeting message from user's Sent Items folder or Inbox
                IMailFolderMessagesCollectionPage eventMessages;

                if (isOrganizer.GetValueOrDefault())
                {
                    eventMessages = await graphClient.Me.MailFolders.SentItems.Messages.Request().Filter("Subject eq '" + subject + "'").GetAsync();
                }
                else
                {
                    eventMessages = await graphClient.Me.MailFolders.Inbox.Messages.Request().Filter("Subject eq '" + subject + "'").GetAsync();
                }

                if (eventMessages.Count > 0)
                {
                    var messageToReplyAll = eventMessages[0];

                    // Reply all to message
                    var replyMessage = await graphClient.Me.Messages[messageToReplyAll.Id].CreateReplyAll().Request().PostAsync();

                    if (!String.IsNullOrEmpty(body))
                    {
                        var replyMessageBody = new ItemBody { ContentType = BodyType.Text, Content = body };
                        replyMessage.Body = replyMessageBody;
                        await graphClient.Me.Messages[replyMessage.Id].Request().UpdateAsync(replyMessage);
                    }
                    await graphClient.Me.Messages[replyMessage.Id].Send().Request().PostAsync();
                    emailSent = true;
                }

            }

            catch (ServiceException e)
            {
                Debug.WriteLine(SampleStrings.SendMessageFailedDebug + e.Error.Message);
                emailSent = false;
            }

            return emailSent;
        }


        public static async Task<bool> SendForwardMessageAsync(string body, string subject, string recipients, bool? isOrganizer)
        {
            bool emailSent = false;

            try
            {
                // Prepare the recipient list
                string[] splitter = { ";" };
                var splitRecipientsString = recipients.Split(splitter, StringSplitOptions.RemoveEmptyEntries);
                List<Recipient> recipientList = new List<Recipient>();

                foreach (string recipient in splitRecipientsString)
                {
                    recipientList.Add(new Recipient { EmailAddress = new EmailAddress { Address = recipient.Trim() } });
                }

                var graphClient = AuthenticationHelper.GetAuthenticatedClient();
                // Get meeting message from user's Sent Items folder or Inbox
                IMailFolderMessagesCollectionPage eventMessages;

                if (isOrganizer.GetValueOrDefault())
                {
                    eventMessages = await graphClient.Me.MailFolders.SentItems.Messages.Request().Filter("Subject eq '" + subject + "'").GetAsync();
                }
                else
                {
                    eventMessages = await graphClient.Me.MailFolders.Inbox.Messages.Request().Filter("Subject eq '" + subject + "'").GetAsync();
                }


                if (eventMessages.Count > 0)
                {
                    var messageToForward = eventMessages[0];


                    // Forward message to recipients list
                    var forwardMessage = await graphClient.Me.Messages[messageToForward.Id].CreateForward().Request().PostAsync();
                    forwardMessage.ToRecipients = recipientList;
                    await graphClient.Me.Messages[forwardMessage.Id].Request().UpdateAsync(forwardMessage);

                    if (!String.IsNullOrEmpty(body))
                    {
                        var forwardMessageBody = new ItemBody { ContentType = BodyType.Text, Content = body };
                        forwardMessage.Body = forwardMessageBody;                       
                        await graphClient.Me.Messages[forwardMessage.Id].Request().UpdateAsync(forwardMessage);
                    }
                    await graphClient.Me.Messages[forwardMessage.Id].Send().Request().PostAsync();
                    emailSent = true;
                }

            }

            catch (ServiceException e)
            {
                Debug.WriteLine(SampleStrings.SendMessageFailedDebug + e.Error.Message);
            }

            return emailSent;
        }

    }
}
