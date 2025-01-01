using Example;
using System.Collections;
using System.Globalization;
using System.Windows;
using System.Windows.Data;
using TreeView.Infrastructure;
using Utility.Nodes.Filters;

namespace Utility.Nodes.Demo.Filters
{


    public class ChildrenSelector : IChildrenSelector
    {

        public IEnumerable Select(object data)
        {
            if (data is Node { Data: { } model } node)
                switch (model)
                {
                    //case GlobalAssembliesModel:
                    //    yield return new AssemblyModel { Assembly = typeof(IndexModel).Assembly, Name = typeof(IndexModel).Assembly.GetName().Name };
                    //    break;
                    //case ResolvableModel resModel:
                    //    yield return new GlobalAssembliesModel { };
                    //    break;
                    //case AssemblyModel assModel:
                    //    yield return new TypeModel { Type = typeof(IndexModel) };
                    //    break;
                    //case TypeModel typeModel:
                    //    yield return new PropertyModel { PropertyInfo = typeof(IndexModel).GetProperty(nameof(IndexModel.Value)) };
                    //    break;
                    //case PropertyModel propertyModel:
                    //    yield return new ValueModel { Value = GetDefault(propertyModel.PropertyInfo.PropertyType) };
                    //    break;
                    //case FilterModel:
                    //    yield return new ResolvableModel { Name = typeof(IndexModel).Assembly.GetName().Name, Assemblies = [typeof(IndexModel).Assembly], Types = [typeof(IndexModel)], };
                    //    yield return new RelationshipModel { };
                    //    yield return new BooleanModel { };
                    //    break;
                    //case TransformersModel:
                    //    yield return new TransformerModel { Name = "transformer" };
                    //    break;      
                    //case NodePropertyRootsModel:
                    //    yield return new NodePropertyRootModel { Name = "n_p_r" };
                    //    break;
                    //case IndexModel:
                    //case Model:
                    //    yield return new FilterModel { Name = "Filter" };
                    //    yield return new AndOrModel { };
                    //    break;
                    
                    default:
                            throw new NotImplementedException();
                }
            throw new Exception("dd333111");
        }

        //public static object GetDefault(Type type)
        //{
        //    if (type.IsValueType)
        //    {
        //        return Activator.CreateInstance(type);
        //    }
        //    return null;
        //}
    }

    public class ChildrenSelectorConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            var childrenSelector = values.SingleOrDefault(a => a is IChildrenSelector);
            var items = values.SingleOrDefault(a => a is IEnumerable);
            var node = values.SingleOrDefault(a => a is Utility.Nodes.Filters.Node);

            if (node != null)
            {
                if (node is Utility.Nodes.Filters.Node { Data: IProliferation data })
                {
                    return data.Proliferation();
                }
                if (childrenSelector is IChildrenSelector selector)
                {
                    return selector.Select(node);
                }
            }
            if (items is IEnumerable)
            {
                return items;
            }

            return DependencyProperty.UnsetValue;
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
