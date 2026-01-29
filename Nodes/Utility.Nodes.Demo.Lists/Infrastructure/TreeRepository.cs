using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Reflection;
using System.Security.Policy;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Forms;
using ActivateAnything;
using AvalonEditB.Highlighting;
using HandyControl.Controls;
using SQLite;
using SQLitePCL;
using Utility.Entities;
using Utility.Enums;
using Utility.Helpers;
using Utility.Helpers.Reflection;
using Utility.Interfaces.Exs;
using Utility.Interfaces.Exs.Diagrams;
using Utility.Interfaces.NonGeneric;
using Utility.Interfaces.NonGeneric.Data;
using Utility.PropertyNotifications;
using Utility.Reactives;
using Utility.Structs.Repos;
using Utility.Trees;
using Change = Utility.Changes.Change;

namespace Utility.Nodes.Demo.Lists.Infrastructure
{
    public class TreeRepository : ITreeRepository, IValueRepository, IDataActivator
    {
        Dictionary<Guid, (Guid parentGuid, Guid root, string name)> dictionary = new();
        Dictionary<Guid, ObjectWrapper> values = new();
        Dictionary<Guid, INodeViewModel> nodes = new();
        Dictionary<(Guid, string), ReplaySubject<DateValue>> replay = new();

        public static class Constants
        {
            public const string Title = "Title";
            public const string TitleValue = "Title.Value";
            public const string SubTitle = "Sub-Title";
            public const string SubTitleValue = "Sub-Title.Value";
            public const string Descriptions = "Descriptions";
            public const string Images = "Images";
            public const string Measurements = "Measurements";
            public const string Disclaimers = "Disclaimers";
            public const string HasShipping = "Has Shipping";
            public const string HasShippingValue = "Has Shipping.Value";
            public const string Description = "Description";
            public const string Image = "ImagePath";
            public const string Measurement = "Measurement";
            public const string MeasurementHeader = "Measurement.Header";
            public const string MeasurementCentimetres = "Measurement.Centimetres";
            public const string MeasurementInches = "Measurement.Inches";
            public const string PitToPit = "Pit To Pit";
            public const string SleeveLength = "Sleeve Length";
            public const string Length = "Length";
            public const string PitToPitIn = "Pit To Pit (in)";
            public const string SleeveLengthIn = "Sleeve Length (in)";
            public const string LengthIn = "Length (in)";
            public const string PitToPitCm = "Pit To Pit (cm)";
            public const string SleeveLengthCm = "Sleeve Length (cm)";
            public const string LengthCm = "Length (cm)";
            public const string Disclaimer = "Disclaimer";
            public const string Value = nameof(Value);
            public const string centimetre = "cm";
            public const string inch = "in";
            public const string Unit = "unit";
        }

        private TreeRepository()
        {
        }

        public INodeViewModel CreateRoot(object instance)
        {
            Guid guid;
            if (instance is Entity _guid)
            {
                if (_guid.Id == default)
                {
                    _guid.Id = Guid.NewGuid();

                }
                guid = _guid.Id;
            }
            else
            {
                throw new Exception("V$ccc");
            }
            if (nodes.ContainsKey(guid))
            {
                return nodes[guid];
            }

            var model = new Models.Model()
            {
                Guid = guid,
                IsExpanded = true,
                Name = instance.GetType().Name,
            };
            values.Add(guid, new ObjectWrapper(instance));
            dictionary.Add(guid, (default, guid, instance.GetType().Name));
            nodes.Add(guid, model);
            return model;
        }

        public object Activate(Key? a)
        {
            var _bool = containerStyle(a.Value.Name) != Enums.VisualLayout.Content;
            var _dataTemplate = dataTemplate(a.Value.Name);
            var value = get(values[dictionary[a.Value.Guid].root], a.Value.Name);
            var model = new Models.Model
            {
                Name = a.Value.Name,
                IsExpanded = _bool,
                IsProliferable = _bool,
                DataTemplate = _dataTemplate,
                Type = type(a.Value.Name),
                Orientation = orientation(a.Value.Name),
                Style = style(a.Value.Name),
                Layout = containerStyle(a.Value.Name),
                IsVisible = isVisible(a.Value.Name),
                Value = value
            };
            (replay[(dictionary[a.Value.Guid].root, a.Value.Name)] = new())
                .Subscribe(a =>
                {
                    model.RaisePropertyChanged(nameof(NodeViewModel.Value));
                });
            return model;

            static bool isVisible(string name)
            {
                if (name.EndsWith(Constants.Value))
                {

                }
                else
                {
                    if (name.Equals(Constants.Title))
                        return false;
                    else if (name.Equals(Constants.SubTitle))
                        return false;
                    if (name.Equals(Constants.Descriptions))
                        return false;
                    if (name.Equals(Constants.Disclaimers))
                        return false;
                    if (name.Equals(Constants.Images))
                        return false;
                    if (name.Equals(Constants.Measurements))
                        return false;
                    if (name.StartsWith(Constants.Measurement))
                        return false;
                }
                return true;
            }

            object get(ObjectWrapper _value, string _name)
            {
                if (_name == Constants.Title)
                    return Constants.Title;
                if (_name == Constants.Title + "." + Constants.Value)
                    return _value.Get<string>(nameof(AuctionItem.Title));
                if (_name == Constants.SubTitle)
                    return Constants.SubTitle;
                if (_name == Constants.SubTitle + "." + Constants.Value)
                    return _value.Get<string>(nameof(AuctionItem.SubTitle));
                if (_name == Constants.Descriptions)
                    return Constants.Descriptions;
                if (_name == Constants.Images)
                    return Constants.Images;
                if (_name == Constants.Measurements)
                    return Constants.Measurements;
                if (_name == Constants.Disclaimers)
                    return Constants.Disclaimers;
                if (_name == Constants.HasShipping)
                    return Constants.HasShipping;
                if (_name == Constants.HasShipping + "." + Constants.Value)
                    return _value.Get<bool>(nameof(AuctionItem.HasShipping));
                if (_name == Constants.PitToPitCm)
                    return _value.Get<int>(nameof(AuctionItem.PitToPitWidthInMillimetres)) / 10d;
                if (_name == Constants.SleeveLengthCm)
                    return _value.Get<int>(nameof(AuctionItem.SleeveLengthInMillimetres)) / 10d;
                if (_name == Constants.LengthCm)
                    return _value.Get<int>(nameof(AuctionItem.LengthInMillimetres)) / 10d;
                if (_name == Constants.PitToPitIn)
                    return _value.Get<int>(nameof(AuctionItem.PitToPitWidthInMillimetres)) * MMtoInchConversionFactory;
                if (_name == Constants.SleeveLengthIn)
                    return _value.Get<int>(nameof(AuctionItem.SleeveLengthInMillimetres)) * MMtoInchConversionFactory;
                if (_name == Constants.LengthIn)
                    return _value.Get<int>(nameof(AuctionItem.LengthInMillimetres)) * MMtoInchConversionFactory;
                if (Regex.Match(_name, @"([a-zA-Z]+\d+)", RegexOptions.IgnoreCase) is Match { Success: true, Groups: { } groups })
                    return _value.Get<string>(groups[1].Value);
                return _name;

            }

            static Enums.Visual style(string name)
            {
                if (name.Equals(Constants.Title))
                {
                    return Enums.Visual.Label;
                }
                else if (name.Equals(Constants.TitleValue))
                {
                    return Enums.Visual.Title;
                }
                else if (name.Equals(Constants.SubTitle))
                {
                    return Enums.Visual.Label;
                }
                else if (name.Equals(Constants.SubTitleValue))
                {
                    return Enums.Visual.Subtitle;
                }
                else if (name.Equals(Constants.Descriptions))
                {
                    return Enums.Visual.SecondaryHeader;
                }
                else if (Regex.IsMatch(name, Constants.Description + "\\d+"))
                {
                    return Enums.Visual.Text;
                }
                else if (name.Equals(Constants.Images))
                {
                    return Enums.Visual.SecondaryHeader;
                }
                else if (Regex.IsMatch(name, Constants.Image + "\\d+"))
                {
                    return Enums.Visual.Image;
                }
                else if (name.Equals(Constants.Disclaimers))
                {
                    return Enums.Visual.SecondaryHeader;
                }
                else if (Regex.IsMatch(name, Constants.Disclaimer + "\\d+"))
                {
                    return Enums.Visual.Text;
                }
                else if (name.Equals(Constants.Measurements))
                {
                    return Enums.Visual.SecondaryHeader;
                }
                else if (name.Equals(Constants.MeasurementHeader))
                {
                    return Enums.Visual.Label;
                }
                else if (name.Equals(Constants.MeasurementInches))
                {
                    return Enums.Visual.Label;
                }
                else if (name.Equals(Constants.MeasurementCentimetres))
                {
                    return Enums.Visual.Label;
                }
                else if (new string[] {
                       Constants.Unit,
                       Constants.PitToPit,
                       Constants.SleeveLength,
                       Constants.Length,
                }.Contains(name))
                {
                    return Enums.Visual.TableHeader;
                }
                else if (new string[] {
                       Constants.inch,
                       Constants.centimetre,
                       Constants.PitToPitIn,
                       Constants.SleeveLengthIn,
                       Constants.LengthIn,
                       Constants.PitToPitCm,
                       Constants.SleeveLengthCm,
                       Constants.LengthCm
                }.Contains(name))
                {
                    return Enums.Visual.TableCell;
                }
                else if (name.Equals(Constants.HasShipping))
                {
                    return Enums.Visual.Label;
                }
                else if (name.Equals(Constants.HasShippingValue))
                {
                    return Enums.Visual.CheckBox;
                }

                throw new Exception("ds3vsdfd d");
            }


            static Enums.VisualLayout containerStyle(string name)
            {
                if (name.Equals(Constants.Title))
                {
                    return Enums.VisualLayout.HeaderedPanel;
                }
                else if (name.Equals(Constants.TitleValue))
                {
                    return Enums.VisualLayout.Content;
                }
                else if (name.Equals(Constants.SubTitle))
                {
                    return Enums.VisualLayout.HeaderedPanel;
                }
                else if (name.Equals(Constants.SubTitleValue))
                {
                    return Enums.VisualLayout.Content;
                }
                else if (name.Equals(Constants.Descriptions))
                {
                    return Enums.VisualLayout.HeaderedPanel;
                }
                else if (Regex.IsMatch(name, Constants.Description + "\\d+"))
                {
                    return Enums.VisualLayout.Content;
                }
                else if (name.Equals(Constants.Images))
                {
                    return Enums.VisualLayout.HeaderedPanel;
                }
                else if (Regex.IsMatch(name, Constants.Image + "\\d+"))
                {
                    return Enums.VisualLayout.Content;
                }
                else if (name.Equals(Constants.Disclaimers))
                {
                    return Enums.VisualLayout.HeaderedPanel;
                }
                else if (Regex.IsMatch(name, Constants.Disclaimer + "\\d+"))
                {
                    return Enums.VisualLayout.Content;
                }
                else if (name.Equals(Constants.Measurements))
                {
                    return Enums.VisualLayout.Table;
                }
                else if (name.Equals(Constants.MeasurementHeader))
                {
                    return Enums.VisualLayout.TableRow;
                }
                else if (name.Equals(Constants.MeasurementInches))
                {
                    return Enums.VisualLayout.TableRow;
                }
                else if (name.Equals(Constants.MeasurementCentimetres))
                {
                    return Enums.VisualLayout.TableRow;
                }
                else if (new string[] {
                       Constants.Unit,
                       Constants.PitToPit,
                       Constants.SleeveLength,
                       Constants.Length,
                       Constants.PitToPitIn,
                }.Contains(name))
                {
                    return Enums.VisualLayout.Content;
                }
                else if (new string[] {
                       Constants.inch,
                       Constants.centimetre,
                       Constants.PitToPitIn,
                       Constants.SleeveLengthIn,
                       Constants.LengthIn,
                       Constants.PitToPitCm,
                       Constants.SleeveLengthCm,
                       Constants.LengthCm
                }.Contains(name))
                {
                    return Enums.VisualLayout.Content;
                }
                else if (name.Equals(Constants.HasShipping))
                {
                    return Enums.VisualLayout.HeaderedPanel;
                }
                else if (name.Equals(Constants.HasShippingValue))
                {
                    return Enums.VisualLayout.Content;
                }

                throw new Exception("ds3vsdfd d");
            }

            static string dataTemplate(string name)
            {
                if (name.Equals(Constants.Title))
                {
                    return default;
                }
                else if (name.Equals(Constants.TitleValue))
                {
                    return default;
                }
                else if (name.Equals(Constants.SubTitle))
                {
                    return default;
                }
                else if (name.Equals(Constants.SubTitleValue))
                {
                    return default;
                }
                else if (name.Equals(Constants.Descriptions))
                {
                    return default;
                }
                else if (Regex.IsMatch(name, Constants.Description + "\\d+"))
                {
                    return default;
                }
                else if (name.Equals(Constants.Images))
                {
                    return default;
                }
                else if (Regex.IsMatch(name, Constants.Image + "\\d+"))
                {
                    return default;
                }
                else if (name.Equals(Constants.Disclaimers))
                {
                    return default;
                }
                else if (Regex.IsMatch(name, Constants.Disclaimer + "\\d+"))
                {
                    return default;
                }
                else if (name.Equals(Constants.Measurements))
                {
                    return default;
                }
                else if (name.Equals(Constants.MeasurementHeader))
                {
                    return default;
                }
                else if (name.Equals(Constants.MeasurementInches))
                {
                    return default;
                }
                else if (name.Equals(Constants.MeasurementCentimetres))
                {
                    return default;
                }
                else if (new string[] {
                       Constants.Unit,
                       Constants.PitToPit,
                       Constants.SleeveLength,
                       Constants.Length,
                       Constants.PitToPitIn,
                }.Contains(name))
                {
                    return default;
                }
                else if (new string[] {
                       Constants.inch,
                       Constants.centimetre,
                       Constants.PitToPitIn,
                       Constants.SleeveLengthIn,
                       Constants.LengthIn,
                       Constants.PitToPitCm,
                       Constants.SleeveLengthCm,
                       Constants.LengthCm
                }.Contains(name))
                {
                    return default;
                }
                else if (name.Equals(Constants.HasShipping))
                {
                    return default;
                }
                else if (name.Equals(Constants.HasShippingValue))
                {
                    return default;
                }

                throw new Exception("ds3vsdfd d");
            }

            static Enums.Orientation orientation(string name)
            {
                if (name == Constants.MeasurementHeader || name == Constants.MeasurementCentimetres || name == Constants.MeasurementInches)
                    return Enums.Orientation.Horizontal;
                if (name == Constants.Images || name == Constants.Descriptions || name == Constants.Measurements || name == Constants.Disclaimers)
                    return Enums.Orientation.Vertical;
                return Enums.Orientation.Horizontal;
            }

            static Type type(string name)
            {
                if (new string[] {
                    Constants.PitToPitIn,
                    Constants.SleeveLengthIn,
                    Constants.LengthIn,
                    Constants.PitToPitCm,
                    Constants.SleeveLengthCm,
                    Constants.LengthCm
                }.Contains(name))
                    return typeof(double);
                else if (name.StartsWith(Constants.HasShipping))
                    return typeof(bool);
                else if (name.EndsWith(Constants.Value))
                    return typeof(string);
                else
                    return typeof(object);
            }
        }


        public IObservable<Utility.Changes.Set<Key>> Find(Guid? parentGuid = default, string? name = null, Guid? guid = null, Type? type = null, int? index = null)
        {
            if (parentGuid.HasValue == false)
                throw new ArgumentNullException(nameof(parentGuid), "Parent Guid cannot be null.");

            (var _parentGuid, var root, var parentName) = dictionary[parentGuid.Value];

            return Observable.Create<Utility.Changes.Set<Key>>(observer =>
            {
                if (guid == null)
                    name = parentName;
                else
                {
                    (var _parentGuid, var root, var name) = dictionary[guid.Value];
                    observer.OnNext(new(Change.Add(new Key(guid.Value, parentGuid.Value, null, name, default, default))));
                    observer.OnCompleted();
                    return Disposable.Empty;
                }

                values[root].Observe(convert(name))
                .Subscribe(a =>
                {
                    replay[(root, name)].OnNext(new DateValue(root, name + "." + "Value", default, a));
                });
                List<Utility.Changes.Change<Key>> changes = new();
                if (name == nameof(AuctionItem))
                {
                    changes.Add(Change.Add(create(Constants.Title)));
                    changes.Add(Change.Add(create(Constants.SubTitle)));
                    changes.Add(Change.Add(create(Constants.Descriptions)));
                    changes.Add(Change.Add(create(Constants.Images)));
                    changes.Add(Change.Add(create(Constants.Measurements)));
                    changes.Add(Change.Add(create(Constants.Disclaimers)));
                    changes.Add(Change.Add(create(Constants.HasShipping)));
                    observer.OnNext(new Utility.Changes.Set<Key>(changes));
                    observer.OnCompleted();
                }
                else if (name == Constants.Title)
                {
                    observer.OnNext(new(Change.Add(create(name + "." + Constants.Value))));
                    observer.OnCompleted();
                }
                else if (name == Constants.SubTitle)
                {
                    observer.OnNext(new(Change.Add(create(name + "." + Constants.Value))));
                    observer.OnCompleted();
                }
                else if (name == Constants.Descriptions)
                {
                    values[root].Where(Constants.Description)
                    .Transform(a => create(a.Name))
                    .Subscribe(a =>
                    {
                        changes.Add(a);
                    },
                    () =>
                    {
                        observer.OnNext(new(changes));
                        observer.OnCompleted();
                    });
                }
                else if (name.StartsWith(Constants.Description))
                {
                    observer.OnNext(new(Change.Add<Key>(create(name + "." + Constants.Value))));
                    observer.OnCompleted();
                }
                else if (name == Constants.Images)
                {
                    values[root].Where(Constants.Image)
                    .Transform(a => create(a.Name))
                    .Subscribe(a =>
                    {
                        changes.Add(a);
                    },
                    () =>
                    {
                        observer.OnNext(new(changes));
                        observer.OnCompleted();
                    });
                }
                else if (name.StartsWith(Constants.Image))
                {
                    observer.OnNext(new(Change.Add(create(name + "." + Constants.Value))));
                    observer.OnCompleted();
                }
                else if (name == Constants.Measurements)
                {
                    changes.Add(Change.Add(create(Constants.MeasurementHeader)));
                    changes.Add(Change.Add(create(Constants.MeasurementCentimetres)));
                    changes.Add(Change.Add(create(Constants.MeasurementInches)));
                    observer.OnNext(new Utility.Changes.Set<Key>(changes));
                    observer.OnCompleted();
                }
                else if (name.StartsWith(Constants.Measurement))
                {
                    values[root]
                    .Observe(nameof(AuctionItem.SleeveLengthInMillimetres))
                    .Subscribe(a =>
                    {
                        replay[(root, Constants.SleeveLengthIn)].OnNext(new DateValue(root, name, default, a));
                        replay[(root, Constants.SleeveLengthCm)].OnNext(new DateValue(root, name, default, a));
                    });
                    values[root]
                    .Observe(nameof(AuctionItem.PitToPitWidthInMillimetres))
                    .Subscribe(a =>
                    {
                        replay[(root, Constants.PitToPitIn)].OnNext(new DateValue(root, name, default, a));
                        replay[(root, Constants.PitToPitCm)].OnNext(new DateValue(root, name, default, a));
                    });
                    values[root]
                    .Observe(nameof(AuctionItem.SleeveLengthInMillimetres))
                    .Subscribe(a =>
                    {
                        replay[(root, Constants.LengthIn)].OnNext(new DateValue(root, name, default, a));
                        replay[(root, Constants.LengthCm)].OnNext(new DateValue(root, name, default, a));
                    });

                    if (name == Constants.MeasurementHeader)
                    {
                        changes.Add(Change.Add(create(Constants.Unit)));
                        changes.Add(Change.Add(create(Constants.PitToPit)));
                        changes.Add(Change.Add(create(Constants.SleeveLength)));
                        changes.Add(Change.Add(create(Constants.Length)));
                        observer.OnNext(new Utility.Changes.Set<Key>(changes));
                        observer.OnCompleted();
                    }
                    else if (name == Constants.MeasurementCentimetres)
                    {
                        changes.Add(Change.Add(create(Constants.inch)));
                        changes.Add(Change.Add(create(Constants.PitToPitIn)));
                        changes.Add(Change.Add(create(Constants.SleeveLengthIn)));
                        changes.Add(Change.Add(create(Constants.LengthIn)));
                        observer.OnNext(new Utility.Changes.Set<Key>(changes));
                        observer.OnCompleted();
                    }
                    else if (name == Constants.MeasurementInches)
                    {
                        changes.Add(Change.Add(create(Constants.centimetre)));
                        changes.Add(Change.Add(create(Constants.PitToPitCm)));
                        changes.Add(Change.Add(create(Constants.SleeveLengthCm)));
                        changes.Add(Change.Add(create(Constants.LengthCm)));
                        observer.OnNext(new Utility.Changes.Set<Key>(changes));
                        observer.OnCompleted();
                    }
                }
                else if (name == Constants.Disclaimers)
                {
                    values[root].Where(Constants.Disclaimer)
                    .Transform(a => create(a.Name))
                    .Subscribe(a =>
                    {
                        changes.Add(a);
                    }, () => observer.OnCompleted());
                }
                else if (name.StartsWith(Constants.Disclaimer))
                {
                    observer.OnNext(new(Change.Add(create(name + "." + Constants.Value))));
                    observer.OnCompleted();
                }
                else if (name == Constants.HasShipping)
                {
                    observer.OnNext(new(Change.Add(create(name + "." + Constants.Value))));
                    observer.OnCompleted();
                }
                else if (name == Constants.inch)
                {
                    observer.OnCompleted();
                }
                else if (name == Constants.centimetre)
                {
                    observer.OnCompleted();
                }
                else if (name == Constants.Unit)
                {
                    observer.OnCompleted();
                }
                else
                    throw new Exception("£G$$DFs33");

                return Disposable.Empty;
            });

            Key create(string name)
            {
                var guid = Guid.NewGuid();
                dictionary.Add(guid, (parentGuid.Value, root, name));
                return new Key(guid, parentGuid.Value, null, name, default, default);
            }
        }

        public IObservable<Key> FindRecursive(Guid parentGuid, int? maxIndex = null)
        {
            throw new NotImplementedException();
        }

        public IObservable<Key?> InsertRoot(Guid guid, string name, Type type)
        {
            return Observable.Return<Key?>(new Key
            {
                Guid = guid,
                Name = name,
                Type = type,
                ParentGuid = Guid.Empty,
                Index = null,
                Removed = null
            });
        }



        public IObservable<DateValue> Get(Guid guid, string? name = null)
        {
            return Observable.Create<DateValue>(observer =>
            {
                observer.OnNext(new DateValue(guid, name, default, get(guid, name)));
                observer.OnCompleted();
                return Disposable.Empty;
                //return (replay[(guid, name)] = new()).Subscribe(observer);
            });
        }

        const double CMtoInchConversionFactory = 0.393701;
        const double MMtoInchConversionFactory = 0.0393701;
        public object get(Guid guid, string? name = null)
        {
            if (name == nameof(IGetValue.Value))
            {
                if (dictionary[guid].root == guid)
                    return null;
                var _value = values[dictionary[guid].root];
                var _name = dictionary[guid].name;

                if (_name == Constants.Title)
                    return Constants.Title;
                if (_name == Constants.Title + "." + Constants.Value)
                    return _value.Get<string>(nameof(AuctionItem.Title));
                if (_name == Constants.SubTitle)
                    return Constants.SubTitle;
                if (_name == Constants.SubTitle + "." + Constants.Value)
                    return _value.Get<string>(nameof(AuctionItem.SubTitle));
                if (_name == Constants.Descriptions)
                    return Constants.Descriptions;
                if (_name == Constants.Images)
                    return Constants.Images;
                if (_name == Constants.Measurements)
                    return Constants.Measurements;
                if (_name == Constants.Disclaimers)
                    return Constants.Disclaimers;
                if (_name == Constants.HasShipping)
                    return Constants.HasShipping;
                if (_name == Constants.HasShipping + "." + Constants.Value)
                    return _value.Get<bool>(nameof(AuctionItem.HasShipping));
                if (_name == Constants.PitToPitCm)
                    return _value.Get<int>(nameof(AuctionItem.PitToPitWidthInMillimetres)) / 10d;
                if (_name == Constants.SleeveLengthCm)
                    return _value.Get<int>(nameof(AuctionItem.SleeveLengthInMillimetres)) / 10d;
                if (_name == Constants.LengthCm)
                    return _value.Get<int>(nameof(AuctionItem.LengthInMillimetres)) / 10d;
                if (_name == Constants.PitToPitIn)
                    return _value.Get<int>(nameof(AuctionItem.PitToPitWidthInMillimetres)) * MMtoInchConversionFactory;
                if (_name == Constants.SleeveLengthIn)
                    return _value.Get<int>(nameof(AuctionItem.SleeveLengthInMillimetres)) * MMtoInchConversionFactory;
                if (_name == Constants.LengthIn)
                    return _value.Get<int>(nameof(AuctionItem.LengthInMillimetres)) * MMtoInchConversionFactory;
                if (Regex.Match(_name, @"([a-zA-Z]+\d+)", RegexOptions.IgnoreCase) is Match { Success: true, Groups: { } groups })
                    return _value.Get<string>(groups[1].Value);
                return _name;
            }
            return get(name);

            static object get(string name)
            {
                return name switch
                {
                    nameof(NodeViewModel.Current) => null,
                    nameof(NodeViewModel.Name) => null,
                    nameof(NodeViewModel.IsActive) => null,
                    nameof(NodeViewModel.IsEditable) => null,
                    nameof(NodeViewModel.IsReadOnly) => null,
                    nameof(NodeViewModel.IsVisible) => null,
                    nameof(NodeViewModel.IsValid) => null,
                    nameof(NodeViewModel.IsHighlighted) => null,
                    nameof(NodeViewModel.IsClicked) => null,
                    nameof(NodeViewModel.IsSelected) => null,
                    nameof(NodeViewModel.IsExpanded) => null,
                    nameof(NodeViewModel.IsReplicable) => null,
                    nameof(NodeViewModel.IsRemovable) => null,
                    nameof(NodeViewModel.IsChildrenRefreshed) => null,
                    nameof(NodeViewModel.Order) => null,
                    nameof(NodeViewModel.Row) => null,
                    nameof(NodeViewModel.Column) => null,
                    nameof(NodeViewModel.Removed) => null,
                    nameof(NodeViewModel.Arrangement) => null,
                    nameof(NodeViewModel.Orientation) => null,
                    nameof(NodeViewModel.ConnectorPosition) => null,
                    nameof(NodeViewModel.Location) => null,
                    nameof(NodeViewModel.Size) => null,
                    nameof(NodeViewModel.DataTemplate) => null,
                    nameof(NodeViewModel.ItemsPanelTemplate) => null,
                    nameof(NodeViewModel.Title) => null,
                    nameof(NodeViewModel.Value) => null,
                    nameof(NodeViewModel.IsEnabled) => null,
                    nameof(NodeViewModel.SelectedItemTemplate) => null,
                    nameof(NodeViewModel.IsWithinWindowBounds) => null,
                    nameof(NodeViewModel.IsProliferable) => null,
                    nameof(NodeViewModel.IsAugmentable) => null,
                    nameof(NodeViewModel.IsLoaded) => null,
                    nameof(NodeViewModel.Style) => null,
                    nameof(NodeViewModel.Layout) => null,
                    nameof(NodeViewModel.BoolValue) => null,
                    nameof(NodeViewModel.ByteValue) => null,
                    _ => throw new ArgumentException($"Unknown field: {name}")
                };
            }
        }

        public void Set(Guid guid, string name, object value, DateTime dateTime)
        {
            if (name == nameof(INodeViewModel.IsProliferable))
            {

            }
            else if (name == nameof(INodeViewModel.IsExpanded))
            {

            }
            else if (name == nameof(INodeViewModel.DataTemplate))
            {

            }
            else
            {
                if (name == nameof(IGetValue.Value))
                {
                    if (dictionary[guid].root == guid)
                        return;
                    var _name = dictionary[guid].name;
                    var _value = values[dictionary[guid].root];
                    if (convert(_name) is string actualName)
                    {
                        var convertedValue = _convertValue(_name, value);
                        if (_value.Get(actualName)?.Equals(convertedValue) == true || convertedValue == null)
                            return;
                        _value.Set(actualName, convertedValue);
                        if (_value.Object is INotifyPropertyChanged changed)
                            changed.RaisePropertyChanged(actualName);
                    }
                    //connection.Update(_value.Object);
                    return;
                }
            }

            static string convert(string _name)
            {
                if (_name == Constants.Title + "." + Constants.Value)
                    return nameof(AuctionItem.Title);
                if (_name == Constants.SubTitle + "." + Constants.Value)
                    return nameof(AuctionItem.SubTitle);
                if (_name == Constants.HasShipping + "." + Constants.Value)
                    return nameof(AuctionItem.HasShipping);
                if (_name == Constants.PitToPitCm)
                    return nameof(AuctionItem.PitToPitWidthInMillimetres);
                if (_name == Constants.SleeveLengthCm)
                    return nameof(AuctionItem.SleeveLengthInMillimetres);
                if (_name == Constants.LengthCm)
                    return nameof(AuctionItem.LengthInMillimetres);
                if (_name == Constants.PitToPitIn)
                    return nameof(AuctionItem.PitToPitWidthInMillimetres);
                if (_name == Constants.SleeveLengthIn)
                    return nameof(AuctionItem.SleeveLengthInMillimetres);
                if (_name == Constants.LengthIn)
                    return nameof(AuctionItem.LengthInMillimetres);
                if (Regex.Match(_name, @"([a-zA-Z]*\d*)\.Value", RegexOptions.IgnoreCase) is Match { Success: true, Groups: { } groups })
                    return groups[1].Value;
                return null;
            }




            static object _convertValue(string _name, object value)
            {
                if (_name == Constants.Title + "." + Constants.Value)
                    return value;
                if (_name == Constants.SubTitle + "." + Constants.Value)
                    return value;
                if (_name == Constants.HasShipping + "." + Constants.Value)
                    return (bool)value;
                if (_name == Constants.PitToPitCm)
                    return (int)((double)value * 10);
                if (_name == Constants.SleeveLengthCm)
                    return (int)((double)value * 10);
                if (_name == Constants.LengthCm)
                    return (int)((double)value * 10);
                if (_name == Constants.PitToPitIn)
                    return (int)((double)value / MMtoInchConversionFactory);
                if (_name == Constants.SleeveLengthIn)
                    return (int)((double)value / MMtoInchConversionFactory);
                if (_name == Constants.LengthIn)
                    return (int)((double)value / MMtoInchConversionFactory);
                if (Regex.Match(_name, @"([a-zA-Z]*\d*)\.Value", RegexOptions.IgnoreCase) is Match { Success: true, Groups: { } groups })
                    return value;
                return null;
            }
        }

        static IEnumerable<string> convertBack(string propertyName)
        {
            if (propertyName == nameof(AuctionItem.Title))
                yield return Constants.Title + "." + Constants.Value;
            if (propertyName == nameof(AuctionItem.SubTitle))
                yield return Constants.SubTitle + "." + Constants.Value;
            if (propertyName == nameof(AuctionItem.HasShipping))
                yield return Constants.HasShipping + "." + Constants.Value;
            if (propertyName == nameof(AuctionItem.PitToPitWidthInMillimetres))
            {
                yield return Constants.PitToPitCm;
                yield return Constants.PitToPitIn;
            }
            if (propertyName == nameof(AuctionItem.SleeveLengthInMillimetres))
            {
                yield return Constants.SleeveLengthCm;
                yield return Constants.SleeveLengthIn;
            }
            if (propertyName == nameof(AuctionItem.LengthInMillimetres))
            {
                yield return Constants.LengthCm;
                yield return Constants.LengthIn;
            }

        }

        static string convert(string _name)
        {
            if (_name == Constants.Title)
                return nameof(AuctionItem.Title) + "." + Constants.Value;
            if (_name == Constants.SubTitle)
                return nameof(AuctionItem.SubTitle) + "." + Constants.Value;
            if (_name == Constants.HasShipping)
                return nameof(AuctionItem.HasShipping) + "." + Constants.Value;
            if (_name == Constants.PitToPit)
                return nameof(AuctionItem.PitToPitWidthInMillimetres);
            if (_name == Constants.SleeveLength)
                return nameof(AuctionItem.SleeveLengthInMillimetres);
            if (_name == Constants.Length)
                return nameof(AuctionItem.LengthInMillimetres);
            if (Regex.Match(_name, @"([a-zA-Z]*\d*)", RegexOptions.IgnoreCase) is Match { Success: true, Groups: { } groups })
                return groups[1].Value;
            return null;
        }

        public IEnumerable<Duplication> Duplicate(Guid oldGuid, Guid? newParentGuid = null)
        {
            throw new NotImplementedException();
        }

        public void Copy(Guid guid, Guid newGuid)
        {
            throw new NotImplementedException();
        }

        public int? MaxIndex(Guid parentGuid, string? name = null)
        {
            throw new NotImplementedException();
        }

        public DateTime Remove(Guid guid)
        {
            throw new NotImplementedException();
        }

        public void Reset()
        {
            throw new NotImplementedException();
        }

        public void UpdateName(Guid parentGuid, Guid guid, string name, string newName)
        {
            throw new NotImplementedException();
        }


        public static TreeRepository Instance { get; } = new TreeRepository();
    }

    public static class ObjectWrapperHelper
    {
        public static IObservable<Changes.Change<PropertyInfoWrapper>> Where(this ObjectWrapper objectWrapper, string name)
        {
            HashSet<object> hash = new();
            return Observable.Create<Changes.Change<PropertyInfoWrapper>>(observer =>
            {
                if (objectWrapper.Object is INotifyPropertyChanged notifyPropertyChanged)
                {
                    reload(observer);
                    return
                    ObservePropertyChanged(notifyPropertyChanged)
                    .Subscribe(a =>
                    {
                        reload(observer);
                    }, () => observer.OnCompleted());
                }
                throw new Exception("RF£DF DDd");
            });

            void reload(IObserver<Changes.Change<PropertyInfoWrapper>> observer)
            {
                var x = objectWrapper._propertyCache.Where(a => a.Key.StartsWith(name))
                       .Select(a => (a.Value, a.Value.GetValue()))
                       .OrderByDescending(a => a.Item1 != default)
                       .ToArray();
                var count = x.Count(c => c.Item2 != default);
                for (int i = 0; i < x.Length; i++)
                {
                    var value = x[i].Value;
                    if (i <= count)
                    {
                        if (hash.Add(value))
                            observer.OnNext(Changes.Change.Add(value));
                    }
                    else if (hash.Contains(x))
                    {
                        hash.Remove(x);
                        observer.OnNext(Changes.Change.Remove(value));
                    }
                }
            }
        }

        public static IObservable<PropertyChangedEventArgs> ObservePropertyChanged(INotifyPropertyChanged notifyPropertyChanged)
        {
            return Observable.FromEvent<PropertyChangedEventHandler, PropertyChangedEventArgs>(
                handler => (sender, args) => handler(args),
                handler => notifyPropertyChanged.PropertyChanged += handler,
                handler => notifyPropertyChanged.PropertyChanged -= handler);
        }

        public static IObservable<object> Observe(this ObjectWrapper wrapper, string name)
        {
            if (wrapper._propertyCache.ContainsKey(name) == false)
                return Observable.Empty<object>();
            if (wrapper.Object is INotifyPropertyChanged changed)
                return changed.WithChangesTo(wrapper._propertyCache[name]._propertyInfo, includeInitialValue: false);
            else
                throw new Exception("VRE£Dfd");
        }
        public static IObservable<Changes.Change<TR>> Transform<T, TR>(this IObservable<Changes.Change<T>> changes, Func<T, TR> transform)
        {
            return Observable.Create<Changes.Change<TR>>(observer =>
            {
                return changes.Subscribe(a =>
                {
                    observer.OnNext(new Changes.Change<TR>(transform(a.Value), a.Type));
                }, () => observer.OnCompleted());
            });
        }

    }
}
