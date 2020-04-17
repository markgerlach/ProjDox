using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

using DevExpress.XtraCharts;

using mgWinChart.Helpers;
using System.Collections;

namespace mgWinChart.usercontrols
{
    public partial class GeneralUI : AbstractUI
    {
  //      #region Constructors
  //      public GeneralUI()
  //      {
  //          InitializeComponent();

  //          mgInit();
  //      }
  //      public void mgInit()
  //      {

  //      }

  //      private void GeneralUI_Load(object sender, EventArgs e)
  //      {
  //          cboHoleRadius.Items.Clear();
  //          foreach (int i in ucWinChart.HoleRadiusDictionary.Values)
  //          {
  //              cboHoleRadius.Items.Add(i);
  //          }
  //      }
  //      #endregion Constructors
  //      #region Public Properties
  //      /// <summary>
  //      /// Whether the UI allows the user to select the 3D model used for bar graphs.
  //      /// </summary>
  //      [Category("Behavior")]
  //      public override bool BarShapeEnabled
  //      {
  //          get
  //          {
  //              return cboBarShape.Enabled;
  //          }
  //          set
  //          {
  //              cboBarShape.Enabled = value;
  //              cboBarShape.Visible = value;
		//		//lblBarShape.Visible = value;
  //          }
  //      }
  //      /// <summary>
  //      /// Whether the UI is set up for Pie or Doughnut ViewType.
  //      /// </summary>
  //      [Category("Behavior")]
  //      public override bool PieAndDoughnutEnabled
  //      {
  //          get
  //          {
		//		return layoutControlGroup2.Enabled;
  //          }
  //          set
  //          {
		//		layoutControlGroup2.Enabled = value;
		//		//layoutControlGroup2.Visible = value;
  //          }
  //      }
  //      /// <summary>
  //      /// Whether the UI allows the user to set series' transparencies.
  //      /// </summary>
  //      [Category("Behavior")]
  //      public override bool TransparencyEnabled
  //      {
  //          get
  //          {
  //              return false;
  //          }
  //          set
  //          {
  //              //do nothing
  //          }
  //      }
  //      /// <summary>
  //      /// Whether or not the Labels Visible checkbox is checked.
  //      /// </summary>
  //      [Browsable(false)]
  //      public override bool LabelsChecked
  //      {
  //          get
  //          {
  //              return chkLabelsVisible.Checked;
  //          }
  //          set
  //          {
  //              chkLabelsVisible.Checked = value;
  //          }
  //      }
  //      /// <summary>
  //      /// The 3D bar graph model selected in the Bar Shape combo box.
  //      /// </summary>
  //      public Bar3DModel ModelType
  //      {
  //          get
  //          {
  //              return GenericEnumConverter.ValueForString<Bar3DModel>((string)cboBarShape.SelectedItem);
  //          }
  //          set
  //          {
  //              string valueString = GenericEnumConverter.StringForValue<Bar3DModel>(value);
  //              if (valueString != cboBarShape.SelectedItem.ToString() && 
  //                  cboBarShape.Items.Contains(valueString))
  //              {
  //                  cboBarShape.SelectedItem = valueString;
  //              }
  //          }
  //      }
  //      #endregion Public Properties
  //      #region Public Methods
  //      /// <summary>
  //      /// Updates the General settings to match those of an existing series.
  //      /// </summary>
  //      /// <param name="series">The series the settings of which are to be used.</param>
  //      public void UpdateToSeries(Series series)
  //      {
  //          UpdateToViewTypeOfSeries(series);
  //      }
  //      #endregion Public Methods
  //      #region Event Listeners
  //      private void cboBarShape_SelectedIndexChanged(object sender, EventArgs e)
  //      {
  //          UpdateBarShape();
  //      }
  //      private void chkLabelsVisible_CheckedChanged(object sender, EventArgs e)
  //      {
  //          ParentChart.LabelsVisible = chkLabelsVisible.Checked;
  //      }
  //      private void cboPosition_SelectedIndexChanged(object sender, EventArgs e)
  //      {
  //          if (ParentChart.DevXChart.Series[0].Label is BarSeriesLabel)
  //          {
  //              UpdateBarPosition();
  //          }
  //          else if (ParentChart.DevXChart.Series[0].Label is PieSeriesLabel)
  //          {
  //              UpdatePiePosition();
  //          }
  //      }
  //      private void cboHoleRadius_SelectedIndexChanged(object sender, EventArgs e)
  //      {
  //          UpdateHoleRadius();
  //      }

  //      private void nudExplodedDist_ValueChanged(object sender, EventArgs e)
  //      {
  //          UpdateExplodedDistance();
  //      }

  //      private void btnAlignSeriesSettings_Click(object sender, EventArgs e)
  //      {
  //          ParentChart.AlignAllSeries();
  //      }
  //      #endregion Event Listeners
  //      #region Protected Methods
  //      /// <summary>
  //      /// Updates the 3D model used for bar graphs to match the user's choice.
  //      /// </summary>
  //      protected override void UpdateBarShape()
  //      {
  //          ParentChart.ModelType = GenericEnumConverter.ValueForString<Bar3DModel>((string)cboBarShape.SelectedItem);
  //      }
  //      /// <summary>
  //      /// Updates the Label Position for bar ViewTypes to match the user's choice.
  //      /// </summary>
  //      protected override void UpdateBarPosition()
  //      {
  //          if (ParentChart.DevXChart.Series[0].Label is BarSeriesLabel)
  //          {
  //              ParentChart.BarLabelPosition = GenericEnumConverter.ValueForString<BarSeriesLabelPosition>((string)cboPosition.SelectedItem);
  //          }
  //      }
  //      /// <summary>
  //      /// Updates the Label Position for Pie and Doughnut ViewTypes to match the user's choice.
  //      /// </summary>
  //      protected override void UpdatePiePosition()
  //      {
  //          if (ParentChart.DevXChart.Series[0].Label is PieSeriesLabel)
  //          {
  //              ParentChart.PieLabelPosition = GenericEnumConverter.ValueForString<PieSeriesLabelPosition>((string)cboPosition.SelectedItem);
  //          }
  //      }
  //      /// <summary>
  //      /// Updates the Radius of Doughnut chart holes to match the user's choice.
  //      /// </summary>
  //      protected override void UpdateHoleRadius()
  //      {
  //          ParentChart.HoleRadius = GenericEnumConverter.ValueForString<HoleRadius>("RADIUS_" + cboHoleRadius.SelectedItem.ToString());
  //      }
  //      /// <summary>
  //      /// Updates the distance "exploded" wedges are removed from the pie chart to match the user's choice.
  //      /// </summary>
  //      protected override void UpdateExplodedDistance()
  //      {
  //          ParentChart.ExplodeDistance = (uint)nudExplodedDist.Value;
  //      }
  //      /// <summary>
  //      /// Sets which values are available from the Label Position combobox.
  //      /// </summary>
  //      /// <param name="choices">The label positions to make available.</param>
  //      protected override void SetPositionChoices(List<string> choices)
  //      {
  //          cboPosition.Items.Clear();
  //          cboPosition.Items.AddRange(choices.ToArray());

  //          if (ParentChart.DevXChart.Series[0].Label is BarSeriesLabel)
  //          {
  //              cboPosition.SelectedItem = GenericEnumConverter.StringForValue<BarSeriesLabelPosition>(ParentChart.BarLabelPosition);
  //          }
  //          else if (ParentChart.DevXChart.Series[0].Label is PieSeriesLabel)
  //          {
  //              cboPosition.SelectedItem = GenericEnumConverter.StringForValue<PieSeriesLabelPosition>(ParentChart.PieLabelPosition);
  //          }
  //      }
  //      #endregion Protected Methods

		//private void btnExplosion_Click(object sender, EventArgs e)
		//{

		//}
    }
}
