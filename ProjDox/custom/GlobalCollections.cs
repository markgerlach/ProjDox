using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace mgCustom
{
	public class GlobalCollections
	{
		/// <summary>
		/// The System Settings collection holding onto the locked settings
		/// </summary>
		//public static SearchFavoritesCollection SearchFavoriteCollection = new SearchFavoritesCollection();
		//public static Search_SearchFavoriteCollection SearchFavoriteCollection = new Search_SearchFavoriteCollection();

		///// <summary>
		///// The Contact Collection to be used in the system
		///// </summary>
		//public static SystemSettingCollection UserSettings
		//{
		//    get { return CompleteSettingsCollection.GetBySettingsType(SystemSettingType.UserSetting); }
		//    //set { CompleteSettingsCollection.GetBySettingsType(SystemSettingType.UserSetting) = value; }
		//}

		/// <summary>
		/// The System Settings collection holding onto the locked settings
		/// </summary>
		//public static SystemSettingCollection CompleteSettingsCollection = new SystemSettingCollection();

		/// <summary>
		/// Take care of a global image collection to allow the loading of elements faster
		/// </summary>
		public static Dictionary<string, System.Drawing.Image> ImageCollection = new Dictionary<string, System.Drawing.Image>();

		///// <summary>
		///// A global collection of vendors
		///// </summary>
		//public static VendorCollection Vendors = new VendorCollection();

		///// <summary>
		///// A global collection of vendor contacts
		///// </summary>
		//public static VendorContactCollection VendorContacts = new VendorContactCollection();

		///// <summary>
		///// A global collection of vendor addresses
		///// </summary>
		//public static VendorAddressCollection VendorAddresses = new VendorAddressCollection();

		///// <summary>
		///// A global collection of vendor phones
		///// </summary>
		//public static VendorPhoneCollection VendorPhones = new VendorPhoneCollection();

		///// <summary>
		///// A global collection of purchase order contacts
		///// </summary>
		//public static PurchaseOrderContactCollection PurchaseOrderContacts = new PurchaseOrderContactCollection();

		///// <summary>
		///// A global collection of purchase order contact addresses
		///// </summary>
		//public static PurchaseOrderContactAddressCollection PurchaseOrderContactAddresses = new PurchaseOrderContactAddressCollection();

		///// <summary>
		///// A global collection of purchase order contact phones
		///// </summary>
		//public static PurchaseOrderContactPhoneCollection PurchaseOrderContactPhones = new PurchaseOrderContactPhoneCollection();
	}
}
