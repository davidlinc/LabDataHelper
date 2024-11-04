using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace LabDataHelper
{
	public class FolderSelectDialog
	{
		private string _initialDirectory;
		private string _title;
		private string _folderPath;

		[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
		public class BROWSEINFO
		{
			public IntPtr hwndOwner;
			public IntPtr pidlRoot;
			public IntPtr pszDisplayName;
			public string lpszTitle;
			public uint ulFlags;
			public IntPtr lpfn;
			public IntPtr lParam;
			public int iImage;
		}

		[DllImport("user32.dll", CharSet = CharSet.Auto)]
		public static extern IntPtr SendMessage(IntPtr hWnd, uint Msg, IntPtr wParam, IntPtr lParam);

		[DllImport("shell32.dll", CharSet = CharSet.Auto)]
		private static extern IntPtr SHBrowseForFolder([In] BROWSEINFO lpbi);

		[DllImport("shell32.dll", CharSet = CharSet.Auto)]
		private static extern bool SHGetPathFromIDList(IntPtr pidl, IntPtr pszPath);
		[DllImport("shell32.dll", CharSet = CharSet.Auto)]
		private static extern IntPtr SHParseDisplayName(string pszName, IntPtr pbc, out IntPtr ppidl, uint sfgd, IntPtr rgfInOut);
		public bool ShowDialog()
		{
			IntPtr buffer = Marshal.AllocHGlobal(260);
			try
			{
				SHParseDisplayName(_initialDirectory, IntPtr.Zero, out IntPtr pidlRoot, 0, IntPtr.Zero);
				BROWSEINFO bi = new BROWSEINFO();
				bi.hwndOwner = IntPtr.Zero;
				bi.pidlRoot = pidlRoot;
				bi.pszDisplayName = buffer;
				bi.lpszTitle = _title;
				bi.ulFlags = 0x40; // BIF_USENEWUI flag
				bi.lpfn = IntPtr.Zero;
				bi.lParam = IntPtr.Zero;
				bi.iImage = 0;

				IntPtr pidl = SHBrowseForFolder(bi);

				if (pidl != IntPtr.Zero)
				{
					SHGetPathFromIDList(pidl, buffer);
					_folderPath = Marshal.PtrToStringAuto(buffer);
					return true;
				}

				return false;
			}
			finally
			{
				Marshal.FreeHGlobal(buffer);
			}
		}

		public string SelectedPath
		{
			get { return _folderPath; }
			set { _folderPath = value; }
		}

		public string Title
		{
			get { return _title; }
			set { _title = value; }
		}

		public string InitialDirectory
		{
			get { return _initialDirectory; }
			set { _initialDirectory = value; }
		}
	}
}
