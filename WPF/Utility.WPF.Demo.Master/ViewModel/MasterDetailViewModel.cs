using System.Collections;
using System.Collections.Generic;
using Utility.Services;
using Utility.WPF.Demo.Common.ViewModels;
using Utility.WPF.Demo.Data.Factory;
using Utility.WPF.Demo.Master.Infrastructure;

namespace Utility.WPF.Demo.Master.ViewModels
{
    public class MasterDetailViewModel : Utility.ViewModels.MasterDetailViewModel
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