[CmdletBinding()]
param(
    [Parameter(Mandatory)]
    [string]$PackageDirectory,
    [string]$ExpectedVersion
)

$ErrorActionPreference = "Stop"
$repositoryRoot = Split-Path -Parent $PSScriptRoot
$packageDirectoryPath = [System.IO.Path]::GetFullPath($PackageDirectory)

Add-Type -AssemblyName System.IO.Compression

$projects = Get-ChildItem (Join-Path $repositoryRoot "src") -Filter "MDev.Dotnet.*.csproj" -Recurse
foreach ($project in $projects) {
    $packagePattern = "^$([regex]::Escape($project.BaseName))\.[0-9].*\.nupkg$"
    $packages = @(Get-ChildItem $packageDirectoryPath -Filter "*.nupkg" |
        Where-Object { $_.Name -cmatch $packagePattern })
    if ($packages.Count -ne 1) {
        throw "Expected one package for $($project.BaseName), found $($packages.Count)."
    }

    $archive = [System.IO.Compression.ZipFile]::OpenRead($packages[0].FullName)
    try {
        $entries = @($archive.Entries | ForEach-Object { $_.FullName })
        $expectedEntries = @(
            "skills/mdev-dotnet/SKILL.md"
            "buildTransitive/$($project.BaseName).targets"
        )

        foreach ($entry in $expectedEntries) {
            if ($entry -cnotin $entries) {
                throw "$($packages[0].Name) does not contain $entry."
            }
        }

        $skillEntry = $archive.GetEntry("skills/mdev-dotnet/SKILL.md")
        $reader = [System.IO.StreamReader]::new($skillEntry.Open())
        try {
            $skillContent = $reader.ReadToEnd()
        }
        finally {
            $reader.Dispose()
        }

        if ($ExpectedVersion -and
            -not $skillContent.Contains("- Source version: $ExpectedVersion")) {
            throw "$($packages[0].Name) does not contain skill version $ExpectedVersion."
        }

        foreach ($link in [regex]::Matches($skillContent, '\[[^\]]+\]\((?<target>[^)]+)\)')) {
            $target = $link.Groups["target"].Value.Split('#')[0]
            if (-not $target -or $target -match '^[a-z][a-z0-9+.-]*:' -or $target.StartsWith("#")) {
                continue
            }

            $packagedTarget = "skills/mdev-dotnet/$target"
            if ($packagedTarget -cnotin $entries) {
                throw "$($packages[0].Name) does not contain linked skill file $packagedTarget."
            }
        }
    }
    finally {
        $archive.Dispose()
    }
}

Write-Host "All $($projects.Count) packages contain a complete MDev.Dotnet agent skill and installer."
