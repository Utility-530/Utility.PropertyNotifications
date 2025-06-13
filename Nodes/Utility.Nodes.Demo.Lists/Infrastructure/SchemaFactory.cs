using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Utility.Interfaces.NonGeneric;
using Utility.Models;

namespace Utility.Nodes.Demo.Lists.Infrastructure
{
    internal class SchemaFactory
    {

        public static Schema EbaySchema => new()
        {
            Properties =
                [
                new SchemaProperty() { Name = nameof(EbayModel.Disclaimers), IsVisible = false },
                new SchemaProperty() { Name = nameof(EbayModel.Descriptions), IsVisible = false },
                new SchemaProperty() { Name = nameof(EbayModel.HasImagePaths), IsVisible = false },
                new SchemaProperty() { Name = nameof(EbayModel.ImagePaths), IsVisible = false },
                new SchemaProperty() { Name = nameof(EbayModel.HasDescriptions), IsVisible = false },
                new SchemaProperty() { Name = nameof(EbayModel.HasDisclaimers), IsVisible = false },
                new SchemaProperty() { Name = nameof(EbayModel.HasMeasurements), IsVisible = false },
                new SchemaProperty() { Name = nameof(EbayModel.MissedCalls), IsVisible = false },
                new SchemaProperty() { Name = nameof(EbayModel.HasTitle), IsVisible = false },

                ]
        };
}
}
