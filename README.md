# BoardOutlook Executive Compensation API

![.NET](https://img.shields.io/badge/.NET-8.0-blue)
![C#](https://img.shields.io/badge/C%23-12-blue)


## Overview

**BoardOutlook Executive Compensation API** is a .NET 8 Web API that fetches, processes, and analyzes executive compensation data for companies listed on stock exchanges (e.g., ASX). 

The API identifies executives whose total compensation exceeds the industry average by 10% or more, helping stakeholders analyze executive pay trends and benchmark against industry standards.

---

## Features

- Fetch companies listed on a specific stock exchange.
- Retrieve executives for each company along with compensation details.
- Fetch industry benchmarks (average executive compensation) for each industry.
- Identify executives paid at least 10% above industry average.
- Support **parallel processing** with controlled concurrency using `SemaphoreSlim`.
- Resilient external API calls using **Polly** (Retry, Circuit Breaker, Fallback).
- JSON serialization with **System.Text.Json**.
- Fully asynchronous using **async/await**.

---

## Technology Stack

- **.NET 8 Web API** – RESTful backend framework.
- **C# 12** – Primary programming language.
- **Async/Await** – Non-blocking I/O operations.
- **Concurrent Collections (`ConcurrentBag`)** – Thread-safe result aggregation.
- **SemaphoreSlim** – Concurrency control.
- **Polly** – Retry, Circuit Breaker, Fallback for resilience.
- **System.Text.Json** – JSON serialization/deserialization.
- **xUnit & Moq** – Unit testing and mocking.

---

## Project Structure

BoardOutlook/
├── BoardOutlook.API/ # Exposes REST endpoints
├── BoardOutlook.Application/ # Application layer (services, DTOs, queries)
├── BoardOutlook.Domain/ # Domain models and business rules
├── BoardOutlook.Infrastructure/ # External API integration and repositories
├── BoardOutlook.Tests/ # Unit and integration tests


### GET `/api/companies/executives/compensation`

- **Description**: Retrieves executives whose total compensation is at least 10% above the industry average for companies in the specified exchange.
- **Query Parameters**:
  - `exchange` (required) – Stock exchange code (e.g., `ASX`)
- **Responses**:
  - `200 OK` – Returns a list of executives above threshold.
  - `400 Bad Request` – If exchange parameter is missing.

---

## Business Flow

1. **Fetch Companies** – Retrieve all companies listed on a stock exchange.
2. **Fetch Executives** – Retrieve executives and compensation for each company.
3. **Fetch Industry Benchmark** – Get average compensation for each executive’s industry.
4. **Filter Executives** – Identify executives with total compensation ≥ 10% above industry benchmark.
5. **Return Results** – Provide a consolidated list in JSON format.

---

## Error Handling & Resilience

- **Polly Policies**:
  - **Retry** – Retries transient errors (5xx, 408, etc.) with exponential backoff.
  - **Circuit Breaker** – Stops repeated failing calls temporarily after consecutive failures.
  - **Fallback** – Returns safe default responses when all retries fail.
- **Cancellation Support** – All async calls support `CancellationToken`.
- **Logging** – Uses `ILogger` to log warnings, errors, and debug info.
