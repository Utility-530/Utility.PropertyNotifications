using RazorEngine.Templating;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using Utility.Entities;

namespace Utility.Services
{ 
    public class RazorService : IObserver<FilePath>, IObserver<Instance>, IObservable<RazorEngineOutput>
    {
        public record ObjectDTO(object Value, string TemplateName, string Directory);
        public record RazorFileDTO(string Content, object Value, string Text);

        private readonly Subject<FilePath> filePaths = new();
        private readonly Subject<Instance> objects = new();
        private readonly Subject<RazorEngineOutput> razorFiles = new();

        public RazorService()
        {
            CancellationTokenSource? source = null;

            string? dir = null;

            int i = 0;
            var obs = filePaths.CombineLatest(objects)
                .Subscribe(async a =>
                {                    
                    Utility.Globals.Logs.OnNext(new Log { Date = DateTime.Now, Message = "Compiling model", Source = nameof(RazorService) });
                    source?.Cancel(false);
                    source = new();
                    var (filePath, instance) = a;
                    var map = await Task.Run<RazorFileDTO>(() =>
                    {
                        try
                        {
                            var service = RazorEngineService.Create();
                            //var x = Path.Combine(Directory, TemplateName);
                            var text = File.ReadAllText(filePath.Full);
                            string textIndex = service.RunCompile(text, filePath.Full, null, instance.Value);
                            Globals.Logs.OnNext(new Log { Date = DateTime.Now, Message = "Compiled model", Source = nameof(RazorService) });
                            return new RazorFileDTO(textIndex, instance.Value, text);
                        }
                        catch (Exception ex)
                        {
                            Globals.Logs.OnNext(new Log { Date = DateTime.Now, Message = $"Failed to Compiled model : {ex.Message}", Source = nameof(RazorService) });
                        }
                        return null;
                    }, source.Token);

                    razorFiles.OnNext(new(map.Content, map.Text, map.Value));
                });

   
        }

        #region boilerplate
        public void OnCompleted()
        {
            throw new NotImplementedException();
        }

        public void OnError(Exception error)
        {
            throw new NotImplementedException();
        }

        public void OnNext(FilePath value)
        {
            filePaths.OnNext(value);
        }
        public void OnNext(Instance value)
        {
            objects.OnNext(value);
        }

        public IDisposable Subscribe(IObserver<RazorEngineOutput> observer)
        {
            return razorFiles.Subscribe(observer);
        }
        #endregion boilerplate
    }
}
