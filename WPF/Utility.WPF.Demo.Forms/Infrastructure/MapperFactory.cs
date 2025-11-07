//using LiteDB;

using System;
using System.ComponentModel;
using System.Linq;
using AutoMapper;
using LiteDB;
using Utility.Helpers.Ex;
using Utility.WPF.Demo.Forms.Model;
using Utility.WPF.Demo.Forms.ViewModels;

namespace Utility.WPF.Demo.Forms.Infrastructure
{
    internal class MapperFactory
    {
        public const int PitToPitIndex = 0;
        public const int ShoulderIndex = 1;
        public const int SleeveLengthIndex = 2;
        public const int LengthIndex = 3;

        public static string HeaderFromIndex(int index)
        {
            return index switch
            {
                0 => "Pit-to-Pit",
                1 => "Shoulder",
                2 => "SleeveLength",
                3 => "Length",
                _ => throw new ArgumentOutOfRangeException(nameof(index), index, null)
            };
        }

        public static MapperConfiguration CreateMapperConfiguration()
        {
            return new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<EditViewModel, EditModel>().ForCtorParam(nameof(EditModel.Collection), opt => opt.MapFrom(src => src.Children)).ReverseMap();
                cfg.CreateMap<ImagesViewModel, ImagesModel>().ForCtorParam(nameof(ImagesModel.Collection), opt => opt.MapFrom(src => src.Children)).ForCtorParam(nameof(ImagesModel.Header), opt => opt.MapFrom(src => src.Key)).ReverseMap();
                cfg.CreateMap<ImageViewModel, ImageModel>().ReverseMap();
                cfg.CreateMap<MeasurementsViewModel, MeasurementsModel>().ForCtorParam(nameof(MeasurementsModel.Collection), opt => opt.MapFrom(src => src.Children)).ForCtorParam(nameof(MeasurementsModel.Header), opt => opt.MapFrom(src => src.Key)).ReverseMap();
                cfg.CreateMap<MeasurementViewModel, MeasurementModel>().ReverseMap();
                cfg.CreateMap<NotesViewModel, NotesModel>().ForCtorParam(nameof(NotesModel.Collection), opt => opt.MapFrom(src => src.Children)).ForCtorParam(nameof(NotesModel.Header), opt => opt.MapFrom(src => src.Key)).ReverseMap();
                cfg.CreateMap<NoteViewModel, NoteModel>().ReverseMap();
                cfg.CreateMap<TitleViewModel, TitleModel>().ReverseMap();
                cfg.CreateMap<INotifyPropertyChanged, HeaderModel>().ConstructUsing((a, b) =>
                {
                    return a switch
                    {
                        ImagesViewModel => b.Mapper.Map<ImagesModel>(a),
                        MeasurementsViewModel => b.Mapper.Map<MeasurementsModel>(a),
                        NotesViewModel => b.Mapper.Map<NotesModel>(a),
                        TitleViewModel => b.Mapper.Map<TitleModel>(a),
                        _ => throw new NotImplementedException(),
                    };
                });

                cfg.CreateMap<HeaderModel, INotifyPropertyChanged>().ConstructUsing((a, b) =>
                {
                    return a switch
                    {
                        ImagesModel => b.Mapper.Map<ImagesViewModel>(a),
                        MeasurementsModel => b.Mapper.Map<MeasurementsViewModel>(a),
                        NotesModel => b.Mapper.Map<NotesViewModel>(a),
                        TitleModel => b.Mapper.Map<TitleViewModel>(a),
                        _ => throw new NotImplementedException(),
                    };
                });
            });
        }

        public static void RegisterBsonMapper()
        {
            BsonMapper.Global.RegisterType(
                serialize: a => new BsonMapper().Serialize(a),
                deserialize: (bson) =>
                {
                    var headerModels =
                        from arr in bson["Collection"].AsArray
                        let type = Type.GetType(arr["_type"]) ?? throw new Exception("g 33fgfd4 444")
                        select (HeaderModel?)BsonHelper.Deserialise(arr.ToString(), type);

                    return new EditModel(bson["_id"].AsGuid, headerModels.ToArray());
                });
        }
    }
}