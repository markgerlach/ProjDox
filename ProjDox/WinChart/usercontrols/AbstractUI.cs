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
    public class AbstractUI : UserControl
    {
        #region Internal Variables
        private ucWinChart _parentChart = null;
        #endregion Internal Variables

        #region Constructors
        public AbstractUI()
        {
            
        }
        #endregion Constructors

        #region Public Properties
        /// <summary>
        /// The WinChart object that contains this control.  If there isn't one, returns null.
        /// </summary>
        [Browsable(false)]
        public ucWinChart ParentChart
        {
            get
            {
                return _parentChart;
            }
            set
            {
                _parentChart = value;
            }
        }

        /// <summary>
        /// The text displayed on the tab.
        /// </summary>
        [Category("Appearance")]
        public string Title
        {
            get
            {
                if (Parent is TabPage)
                {
                    TabPage page = (TabPage)Parent;
                    return page.Text;
                }
                else
                {
                    return string.Empty;
                }
            }
            set
            {
                if (Parent is TabPage)
                {
                    TabPage page = (TabPage)Parent;
                    page.Text = value; 
                }
            }
        }
        /// <summary>
        /// Whether this UI allows the user to set the 3D model for bar graphs.
        /// </summary>
        [Category("Behavior")]
        public virtual bool BarShapeEnabled
        {
            get;
            set;
        }
        /// <summary>
        /// Whether this UI is set up for Pie/Doughnut Charts.
        /// </summary>
        [Category("Behavior")]
        public virtual bool PieAndDoughnutEnabled
        {
            get;
            set;
        }
        /// <summary>
        /// Whether this UI allows the user to set Transparency.
        /// </summary>
        [Category("Behavior")]
        public virtual bool TransparencyEnabled
        {
            get;
            set;
        }
        /// <summary>
        /// Whether the Labels checkbox is checked.
        /// </summary>
        [Browsable(false)]
        public virtual bool LabelsChecked
        {
            get;
            set;
        }
        #endregion Public Properties

        #region Protected Methods
        /// <summary>
        /// Prepares the UI for a particular chart ViewType.
        /// </summary>
        /// <param name="series">A series of the desired ViewType.</param>
        protected virtual void UpdateToViewTypeOfSeries(Series series)
        {
            if (series.View is Bar3DSeriesView)
            {
                BarShapeEnabled = true;
            }
            else
            {
                BarShapeEnabled = false;
            }

            if (series.View is BarSeriesView || series.View is Bar3DSeriesView)
            {
                List<string> barChoices = new List<string>(Enum.GetNames(typeof(BarSeriesLabelPosition)));
                if (series.View is StackedBarSeriesView || series.View is StackedBar3DSeriesView)
                {
                    barChoices.Remove("Top");
                }
                SetPositionChoices(barChoices);
            }

            if (series.View is PieSeriesViewBase)
            {
                PieAndDoughnutEnabled = true;

                List<string> pieChoices = new List<string>(Enum.GetNames(typeof(PieSeriesLabelPosition)));
                SetPositionChoices(pieChoices);
            }
            else
            {
                PieAndDoughnutEnabled = false;
            }

            if (series.View is ISupportTransparency)
            {
                TransparencyEnabled = true;
            }
            else
            {
                TransparencyEnabled = false;
            }
        }
        /// <summary>
        /// Updates the 3D model to match the user's choice.
        /// </summary>
        protected virtual void UpdateBarShape()
        {
            throw new NotImplementedException(this.ToString() + " must override UpdateBarShape");
        }
        /// <summary>
        /// Updates the BarSeriesLabelPosition to match the user's choice.
        /// </summary>
        protected virtual void UpdateBarPosition()
        {
            throw new NotImplementedException(this.ToString() + " must override UpdateBarPosition");
        }
        /// <summary>
        /// Updates the PieSeriesLabelPosition to match the user's choice.
        /// </summary>
        protected virtual void UpdatePiePosition()
        {
            throw new NotImplementedException(this.ToString() + " must override UpdatePiePosition");
        }
        /// <summary>
        /// Updates the HoleRadius to match the user's choice.
        /// </summary>
        protected virtual void UpdateHoleRadius()
        {
            throw new NotImplementedException(this.ToString() + " must override UpdateHoleRadius");
        }
        /// <summary>
        /// Updates the ExplodedDistance to match the user's choice.
        /// </summary>
        protected virtual void UpdateExplodedDistance()
        {
            throw new NotImplementedException(this.ToString() + "must override UpdateExplodedDistance");
        }
        /// <summary>
        /// Sets the options for Label Positions.
        /// </summary>
        /// <param name="choices">A list of the options.</param>
        protected virtual void SetPositionChoices(List<string> choices)
        {
            throw new NotImplementedException(this.ToString() + " must override SetPositionChoices.");
        }
        #endregion Protected Methods
    }
}
