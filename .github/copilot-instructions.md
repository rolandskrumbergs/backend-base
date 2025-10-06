# Project Architecture and Guidelines

This project follows a clean architecture approach with Domain-Driven Design (DDD) principles. The solution is structured into the following key projects:

## Project Structure

### Domain Layer 
- Contains domain entities, value objects, interfaces, and business logic
- Must not depend on any other project
- Follows DDD patterns:
  - Entities inherit from IAggregateRoot interface
  - Value Objects inherit from ValueObject base class
  - Rich domain models with encapsulated business logic

### Core Layer 
- Contains application business logic and use cases
- Depends only on Domain project
- Uses CQRS pattern with MediatR:
  - Commands: modify data, return Result<T>
  - Queries: retrieve data, return Result<T>
- Use cases are organized in feature folders:
  ```
  Features/
    FeatureName/
      UseCases/
        CommandName/
          Command.cs
          CommandHandler.cs
          CommandValidator.cs
        QueryName/
          Query.cs
          QueryHandler.cs
          QueryValidator.cs
      Models/
      Specifications/
  ```

### Infrastructure Layer 
- Contains external concerns and implementations
- Depends on Core project
- Implements repository interfaces
- Handles database access, external services, etc.

### API Layer
- Contains API endpoints and controllers
- Depends on Core project
- Endpoints are organized by feature in the Endpoints folder
- Endpoint request and response models are in the same file as the endpoint
- Endpoint requests have only [FromRoute] and [FromBody] attributes

## Key Technologies and Patterns

### CQRS Implementation
- Commands and Queries use mediator pattern
- Handlers implement IRequestHandler<TRequest, Result<T>>

### Validation
- Use FluentValidation for command/query validation
- Validators are in the same folder as the command/query
- Return Result.Invalid with validation errors

### Repository Pattern
- Repositories implement IRepository<T> or IReadRepository<T>
- Entities must implement IAggregateRoot

### Value Objects
- Inherit from ValueObject base class
- Override GetEqualityComponents()
- Immutable with private setters
- Used for complex value types

### Testing
- Use NUnit for unit testing
- Use Moq for mocking
- Test projects mirror the main project structure
- Repository mocks available in TestDoubles project

## Code Organization

### File Naming and Location
- One class per file
- File name matches class name
- Namespace matches folder structure
- Place related files in feature folders

### Code Style
- Use C# 12 features including file-scoped namespaces
- Use primary constructor syntax where appropriate
- Make properties private set where possible
- Use Result pattern for operation results

### Domain Model Guidelines
- Use strong typing over primitive types
- Encapsulate business logic in domain entities
- Use value objects for complex values
- Make invalid states unrepresentable

### Best Practices
- Follow SOLID principles
- Use dependency injection
- Write unit tests for business logic
- Use specifications for complex queries
- Keep handlers focused and single-purpose
- Validate commands and queries
- Return Result types for operations
