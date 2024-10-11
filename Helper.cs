using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Runtime.Intrinsics;
using System.Runtime.Intrinsics.X86;
using System.Text;
using System.Threading.Tasks;
using DVLib.LabDataHelper;
using DVOSLib;
using Images;
using MathBase;
using Microsoft.Office.Interop.Excel;
using static System.Runtime.InteropServices.JavaScript.JSType;
using App = Microsoft.Office.Interop.Excel.Application;
using Complex = MathBase.Complex;

namespace LabDataHelper
{

	public static class LocalImageHelper
	{
	  public unsafe static	Bitmap toBitmap(this bitmap bitmap)
		{
			var r=new Bitmap(bitmap.Width, bitmap.Height);
			var data=r.LockBits(new System.Drawing.Rectangle(0,0,bitmap.Width,bitmap.HWidth),System.Drawing.Imaging.ImageLockMode.ReadWrite,System.Drawing.Imaging.PixelFormat.Format32bppArgb);
			bitmap.ToBitmap((byte*)data.Scan0, data.Stride);
			r.UnlockBits(data);
			return r;
		}
	}
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
		public static Complex mul(Complex[] a)
		{
			Complex c=0;
			for (int i = 0; i < a.Length; i++)
			{
				c+=mul(a[i] , a[i]);	
			}

			return c;
		}
		public static Complex mul(Complex a,Complex b)
		{
			return new Complex(a.realPart * b.realPart - a.imaginaryPart * b.imaginaryPart, a.realPart * b.imaginaryPart + a.imaginaryPart * b.realPart);

		}
		public unsafe static Complex[] mul3(Complex[] a, Complex[] b)
		{
			Complex[] re = new Complex[a.Length];
			for (int i = 0; i < a.Length; i++)
			{
				re[i] =mul(a[i],b[i]) ;
			}
			return re;
		}
		public unsafe static double muldouble(double[] a)
		{
			double sum = 0; ;
			for (int i = 0; i < a.Length; i++)
			{
				var t = a[i] *a[i];
				sum +=t;
			}

			return sum;
		}

		/// <summary>
		/// 使用SIMD计算矩阵
		/// </summary>
		/// <param name="mat1"></param>
		/// <param name="mat2"></param>
		/// <param name="N"></param>
		/// <returns></returns>
		/// 
		/*
		static float[,] SIMDMatrix(float[,] mat1, float[,] mat2, ref int N)
		{
			int mat1Height = mat1.GetLength(0);
			int mat1Width = mat1.GetLength(1);
			int mat2Width = mat2.GetLength(1);
			int mat2Height = mat2.GetLength(0);
			float[,] result = new float[mat1Height, mat2Width];

			Vector256<float>[] local1;
			Vector256<float>[] local2;
			int gMask = mat1Width / 8;
			if (mat1Width % 8 != 0) gMask++;

			local1 = new Vector256<float>[gMask * mat1Height];
			local2 = new Vector256<float>[gMask * mat2Width];
			float[] f = new float[8];

			int number1 = 0;
			for (int a = 0; a < mat1Height; a++)
			{
				for (int b = 0; b < mat1Width; b += 8)
				{
					for (int c = 0; c < 8; c++)
					{
						if (c + b < mat1Width) f[c] = mat1[a, c + b];
						else f[c] = 0.0f; //超出矩阵范围的变量被置零
						N++;
					}
					local1[number1] = Vector256.Create(f[0], f[1], f[2], f[3], f[4], f[5], f[6], f[7]);
					number1++;
				}
			}

			int number2 = 0;
			for (int a = 0; a < mat2Width; a++)
			{
				for (int b = 0; b < mat2Height; b += 8)
				{
					for (int c = 0; c < 8; c++)
					{
						if (c + b < mat2Height) f[c] = mat2[c + b, a];
						else f[c] = 0.0f;
						N++;
					}
					local2[number2] = Vector256.Create(f[0], f[1], f[2], f[3], f[4], f[5], f[6], f[7]);
					number2++;
				}
			}

			for (int i = 0; i < mat1Height; i++)
			{
				Vector256<float>[] m1 = local1[i * gMask..(i + 1) * gMask]; //C#8.0的语法，范围Range
				for (int j = 0; j < mat2Width; j++)
				{
					float C = 0;
					Vector256<float>[] m2 = local2[j * gMask..(j + 1) * gMask];
					Vector256<float> m3;
					for (int k = 0; k < gMask; k++)
					{
						m3 = Avx.Multiply(m1[k], m2[k]);
						Vector256<float> m4 = Avx.Permute2x128(m3, m3, 1);
						m3 = Avx.HorizontalAdd(m3, m4);
						m3 = Avx.HorizontalAdd(m3, m3);
						m3 = Avx.HorizontalAdd(m3, m3);
						C += m3.GetElement(0);
						N++;
					}
					result[i, j] = C;
				}
			}
			return result;
		}
		*/
		/// <summary>
		/// 使用枚举法计算矩阵
		/// </summary>
		/// <param name="mat1"></param>
		/// <param name="mat2"></param>
		/// <param name="N"></param>
		/// <returns></returns>
		static float[,] EnumerationMatrix(float[,] mat1, float[,] mat2, ref int N)
		{
			float[,] result = new float[mat1.GetLength(0), mat2.GetLength(1)];
			for (int i = 0; i < mat1.GetLength(0); i++)
			{
				for (int j = 0; j < mat2.GetLength(1); j++)
				{
					float C = 0;
					for (int k = 0; k < mat1.GetLength(1); k++)
					{
						C += mat1[i, k] * mat2[k, j];
						N++;
					}
					result[i, j] = C;
				}
			}
			return result;
		}

		/// <summary>
		/// 创建方阵
		/// </summary>
		/// <param name="mat1"></param>
		/// <param name="mat2"></param>
		/// <param name="N"></param>
		static void CreateMat(out float[,] mat1, out float[,] mat2, int N)
		{
			mat1 = new float[N, N];
			mat2 = new float[N, N];
			for (int i = 0; i < N; i++)
			{
				for (int j = 0; j < N; j++)
				{
					mat1[i, j] = 1;
					mat2[i, j] = 1;
				}
			}
		}

		/// <summary>
		/// 判断两个矩阵是否一致
		/// </summary>
		/// <param name="mat1"></param>
		/// <param name="mat2"></param>
		/// <returns></returns>
		static bool IsSame(in float[,] mat1, in float[,] mat2)
		{
			for (int i = 0; i < mat1.GetLength(0); i++)
			{
				for (int j = 0; j < mat1.GetLength(1); j++)
				{
					if (mat1[i, j] != mat2[i, j])
					{
						return false;
					}
				}
			}
			return true;
		}

		public unsafe static double muldouble2(double[] a)
		{
			var r_= new Vector256<double>();

			var temp = new Vector256<double>();
			double sum=0;


			fixed(double* a_=a)
			{
				
				Vector256<double>* ad=(Vector256<double>*)a_;
				int c = a.Length >>2;
				int left = a.Length - (c << 2);


	         for (int i = 0; i < c; i++)
			{
					temp = *(ad + i);
					var v = Avx.Multiply(temp,temp);
					r_=Avx.Add(v ,r_);
			}
			for(int i=a.Length-left;i<a.Length;i++)
				{

					sum+= a[i] * a[i];
				}
			
			}
		

			return sum+r_[0]+r_[1]+r_[2]+r_[3];
		}

		public static double MultiplyN(double[] nums)
		{
			double result = 1.0d;

			for (int i = 0; i < nums.Length; i++)
			{
				result *= nums[i];
			}
			return result;
		}

		public unsafe static double MultiplyM(double[] nums)
		{
			int vectorSize = Vector<double>.Count;
			var accVector = Vector<double>.One;
			int i;
			var array = nums;
			double result = 1.0d;
			fixed (double* p = array)
			{
				for (i = 0; i <= array.Length - vectorSize; i += vectorSize)
				{
					//var v = new Vector<double>(array, i);
					var v = Unsafe.Read<Vector<double>>(p + i);
					accVector = System.Numerics.Vector.Multiply(accVector, v);
				}
			}
			var tempArray = new double[Vector<double>.Count];
			accVector.CopyTo(tempArray);
			for (int j = 0; j < tempArray.Length; j++)
			{
				result = result * tempArray[j];
			}

			for (; i < array.Length; i++)
			{
				result *= array[i];
			}

			return result;
		}

		[MethodImpl(MethodImplOptions.AggressiveOptimization)]
		public unsafe static Complex[]  mul2(Complex[] a, Complex[]b)
		{

			Complex[] r=new Complex[a.Length];
			int li = a.Length - 1;
			fixed (Complex* a_ = a,b_=b,re_=r)
			{

				
				Vector256<double>* sa = (Vector256<double>*)a_;

				Vector256<double>* sb = (Vector256<double>*)b_;
				Vector256<double>* sr = (Vector256<double>*)re_;
				var mask = Vector256.Create(0.0, -0.0, 0.0, -0.0);
				mask = Avx.LoadVector256((double*)&mask);
				int t = a.Length >>1;
				int left = a.Length - (t << 1);
				var r_= Vector256.Create(0.0);
				
				//var v7 = Avx.LoadVector256((double*)&mask);
				var v8=Avx.LoadVector256((double*)&r_);
				Complex* r1 = (Complex*)&r_;
				Complex* r2 = r1+1;
				 Vector256<double>  v02, v01, v1, v2, v3, v4, v5;
				for (int i = 0; i <t; i++)
				{
					 v01 = Avx.LoadVector256((double*)(sa + i));
					 v02 = Avx.LoadVector256((double*)(sb + i));
					 v1 = Avx.Xor(v01,mask);
					 v2 = Avx.Multiply(v1, v02);
					 v3 = Avx2.Permute4x64(v02, 0b10110001);
					 v4 = Avx.Multiply(v01, v3);
					 v5 = Avx.HorizontalAdd(v2, v4);
					Avx.Store((double*)(sr + i),v5);
				}
				if (left > 0)
				{
					r[li] = a[li] * b[li];
				}
			}

			return r;
		}
	}
}
