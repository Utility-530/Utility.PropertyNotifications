using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace Utility.Entities
{
    public readonly record struct FilePath(string Directory, string FileName)
    {
        public string Full => Path.Combine(Directory, FileName);
        public static FilePath FromFilePath(string filePath) => new(Path.GetDirectoryName(filePath), Path.GetFileName(filePath));
    }
    public readonly record struct Instance(object Value)
    {
    }

    public readonly record struct RazorEngineOutput(string Output, string Template, object Instance);
    public readonly record struct Filter(string Value);
    public readonly record struct AddItem(object Value);
    public readonly record struct FilterQuery(string Filter, object Value);
    public readonly record struct InList(IList Collection);
    public readonly record struct ViewList(IEnumerable Collection);
    public readonly record struct FilterPredicate(Predicate<object> Value);
    public readonly record struct TypeValue(Type Type);
    public readonly record struct SelectionInput(object Value);
    public readonly record struct SelectionOutput(object Value);
}
