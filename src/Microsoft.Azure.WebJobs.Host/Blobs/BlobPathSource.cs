﻿// Copyright (c) Microsoft Open Technologies, Inc. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Azure.WebJobs.Host.Bindings;
using Microsoft.Azure.WebJobs.Host.Bindings.Path;

namespace Microsoft.Azure.WebJobs.Host.Blobs
{
    internal static class BlobPathSource
    {
        public static IBlobPathSource Create(string pattern)
        {
            if (pattern == null)
            {
                throw new FormatException("Blob paths must not be null.");
            }

            string containerNamePattern;
            string blobNamePattern;
            int slashIndex = pattern.IndexOf('/');
            bool hasBlobName = slashIndex != -1;

            if (hasBlobName)
            {
                // There must be at least one character before the slash and one character after the slash.
                bool hasNonEmptyBlobAndContainerNames = slashIndex > 0 && slashIndex < pattern.Length - 1;

                if (!hasNonEmptyBlobAndContainerNames)
                {
                    throw new FormatException("Blob paths must be in the format container/blob.");
                }

                containerNamePattern = pattern.Substring(0, slashIndex);
                blobNamePattern = pattern.Substring(slashIndex + 1);
            }
            else
            {
                containerNamePattern = pattern;
                blobNamePattern = String.Empty;
            }

            BindingTemplateSource template = BindingTemplateSource.FromString(pattern);

            if (template.ParameterNames.Count() > 0)
            {
                return new ParameterizedBlobPathSource(containerNamePattern, blobNamePattern, template);
            }

            BlobClient.ValidateContainerName(containerNamePattern);

            if (hasBlobName)
            {
                BlobClient.ValidateBlobName(blobNamePattern);
            }

            return new FixedBlobPathSource(new BlobPath(containerNamePattern, blobNamePattern));
        }
    }
}
