---
layout: default
title: Real-World Scenarios
parent: Examples
nav_order: 3
---

# Real-World Scenarios

This guide demonstrates how Dumpify fits into practical development workflows and solves common debugging challenges.

## Table of Contents

- [Debugging API Responses](#debugging-api-responses)
- [Entity Framework Debugging](#entity-framework-debugging)
- [Logging and Diagnostics](#logging-and-diagnostics)
- [Unit Test Debugging](#unit-test-debugging)
- [Console Application Development](#console-application-development)
- [Configuration Inspection](#configuration-inspection)
- [Data Transformation Pipelines](#data-transformation-pipelines)
- [Error Investigation](#error-investigation)

## Debugging API Responses

### Inspecting HTTP Client Responses

```csharp
public async Task<User> GetUserAsync(int userId)
{
    var response = await _httpClient.GetAsync($"/api/users/{userId}");
    response.Dump("HTTP Response");
    
    var content = await response.Content.ReadAsStringAsync();
    content.Dump("Raw JSON");
    
    var user = JsonSerializer.Deserialize<User>(content);
    user.Dump("Deserialized User");
    
    return user;
}
```

### Debugging Minimal API Endpoints

```csharp
app.MapGet("/api/orders/{id}", async (int id, OrderService service) =>
{
    var order = await service.GetOrderAsync(id);
    order.Dump($"Order {id}"); // Debug output
    
    return order is null 
        ? Results.NotFound() 
        : Results.Ok(order);
});
```

### Examining Request Data

```csharp
app.MapPost("/api/orders", async (CreateOrderRequest request, OrderService service) =>
{
    request.Dump("Incoming Order Request");
    
    var validationResult = ValidateRequest(request);
    validationResult.Dump("Validation Result");
    
    if (!validationResult.IsValid)
        return Results.BadRequest(validationResult.Errors);
    
    var order = await service.CreateOrderAsync(request);
    order.Dump("Created Order");
    
    return Results.Created($"/api/orders/{order.Id}", order);
});
```

## Entity Framework Debugging

### Inspecting Query Results

```csharp
public async Task<List<Order>> GetOrdersWithDetailsAsync(int customerId)
{
    var orders = await _context.Orders
        .Where(o => o.CustomerId == customerId)
        .Include(o => o.OrderItems)
            .ThenInclude(oi => oi.Product)
        .ToListAsync();
    
    orders.Dump("Orders with Details");
    
    return orders;
}
```

### Debugging Navigation Properties

```csharp
// Check what's loaded vs. null
var customer = await _context.Customers
    .Include(c => c.Address)
    .FirstOrDefaultAsync(c => c.Id == customerId);

customer.Dump(config => config
    .SetMaxDepth(3)
    .UseMembersConfig(m => m.IncludeNonPublicMembers(false)));
```

### Tracking Entity State

```csharp
public async Task UpdateOrderAsync(Order order)
{
    _context.Entry(order).State.Dump("Entity State Before");
    
    _context.Orders.Update(order);
    
    var changes = _context.ChangeTracker.Entries()
        .Where(e => e.State != EntityState.Unchanged)
        .Select(e => new 
        { 
            Entity = e.Entity.GetType().Name, 
            State = e.State,
            Original = e.OriginalValues.ToObject(),
            Current = e.CurrentValues.ToObject()
        })
        .ToList();
    
    changes.Dump("Pending Changes");
    
    await _context.SaveChangesAsync();
}
```

### Hiding Sensitive EF Properties

```csharp
// Filter out EF Core internal properties
var config = new DumpConfig
{
    MembersConfig = new MembersConfig
    {
        MemberFilter = member => 
            !member.Name.StartsWith("__") &&
            member.Name != "LazyLoader"
    }
};

dbEntity.Dump(config);
```

## Logging and Diagnostics

### Structured Logging Integration

```csharp
public class OrderProcessor
{
    private readonly ILogger<OrderProcessor> _logger;
    
    public async Task ProcessOrderAsync(Order order)
    {
        // Get string representation for logging
        var orderDump = order.DumpText();
        _logger.LogInformation("Processing order: {OrderDetails}", orderDump);
        
        try
        {
            await ProcessInternalAsync(order);
            _logger.LogInformation("Order {OrderId} processed successfully", order.Id);
        }
        catch (Exception ex)
        {
            var context = new { Order = order, Timestamp = DateTime.UtcNow };
            _logger.LogError(ex, "Failed to process order: {Context}", 
                context.DumpText());
            throw;
        }
    }
}
```

### Debug vs Release Output

```csharp
public class DiagnosticService
{
    [Conditional("DEBUG")]
    public void DumpState<T>(T obj, string label)
    {
        obj.Dump(label);
    }
    
    public void ProcessWithDiagnostics(Request request)
    {
        DumpState(request, "Input Request");
        
        var result = Process(request);
        
        DumpState(result, "Processing Result");
    }
}
```

### Application Startup Diagnostics

```csharp
var builder = WebApplication.CreateBuilder(args);

// Dump configuration during startup
#if DEBUG
builder.Configuration.AsEnumerable()
    .Where(kvp => !kvp.Key.Contains("Password", StringComparison.OrdinalIgnoreCase))
    .ToDictionary(kvp => kvp.Key, kvp => kvp.Value)
    .Dump("Application Configuration");
#endif

var app = builder.Build();
```

## Unit Test Debugging

### Test Assertion Debugging

```csharp
[Fact]
public void ProcessOrder_ShouldCalculateTotal_Correctly()
{
    // Arrange
    var order = new Order
    {
        Items = new List<OrderItem>
        {
            new() { ProductId = 1, Quantity = 2, UnitPrice = 10.00m },
            new() { ProductId = 2, Quantity = 1, UnitPrice = 25.00m }
        }
    };
    order.Dump("Test Input");
    
    // Act
    var processor = new OrderProcessor();
    var result = processor.CalculateTotal(order);
    result.Dump("Calculated Result");
    
    // Assert
    Assert.Equal(45.00m, result.Total);
}
```

### Debugging Collection Assertions

```csharp
[Fact]
public void GetActiveUsers_ShouldFilterCorrectly()
{
    // Arrange
    var users = GetTestUsers();
    users.Dump("All Users");
    
    // Act
    var activeUsers = _service.GetActiveUsers(users);
    activeUsers.Dump("Active Users");
    
    // Debug: Show which ones were filtered
    var filteredOut = users.Except(activeUsers);
    filteredOut.Dump("Filtered Out Users");
    
    // Assert
    Assert.All(activeUsers, u => Assert.True(u.IsActive));
}
```

### Debugging Test Data

```csharp
[Theory]
[MemberData(nameof(GetTestCases))]
public void Calculator_Add_ReturnsExpectedResult(int a, int b, int expected)
{
    var testCase = new { a, b, expected };
    testCase.Dump($"Test Case: {a} + {b}");
    
    var result = Calculator.Add(a, b);
    result.Dump("Result");
    
    Assert.Equal(expected, result);
}
```

## Console Application Development

### Interactive Menu Display

```csharp
class Program
{
    static async Task Main(string[] args)
    {
        while (true)
        {
            Console.Clear();
            ShowMenu();
            
            var choice = Console.ReadLine();
            
            switch (choice)
            {
                case "1":
                    var users = await GetUsersAsync();
                    users.Dump("All Users");
                    break;
                    
                case "2":
                    Console.Write("Enter user ID: ");
                    var id = int.Parse(Console.ReadLine()!);
                    var user = await GetUserAsync(id);
                    user.Dump($"User #{id}");
                    break;
                    
                case "3":
                    var stats = await GetStatisticsAsync();
                    stats.Dump("System Statistics");
                    break;
                    
                case "q":
                    return;
            }
            
            Console.WriteLine("\nPress any key to continue...");
            Console.ReadKey();
        }
    }
}
```

### CLI Tool Development

```csharp
// dotnet run -- analyze --path ./data
var rootCommand = new RootCommand("Data Analysis Tool");

var analyzeCommand = new Command("analyze", "Analyze data files");
var pathOption = new Option<string>("--path", "Path to data directory");
analyzeCommand.AddOption(pathOption);

analyzeCommand.SetHandler(async (string path) =>
{
    Console.WriteLine($"Analyzing: {path}");
    
    var results = await AnalyzeDataAsync(path);
    
    results.Summary.Dump("Analysis Summary");
    results.Issues.Dump("Found Issues");
    results.Recommendations.Dump("Recommendations");
    
}, pathOption);

rootCommand.AddCommand(analyzeCommand);
await rootCommand.InvokeAsync(args);
```

## Configuration Inspection

### Options Pattern Debugging

```csharp
services.Configure<MyOptions>(configuration.GetSection("MyOptions"));

services.AddSingleton(sp =>
{
    var options = sp.GetRequiredService<IOptions<MyOptions>>().Value;
    options.Dump("Loaded MyOptions");
    return options;
});
```

### Feature Flag Inspection

```csharp
public class FeatureFlagService
{
    private readonly IFeatureManager _featureManager;
    
    public async Task DumpFeatureFlagsAsync()
    {
        var flags = new Dictionary<string, bool>();
        
        await foreach (var name in _featureManager.GetFeatureNamesAsync())
        {
            flags[name] = await _featureManager.IsEnabledAsync(name);
        }
        
        flags.Dump("Feature Flags");
    }
}
```

## Data Transformation Pipelines

### ETL Pipeline Debugging

```csharp
public async Task<ProcessingResult> ProcessDataAsync(DataSource source)
{
    // Extract
    var rawData = await ExtractAsync(source);
    rawData.Take(5).Dump("Extracted (sample)");
    
    // Transform
    var transformed = Transform(rawData);
    transformed.Take(5).Dump("Transformed (sample)");
    
    // Validate
    var (valid, invalid) = Partition(transformed, IsValid);
    valid.Count().Dump("Valid Records");
    invalid.Take(3).Dump("Invalid Records (sample)");
    
    // Load
    var result = await LoadAsync(valid);
    result.Dump("Load Result");
    
    return result;
}
```

### LINQ Pipeline Debugging

```csharp
var result = orders
    .Dump("1. All Orders")
    .Where(o => o.Status == OrderStatus.Completed)
    .Dump("2. Completed Orders")
    .GroupBy(o => o.CustomerId)
    .Dump("3. Grouped by Customer")
    .Select(g => new 
    { 
        CustomerId = g.Key, 
        TotalOrders = g.Count(),
        TotalValue = g.Sum(o => o.Total)
    })
    .Dump("4. Customer Summaries")
    .OrderByDescending(x => x.TotalValue)
    .Dump("5. Sorted by Value")
    .Take(10)
    .Dump("6. Top 10 Customers")
    .ToList();
```

## Error Investigation

### Exception Context Capture

```csharp
public async Task<Result> ProcessAsync(Request request)
{
    try
    {
        return await ProcessInternalAsync(request);
    }
    catch (Exception ex)
    {
        var errorContext = new
        {
            Request = request,
            Timestamp = DateTime.UtcNow,
            Exception = new
            {
                ex.Message,
                ex.StackTrace,
                InnerMessage = ex.InnerException?.Message
            },
            Environment = new
            {
                MachineName = Environment.MachineName,
                OSVersion = Environment.OSVersion.ToString(),
                ProcessId = Environment.ProcessId
            }
        };
        
        errorContext.Dump("Error Context");
        
        // Log for production
        var contextText = errorContext.DumpText();
        _logger.LogError(ex, "Processing failed: {Context}", contextText);
        
        throw;
    }
}
```

### State Comparison

```csharp
public void InvestigateBug()
{
    var expected = LoadExpectedState();
    var actual = LoadActualState();
    
    expected.Dump("Expected State");
    actual.Dump("Actual State");
    
    // Find differences
    var differences = CompareStates(expected, actual);
    differences.Dump("State Differences");
}
```

### Memory/Performance Debugging

```csharp
public void AnalyzePerformance()
{
    var metrics = new
    {
        TotalMemory = GC.GetTotalMemory(false),
        Gen0Collections = GC.CollectionCount(0),
        Gen1Collections = GC.CollectionCount(1),
        Gen2Collections = GC.CollectionCount(2),
        ThreadCount = Process.GetCurrentProcess().Threads.Count,
        HandleCount = Process.GetCurrentProcess().HandleCount,
        WorkingSet = Process.GetCurrentProcess().WorkingSet64
    };
    
    metrics.Dump("Performance Metrics");
}
```

## See Also

- [Basic Usage](basic-usage.md) - Fundamental examples
- [Advanced Usage](advanced-usage.md) - Complex configurations
- [Output Targets](../features/output-targets.md) - Different output destinations
- [Configuration](../configuration/index.md) - All options
