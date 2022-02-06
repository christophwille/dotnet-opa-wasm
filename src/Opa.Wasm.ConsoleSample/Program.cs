using Opa.Wasm;
using ICSharpCode.SharpZipLib.GZip;
using ICSharpCode.SharpZipLib.Tar;

EvaluateHelloWorld();
EvaluateRbac();
ReadFromBundle();

Console.Read();

// https://play.openpolicyagent.org/ "Role-based" example stripped down to minimum
static void EvaluateRbac()
{
	using var module = OpaPolicyModule.Load("rbac.wasm");

	// Now you can create as many instances of OpaPolicy on top of this runtime & loaded module as you want
	using var opaPolicy = module.CreatePolicyInstance();

	opaPolicy.SetDataJson(@"{""user_roles"": { ""alice"": [""admin""],""bob"": [""employee"",""billing""],""eve"": [""customer""]}}");
	var input = new RbacPolicyInputModel("alice", "read", "id123", "dog");
	var output = opaPolicy.Evaluate<RbacPolicyOutputModel>(input);

	Console.WriteLine($"RBAC output - allowed: {output.Value.Allow} is admin: {output.Value.user_is_admin}");
}

static void EvaluateHelloWorld()
{
	using var module = OpaPolicyModule.Load("example.wasm");
	using var opaPolicy = module.CreatePolicyInstance();

	opaPolicy.SetDataJson(@"{""world"": ""world""}");

	string input = @"{""message"": ""world""}";
	string output = opaPolicy.EvaluateJson(input);

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

// { ""user"": ""alice"", ""action"": ""read"", ""object"": ""id123"", ""type"": ""dog"" }
record RbacPolicyInputModel(string User, string Action, string Object, string Type);
// [{"result":{"allow":true,"user_is_admin":true}}]
record RbacPolicyOutputModel(bool Allow, bool user_is_admin);
