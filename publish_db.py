
#!/usr/bin/env python3
import os
import sys
import subprocess
import argparse
from datetime import datetime
from pathlib import Path

def log(message):
    timestamp = datetime.now().strftime("%Y-%m-%d %H:%M:%S")
    print(f"[{timestamp}] {message}")

def validate_environment():
    SERVER = os.environ.get("SERVER")
    DB_USER = os.environ.get("DB_USER")
    DB_PASS = os.environ.get("DB_PASS")

    if not SERVER or not DB_USER or not DB_PASS:
        log("ERROR: Missing required environment variables.")
        print("Required: SERVER, DB_USER, DB_PASS")
        sys.exit(1)
    
    return SERVER, DB_USER, DB_PASS

def validate_project_files(databases, build_config):
    log("Validating project files...")
    missing_files = []
    for db in databases:
        project_file = Path(f"./src/database/{db}/{db}.sqlproj")
        if not project_file.exists():
            missing_files.append(str(project_file))
    
    if missing_files:
        log("ERROR: Missing project files:")
        for f in missing_files:
            print(f"  - {f}")
        sys.exit(1)
    
    log("All project files found.")

def build_all_databases(databases, build_config, dry_run):
    log(f"Building all database projects ({build_config} configuration)...")
    
    if dry_run:
        log("[DRY RUN] Would build all projects")
        return True
    
    try:
        for db in databases:
            log(f"Building {db}...")
            subprocess.check_call(
                ["dotnet", "build", f"./src/database/{db}/{db}.sqlproj", 
                 "-c", build_config, "--nologo", "-v", "minimal"],
                stdout=subprocess.DEVNULL
            )
        log("All projects built successfully.")
        return True
    except subprocess.CalledProcessError as e:
        log(f"ERROR: Build failed for {db}")
        return False

def publish_database(db, server, user, password, build_config, dry_run):
    conn = (
        f"Server={server};Database={db};"
        f"User Id={user};Password={password};"
        f"TrustServerCertificate=True;"
    )
    
    dacpac_path = f"./src/database/{db}/bin/{build_config}/{db}.dacpac"
    
    if dry_run:
        log(f"[DRY RUN] Would publish {dacpac_path} to {server}/{db}")
        return True
    
    log(f"Publishing {db} to {server}...")
    
    try:
        subprocess.check_call([
            "dotnet", "sqlpackage",
            "/action:Publish",
            f"/SourceFile:{dacpac_path}",
            f"/TargetConnectionString:{conn}",
            "/v:OfflocRunningPictureDb=OfflocRunningPictureDb",
            "/v:DeliusRunningPictureDb=DeliusRunningPictureDb",
            "/v:MatchingDb=MatchingDb",
            "/Quiet:True"
        ])
        log(f"Successfully published {db}")
        return True
    except subprocess.CalledProcessError as e:
        log(f"ERROR: Failed to publish {db}")
        return False

def main():
    parser = argparse.ArgumentParser(description="Deploy database projects to test environments")
    parser.add_argument("--dry-run", action="store_true", help="Show what would be done without making changes")
    parser.add_argument("--config", choices=["Debug", "Release"], default="Release", help="Build configuration (default: Release)")
    args = parser.parse_args()

    DATABASES = [
        "AuditDb",
        "OfflocStagingDb",
        "DeliusStagingDb",
        "OfflocRunningPictureDb",
        "DeliusRunningPictureDb",
        "MatchingDb",
        "ClusterDb",
    ]

    log("=== Database Deployment Script ===")
    
    SERVER, DB_USER, DB_PASS = validate_environment()
    
    log(f"Target Server: {SERVER}")
    log(f"User: {DB_USER}")
    log(f"Build Config: {args.config}")
    log(f"Databases: {', '.join(DATABASES)}")
    
    if args.dry_run:
        log("DRY RUN MODE - No changes will be made")
    else:
        response = input("\nProceed with deployment? (yes/no): ")
        if response.lower() not in ["yes", "y"]:
            log("Deployment cancelled by user")
            sys.exit(0)
    
    validate_project_files(DATABASES, args.config)
    
    if not build_all_databases(DATABASES, args.config, args.dry_run):
        log("Deployment aborted due to build failures")
        sys.exit(1)
    
    failed_databases = []
    for db in DATABASES:
        if not publish_database(db, SERVER, DB_USER, DB_PASS, args.config, args.dry_run):
            failed_databases.append(db)
    
    if failed_databases:
        log(f"ERROR: Deployment completed with failures: {', '.join(failed_databases)}")
        sys.exit(1)
    
    log("=== All databases deployed successfully ===")

if __name__ == "__main__":
    main()
