# Docker Management Scripts for Math Tutor Application
# PowerShell scripts for Windows

param(
    [Parameter(Mandatory=$true)]
    [ValidateSet("start", "stop", "restart", "logs", "build", "clean", "dev", "prod")]
    [string]$Action,
    
    [string]$Service = ""
)

function Show-Help {
    Write-Host "Math Tutor Docker Management Script" -ForegroundColor Green
    Write-Host ""
    Write-Host "Usage: .\docker-scripts.ps1 -Action <action> [-Service <service>]" -ForegroundColor Yellow
    Write-Host ""
    Write-Host "Actions:" -ForegroundColor Cyan
    Write-Host "  start    - Start all services (production)"
    Write-Host "  dev      - Start development environment"
    Write-Host "  stop     - Stop all services"
    Write-Host "  restart  - Restart all services"
    Write-Host "  logs     - Show logs (optionally for specific service)"
    Write-Host "  build    - Rebuild all images"
    Write-Host "  clean    - Clean up containers and volumes"
    Write-Host ""
    Write-Host "Services (for logs): backend, frontend, postgres" -ForegroundColor Cyan
    Write-Host ""
    Write-Host "Examples:" -ForegroundColor Yellow
    Write-Host "  .\docker-scripts.ps1 -Action start"
    Write-Host "  .\docker-scripts.ps1 -Action dev"
    Write-Host "  .\docker-scripts.ps1 -Action logs -Service backend"
    Write-Host "  .\docker-scripts.ps1 -Action clean"
}

function Start-Production {
    Write-Host "Starting Math Tutor in Production Mode..." -ForegroundColor Green
    docker-compose up -d
    Write-Host "Services started. Access the application at:" -ForegroundColor Green
    Write-Host "  Frontend: http://localhost" -ForegroundColor Yellow
    Write-Host "  Backend API: http://localhost:5000" -ForegroundColor Yellow
    Write-Host "  Swagger: http://localhost:5000/swagger" -ForegroundColor Yellow
}

function Start-Development {
    Write-Host "Starting Math Tutor in Development Mode..." -ForegroundColor Green
    docker-compose -f docker-compose.dev.yml up -d
    Write-Host "Development services started. Access the application at:" -ForegroundColor Green
    Write-Host "  Frontend (Dev): http://localhost:3000" -ForegroundColor Yellow
    Write-Host "  Backend API: http://localhost:5000" -ForegroundColor Yellow
    Write-Host "  Swagger: http://localhost:5000/swagger" -ForegroundColor Yellow
}

function Stop-Services {
    Write-Host "Stopping all services..." -ForegroundColor Yellow
    docker-compose down
    docker-compose -f docker-compose.dev.yml down
    Write-Host "All services stopped." -ForegroundColor Green
}

function Restart-Services {
    Write-Host "Restarting services..." -ForegroundColor Yellow
    Stop-Services
    Start-Sleep -Seconds 2
    Start-Production
}

function Show-Logs {
    if ($Service) {
        Write-Host "Showing logs for service: $Service" -ForegroundColor Cyan
        docker-compose logs -f $Service
    } else {
        Write-Host "Showing logs for all services..." -ForegroundColor Cyan
        docker-compose logs -f
    }
}

function Build-Images {
    Write-Host "Rebuilding all Docker images..." -ForegroundColor Yellow
    docker-compose build --no-cache
    Write-Host "Images rebuilt successfully." -ForegroundColor Green
}

function Clean-Docker {
    Write-Host "Cleaning up Docker resources..." -ForegroundColor Yellow
    Write-Host "WARNING: This will remove all containers, volumes, and unused images!" -ForegroundColor Red
    $confirm = Read-Host "Are you sure? (y/N)"

    if ($confirm -eq "y" -or $confirm -eq "Y") {
        docker-compose down -v
        docker-compose -f docker-compose.dev.yml down -v
        docker system prune -f
        docker volume prune -f
        Write-Host "Docker cleanup completed." -ForegroundColor Green
    } else {
        Write-Host "Cleanup cancelled." -ForegroundColor Yellow
    }
}

# Main script logic
switch ($Action) {
    "start" { Start-Production }
    "prod" { Start-Production }
    "dev" { Start-Development }
    "stop" { Stop-Services }
    "restart" { Restart-Services }
    "logs" { Show-Logs }
    "build" { Build-Images }
    "clean" { Clean-Docker }
    default { Show-Help }
}
