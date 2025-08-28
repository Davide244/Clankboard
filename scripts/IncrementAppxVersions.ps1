param([string]$workingDirectory = $PSScriptRoot)
Write-Host "Working Directory: $workingDirectory"

# Project has multiple .appxmanifest files. Find them all and increment the build number in each.
$appxManifestFiles = Get-ChildItem -Path $workingDirectory -Recurse -Filter *.appxmanifest
foreach ($file in $appxManifestFiles) {
	Write-Host "Processing file: $($file.FullName)"
	[xml]$xml = Get-Content $file.FullName

	$version = $xml.Package.Identity.Version
	Write-Host "Current Version: $version"

	# Split the version into its components
	$versionParts = $version -split '\.'
	if ($versionParts.Length -ne 4) {
		Write-Error "Version format is incorrect. Expected format: Major.Minor.Build.Revision"
		continue
	}
	# Increment
	$buildNumber = [int]$versionParts[2]
	$buildNumber++
	$versionParts[2] = $buildNumber.ToString()

	$newVersion = ($versionParts -join '.')
	Write-Host "New Version: $newVersion"

	$xml.Package.Identity.Version = $newVersion
	$xml.Save($file.FullName)
	Write-Host "Updated file saved: $($file.FullName)"
}