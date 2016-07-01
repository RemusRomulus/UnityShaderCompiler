using System;
using System.Collections.Generic;
using System.Text;

namespace OfflineShaderCompiler
{
	class Program
	{
		static int Main(string[] args)
		{
			var service = new CompilerService("shadercompiler.log");
			var result = service.Preprocess(System.IO.File.ReadAllText("TestCompiler.shader"), "MyLocation");
			var snip = result.Snippets[0];
			var cResult = service.CompileSnippet(snip.Text, "MyLocation", null, Platform.Flash, Function.Vertex);
			service.Dispose();
			Console.Read();
			return 0;
		}
	}
}
