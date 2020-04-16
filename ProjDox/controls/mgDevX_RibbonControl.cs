using System;
using System.Collections.Generic;
using System.Text;

using DevExpress.XtraLayout.Utils;
using DevExpress.XtraBars;
using DevExpress.XtraBars.Ribbon;
using System.Windows.Forms;

namespace mgControls
{
	public class mgDevX_RibbonControl : DevExpress.XtraBars.Ribbon.RibbonControl
	{
		public mgDevX_RibbonControl()
			: base()
		{
			// When the control is created, set the properties accordingly
			this.ToolbarLocation = RibbonQuickAccessToolbarLocation.Hidden;

			this.DoubleClick += new EventHandler(mgDevX_RibbonControl_DoubleClick);

			// Resize the buttons in the resize event
			//this.ResizeRedraw = true;
			//this.Resize += new EventHandler(mgDevX_RibbonControl_Resize);
		}

		private void mgDevX_RibbonControl_DoubleClick(object sender, EventArgs e)
		{
			// When they double-click, throw an event
			this.Minimized = false;
		}

		private void mgDevX_RibbonControl_Resize(object sender, EventArgs e)
		{
			// Draw the width of the buttons in the current page
			ResizeButtons();
		}

		/// <summary>
		/// Resize the buttons based on the width of the control
		/// </summary>
		private void ResizeButtons()
		{
			int ribbonWidth = this.Width;
			int buttonCount = 0, buttonWidth = 0;
			#region Page Categories
			if (this.PageCategories.Count > 0)
			{
				foreach (RibbonPageCategory category in this.PageCategories)
				{
					foreach (RibbonPage page in category.Pages)
					{
						buttonCount = 0;
						foreach (RibbonPageGroup group in page.Groups)
						{
							for (int i = 0; i < group.ItemLinks.Count; i++)
							{
								//if (group.ItemLinks[i] is BarButtonItem)
								if (typeof(List<object>).IsAssignableFrom(typeof(BarButtonItem)))
								{
									buttonCount++;
								}
							}
						}
						buttonWidth = (ribbonWidth / buttonCount) - (page.Groups.Count * 4);
						foreach (RibbonPageGroup group in page.Groups)
						{
							for (int i = 0; i < group.ItemLinks.Count; i++)
							{
								//if (group.ItemLinks[i] is BarButtonItem)
								if (typeof(List<object>).IsAssignableFrom(typeof(BarButtonItem))) 
								{
									((BarButtonItem)group.ItemLinks[i].Item).LargeWidth = buttonWidth;
								}
							}
						}
					}
				}
			}
			#endregion Page Categories

			#region Uncheck Standard Buttons
			foreach (RibbonPage page in this.Pages)
			{
				buttonCount = 0; 
				foreach (RibbonPageGroup group in page.Groups)
				{
					for (int i = 0; i < group.ItemLinks.Count; i++)
					{
						if (group.ItemLinks[i].Item is BarButtonItem)
						{
							buttonCount++;
						}
					}
				}
				if (buttonCount > 0)
				{
					buttonWidth = (ribbonWidth / buttonCount) - (page.Groups.Count * 4);
					foreach (RibbonPageGroup group in page.Groups)
					{
						for (int i = 0; i < group.ItemLinks.Count; i++)
						{
							if (group.ItemLinks[i].Item is BarButtonItem)
							{
								((BarButtonItem)group.ItemLinks[i].Item).LargeWidth = buttonWidth;
							}
						}
					}
				}
			}
			#endregion Uncheck Standard Buttons
		}

		/// <summary>
		/// Uncheck all the buttons
		/// </summary>
		public void UncheckButtons()
		{
			#region Page Categories
			if (this.PageCategories.Count > 0)
			{
				foreach (RibbonPageCategory category in this.PageCategories)
				{
					foreach (RibbonPage page in category.Pages)
					{
						foreach (RibbonPageGroup group in page.Groups)
						{
							for (int i = 0; i < group.ItemLinks.Count; i++)
							{
								//if (group.ItemLinks[i] is BarButtonItem)
								if (typeof(List<object>).IsAssignableFrom(typeof(BarButtonItem))) 
								{
									((BarButtonItem)group.ItemLinks[i].Item).Down = false;
								}
							}
						}
					}
				}
			}
			#endregion Page Categories

			#region Uncheck Standard Buttons
			foreach (RibbonPage page in this.Pages)
			{
				foreach (RibbonPageGroup group in page.Groups)
				{
					for (int i = 0; i < group.ItemLinks.Count; i++)
					{
						if (group.ItemLinks[i].Item is BarButtonItem)
						{
							((BarButtonItem)group.ItemLinks[i].Item).Down = false;
						}
					}
				}
			}
			#endregion Uncheck Standard Buttons
		}

		/// <summary>
		/// Click on the first visible button in the control
		/// </summary>
		public void ClickFirstVisibleButton()
		{
			BarItemLink linkItem = null;
			foreach (RibbonPage page in this.Pages)
			{
				if (page.Visible)
				{
					foreach (RibbonPageGroup group in page.Groups)
					{
						for (int i = 0; i < group.ItemLinks.Count; i++)
						{
							if (group.ItemLinks[i].Item is BarButtonItem &&
								((BarButtonItem)group.ItemLinks[i].Item).Visibility == BarItemVisibility.Always &&
								!group.ItemLinks[i].Item.Name.StartsWith("btnPrint") &&
								!group.ItemLinks[i].Item.Name.StartsWith("btnPreview") &&
								!group.ItemLinks[i].Item.Name.StartsWith("btnRun") &&
								!group.ItemLinks[i].Item.Name.Equals("btnCopyChart") &&
								!group.ItemLinks[i].Item.Name.Equals("btnSaveChart"))
							{
								linkItem = group.ItemLinks[i];
								break;
							}
						}
						if (linkItem != null) { break; }
					}
					if (linkItem != null) { break; }
				}
			}
			if (linkItem != null)
			{
				if (linkItem.Item is BarButtonItem)
				{
					((BarButtonItem)linkItem.Item).PerformClick();
				}
			}
		}

		/// <summary>
		/// Activate the button item in question
		/// </summary>
		/// <param name="btnItem">The button item to activate</param>
		public void ActivateButton(BarButtonItem btnItem)
		{
			btnItem.PerformClick();
		}

		/// <summary>
		/// Gets the currently selected button
		/// </summary>
		/// <returns></returns>
		public BarItemLink GetCurrentButton()
		{
			mgDevX_RibbonControlBarItemCollection buttonArray = GetBarButtonItems();
			BarItemLink btnItem = null;

			//int targetItem = -1;
			foreach (mgDevX_RibbonControlBarItem kvp in buttonArray)
			{
				if (((BarButtonItem)kvp.BarItemLink.Item).Down &&
					kvp.Page == this.SelectedPage)
				{
					btnItem = kvp.BarItemLink;
					break;
				}
			}

			return btnItem;
		}

		/// <summary>
		/// Get the next element in the ribbon control
		/// </summary>
		public void GetNextButton()
		{
			mgDevX_RibbonControlBarItemCollection buttonArray = GetBarButtonItems();

			int targetItem = -1;
			foreach (mgDevX_RibbonControlBarItem kvp in buttonArray)
			{
				//if (((BarButtonItem)kvp.BarItemLink.Item).Down && 
				//    kvp.Page == this.SelectedPage
				if (((BarButtonItem)kvp.BarItemLink.Item).Down)
				{
					targetItem = buttonArray.IndexOf(kvp) + 1;
					break;
				}
			}
			if (targetItem > buttonArray.Count - 1) { targetItem = 0; }

			// Make the page visible
			ActivatePageAndGroup(buttonArray[targetItem]);

			// Call the click event
			buttonArray[targetItem].BarItemLink.Item.PerformClick();

			buttonArray = null;
		}

		/// <summary>
		/// Get the prev element in the ribbon control
		/// </summary>
		public void GetPrevButton()
		{
			mgDevX_RibbonControlBarItemCollection buttonArray = GetBarButtonItems();

			int targetItem = -1;
			foreach (mgDevX_RibbonControlBarItem kvp in buttonArray)
			{
				//if (((BarButtonItem)kvp.BarItemLink.Item).Down &&
				//    kvp.Page == this.SelectedPage)
				if (((BarButtonItem)kvp.BarItemLink.Item).Down)
				{
					targetItem = buttonArray.IndexOf(kvp) - 1;
					break;
				}
			}
			if (targetItem < 0) { targetItem = buttonArray.Count - 1; }

			// Make the page visible
			ActivatePageAndGroup(buttonArray[targetItem]);

			// Call the click event
			buttonArray[targetItem].BarItemLink.Item.PerformClick();

			buttonArray = null;
		}

		/// <summary>
		/// Activate the page and group
		/// </summary>
		/// <param name="link">The link to target</param>
		private void ActivatePageAndGroup(mgDevX_RibbonControlBarItem item)
		{
			this.SelectedPage = item.Page;
			#region Page Categories
			//if (this.PageCategories.Count > 0)
			//{
			//    foreach (RibbonPageCategory category in this.PageCategories)
			//    {
			//        foreach (RibbonPage page in category.Pages)
			//        {
			//            foreach (RibbonPageGroup group in page.Groups)
			//            {
			//                for (int i = 0; i < group.ItemLinks.Count; i++)
			//                {
			//                    if (group.ItemLinks[i] == link)
			//                    {
			//                        // Activate the page and group
			//                        this.SelectedPage = page;
			//                        break;
			//                    }
			//                }
			//            }
			//        }
			//    }
			//}
			//#endregion Page Categories

			//#region Uncheck Standard Buttons
			//foreach (RibbonPage page in this.Pages)
			//{
			//    foreach (RibbonPageGroup group in page.Groups)
			//    {
			//        for (int i = 0; i < group.ItemLinks.Count; i++)
			//        {
			//            if (group.ItemLinks[i] == link)
			//            {
			//                // Activate the page and group
			//                this.SelectedPage = page;
			//                break;
			//            }
			//        }
			//    }
			//}
			#endregion Uncheck Standard Buttons
		}

		/// <summary>
		/// Get the button items on the bar
		/// </summary>
		/// <returns>A collection of the bar Item Links</returns>
		//private Dictionary<int, BarItemLink> GetBarButtonItems()
		public mgDevX_RibbonControlBarItemCollection GetBarButtonItems()
		{
			mgDevX_RibbonControlBarItemCollection buttonArray = new mgDevX_RibbonControlBarItemCollection();

			#region Page Categories
			if (this.PageCategories.Count > 0)
			{
				foreach (RibbonPageCategory category in this.PageCategories)
				{
					foreach (RibbonPage page in category.Pages)
					{
						if (page.Visible)
						{
							foreach (RibbonPageGroup group in page.Groups)
							{
								if (group.Visible)
								{
									for (int i = 0; i < group.ItemLinks.Count; i++)
									{
										if (group.ItemLinks[i].Item is BarButtonItem &&
											((BarButtonItem)group.ItemLinks[i].Item).Visibility != BarItemVisibility.Never &&
											!group.ItemLinks[i].Item.Name.StartsWith("btnPrint") &&
											!group.ItemLinks[i].Item.Name.StartsWith("btnPreview") &&
											!group.ItemLinks[i].Item.Name.StartsWith("btnRun") &&
											!group.ItemLinks[i].Item.Name.Equals("btnCopyChart") &&
											!group.ItemLinks[i].Item.Name.Equals("btnSaveChart"))
										{
											//((BarButtonItem)group.ItemLinks[i].Item).Down = false;
											//buttonArray.Add(buttonArray.Count, group.ItemLinks[i]);
											buttonArray.Add(new mgDevX_RibbonControlBarItem(page,
												group,
												group.ItemLinks[i]));

										}
									}
								}
							}
						}
					}
				}
			}
			#endregion Page Categories

			#region Uncheck Standard Buttons
			if (this.ShowPageHeadersMode != DevExpress.XtraBars.Ribbon.ShowPageHeadersMode.Hide)
			{
				foreach (RibbonPage page in this.Pages)
				{
					if (page.Visible)
					{
						foreach (RibbonPageGroup group in page.Groups)
						{
							if (group.Visible)
							{
								for (int i = 0; i < group.ItemLinks.Count; i++)
								{
									if (group.ItemLinks[i].Item is BarButtonItem &&
										((BarButtonItem)group.ItemLinks[i].Item).Visibility != BarItemVisibility.Never &&
										!group.ItemLinks[i].Item.Name.StartsWith("btnPrint") &&
										!group.ItemLinks[i].Item.Name.StartsWith("btnPreview") &&
										!group.ItemLinks[i].Item.Name.StartsWith("btnRun") &&
										!group.ItemLinks[i].Item.Name.Equals("btnCopyChart") &&
										!group.ItemLinks[i].Item.Name.Equals("btnSaveChart"))
									{
										//((BarButtonItem)group.ItemLinks[i].Item).Down = false;
										//buttonArray.Add(buttonArray.Count, group.ItemLinks[i]);
										buttonArray.Add(new mgDevX_RibbonControlBarItem(page,
											group,
											group.ItemLinks[i]));
									}
								}
							}
						}
					}
				}
			}
			else
			{
				RibbonPage page = this.SelectedPage;
				foreach (RibbonPageGroup group in page.Groups)
				{
					if (group.Visible)
					{
						for (int i = 0; i < group.ItemLinks.Count; i++)
						{
							if (group.ItemLinks[i].Item is BarButtonItem &&
								((BarButtonItem)group.ItemLinks[i].Item).Visibility != BarItemVisibility.Never &&
								!group.ItemLinks[i].Item.Name.StartsWith("btnPrint") &&
								!group.ItemLinks[i].Item.Name.StartsWith("btnPreview") &&
								!group.ItemLinks[i].Item.Name.StartsWith("btnRun") &&
								!group.ItemLinks[i].Item.Name.Equals("btnCopyChart") &&
								!group.ItemLinks[i].Item.Name.Equals("btnSaveChart"))
							{
								//((BarButtonItem)group.ItemLinks[i].Item).Down = false;
								//buttonArray.Add(buttonArray.Count, group.ItemLinks[i]);
								buttonArray.Add(new mgDevX_RibbonControlBarItem(page,
									group,
									group.ItemLinks[i]));
							}
						}
					}
				}
			}
			#endregion Uncheck Standard Buttons

			return buttonArray;
		}

		/// <summary>
		/// Handle the bar items in the list
		/// </summary>
		public class mgDevX_RibbonControlBarItem
		{
			private RibbonPage _page = null;
			private RibbonPageGroup _pageGroup = null;
			private BarItemLink _barItemLink = null;
			private int _pageIndex = -1;
			private int _itemIndex = -1;

			public mgDevX_RibbonControlBarItem(RibbonPage page, 
				RibbonPageGroup group,
				BarItemLink barItemLink)
			{
				_page = page;
				_pageGroup = group;
				_barItemLink = barItemLink;
			}

			public RibbonPage Page
			{
				get { return _page; }
				set { _page = value; }
			}

			public RibbonPageGroup PageGroup
			{
				get { return _pageGroup; }
				set { _pageGroup = value; }
			}

			public BarItemLink BarItemLink
			{
				get { return _barItemLink; }
				set { _barItemLink = value; }
			}

			public int PageIndex
			{
				get { return _pageIndex; }
				set { _pageIndex = value; }
			}

			public int ItemIndex
			{
				get { return _itemIndex; }
				set { _itemIndex = value; }
			}
		}

		public class mgDevX_RibbonControlBarItemCollection : List<mgDevX_RibbonControlBarItem>
		{
			private Dictionary<int, mgDevX_RibbonControlBarItem> _keyedCollection = new Dictionary<int, mgDevX_RibbonControlBarItem>();

			public mgDevX_RibbonControlBarItemCollection()
			{
			}

			// Add Method
			public new void Add(mgDevX_RibbonControlBarItem item)
			{
				this.Insert(this.Count, item);
			}

			// AddRange Method
			public new void AddRange(IEnumerable<mgDevX_RibbonControlBarItem> collection)
			{
				foreach (mgDevX_RibbonControlBarItem item in collection)
				{
					this.Add(item);
				}
			}

			// Clear method
			public new void Clear()
			{
				base.Clear();		// Call the base method
				_keyedCollection.Clear();
			}

			//// Alternate Contains Method
			//public bool Contains(string key)
			//{
			//    return _keyedCollection.ContainsKey(key);
			//}

			// Insert Method
			public new void Insert(int index, mgDevX_RibbonControlBarItem item)
			{
				// When the item is added, find out where in the collection it is and change its index
				//Console.WriteLine(item.Page.Name);
				//Console.WriteLine(item.BarItemLink.Item.Name);

				item.PageIndex = GetPageIndex(item.Page);
				item.ItemIndex = GetItemIndex(item.Page, item.BarItemLink);
				
				base.Insert(index, item);		// Call the base method
				
				_keyedCollection.Add(_keyedCollection.Count, item);
			}

			/// <summary>
			/// Get the page index for the page specified within the current collection
			/// </summary>
			public int GetPageIndex(RibbonPage page)
			{
				int rtv = 0;

				foreach (mgDevX_RibbonControlBarItem item in this)
				{
					if (item.Page == page)
					{
						rtv = item.PageIndex;
						break;
					}
				}

				return rtv;
			}

			/// <summary>
			/// Get the page index for the page specified within the current collection
			/// </summary>
			public int GetItemIndex(RibbonPage page, BarItemLink barItem)
			{
				int rtv = 0;

				int count = 0;
				foreach (mgDevX_RibbonControlBarItem item in this)
				{
					if (item.Page == page)
					{
						if (item.BarItemLink == barItem)
						{
							rtv = item.PageIndex;
							break;
						}
						else
						{
							count++;
						}
					}
				}
				if (count > 0) { rtv = count; }

				return rtv;
			}

			// InsertRange Method
			public new void InsertRange(int index, IEnumerable<mgDevX_RibbonControlBarItem> collection)
			{
				int count = 0;
				foreach (mgDevX_RibbonControlBarItem item in collection)
				{
					this.Insert(index + count, item);
					count++;
				}
			}

			// Remove Method
			public new bool Remove(mgDevX_RibbonControlBarItem item)
			{
				return base.Remove(item);		// Call the base method
			}

			// RemoveAll
			public new int RemoveAll(Predicate<mgDevX_RibbonControlBarItem> match)
			{
				int count = 0;
				foreach (mgDevX_RibbonControlBarItem item in this)
				{
					if (match.Invoke(item))
					{
						this.Remove(item);
						count++;
					}
				}
				return count;
			}

			// RemoveAt method
			public new void RemoveAt(int index)
			{
				this.Remove(this[index]);
			}

			// Remove Range Method
			public new void RemoveRange(int index, int count)
			{
				for (int i = index + count - 1; i >= 0; i--)
				{
					this.Remove(this[i]);
				}
			}

		}
	}	
}
