using System;
using System.Reflection;
using System.IO;

namespace Eto.Drawing
{
	/// <summary>
	/// Platform handler for the <see cref="Icon"/> class
	/// </summary>
	public interface IIcon : IImage
	{
		/// <summary>
		/// Called when creating an instance from a stream
		/// </summary>
		/// <param name="stream">Stream to load the icon from</param>
		void Create (Stream stream);

		/// <summary>
		/// Called when creating an instance from a file name
		/// </summary>
		/// <param name="fileName">File name to load the icon from</param>
		void Create (string fileName);
	}
	
	/// <summary>
	/// Represents an icon which allows for multiple sizes of an image
	/// </summary>
	/// <remarks>
	/// The formats supported vary by platform, however all platforms do support loadin windows .ico format.
	/// 
	/// Using an icon for things like menus, toolbars, etc are preferred so that each platform can use the appropriate
	/// sized image.
	/// 
	/// For HiDPI/Retina displays (e.g. on OS X), this will allow using a higher resolution image automatically.
	/// </remarks>
	public class Icon : Image
	{
		new IIcon Handler { get { return (IIcon)base.Handler; } }
		
		/// <summary>
		/// Initializes a new instance of the Icon class with the specified handler
		/// </summary>
		/// <param name="generator">Generator for this widget</param>
		/// <param name="handler">Handler for the icon backend</param>
		public Icon (Generator generator, IIcon handler) : base(generator, handler)
		{
		}
	
		/// <summary>
		/// Initializes a new instance of the Icon class with the contents of the specified <paramref name="stream"/>
		/// </summary>
		/// <param name="generator">Generator for this widget</param>
		/// <param name="stream">Stream to load the content from</param>
		public Icon (Stream stream, Generator generator = null) : base(generator, typeof(IIcon))
		{
			Handler.Create (stream);
		}

		/// <summary>
		/// Intitializes a new instanc of the Icon class with the contents of the specified <paramref name="fileName"/>
		/// </summary>
		/// <param name="generator">Generator for this widget</param>
		/// <param name="fileName">Name of the file to loat the content from</param>
		public Icon (string fileName, Generator generator = null) : base(generator, typeof(IIcon))
		{
			Handler.Create (fileName);
		}
		
		/// <summary>
		/// Loads an icon from an embedded resource of the specified assembly
		/// </summary>
		/// <param name="assembly">Assembly to load the resource from</param>
		/// <param name="resourceName">Fully qualified name of the resource to load</param>
		/// <param name="generator">Generator for this widget</param>
		/// <returns>A new instance of an Icon loaded with the contents of the specified resource</returns>
		#if PCL
		public static Icon FromResource (string resourceName, Assembly assembly, Generator generator = null)
		#else
		public static Icon FromResource (string resourceName, Assembly assembly = null, Generator generator = null)
		#endif
		{
			if (assembly == null)
			{
				#if PCL
				throw new ArgumentNullException("assembly");
				#else
				assembly = Assembly.GetCallingAssembly();
				#endif
			}
			using (var stream = assembly.GetManifestResourceStream(resourceName)) {
				if (stream == null)
					throw new ResourceNotFoundException (assembly, resourceName);
				return new Icon (stream, generator);
			}
		}

		public static Icon FromResource (string resourceName, Type type, Generator generator = null)
		{
			#if PCL
			return FromResource(resourceName, type.GetTypeInfo().Assembly, generator);
			#else
			return FromResource(resourceName, type.Assembly, generator);
			#endif
		}

		/// <summary>
		/// Loads an icon from an embedded resource of the specified assembly
		/// </summary>
		/// <param name="assembly">Assembly to load the resource from</param>
		/// <param name="resourceName">Fully qualified name of the resource to load</param>
		/// <param name="generator">Generator for this widget</param>
		/// <returns>A new instance of an Icon loaded with the contents of the specified resource</returns>
		[Obsolete("Use FromResource(string, Assembly, Generator) instead")]
		public static Icon FromResource (Assembly assembly, string resourceName, Generator generator = null)
		{
			return FromResource(resourceName, assembly, generator);
		}

		#if !PCL
		/// <summary>
		/// Loads an icon from an embedded resource of the caller's assembly
		/// </summary>
		/// <remarks>
		/// This is a shortcut for <see cref="FromResource(Assembly,string,Generator)"/> where it will
		/// use the caller's assembly to load the resource from
		/// </remarks>
		/// <param name="resourceName">Fully qualified name of the resource to load</param>
		/// <param name="generator">Generator for this widget</param>
		/// <returns>A new instance of an Icon loaded with the contents of the specified resource</returns>
		[Obsolete("Use FromResource(string, Assembly, Generator) instead")]
		public static Icon FromResource (string resourceName, Generator generator = null)
		{
			throw new NotImplementedException("Portable Class Libraries do not support Assembly.GetCallingAssembly");
			var asm = Assembly.GetCallingAssembly ();
			return FromResource (resourceName, asm, generator);
		}		
		#endif
	}
}
