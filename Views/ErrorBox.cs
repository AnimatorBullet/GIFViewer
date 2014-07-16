﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Media;
using System.Text;
using System.Windows.Forms;

namespace GIF_Viewer.Views
{
    /// <summary>
    /// A form that is used to display an error that has a field that has copiable
    /// information about the error
    /// </summary>
    public partial class ErrorBox : Form
    {
        /// <summary>
        /// Initializes a new instance of the ErrorBox
        /// </summary>
        protected ErrorBox(string message, string caption, string copyInfo)
        {
            InitializeComponent();

            lbl_error.Text = message;
            this.Text = caption;
            rtb_errorInfo.Text = copyInfo;

            SystemSounds.Hand.Play();
        }

        /// <summary>
        /// Shows an ErrorBox with the given information on it
        /// </summary>
        /// <param name="message">The message to display</param>
        /// <param name="caption">The caption to use on the box</param>
        /// <param name="copyInfo">The copiable info</param>
        /// <returns></returns>
        public static DialogResult Show(string message, string caption, string copyInfo)
        {
            ErrorBox box = new ErrorBox(message, caption, copyInfo);

            return box.ShowDialog();
        }

        // 
        // Copy to Clipboard button click
        // 
        private void btn_copyToClipboard_Click(object sender, EventArgs e)
        {
            Clipboard.SetText(rtb_errorInfo.Text);
        }

        // 
        // Ok button click
        // 
        private void btn_ok_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.OK;

            this.Close();
        }
    }
}