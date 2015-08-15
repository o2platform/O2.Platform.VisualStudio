using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FluentSharp.WPF;

namespace O2.FluentSharp.VisualStudio.ExtensionMethods
{
	//Extra WPF Extension Methods
	public static class WPF_ExtensionMethods_Window
	{
		public static string title<T>(this T window) where T : System.Windows.Window
		{
			return window.wpfInvoke(() => window.Title);
		}
		public static T title<T>(this T window, string title) where T : System.Windows.Window
		{
			return window.wpfInvoke(() => { window.Title = title; return window; });
		}
	}
}