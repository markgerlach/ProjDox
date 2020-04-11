using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Xml;

using DevExpress.XtraCharts;

using mgWinChart.Helpers;
using mgWinChart.Interfaces;
using mgWinChart.SerializationExample;

namespace mgWinChart
{
    [Serializable]
    public class VisualSettingsObject
    {
        public const double CURRENT_VERSION = 1.0;

        #region Private Variables

        private ViewType _viewType = ViewType.Bar;
        private bool _labelsVisible = true;

        private string _chartTitle = string.Empty;
        private bool _valueAsPercent = false;
        private LegendPosition _legendPosition = LegendPosition.None;
		//private ChartAppearance _chartAppearance = ChartAppearance.NATURE_COLORS;
		private Palette _palette = null;

        private LabelAngle _labelAngle = LabelAngle.Angle_000;
		private mgWinChart.Helpers.MarkerKind _markerkind = mgWinChart.Helpers.MarkerKind.Circle;
        private MarkerSize _markerSize = MarkerSize.Size_08;
        private bool _showMarkers = true;

        private bool _primaryAxisXLabelsVisible = true;

        private LabelAngle _xAxisLabelAngle = LabelAngle.Angle_000;
        private int _xAxisLabelTickScale = 1;
        private AxisAlignment _primaryAxisXAlignment = AxisAlignment.Near;
        private SecondaryAxisX _secondaryAxisX = new SecondaryAxisX("xX");
        private SecondaryAxisY _secondaryAxisY = new SecondaryAxisY("yY");
        private string _primaryAxisYTitle = "";
        private string _secondaryAxisYTitle = "";
        private bool _axesAligned = false;
        private double _axesAlignedAtValue = 0.0;
        private double _axesScale = 1.0;
        private NumericFormat _primaryFormat = NumericFormat.General;
        private NumericFormat _secondaryFormat = NumericFormat.General;
        private int _primaryFormatPrecision = -1;
        private int _secondaryFormatPrecision = -1;

        private Bar3DModel _modelType = Bar3DModel.Box;
        private BarSeriesLabelPosition _barLabelPosition = BarSeriesLabelPosition.Top;

        private uint _explodeDistance = 5;
        private PieExplodeMode _explodedMode = PieExplodeMode.None;
        private string _argumentExploded = string.Empty;
        private PieSeriesLabelPosition _pieLabelPosition = PieSeriesLabelPosition.Radial;
        private HoleRadius _holeRadius = HoleRadius.Radius_050;

        private bool _showOnlyTopRecords = false;
        private uint _numOfRecordsToShow = 5;
        private bool _showOther = true;

        private string _printingTitle = "";
        private string _printingFilter = "";

        #endregion Private Variables

        #region Constructors

        public VisualSettingsObject()
        {
            mgInit();
        }
        public void mgInit()
        {
            //nothing yet.
        }

        public static VisualSettingsObject DeserializeNewObjectFromXML(string xml)
        {
            /*
            XmlDocument doc = new XmlDocument();
            doc.LoadXml(xml);
            if (doc.DocumentElement.HasAttribute("version") && doc.DocumentElement.GetAttribute("version") == "1.0")
            {
                //do nothing for now.
            }
            */

            return GenericSerializer<VisualSettingsObject>.Deserialize(xml);
        }

        #endregion Constructors

        #region Public Properties

        [DefaultValue(ViewType.Bar)]
        public ViewType ViewType
        {
            get
            {
                return _viewType;
            }
            set
            {
                _viewType = value;
            }
        }

        [DefaultValue(true)]
        public bool LabelsVisible
        {
            get
            {
                return _labelsVisible;
            }
            set
            {
                _labelsVisible = value;
            }
        }

        #region IChartingBase Support

        [DefaultValue("")]
        public string ChartTitle
        {
            get
            {
                return _chartTitle;
            }
            set
            {
                _chartTitle = value;
            }
        }

        [DefaultValue(false)]
        public bool ValueAsPercent
        {
            get
            {
                return _valueAsPercent;
            }
            set
            {
                _valueAsPercent = value;
            }
        }

        [DefaultValue(LegendPosition.None)]
        public LegendPosition LegendPosition
        {
            get
            {
                return _legendPosition;
            }
            set
            {
                _legendPosition = value;
            }
        }

		//[DefaultValue(ChartAppearance.NATURE_COLORS)]
        public Palette Palette
        {
            get
            {
                return _palette;
            }
            set
            {
				_palette = value;
            }
        }

        #endregion IChartingBase Support

        #region IAreaChart Support

        [DefaultValue(LabelAngle.Angle_000)]
        public LabelAngle LabelAngle
        {
            get
            {
                return _labelAngle;
            }
            set
            {
                _labelAngle = value;
            }
        }

        [DefaultValue(Helpers.MarkerKind.Circle)]
        public Helpers.MarkerKind MarkerKind
        {
            get
            {
                return _markerkind;
            }
            set
            {
                _markerkind = value;
            }
        }

        [DefaultValue(MarkerSize.Size_08)]
        public MarkerSize MarkerSize
        {
            get
            {
                return _markerSize;
            }
            set
            {
                _markerSize = value;
            }
        }

        [DefaultValue(true)]
        public bool ShowMarkers
        {
            get
            {
                return _showMarkers;
            }
            set
            {
                _showMarkers = value;
            }
        }

        [DefaultValue(1)]
        public int XAxisLabelTickScale
        {
            get
            {
                return _xAxisLabelTickScale;
            }
            set
            {
                _xAxisLabelTickScale = value;
            }
        }

        [DefaultValue(true)]
        public bool PrimaryAxisXLabelsVisible
        {
            get { return _primaryAxisXLabelsVisible; }
            set { _primaryAxisXLabelsVisible = value; }
        }

        [DefaultValue(LabelAngle.Angle_000)]
        public LabelAngle XAxisLabelAngle
        {
            get
            {
                return _xAxisLabelAngle;
            }
            set
            {
                _xAxisLabelAngle = value;
            }
        }

        [DefaultValue(AxisAlignment.Near)]
        public AxisAlignment PrimaryAxisXAlignment
        {
            get
            {
                return _primaryAxisXAlignment;
            }
            set
            {
                _primaryAxisXAlignment = value;
            }
        }
        

        public SecondaryAxisX SecondaryAxisX
        {
            get
            {
                return _secondaryAxisX;
            }
            set
            {
                _secondaryAxisX = value;
            }
        }

        public SecondaryAxisY SecondaryAxisY
        {
            get
            {
                return _secondaryAxisY;
            }
            set
            {
                _secondaryAxisY = value;
            }
        }

        [DefaultValue("")]
        public string PrimaryAxisYTitle
        {
            get
            {
                return _primaryAxisYTitle;
            }
            set
            {
                _primaryAxisYTitle = value;
            }
        }

        [DefaultValue("")]
        public string SecondaryAxisYTitle
        {
            get
            {
                return _secondaryAxisYTitle;
            }
            set
            {
                _secondaryAxisYTitle = value;
            }
        }

        [DefaultValue(false)]
        public bool AxesAligned
        {
            get
            {
                return _axesAligned;
            }
            set
            {
                _axesAligned = value;
            }
        }

        [DefaultValue(0.0)]
        public double AxesAlignedAtValue
        {
            get
            {
                return _axesAlignedAtValue;
            }
            set
            {
                _axesAlignedAtValue = value;
            }
        }

        [DefaultValue(1.0)]
        public double AxesScale
        {
            get
            {
                return _axesScale;
            }
            set
            {
                if (value > 0)
                {
                    _axesScale = value;
                }
                else
                {
                    _axesScale = 1.0;
                }
            }
        }

        [DefaultValue(NumericFormat.General)]
        public NumericFormat PrimaryFormat
        {
            get
            {
                return _primaryFormat;
            }
            set
            {
                _primaryFormat = value;
            }
        }

        [DefaultValue(NumericFormat.General)]
        public NumericFormat SecondaryFormat
        {
            get
            {
                return _secondaryFormat;
            }
            set
            {
                _secondaryFormat = value;
            }
        }

        [DefaultValue(-1)]
        public int PrimaryFormatPrecision
        {
            get
            {
                return _primaryFormatPrecision;
            }
            set
            {
                if (value < 0)
                {
                    _primaryFormatPrecision = -1;
                }
                else
                {
                    _primaryFormatPrecision = value;
                }
            }
        }
        
        [DefaultValue(-1)]
        public int SecondaryFormatPrecision
        {
            get
            {
                return _secondaryFormatPrecision;
            }
            set
            {
                if (value < 0)
                {
                    _secondaryFormatPrecision = -1;
                }
                else
                {
                    _secondaryFormatPrecision = value;
                }
            }
        }

        #endregion IAreaChart Support

        #region IBarChart Support

        [DefaultValue(Bar3DModel.Box)]
        public Bar3DModel ModelType
        {
            get
            {
                return _modelType;
            }
            set
            {
                _modelType = value;
            }
        }

        [DefaultValue(BarSeriesLabelPosition.Top)]
        public BarSeriesLabelPosition BarLabelPosition
        {
            get
            {
                return _barLabelPosition;
            }
            set
            {
                _barLabelPosition = value;
            }
        }

        #endregion IAreaChart Support

        #region IPieDoughnut Support

        [DefaultValue(5)]
        public uint ExplodeDistance
        {
            get
            {
                return _explodeDistance;
            }
            set
            {
                _explodeDistance = value;
            }
        }

        [DefaultValue(PieExplodeMode.None)]
        public PieExplodeMode ExplodeMode
        {
            get
            {
                return _explodedMode;
            }
            set
            {
                _explodedMode = value;
            }
        }

        [DefaultValue("")]
        public string ArgumentExploded
        {
            get
            {
                return _argumentExploded;
            }
            set
            {
                _argumentExploded = value;
            }
        }

        [DefaultValue(PieSeriesLabelPosition.Radial)]
        public PieSeriesLabelPosition PieLabelPosition
        {
            get
            {
                return _pieLabelPosition;
            }
            set
            {
                _pieLabelPosition = value;
            }
        }

        [DefaultValue(HoleRadius.Radius_050)]
        public HoleRadius HoleRadius
        {
            get
            {
                return _holeRadius;
            }
            set
            {
                _holeRadius = value;
            }
        }

        #endregion IPieDoughnut Support

        #region TopNRecords Support

        [DefaultValue(false)]
        public bool ShowOnlyTopRecords
        {
            get
            {
                return _showOnlyTopRecords;
            }
            set
            {
                _showOnlyTopRecords = value;
            }
        }

        [DefaultValue(5)]
        public uint NumberOfRecordsToShow
        {
            get
            {
                return _numOfRecordsToShow;
            }
            set
            {
                if (value < 1)
                {
                    _numOfRecordsToShow = 1;
                }
                else
                {
                    _numOfRecordsToShow = value;
                }
            }
        }

        [DefaultValue(true)]
        public bool ShowOther
        {
            get
            {
                return _showOther;
            }
            set
            {
                _showOther = value;
            }
        }

        #endregion TopNRecords Support

        #region Printing

        [DefaultValue("")]
        public string PrintingTitle
        {
            get
            {
                return _printingTitle;
            }
            set
            {
                _printingTitle = value;
            }
        }

        [DefaultValue("")]
        public string PrintingFilter
        {
            get
            {
                return _printingFilter;
            }
            set
            {
                _printingFilter = value;
            }
        }

        #endregion Printing

        #region Serialization Support (Commented Out)
        /*
        /// <summary>
        /// The View Type as a string value.
        /// Used for Serialization.
        /// </summary>
        [DefaultValue("Bar")]
        public string ViewTypeAsString
        {
            get
            {
                return GenericEnumConverter.StringForValue<ViewType>(_viewType);
            }
            set
            {
                _viewType = GenericEnumConverter.ValueForString<ViewType>(value);
            }
        }
        
        /// <summary>
        /// The shape used for 3D bar graphs, as a string value.
        /// Used for Serialization.
        /// </summary>
        [DefaultValue("Box")]
        public string ModelTypeAsString
        {
            get
            {
                return GenericEnumConverter.StringForValue<Bar3DModel>(_modelType);
            }
            set
            {
                _modelType = GenericEnumConverter.ValueForString<Bar3DModel>(value);
            }
        }
        
        /// <summary>
        /// The placement of labels for bar series, as a string value.
        /// Used for Serialization.
        /// </summary>
        [DefaultValue("Top")]
        public string BarLabelPositionAsString
        {
            get
            {
                return GenericEnumConverter.StringForValue<BarSeriesLabelPosition>(_barLabelPosition);
            }
            set
            {
                _barLabelPosition = GenericEnumConverter.ValueForString<BarSeriesLabelPosition>(value);
            }
        }
        
        /// <summary>
        /// The position of labels for Pie and Doughnut series, as a string value.
        /// Used for Serialization.
        /// </summary>
        [DefaultValue("Radial")]
        public string PieLabelPositionAsString
        {
            get
            {
                return GenericEnumConverter.StringForValue<PieSeriesLabelPosition>(_pieLabelPosition);
            }
            set
            {
                _pieLabelPosition = GenericEnumConverter.ValueForString<PieSeriesLabelPosition>(value);
            }
        }
         */
        #endregion Serialization Support

        #endregion Public Properties

    }
}
