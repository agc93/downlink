#tool "nuget:?package=GitVersion.CommandLine"
#load "build/helpers.cake"
#tool nuget:?package=docfx.console
#addin nuget:?package=Cake.DocFx
#addin nuget:?package=Cake.Docker

///////////////////////////////////////////////////////////////////////////////
// ARGUMENTS
///////////////////////////////////////////////////////////////////////////////

var target = Argument("target", "Default");
var configuration = Argument("configuration", "Release");

///////////////////////////////////////////////////////////////////////////////
// GLOBAL VARIABLES
///////////////////////////////////////////////////////////////////////////////

var solutionPath = File("./src/Downlink.sln");
var projects = GetProjects(solutionPath, configuration);
var artifacts = "./dist/";
var testResultsPath = MakeAbsolute(Directory(artifacts + "./test-results"));
GitVersion versionInfo = null;
var frameworks = new List<string> { "netcoreapp2.0" };
var runtimes = new List<string> { "win10-x64", "osx.10.12-x64", "ubuntu.16.04-x64", "ubuntu.14.04-x64", "centos.7-x64", "debian.8-x64", "rhel.7-x64" };
var PackagedRuntimes = new List<string> { "centos", "ubuntu", "debian", "fedora", "rhel" };

///////////////////////////////////////////////////////////////////////////////
// SETUP / TEARDOWN
///////////////////////////////////////////////////////////////////////////////

Setup(ctx =>
{
	// Executed BEFORE the first task.
	Information("Running tasks...");
	//versionInfo = GetVersion();
	//Information("Building for version {0}", versionInfo.FullSemVer);
	Verbose("Building for " + string.Join(", ", frameworks));
});

Teardown(ctx =>
{
	// Executed AFTER the last task.
	Information("Finished running tasks.");
});

///////////////////////////////////////////////////////////////////////////////
// TASK DEFINITIONS
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
	foreach (var project in projects.AllProjectPaths) {
		DotNetCoreRestore(project.FullPath);
	}
});

Task("Build")
	.IsDependentOn("Clean")
	.IsDependentOn("Restore")
	.Does(() =>
{
	Information("Building solution...");
	foreach(var framework in frameworks) {
		foreach (var project in projects.SourceProjectPaths) {
			var settings = new DotNetCoreBuildSettings {
				Framework = framework,
				Configuration = configuration,
				NoIncremental = true,
			};
			DotNetCoreBuild(project.FullPath, settings);
		}
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
	DocFxBuild("./docfx/docfx.json");
	Zip("./docfx/_site/", artifacts + "/docfx.zip");
})
.OnError(ex => 
{
	Warning("Error generating documentation!");
});

Task("Post-Build")
	.IsDependentOn("Build")
	.IsDependentOn("Run-Unit-Tests")
	//.IsDependentOn("Generate-Docs")
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

Task("Publish-Runtimes")
	.IsDependentOn("Post-Build")
	.Does(() =>
{
	CreateDirectory(artifacts + "publish/");
	foreach (var project in projects.SourceProjects) {
		foreach (var framework in frameworks) {
			var projectDir = $"{artifacts}publish/{project.Name}";
			CreateDirectory(projectDir);
			foreach(var runtime in runtimes) {
				var runtimeDir = $"{projectDir}/{runtime}";
				CreateDirectory(runtimeDir);
				Information("Publishing {0} for {1} runtime", project.Name, runtime);
				var rSettings = new DotNetCoreRestoreSettings {
					ArgumentCustomization = args => args.Append("-r " + runtime)
				};
				DotNetCoreRestore(project.Path.FullPath, rSettings);
				var settings = new DotNetCorePublishSettings {
					ArgumentCustomization = args => args.Append("-r " + runtime),
					Configuration = configuration
				};
				DotNetCorePublish(project.Path.FullPath, settings);
				var publishDir = $"{project.Path.GetDirectory()}/bin/{configuration}/{framework}/{runtime}/publish/";
				CopyDirectory(publishDir, runtimeDir);
				CopyFiles(GetFiles("./build/*.sh"), runtimeDir);
			}
		}
	}
});

Task("Build-Linux-Packages")
	.IsDependentOn("Publish-Runtimes")
	.Does(() => 
{
	Information("Building packages in new container");
	CreateDirectory($"{artifacts}/packages/");
	foreach(var project in projects.SourceProjects) {
		foreach(var runtime in runtimes.Where(rt => PackagedRuntimes.Any(r => rt.Contains(r)))) {
			var publishDir = $"{artifacts}publish/{project.Name}/{runtime}";
			var sourceDir = MakeAbsolute(Directory(publishDir));
			var packageDir = MakeAbsolute(Directory($"{artifacts}packages/{runtime}"));
			var runSettings = new DockerRunSettings {
				Name = $"docker-fpm-{(runtime.Replace(".", "-"))}",
				Volume = new[] { $"{sourceDir}:/src:ro", $"{packageDir}:/out:rw"},
				Workdir = "/out",
				Rm = true,
				//User = "1000"
			};
			var opts = @"-s dir -a all --force
			-m 'Alistair Chapman <alistair@agchapman.com>'
			-n 'git-profile-manager'
			--after-install /src/post-install.sh
			--before-remove /src/pre-remove.sh";
			DockerRun(runSettings, "tenzer/fpm", $"{opts} -v {versionInfo.FullSemVer} {GetRuntimeBuild(runtime)} /src/=/usr/lib/git-profile-manager/");
		}
	}
});

Task("Build-Windows-Packages")
	.IsDependentOn("Publish-Runtimes")
	.Does(() => 
{
	Information("Building Chocolatey package in new container");
	CreateDirectory($"{artifacts}packages");
	foreach(var project in projects.SourceProjects) {
		foreach(var runtime in runtimes.Where(r => r.StartsWith("win"))) {
			var publishDir = $"{artifacts}publish/{project.Name}/{runtime}";
			CopyFiles(GetFiles($"./build/{runtime}.nuspec"), publishDir);
			var sourceDir = MakeAbsolute(Directory(publishDir));
			var packageDir = MakeAbsolute(Directory($"{artifacts}packages/{runtime}"));
			var runSettings = new DockerRunSettings {
				Name = $"docker-choco-{(runtime.Replace(".", "-"))}",
				Volume = new[] { 
					$"{sourceDir}:/src/{runtime}:ro",
					$"{packageDir}:/out:rw",
					$"{sourceDir}/{runtime}.nuspec:/src/package.nuspec:ro"
				},
				Workdir = "/src",
				Rm = true
			};
			var opts = @"-y -v
				--outputdirectory /out/
				/src/package.nuspec";
			DockerRun(runSettings, "agc93/mono-choco", $"choco pack --version {versionInfo.NuGetVersionV2} {opts}");
		}
	}
});


Task("Default")
    .IsDependentOn("Post-Build");

Task("Publish")
	.IsDependentOn("Build-Linux-Packages")
	.IsDependentOn("Build-Windows-Packages");

RunTarget(target);