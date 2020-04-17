using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;

using DevExpress.XtraCharts;

using mgWinChart.Helpers;
using mgWinChart.Interfaces;

namespace mgWinChart.usercontrols
{
    public partial class SeriesUI : AbstractUI
    {
        #region Private Variables
        private Series _series = null;
        #endregion Private Variables
        #region Constructors
        public SeriesUI()
        {
            InitializeComponent();

            mgInit();
        }
        private void mgInit()
        {

        }
        private void SeriesUI_Load(object sender, EventArgs e)
        {
            if (!DesignMode)
            {
                if (Series != null)
                {
                    //Make sure the UI reflects the Series as much as possible.
                    if (cboSeriesSource.Items.Contains(Series.Name))
                    {
                        cboSeriesSource.SelectedItem = Series.Name;
                    }

                    //LabelsChecked = Series.Label.Visible;
                }
            }
        }
        #endregion Constructors
        #region Public Properties
        /// <summary>
        /// The Series the UI controls.
        /// </summary>
        [Browsable(false)]
        public Series Series
        {
            get
            {
                return _series;
            }
            set
            {
                if (_series != null && Title == _series.Name)
                {
                    Title = value.Name;
                }
                _series = value;
                cboSeriesSource.SelectedItem = value.Name;
            }
        }
        /// <summary>
        /// The Group this Series should belong to for Grouped ViewTypes.
        /// </summary>
        [Browsable(false)]
        public string SelectedGroup
        {
            get
            {
                return (string)cboSeriesGroup.SelectedItem;
            }
            set
            {
                cboSeriesGroup.SelectedItem = value;
            }
        }
        /// <summary>
        /// Whether the UI allows the user to set the SelectedGroup.
        /// </summary>
        [Category("Behavior")]
        public bool SeriesGroupEnabled
        {
            get
            {
                return cboSeriesGroup.Enabled;
            }
            set
            {
                cboSeriesGroup.Enabled = value;
                cboSeriesGroup.Visible = value;
				//lblSeriesGroup.Visible = value;
            }
        }
        /// <summary>
        /// Whether this UI allows the user to set the 3D model for bar graphs.
        /// </summary>
        public override bool BarShapeEnabled
        {
            get
            {
                return cboBarShape.Enabled;
            }
            set
            {
            	cboBarShape.Enabled = value;
                cboBarShape.Visible = value;
				//lblBarShape.Visible = value;
            }
        }
        /// <summary>
        /// Whether this UI is set up for a Pie or Doughnut series.
        /// </summary>
        public override bool PieAndDoughnutEnabled
        {
            get
            {
				return layoutControlGroup3.Enabled;
            }
            set
            {
				layoutControlGroup3.Enabled = value;
				//grpPieAndDoughnut.Visible = value;
            }
        }
        /// <summary>
        /// Whether this UI allows the user to set the series's transparency.
        /// </summary>
        public override bool TransparencyEnabled
        {
            get
            {
                return false;
            }
            set
            {
                //do nothing
            }
        }
        /// <summary>
        /// Whether the Labels Visible checkbox is checked.
        /// </summary>
        public override bool LabelsChecked
        {
            get
            {
                return chkLabelsVisible.Checked;
            }
            set
            {
                chkLabelsVisible.Checked = value;
            }
        }
        #endregion Public Properties
        #region Public Methods
        /// <summary>
        /// Sets the columns or other data sources the user can have this Series represent.
        /// </summary>
        /// <param name="choices">The strings (names) the user will be able to choose from the Data Member combo box.</param>
        public void SetDataMemberChoices(List<string> choices)
        {
            cboSeriesSource.Items.Clear();
            cboSeriesSource.Items.Add("none");
            
            foreach (string s in choices)
            {
                cboSeriesSource.Items.Add(s);
            }
        }
        /// <summary>
        /// Sets how many groups the user will be able to group series into for Grouped ViewTypes.
        /// </summary>
        /// <param name="numChoices">The number of groups allowed.</param>
        public void SetGroupingChoices(int numChoices)
        {
            cboSeriesGroup.Items.Clear();
            for (int i = 0; i < numChoices; i++)
            {
                cboSeriesGroup.Items.Add("Group " + (i + 1).ToString());
            }
            if (cboSeriesGroup.Items.Count > 0)
            {
                cboSeriesGroup.SelectedItem = "Group 1";
            }
        }
        /// <summary>
        /// Updates the UI to match the Series's ViewType, then updates the Series properties to match the user's selections.
        /// </summary>
        public void UpdateSeries()
        {
            UpdateToViewTypeOfSeries(Series);

            //Series.Label.Visible = LabelsChecked;

            UpdateBarShape();
            UpdateBarPosition();
            UpdatePiePosition();
            UpdateHoleRadius();
            //UpdateExplodedDistance();

            /*
            if (Series.View is ISupportTransparency)
            {
                byte alphaFactor = 0;
                switch (ddlTransparency.SelectedValue)
                {
                    case "Solid":
                        alphaFactor = 0;
                        break;
                    case "75%":
                        alphaFactor = 63;
                        break;
                    case "50%":
                        alphaFactor = 127;
                        break;
                    case "25%":
                        alphaFactor = 191;
                        break;
                    case "Invisible":
                        alphaFactor = 255;
                        break;
                    default:
                        alphaFactor = 0;
                        break;
                }
                ((ISupportTransparency)Series.View).Transparency = alphaFactor;
            }
            */
        }
        /// <summary>
        /// Resets all properties to match those of the VisualSettingsObject.
        /// </summary>
        /// <param name="visualSettings">The set of Visual Settings the Series and SeriesUI should implement.</param>
        public void AlignToVisualSettings(VisualSettingsObject visualSettings)
        {
            Series.ChangeView(visualSettings.ViewType);
            LabelsChecked = visualSettings.LabelsVisible;
            cboBarShape.SelectedItem = "default";
            cboPosition.SelectedItem = "default";
            nudHoleRadius.Value = ucWinChart.HoleRadiusDictionary[visualSettings.HoleRadius];
            nudExplodedDist.Value = visualSettings.ExplodeDistance;

            UpdateSeries();
        }
        #endregion Public Methods
        #region Event Handlers
        private void cboSeriesSource_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (Series != null)
            {
                string selectedSeriesName = (string)cboSeriesSource.SelectedItem;
                Series.Visible = true;
                if (selectedSeriesName == "none")
                {
                    Series.Visible = false;
                }
                else if (Series.Name != selectedSeriesName)
                {
                    Series.Name = selectedSeriesName;
                    OnSeriesDataMemberChanged(new EventArgs());
                    UpdateSeries();
                } 
            }
        }
        private void cboSeriesGroup_SelectedIndexChanged(object sender, EventArgs e)
        {
            OnSeriesGroupChanged(new EventArgs());
        }
        private void chkLabelsVisible_CheckedChanged(object sender, EventArgs e)
        {
            //Series.Label.Visible = LabelChecked;
            //Series.Label.LineVisibility = (LabelsChecked ? DevExpress.Utils.DefaultBoolean : false);
        }
        private void cboPosition_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (Series != null)
            {
                if (Series.Label is BarSeriesLabel)
                {
                    UpdateBarPosition();
                }
                else if (Series.Label is PieSeriesLabel)
                {
                    UpdatePiePosition();
                }
            }
        }
        private void cboBarShape_SelectedIndexChanged(object sender, EventArgs e)
        {
            UpdateBarShape();
        }
        private void nudHoleRadius_ValueChanged(object sender, EventArgs e)
        {
            UpdateHoleRadius();
        }
        private void btnExplosion_Click(object sender, EventArgs e)
        {
            //do nothing
        }
        private void nudExplodedDist_ValueChanged(object sender, EventArgs e)
        {
            UpdateExplodedDistance();
        }
        #endregion Event Handlers
        #region Protected Methods
        /// <summary>
        /// Updates the UI to match the chosen series's ViewType.
        /// </summary>
        /// <param name="series">The chosen series.</param>
        protected override void UpdateToViewTypeOfSeries(Series series)
        {
            if (series.View is ISupportStackedGroup)
            {
                SeriesGroupEnabled = true;
            }
            else
            {
                SeriesGroupEnabled = false;
            }

            base.UpdateToViewTypeOfSeries(series);
        }
        /// <summary>
        /// Updates the Series's 3D model to match the user's choice.
        /// </summary>
        protected override void UpdateBarShape()
        {            
            //if (Series.View is Bar3DSeriesView)
            //{
            //    if (cboBarShape.SelectedItem == null)
            //    {
            //        cboBarShape.SelectedItem = "default";
            //        return;
            //    }
            //    if (cboBarShape.SelectedItem.ToString().ToLower().Equals(cboBarShape.SelectedItem))
            //    {
            //        ((Bar3DSeriesView)Series.View).Model = ParentChart.ModelType;
            //    }
            //    else
            //    {
            //        ((Bar3DSeriesView)Series.View).Model = GenericEnumConverter.ValueForString<Bar3DModel>((string)cboBarShape.SelectedItem);
            //    }
            //}
        }
        /// <summary>
        /// Updates the Series's BarSeriesLabelPosition to match the user's choice.
        /// </summary>
        protected override void UpdateBarPosition()
        {
            if (Series != null && 
                Series.Label is BarSeriesLabel)
            {
                if (cboPosition.SelectedItem == null)
                {
                    cboPosition.SelectedItem = "default";
                    return;
                }
     //           if (cboPosition.SelectedItem.ToString().ToLower().Trim().Contains("bottom"))
     //           {
					////((BarSeriesLabel)Series.Label).Position = ParentChart.BarLabelPosition;
     //               return;
     //           }
     //           if (cboPosition.SelectedItem.ToString().ToLower().Trim().Equals("top") && 
     //               (Series.View is   typeof(StackedBarSeriesView) || 
     //               Series.View is StackedBar3DSeriesView))
     //           {
     //               cboPosition.SelectedItem = "Top Inside";
     //           }
                string cleanedString = ((string)cboPosition.SelectedItem).Replace(" ", "");
                //((BarSeriesLabel)Series.Label).Position = GenericEnumConverter.ValueForString<BarSeriesLabelPosition>(cleanedString);
            }
        }
        /// <summary>
        /// Updates the Series's PieSeriesLabelPosition to match the user's choice.
        /// </summary>
        protected override void UpdatePiePosition()
        {
            if (Series != null && Series.Label is PieSeriesLabel)
            {
                if (cboPosition.SelectedItem == null)
                {
                    cboPosition.SelectedItem = "default";
                    return;
                }
                //if (cboPosition.SelectedItem == "default")
                //{
                //    ((PieSeriesLabel)Series.Label).Position = ParentChart.PieLabelPosition;
                //}
                //else
                //{
                //    string cleanedString = ((string)cboPosition.SelectedItem).Replace(" ", "");
                //    ((PieSeriesLabel)Series.Label).Position = GenericEnumConverter.ValueForString<PieSeriesLabelPosition>(cleanedString);
                //}
            }
        }
        /// <summary>
        /// Updates the Series's HoleRadius to match the user's choice.
        /// </summary>
        protected override void UpdateHoleRadius()
        {
            if (Series == null)
            {
                return;
            }
            if (ParentChart.HoleRadius != HoleRadius.Radius_000)
            {
                return;
            }
            if (Series.View is DoughnutSeriesView)
            {
                ((DoughnutSeriesView)Series.View).HoleRadiusPercent = (int)nudHoleRadius.Value;
            }
            else if (Series.View is Doughnut3DSeriesView)
            {
                ((Doughnut3DSeriesView)Series.View).HoleRadiusPercent = (int)nudHoleRadius.Value;
            }
        }
        /// <summary>
        /// Updates the Series's Exploded Distance to match the user's choice.
        /// </summary>
        protected override void UpdateExplodedDistance()
        {
            if (Series.View is PieSeriesViewBase)
            {
                ((PieSeriesViewBase)Series.View).ExplodedDistancePercentage = (int)nudExplodedDist.Value;
            }
        }
        /// <summary>
        /// Sets the Label Positions the user will be able to choose from.
        /// </summary>
        /// <param name="choices">A list of the positions' names.</param>
        protected override void SetPositionChoices(List<string> choices)
        {
            string oldSelectedItem = (string)cboPosition.SelectedItem;
            cboPosition.Items.Clear();
            cboPosition.Items.Add("default");
            cboPosition.Items.AddRange(choices.ToArray());
            if (oldSelectedItem != null && cboPosition.Items.Contains(oldSelectedItem))
            {
                cboPosition.SelectedItem = oldSelectedItem;
            }
            else
            {
                cboPosition.SelectedItem = "default";
            }
        }
        #endregion Protected Methods
        #region Class Events
        public event EventHandler SeriesDataMemberChanged;
        protected virtual void OnSeriesDataMemberChanged(EventArgs e)
        {
            if (SeriesDataMemberChanged != null)
            {
                SeriesDataMemberChanged(this, e);
            }
        }
        public event EventHandler SeriesGroupChanged;
        protected virtual void OnSeriesGroupChanged(EventArgs e)
        {
            if (SeriesGroupChanged != null)
            {
                SeriesGroupChanged(this, e);
            }
        }
        #endregion Class Events
    }
}
