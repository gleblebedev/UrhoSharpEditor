using System;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using Windows.UI.Xaml.Controls;
using Urho;

namespace UrhoSharpEditor.UWP
{
	/// <summary>
	/// An empty page that can be used on its own or navigated to within a Frame.
	/// </summary>
	public sealed partial class MainPage : Page
	{
		Application currentApplication;
		Type selectedGameType;

		public MainPage()
		{
			InitializeComponent();
		    selectedGameType = Assembly.Load(new AssemblyName("UrhoSharpEditor"))
                .GetTypes()
                .Where(_ => _.GetTypeInfo().IsSubclassOf(typeof(Application)))
				.First(_ => !_.GetTypeInfo().IsAbstract);
			DataContext = this;
			Loaded += (s, e) => RunGame(selectedGameType);
		}

		public void RunGame(Type value)
		{
			currentApplication?.Exit();
			//at this moment, UWP supports assets only in pak files (see PackageTool)
			currentApplication = UrhoSurface.Run(value, new ApplicationOptions("Data") {WindowedMode = Debugger.IsAttached, LimitFps = true});
		}
	}
}
