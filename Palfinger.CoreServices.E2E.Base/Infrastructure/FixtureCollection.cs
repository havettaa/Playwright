using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace Palfinger.CoreServices.E2E.Base.Infrastructure
{
    [CollectionDefinition("UITests collection")]
    public class FixtureCollection : ICollectionFixture<FixtureServiceProvider>
    {
        // This class has no code, and is never created. Its purpose is simply
        // to be the place to apply [CollectionDefinition] and all the
        // ICollectionFixture<> interfaces.
    }
}
