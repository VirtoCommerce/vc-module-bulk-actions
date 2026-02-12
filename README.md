# Bulk Actions Module

[![CI status](https://github.com/VirtoCommerce/vc-module-bulk-actions/workflows/Module%20CI/badge.svg?branch=dev)](https://github.com/VirtoCommerce/vc-module-bulk-actions/actions?query=workflow%3A"Module+CI") [![Quality gate](https://sonarcloud.io/api/project_badges/measure?project=VirtoCommerce_vc-module-bulk-actions&metric=alert_status&branch=dev)](https://sonarcloud.io/dashboard?id=VirtoCommerce_vc-module-bulk-actions) [![Reliability rating](https://sonarcloud.io/api/project_badges/measure?project=VirtoCommerce_vc-module-bulk-actions&metric=reliability_rating&branch=dev)](https://sonarcloud.io/dashboard?id=VirtoCommerce_vc-module-bulk-actions) [![Security rating](https://sonarcloud.io/api/project_badges/measure?project=VirtoCommerce_vc-module-bulk-actions&metric=security_rating&branch=dev)](https://sonarcloud.io/dashboard?id=VirtoCommerce_vc-module-bulk-actions) [![Sqale rating](https://sonarcloud.io/api/project_badges/measure?project=VirtoCommerce_vc-module-bulk-actions&metric=sqale_rating&branch=dev)](https://sonarcloud.io/dashboard?id=VirtoCommerce_vc-module-bulk-actions)

## Overview

The Bulk Actions Module (BAM) provides a general-purpose, extensible pipeline for executing bulk operations asynchronously within the Virto Commerce platform. Built on the [Template Method pattern](https://en.wikipedia.org/wiki/Template_method_pattern), it offers a framework where other modules can register custom bulk actions with their own data sources, execution logic, and permission requirements. The module handles orchestration, background job execution via Hangfire, progress tracking through push notifications, and cancellation support — allowing consumer modules to focus solely on their domain-specific logic.

## Key Features

* **Extensible Provider Model** — Register custom bulk actions by implementing a small set of interfaces (`IBulkActionFactory`, `IDataSourceFactory`) and adding a `BulkActionProvider` to the storage.
* **Asynchronous Background Execution** — Bulk actions run as Hangfire background jobs, preventing long-running operations from blocking the API.
* **Real-Time Progress Tracking** — Push notifications report processed/total counts, descriptions, and errors to the UI in real time.
* **Job Cancellation** — Running bulk action jobs can be cancelled via the REST API using cancellation tokens.
* **Permission-Based Authorization** — Each registered bulk action provider declares its required permissions, which are checked against the current user before execution.
* **Polymorphic Context Deserialization** — Automatically resolves `BulkActionContext` subtypes via `PolymorphJsonConverter`, enabling strongly-typed action contexts across modules.

## Configuration

### Application Settings

This module does not define any configurable application settings.

### Permissions

| Permission | Description |
|---|---|
| `bulk-actions:read` | View the list of registered bulk actions and retrieve action initialization data |
| `bulk-actions:execute` | Start and cancel bulk action background jobs |

## Architecture

### Key Flow

1. A client sends `POST /api/bulk/actions` with a `BulkActionContext`-derived payload.
2. `BulkActionsController` validates the context, resolves the matching `IBulkActionProvider` from storage by `ActionName`, and checks the user's permissions against the provider's required permissions.
3. The controller creates a `BulkActionPushNotification` and enqueues a `BulkActionJob` via Hangfire.
4. `BulkActionJob.ExecuteAsync` is invoked by Hangfire, which delegates to `BulkActionExecutor.ExecuteAsync` with a progress callback and cancellation token.
5. `BulkActionExecutor` validates the context, creates an `IBulkAction` instance via the provider's `BulkActionFactory`, and runs `ValidateAsync()` on it.
6. The executor creates an `IDataSource` via the provider's `DataSourceFactory` and retrieves the total item count.
7. The executor iterates in a loop: `DataSource.FetchAsync()` retrieves the next batch, `BulkAction.ExecuteAsync()` processes it, and progress is reported via the callback.
8. On each progress callback, `BulkActionPushNotification` is patched with current counts/errors and sent to the UI via `IPushNotificationManager`.
9. Upon completion (or error/cancellation), a final notification is sent with the finished timestamp and summary.

## Components

### Projects

| Project | Layer | Purpose |
|---|---|---|
| VirtoCommerce.BulkActionsModule.Core | Core | Domain models, service interfaces, permission constants |
| VirtoCommerce.BulkActionsModule.Data | Data | Service implementations (executor, provider storage, extensions) |
| VirtoCommerce.BulkActionsModule.Web | Web | API controller, Hangfire background jobs, module initialization |
| VirtoCommerce.BulkActionsModule.Tests | Tests | Unit tests (xUnit v3, Moq, FluentAssertions) |

### Key Services

| Service | Interface | Responsibility |
|---|---|---|
| `BulkActionExecutor` | `IBulkActionExecutor` | Orchestrates bulk action execution: validation, batch iteration, progress reporting, error handling |
| `BulkActionProviderStorage` | `IBulkActionProviderStorage` | Thread-safe registry (ConcurrentDictionary) for managing registered bulk action providers |
| `BackgroundJobExecutor` | `IBackgroundJobExecutor` | Wraps Hangfire's `BackgroundJob.Enqueue` and `BackgroundJob.Delete` for enqueueing and cancelling jobs |
| `BulkActionJob` | _(Hangfire job)_ | Entry point for background execution; bridges Hangfire with `IBulkActionExecutor` and push notifications |

### REST API

Base route: `api/bulk/actions`

| Method | Endpoint | Description |
|---|---|---|
| `GET` | `/api/bulk/actions` | Returns the list of all registered bulk action providers |
| `POST` | `/api/bulk/actions` | Starts a bulk action as a background job; returns a push notification with the job ID |
| `DELETE` | `/api/bulk/actions?jobId={jobId}` | Cancels a running bulk action job by its Hangfire job ID |
| `POST` | `/api/bulk/actions/data` | Retrieves action initialization data for a given context (used for UI setup) |

## Documentation

* [Bulk Actions Module Documentation](https://docs.virtocommerce.org/platform/developer-guide/Modules/VirtoCommerce.BulkActionsModule/)
* [REST API Reference](https://docs.virtocommerce.org/platform/developer-guide/Modules/VirtoCommerce.BulkActionsModule/api/)
* [GitHub Repository](https://github.com/VirtoCommerce/vc-module-bulk-actions)

## References

* [Deployment Guide](https://docs.virtocommerce.org/developer-guide/deploy-module/)
* [Installation Guide](https://docs.virtocommerce.org/user-guide/modules/)
* [Virto Commerce Home](https://virtocommerce.com)
* [Community](https://www.virtocommerce.org)
* [Latest Release](https://github.com/VirtoCommerce/vc-module-bulk-actions/releases/latest)

## License

Copyright (c) Virto Solutions LTD. All rights reserved.

Licensed under the Virto Commerce Open Software License (the "License"); you may not use this file except in compliance with the License. You may obtain a copy of the License at

http://virtocommerce.com/opensourcelicense

Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
