# Code Challenge for Senior Backend Engineer (Job4435)

## Code Challenge

### Title: Build a Scalable Event-Driven Backend Service with .NET Core and Kafka

### Description:

You are tasked with building a simplified backend microservice that processes user activity events asynchronously using an event-driven architecture. The service should:

1.  Expose a REST API endpoint to receive user activity events (e.g., user login, logout, purchase).
2.  Publish these events asynchronously to an Apache Kafka topic.
3.  Implement a Kafka consumer within the same service that listens to the topic and processes the events by logging them or storing them in an in-memory data structure.
4.  Provide another REST API endpoint to query the processed events (e.g., return all events processed so far).

### Requirements:

*   Use **.NET Core (preferably .NET 6 or later)** and **C# **.
*   Use asynchronous programming patterns (async/await).
*   Use a Kafka client library for .NET (e.g., [Confluent.Kafka](https://github.com/confluentinc/confluent-kafka-dotnet)).
*   The Kafka producer and consumer should be part of the same service for simplicity.
*   The service should be designed with scalability and maintainability in mind (e.g., use dependency injection, clean architecture principles).
*   Include basic error handling and logging.
*   The in-memory store can be a simple thread-safe collection.
*   The REST API should be minimal but follow RESTful conventions.

### Constraints:

*   The challenge should be solvable within 30 minutes.
*   Focus on demonstrating understanding of event-driven architecture, Kafka integration, and .NET Core backend development.
*   No need to implement persistence beyond in-memory storage.
*   No need to deploy on AWS or use Terraform for this challenge (those are infrastructure skills better assessed separately).

## Technologies Relevant to the Challenge

*   .NET Core (6 or later)
*   C#
*   Apache Kafka (using Confluent.Kafka .NET client)
*   REST API development with ASP.NET Core Web API
*   Asynchronous programming (async/await)
*   Dependency Injection (built-in .NET Core DI)
*   Thread-safe collections (e.g., `ConcurrentQueue` or `ConcurrentBag`)
*   Logging (e.g., Microsoft.Extensions.Logging)

## External Resources

*   **Apache Kafka:**\
    Use a free public Kafka cluster for testing or run a local Kafka instance via Docker.

    *   Public Kafka clusters (free tier):

        *   [Confluent Cloud Free Tier](https://www.confluent.io/confluent-cloud/free/) — offers a free Kafka cluster with limited usage.
        *   [Redpanda Cloud Free Tier](https://redpanda.com/cloud) — another managed Kafka-compatible service with a free tier.

    *   Alternatively, candidates can run Kafka locally using Docker:

        *   [Kafka Docker Quickstart](https://docs.confluent.io/platform/current/quickstart/ce-docker-quickstart.html)

*   **Confluent.Kafka .NET Client:**

    *   NuGet package: <https://www.nuget.org/packages/Confluent.Kafka/>
    *   GitHub: <https://github.com/confluentinc/confluent-kafka-dotnet>

##