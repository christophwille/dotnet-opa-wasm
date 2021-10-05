using ICSharpCode.SharpZipLib.GZip;
using ICSharpCode.SharpZipLib.Tar;
using System;
using System.IO;

namespace Opa.Wasm.ConsoleSample
{
	// See https://github.com/open-policy-agent/npm-opa-wasm/blob/master/examples/nodejs-app/app.js
	class Program
	{
		static void Main(string[] args)
		{
			EvaluateHelloWorld();
			EvaluateRbac();

			ReadFromBundle();

			Console.Read();
		}

		// https://play.openpolicyagent.org/ "Role-based" example stripped down to minimum
		static void EvaluateRbac()
		{
			using var opaModule = new OpaModule();
			using var module = opaModule.Load("rbac.wasm");
			using var opaPolicy = new OpaPolicy(opaModule, module);

			opaPolicy.SetData(@"{""user_roles"": { ""alice"": [""admin""],""bob"": [""employee"",""billing""],""eve"": [""customer""]}}");

			string input = @"{ ""user"": ""alice"", ""action"": ""read"", ""object"": ""id123"", ""type"": ""dog"" }";
			string output = opaPolicy.Evaluate(input);

			Console.WriteLine($"RBAC output: {output}");
		}

		static void EvaluateHelloWorld()
		{
			using var opaModule = new OpaModule();
			using var module = opaModule.Load("example.wasm");
			using var opaPolicy = new OpaPolicy(opaModule, module);

			opaPolicy.SetData(@"{""world"": ""world""}");

			string input = @"{""message"": ""world""}";
			string output = opaPolicy.Evaluate(input);

			Console.WriteLine($"Hello world output: {output}");
		}

		static void ReadFromBundle()
		{
			using var inStream = File.OpenRead("bundle-example.tar.gz"); // by default would be bundle.tar.gz
			using var gzipStream = new GZipInputStream(inStream);
			using var tarStream = new TarInputStream(gzipStream, null);

			TarEntry current = null;
			MemoryStream ms = null;
			while (null != (current = tarStream.GetNextEntry()))
			{
				if ("/policy.wasm" == current.Name)
				{
					ms = new MemoryStream();
					tarStream.CopyEntryContents(ms);
					break;
				}
			}

			tarStream.Close();
			gzipStream.Close();
			inStream.Close();

			if (null != ms)
			{
				ms.Position = 0;
				var bytes = ms.ToArray();
				int length = bytes.Length; // 116020
			}
		}
	}
}
