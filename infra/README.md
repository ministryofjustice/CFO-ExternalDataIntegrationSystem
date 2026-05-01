# DMS Infrastructure - AWS Fargate

This directory contains AWS Fargate ECS task definitions for the DMS (Data Management System).

## Contents

## Structure

```
infra/
├── services.json              # Service deployment configuration
├── README.md                  # This file
└── task-definitions/          # ECS task definition files
    ├── api-task-def.json
    ├── blocking-task-def.json
    └── ...
```

### Service Mappings
`services.json` - Central configuration mapping services to projects and task definitions.
This file is used by the CI/CD pipeline to automatically deploy all services.

**Adding a new service:**
1. Create the task definition JSON file in `task-definitions/` (e.g., `my-service-task-def.json`)
2. Add an entry to `services.json`:
```json
{
  "name": "my-service",
  "project": "MyService/MyService.csproj",
  "taskDef": "infra/task-definitions/my-service-task-def.json",
  "container": "my-service-container"
}
```
3. Push to `develop` or `main` - deployment is automatic!

### Task Definitions
Each service has its own ECS Fargate task definition JSON file in `task-definitions/`:
- `api-task-def.json` - REST API service (512 CPU, 1024 MB)
- `blocking-task-def.json` - Blocking service (512 CPU, 1024 MB)
- `cleanup-task-def.json` - Cleanup service (256 CPU, 512 MB)
- `delius-parser-task-def.json` - Delius data parser (512 CPU, 1024 MB)
- `filesync-task-def.json` - File synchronization service (512 CPU, 1024 MB)
- `import-task-def.json` - Data import service (512 CPU, 1024 MB)
- `logging-task-def.json` - Logging aggregation (256 CPU, 512 MB)
- `matching-engine-task-def.json` - Matching engine (1024 CPU, 2048 MB)
- `meow-task-def.json` - Meow service (512 CPU, 1024 MB)
- `offloc-cleaner-task-def.json` - Offloc cleaner (512 CPU, 1024 MB)
- `offloc-parser-task-def.json` - Offloc parser (512 CPU, 1024 MB)
- `visualiser-task-def.json` - Visualiser UI (256 CPU, 512 MB)

## Prerequisites

1. AWS CLI configured with appropriate credentials
2. .NET 8+ SDK installed
3. ECR repositories created for each service
4. VPC with public and private subnets
5. RDS databases (or equivalent) for data storage
6. RabbitMQ or Amazon MQ for message queuing

## Building and Publishing Containers

### Using .NET SDK Container Publishing (Recommended - No Dockerfiles needed!)

.NET 8+ has built-in container support. Build and push directly to ECR using CLI arguments:

```bash
# Login to ECR
aws ecr get-login-password --region eu-west-2 | \
  docker login --username AWS --password-stdin {account-id}.dkr.ecr.eu-west-2.amazonaws.com

# Build and publish - pass container settings as CLI arguments
dotnet publish src/API/API.csproj \
  --configuration Release \
  --target:PublishContainer \
  --property:ContainerRegistry={account-id}.dkr.ecr.eu-west-2.amazonaws.com \
  --property:ContainerRepository=preprod/api \
  --property:ContainerImageTag=abc123

dotnet publish src/Visualiser/Visualiser.csproj \
  --configuration Release \
  --target:PublishContainer \
  --property:ContainerRegistry={account-id}.dkr.ecr.eu-west-2.amazonaws.com \
  --property:ContainerRepository=prod/visualiser \
  --property:ContainerImageTag=def456

# Repeat for other services: Blocking, Cleanup, Delius.Parser, FileSync, Import, 
# Logging, Matching.Engine, Meow, Offloc.Cleaner, Offloc.Parser
```

**Note**: 
- Dockerfiles are not required when using SDK container publishing
- Pass configuration via CLI args (like the CI/CD pipeline does) rather than hardcoding in .csproj
- Use environment-specific repository names: `preprod/{service}` or `prod/{service}`

## Configuration Steps

### 1. Configure Secrets and Environment Variables

Each service requires configuration for:
- Database connection strings (via AWS Secrets Manager)
- RabbitMQ connection details
- S3 bucket names
- Sentry DSN (optional)

Add these to the `secrets` array in task definitions:
```json
"secrets": [
  {
    "name": "ConnectionStrings__ClusterDb",
    "valueFrom": "arn:aws:secretsmanager:eu-west-2:{account-id}:secret:dms/cluster-db"
  }
]
```

**Note**: Container images are automatically set by the CI/CD pipeline using the `PLACEHOLDER` value. The deployment workflow dynamically injects the correct image URIs at deploy time.

## Registering Task Definitions

**Note**: Task definitions are automatically registered by the CI/CD pipeline during deployment.

For manual registration (if needed):
```bash
aws ecs register-task-definition --cli-input-json file://task-definitions/api-task-def.json
aws ecs register-task-definition --cli-input-json file://task-definitions/blocking-task-def.json
# ... repeat for each service
```

## Create ECS Services

After registering task definitions, create services:
```bash
aws ecs create-service \
  --cluster dms-cluster-dev \
  --service-name dms-api \
  --task-definition dms-api-task \
  --desired-count 2 \
  --launch-type FARGATE \
  --network-configuration "awsvpcConfiguration={subnets=[subnet-xxxxx],securityGroups=[sg-xxxxx],assignPublicIp=DISABLED}" \
  --load-balancers "targetGroupArn=arn:aws:elasticloadbalancing:...,containerName=api-container,containerPort=8080"
```

## Service Architecture

### Public Services (behind ALB)
- **API** - Main REST API endpoint
- **Visualiser** - Web UI for data visualization

### Internal Services (no load balancer)
- **Blocking** - Handles blocking operations
- **Cleanup** - Data cleanup tasks
- **Delius Parser** - Parses Delius data files
- **FileSync** - Syncs files to/from S3
- **Import** - Imports data from external sources
- **Logging** - Aggregates and processes logs
- **Matching Engine** - Performs data matching operations
- **Meow** - Message processing service
- **Offloc Cleaner** - Cleans Offloc data
- **Offloc Parser** - Parses Offloc data files

## Monitoring

All services log to CloudWatch Logs:
- Log group format: `/ecs/dms-{service-name}`
- Retention: 30 days (configurable in Terraform)
- Container Insights enabled for metrics
