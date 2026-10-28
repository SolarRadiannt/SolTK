using SolTK;

// 1. Custom error handling
Scheduler.OnError = (name, ex) => Console.WriteLine($"[SOLTK ERROR] {name} failed: {ex.Message}");

// 2. Register some systems
Scheduler.Register((delta) => Console.WriteLine($"System A (Priority 0) running. Delta: {delta}"), SPriority.Default);
Scheduler.Register((delta) => Console.WriteLine($"System B (Priority 10) running. Delta: {delta}"), 10);
Scheduler.Register((delta) => throw new Exception("I am broken!"), SPriority.Low); // This will crash and remove itself

// 3. Process
Scheduler.ProcessAll(0.016); // Simulate 60fps
Console.WriteLine("--- Next Frame ---");
Scheduler.ProcessAll(0.016); // The broken system is gone!