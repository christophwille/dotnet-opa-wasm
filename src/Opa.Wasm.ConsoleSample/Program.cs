using Opa.Wasm;
using ICSharpCode.SharpZipLib.GZip;
using ICSharpCode.SharpZipLib.Tar;

EvaluateHelloWorld();
EvaluateRbac();
ReadFromBundle();

Console.Read();

static void EvaluateHelloWorld()
{
	using var module = OpaPolicyModule.Load("example.wasm");
	using var opaPolicy = module.CreatePolicyInstance();

	opaPolicy.SetDataJson(@"{""world"": ""world""}"); // use SetData(object) for a higher-level API

	string input = @"{""message"": ""world""}";
	string output = opaPolicy.EvaluateJson(input); // use Evaluate<T>(...) for a higher-level API

	Console.WriteLine($"Hello world output: {output}");
}

// https://play.openpolicyagent.org/ "Role-based" example stripped down to minimum
static void EvaluateRbac()
{
	using var module = OpaPolicyModule.Load("rbac.wasm");
	using var opaPolicy = module.CreatePolicyInstance();

	// You can use SetDataJson or SetData and mix with EvaluateJson or Evaluate<T> to your liking
	const string data = @"{""user_roles"": { ""alice"": [""admin""],""bob"": [""employee"",""billing""],""eve"": [""customer""]}}";
	opaPolicy.SetDataJson(data);

	var input = new RbacPolicyInputModel(User: "alice", Action: "read", Object: "id123", Type: "dog");
	var result = opaPolicy.Evaluate<RbacPolicyResultModel>(input);

	Console.WriteLine($"RBAC output - allowed: {result.Value.Allow} is admin: {result.Value.user_is_admin}");
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

record RbacPolicyInputModel(string User, string Action, string Object, string Type);
record RbacPolicyResultModel(bool Allow, bool user_is_admin);
