[![Ministry of Justice Repository Compliance Badge](https://github-community.service.justice.gov.uk/repository-standards/api/CFO-ExternalDataIntegrationSystem/badge?1)](https://github-community.service.justice.gov.uk/repository-standards/CFO-ExternalDataIntegrationSystem)

HMPPS CFO DMS
=============
[![Run Tests](https://github.com/ministryofjustice/CFO-ExternalDataIntegrationSystem/actions/workflows/run-tests.yml/badge.svg)](https://github.com/ministryofjustice/CFO-ExternalDataIntegrationSystem/actions/workflows/run-tests.yml)
[![.NET](https://img.shields.io/badge/.NET-10.0-512BD4)](https://dotnet.microsoft.com/)
[![Issues](https://img.shields.io/github/issues/ministryofjustice/CFO-ExternalDataIntegrationSystem)](https://github.com/ministryofjustice/CFO-ExternalDataIntegrationSystem/issues)
[![Pull Requests](https://img.shields.io/github/issues-pr/ministryofjustice/CFO-ExternalDataIntegrationSystem)](https://github.com/ministryofjustice/CFO-ExternalDataIntegrationSystem/pulls)
## Overview
HMPPS Creating Future Opportunities (CFO) - Data Management System (DMS). It is intended for internal use only and is used to process PNOMIS and NDelius offender data to supply CATS (Case Assessment and Tracking System - also used by HMPPS CFO) with accurate offender movements and updates.

## Queries
Any queries, please contact andrew.grocott@justice.gov.uk or visit our slack channel. https://app.slack.com/client/T02DYEB3A/C011Z8PGWCU/details/

# Development Setup and Execution Guide

## Prerequisites
- [.NET 10 SDK](https://dotnet.microsoft.com/download/dotnet/10.0)
- **Visual Studio Code users**: Install the [C# Dev Kit extension](https://marketplace.visualstudio.com/items?itemName=ms-dotnettools.csdevkit)

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

## Services and Credentials
When running via Aspire, the following services are available:

### API
Provides REST endpoints for querying offender data, performing searches, and managing clustering operations.
- **HTTPS**: `https://localhost:7013`
- **API Key**: `password`

### MinIO (S3 Storage)
Used for file synchronization and storage.
- **Console**: Randomly assigned port (check Aspire dashboard)
- **Username**: `minioadmin`
- **Password**: `minioadmin`

### MSSQL Server
Hosts all application databases including staging, running picture, matching, and cluster databases.
- **Host**: `127.0.0.1,61749`
- **Username**: `sa`
- **Password**: `P@ssword123!`

### RabbitMQ
Handles messaging between services.
- **Management Console**: `http://localhost:15672`
- **Username**: `guest`
- **Password**: `guest`
