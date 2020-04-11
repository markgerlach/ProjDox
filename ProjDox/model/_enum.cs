using System;
using System.Reflection;
using System.Text;
using System.IO;
using System.Data;

namespace mgModel
{
	public enum ResImageSize : int
	{
		Empty = 0,
		Size16x16,
		Size24x24,
		Size32x32,
		Size48x48,
		Full
	}

	public enum VendorAddressType : int
	{
		Empty = 0,
		Work,
	}

	public enum VendorPhoneType : int
	{
		Empty = 0,
		Phone,
		Fax,
	}

	public enum POContactAddressType : int
	{
		Empty = 0,
		Work,
	}

	public enum POContactPhoneType : int
	{
		Empty = 0,
		Phone,
		Fax,
	}

	public enum ContactFormType : int
	{
		Empty = 0,
		CustomerContact,
		VendorContact,
	}

	/// <summary>
	/// The attachment screen type
	/// </summary>
	public enum AttachmentScreenType : int
	{
		Empty = 0,
		Metallurgy_LabTestEdit,
	}

	public enum LookUpEditControlType : int
	{
		Listbox = 0,
		CheckedList,
		GridView,
		CheckedGrid,
	}

	public enum DigSignContentType : int
	{
		Images,
		Video,
		WebPage,
	}

	public enum SystemConfigScreen : int
	{
		Empty = 0,
		//Desktop,
		//Display,
		//DBConnection,
		//DropDownVals,
		//FieldCaptions,
		//AddressBook,
		//Messaging,

		DatabaseConnection,
		ManageFavorites,
		DropDownValues,

		UserGroupConfig,
		UserToGroup,
		AssignByItem,
		AssignByGroupUser,
		//NavBar,
		//NavButtons,
		UserConfig,
		DataEncryption,
		//LoginOptions,
		AppEntryConfig,

		//ExportData,
		//RunSQL,
		//RunDailyOps,
		//FixSSNs,
		//TestPDFReporting,

		MarkGerlach_ImportPressData,
		MarkGerlach_ConfigureViews,
		MarkGerlach_RunSystemUtils,
		MarkGerlach_SyncTablesBetweenSavageSutton,
	}

	public enum GridLinkType : int
	{
		None = 0,
		Custom1,
		Custom2,
		Custom3,
		Custom4,
		Custom5,
		Add,
		Edit,
		Delete,
		Print,
	}

	public enum DotNetVersion : int
	{
		Unknown = 0,
		V_1_0,
		V_1_1,
		V_1_2,
		V_2_0,
		V_2_0_SP1,
		V_3_0,
		V_3_5,
		V_4_0,
	}

	public enum GridUIActionType : int
	{
		ExpandAllGroups = 0,
		CollapseAllGroups,
		ExpandAllDetails,
		CollapseAllDetails,
	}

	//public enum GridDisplayFormat : int
	//{
	//    None = 0,
	//    SSN,
	//    PhoneNumber,
	//    Currency,
	//    CurrencyNoRounding,
	//    ConciseDateTime,
	//    DateTime,
	//    FullDateTime,
	//    Date,
	//    Time,
	//    Decimal1Place,
	//    Decimal2Places,
	//    Decimal3Places,
	//    Decimal4Places,
	//    Decimal1PlaceWithPlaceholder,
	//    Decimal2PlacesWithPlaceholder,
	//    Decimal3PlacesWithPlaceholder,
	//    Decimal4PlacesWithPlaceholder,
	//    Integer,
	//    Percentage,
	//    Percentage1Decimal,
	//    Percentage2Decimals,
	//    Percentage3Decimals,
	//    Percentage4Decimals,
	//    Multiline,
	//}

	public enum GridEditorFormat : int
	{
		None = 0,
		SSN,
		PhoneNumber,
		Currency,
		DateTime,
		Date,
		Time,

		CurrencyNoRounding,
		ConciseDateTime,
		FullDateTime,
		Decimal1Place,
		Decimal2Places,
		Decimal3Places,
		Decimal4Places,
		Decimal1PlaceWithPlaceholder,
		Decimal2PlacesWithPlaceholder,
		Decimal3PlacesWithPlaceholder,
		Decimal4PlacesWithPlaceholder,
		Integer,
		Percentage,
		Percentage1Decimal,
		Percentage2Decimals,
		Percentage3Decimals,
		Percentage4Decimals,
		Multiline,
	}

	public enum GridGroupSummaryDisplayFormat : int
	{
		None = 0,
		Currency,
		CurrencyNoRounding,
		Decimal1Place,
		Decimal2Places,
		Decimal3Places,
		Decimal4Places,
		Integer,
		Percentage,
		Percentage1Decimal,
		Percentage2Decimals,
		Percentage3Decimals,
		Percentage4Decimals,
	}

	public enum mgButtonGUIButtonColor
	{
		Default = 0,
		Blue,
		Red,
		Green,
		Yellow,
	}

	public enum RichEditBarType : int
	{
		Bars = 0,
		Ribbon,
	}

	public enum _BaseWinProjectGridTypes
	{
		Empty = 0,
		BlueStandard,
		GreenStandard,
		RedStandard,
		PurpleStandard,
		OrangeStandard,
		BlueExtended,
		GreenExtended,
		RedExtended,
		PurpleExtended,
		OrangeExtended,
	}

	/// <summary>
	/// Declare the form type to 
	/// </summary>
	public enum ScreenObjectFormType : int
	{
		Empty = 0,

		// Run the following code against the database to generate this:
		// SELECT TOP 10000 [sCaseDesc] FROM [_BaseWinProject].[dbo].[vwNavBarObjectItems] ORDER BY [sCaseDesc]

		#region Elements added by the database

		AppraisalList,
		AppraisalNew,
		Appraisals,
		BreakApartList,
		BreakApartNew,
		BreakAparts,
		CustomerList,
		CustomerNew,
		Customers,
		CustomersMain,
		GiftCertificates,
		GiftCertList,
		GiftCertNew,
		GiftRegistry,
		GiftRegistryList,
		GiftRegistryNew,
		Gifts,
		InsuranceQuoteList,
		InsuranceQuoteNew,
		InsuranceQuotes,
		InsuranceSales,
		InsuranceSalesList,
		InsuranceSalesNew,
		Inventory,
		InventoryList,
		InventoryMain,
		InventoryNew,
		JobBagList,
		JobBagNew,
		JobBags,
		LayawayList,
		LayawayNew,
		Layaways,
		MemoList,
		MemoNew,
		Memorandums,
		Miscellaneous,
		PaymentList,
		PaymentNew,
		Payments,
		POList,
		PONew,
		PurchaseOrders,
		ReturnItems,
		ReturnsList,
		ReturnsNew,
		Sales,
		SalesList,
		SalesMain,
		SalesNew,
		SpecialOrderList,
		SpecialOrderNew,
		SpecialOrders,
		SupplierList,
		SupplierNew,
		Suppliers,
		TimeClock,
		TimeClockList,
		TimeClockNew,

		#endregion Elements added by the database

		#region Elements added by the developer that don't exist in the database
		//DigSignAddPackage,
		//DigSignAddEditContent,
		//DigSignEmergencyConfigEdit,

		//ForgeMetricEdit,
		//ForgeCondRateEdit,
		//SawsViewChartEdit,
		//InfoSysIPAddressMoveAnIP,
		//InfoSysIPAddressEdit,
		//InfoSysIPAddressCategoryEdit,
		//DocumentTemplateCreation,			// Used in Proof of concept for new document creation
		//DocumentTemplateEntry,				// Used in Proof of concept for an existing document being filled out
		//DocumentControlAdd,					// Used in the adding of document controls
		//DocumentControlEdit,				// Used in the editing of document controls

		//PublishControlledDocument,

		//DCOperationLogEdit,

		//SawsCutReportEdit,
		//SawsBladeChangeEdit,
		//VendorList,
		//POList,
		//ContactSelect,
		#endregion Elements added by the developer that don't exist in the database
	}

	public enum MessageBoxDontShowButtons : int
	{
		Ok,
		OkAndCancel,
		Cancel,
	}

	public enum MessageBoxKey : int
	{
		Empty = 0,
		ExportToExcelSaveFile,
	}

	public enum SawCutStructure : int
	{
		Normal = 0,
		BottomCrop,
		TopCrop,
		Rem,
		RemOnly,
	}

	public enum SawCutType : int
	{
		ByWeight = 0,
		ByLength,
		ConversionOnly,
	}

	public enum CalcButton
	{
		Empty,

		Num0,
		Num1,
		Num2,
		Num3,
		Num4,

		Num5,
		Num6,
		Num7,
		Num8,
		Num9,

		Enter,
	}

	public enum AdditionalOpIssueButtons : int
	{
		Both = 0,
		AdditionalOps,
		Issues,
	}

	public enum MotionStudyCurrentSelectJobFocus : int
	{
		None,
		JobNum,
		StepNum,
		StartingSerial,
	}

	public enum MotionStudySelectedJob : int
	{
		Empty = 0,
		Job1,
		Job2,
		Job3,
		Job4,
		Job5,
	}

	public enum MotionStudyJobSelectorDefaultLayout : int
	{
		Horizontal,
		Vertical,
	}

	public enum SecurityException
	{
		UserNotFound,
		UserNameBlank,
		PasswordCorrect,
		PasswordIncorrect,
		PasswordBlank,
	}

	public enum BatchEditType
	{
		Empty = 0,
		SawRunChartDetail,
		SawConversionMaster,
		ForgeMetric,
	}

	public enum IncomingDataType
	{
		DataPacket,
		SinglePacket,
	}

	public enum AttachmentKey : int
	{
		BestPractice,
		ControlledDocument,
		DigitalSignage,
		LabImport,
		MetricFilterNote,
		RotationalPlacement,
	}

	public enum UserGender : int
	{
		Female = 0,
		Male,
	}

	public enum SecObjectType : int
	{
		FormName = 0,
		Menu,
		NavBar,
		Screen,
	}

	public enum NotesRTFKey : int
	{
		Empty = 0,
		//PONote,
		//ForgeMetricNote,
		//ForgeCondRateNote,
		//HRApplicantNote,
	}

	public enum mgPrintOrientation : int
	{
		Portrait = 0,
		Landscape
	}

	public enum LeftNavTab
	{
		OutlookNavigation,
		TreeNavigation,
		MyViews,
		QuickViews,
	}

	public enum _BaseWinProjectScreenSize : int
	{
		FormFillsClientArea = 0,
		CenterAndSize,
		CascadeFromLastForm,
	}

	public enum ClientFormPosition : int
	{
		TopLeft = 0,
		TopCenter,
		TopRight,
		MiddleLeft,
		MiddleCenter,
		MiddleRight,
		BottomLeft,
		BottomCenter,
		BottomRight,
	}

	public enum _BaseWinProjectFilterDataType : int
	{
		Empty = 0,
		SupportDropDown,
		CustomSQL,
		Dictionary,
		List,
	}

	public enum _BaseWinProjectFilterBasicType : int
	{
		CheckedList = 0,
		StandardList,
	}

	public enum EncryptedField : int
	{
		Password = 0,
	}

	public enum EncryptAction : int
	{
		Encrypt = 0,
		Decrypt,
	}

	public enum QVType : int
	{
		// Standard Views - used by the ExecuteView Code
		Empty = 0,
	}

	public enum ActionType : int
	{
		Empty = 0,
		Login,
		Logout,
		ApplicationClose,
		Error,

		// Run System utilities
		RunSystemUtils,

		// Standard CRUD entries
		Create,
		Read,
		Update,
		Delete,

		// Other actions
		OpenScreen,
		EmailSent,
	}

	public enum SearchExControlType : int
	{
		Empty = 0,
		CheckBox,
		ComboBoxEdit,
		LookupEdit,
		GridLookupEdit,
		DateRange,
		TextBox,
		ListBox,
		UserControl,
		ucLookupEdit,
		DateTime,
		RadioGroup,
	}

	public enum SearchExTypes : int
	{
		Empty = 0,
		Basic,
		Advanced,
		Favorites,
		None,
	}

	[Flags]
	public enum SearchActionType : int
	{
		None = 0,
		Print = 1,
		Edit = 2,
		Archive = 4,
		Delete = 8,
		//BackgroundAddToList = 16,
		//BackgroundRemoveFromList = 32,
		//CopyReportTemplate = 64,
	}

	public enum SearchMatchType : int
	{
		None = 0,
		StartsWith,
		EndsWith,
		Contains,
		ExactMatch,
	}

	public enum GridDisplayFormat : int
	{
		None = 0,
		SSN,
		PhoneNumber,
		Currency,
		CurrencyNoRounding,
		ConciseDateTime,
		DateTime,
		FullDateTime,
		Date,
		Time,
		Decimal1Place,
		Decimal2Places,
		Decimal3Places,
		Decimal4Places,
		Decimal1PlaceWithPlaceholder,
		Decimal2PlacesWithPlaceholder,
		Decimal3PlacesWithPlaceholder,
		Decimal4PlacesWithPlaceholder,
		Integer,
		Percentage,
		Percentage1Decimal,
		Percentage2Decimals,
		Percentage3Decimals,
		Percentage4Decimals,
		Multiline,
	}

	public enum SystemSettingDropDown : int
	{
		Empty = 0,

		Gender,

		//HR_EEOCode,
		//HR_JobCode,
		//HR_JobCodeNoDesc,
		//HR_Ethnicity,
		//HR_ReferralSource,

		//ForgeMetricReason,
		//ForgeMetricDieSet,
	}

	[Serializable]
	public enum RefreshFrequency : int
    {
        ByDeveloperOnly = 2000000000,
        Every01Minute = 1,
        Every02Minutes = 2,
        Every03Minutes = 3,
        Every04Minutes = 4,
        Every05Minutes = 5,
        Every06Minutes = 6,
        Every07Minutes = 7,
        Every08Minutes = 8,
        Every09Minutes = 9,
        Every10Minutes = 10,
        Every15Minutes = 15,
        Every20Minutes = 20,
        Every30Minutes = 30,
        Every45Minutes = 45,
        Every60Minutes = 60,
    }

	[Serializable]
	public enum BrokenRuleType : int
	{
		Empty = 0,
		SimpleCustomRule,
		PropertyRequiredCustomRule,
		MinValueCustomRule,
		MaxValueCustomRule,
		MinMaxValueCustomRule,
		MinLengthCustomRule,
		MaxLengthCustomRule,
		DuplicateInCollectionCustomRule,
		Value1MustBeLessThanValue2CustomRule,
		RegExCustomRule,
	}

	[Serializable]
	[Flags]
	public enum RecordStatus : int
    {
        Current = 1,
        Modified = 2,
        Deleted = 4,
        New = 8,
    }

	[Serializable]
	public enum ClassGenExceptionIconType : int
	{
		None = 0,
		Default,
		Critical,
		Warning,
		Information,
		User1,
		User2,
		User3,
		System,
	}

	[Serializable]
	public enum ClassGenExceptionType : int
	{
		Empty = 0,
		Exception,
		SQLException,
		TextOnly,
		BrokenRule,
	}

	
}

