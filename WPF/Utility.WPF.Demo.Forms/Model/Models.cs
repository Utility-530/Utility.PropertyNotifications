using System;
using System.Collections.Generic;
using UnitsNet.Units;

namespace Utility.WPF.Demo.Forms.Model
{
    public record Model;
    public record EditModel(Guid Id, IReadOnlyCollection<HeaderModel> Collection) : Model;
    public record HeaderModel(string Header) : Model();
    public record ImagesModel(string Header, IReadOnlyCollection<ImageModel> Collection) : HeaderModel(Header);
    public record ImageModel(Guid Guid, string URL, string WebURL, bool IsChecked) : Model;
    public record MeasurementsModel(string Header, LengthUnit Unit, IReadOnlyCollection<MeasurementModel> Collection) : HeaderModel(Header);
    public record MeasurementModel(string Header, double Value) : HeaderModel(Header);
    public record NotesModel(string Header, IReadOnlyCollection<NoteModel> Collection) : HeaderModel(Header);
    public record NoteModel(string Text) : Model;
    public record TitleModel(string Title, string SubTitle) : HeaderModel("Title");
}