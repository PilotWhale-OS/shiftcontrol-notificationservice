# Notification Service

## Brief architecture overview
- Hubs contain logic for a topic
- Hubs have a receiver and hub interface
  - Receiver interfaces define the events that clients can subscribe to
  - Hub interfaces define the methods that clients can call
- DTOs that are used need the TranspilationSource attribute
- Hubs are registered in Program.cs
- Services need to be registered in Program.cs

## Code Generation
Event Classes and Shiftservice clients are generated using codegen tools, packaged in a docker compose for convenience:
```
docker compose -f scripts\scripts.compose.yml up --build
```