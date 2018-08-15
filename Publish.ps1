[CmdletBinding(SupportsShouldProcess=$True, ConfirmImpact="High")]
param (
    [string]$Filter = ""
    ,[string]$NugetAPIKey
    ,[string]$Source='nuget.org'
    ,[switch]$SaveAPIKey
)

$projectFolders = Get-ChildItem $PSScriptRoot\IntelliTect.* -Directory -Filter $filter | Where-Object {
    $_.Name -notlike '*.Tests'
}
$pacakgesToPublish = @()

Write-Progress -Activity "Publish IntelliTect TestTools" -Status "Searching for projects ready to publish"

if(!$projectFolders) {
    throw "Nothing matches the filter, '$Filter'"
}

foreach ($item in $projectFolders){
    $projectName = $item.Name

    $Nupkg = Get-ChildItem $projectName $projectName*.nupkg -Recurse | Sort-Object Name -Descending | Select-Object -First 1

    if($Nupkg) {
        $localVersion = $Nupkg.Name | Where-Object{
            $_ -match "$projectName\.(?<Version>.*)\.nupkg"
        } | ForEach-Object {
            $matches.Version
        }

        Write-Progress -Activity "Publish IntelliTect TestTools" -Status "Checking for previously published versions of '$projectName'"
        $latestPublishedProjectVersion = nuget.exe list $projectName

        [string]$lastPublishedVersion=$null
        if($latestPublishedProjectVersion) {
            if($latestPublishedProjectVersion -match "$projectName\s*(?<Version>\d{1,4}\.\d{1,4}\.\d{1,4})") {
                $lastPublishedVersion = $matches.Version
            }
            elseif($latestPublishedProjectVersion -ne 'No packages found.') {
                throw "Unable to determine version of $projectName from nuget output '$latestPublishedProjectVersion'"
            }
        }

        if($localVersion -gt $lastPublishedVersion) {
            $verbosityOption = if($PSBoundParameters['Verbose']){'-Verbosity','detailed'}
            $apiKeyOption = if($NugetAPIKey){'-ApiKey',$NugetAPIKey}
            $sourceOption = '-Source',$Source
            if($PSCmdlet.ShouldProcess("Should publish '$Nupkg'")) {
                Write-Progress -Activity "Publish IntelliTect TestTools" -Status "Publishing '$Nupkg'"
                Write-Verbose "Executing: & nuget.exe push $Nupkg $sourceOption $apiKeyOption $verbosityOption 2>&1"
                $output = & nuget.exe push $Nupkg.FullName $sourceOption $apiKeyOption $verbosityOption 2>&1
                if($LASTEXITCODE -ne 0) {
                    $stdErr = @()
                    $stdOut = @()
                    $output | ForEach-Object {
                        if( $_ -is [System.Management.Automation.ErrorRecord] ) {
                            $stdErr += $_
                        }
                        else {
                            # $stdOut += $_  # Not currently used anywhere
                        }
                    }
                    throw $stdErr
                }
                if($PSCmdlet.ShouldProcess("Should publish '$Nupkg'")) {
                if($SaveAPIKey -and $NugetAPIKey) {
                    nuget.exe setApiKey $NugetAPIKey $source $verbosity
                }
                }
            }
        }
    }
}

