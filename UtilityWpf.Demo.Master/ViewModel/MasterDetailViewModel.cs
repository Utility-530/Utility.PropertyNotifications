using System.Collections;
using System.Collections.Generic;
using Utility.Service;
using Utility.WPF.Demo.Common.ViewModels;
using UtilityWpf.Demo.Data.Factory;
using UtilityWpf.Demo.Master.Infrastructure;
using mdvm = Utility.ViewModels.MasterDetailViewModel;

namespace UtilityWpf.Demo.Master.ViewModels
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