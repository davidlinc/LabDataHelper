using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DVLib.LabDataHelper;
using DVOSLib;
using MathBase;
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

		public void saveExcel(string path, int x, int y, DataConverter converter = null, string unit = null, DataConverter refConverter=null)
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
			if(refConverter!=null)
			{

				worksheet.Cells[maxY+1, xpos] = "参考值";
				worksheet.Cells[maxY + 2, xpos] = "相差";
				worksheet.Cells[maxY + 3, xpos] = "R2";
			}
			xpos++;
			int index = 0;
			foreach (var v in dataManager)
			{
				worksheet.Cells[ypos, xpos] = converter(v.Mean);
				
				double[] rdata = dataManager.getDataFromMean(index + 1, converter);
				double[] refdata = dataManager.getRefData(dataManager.getDataFromDescribe(index + 1, refConverter), rdata[0]);
				//r2
				double refd = refdata[index].keep(2);
				double readd = rdata[index].keep(2);
				double r2 = DataManager.CalculateRSquared(refdata, rdata);
				worksheet.Cells[ypos+1, xpos] = refd;

				worksheet.Cells[ypos + 2, xpos] = readd-refd;
				worksheet.Cells[ypos + 3, xpos] = r2;
				xpos++;
				index++;
			}

				wb.SaveAs(path);
			wb.Close();
			e.Quit();
		}
	}
}
