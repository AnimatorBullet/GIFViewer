#region Copyright (C) Simon Bridewell
// 
// This file is part of the GifComponents library.
// GifComponents is free software; you can redistribute it and/or
// modify it under the terms of the Code Project Open License.
// 
// This program is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// Code Project Open License for more details.
// 
// You can read the full text of the Code Project Open License at:
// http://www.codeproject.com/info/cpol10.aspx
//
// GifComponents is a derived work based on NGif written by gOODiDEA.NET
// and published at http://www.codeproject.com/KB/GDI-plus/NGif.aspx,
// with an enhancement by Phil Garcia published at
// http://www.thinkedge.com/blogengine/post/2008/02/20/Animated-GIF-Encoder-for-NET-Update.aspx
//
// Simon Bridewell makes no claim to be the original author of this library,
// only to have created a derived work.
#endregion

using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Text;
using GIF_Viewer.GifComponents.Enums;

namespace GIF_Viewer.GifComponents.Components
{
    /// <summary>
    /// The Application Extension contains application-specific information; 
    /// it conforms with the extension block syntax, and its block label is 
    /// 0xFF.
    /// 
    /// See http://www.w3.org/Graphics/GIF/spec-gif89a.txt section 26.
    /// </summary>
    [Description("The Application Extension contains application-specific " +
                 "information; it conforms with the extension block syntax, " +
                 "and its block label is 0xFF.")]
    public class ApplicationExtension : GifComponent
    {
        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="identificationBlock">
        /// Sets the <see cref="IdentificationBlock"/>
        /// </param>
        /// <param name="applicationData">
        /// Sets the <see cref="ApplicationData"/>
        /// </param>
        public ApplicationExtension(DataBlock identificationBlock,
                                     Collection<DataBlock> applicationData)
        {
            SaveData(identificationBlock, applicationData);
        }
        
        /// <summary>
        /// Reads and returns an application extension from the supplied input 
        /// stream.
        /// </summary>
        /// <param name="inputStream">
        /// The input stream to read.
        /// </param>
        public ApplicationExtension(Stream inputStream)
        {
            var identificationBlock = new DataBlock(inputStream);
            var applicationData = new Collection<DataBlock>();
            if (!identificationBlock.TestState(ErrorState.EndOfInputStream))
            {
                // Read application specific data
                DataBlock thisBlock;
                do
                {
                    thisBlock = new DataBlock(inputStream);
                    applicationData.Add(thisBlock);
                }
                // A zero-length block indicates the end of the data blocks
                while (thisBlock.DeclaredBlockSize != 0
                       && !thisBlock.TestState(ErrorState.EndOfInputStream)
                     );
            }

            SaveData(identificationBlock, applicationData);
        }
        
        private void SaveData(DataBlock identificationBlock,
                               Collection<DataBlock> applicationData)
        {
            IdentificationBlock = identificationBlock;

            if (IdentificationBlock.Data.Length < 11)
            {
                string message
                    = "The identification block should be 11 bytes long but "
                    + "is only " + IdentificationBlock.Data.Length + " bytes.";
                throw new ArgumentException(message, nameof(identificationBlock));
            }

            if (IdentificationBlock.Data.Length > 11)
            {
                string message
                    = "The identification block should be 11 bytes long but "
                    + "is " + IdentificationBlock.Data.Length + " bytes long. "
                    + "Additional bytes are ignored.";
                SetStatus(ErrorState.IdentificationBlockTooLong, message);
            }

            // Read application identifer
            var sb = new StringBuilder();
            for (int i = 0; i < 8; i++)
            {
                sb.Append((char)IdentificationBlock[i]);
            }
            ApplicationIdentifier = sb.ToString();

            // Read application authentication code
            sb = new StringBuilder();
            for (int i = 8; i < 11; i++)
            {
                sb.Append((char)IdentificationBlock[i]);
            }
            ApplicationAuthenticationCode = sb.ToString();

            ApplicationData = applicationData;
        }

        /// <summary>
        /// Returns a data block which identifies the application defining this 
        /// extension.
        /// </summary>
        [Description("Returns a data block which identifies the application " +
                     "defining this extension.")]
        public DataBlock IdentificationBlock { get; private set; }

        /// <summary>
        /// Sequence of eight printable ASCII characters used to identify the 
        /// application owning the Application Extension.
        /// </summary>
        [Description("Sequence of eight printable ASCII characters used " +
                     "to identify the application owning the Application " +
                     "Extension.")]
        public string ApplicationIdentifier { get; private set; }

        /// <summary>
        /// Sequence of three bytes used to authenticate the Application 
        /// Identifier. 
        /// An Application program may use an algorithm to compute a binary 
        /// code that uniquely identifies it as the application owning the 
        /// Application Extension.
        /// </summary>
        [Description("Sequence of three bytes used to authenticate the " +
                     "Application Identifier. " +
                     "An Application program may use an algorithm to compute " +
                     "a binary code that uniquely identifies it as the " +
                     "application owning the Application Extension.")]
        public string ApplicationAuthenticationCode { get; private set; }

        /// <summary>
        /// Data specific to the application declared by the 
        /// <see cref="ApplicationIdentifier"/>
        /// </summary>
        public Collection<DataBlock> ApplicationData { get; private set; }
    }
}