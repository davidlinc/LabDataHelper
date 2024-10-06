using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Runtime.Intrinsics;
using System.Runtime.Intrinsics.X86;
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
				double[] refdata = dataManager.getDataFromDescribe(index + 1, refConverter);
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
		public static Complex save;

		public static Vector256<double> save2;
		public unsafe static Complex[] mul(Complex[] a, Complex[] b)
		{
			Complex[] re = new Complex[a.Length];
			Complex tempa,tempb;
			for (int i = 0; i < a.Length; i++)
			{
				re[i] = a[i] * b[i];	}

			return re;
		}
		public unsafe static Complex[] mul3(Complex[] a, Complex[] b)
		{
			Complex[] re = new Complex[a.Length];
			Complex tempa, tempb;
			for (int i = 0; i < a.Length; i++)
			{
				tempa = a[i];
				tempb = b[i];
				re[i] = new Complex(-tempa.realPart * tempb.realPart * tempa.realPart * tempb.imaginaryPart + tempb.realPart * tempa.imaginaryPart); ;
			}

			return re;
		}
		public unsafe static double muldouble(double[] a, double[] b)
		{
			double sum = 0; ;
			for (int i = 0; i < a.Length; i++)
			{

				sum+= a[i] * b[i];
			}

			return sum;
		}
		public unsafe static double muldouble2(double[] a, double[] b)
		{
			double[] re= new double[4];
			double sum=0;
			fixed(double* a_=a,b_=b,r_=re)
			{
				Vector256<double>* ad=(Vector256<double>*)a_;
				Vector256<double>* bd=(Vector256<double>*)b_;
				Vector256<double>* rd = (Vector256<double>*)r_;
				int c = a.Length >>2;
				int left = a.Length - (c << 2);
	         for (int i = 0; i < c; i++)
			{
				*(rd)=Avx.Add( Avx.Multiply(*(ad + i), *(bd + i)),*rd);
			}
			for(int i=a.Length-left;i<a.Length;i++)
				{

					sum+= a[i] * b[i];
				}
			
			}
		

			return sum+re[0]+re[1]+re[2]+re[3];
		}
		public unsafe static Complex[] mul2(Complex[] a, Complex[] b)
		{
			Complex[] re = new Complex[a.Length];
			int li = a.Length - 1;
			fixed (Complex* a_ = a, b_ = b, re_ = re)
			{
				Vector256<double>* sre = (Vector256<double>*)re_;
				Vector256<double>* sa = (Vector256<double>*)a_;
				Vector256<double>* sb = (Vector256<double>*)b_;
				//Span<Vector256<double>> span = MemoryMarshal.Cast<Complex, Vector256<double>>(re);
				Vector256<double> M1=new(), M2=new();
				var mask = Vector256.Create(0.0, -0.0, 0.0, -0.0);
				int t = a.Length >> 1;
				int left = a.Length - (t << 1);
				for (int i = 0; i < t; i++)
				{
					M1 = *(sa + i);
					M2 = *(sb + i);
					Avx.Store((double*)(sre + i), Avx.HorizontalAdd(Avx.Multiply(Avx.Xor(M1, mask), M2), Avx.Multiply(M1, Avx2.Permute4x64(M2, 0b10110001))));
				}
				if (left > 0)
				{
					re[li] = a[li] * b[li];
				}
			}

			return re;
		}
	}
}
