//using System;
//using System.Drawing;
//using System.Data;
//using System.Data.SqlClient;
//using System.Collections;
//using System.Collections.Specialized;
//using System.ComponentModel;
//using System.Windows.Forms;
//using System.Text;
//using System.IO;

//namespace_BaseWinProject.Win.Common
//{
//    /// <summary>
//    /// Summary description for mgButton.
//    /// </summary>
//    public class mgButton : System.Windows.Forms.Button
//    {
//        private bool _enabled = true;
//        private bool _ignoresColorChange = false;
//        private ssGridTypes _labelScheme = ssGridTypes.Empty;
//        private Color _enabledColor = Color.MediumBlue;

//        public mgButton()
//        {
//        }

//        [Browsable(true)]
//        [Category("Appearance")]
//        [Description("The button's background color.")]
//        [DefaultValue(true)]
//        public mgButtonGUIButtonColor ButtonColor
//        {
//            get 
//            {
//                return ; 
//            }
//            set 
//            {
//                switch (value)
//                {
//                    case mgButtonGUIButtonColor.Blue:
						
//                        break;
//                }
				 
//                ResetColoring();	// Reset the coloring on the box
//            }
//        }

//        [Browsable(true)]
//        [Category("Appearance")]
//        [Description("Tells if the control is currently enabled.")]
//        [DefaultValue(true)]
//        public new bool Enabled
//        {
//            get { return _enabled; }
//            set { _enabled = value; ResetColoring(); }
//        }

//        [Browsable(true)]
//        [Category("Appearance")]
//        [Description("Tells if the control ignores color changes imposed by the custom control.")]
//        [DefaultValue(false)]
//        public bool IgnoresColorChange
//        {
//            get { return _ignoresColorChange; }
//            set { _ignoresColorChange = value; ResetColoring(); }
//        }

//        [Browsable(true)]
//        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
//        public ssGridTypes ColorScheme
//        {
//            get { return _labelScheme; }
//            set { _labelScheme = value; ResetColoring(); }
//        }

//        protected override void OnClick(EventArgs e)
//        {
//            if (_enabled) { base.OnClick (e); }
//        }

//        protected override void OnMouseDown(MouseEventArgs e)
//        {
//            if (_enabled) { base.OnMouseDown (e); }
//        }

//        protected override void OnEnabledChanged(EventArgs e)
//        {
//            base.OnEnabledChanged(e);
//            ResetColoring();
//        }

//        /// <summary>
//        /// Reset the coloring on the button
//        /// </summary>
//        public void ResetColoring()
//        {
////			if (String.IsNullOrEmpty(this.Text)) { return; }

//            if (_ignoresColorChange) 
//            { 
//                this.BackColor = System.Drawing.SystemColors.Control;
//                this.ForeColor = System.Drawing.SystemColors.ControlText;
//                return; 
//            }

//            switch (_labelScheme)
//            {
//                case ssGridTypes.BlueStandard:
//                    _enabledColor = Color.MediumBlue;
//                    break;
//                case ssGridTypes.GreenStandard:
//                    _enabledColor = Color.Green;
//                    break;
//                case ssGridTypes.OrangeStandard:
//                    _enabledColor = Color.OrangeRed;
//                    break;
//                case ssGridTypes.PurpleStandard:
//                    _enabledColor = Color.DarkViolet;
//                    break;
//                case ssGridTypes.RedStandard:
//                    _enabledColor = Color.Firebrick;
//                    break;
//                case ssGridTypes.Empty:
//                default:
//                    _enabledColor = Color.MediumBlue;
//                    break;
//            }

//            if (_enabled)
//            {
////				if (!String.IsNullOrEmpty(this.Text))
////				{
//                    //this.BackColor = _enabledColor;
//                    //this.ForeColor = Color.White;

//                if (this.Image == null ||
//                    !String.IsNullOrEmpty(this.Text))
//                {
//                    this.BackColor = _enabledColor;
//                    this.ForeColor = Color.White;
//                }
//                else
//                {
//                    this.ForeColor = _enabledColor;
//                    this.BackColor = Color.White;
//                }
////				}
////				else
////				{
////					this.BackColor = Color.White;
////					this.ForeColor = Color.Black;
////				}
//            }
//            else
//            {
//                this.BackColor = Color.FromArgb(80, 80, 80);
//                this.ForeColor = Color.LightGray;
//            }	
//        }

//        protected override void OnMouseEnter(EventArgs e)
//        {
//            this.Cursor = (_enabled ? Cursors.Hand : Cursors.Default);
//            base.OnMouseEnter (e);
//        }
//    }
//}
