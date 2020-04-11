using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace mgWinChart.Helpers
{
    /// <summary>
    /// The many types of positions on the chart control where the label can appear.
    /// </summary>
    public enum LabelPosition
    {
        None,
        Top,
        TopInside,
        Center,
        BottomInside,
    }

    /// <summary>
    /// The many types of positions on the chart control where the label can appear.
    /// </summary>
    public enum PieDoughnutLabelPosition
    {
		None,
        Inside,
        Outside,
        TwoColumnsRadial,
    }

    /// <summary>
    /// The position where the legend is displayed on the chart control.
    /// </summary>
    [Serializable]
    public enum LegendPosition
    {
        None,
        UpperLeft,
        CenteredLeft,
        LowerLeft,
        TopCenter,
        BottomCenter,
        UpperRight,
        CenteredRight,
        LowerRight
    }

	///// <summary>
	///// The type of appearance you would like to set for your chart control.
	///// </summary>
	//[Serializable]
	//public enum ChartAppearance
	//{
	//    CHAMELEON,
	//    DARK,
	//    DARK_FLAT,
	//    GRAY,
	//    IN_A_FOG,
	//    LIGHT,
	//    NATURE_COLORS,
	//    NORTHERN_LIGHTS,
	//    PASTEL_KIT,
	//    TERRACOTTA_PIE,
	//    THE_TREES
	//}

    /// <summary>
    /// The type of point marker to display on an area chart.
    /// </summary>
    [Serializable]
    public enum MarkerKind
    {
        Square,
        Diamond,
        Triangle,
        InvertedTriangle,
        Circle,
        Plus,
        Cross,
        Star3Points,
		Star4Points,
		Star5Points,
		Star6Points,
		Star10Points,
        Pentagon,
        Hexagon
    }

    /// <summary>
    /// The size of the marker displayed on an area chart.
    /// </summary>
    [Serializable]
    public enum MarkerSize
    {
        Size_08 = 8,
        Size_10 = 10,
        Size_12 = 12,
        Size_14 = 14,
        Size_16 = 16,
        Size_18 = 18,
        Size_20 = 20,
        Size_22 = 22,
        Size_24 = 24,
        Size_26 = 26,
        Size_28 = 28,
        Size_30 = 30
    }

    /// <summary>
    /// The angle that the label is displayed on the area chart.
    /// </summary>
    [Serializable]
    public enum LabelAngle
    {
        Angle_000 = 0,
        Angle_045 = 45,
        Angle_090 = 90,
        Angle_135 = 135,
        Angle_180 = 180,
        Angle_225 = 225,
        Angle_270 = 270,
        Angle_315 = 315
    }

    /// <summary>
    /// The radius distance made in the doughbat
    /// </summary>
    [Serializable]
    public enum HoleRadius
    {
        Radius_000 = 0,
        Radius_015 = 15,
        Radius_030 = 30,
        Radius_050 = 50,
        Radius_060 = 60,
        Radius_075 = 75,
        Radius_090 = 90,
        Radius_100 = 100
    }

    /// <summary>
    /// The type of 3D modeling we want to show for a bar graph.
    /// </summary>
    [Serializable]
    public enum ModelType3D
    {
        Box,
        Cylinder,
        Cone,
        Pyramid,
    }
}