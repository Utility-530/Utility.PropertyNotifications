using DynamicData;
using ReactiveUI;
using System;
using System.Collections;
using System.Collections.ObjectModel;
using System.Linq;
using System.Linq.Expressions;
using System.Reactive;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Reflection;
using Utility.Reactives;

namespace Utility.Models.Filters
{
    public abstract class PropertyFilter<T, TProperty> : PropertyFilter<T>
    {
        public PropertyFilter(Expression<Func<T, TProperty>> expression) : base(expression.PropertyInfo())
        {
        }
    }

    public abstract class PropertyFilter<T> : SubjectFilter<T>
    {
        private readonly PropertyInfo propertyInfo;
        private readonly IConnectableObservable<Unit> selections;
        private readonly Subject<IChangeSet<T>> subjects = new();
        private readonly ReadOnlyObservableCollection<object> collection;

        public PropertyFilter(string property) : this(typeof(T)
                .GetProperties()
                .Single(a => a.Name == property))
        {
        }

        public PropertyFilter(PropertyInfo propertyInfo) : base(propertyInfo.Name)
        {
            _ = subjects
                .GroupOn(a => propertyInfo.GetValue(a) ?? throw new Exception(" ___ ER43333 gggggg"))
                .Transform(a => a.GroupKey ?? throw new Exception("ER43 gggggg"))
                .Bind(out collection)
                .Subscribe();

            //selections = this.Value
            //      .Select(a => Unit.Default)
            //      .Replay(1);

            this.propertyInfo = propertyInfo;
        }

        public override bool Evaluate(object value)
        {
            return propertyInfo.GetValue(value).Equals(Value.Value);
        }

        public override ReactiveProperty<object> Value { get; }

        public IEnumerable Values => collection;

        public override void OnNext(IChangeSet<T> value)
        {
            subjects.OnNext(value);
        }

        public override IDisposable Subscribe(IObserver<Unit> observer)
        {
            CompositeDisposable disposable = new();
            selections?.Connect().DisposeWith(disposable);
            return selections?
                .Subscribe(observer)
                .DisposeWith(disposable)?? Disposable.Empty;
        }

        //public static PT Create<PT, TR>(Expression<Func<T, TR>> expression) where PT:PropertyFilter<TR>, new()
        //{
        //    return new PropertyFilter<T>(expression.PropertyInfo());
        //}

        protected virtual string Get(object selectedValue)
        {
            return selectedValue.ToString() ?? throw new Exception("2gfd gfdd444");
        }

        protected abstract object Set(string value);
    }

    public static class PropertyHelper
    {
        public static PropertyInfo PropertyInfo<TSource, TProperty>(this Expression<Func<TSource, TProperty>> propertyLambda)
        {
            return PropertyInfo(typeof(TSource), propertyLambda);
        }

        public static PropertyInfo PropertyInfo<TProperty>(object source, Expression<Func<object, TProperty>> propertyLambda)
        {
            return PropertyInfo(source.GetType(), propertyLambda);
        }

        /// <summary>
        /// <a href="https://stackoverflow.com/questions/671968/retrieving-property-name-from-lambda-expression"></a>
        /// </summary>
        public static PropertyInfo PropertyInfo<T>(Type type, Expression<T> propertyLambda)
        {
            const string ExceptionOne = "Expression '{0}' refers to a method, not a property.";
            const string ExceptionTwo = "Expression '{0}' refers to a field, not a property.";
            const string ExceptionThree = "Expression '{0}' refers to a property that is not from type {1}.";

            if (propertyLambda.Body is not MemberExpression member)
                throw new ArgumentException(string.Format(ExceptionOne, propertyLambda.ToString()));

            if (member.Member is not PropertyInfo propInfo)
                throw new ArgumentException(string.Format(ExceptionTwo, propertyLambda.ToString()));

            if (type != propInfo.ReflectedType &&
                propInfo.ReflectedType is not null &&
                !type.IsSubclassOf(propInfo.ReflectedType) &&
                !propInfo.ReflectedType.IsAssignableFrom(type))
                throw new ArgumentException(string.Format(ExceptionThree, propertyLambda.ToString(), type));

            return propInfo;
        }
    }
}