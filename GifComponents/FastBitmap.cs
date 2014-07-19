﻿/* Copyright Visual C# Kicks, Simon Bridewell
 * This file is open source software and is subject to the Visual C# Kicks
 * Open License, which can be downloaded from http://www.vcskicks.com/license.php
 */

#region changes
/*
 Based on a class downloaded from http://www.vcskicks.com/fast-image-processing.php
 Modified by Simon Bridewell May 2010:
 	* XML and inline comments added and whitespace added to code.
 	* Private members renamed to begin with an underscore.
 	* CheckImageIsLocked and ValidateCoordinates methods added.
*/
#endregion

using System;
using System.Diagnostics.CodeAnalysis;
using System.Drawing;
using System.Drawing.Imaging;

namespace GifComponents
{
    /// <summary>
    /// A class which provides faster GetPixel and SetPixel methods than those
    /// provided by the System.Drawing.Bitmap class, using unsafe code and 
    /// pointers to access the pixel data directly.
    /// Project properties must be set to allow unsafe code.
    /// </summary>
    unsafe public class FastBitmap
    {
        /// <summary>
        /// The bitmap we are working on.
        /// </summary>
        private Bitmap _workingBitmap;

        /// <summary>
        /// Width of the bitmap in pixels.
        /// </summary>
        private int _imageWidth;

        /// <summary>
        /// Height of the bitmap in pixels.
        /// </summary>
        private int _imageHeight;

        /// <summary>
        /// Holds information about the lock operation which we perform on the
        /// working bitmap.
        /// </summary>
        private BitmapData _bitmapData;

        /// <summary>
        /// Pointer to the pixel we're working with.
        /// </summary>
        private int* _pBase;

        private int _strideWidth;

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="inputBitmap">The bitmap to work with</param>
        public FastBitmap(Bitmap inputBitmap)
        {
            if (inputBitmap == null)
            {
                throw new ArgumentNullException("inputBitmap");
            }
            _workingBitmap = inputBitmap;
        }

        /// <summary>
        /// Locks the image data into memory.
        /// Call this before calling the GetPixel or SetPixel methods.
        /// </summary>
        public void LockImage()
        {
            Rectangle bounds = new Rectangle(Point.Empty, _workingBitmap.Size);

            _imageWidth = _workingBitmap.Width;
            _imageHeight = _workingBitmap.Height;

            // Lock image
            _bitmapData = _workingBitmap.LockBits(bounds, ImageLockMode.ReadWrite, PixelFormat.Format32bppArgb);
            // Get a pointer to the first pixel in the bitmap.
            _pBase = (int*)_bitmapData.Scan0;

            // Calculate stride width, for 
            _strideWidth = _bitmapData.Stride / 4;
        }

        /// <summary>
        /// Sets the colour of the pixel at the specified co-ordinates.
        /// </summary>
        /// <param name="x">The horizontal co-ordinate</param>
        /// <param name="y">The vertical co-ordinate</param>
        /// <param name="colour">The colour to set the pixel to</param>
        public void SetPixel(int x, int y, int colour)
        {
            // The calls to CheckImageIsLocked and ValidateCoordinates slow
            // processing slightly but ensure that meaningful error messages are
            // returned to the caller in the event that the image isn't locked
            // into memory or the x or y co-ordinates are outside the bounds of
            // the image.
            if (_pBase == null)
            {
                string message
                    = "The LockImage method must be called before the SetPixel method.";
                throw new InvalidOperationException(message);
            }
            //ValidateCoordinates(x, y);
            if (x < 0 || x >= _imageWidth)
            {
                throw new ArgumentOutOfRangeException("x", "Something bad happened!");
            }
            if (y < 0 || y >= _imageHeight)
            {
                throw new ArgumentOutOfRangeException("y", "Something bad happened!");
            }
            // Put the color pixel on the image data
            *(_pBase + x + y * _strideWidth) = colour;
        }

        /// <summary>
        /// Unlocks the area of memory where the bitmap is held.
        /// Call this method when you have finished calling GetPixel and 
        /// SetPixel.
        /// </summary>
        public void UnlockImage()
        {
            _workingBitmap.UnlockBits(_bitmapData);
            _bitmapData = null;
            _pBase = null;
        }
    }
}
