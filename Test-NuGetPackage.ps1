[CmdletBinding()]
param(
    [ValidateScript({Test-Path $_})][Parameter(Mandatory)][Alias('NugetPackagePath')][string]$Path,
    [string]$AssemblyName
)

Function Test-NuGetPackage {
    [CmdletBinding()]
    param(
        [ValidateScript({Test-Path $_})][string]$Path,
        [string]$AssemblyName
    )
    if(-not $AssemblyName) {
        Write-Warning '$AssemblyName parameter is empty.'
    }
    $Path = Resolve-Path $Path
    [bool]$assemblyFound = $false
    Read-Archive -Path $Path | %{
        # Look for the specific assembly within the Nuget package if $AssemblyName is provided.
        if($_.Path -like "lib\*$AssemblyName.dll") {
            $assemblyFound = $true
            Write-Information -
        }
    }
    if(-not $assemblyFound) {
        Write-Error "No assembly found in 'lib' directory."
        Write-Output $false
    }
    else {
        Write-Output $true
    }
}

if($PSBoundParameters.Count -gt 0) {
    Test-NuGetPackage @PSBoundParameters
}