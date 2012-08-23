param($installPath, $toolsPath, $package, $project)

$targetsFile = [System.IO.Path]::Combine($toolsPath, 'PostSharp.targets')

# Need to load MSBuild assembly if it's not loaded yet.
Add-Type -AssemblyName 'Microsoft.Build, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a'

# Grab the loaded MSBuild project for the project
$msbuild = [Microsoft.Build.Evaluation.ProjectCollection]::GlobalProjectCollection.GetLoadedProjects($project.FullName) | Select-Object -First 1

# Make the path to the targets file relative.
$projectUri = new-object Uri('file://' + $project.FullName)
$targetUri = new-object Uri('file://' + $targetsFile)
$relativePath = $projectUri.MakeRelativeUri($targetUri).ToString().Replace([System.IO.Path]::AltDirectorySeparatorChar, [System.IO.Path]::DirectorySeparatorChar)

# Remove previous properties DontImportPostSharp
$msbuild.Xml.Properties | Where-Object {$_.Name.ToLowerInvariant() -eq "dontimportpostsharp" } | Foreach { 
	$_.Parent.RemoveChild( $_ ) 
	"Removed property DontImportPostSharp"
}

# Remove previous imports to PostSharp.targets
$msbuild.Xml.Imports | Where-Object {$_.Project.ToLowerInvariant().EndsWith("postsharp.targets") } | Foreach { 
	$_.Parent.RemoveChild( $_ ) 
	[string]::Format( "Removed import of '{0}'" , $_.Project )
}

# Add the property DontImportPostSharp
$msbuild.Xml.AddProperty( "DontImportPostSharp", "True" ) | Out-Null
"Added property DontImportPostSharp=true."

# Add import to PostSharp.targets
$msbuild.Xml.AddImport($relativePath) | Out-Null
[string]::Format("Added import of '{0}'.", $relativePath )

# Check if we should install the VSX add-in.
$vsxInstaller = [System.IO.Path]::Combine($toolsPath, 'Release\PostSharp.HQ.exe')
Start-Process -FilePath $vsxInstaller -ArgumentList @("/first-setup")
	