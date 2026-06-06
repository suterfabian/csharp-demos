using System.Collections.Concurrent;
using System.Diagnostics;
using System.Threading.Channels;
using System.Windows;
using HeyAsync.Services;

namespace HeyAsync;

public partial class MainWindow : Window
{
    private readonly object _counterLock = new();
    private readonly UiLogger _logger;
    private CancellationTokenSource? _cts;
    private CancellationTokenSource? _bleTimeoutCts;
    private TaskCompletionSource<string>? _bleResponseSource;

    public MainWindow()
    {
        InitializeComponent();

        _logger = new UiLogger(OutputTextBox);

        _logger.WriteLine("Async WPF Lab gestartet.");
        _logger.WriteLine($"UI Thread ID: {Environment.CurrentManagedThreadId}");
        _logger.AddEmptyLine();
    }

    private async void BasicAsyncButton_Click(object sender, RoutedEventArgs e)
    {
        _logger.WriteHeader("01 - Basic async/await");

        _logger.WriteLine("Start");
        _logger.WriteLine($"Vor await - Thread ID: {Environment.CurrentManagedThreadId}");

        await Task.Delay(6000);
        // Thread.Sleep(6000);

        _logger.WriteLine($"Nach await - Thread ID: {Environment.CurrentManagedThreadId}");
        _logger.WriteLine("Ende");
    }

    private async void TaskRunButton_Click(object sender, RoutedEventArgs e)
    {
        _logger.WriteHeader("02 - Task.Run");

        _logger.WriteLine($"UI Thread vor Task.Run: {Environment.CurrentManagedThreadId}");

        var result = await Task.Run(() =>
        {
            Thread.Sleep(4000);
            return Environment.CurrentManagedThreadId;
        });

        _logger.WriteLine($"Worker Thread in Task.Run: {result}");
        _logger.WriteLine($"UI Thread nach await: {Environment.CurrentManagedThreadId}");
    }

    /*
    // Alte Variante
    private void DispatcherButton_Click(object sender, RoutedEventArgs e)
    {
        _logger.WriteHeader("03 - Dispatcher");

        Task.Run(() =>
        {
            int backgroundThreadId = Environment.CurrentManagedThreadId;

            // Asynchrone alte Variante
            // Dispatcher.BeginInvoke = asynchron, wartet nicht
            Dispatcher.BeginInvoke(() =>
            {
                _logger.WriteLine($"Background Thread: {backgroundThreadId}");
                _logger.WriteLine($"Zurück auf UI Thread: {Environment.CurrentManagedThreadId}");
                _logger.WriteLine("UI-Ausgabe wurde über Dispatcher.Invoke geschrieben.");
            });

            // Ganze alt
            // Dispatcher.Invoke      = synchron, wartet
            // Dispatcher.Invoke(() =>
            // {
            //     _logger.WriteLine($"Background Thread: {backgroundThreadId}");
            //     _logger.WriteLine($"Zurück auf UI Thread: {Environment.CurrentManagedThreadId}");
            //     _logger.WriteLine("UI-Ausgabe wurde über Dispatcher.Invoke geschrieben.");
            // });
        });
    }
    */

    // Moderne Variante
    private async void DispatcherButton_Click(object sender, RoutedEventArgs e)
    {
        _logger.WriteHeader("03 - Dispatcher");

        await Task.Run(async () =>
        {
            int backgroundThreadId = Environment.CurrentManagedThreadId;
            Thread.Sleep(4000);

            await Dispatcher.InvokeAsync(() =>
            {
                _logger.WriteLine($"Background Thread: {backgroundThreadId}");
                _logger.WriteLine($"Zurück auf UI Thread: {Environment.CurrentManagedThreadId}");
                _logger.WriteLine("UI-Ausgabe wurde über Dispatcher.InvokeAsync geschrieben.");
            });
        });
    }

    private async void RaceConditionButton_Click(object sender, RoutedEventArgs e)
    {
        _logger.WriteHeader("04 - Race Condition");

        var counter = 0;

        await Task.Run(() => { Parallel.For(0, 100_000, _ => { counter++; }); });

        _logger.WriteLine("Erwartet: 100000");
        _logger.WriteLine($"Tatsächlich: {counter}");
        _logger.WriteLine("Die Abweichung entsteht durch gleichzeitige Schreibzugriffe.");
    }

    private async void LockButton_Click(object sender, RoutedEventArgs e)
    {
        _logger.WriteHeader("05 - Lock");

        var counter = 0;

        await Task.Run(() =>
        {
            Parallel.For(0, 100_000, _ =>
            {
                lock (_counterLock)
                {
                    counter++;
                }
            });
        });

        _logger.WriteLine("Erwartet: 100000");
        _logger.WriteLine($"Tatsächlich: {counter}");
        _logger.WriteLine("Der lock schützt den kritischen Bereich.");
    }

    private async void InterlockedButton_Click(object sender, RoutedEventArgs e)
    {
        _logger.WriteHeader("06 - Interlocked");

        var counter = 0;

        await Task.Run(() => { Parallel.For(0, 100_000, _ => { Interlocked.Increment(ref counter); }); });

        _logger.WriteLine("Erwartet: 100000");
        _logger.WriteLine($"Tatsächlich: {counter}");
        _logger.WriteLine("Interlocked.Increment erhöht den Wert atomar.");
    }

    private async void ParallelObjectListButton_Click(object sender, RoutedEventArgs e)
    {
        _logger.WriteHeader("07 - Parallel Object List");

        var items = Enumerable.Range(1, 100_000)
            .Select(id => new TestItem
            {
                Id = id,
                Value = 0
            })
            .ToList();

        await Task.Run(() =>
        {
            Parallel.For(0, items.Count, index =>
            {
                var item = items[index];

                item.Value = item.Id * 2;
            });
        });

        var wrongItems = items.Count(item => item.Value != item.Id * 2);

        _logger.WriteLine($"Anzahl Objekte: {items.Count}");
        _logger.WriteLine($"Fehlerhafte Objekte: {wrongItems}");
        _logger.WriteLine("Jeder Parallel.For-Durchlauf verändert ein eigenes Objekt.");
    }

    private async void ConcurrentBagButton_Click(object sender, RoutedEventArgs e)
    {
        _logger.WriteHeader("08 - ConcurrentBag");

        var items = new ConcurrentBag<TestItem>();

        await Task.Run(() =>
        {
            Parallel.For(0, 100_000, i =>
            {
                items.Add(new TestItem
                {
                    Id = i,
                    Value = i * 2
                });
            });
        });

        _logger.WriteLine($"Anzahl Objekte: {items.Count}");
        _logger.WriteLine("ConcurrentBag ist für paralleles Hinzufügen geeignet.");
    }

    private async void TaskErrorButton_Click(object sender, RoutedEventArgs e)
    {
        _logger.WriteHeader("09 - Fehler durch parallelen List-Zugriff");

        var items = new List<int>();
        var errors = new ConcurrentBag<Exception>();

        await Task.Run(() =>
        {
            Parallel.For(0, 1_000_000, i =>
            {
                try
                {
                    items.Add(i);
                }
                catch (Exception ex)
                {
                    errors.Add(ex);
                }
            });
        });

        var distinctCount = items.Distinct().Count();
        var duplicateCount = items.Count - distinctCount;
        var missingCount = 1_000_000 - distinctCount;

        _logger.WriteLine("Erwartet: 1000000");
        _logger.WriteLine($"Tatsächlich in Liste: {items.Count}");
        _logger.WriteLine($"Eindeutige Werte: {distinctCount}");
        _logger.WriteLine($"Duplikate: {duplicateCount}");
        _logger.WriteLine($"Fehlende Werte: {missingCount}");
        _logger.WriteLine($"Exceptions: {errors.Count}");

        _logger.WriteLine("");
        _logger.WriteLine("Wichtig: Keine Exception bedeutet nicht, dass der Zugriff threadsicher war.");
    }

    private async void CancellationButton_Click(object sender, RoutedEventArgs e)
    {
        _logger.WriteHeader("10 - Cancellation");

        _cts = new CancellationTokenSource();

        try
        {
            await Task.Run(async () =>
            {
                for (int i = 0; i < 100; i++)
                {
                    _cts.Token.ThrowIfCancellationRequested();

                    await Task.Delay(100);

                    Dispatcher.Invoke(() => { _logger.WriteLine($"Schritt {i}"); });
                }
            });

            _logger.WriteLine("Fertig");
        }
        catch (OperationCanceledException)
        {
            _logger.WriteLine("Abgebrochen");
        }
    }

    // -----------------------------------------------------------------------

    private void DeadlockButton_Click(object sender, RoutedEventArgs e)
    {
        _logger.WriteHeader("11 - Deadlock");

        var result = GetValueAsync().Result;

        _logger.WriteLine(result);
    }

    private async void NoDeadlockButton_Click(object sender, RoutedEventArgs e)
    {
        _logger.WriteHeader("12 - Kein Deadlock");

        var result = await GetValueAsync();

        _logger.WriteLine(result);
    }

    private static async Task<string> GetValueAsync()
    {
        await Task.Delay(1000);
        return "Hallo";
    }

    // -----------------------------------------------------------------------

    private async void DeadlockExerciseButton_Click(object sender, RoutedEventArgs e)
    {
        _logger.WriteHeader("13 - Knifflige Deadlock Übung");

        _logger.WriteLine($"Button Start - Thread ID: {Environment.CurrentManagedThreadId}");

        var result = await LoadCustomerReportAsync();

        _logger.WriteLine($"Resultat: {result}");
        _logger.WriteLine($"Button Ende - Thread ID: {Environment.CurrentManagedThreadId}");
    }

    private async Task<string> LoadCustomerReportAsync()
    {
        _logger.WriteLine($"LoadCustomerReportAsync Start - Thread ID: {Environment.CurrentManagedThreadId}");

        var customer = await LoadCustomerAsync();

        _logger.WriteLine($"Kunde geladen: {customer}");

        var statistics = await BuildStatistics();

        return $"{customer} / {statistics}";
    }

    private async Task<string> BuildStatistics()
    {
        _logger.WriteLine($"BuildStatistics Start - Thread ID: {Environment.CurrentManagedThreadId}");

        var value = await LoadStatisticsAsync();
        // var value = LoadStatisticsAsync().Result;

        _logger.WriteLine($"BuildStatistics Ende - Thread ID: {Environment.CurrentManagedThreadId}");

        return value;
    }

    private async Task<string> LoadCustomerAsync()
    {
        _logger.WriteLine($"LoadCustomerAsync vor await - Thread ID: {Environment.CurrentManagedThreadId}");

        await Task.Delay(500);

        _logger.WriteLine($"LoadCustomerAsync nach await - Thread ID: {Environment.CurrentManagedThreadId}");

        return "Muster AG";
    }

    private async Task<string> LoadStatisticsAsync()
    {
        _logger.WriteLine($"LoadStatisticsAsync vor await - Thread ID: {Environment.CurrentManagedThreadId}");

        await Task.Delay(1000);

        _logger.WriteLine($"LoadStatisticsAsync nach await - Thread ID: {Environment.CurrentManagedThreadId}");

        return "42 offene Aufträge";
    }

    // -----------------------------------------------------------------------

    private readonly SemaphoreSlim _semaphore = new(1, 1);

    private async void SemaphoreSlimButton_Click(object sender, RoutedEventArgs e)
    {
        _logger.WriteHeader("14 - SemaphoreSlim");

        var tasks = Enumerable.Range(1, 5)
            .Select(id => RunProtectedAsync(id))
            .ToList();

        await Task.WhenAll(tasks);

        _logger.WriteLine("Alle Tasks abgeschlossen.");
    }

    private async Task RunProtectedAsync(int id)
    {
        _logger.WriteLine($"Task {id}: wartet auf Eintritt");

        await _semaphore.WaitAsync();

        try
        {
            _logger.WriteLine($"Task {id}: betritt kritischen Bereich");

            await Task.Delay(1000);

            _logger.WriteLine($"Task {id}: verlässt kritischen Bereich");
        }
        finally
        {
            _semaphore.Release();
        }
    }

    // -----------------------------------------------------------------------
    // Das ist später sehr nützlich für BLE, Logs, Events oder Datenströme.

    private async void ChannelButton_Click(object sender, RoutedEventArgs e)
    {
        _logger.WriteHeader("15 - Channel Producer/Consumer");

        var channel = Channel.CreateUnbounded<string>();

        var producer = Task.Run(async () =>
        {
            for (var i = 1; i <= 10; i++)
            {
                var message = $"Nachricht {i}";

                await channel.Writer.WriteAsync(message);

                await Dispatcher.InvokeAsync(() => { _logger.WriteLine($"Producer schreibt: {message}"); });

                await Task.Delay(300);
            }

            channel.Writer.Complete();
        });

        var consumer = Task.Run(async () =>
        {
            await foreach (var message in channel.Reader.ReadAllAsync())
            {
                await Dispatcher.InvokeAsync(() => { _logger.WriteLine($"Consumer liest: {message}"); });

                await Task.Delay(600);
            }
        });

        await Task.WhenAll(producer, consumer);

        _logger.WriteLine("Producer und Consumer sind fertig.");
    }

    // -----------------------------------------------------------------------

    private async void AsyncStreamButton_Click(object sender, RoutedEventArgs e)
    {
        _logger.WriteHeader("16 - Async Stream");

        await foreach (var value in GenerateValuesAsync())
        {
            _logger.WriteLine(
                $"Wert: {value} | Thread: {Environment.CurrentManagedThreadId}");
        }

        _logger.WriteLine("Stream beendet");
    }

    private static async IAsyncEnumerable<int> GenerateValuesAsync()
    {
        for (var i = 1; i <= 10; i++)
        {
            await Task.Delay(500);

            yield return i;
        }
    }

    // -----------------------------------------------------------------------

    private async void ProgressButton_Click(object sender, RoutedEventArgs e)
    {
        _logger.WriteHeader("17 - Progress Reporting");

        var progress = new Progress<int>(value =>
        {
            _logger.WriteLine($"Fortschritt: {value}%");
            // z.B.
            // ProgressBar.Value = p;
        });

        await ProcessFileAsync(progress);

        _logger.WriteLine("Fertig");
    }

    private async Task ProcessFileAsync(IProgress<int> progress)
    {
        for (int i = 0; i <= 100; i += 10)
        {
            await Task.Delay(300);

            progress.Report(i);
        }
    }

    // -----------------------------------------------------------------------

    private async void WhenAllButton_Click(object sender, RoutedEventArgs e)
    {
        _logger.WriteHeader("18 - Task.WhenAll");

        var sw = Stopwatch.StartNew();

        var task1 = SimulateWorkAsync("Task 1", 3000);
        var task2 = SimulateWorkAsync("Task 2", 2000);
        var task3 = SimulateWorkAsync("Task 3", 1000);

        var results = await Task.WhenAll(task1, task2, task3);

        sw.Stop();

        foreach (var result in results)
        {
            _logger.WriteLine(result);
        }

        _logger.WriteLine($"Gesamtdauer: {sw.ElapsedMilliseconds} ms");
    }

    private async Task<string> SimulateWorkAsync(string name, int delay)
    {
        _logger.WriteLine($"{name} gestartet");

        await Task.Delay(delay);

        _logger.WriteLine($"{name} beendet");

        return name;
    }

    // -----------------------------------------------------------------------

    private async void WhenAnyButton_Click(object sender, RoutedEventArgs e)
    {
        _logger.WriteHeader("19 - Task.WhenAny");

        var task1 = SimulateWorkAsync("Task 1", 3000);
        var task2 = SimulateWorkAsync("Task 2", 2000);
        var task3 = SimulateWorkAsync("Task 3", 1000);

        var winner = await Task.WhenAny(task1, task2, task3);

        var result = await winner;

        _logger.WriteLine($"Gewinner: {result}");
    }

    // -----------------------------------------------------------------------

    private async void WhenAllExceptionButton_Click(object sender, RoutedEventArgs e)
    {
        _logger.WriteHeader("20 - WhenAll Exceptions");

        try
        {
            var task1 = SuccessAsync("Task 1", 1000);
            var task2 = FailAsync("Task 2", 2000);
            var task3 = SuccessAsync("Task 3", 3000);

            await Task.WhenAll(task1, task2, task3);

            _logger.WriteLine("Alle erfolgreich.");
        }
        catch (Exception ex)
        {
            _logger.WriteLine($"Exception: {ex.GetType().Name}");
            _logger.WriteLine(ex.Message);
        }
    }

    private async Task SuccessAsync(string name, int delay)
    {
        _logger.WriteLine($"{name} gestartet");

        await Task.Delay(delay);

        _logger.WriteLine($"{name} erfolgreich beendet");
    }

    private async Task FailAsync(string name, int delay)
    {
        _logger.WriteLine($"{name} gestartet");

        await Task.Delay(delay);

        throw new InvalidOperationException($"{name} ist fehlgeschlagen");
    }

    // -----------------------------------------------------------------------

    private async void CancellationWhenAllButton_Click(object sender, RoutedEventArgs e)
    {
        _logger.WriteHeader("21 - Cancellation + WhenAll");

        using var cts = new CancellationTokenSource();

        cts.CancelAfter(2500);

        var task1 = SuccessTaskAsync("Task 1", 1000, cts.Token);
        var task2 = FailureTaskAsync("Task 2", 1500, cts.Token);
        var task3 = LongRunningTaskAsync("Task 3", 5000, cts.Token);

        var allTasks = Task.WhenAll(task1, task2, task3);

        try
        {
            await allTasks;
        }
        catch
        {
            _logger.WriteLine("WhenAll beendet mit Fehler.");

            _logger.WriteLine($"Task1 Status: {task1.Status}");
            _logger.WriteLine($"Task2 Status: {task2.Status}");
            _logger.WriteLine($"Task3 Status: {task3.Status}");

            if (allTasks.Exception is not null)
            {
                foreach (var ex in allTasks.Exception.InnerExceptions)
                {
                    _logger.WriteLine($"{ex.GetType().Name}: {ex.Message}");
                }
            }
        }
    }

    private async Task SuccessTaskAsync(
        string name,
        int delay,
        CancellationToken token)
    {
        _logger.WriteLine($"{name} gestartet");

        await Task.Delay(delay, token);

        _logger.WriteLine($"{name} erfolgreich");
    }

    private async Task FailureTaskAsync(
        string name,
        int delay,
        CancellationToken token)
    {
        _logger.WriteLine($"{name} gestartet");

        await Task.Delay(delay, token);

        throw new InvalidOperationException($"{name} fehlgeschlagen");
    }

    private async Task LongRunningTaskAsync(
        string name,
        int delay,
        CancellationToken token)
    {
        _logger.WriteLine($"{name} gestartet");

        await Task.Delay(delay, token);

        _logger.WriteLine($"{name} erfolgreich");
    }

    // -----------------------------------------------------------------------

    private async void TaskCompletionSourceButton_Click(
        object sender,
        RoutedEventArgs e)
    {
        _logger.WriteHeader("22 - TaskCompletionSource");

        _logger.WriteLine("Warte auf Antwort...");

        var result = await WaitForResponseAsync();

        _logger.WriteLine($"Antwort erhalten: {result}");
    }

    private static Task<string> WaitForResponseAsync()
    {
        var tcs = new TaskCompletionSource<string>();

        Task.Run(async () =>
        {
            await Task.Delay(2000);

            tcs.SetResult("Hallo vom Gerät");
        });

        return tcs.Task;
    }

    // -----------------------------------------------------------------------

    private async void TaskCompletionSourceErrorButton_Click(object sender, RoutedEventArgs e)
    {
        _logger.WriteHeader("23 - TaskCompletionSource Fehlerfall");

        try
        {
            _logger.WriteLine("Warte auf Antwort...");

            var result = await WaitForFailedResponseAsync();

            _logger.WriteLine($"Antwort erhalten: {result}");
        }
        catch (Exception ex)
        {
            _logger.WriteLine("Fehler wurde über TaskCompletionSource geliefert.");
            _logger.WriteLine($"Typ: {ex.GetType().Name}");
            _logger.WriteLine($"Meldung: {ex.Message}");
        }
    }

    private static Task<string> WaitForFailedResponseAsync()
    {
        var tcs = new TaskCompletionSource<string>();

        Task.Run(async () =>
        {
            await Task.Delay(1500);

            tcs.SetException(
                new InvalidOperationException("Das Gerät hat mit einem Fehler geantwortet."));
        });

        return tcs.Task;
    }

    // -----------------------------------------------------------------------

    private async void BleRequestResponseButton_Click(object sender, RoutedEventArgs e)
    {
        _logger.WriteHeader("24 - BLE Request / Response");

        try
        {
            var response = await SendBleCommandAsync("READ_STATUS");

            _logger.WriteLine($"Antwort erhalten: {response}");
        }
        catch (Exception ex)
        {
            _logger.WriteLine($"Fehler: {ex.Message}");
        }
    }

    private async Task<string> SendBleCommandAsync(string command)
    {
        _logger.WriteLine($"Sende Command: {command}");

        _bleResponseSource = new TaskCompletionSource<string>(
            TaskCreationOptions.RunContinuationsAsynchronously);

        SimulateBleDeviceResponse(command);

        return await _bleResponseSource.Task;
    }

    private void SimulateBleDeviceResponse(string command)
    {
        Task.Run(async () =>
        {
            await Task.Delay(1500);

            var response = command switch
            {
                "READ_STATUS" => "STATUS: OK",
                "READ_VERSION" => "VERSION: 1.0.0",
                _ => "UNKNOWN_COMMAND"
            };

            OnBleNotificationReceived(response);
        });
    }

    // private void OnBleNotificationReceived(string data)
    // {
    //     Dispatcher.Invoke(() => { _logger.WriteLine($"BLE Notification empfangen: {data}"); });
    //
    //     _bleResponseSource?.TrySetResult(data);
    // }
    
    private void OnBleNotificationReceived(string data)
    {
        Dispatcher.BeginInvoke(() =>
        {
            _logger.WriteLine($"BLE Notification empfangen: {data}");
        });

        _bleResponseSource?.TrySetResult(data);
    }

    // ----------------------------------------------------------------------- 

    private async void BleTimeoutButton_Click(object sender, RoutedEventArgs e)
    {
        _logger.WriteHeader("25 - BLE Timeout + Cancel");

        _bleTimeoutCts = new CancellationTokenSource();

        try
        {
            var response = await SendBleCommandWithTimeoutAsync(
                command: "NO_RESPONSE",
                timeout: TimeSpan.FromSeconds(5),
                cancellationToken: _bleTimeoutCts.Token);

            _logger.WriteLine($"Antwort erhalten: {response}");
        }
        catch (OperationCanceledException)
        {
            _logger.WriteLine("Vorgang wurde durch Benutzer abgebrochen.");
        }
        catch (TimeoutException ex)
        {
            _logger.WriteLine($"Timeout: {ex.Message}");
        }
        finally
        {
            _bleTimeoutCts.Dispose();
            _bleTimeoutCts = null;
        }
    }

    // TaskCompletionSource erzeugt den wartenden Task.
    // TrySetResult beendet ihn erfolgreich.
    // TrySetException beendet ihn mit Fehler.
    // TrySetCanceled beendet ihn mit Abbruch.
    // finally räumt die Referenz wieder auf.
    private async Task<string> SendBleCommandWithTimeoutAsync(
        string command,
        TimeSpan timeout,
        CancellationToken cancellationToken)
    {
        _logger.WriteLine($"Sende Command: {command}");

        // Wenn SetResult aufgerufen wird,
        // führe die await-Fortsetzung nicht direkt inline aus,
        // sondern plane sie sauber asynchron ein.
        _bleResponseSource = new TaskCompletionSource<string>(
            TaskCreationOptions.RunContinuationsAsynchronously);

        try
        {
            SimulateBleDeviceResponseWithTimeout(command);
            
            var responseTask = _bleResponseSource.Task;
            var timeoutTask = Task.Delay(timeout); // kein cancellationToken - wegen Unterscheidbarkeit
            var cancelTask = Task.Delay(Timeout.InfiniteTimeSpan, cancellationToken);

            var completedTask = await Task.WhenAny(
                responseTask,
                timeoutTask,
                cancelTask);

            if (completedTask == cancelTask)
            {
                _bleResponseSource.TrySetCanceled(cancellationToken);
                cancellationToken.ThrowIfCancellationRequested();
                // throw new OperationCanceledException(cancellationToken);
            }

            if (completedTask == timeoutTask)
            {
                var exception = new TimeoutException(
                    $"Keine Antwort innerhalb von {timeout.TotalSeconds} Sekunden.");

                _bleResponseSource.TrySetException(exception);

                throw exception;

                // _bleResponseSource.TrySetException(
                //     new TimeoutException($"Keine Antwort innerhalb von {timeout.TotalSeconds} Sekunden."));
                //
                // throw new TimeoutException(
                //     $"Keine Antwort innerhalb von {timeout.TotalSeconds} Sekunden.");
            }

            return await responseTask;
        }
        finally
        {
            _bleResponseSource = null;
        }
    }

    private void SimulateBleDeviceResponseWithTimeout(string command)
    {
        Task.Run(async () =>
        {
            if (command == "NO_RESPONSE")
            {
                return;
            }

            await Task.Delay(1500);

            var response = command switch
            {
                "READ_STATUS" => "STATUS: OK",
                "READ_VERSION" => "VERSION: 1.0.0",
                _ => "UNKNOWN_COMMAND"
            };

            OnBleNotificationReceived(response);
        });
    }

    private void CancelBleTimeoutButton_Click(object sender, RoutedEventArgs e)
    {
        _bleTimeoutCts?.Cancel();
    }

    // -----------------------------------------------------------------------

    private async void BleRetryButton_Click(object sender, RoutedEventArgs e)
    {
        _logger.WriteHeader("26 - BLE Retry");

        using var cts = new CancellationTokenSource();

        try
        {
            var response = await SendBleCommandWithRetryAsync(
                command: "READ_STATUS",
                timeout: TimeSpan.FromSeconds(2),
                maxAttempts: 3,
                cancellationToken: cts.Token);

            _logger.WriteLine($"Antwort erhalten: {response}");
        }
        catch (Exception ex)
        {
            _logger.WriteLine($"Fehlgeschlagen: {ex.Message}");
        }
    }
    
    private async Task<string> SendBleCommandWithRetryAsync(
        string command,
        TimeSpan timeout,
        int maxAttempts,
        CancellationToken cancellationToken)
    {
        for (var attempt = 1; attempt <= maxAttempts; attempt++)
        {
            cancellationToken.ThrowIfCancellationRequested();

            try
            {
                _logger.WriteLine($"Versuch {attempt} von {maxAttempts}");

                return await SendBleCommandWithTimeoutAsync(
                    command,
                    timeout,
                    cancellationToken);
            }
            catch (TimeoutException)
            {
                _logger.WriteLine($"Versuch {attempt} Timeout");

                if (attempt == maxAttempts)
                {
                    throw;
                }

                await Task.Delay(500, cancellationToken);
            }
        }

        throw new InvalidOperationException("Unerwarteter Zustand im Retry-Loop.");
    }
    
    // -----------------------------------------------------------------------
    
    private int _retryErrorSimulationCounter;
    
    private async void BleRetryErrorButton_Click(object sender, RoutedEventArgs e)
    {
        _logger.WriteHeader("27 - Retry mit Fehlern");

        _retryErrorSimulationCounter = 0;

        using var cts = new CancellationTokenSource();

        try
        {
            var response = await SendBleCommandWithRetryAndErrorsAsync(
                command: "READ_STATUS",
                timeout: TimeSpan.FromSeconds(2),
                maxAttempts: 5,
                cancellationToken: cts.Token);

            _logger.WriteLine($"Antwort erhalten: {response}");
        }
        catch (Exception ex)
        {
            _logger.WriteLine("Endgültig fehlgeschlagen.");
            _logger.WriteLine($"{ex.GetType().Name}: {ex.Message}");
        }
    }
    
    private async Task<string> SendBleCommandWithRetryAndErrorsAsync(
        string command,
        TimeSpan timeout,
        int maxAttempts,
        CancellationToken cancellationToken)
    {
        Exception? lastException = null;

        for (int attempt = 1; attempt <= maxAttempts; attempt++)
        {
            cancellationToken.ThrowIfCancellationRequested();

            try
            {
                _logger.WriteLine($"Versuch {attempt} von {maxAttempts}");

                return await SendBleCommandWithTimeoutAndErrorsAsync(
                    command,
                    timeout,
                    cancellationToken);
            }
            catch (TimeoutException ex)
            {
                lastException = ex;
                _logger.WriteLine($"Timeout bei Versuch {attempt}");
            }
            catch (InvalidOperationException ex)
            {
                lastException = ex;
                _logger.WriteLine($"Gerätefehler bei Versuch {attempt}: {ex.Message}");
            }

            if (attempt < maxAttempts)
            {
                await Task.Delay(500, cancellationToken);
            }
        }

        throw lastException ?? new InvalidOperationException("Alle Versuche fehlgeschlagen.");
    }
    
    private async Task<string> SendBleCommandWithTimeoutAndErrorsAsync(
        string command,
        TimeSpan timeout,
        CancellationToken cancellationToken)
    {
        _logger.WriteLine($"Sende Command: {command}");

        _bleResponseSource = new TaskCompletionSource<string>(
            TaskCreationOptions.RunContinuationsAsynchronously);

        try
        {
            SimulateBleDeviceResponseWithErrors(command);

            var responseTask = _bleResponseSource.Task;
            var timeoutTask = Task.Delay(timeout);
            var cancelTask = Task.Delay(Timeout.InfiniteTimeSpan, cancellationToken);

            var completedTask = await Task.WhenAny(
                responseTask,
                timeoutTask,
                cancelTask);

            if (completedTask == cancelTask)
            {
                _bleResponseSource.TrySetCanceled(cancellationToken);
                cancellationToken.ThrowIfCancellationRequested();
            }

            if (completedTask == timeoutTask)
            {
                var exception = new TimeoutException(
                    $"Keine Antwort innerhalb von {timeout.TotalSeconds} Sekunden.");

                _bleResponseSource.TrySetException(exception);
                throw exception;
            }

            return await responseTask;
        }
        finally
        {
            _bleResponseSource = null;
        }
    }
    
    private void SimulateBleDeviceResponseWithErrors(string command)
    {
        Task.Run(async () =>
        {
            _retryErrorSimulationCounter++;

            int attempt = _retryErrorSimulationCounter;

            if (attempt == 1)
            {
                // Keine Antwort -> Timeout
                return;
            }

            await Task.Delay(1000);

            if (attempt == 2)
            {
                // Gerätefehler
                _bleResponseSource?.TrySetException(
                    new InvalidOperationException("CRC Fehler"));
                return;
            }

            // Ab Versuch 3 Erfolg
            _bleResponseSource?.TrySetResult("STATUS: OK");
        });
    }
    
    // -----------------------------------------------------------------------
    // Das ist das Grundmuster für BLE:
    // ViewModel
    // ↓
    // Command Queue
    // ↓
    // ein BLE Worker
    // ↓
    // Gerät
    
    private async void BleCommandQueueButton_Click(object sender, RoutedEventArgs e)
    {
        _logger.WriteHeader("28 - BLE Command Queue");

        var channel = Channel.CreateUnbounded<BleCommandRequest>();

        Task worker = Task.Run(async () =>
        {
            await foreach (var request in channel.Reader.ReadAllAsync())
            {
                try
                {
                    await Dispatcher.InvokeAsync(() =>
                    {
                        _logger.WriteLine($"Worker verarbeitet: {request.Command}");
                    });

                    await Task.Delay(1000);

                    request.ResponseSource.TrySetResult($"Antwort für {request.Command}");
                }
                catch (Exception ex)
                {
                    request.ResponseSource.TrySetException(ex);
                }
            }
        });

        Task<string> task1 = EnqueueBleCommandAsync(channel, "READ_STATUS");
        Task<string> task2 = EnqueueBleCommandAsync(channel, "READ_VERSION");
        Task<string> task3 = EnqueueBleCommandAsync(channel, "READ_CONFIG");

        string[] responses = await Task.WhenAll(task1, task2, task3);

        foreach (var response in responses)
        {
            _logger.WriteLine(response);
        }

        channel.Writer.Complete();

        await worker;

        _logger.WriteLine("Queue beendet.");
    }
    
    private static async Task<string> EnqueueBleCommandAsync(
        Channel<BleCommandRequest> channel,
        string command)
    {
        var responseSource = new TaskCompletionSource<string>(
            TaskCreationOptions.RunContinuationsAsynchronously);

        var request = new BleCommandRequest
        {
            Command = command,
            ResponseSource = responseSource
        };

        await channel.Writer.WriteAsync(request);

        return await responseSource.Task;
    }
    
    // -----------------------------------------------------------------------
    
    private async void FirmwareUploadButton_Click(object sender, RoutedEventArgs e)
    {
        _logger.WriteHeader("29 - Firmware Upload Simulation");

        using var cts = new CancellationTokenSource();

        var progress = new Progress<int>(percent =>
        {
            _logger.WriteLine($"Fortschritt: {percent}%");
        });

        try
        {
            byte[] firmware = Enumerable.Range(0, 5000)
                .Select(i => (byte)(i % 256))
                .ToArray();

            await UploadFirmwareAsync(
                firmware,
                chunkSize: 180,
                progress,
                cts.Token);

            _logger.WriteLine("Firmware Upload abgeschlossen.");
        }
        catch (OperationCanceledException)
        {
            _logger.WriteLine("Firmware Upload abgebrochen.");
        }
    }
    
    private async Task UploadFirmwareAsync(
        byte[] firmware,
        int chunkSize,
        IProgress<int> progress,
        CancellationToken cancellationToken)
    {
        int totalChunks = (int)Math.Ceiling(firmware.Length / (double)chunkSize);

        for (int chunkIndex = 0; chunkIndex < totalChunks; chunkIndex++)
        {
            cancellationToken.ThrowIfCancellationRequested();

            int offset = chunkIndex * chunkSize;
            int length = Math.Min(chunkSize, firmware.Length - offset);

            byte[] chunk = new byte[length];
            Array.Copy(firmware, offset, chunk, 0, length);

            await WriteFirmwareChunkAsync(
                chunkIndex,
                totalChunks,
                chunk,
                cancellationToken);

            int percent = (int)(((chunkIndex + 1) / (double)totalChunks) * 100);
            progress.Report(percent);
        }
    }
    
    private async Task WriteFirmwareChunkAsync(
        int chunkIndex,
        int totalChunks,
        byte[] chunk,
        CancellationToken cancellationToken)
    {
        _logger.WriteLine(
            $"Schreibe Chunk {chunkIndex + 1}/{totalChunks}, Bytes: {chunk.Length}");

        await Task.Delay(80, cancellationToken);
    }
    
    // -----------------------------------------------------------------------
    
    private CancellationTokenSource? _firmwareUploadCts;
    
    private async void FirmwareUploadWithCancelButton_Click(object sender, RoutedEventArgs e)
    {
        _logger.WriteHeader("30 - Firmware Upload mit Abbrechen");

        _firmwareUploadCts = new CancellationTokenSource();

        var progress = new Progress<int>(percent =>
        {
            _logger.WriteLine($"Fortschritt: {percent}%");
        });

        try
        {
            byte[] firmware = Enumerable.Range(0, 20_000)
                .Select(i => (byte)(i % 256))
                .ToArray();

            await UploadFirmwareAsync(
                firmware,
                chunkSize: 180,
                progress,
                _firmwareUploadCts.Token);

            _logger.WriteLine("Firmware Upload abgeschlossen.");
        }
        catch (OperationCanceledException)
        {
            _logger.WriteLine("Firmware Upload wurde abgebrochen.");
        }
        finally
        {
            _firmwareUploadCts.Dispose();
            _firmwareUploadCts = null;
        }
    }
    
    private void CancelFirmwareUploadButton_Click(object sender, RoutedEventArgs e)
    {
        _firmwareUploadCts?.Cancel();
    }
    
    // -----------------------------------------------------------------------
    
    private async void FirmwareUploadRetryButton_Click(object sender, RoutedEventArgs e)
    {
        _logger.WriteHeader("31 - Firmware Upload mit Retry");

        using var cts = new CancellationTokenSource();

        var progress = new Progress<int>(percent =>
        {
            _logger.WriteLine($"Fortschritt: {percent}%");
        });

        try
        {
            byte[] firmware = Enumerable.Range(0, 10_000)
                .Select(i => (byte)(i % 256))
                .ToArray();

            await UploadFirmwareWithRetryAsync(
                firmware,
                chunkSize: 180,
                maxAttemptsPerChunk: 3,
                progress,
                cts.Token);

            _logger.WriteLine("Firmware Upload abgeschlossen.");
        }
        catch (Exception ex)
        {
            _logger.WriteLine("Firmware Upload fehlgeschlagen.");
            _logger.WriteLine($"{ex.GetType().Name}: {ex.Message}");
        }
    }
    
    private async Task UploadFirmwareWithRetryAsync(
        byte[] firmware,
        int chunkSize,
        int maxAttemptsPerChunk,
        IProgress<int> progress,
        CancellationToken cancellationToken)
    {
        int totalChunks = (int)Math.Ceiling(firmware.Length / (double)chunkSize);

        for (int chunkIndex = 0; chunkIndex < totalChunks; chunkIndex++)
        {
            cancellationToken.ThrowIfCancellationRequested();

            int offset = chunkIndex * chunkSize;
            int length = Math.Min(chunkSize, firmware.Length - offset);

            byte[] chunk = new byte[length];
            Array.Copy(firmware, offset, chunk, 0, length);

            await WriteFirmwareChunkWithRetryAsync(
                chunkIndex,
                totalChunks,
                chunk,
                maxAttemptsPerChunk,
                cancellationToken);

            int percent = (int)(((chunkIndex + 1) / (double)totalChunks) * 100);
            progress.Report(percent);
        }
    }
    
    private async Task WriteFirmwareChunkWithRetryAsync(
        int chunkIndex,
        int totalChunks,
        byte[] chunk,
        int maxAttempts,
        CancellationToken cancellationToken)
    {
        for (int attempt = 1; attempt <= maxAttempts; attempt++)
        {
            cancellationToken.ThrowIfCancellationRequested();

            try
            {
                _logger.WriteLine(
                    $"Schreibe Chunk {chunkIndex + 1}/{totalChunks}, Versuch {attempt}");

                await WriteFirmwareChunkWithRandomErrorAsync(
                    chunkIndex,
                    totalChunks,
                    chunk,
                    cancellationToken);

                return;
            }
            catch (TimeoutException ex)
            {
                _logger.WriteLine($"Timeout bei Chunk {chunkIndex + 1}: {ex.Message}");

                if (attempt == maxAttempts)
                {
                    throw;
                }

                await Task.Delay(300, cancellationToken);
            }
        }
    }
    
    private async Task WriteFirmwareChunkWithRandomErrorAsync(
        int chunkIndex,
        int totalChunks,
        byte[] chunk,
        CancellationToken cancellationToken)
    {
        await Task.Delay(80, cancellationToken);

        if (chunkIndex % 7 == 0)
        {
            throw new TimeoutException(
                $"Simulierter Timeout bei Chunk {chunkIndex + 1}");
        }

        _logger.WriteLine(
            $"Chunk {chunkIndex + 1}/{totalChunks} erfolgreich geschrieben, Bytes: {chunk.Length}");
    }
    
    // -----------------------------------------------------------------------

    private readonly HashSet<int> _failedFirmwareChunksOnce = [];
    
    private async void FirmwareUploadTransientRetryButton_Click(object sender, RoutedEventArgs e)
    {
        _logger.WriteHeader("32 - Firmware Upload transienter Retry");

        _failedFirmwareChunksOnce.Clear();

        using var cts = new CancellationTokenSource();

        var progress = new Progress<int>(percent =>
        {
            _logger.WriteLine($"Fortschritt: {percent}%");
        });

        try
        {
            byte[] firmware = Enumerable.Range(0, 10_000)
                .Select(i => (byte)(i % 256))
                .ToArray();

            await UploadFirmwareWithTransientRetryAsync(
                firmware,
                chunkSize: 180,
                maxAttemptsPerChunk: 3,
                progress,
                cts.Token);

            _logger.WriteLine("Firmware Upload abgeschlossen.");
        }
        catch (Exception ex)
        {
            _logger.WriteLine("Firmware Upload fehlgeschlagen.");
            _logger.WriteLine($"{ex.GetType().Name}: {ex.Message}");
        }
    }
    
    private async Task UploadFirmwareWithTransientRetryAsync(
        byte[] firmware,
        int chunkSize,
        int maxAttemptsPerChunk,
        IProgress<int> progress,
        CancellationToken cancellationToken)
    {
        int totalChunks = (int)Math.Ceiling(firmware.Length / (double)chunkSize);

        for (int chunkIndex = 0; chunkIndex < totalChunks; chunkIndex++)
        {
            cancellationToken.ThrowIfCancellationRequested();

            int offset = chunkIndex * chunkSize;
            int length = Math.Min(chunkSize, firmware.Length - offset);

            byte[] chunk = new byte[length];
            Array.Copy(firmware, offset, chunk, 0, length);

            await WriteFirmwareChunkWithTransientRetryAsync(
                chunkIndex,
                totalChunks,
                chunk,
                maxAttemptsPerChunk,
                cancellationToken);

            int percent = (int)(((chunkIndex + 1) / (double)totalChunks) * 100);
            progress.Report(percent);
        }
    }
    
    private async Task WriteFirmwareChunkWithTransientRetryAsync(
        int chunkIndex,
        int totalChunks,
        byte[] chunk,
        int maxAttempts,
        CancellationToken cancellationToken)
    {
        for (int attempt = 1; attempt <= maxAttempts; attempt++)
        {
            cancellationToken.ThrowIfCancellationRequested();

            try
            {
                _logger.WriteLine(
                    $"Schreibe Chunk {chunkIndex + 1}/{totalChunks}, Versuch {attempt}");

                await WriteFirmwareChunkWithTransientErrorAsync(
                    chunkIndex,
                    totalChunks,
                    chunk,
                    cancellationToken);

                return;
            }
            catch (TimeoutException ex)
            {
                _logger.WriteLine($"Timeout: {ex.Message}");

                if (attempt == maxAttempts)
                {
                    throw;
                }

                await Task.Delay(300, cancellationToken);
            }
        }
    }
    
    private async Task WriteFirmwareChunkWithTransientErrorAsync(
        int chunkIndex,
        int totalChunks,
        byte[] chunk,
        CancellationToken cancellationToken)
    {
        await Task.Delay(80, cancellationToken);

        bool shouldFailOnce =
            chunkIndex > 0 &&
            chunkIndex % 7 == 0 &&
            !_failedFirmwareChunksOnce.Contains(chunkIndex);

        if (shouldFailOnce)
        {
            _failedFirmwareChunksOnce.Add(chunkIndex);

            throw new TimeoutException(
                $"Simulierter transienter Timeout bei Chunk {chunkIndex + 1}");
        }

        _logger.WriteLine(
            $"Chunk {chunkIndex + 1}/{totalChunks} erfolgreich geschrieben, Bytes: {chunk.Length}");
    }
    
    
    
    
    
    
    
    
    
    
    
    
    
    
    
    
    
    
    
    
    
    // -----------------------------------------------------------------------

    private void CancelButton_Click(object sender, RoutedEventArgs e)
    {
        _cts?.Cancel();
    }

    // -----------------------------------------------------------------------

    private void ClearButton_Click(object sender, RoutedEventArgs e)
    {
        _logger.Clear();
    }

    // -----------------------------------------------------------------------

    private sealed class TestItem
    {
        public int Id { get; init; }
        public int Value { get; set; }
    }
    
    private sealed class BleCommandRequest
    {
        public required string Command { get; init; }
        public required TaskCompletionSource<string> ResponseSource { get; init; }
    }
}