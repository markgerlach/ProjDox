using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel;

using DevExpress.XtraGrid;
using DevExpress.XtraGrid.Views;
using DevExpress.XtraGrid.Views.Base;
using DevExpress.XtraGrid.Views.Card;
using DevExpress.XtraGrid.Views.Grid;
using DevExpress.XtraGrid.Views.Layout;

namespace mgControls
{
	public class mgDevX_Label_Blinking : DevExpress.XtraEditors.LabelControl
	{
		private System.Windows.Forms.Timer _timer = new System.Windows.Forms.Timer();
		private int _flashInterval = 0;
		private bool _flashOn = true;
		private string _textHolder = string.Empty;
		private int _tickCounter = 0;

		public mgDevX_Label_Blinking()
			: base()
		{
			// When the control is created, set the properties accordingly
			if (this.DesignMode)
			{
				this.Text = "< Blinking Label >";
			}
			else
			{
				this.Text = string.Empty;
			}
			this.AutoSizeMode = DevExpress.XtraEditors.LabelAutoSizeMode.None;

			_timer.Tick += new EventHandler(_timer_Tick);
		}

		private void _timer_Tick(object sender, EventArgs e)
		{
			// When the timer ticks, flash the text
			_tickCounter++;
			if (_tickCounter % 5 == 0) { _flashOn = true; }
			else { _flashOn = false; }
			if (_flashOn)
			{
				// Turn off the text
				this.Text = string.Empty;
			}
			else
			{
				// Turn on the text
				this.Text = _textHolder;
			}
		}

		/// <summary>
		/// Flash the control at the default interval
		/// </summary>
		public void StartBlinking()
		{
			if (_flashInterval == 0) { _flashInterval = 500; }
			StartBlinking(_flashInterval);
		}

		/// <summary>
		/// Flash the control at the default interval
		/// </summary>
		/// <param name="interval">The interval to flash at</param>
		public void StartBlinking(int interval)
		{
			_flashInterval = interval;
			_textHolder = this.Text;
			_timer.Interval = _flashInterval;
			_tickCounter = 0;
			_timer.Start();
		}

		public void StopBlinking()
		{
			_timer.Stop();

			// Make sure the text is showing before leaving
			this.Text = _textHolder;

			// Reset the flash on
			_flashOn = true;
		}
	}
}
