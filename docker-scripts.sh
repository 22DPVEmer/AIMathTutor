#!/bin/bash

# Docker Management Scripts for Math Tutor Application
# Bash scripts for Linux/Mac

show_help() {
    echo -e "\033[32mMath Tutor Docker Management Script\033[0m"
    echo ""
    echo -e "\033[33mUsage: ./docker-scripts.sh <action> [service]\033[0m"
    echo ""
    echo -e "\033[36mActions:\033[0m"
    echo "  start    - Start all services (production)"
    echo "  dev      - Start development environment"
    echo "  stop     - Stop all services"
    echo "  restart  - Restart all services"
    echo "  logs     - Show logs (optionally for specific service)"
    echo "  build    - Rebuild all images"
    echo "  clean    - Clean up containers and volumes"
    echo ""
    echo -e "\033[36mServices (for logs): backend, frontend, postgres\033[0m"
    echo ""
    echo -e "\033[33mExamples:\033[0m"
    echo "  ./docker-scripts.sh start"
    echo "  ./docker-scripts.sh dev"
    echo "  ./docker-scripts.sh logs backend"
    echo "  ./docker-scripts.sh clean"
}

start_production() {
    echo -e "\033[32mStarting Math Tutor in Production Mode...\033[0m"
    docker-compose up -d
    echo -e "\033[32mServices started. Access the application at:\033[0m"
    echo -e "\033[33m  Frontend: http://localhost\033[0m"
    echo -e "\033[33m  Backend API: http://localhost:5000\033[0m"
    echo -e "\033[33m  Swagger: http://localhost:5000/swagger\033[0m"
}

start_development() {
    echo -e "\033[32mStarting Math Tutor in Development Mode...\033[0m"
    docker-compose -f docker-compose.dev.yml up -d
    echo -e "\033[32mDevelopment services started. Access the application at:\033[0m"
    echo -e "\033[33m  Frontend (Dev): http://localhost:3000\033[0m"
    echo -e "\033[33m  Backend API: http://localhost:5000\033[0m"
    echo -e "\033[33m  Swagger: http://localhost:5000/swagger\033[0m"
}

stop_services() {
    echo -e "\033[33mStopping all services...\033[0m"
    docker-compose down
    docker-compose -f docker-compose.dev.yml down
    echo -e "\033[32mAll services stopped.\033[0m"
}

restart_services() {
    echo -e "\033[33mRestarting services...\033[0m"
    stop_services
    sleep 2
    start_production
}

show_logs() {
    if [ -n "$2" ]; then
        echo -e "\033[36mShowing logs for service: $2\033[0m"
        docker-compose logs -f "$2"
    else
        echo -e "\033[36mShowing logs for all services...\033[0m"
        docker-compose logs -f
    fi
}

build_images() {
    echo -e "\033[33mRebuilding all Docker images...\033[0m"
    docker-compose build --no-cache
    echo -e "\033[32mImages rebuilt successfully.\033[0m"
}

clean_docker() {
    echo -e "\033[33mCleaning up Docker resources...\033[0m"
    echo -e "\033[31mWARNING: This will remove all containers, volumes, and unused images!\033[0m"
    read -p "Are you sure? (y/N): " confirm

    if [[ $confirm == [yY] ]]; then
        docker-compose down -v
        docker-compose -f docker-compose.dev.yml down -v
        docker system prune -f
        docker volume prune -f
        echo -e "\033[32mDocker cleanup completed.\033[0m"
    else
        echo -e "\033[33mCleanup cancelled.\033[0m"
    fi
}

# Main script logic
case "$1" in
    "start"|"prod")
        start_production
        ;;
    "dev")
        start_development
        ;;
    "stop")
        stop_services
        ;;
    "restart")
        restart_services
        ;;
    "logs")
        show_logs "$@"
        ;;
    "build")
        build_images
        ;;
    "clean")
        clean_docker
        ;;
    *)
        show_help
        ;;
esac
