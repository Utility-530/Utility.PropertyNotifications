using Utility.Tasks.Model;
using System;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Threading;
using System.Threading.Tasks;
using Utility.Infrastructure;

namespace Utility.Tasks.DemoApp.ViewModel
{


    public record UserAuthenticationResult(string UserName, bool Result);

    public record UserAuthenticationTaskOutput : TaskOutput
    {
        public UserAuthenticationTaskOutput(string key, UserAuthenticationResult value) : base(key, value)
        {
            Value = value;
        }

        public override UserAuthenticationResult Value { get; }
    }


    public class Authenticator : IObserver<LoginRequest>, IObservable<IWorkerItem>
    {
        protected readonly ReplaySubject<LoginRequest> loginRequestSubject = new();
        protected readonly ReplaySubject<IWorkerItem> taskItemSubject = new();
        private readonly Random random = new();
        private readonly TimeSpan timeSpan = TimeSpan.FromMilliseconds(2000);

        public Authenticator(IObservable<LoginRequest> loginRequest)
        {
            loginRequest
                .Subscribe(loginRequestSubject);

            loginRequestSubject
                .Select(a => (a, date: DateTime.Now))
                .Select(a => new AsyncWorkerItem(a.date.ToString("HH:mm:ss"),Observable.Empty<TaskChangeRequest>(), GetTask(a.a.UserName, a.a.Password, random), timeSpan, new CancellationTokenSource()))
                .Subscribe(taskItemSubject.OnNext);
        }

        static Task<ITaskOutput> GetTask(string userName, string passWord, Random random)
        {
            try
            {
                return new Task<ITaskOutput>(() =>
                {
                    Thread.Sleep(2000);
                    return new UserAuthenticationTaskOutput(userName, new UserAuthenticationResult(userName, random.NextDouble() > 0.5));
                });// client.GetStringAsync("https://msdn.microsoft.com");
            }
            catch (Exception ex)
            {
                return Task.FromResult<ITaskOutput>(new StringTaskOutput(userName, ex.Message));
            }
        }

        public void OnCompleted()
        {
            throw new NotImplementedException();
        }

        public void OnError(Exception error)
        {
            throw new NotImplementedException();
        }

        public void OnNext(LoginRequest value)
        {
            loginRequestSubject.OnNext(value);
        }

        public IDisposable Subscribe(IObserver<IWorkerItem> observer)
        {
            return taskItemSubject.Subscribe(observer);
        }
    }
}