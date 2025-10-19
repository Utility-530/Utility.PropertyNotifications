using RazorEngine.Templating;
using Utility.Entities;
using Utility.Models;
using Utility.Services.Meta;
using Utility.Structs;

namespace Utility.Services
{
    public record RazorInstanceParam() : Param<RazorService>(nameof(RazorService.ToRazorFile), "instance");
    public record RazorFilePathParam() : Param<RazorService>(nameof(RazorService.ToRazorFile), "filePath");
    public record RazorFileReturnParam() : Param<RazorService>(nameof(RazorService.ToRazorFile));

    public readonly record struct RazorFileDTO(string Content, object Value, string Text);


    public class RazorService //: IObserver<FilePath>, IObserver<Instance>, IObservable<RazorEngineOutput>
    {

        CancellationTokenSource? source = null;


        public async Task<RazorFileDTO> ToRazorFile(FilePath filePath, object instance)
        {
            //Utility.Globals.Logs.OnNext(new Log { Date = DateTime.Now, Message = "Compiling model", Source = nameof(RazorService) });
            source?.Cancel(false);
            source = new();

            var map = await Task.Run<RazorFileDTO>(() =>
            {
                try
                {
                    var service = RazorEngineService.Create();
                    //var x = Path.Combine(Directory, TemplateName);
                    var text = File.ReadAllText(filePath.Full);
                    string textIndex = service.RunCompile(text, filePath.Full, null, instance);
                    //Globals.Logs.OnNext(new Log { Date = DateTime.Now, Message = "Compiled model", Source = nameof(RazorService) });
                    return new RazorFileDTO(textIndex, instance, text);
                }
                catch (Exception ex)
                {
                    throw ex;
                    //Globals.Logs.OnNext(new Log { Date = DateTime.Now, Message = $"Failed to Compiled model : {ex.Message}", Source = nameof(RazorService) });
                }
                return default;
            }, source.Token);
            return map;
        }
    }
}
