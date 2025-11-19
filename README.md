[![Ministry of Justice Repository Compliance Badge](https://github-community.service.justice.gov.uk/repository-standards/api/CFO-ExternalDataIntegrationSystem/badge?1)](https://github-community.service.justice.gov.uk/repository-standards/CFO-ExternalDataIntegrationSystem)

HMPPS CFO DMS
=============

HMPPS Creating Future Opportunities (CFO) - Data Management System (DMS). It is intended for internal use only and is used to process PNOMIS and NDelius offender data to supply CATS (Case Assessment and Tracking System - also used by HMPPS CFO) with accurate offender movements and updates.

# Queries

Any queries, please contact andrew.grocott@justice.gov.uk or visit our slack channel. https://app.slack.com/client/T02DYEB3A/C011Z8PGWCU/details/

# Development Setup and Execution Guide

## Setup (development)
1. To use the Visualiser app, you must configure secret(s) for applications in the *src* directory:
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

### Known issues
1. Due to a [known issue](https://github.com/CommunityToolkit/Aspire/issues/942) in the Aspire Community Toolkit, some SQL projects may start too early.
    - You may need to re-run the project for database changes to apply correctly.
