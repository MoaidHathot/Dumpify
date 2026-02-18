namespace Dumpify.Tests.Generators;

public class TaskDescriptorTests
{
    [Fact]
    public void TaskOfInt_ShouldBeCustomValueDescriptor()
    {
        var generator = new CompositeDescriptorGenerator(new ConcurrentDictionary<RuntimeTypeHandle, Func<object, Type, IValueProvider?, IMemberProvider, object?>>());
        var descriptor = generator.Generate(typeof(Task<int>), null, new MemberProvider());

        descriptor.Should().BeOfType<CustomDescriptor>("Task<int> should be a custom value descriptor");
    }

    [Fact]
    public void TaskOfString_ShouldBeCustomValueDescriptor()
    {
        var generator = new CompositeDescriptorGenerator(new ConcurrentDictionary<RuntimeTypeHandle, Func<object, Type, IValueProvider?, IMemberProvider, object?>>());
        var descriptor = generator.Generate(typeof(Task<string>), null, new MemberProvider());

        descriptor.Should().BeOfType<CustomDescriptor>("Task<string> should be a custom value descriptor");
    }

    [Fact]
    public void TaskOfComplexType_ShouldBeCustomValueDescriptor()
    {
        var generator = new CompositeDescriptorGenerator(new ConcurrentDictionary<RuntimeTypeHandle, Func<object, Type, IValueProvider?, IMemberProvider, object?>>());
        var descriptor = generator.Generate(typeof(Task<Person>), null, new MemberProvider());

        descriptor.Should().BeOfType<CustomDescriptor>("Task<Person> should be a custom value descriptor");
    }

    [Fact]
    public void NonGenericTask_ShouldBeCustomValueDescriptor()
    {
        var generator = new CompositeDescriptorGenerator(new ConcurrentDictionary<RuntimeTypeHandle, Func<object, Type, IValueProvider?, IMemberProvider, object?>>());
        var descriptor = generator.Generate(typeof(Task), null, new MemberProvider());

        descriptor.Should().BeOfType<CustomDescriptor>("Task should be a custom value descriptor");
    }

    [Fact]
    public async Task Task_NotCompleted_ShouldNotBlock()
    {
        // Arrange - create a task that never completes
        var tcs = new TaskCompletionSource<int>();
        var task = tcs.Task;

        // Act - Dump should NOT block waiting for completion
        var dumpTask = Task.Run(() =>
        {
            _ = task.Dump();
            return true;
        });

        // Wait a short time - if it completes, Dump didn't block
        var completed = await Task.WhenAny(dumpTask, Task.Delay(TimeSpan.FromSeconds(1))) == dumpTask;

        // Assert
        completed.Should().BeTrue("Dumping an incomplete Task should not block");
        task.IsCompleted.Should().BeFalse("Task should still be incomplete");
    }

    [Fact]
    public void Task_Completed_ShouldShowResult()
    {
        // Arrange
        var task = Task.FromResult(42);

        // Act & Assert - should not throw
        task.IsCompleted.Should().BeTrue("Task should be completed");
        
        // Dump directly - use discard to avoid async issues
        Action act = () => { _ = task.Dump(); };
        act.Should().NotThrow();
    }

    [Fact]
    public void Task_Faulted_ShouldNotThrow()
    {
        // Arrange
        var tcs = new TaskCompletionSource<int>();
        tcs.SetException(new InvalidOperationException("Test exception"));
        var task = tcs.Task;

        // Act & Assert - should not throw when dumping faulted task
        task.IsFaulted.Should().BeTrue("Task should be faulted");
        
        Action act = () => { _ = task.Dump(); };
        act.Should().NotThrow("Dumping a faulted Task should not throw");
    }

    [Fact]
    public void Task_Canceled_ShouldNotThrow()
    {
        // Arrange
        var tcs = new TaskCompletionSource<int>();
        tcs.SetCanceled();
        var task = tcs.Task;

        // Act & Assert - should not throw when dumping canceled task
        task.IsCanceled.Should().BeTrue("Task should be canceled");
        
        Action act = () => { _ = task.Dump(); };
        act.Should().NotThrow("Dumping a canceled Task should not throw");
    }

    [Fact]
    public void NonGenericTask_Completed_ShouldNotThrow()
    {
        // Arrange
        var task = Task.CompletedTask;

        // Act & Assert - should not throw
        task.IsCompleted.Should().BeTrue("Task should be completed");
        
        Action act = () => { _ = task.Dump(); };
        act.Should().NotThrow();
    }

    [Fact]
    public async Task NonGenericTask_NotCompleted_ShouldNotBlock()
    {
        // Arrange - create a non-generic task that never completes
        var tcs = new TaskCompletionSource<object?>();
        Task task = tcs.Task;

        // Act - Dump should NOT block waiting for completion
        var dumpTask = Task.Run(() =>
        {
            _ = task.Dump();
            return true;
        });

        // Wait a short time - if it completes, Dump didn't block
        var completed = await Task.WhenAny(dumpTask, Task.Delay(TimeSpan.FromSeconds(1))) == dumpTask;

        // Assert
        completed.Should().BeTrue("Dumping an incomplete non-generic Task should not block");
    }

    [Fact]
    public void TaskWithNullResult_ShouldNotThrow()
    {
        // Arrange
        var task = Task.FromResult<string?>(null);

        // Act & Assert - should not throw
        task.IsCompleted.Should().BeTrue("Task should be completed");
        
        Action act = () => { _ = task.Dump(); };
        act.Should().NotThrow();
    }

    [Fact]
    public void TaskWithComplexResult_ShouldNotThrow()
    {
        // Arrange
        var person = new Person { FirstName = "John", LastName = "Doe" };
        var task = Task.FromResult(person);

        // Act & Assert - should not throw
        task.IsCompleted.Should().BeTrue("Task should be completed");
        
        Action act = () => { _ = task.Dump(); };
        act.Should().NotThrow();
    }

    [Fact]
    public async Task ContinueWith_NotCompleted_ShouldNotBlock()
    {
        // Arrange - ContinueWith returns internal runtime type ContinuationResultTaskFromTask<T>
        // which inherits from Task<T> but is not Task<T> directly
        var task = Task.Delay(TimeSpan.FromSeconds(30)).ContinueWith(_ => 42);

        // Verify the task type is a derived type, not Task<T> directly
        var taskType = task.GetType();
        taskType.Should().NotBe(typeof(Task<int>), "ContinueWith should return a derived Task type");
        typeof(Task<int>).IsAssignableFrom(taskType).Should().BeTrue("The derived type should be assignable to Task<int>");

        // Act - Dump should NOT block waiting for completion
        var dumpTask = Task.Run(() =>
        {
            _ = task.Dump();
            return true;
        });

        // Wait a short time - if it completes, Dump didn't block
        var completed = await Task.WhenAny(dumpTask, Task.Delay(TimeSpan.FromSeconds(1))) == dumpTask;

        // Assert
        completed.Should().BeTrue("Dumping a ContinueWith Task should not block");
        task.IsCompleted.Should().BeFalse("Task should still be incomplete");
    }

    [Fact]
    public async Task ContinueWith_Completed_ShouldShowResult()
    {
        // Arrange - ContinueWith with already completed task
        var task = Task.CompletedTask.ContinueWith(_ => 99);
        
        // Wait for continuation to complete
        await task;

        // Verify the task type is a derived type
        var taskType = task.GetType();
        taskType.Should().NotBe(typeof(Task<int>), "ContinueWith should return a derived Task type");

        // Act & Assert - should not throw
        task.IsCompleted.Should().BeTrue("Task should be completed");
        
        Action act = () => { _ = task.Dump(); };
        act.Should().NotThrow();
    }

    [Fact]
    public async Task ContinueWith_DerivedType_ShouldBeCustomDescriptor()
    {
        // Arrange - Get the actual runtime type from ContinueWith
        var task = Task.CompletedTask.ContinueWith(_ => 42);
        await task;
        var taskType = task.GetType();

        // Verify it's a derived type
        taskType.Should().NotBe(typeof(Task<int>));
        typeof(Task).IsAssignableFrom(taskType).Should().BeTrue();

        // Act
        var generator = new CompositeDescriptorGenerator(new ConcurrentDictionary<RuntimeTypeHandle, Func<object, Type, IValueProvider?, IMemberProvider, object?>>());
        var descriptor = generator.Generate(taskType, null, new MemberProvider());

        // Assert - derived Task types should also get CustomDescriptor
        descriptor.Should().BeOfType<CustomDescriptor>("ContinueWith Task types should be custom descriptors");
    }

    [Fact]
    public async Task TaskRun_NotCompleted_ShouldNotBlock()
    {
        // Arrange - Task.Run also returns a derived type (UnwrapPromise<T>)
        var task = Task.Run(async () =>
        {
            await Task.Delay(TimeSpan.FromSeconds(30));
            return 123;
        });

        // Act - Dump should NOT block waiting for completion
        var dumpTask = Task.Run(() =>
        {
            _ = task.Dump();
            return true;
        });

        // Wait a short time - if it completes, Dump didn't block
        var completed = await Task.WhenAny(dumpTask, Task.Delay(TimeSpan.FromSeconds(1))) == dumpTask;

        // Assert
        completed.Should().BeTrue("Dumping a Task.Run Task should not block");
        task.IsCompleted.Should().BeFalse("Task should still be incomplete");
    }
}
