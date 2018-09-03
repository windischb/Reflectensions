using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Reflectensions.Helpers
{
    public class ConsoleHelper : IObservable<ConsoleKeyInfo>
    {
        private static List<IObserver<ConsoleKeyInfo>> observers = new List<IObserver<ConsoleKeyInfo>>();

        public IDisposable Subscribe(IObserver<ConsoleKeyInfo> observer) {
            if (!observers.Contains(observer)) {
                observers.Add(observer);
            }
            return new UnSubscriber(observers, observer);
        }

        public static void RegisterNewLineHandler(CancellationToken cancellationToken = default(CancellationToken))
        {

#pragma warning disable 4014
            RegisterNewLineHandlerAsync(cancellationToken);
#pragma warning restore 4014

        }
        public static async Task RegisterNewLineHandlerAsync(CancellationToken cancellationToken = default(CancellationToken))
        {
            if (!Environment.UserInteractive || Console.IsInputRedirected)
                return;

            await Task.Run(() =>
            {
                var t = true;
                while (t)
                {
                    if (cancellationToken.IsCancellationRequested)
                    {
                        t = false;
                    }
                    var key = Console.ReadKey(true);

                    foreach (var obs in observers) {
                        obs.OnNext(key);
                    }

                    switch (key.Key)
                    {
                        case ConsoleKey.Enter:
                            Console.WriteLine();
                            break;
                        default:
                            break;
                    }
                    if (cancellationToken.IsCancellationRequested)
                    {
                        t = false;
                    }
                }
            }, cancellationToken);
        }

        private class UnSubscriber : IDisposable {
            private List<IObserver<ConsoleKeyInfo>> lstObservers;
            private IObserver<ConsoleKeyInfo> observer;

            public UnSubscriber(List<IObserver<ConsoleKeyInfo>> ObserversCollection, IObserver<ConsoleKeyInfo> observer) {
                this.lstObservers = ObserversCollection;
                this.observer = observer;
            }

            public void Dispose() {
                if (this.observer != null) {
                    lstObservers.Remove(this.observer);
                }
            }
        }
    }
}
