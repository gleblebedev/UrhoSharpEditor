using System;
using System.Linq;
using System.Reflection;
using System.Diagnostics;
using Urho.Desktop;

namespace Urho.Samples.Mac
{
	class Program
	{
		static Type[] samples;

		/// <param name="args">sample number, e.g. "19"</param>
		static void Main(string[] args)
		{
			var assembly = Assembly.LoadFrom("UrhoSharpEditor.dll");
			var appType = assembly.GetTypes()
				.Where(_ => _.IsSubclassOf(typeof (Urho.Application)))
				.Where(_=>!_.IsAbstract)
				.FirstOrDefault();
			var game = (Application) Activator.CreateInstance(appType, new ApplicationOptions("Data"){WindowedMode = Debugger.IsAttached, LimitFps = true});
			var exitCode = game.Run();
			Console.WriteLine($"Exit code: {exitCode}. Press any key to exit...");
			Console.ReadKey();
		}

		static Type ParseSampleFromNumber(string input)
		{
			int number;
			if (!int.TryParse(input, out number))
			{
				Console.WriteLine("Invalid format.");
				return null;
			}

			if (number >= samples.Length || number < 0)
			{
				Console.WriteLine("Invalid number.");
				return null;
			}

			return samples[number];
		}
	}
}
