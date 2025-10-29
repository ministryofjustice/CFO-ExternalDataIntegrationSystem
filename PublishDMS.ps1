# Overview : Publish DMS Projects in the list

$msbuild = "C:\Program Files\Microsoft Visual Studio\2022\Enterprise\MSBuild\Current\Bin"

# Project root : Make sure it doesn't have a \ at the end!
$Root = "$PSScriptRoot\src"

$Output = "$($env:USERPROFILE)\DMS\publish"

cd $msbuild

# Project names
$Projects = @(
    'API',
    'Blocking',
    'Cleanup',
    'DbInteractions',
    'Delius.Parser',
    'Import',
    'Kickoff',
    'Logging',
    'Meow',
    'Matching.Engine',
    'Offloc.Cleaner',
    'Offloc.Parser',
    'Orchestrator',
    'Visualiser'
)

$DbProjects = @(
    'ClusterDb',
    'MatchingDb',
    'OfflocRunningPictureDb',
    'OfflocStagingDb',
    'DeliusRunningPictureDb',
    'DeliusStagingDb'
)

# Delete old publishes
If (test-path $Output)
{
    Remove-Item $Output\* -Force -Recurse
}

# Publish the projects
foreach ($P in $Projects) {
    $Command = "dotnet publish $Root\$P\$P.csproj -c Release --runtime win-x64 --output $Output\app\$P" 
    Invoke-Expression $Command
}

$dbOutputPath = "$Output\db\build"

# Publish the db projects
foreach ($P in $DbProjects) {
    .\msbuild.exe $Root\$P\$P.sqlproj /p:OutputPath="$dbOutputPath" /p:Configuration=Release /t:Rebuild
    Copy-Item "$dbOutputPath\$P*.sql" -Destination "$dbOutputPath\.."
}

If (test-path $dbOutputPath)
{
    Remove-Item $dbOutputPath -Recurse -Force
}

cd $PSScriptRoot