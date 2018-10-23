using System;
using System.Linq;
using System.Reflection;
using System.Diagnostics;
using System.Windows.Forms;
using Urho;

namespace UrhoSharpEditor
{
    public static class Program
    {
        /// <summary>
        ///     The main entry point for the application.
        /// </summary>
        [STAThread]
        public static void Main(string[] args)
        {
            try
            {
				var assembly = Assembly.LoadFrom("UrhoSharpEditor.dll");
				var appType = assembly.GetTypes()
					.Where(_ => _.IsSubclassOf(typeof (Urho.Application)))
					.Where(_=>!_.IsAbstract)
					.FirstOrDefault();
				if (appType == null)
				{
					MessageBox.Show("Application type not found", "Can't launch Urho application", MessageBoxButtons.OK,
						MessageBoxIcon.Error);
					return;
				}
				var options = new ApplicationOptions("Data") {WindowedMode = Debugger.IsAttached, LimitFps = true};
				var _app = (Urho.Application)Activator.CreateInstance(appType,new object[] { options });
				_app.Run();
            }
            catch (BadImageFormatException exception)
            {
                MessageBox.Show(exception.ToString(), "Can't load file " + exception.FileName, MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
		}
    }
}