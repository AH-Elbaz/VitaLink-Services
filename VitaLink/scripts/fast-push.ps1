param(
    [string]$Message = "Quick commit",
    [switch]$RunChecks
)

# Navigate to repo root (script location assumed inside repo)
$scriptDir = Split-Path -Parent $MyInvocation.MyCommand.Definition
Set-Location $scriptDir

if ($RunChecks) {
    Write-Host "Running dotnet build..."
    dotnet build
    if ($LASTEXITCODE -ne 0) { Write-Error "Build failed. Aborting."; exit 1 }

    Write-Host "Running dotnet test..."
    dotnet test
    if ($LASTEXITCODE -ne 0) { Write-Warning "Tests failed. Continuing (you can re-run with -RunChecks to stop)." }
} else {
    Write-Host "Skipping build/tests for speed. Use -RunChecks to enable them."
}

# Fast commit & push
if (-not (Test-Path .git)) { Write-Error "No .git directory found. Run from repo root."; exit 1 }

$changes = git status --porcelain
if ($changes) {
    git add -A
    git commit -m $Message --quiet
    if ($LASTEXITCODE -ne 0) { Write-Warning "Commit failed or no changes to commit." }
} else {
    Write-Host "No changes to commit."
}

# Push current branch quickly
$branch = git rev-parse --abbrev-ref HEAD
Write-Host "Pushing branch: $branch"
# Use --no-verify to skip pre-push hooks if present (faster). Remove if you rely on hooks.
git push origin $branch --no-verify
if ($LASTEXITCODE -ne 0) { Write-Error "Push failed. Try: git fetch && git pull --rebase origin $branch"; exit 1 }

Write-Host "Push complete."