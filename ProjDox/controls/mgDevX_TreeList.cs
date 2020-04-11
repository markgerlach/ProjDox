using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using System.Data;

using DevExpress.XtraTreeList;
using DevExpress.XtraTreeList.Nodes;
using DevExpress.XtraTreeList.ViewInfo;

namespace mgControls
{
	public class mgDevX_TreeList : DevExpress.XtraTreeList.TreeList
	{
		public mgDevX_TreeList()
			: base()
		{
		}

		public mgDevX_TreeList(object ignore) : base (ignore)
		{
		}

		public void SetCheckedChildNodes(TreeListNode node, CheckState check)
		{
			for (int i = 0; i < node.Nodes.Count; i++)
			{
				node.Nodes[i].CheckState = check;
				SetCheckedChildNodes(node.Nodes[i], check);
			}
		}

		public void SetCheckedParentNodes(TreeListNode node, CheckState check)
		{
			if (node.ParentNode != null)
			{
				bool b = false;
				CheckState state;
				for (int i = 0; i < node.ParentNode.Nodes.Count; i++)
				{
					state = (CheckState)node.ParentNode.Nodes[i].CheckState;
					if (!check.Equals(state))
					{
						b = !b;
						break;
					}
				}
				node.ParentNode.CheckState = b ? CheckState.Indeterminate : check;
				SetCheckedParentNodes(node.ParentNode, check);
			}
		}

		protected override TreeListViewInfo CreateViewInfo()
		{
			return new mgDevX_TreeListViewInfo(this);
		}
	}

	public class mgDevX_TreeListViewInfo : TreeListViewInfo
	{
		public mgDevX_TreeListViewInfo(TreeList treeList) : base(treeList) { }

		protected override Point GetDataBoundsLocation(TreeListNode node, int top)
		{
			Point result = base.GetDataBoundsLocation(node, top);
			if (Size.Empty != RC.SelectImageSize && -1 == node.SelectImageIndex)
				result.X -= RC.SelectImageSize.Width;
			if (Size.Empty != RC.StateImageSize && -1 == node.StateImageIndex)
				result.X -= RC.StateImageSize.Width;
			return result;
		}

		protected override void CalcStateImage(RowInfo ri)
		{
			base.CalcStateImage(ri);
			if (Size.Empty != RC.SelectImageSize && -1 == ri.Node.SelectImageIndex)
				ri.StateImageLocation.X -= RC.SelectImageSize.Width;
		}

		protected override void CalcSelectImage(RowInfo ri)
		{
			base.CalcSelectImage(ri);
			if (-1 == ri.Node.SelectImageIndex) ri.SelectImageLocation = Point.Empty;
		}
	}

}
