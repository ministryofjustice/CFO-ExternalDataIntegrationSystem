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
CFO DMS is built as a microservices architecture using .NET Aspire for orchestration. Data flows through the following pipeline:

**File Ingestion → Parsing/Cleaning → Staging → Import → Running Picture → Blocking/Matching → Clustering**

1. **FileSync** monitors MinIO/S3/FileSystem storage and syncs incoming files
2. **Parsers/Cleaners** (Offloc, Delius) transform raw PNOMIS and NDelius files into structured records in staging databases
3. **Import** validates and migrates data from staging to running picture databases
4. **Matching Engine** identifies and links related offender records across systems
5. **Cluster database** maintains grouped offender data
6. **API** exposes the processed data via REST endpoints for downstream consumers (e.g., CATS)
7. **Visualiser** provides a web UI for exploring and visualising relationships between offender data

Supporting services include **DbInteractions** (complex database operations), **Blocking** (matching rules), **Cleanup** (data maintenance), and **Logging**. Services communicate asynchronously via RabbitMQ message queues.

## Queries
Any queries, please contact andrew.grocott@justice.gov.uk or visit our slack channel. https://app.slack.com/client/T02DYEB3A/C011Z8PGWCU/details/

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