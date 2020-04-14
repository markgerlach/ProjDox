using System;
using System.Collections.Generic;
using System.Text;

namespace mgControls
{
	public class mgDevX_SortButton : DevExpress.XtraEditors.SimpleButton, DevExpress.Utils.Controls.IXtraResizableControl
	{
		private mgDevX_SortButtonState _currentState = mgDevX_SortButtonState.None;
		private System.Drawing.Size _defaultSize = new System.Drawing.Size(24, 20);

		public mgDevX_SortButton()
			: base()
		{
			this.ImageLocation = DevExpress.XtraEditors.ImageLocation.MiddleCenter;

			this.Layout += new System.Windows.Forms.LayoutEventHandler(mgDevX_SortButton_Layout);
			this.Click += new EventHandler(mgDevX_SortButton_Click);
		}

		private void mgDevX_SortButton_Click(object sender, EventArgs e)
		{
			// Reset the state
			mgDevX_SortButtonState oldState = _currentState;
			if (_currentState == mgDevX_SortButtonState.None) { _currentState = mgDevX_SortButtonState.Ascending; }
			else if (_currentState == mgDevX_SortButtonState.Ascending) { _currentState = mgDevX_SortButtonState.Descending; }
			else if (_currentState == mgDevX_SortButtonState.Descending) { _currentState = mgDevX_SortButtonState.None; }

			ResetImageBasedOnState();		// Reset the image based on state

			// Fire the event
			if (!this.DesignMode) { this.OnSortStateChanged(oldState, _currentState); }
		}

		private void mgDevX_SortButton_Layout(object sender, System.Windows.Forms.LayoutEventArgs e)
		{
			ResetImageBasedOnState();		// Reset the image based on state
		}

		protected override DevExpress.XtraEditors.Drawing.BaseControlPainter CreatePainter()
		{
			return new CButtonPainter();
		}

		protected class CButtonPainter : DevExpress.XtraEditors.Drawing.BaseButtonPainter
		{
			protected override void DrawContent(DevExpress.XtraEditors.Drawing.ControlGraphicsInfoArgs info)
			{
				DevExpress.XtraEditors.ViewInfo.BaseButtonViewInfo vi = info.ViewInfo as DevExpress.XtraEditors.ViewInfo.BaseButtonViewInfo;
				vi.ButtonInfo.DrawFocusRectangle = false;
				base.DrawContent(info);
			}
		}

		public delegate void SortStateChangedEventHandler(object sender, mgDevX_SortButton_EventArgs e);
		public event SortStateChangedEventHandler SortStateChanged;
		protected void OnSortStateChanged(mgDevX_SortButtonState oldState, mgDevX_SortButtonState newState)
		{
			if (SortStateChanged != null)
			{
				SortStateChanged(this, new mgDevX_SortButton_EventArgs(oldState, newState));
			}
		}

		/// <summary>
		/// Reset the image based on the state
		/// </summary>
		private void ResetImageBasedOnState()
		{
			// Set the sort order
			this.ToolTip = "Sort Order: " + _currentState.ToString();

			// Depending on the state, attempt to change the image
			//try
			//{
			//	switch (_currentState)
			//	{
			//		//case mgDevX_SortButtonState.None:
			//		//    this.Image = global::<projectName>.Properties.Resources.Symbol_Cancel_Green_01_16;
			//		//    break;
			//		//case mgDevX_SortButtonState.Ascending:
			//		//    this.Image = global::<projectName>.Properties.Resources.Sort_Ascending_02_16;
			//		//    break;
			//		//case mgDevX_SortButtonState.Descending:
			//		//    this.Image = global::<projectName>.Properties.Resources.Sort_Descending_02_16;
			//		//    break;
			//	}
			//}
			//catch
			//{
			//	// Don't do anything here
			//}
		}

		/// <summary>
		/// The current state for the control
		/// </summary>
		public mgDevX_SortButtonState State
		{
			get { return _currentState; }
			set
			{
				mgDevX_SortButtonState oldState = _currentState;
				_currentState = value;

				ResetImageBasedOnState();		// Reset the image based on state

				// Fire the event
				if (!this.DesignMode) { this.OnSortStateChanged(oldState, _currentState); }
			}
		}

		/// <summary>
		/// The current state for the control
		/// </summary>
		public mgDevX_SortButtonState CurrentState
		{
			get { return _currentState; }
		}

		#region IXtraResizableControl Members

		public event EventHandler Changed;

		public bool IsCaptionVisible
		{
			get { return false; }
		}

		public System.Drawing.Size MaxSize
		{
			get { return _defaultSize; }
		}

		public System.Drawing.Size MinSize
		{
			get { return _defaultSize; }
		}

		#endregion
	}

	public enum mgDevX_SortButtonState : int
	{
		None = 0,
		Ascending,
		Descending,
	}

	#region Custom event handler for the sort button
	/// <summary>
	/// Custom event handler for the sort button
	/// </summary>
	[Serializable]
	public class mgDevX_SortButton_EventArgs : EventArgs
	{
		private mgDevX_SortButtonState _oldState = mgDevX_SortButtonState.None;
		private mgDevX_SortButtonState _newState = mgDevX_SortButtonState.None;

		public mgDevX_SortButton_EventArgs(mgDevX_SortButtonState oldState, mgDevX_SortButtonState newState)
		{
			_newState = newState;
			_oldState = oldState;
		}

		public mgDevX_SortButton_EventArgs()
		{
		}

		public mgDevX_SortButtonState OldState
		{
			get { return _oldState; }
		}

		public mgDevX_SortButtonState NewState
		{
			get { return _newState; }
		}
	}
	#endregion Custom event handler for the sort button
}