using System;

namespace OfflineShaderCompiler
{
	class Program
	{
		static int Main(string[] args)
		{
			var service = new CompilerService("shadercompiler.log");
			var result = service.Preprocess(System.IO.File.ReadAllText("TestCompiler.shader"), "MyLocation");
			var snip = result.Snippets[0];

			var cResult = service.CompileSnippet(snip.Text, "MyLocation", null, Platform.OpenGL, Function.Vertex);

			Console.WriteLine(cResult.Shader);
			return 0;
		}
	}
}
