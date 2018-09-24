[CmdletBinding()]
param(
    [ValidateScript({Test-Path $_})][string]$path
)

Function Test-NuGetPackage {
    [CmdletBinding()]
    param(
        [ValidateScript({Test-Path $_})][string]$path
    )

    $path = Resolve-Path $path
    [bool]$assemblyFound = $false
    Read-Archive -Path $path | %{
        if($_.Path -like 'lib\*.dll') {
            $assemblyFound = $true
        }
    }
    if(-not $assemblyFound) {
        throw "No assembly found in 'lib' directory."
    }


}

Test-NuGetPackage @PSBoundParameters