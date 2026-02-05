[![Ministry of Justice Repository Compliance Badge](https://github-community.service.justice.gov.uk/repository-standards/api/CFO-ExternalDataIntegrationSystem/badge?1)](https://github-community.service.justice.gov.uk/repository-standards/CFO-ExternalDataIntegrationSystem)

HMPPS CFO DMS
=============
[![Run Tests](https://github.com/ministryofjustice/CFO-ExternalDataIntegrationSystem/actions/workflows/run-tests.yml/badge.svg)](https://github.com/ministryofjustice/CFO-ExternalDataIntegrationSystem/actions/workflows/run-tests.yml)
[![.NET](https://img.shields.io/badge/.NET-10.0-512BD4)](https://dotnet.microsoft.com/)
[![License](https://img.shields.io/badge/license-MIT-blue.svg)](LICENSE)
[![Issues](https://img.shields.io/github/issues/ministryofjustice/CFO-ExternalDataIntegrationSystem)](https://github.com/ministryofjustice/CFO-ExternalDataIntegrationSystem/issues)
[![Pull Requests](https://img.shields.io/github/issues-pr/ministryofjustice/CFO-ExternalDataIntegrationSystem)](https://github.com/ministryofjustice/CFO-ExternalDataIntegrationSystem/pulls)
## Overview
HMPPS Creating Future Opportunities (CFO) - Data Management System (DMS). It is intended for internal use only and is used to process PNOMIS and NDelius offender data to supply CATS (Case Assessment and Tracking System - also used by HMPPS CFO) with accurate offender movements and updates.

## Architecture
CFO DMS is built as a distributed microservices architecture. Data flows through the following pipeline:

**File Ingestion → Parsing/Cleaning → Staging → Import → Running Picture → Blocking/Matching → Clustering → Data Consumption**

### Pipeline Applications
1. **File Ingestion** - [**FileSync**](src/FileSync) monitors MinIO/S3/FileSystem storage and syncs incoming files
2. **Parsing/Cleaning** - [**Offloc.Parser**](src/Offloc.Parser), [**Offloc.Cleaner**](src/Offloc.Cleaner), [**Delius.Parser**](src/Delius.Parser) transform raw p-NOMIS and nDelius files into structured records
3. **Staging/Import/Running Picture** - [**Import**](src/Import) validates and migrates data from staging to running picture databases
4. **Blocking/Matching** - [**Blocking**](src/Blocking) generates candidate record pairs, [**Matching.Engine**](src/Matching.Engine) identifies and links related offender records across systems
5. **Clustering** - [**Matching.Engine**](src/Matching.Engine) groups related records into clusters representing unique individuals
6. **Data Consumption** - [**API**](src/API) exposes the processed data via REST endpoints for downstream consumers (e.g., CATS), [**Visualiser**](src/Visualiser) provides a web UI for exploring and visualising relationships between offender data

### Supporting Applications
- [**Cleanup**](src/Cleanup) - Performs data maintenance tasks
- [**DbInteractions**](src/DbInteractions) handles complex database operations
- [**Logging**](src/Logging) - Centralised logging service

Services communicate asynchronously via RabbitMQ message queues. See the Message Flow Diagram below for detailed service interactions.

# Development Setup and Execution Guide

## Prerequisites
- [.NET 10 SDK](https://dotnet.microsoft.com/download/dotnet/10.0)
- **Visual Studio Code users**:
    - [C# Dev Kit Extension](https://marketplace.visualstudio.com/items?itemName=ms-dotnettools.csdevkit)
    - [Aspire Extension](https://marketplace.visualstudio.com/items?itemName=microsoft-aspire.aspire-vscode)

## Setup (development)
1. **(Optional)** To use the Visualiser app, you must configure secret(s) for applications in the *src* directory:
    - *Visualiser.csproj* → Manage User Secrets
        ```json
        {
            "AzureAd:ClientSecret": "<ENTRA_CLIENT_SECRET>"
        }
        ```

## Database Deployment (Test Environments)

If you are deploying to test DROP existing databases before continuing. 

````sql
DECLARE @Databases TABLE (DbName sysname);

INSERT INTO @Databases (DbName)
VALUES
    ('ClusterDb'),
    ('DeliusRunningPictureDb'),
    ('DeliusStagingDb'),
    ('MatchingDb'),
    ('OfflocRunningPictureDb'),
    ('OfflocStagingDb'),
    ('AuditDb');

DECLARE @sql nvarchar(max) = N'';

SELECT @sql += '
IF EXISTS (SELECT 1 FROM sys.databases WHERE name = ''' + DbName + ''')
BEGIN
    ALTER DATABASE [' + DbName + '] SET SINGLE_USER WITH ROLLBACK IMMEDIATE;
    DROP DATABASE [' + DbName + '];
END;'
FROM @Databases;

EXEC sys.sp_executesql @sql;
````

The `publish_db.py` script is provided for deploying database projects to test environments.

### Prerequisites

| Requirement | How to check | How to install (bash) |
|------------|-------------|------------------------|
| **Python 3** | `python3 --version` | **macOS (Homebrew)**<br>`brew install python`<br><br>**Ubuntu / Debian**<br>`sudo apt update && sudo apt install -y python3 python3-pip` |
| **.NET SDK 8.x** | `dotnet --list-sdks` | **macOS (Homebrew)**<br>`brew install --cask dotnet-sdk@8`<br><br>**Ubuntu / Debian**<br>`sudo apt update && sudo apt install -y dotnet-sdk-8.0` |
| **.NET SDK 10.x** | `dotnet --list-sdks` | **macOS (Homebrew)**<br>`brew install --cask dotnet-sdk@10`<br><br>**Ubuntu / Debian**<br>`sudo apt update && sudo apt install -y dotnet-sdk-10.0` |
| **sqlpackage (dotnet tool)** | `dotnet tool list -g` | `dotnet tool install -g microsoft.sqlpackage` |


### Usage
1. **Set required environment variables** before running:
   ```bash
   export SERVER="your-test-sql-server-address"
   export DB_USER="your-database-username"
   export DB_PASS="your-database-password"
   ```

2. **Run the script from the project root directory**:
   ```bash
   # Preview changes without deploying
   python3 publish_db.py --dry-run
   
   # Deploy to test environment (Release build - recommended)
   python3 publish_db.py
   
   # Deploy using Debug build
   python3 publish_db.py --config Debug
   ```

The script will build and publish all database projects (AuditDb, OfflocStagingDb, DeliusStagingDb, OfflocRunningPictureDb, DeliusRunningPictureDb, MatchingDb, ClusterDb) to the specified test server. You will be prompted to confirm before deployment begins.

#### Seeding Test Data

If you want to seed the test data, you can run the Fake Data Seeder project. 

   ```bash
      export ConnectionStrings__ClusterDb="Server=$SERVER;Database=ClusterDb;User Id=$DB_USER;Password=$DB_PASS;TrustServerCertificate=True;"

      dotnet run --project ./src/FakeDataSeeder/FakeDataSeeder.csproj;  
   ```

## Running the apps
The recommended way to run and debug these apps is using .NET Aspire.
- **Using Visual Studio Code**: open the project and press `F5`, selecting the *Default Configuration*.
- **Using Visual Studio or other IDEs**: From the debug configuration dropdown, select `Aspire.AppHost` and start the application.

### Services and Credentials
When running via Aspire, the following services are available:

| Service | Purpose | Access | Credentials |
|---------|---------|--------|-------------|
| **API** | REST endpoints for querying offender data, searches, and clustering operations | https://localhost:7013/swagger | API Key: `password` |
| **MinIO** | S3-compatible file storage |  *random port* (check Aspire) | Username: `minioadmin`<br>Password: `minioadmin` |
| **MSSQL** | Application databases (staging, running picture, matching, cluster) | `127.0.0.1,61749` | Username: `sa`<br>Password: `P@ssword123!` |
| **RabbitMQ** | Message broker for inter-service communication | http://localhost:15672 | Username: `guest`<br>Password: `guest` |

## Message Flow Diagram

```
┌─────────────────────────────────────────────────────────────────────────────┐
│                         INITIAL FILE DETECTION                              │
└─────────────────────────────────────────────────────────────────────────────┘

    ┌──────────────┐
    │   FileSync   │  (Monitors storage for new files)
    └──────┬───────┘
           │
           ├─────────────────────────────────────────┐
           │                                         │
           ▼                                         ▼
    DeliusDownloadFinishedMessage           OfflocDownloadFinished
           │                                         │
           │                                         │
           ▼                                         ▼

┌─────────────────────────────────────────────────────────────────────────────┐
│                      PARSING & CLEANING STAGE                               │
└─────────────────────────────────────────────────────────────────────────────┘

    ┌──────────────────┐                    ┌─────────────────┐
    │  Delius.Parser   │                    │ Offloc.Cleaner  │
    │                  │                    │                 │
    │ (Parses Delius   │                    │ (Cleans Offloc  │
    │  files into      │                    │  files, removes │
    │  structured      │                    │  redundant      │
    │  records)        │                    │  fields)        │
    └────────┬─────────┘                    └────────┬────────┘
             │                                       │
             │ Sends DB requests:                    │
             │ - StartDeliusFileProcessingRequest    │
             │                                       │
             ▼                                       ▼
    DeliusParserFinishedMessage         OfflocCleanerFinishedMessage
             │                                       │
             │                                       │
             │                                       ▼
             │                              ┌─────────────────┐
             │                              │ Offloc.Parser   │
             │                              │                 │
             │                              │ (Parses cleaned │
             │                              │  Offloc files   │
             │                              │  into structured│
             │                              │  records)       │
             │                              └────────┬────────┘
             │                                       │
             │                                       │ Sends DB requests:
             │                                       │ - StartOfflocFileProcessingRequest
             │                                       │
             │                                       ▼
             │                         OfflocParserFinishedMessage
             │                                       │
             └───────────────┬───────────────────────┘
                             │
                             ▼

┌─────────────────────────────────────────────────────────────────────────────┐
│                    STAGING & IMPORT STAGE                                   │
└─────────────────────────────────────────────────────────────────────────────┘

                    ┌────────────────┐
                    │     Import     │
                    │                │
                    │ (Coordinates   │
                    │  staging and   │
                    │  merging of    │
                    │  both data     │
                    │  sources)      │
                    └───────┬────────┘
                            │
                            │ Sends DB requests:
                            │ - StageDeliusRequest
                            │ - MergeDeliusRequest
                            │ - StageOfflocRequest
                            │ - MergeOfflocRequest
                            │
                            ▼
                    ┌────────────────┐
                    │ DbInteractions │
                    │                │
                    │ (Stages data   │
                    │  from parsers, │
                    │  merges into   │
                    │  running       │
                    │  picture DB)   │
                    └───────┬────────┘
                            │
                            │ Sends responses:
                            │ - StageDeliusResponse
                            │ - MergeDeliusResponse
                            │ - StageOfflocResponse
                            │ - MergeOfflocResponse
                            │ - DeliusFilesCleanupMessage
                            │ - OfflocFilesCleanupMessage
                            │
                            ▼
                    ImportFinishedMessage
                            │
                            │
                            ▼

┌─────────────────────────────────────────────────────────────────────────────┐
│                      MATCHING & BLOCKING STAGE                              │
└─────────────────────────────────────────────────────────────────────────────┘

                    ┌────────────────┐
                    │    Blocking    │
                    │                │
                    │ (Generates     │
                    │  candidate     │
                    │  pairs of      │
                    │  records that  │
                    │  may match)    │
                    └───────┬────────┘
                            │
                            ▼
                  BlockingFinishedMessage
                            │
                            │
                            ▼
            ┌───────────────────────────────┐
            │   Matching.Engine             │
            │   (ComparatorService)         │
            │                               │
            │ (Compares candidate pairs     │
            │  using matching rules to      │
            │  identify potential matches)  │
            └───────────────┬───────────────┘
                            │
                            ▼
              MatchingScoreCandidatesMessage
                            │
                            │
                            ▼
            ┌───────────────────────────────┐
            │   Matching.Engine             │
            │   (ScorerService)             │
            │                               │
            │ (Scores comparisons using     │
            │  Bayesian probability to      │
            │  determine match likelihood)  │
            └───────────────┬───────────────┘
                            │
                            ▼
          MatchingScoreCandidatesFinishedMessage
                            │
                            │
                            ▼

┌─────────────────────────────────────────────────────────────────────────────┐
│                        CLUSTERING STAGE                                     │
└─────────────────────────────────────────────────────────────────────────────┘

            ┌───────────────────────────────┐
            │   Matching.Engine             │
            │   (ClusteringService)         │
            │                               │
            │ (Pre-processes clustering:    │
            │  prepares data for grouping)  │
            └───────────────┬───────────────┘
                            │
                            ▼
          ClusteringPreProcessingStartedMessage
                            │
                            │
                            ▼
            ┌───────────────────────────────┐
            │   Matching.Engine             │
            │   (ComparatorService)         │
            │                               │
            │ (Compares outstanding edges   │
            │  for clustering)              │
            └───────────────┬───────────────┘
                            │
                            ▼
          MatchingScoreOutstandingEdgesMessage
                            │
                            │
                            ▼
            ┌───────────────────────────────┐
            │   Matching.Engine             │
            │   (ScorerService)             │
            │                               │
            │ (Scores outstanding edges)    │
            └───────────────┬───────────────┘
                            │
                            ▼
          ClusteringPreProcessingFinishedMessage
                            │
                            │
                            ▼
            ┌───────────────────────────────┐
            │   Matching.Engine             │
            │   (ClusteringService)         │
            │                               │
            │ (Post-processes clustering:   │
            │  groups related records into  │
            │  clusters representing        │
            │  unique individuals)          │
            └───────────────┬───────────────┘
                            │
                            ▼
          ClusteringPostProcessingFinishedMessage
                            │
                            │
                            ▼
                    ┌────────────────┐
                    │   FileSync     │
                    │                │
                    │ (Triggers next │
                    │  processing    │
                    │  cycle if      │
                    │  configured)   │
                    └────────────────┘

┌─────────────────────────────────────────────────────────────────────────────┐
│                        DATA CONSUMPTION                                     │
└─────────────────────────────────────────────────────────────────────────────┘

                    ┌────────────────┐
                    │      API       │
                    │                │
                    │ (Exposes REST  │
                    │  endpoints for │
                    │  querying      │
                    │  processed     │
                    │  data)         │
                    └───────┬────────┘
                            │
                            ▼
                    External Consumers
                    (e.g., CATS system)
```