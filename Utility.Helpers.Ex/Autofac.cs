using Autofac;
using Autofac.Builder;

namespace Utility.Helpers.Ex
{
    public static class AutofacHelper
    {

        public static void RegisterSelf(this ContainerBuilder builder)
        {
            IContainer container = null;
            builder.Register(c => container).AsSelf().SingleInstance();
            builder.RegisterBuildCallback(c => container = (IContainer)c);
        }


        //        public static IRegistrationBuilder<TR, IConcreteActivatorData, SingleRegistrationStyle> WithOptions<TR, TY>(
        //this IRegistrationBuilder<TR, IConcreteActivatorData, TY> builder,
        //bool autoActivate = false,
        //bool asSelf = true,
        //bool asImplementedInterfaces = true,
        //bool singleInstance = true,
        //bool watch = true)
        //where TY : SingleRegistrationStyle
        //where TR : class
        //        {

        //            //if (autoActivate)
        //            //{
        //            //    builder.AutoActivate();
        //            //}
        //            //builder.Watch();
        //            return builder.AsSelf().AsImplementedInterfaces().SingleInstance();
        //        }

        //        public static IRegistrationBuilder<TR, IConcreteActivatorData, SingleRegistrationStyle> Watch<TR, TY>(
        //      this IRegistrationBuilder<TR, IConcreteActivatorData, TY> builder, bool autoActivate = true)
        //      where TY : SingleRegistrationStyle
        //      where TR : class
        //        {



        //            //builder
        //            //    .OnActivated(a =>
        //            //    {
        //            //        if ((a.Instance ?? throw new Exception("Instance is null")) is INode node)
        //            //        {
        //            //            var graphExample = a.Context.Resolve<GraphExample>();   
        //            //            graphExample.BuildVertex(new GraphX.Measure.Point(0,0), node.Key);
        //            //        }
        //            //    });

        //            return builder;
        //        }
    }
}
