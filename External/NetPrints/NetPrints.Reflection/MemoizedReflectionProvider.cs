//using System;
//using System.Collections.Generic;
//using NetPrints.Core;
//using NetPrints.Interfaces;

//namespace NetPrintsEditor.Reflection
//{
//    public class MemoizedReflectionProvider : IReflectionProvider
//    {
//        private readonly IReflectionProvider provider;

//        private Func<ITypeSpecifier, IEnumerable<IConstructorSpecifier>> memoizedGetConstructors;
//        private Func<ITypeSpecifier, IEnumerable<string>> memoizedGetEnumNames;
//        private Func<IMethodSpecifier, string> memoizedGetMethodDocumentation;
//        private Func<IMethodSpecifier, int, string> memoizedGetMethodParameterDocumentation;
//        private Func<IMethodSpecifier, int, string> memoizedGetMethodReturnDocumentation;
//        private Func<IEnumerable<ITypeSpecifier>> memoizedGetNonStaticTypes;
//        private Func<ITypeSpecifier, IEnumerable<IMethodSpecifier>> memoizedGetOverridableMethodsForType;
//        private Func<IMethodSpecifier, IEnumerable<IMethodSpecifier>> memoizedGetPublicMethodOverloads;
//        private Func<ITypeSpecifier, ITypeSpecifier, bool> memoizedHasImplicitCast;
//        private Func<ITypeSpecifier, ITypeSpecifier, bool> memoizedTypeSpecifierIsSubclassOf;
//        private Func<IReflectionProviderMethodQuery, IEnumerable<IMethodSpecifier>> memoizedGetMethods;
//        private Func<IReflectionProviderVariableQuery, IEnumerable<IVariableSpecifier>> memoizedGetVariables;

//        public MemoizedReflectionProvider(IReflectionProvider reflectionProvider)
//        {
//            provider = reflectionProvider;

//            Reset();
//        }

//        /// <summary>
//        /// Resets the memoization.
//        /// </summary>
//        public void Reset()
//        {
//            memoizedGetConstructors = provider.GetConstructors;
//            memoizedGetConstructors = memoizedGetConstructors.Memoize();

//            memoizedGetEnumNames = provider.GetEnumNames;
//            memoizedGetEnumNames = memoizedGetEnumNames.Memoize();

//            memoizedGetMethodDocumentation = provider.GetMethodDocumentation;
//            memoizedGetMethodDocumentation = memoizedGetMethodDocumentation.Memoize();

//            memoizedGetMethodParameterDocumentation = provider.GetMethodParameterDocumentation;
//            memoizedGetMethodParameterDocumentation = memoizedGetMethodParameterDocumentation.Memoize();

//            memoizedGetMethodReturnDocumentation = provider.GetMethodReturnDocumentation;
//            memoizedGetMethodReturnDocumentation = memoizedGetMethodReturnDocumentation.Memoize();

//            memoizedGetNonStaticTypes = provider.GetNonStaticTypes;
//            memoizedGetNonStaticTypes = memoizedGetNonStaticTypes.Memoize();

//            memoizedGetOverridableMethodsForType = provider.GetOverridableMethodsForType;
//            memoizedGetOverridableMethodsForType = memoizedGetOverridableMethodsForType.Memoize();

//            memoizedGetMethods = provider.GetMethods;
//            memoizedGetMethods = memoizedGetMethods.Memoize();

//            memoizedGetPublicMethodOverloads = provider.GetPublicMethodOverloads;
//            memoizedGetPublicMethodOverloads = memoizedGetPublicMethodOverloads.Memoize();

//            memoizedGetVariables = provider.GetVariables;
//            memoizedGetVariables = memoizedGetVariables.Memoize();

//            memoizedHasImplicitCast = provider.HasImplicitCast;
//            memoizedHasImplicitCast = memoizedHasImplicitCast.Memoize();

//            memoizedTypeSpecifierIsSubclassOf = provider.TypeSpecifierIsSubclassOf;
//            memoizedTypeSpecifierIsSubclassOf = memoizedTypeSpecifierIsSubclassOf.Memoize();
//        }

//        public IEnumerable<IConstructorSpecifier> GetConstructors(ITypeSpecifier typeSpecifier)
//            => memoizedGetConstructors(typeSpecifier);

//        public IEnumerable<string> GetEnumNames(ITypeSpecifier typeSpecifier)
//            => memoizedGetEnumNames(typeSpecifier);

//        public string GetMethodDocumentation(IMethodSpecifier methodSpecifier)
//            => memoizedGetMethodDocumentation(methodSpecifier);

//        public string GetMethodParameterDocumentation(IMethodSpecifier methodSpecifier, int parameterIndex)
//            => memoizedGetMethodParameterDocumentation(methodSpecifier, parameterIndex);

//        public string GetMethodReturnDocumentation(IMethodSpecifier methodSpecifier, int returnIndex)
//            => memoizedGetMethodReturnDocumentation(methodSpecifier, returnIndex);

//        public IEnumerable<ITypeSpecifier> GetNonStaticTypes()
//            => memoizedGetNonStaticTypes();

//        public IEnumerable<IMethodSpecifier> GetOverridableMethodsForType(ITypeSpecifier typeSpecifier)
//            => memoizedGetOverridableMethodsForType(typeSpecifier);

//        public IEnumerable<IMethodSpecifier> GetPublicMethodOverloads(IMethodSpecifier methodSpecifier)
//            => memoizedGetPublicMethodOverloads(methodSpecifier);

//        public IEnumerable<IMethodSpecifier> GetMethods(IReflectionProviderMethodQuery query)
//            => memoizedGetMethods(query);

//        public IEnumerable<IVariableSpecifier> GetVariables(IReflectionProviderVariableQuery query)
//            => memoizedGetVariables(query);

//        public bool HasImplicitCast(ITypeSpecifier fromType, ITypeSpecifier toType)
//            => memoizedHasImplicitCast(fromType, toType);

//        public bool TypeSpecifierIsSubclassOf(ITypeSpecifier a, ITypeSpecifier b)
//            => memoizedTypeSpecifierIsSubclassOf(a, b);
//    }
//}
