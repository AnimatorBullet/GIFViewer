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

using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Text;

namespace GIF_Viewer.GifComponents.Components
{
	/// <summary>
	/// An application extension which controls the number of times an animation
	/// should be displayed.
	/// See http://www.let.rug.nl/~kleiweg/gif/netscape.html for format
	/// </summary>
	public class NetscapeExtension : ApplicationExtension
	{
	    /// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="repeatCount">
		/// Number of times to repeat the animation.
		/// 0 to repeat indefinitely, -1 to not repeat.
		/// </param>
		public NetscapeExtension( int repeatCount )
			: this( new ApplicationExtension( GetIdentificationBlock(), GetApplicationData( repeatCount ) ) )
		{
			LoopCount = repeatCount;
		}

	    /// <summary>
	    /// Constructor.
	    /// </summary>
	    /// <param name="applicationExtension">
	    /// The application extension to build the Netscape extension from.
	    /// </param>
	    public NetscapeExtension(ApplicationExtension applicationExtension)
	        : base(applicationExtension.IdentificationBlock,
	            applicationExtension.ApplicationData)
	    {
	        string message;
	        if (applicationExtension.ApplicationIdentifier != "NETSCAPE")
	        {
	            message = "The application identifier is not 'NETSCAPE' "
	                      + "therefore this application extension is not a "
	                      + "Netscape extension. Application identifier: "
	                      + applicationExtension.ApplicationIdentifier;
	            throw new ArgumentException(message, nameof(applicationExtension));
	        }

	        if (applicationExtension.ApplicationAuthenticationCode != "2.0")
	        {
	            message = "The application authentication code is not '2.0' "
	                      + "therefore this application extension is not a "
	                      + "Netscape extension. Application authentication code: "
	                      + applicationExtension.ApplicationAuthenticationCode;
	            throw new ArgumentException(message, nameof(applicationExtension));
	        }

	        foreach (var block in ApplicationData)
	        {
	            if (block.ActualBlockSize == 0)
	            {
	                // then we've found the block terminator
	                break;
	            }
	            // The first byte in a Netscape application extension data
	            // block should be 1. Ignore if anything else.
	            if (block.ActualBlockSize > 2 && block[0] == 1)
	            {
	                // The loop count is held in the second and third bytes
	                // of the data block, least significant byte first.
	                int byte1 = block[1] & 0xff;
	                int byte2 = block[2] & 0xff;

	                // String the two bytes together to make an integer,
	                // with byte 2 coming first.
	                LoopCount = (byte2 << 8) | byte1;
	            }
	        }
	    }

	    /// <summary>
		/// Number of times to repeat the frames of the animation.
		/// 0 to repeat indefinitely. -1 to not repeat.
		/// </summary>
		public int LoopCount { get; }

	    private static DataBlock GetIdentificationBlock()
	    {
	        var s = new MemoryStream();
	        var bytes = Encoding.ASCII.GetBytes("NETSCAPE2.0".ToCharArray());
	        s.Write(bytes, 0, bytes.Length);
	        s.Seek(0, SeekOrigin.Begin);
	        byte[] identificationData = new byte[11];
	        s.Read(identificationData, 0, 11);
	        var identificationBlock = new DataBlock(11, identificationData);
	        return identificationBlock;
	    }

        private static Collection<DataBlock> GetApplicationData( int repeatCount )
		{
		    var s = new MemoryStream();
            s.WriteByte(1);
		    var repeatCountBytes = BitConverter.GetBytes((short)repeatCount);
		    s.Write(repeatCountBytes, 0, BitConverter.GetBytes((short)repeatCount).Length);
		    s.Seek(0, SeekOrigin.Begin);
		    byte[] repeatData = new byte[3];
		    s.Read(repeatData, 0, 3);
		    var repeatBlock = new DataBlock(3, repeatData);

		    byte[] terminatorData = new byte[0];
		    var terminatorBlock = new DataBlock(0, terminatorData);

		    var appData = new Collection<DataBlock> { repeatBlock, terminatorBlock };
            return appData;
		}
	}
}
