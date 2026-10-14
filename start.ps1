[CmdletBinding()]
param(
    [ValidateRange(5, 180)]
    [int]$StartupTimeoutSeconds = 60
)

$ErrorActionPreference = "Stop"
$root = $PSScriptRoot
$apiProject = Join-Path $root "src\DeLimaIt.Dictionary.Api\DeLimaIt.Dictionary.Api.csproj"
$webProject = Join-Path $root "src\DeLimaIt.Dictionary.Web\DeLimaIt.Dictionary.Web.csproj"
$apiUrl = "http://localhost:5080"
$webUrl = "http://localhost:5050"
$logDirectory = Join-Path $env:LOCALAPPDATA "DeLimaIt.Dictionary\logs"

if (-not (Get-Command dotnet -ErrorAction SilentlyContinue)) {
    throw "O comando 'dotnet' não foi encontrado. Instale o SDK do .NET 8 e tente novamente."
}

foreach ($project in @($apiProject, $webProject)) {
    if (-not (Test-Path -LiteralPath $project)) {
        throw "Projeto não encontrado: $project"
    }
}

New-Item -ItemType Directory -Path $logDirectory -Force | Out-Null

function Test-HttpEndpoint {
    param([string]$Url)

    try {
        $response = Invoke-WebRequest -Uri $Url -TimeoutSec 2 -UseBasicParsing
        return $response.StatusCode -ge 200 -and $response.StatusCode -lt 500
    }
    catch {
        return $false
    }
}

function Start-DictionaryService {
    param(
        [string]$Name,
        [string]$Project,
        [string]$Url
    )

    if (Test-HttpEndpoint -Url $Url) {
        Write-Host "$Name já está respondendo em $Url"
        return $null
    }

    $stdout = Join-Path $logDirectory "$Name.stdout.log"
    $stderr = Join-Path $logDirectory "$Name.stderr.log"
    $arguments = "run --project `"$Project`" --launch-profile http"

    Start-Process `
        -FilePath (Get-Command dotnet).Source `
        -ArgumentList $arguments `
        -WorkingDirectory $root `
        -WindowStyle Hidden `
        -RedirectStandardOutput $stdout `
        -RedirectStandardError $stderr `
        -PassThru
}

$previousBrowserSetting = [Environment]::GetEnvironmentVariable("DOTNET_LAUNCH_BROWSER", "Process")
$startedProcesses = [System.Collections.Generic.List[System.Diagnostics.Process]]::new()

try {
    # Keep the launch profiles' Development environment while suppressing automatic browser launches.
    $env:DOTNET_LAUNCH_BROWSER = "0"

    $apiProcess = Start-DictionaryService -Name "api" -Project $apiProject -Url $apiUrl
    if ($null -ne $apiProcess) { $startedProcesses.Add($apiProcess) }

    $webProcess = Start-DictionaryService -Name "web" -Project $webProject -Url $webUrl
    if ($null -ne $webProcess) { $startedProcesses.Add($webProcess) }
}
finally {
    if ($null -eq $previousBrowserSetting) {
        Remove-Item Env:\DOTNET_LAUNCH_BROWSER -ErrorAction SilentlyContinue
    }
    else {
        $env:DOTNET_LAUNCH_BROWSER = $previousBrowserSetting
    }
}

$deadline = (Get-Date).AddSeconds($StartupTimeoutSeconds)
while ((Get-Date) -lt $deadline) {
    $apiReady = Test-HttpEndpoint -Url "$apiUrl/swagger/index.html"
    $webReady = Test-HttpEndpoint -Url $webUrl
    if ($apiReady -and $webReady) {
        Write-Host "API:       $apiUrl/swagger"
        Write-Host "Frontend:  $webUrl"
        Write-Host "Logs:      $logDirectory"
        if ($startedProcesses.Count -gt 0) {
            Write-Host "Processos iniciados: $($startedProcesses.Id -join ', ')"
        }
        return
    }

    foreach ($process in $startedProcesses) {
        $process.Refresh()
        if ($process.HasExited) {
            $name = if ($process.Id -eq $apiProcess.Id) { "API" } else { "Frontend" }
            throw "$name encerrou durante a inicialização. Consulte os logs em $logDirectory"
        }
    }

    Start-Sleep -Milliseconds 500
}

throw "API e frontend não ficaram prontos em $StartupTimeoutSeconds segundos. Consulte os logs em $logDirectory"
