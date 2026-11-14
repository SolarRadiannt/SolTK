# Solar's ToolKit (SolTK)

**Status:** In Progress (ongoing personal toolkit)

**Stack:** C# (.NET)

## Overview
Mono-repo for my personal toolkits. This toolkit is utility-first and designed to be very generic, so it is transferable to almost any project.

## C# (`/csharp`)
Currently the only supported language. Includes optional framework-like behavior via `IoCManager`.

### Current Tools
| Tool | Purpose |
|------|---------|
| `IoCManager` | Lightweight IoC container with `IService` registration and `[Dependency]` field and property injection. Also holds `Resolve<T>()` to fetch a service without dependency and `Register<T>()` to manually register and inject dependencies |
| `Scheduler` | Static priority-based scheduler with separate Regular and Simulation action queues. |
| `ECS` | Lazy-initialized static World with auto-despawn system for `MarkDespawn` entities. |
| `ETransient` | Transient entity tag + template with auto-despawn scheduling. |
| `EEvent` | Event entity tag + template for decoupled events. |
| `CentralClock` | Static clock with separate `RealNow` and `SimNow` with optional TimeScale for how much a tick contribute to the Simulation time; requires manual ticks. |
| `EventBus` | Static centralized event bus with type-based `Subscribe`/`Raise` for global events and entity-scoped `SubscribeLocal`/`RaiseLocal` for local events. |

### Extensibility
**Error Handling:** `IoCManager` and `Scheduler` expose a static `OnError` hook (`Action<string, Exception>`) that defaults to `Console.WriteLine`. This can be overridden anywhere to route errors to a specific engine (e.g., Godot's `GD.PushError` or Unity's `Debug.LogError`) or a custom logging framework.

## Future Plans
Add more language toolkits as needed.
