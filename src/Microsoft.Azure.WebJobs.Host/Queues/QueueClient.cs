﻿// Copyright (c) Microsoft Open Technologies, Inc. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.Azure.WebJobs.Host.Storage.Queue;
using Microsoft.WindowsAzure.Storage.Queue;

namespace Microsoft.Azure.WebJobs.Host.Queues
{
    internal static class QueueClient
    {
        public static string GetAccountName(IStorageQueueClient client)
        {
            if (client == null)
            {
                return null;
            }

            return StorageClient.GetAccountName(client.Credentials);
        }

        /// <summary>
        /// Ensures that the passed name is a valid queue name.
        /// If not, an ArgumentException is thrown
        /// </summary>
        /// <exception cref="System.ArgumentException">If the name is invalid</exception>
        /// <param name="name">The name to be tested</param>
        public static void ValidateQueueName(string name)
        {
            string errorMessage;

            if (!IsValidQueueName(name, out errorMessage))
            {
                throw new ArgumentException(errorMessage, "name");
            }
        }

        public static bool IsValidQueueName(string name)
        {
            string ignore;
            return IsValidQueueName(name, out ignore);
        }

        // This function from: http://blogs.msdn.com/b/neilkidd/archive/2008/11/11/windows-azure-queues-are-quite-particular.aspx
        // See http://msdn.microsoft.com/library/dd179349.aspx for rules to enforce.
        private static bool IsValidQueueName(string name, out string errorMessage)
        {
            if (String.IsNullOrEmpty(name))
            {
                errorMessage = "A queue name can't be null or empty";
                return false;
            }

            // A queue name must be from 3 to 63 characters long.
            if (name.Length < 3 || name.Length > 63)
            {
                errorMessage = "A queue name must be from 3 to 63 characters long - \"" + name + "\"";
                return false;
            }

            // The dash (-) character may not be the first or last letter.
            // we will check that the 1st and last chars are valid later on.
            if (name[0] == '-' || name[name.Length - 1] == '-')
            {
                errorMessage = "The dash (-) character may not be the first or last letter - \"" + name + "\"";
                return false;
            }

            Char previousCharacter = 'a';

            // A queue name must start with a letter or number, and may 
            // contain only letters, numbers and the dash (-) character
            // All letters in a queue name must be lowercase.
            foreach (Char ch in name)
            {
                if (Char.IsUpper(ch))
                {
                    errorMessage = "Queue names must be all lower case - \"" + name + "\"";
                    return false;
                }
                if (Char.IsLetterOrDigit(ch) == false && ch != '-')
                {
                    errorMessage =
                        "A queue name can contain only letters, numbers, and and dash(-) characters - \"" + name + "\"";
                    return false;
                }
                if (ch == '-' && previousCharacter == '-')
                {
                    errorMessage = "A queue name cannot contain consecutive dash(-) characters.";
                    return false;
                }
                previousCharacter = ch;
            }

            errorMessage = null;
            return true;
        }
    }
}
