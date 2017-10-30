#load "build/helpers.cake"

#tool "nuget:?package=GitVersion.CommandLine"
#tool nuget:?package=docfx.console&version=2.18.5
#addin nuget:?package=Cake.DocFx&version=0.5.0
#addin nuget:?package=Cake.Docker&version=0.8.2

///////////////////////////////////////////////////////////////////////////////
// ARGUMENTS
///////////////////////////////////////////////////////////////////////////////

var target = Argument("target", "Default");
var configuration = Argument("configuration", "Release");
var tag = Argument("tag", "latest");
var fallbackVersion = Argument<string>("force-version", EnvironmentVariable("FALLBACK_VERSION") ?? "0.2.0");

///////////////////////////////////////////////////////////////////////////////
// VERSIONING
///////////////////////////////////////////////////////////////////////////////

var packageVersion = string.Empty;
#load "build/version.cake"

///////////////////////////////////////////////////////////////////////////////
// GLOBAL VARIABLES
///////////////////////////////////////////////////////////////////////////////

var solution = File("./src/Downlink.sln");
var projects = GetProjects(solution, configuration);
var artifacts = "./dist/";
var testResultsPath = MakeAbsolute(Directory(artifacts + "./test-results"));
var frameworks = new List<string> { "netcoreapp2.0" };
var runtimes = new List<string> { "win10-x64", "osx.10.12-x64", "ubuntu.16.04-x64", "ubuntu.14.04-x64", "centos.7-x64", "debian.8-x64", "rhel.7-x64" };

///////////////////////////////////////////////////////////////////////////////
// SETUP / TEARDOWN
///////////////////////////////////////////////////////////////////////////////

Setup(ctx =>
{
	// Executed BEFORE the first task.
	Information("Running tasks...");
	CreateDirectory(artifacts);
	Verbose("Building for " + string.Join(", ", frameworks));
	packageVersion = BuildVersion(fallbackVersion);
	if (FileExists("./build/.dotnet/dotnet.exe")) {
		Information("Using local install of `dotnet` SDK!");
		Context.Tools.RegisterFile("./build/.dotnet/dotnet.exe");
	}
});

Teardown(ctx =>
{
	// Executed AFTER the last task.
	Information("Finished running tasks.");
});

///////////////////////////////////////////////////////////////////////////////
// TASKS
///////////////////////////////////////////////////////////////////////////////

Task("Clean")
	.Does(() =>
{
	// Clean solution directories.
	foreach(var path in projects.AllProjectPaths)
	{
		Information("Cleaning {0}", path);
		CleanDirectories(path + "/**/bin/" + configuration);
		CleanDirectories(path + "/**/obj/" + configuration);
	}
	Information("Cleaning common files...");
	CleanDirectory(artifacts);
});

Task("Restore")
	.Does(() =>
{
	// Restore all NuGet packages.
	Information("Restoring solution...");
	DotNetCoreRestore(solution);
});

Task("Build")
	.IsDependentOn("Clean")
	.IsDependentOn("Restore")
	.Does(() =>
{
	Information("Building solution...");
	foreach (var project in projects.SourceProjectPaths) {
		Information($"Building {project.GetDirectoryName()} for {configuration}");
		var settings = new DotNetCoreBuildSettings {
			Configuration = configuration,
			ArgumentCustomization = args => args.Append("/p:NoWarn=NU1701"),
		};
		DotNetCoreBuild(project.FullPath, settings);
	}
	
});

Task("Run-Unit-Tests")
	.IsDependentOn("Build")
	.Does(() =>
{
    CreateDirectory(testResultsPath);
	if (projects.TestProjects.Any()) {

		var settings = new DotNetCoreTestSettings {
			Configuration = configuration
		};

		foreach(var project in projects.TestProjects) {
			DotNetCoreTest(project.Path.FullPath, settings);
		}
	}
});

Task("Generate-Docs")
	.Does(() => 
{
	Information("Building metadata...");
	DocFxMetadata("./docfx/docfx.json");
	Information("Building docs...");
	DocFxBuild("./docfx/docfx.json");
	Information("Packaging built docs...");
	Zip("./docfx/_site/", artifacts + "/docfx.zip");
})
.OnError(ex => 
{
	Warning(ex.Message);
	Warning("Error generating documentation!");
});

Task("Post-Build")
	.IsDependentOn("Build")
	.IsDependentOn("Run-Unit-Tests")
	.IsDependentOn("Generate-Docs")
	.Does(() =>
{
	CreateDirectory(artifacts + "build");
	foreach (var project in projects.SourceProjects) {
		CreateDirectory(artifacts + "build/" + project.Name);
		foreach (var framework in frameworks) {
			var frameworkDir = $"{artifacts}build/{project.Name}/{framework}";
			CreateDirectory(frameworkDir);
			var files = GetFiles($"{project.Path.GetDirectory()}/bin/{configuration}/{framework}/*.*");
			CopyFiles(files, frameworkDir);
		}
	}
});

Task("Publish")
	.IsDependentOn("Post-Build")
	.Does(() =>
{
	CreateDirectory(artifacts + "publish/");
	var project = projects.SourceProjects.First(p => p.Name == "Downlink.Host");
		foreach (var framework in frameworks) {
			var projectDir = $"{artifacts}publish/Downlink"; // BAD
			CreateDirectory(projectDir);
			Information("Publishing {0} to {1}", project.Name, projectDir);
			var settings = new DotNetCorePublishSettings {
				Configuration = configuration,
				OutputDirectory = projectDir
			};
			DotNetCorePublish(project.Path.FullPath, settings);
		}
});

Task("Docker-Build")
	.IsDependentOn("Publish")
	.Does(() =>
{
	CopyFileToDirectory("./build/Dockerfile", artifacts);
	CopyFileToDirectory("./build/appsettings.json", artifacts);
	var dSettings = new DockerImageBuildSettings {
		Tag = new[] { 
			$"agc93/downlink:{tag}",
			$"agc93/downlink:{packageVersion}"
		}
	};
	DockerBuild(dSettings, artifacts);
	DeleteFile(artifacts + "Dockerfile");
	DeleteFile(artifacts + "appsettings.json");
});

#load "build/nuget.cake"

Task("Default")
.IsDependentOn("Publish")
.IsDependentOn("NuGet");

Task("CI-Build")
.IsDependentOn("Publish")
.IsDependentOn("NuGet")
.IsDependentOn("Generate-Docs")
.Does(() => 
{
	CopyFileToDirectory("./build/Dockerfile", artifacts);
	CopyFileToDirectory("./build/appsettings.json", artifacts);
});

RunTarget(target);
