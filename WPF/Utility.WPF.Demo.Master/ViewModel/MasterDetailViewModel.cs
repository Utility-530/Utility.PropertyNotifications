using System.Collections;
using System.Collections.Generic;
using Utility.Service;
using Utility.WPF.Demo.Common.ViewModels;
using Utility.WPF.Demo.Data.Factory;
using Utility.WPF.Demo.Master.Infrastructure;
using mdvm = Utility.ViewModels.MasterDetailViewModel;

namespace Utility.WPF.Demo.Master.ViewModels
{
    public class MasterDetailViewModel : mdvm
    {
        private IEnumerator<Fields> build;

        private static FieldsFactory Factory() => new();

        private static readonly CollectionService CollectionService = new();

        public MasterDetailViewModel() : base(CollectionService, new MockDatabaseService())
        {
        }

        public override IEnumerator NewItem => build ??= Factory().Build();
    }
}