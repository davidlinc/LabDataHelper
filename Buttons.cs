using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LabDataHelper
{
	internal class buttonInfo
	{
		internal string text;
		internal Action onPress;
	}
	public class Buttons
	{

	public	int buttonCount { get; private set; } = 10;

		public int page { get; private set; } = 0;

		public int currentCount { get; private set; } = 0;

		public int maxPage { get; private set; } = 0;

		List<buttonInfo> list=new List<buttonInfo>();

		public void clear()
		{
			list.Clear();
		}

		void update()
		{
			maxPage = list.Count / buttonCount;
			int rest = list.Count % buttonCount;
			if (rest!= 0)
			{
				maxPage++;
			}

			maxPage = Math.Max(maxPage, 1);

			if (page >= maxPage)
			{
				page = 0;
			}
			else if (page < 0)
			{
				page = maxPage - 1;
			}

			if (page == maxPage - 1)
			{
				if(rest>0)
				{
					currentCount = rest;
				}
				else
				{
					currentCount = buttonCount;
				}
			}
		}

		public void setPage(int page) { 
		this.page = page;
			update();
		}

		public void nextPage()
		{
			page++;
			update();
		}
		public void lastPage()
		{
			page--;
			update();
		}
		public void onPress(int index)
		{
			index=page*buttonCount+index;

			if(index<list.Count)
			{
				list[index].onPress();
			}
		}

		public string getName(int index)
		{
			index = page * buttonCount + index;

			if (index < list.Count)
			{
				return list[index].text;
			}

			return "";
		}

		public void addButton(string text, Action action)
		{
			list.Add(new buttonInfo() { text = text,onPress=action });
		}

		

	}
}
