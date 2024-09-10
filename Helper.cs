using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DVLib.LabDataHelper;
using DVOSLib;
using Microsoft.Office.Interop.Excel;
using App = Microsoft.Office.Interop.Excel.Application;

namespace LabDataHelper
{

	public class RichBoxHelper
	{
		/*RichTextBox rich;
		Dictionary<>*/
		/*public RichBoxHelper(RichTextBox textBox)
		{
			this.rich = textBox;
		}*/
		StringBuilder stringBuilder = new StringBuilder();
		int lines;

	}
	public class Helper
	{
		DataManager dataManager;
		public Helper(DataManager dataManager)
		{
			this.dataManager = dataManager;
		}

		public void saveExcel(string path,int x,int y,DataConverter converter=null,string unit=null,int baseIndex=-1)
		{
			if(converter==null)
			{
				converter = (d) => d;
			}
			App e = new App();
			Workbook wb=e.Workbooks.Add();
			Worksheet worksheet = (Worksheet)wb.Sheets[1];
			int ypos, xpos;
			xpos = x+1;
			int maxY = 0;
			foreach(var v in dataManager)
			{
				ypos = y;
				worksheet.Cells[ypos, xpos] = v.name+(unit!=null?"("+unit+")":(""));
				ypos++;
				worksheet.Cells[ypos, xpos] = v.describe;
				ypos++;
				foreach (var v2 in v)
				{
					worksheet.Cells[ypos,xpos] =converter(v2);
					ypos++;
				}

				if(ypos>maxY)
				{
					maxY = ypos; 
				}
				xpos++;
			}
			xpos = x;
			ypos = maxY;
			worksheet.Cells[maxY, xpos] = "平均数";
			worksheet.Cells[maxY+1, xpos] = "最大偏差";
			DataSet baseData=null;
			if(baseIndex>-1)
			{

				worksheet.Cells[maxY + 2, xpos] = "与基准偏差";
				baseData = dataManager[baseIndex];
			}
			xpos++;
			foreach (var v in dataManager)
			{
				worksheet.Cells[ypos, xpos] = converter(v.Mean);
				worksheet.Cells[ypos+1, xpos] = converter(v.Max-v.Min);
				if(baseData!=null)
				{ 
					worksheet.Cells[ypos + 2, xpos] = converter(v.Mean-baseData.Mean);
				}
				xpos++;
			}

				wb.SaveAs(path);
			wb.Close();
			e.Quit();
		}
	}
}
