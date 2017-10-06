using System;
using System.Linq;
using System.Reflection;
using System.Windows;

namespace ServerWPF
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App
    {
	    public static MainWindow _mainWindow;
		
	    private void App_Startup(object sender, StartupEventArgs e)
	    {
		   // AppDomain.CurrentDomain.AssemblyResolve += CurrentDomain_AssemblyResolve;
			
			_mainWindow = new MainWindow();
		    _mainWindow.Show();
		}

	    static Assembly CurrentDomain_AssemblyResolve(object sender, ResolveEventArgs args)
	    {
		    var dllName = new AssemblyName(args.Name).Name + ".dll";
		    var execAsm = Assembly.GetExecutingAssembly();
		    var resourceName = execAsm.GetManifestResourceNames().FirstOrDefault(s => s.EndsWith(dllName));
		    if (resourceName == null) return null;
		    using (var stream = execAsm.GetManifestResourceStream(resourceName))
		    {
			    // ReSharper disable once PossibleNullReferenceException
			    var assbebmlyBytes = new byte[stream.Length];
			    stream.Read(assbebmlyBytes, 0, assbebmlyBytes.Length);
			    return Assembly.Load(assbebmlyBytes);
		    }
	    }
	}
}
