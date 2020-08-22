using JetBrains.Annotations;
using Volo.Abp.EntityFrameworkCore.Modeling;

namespace KissU.Modules.Blogging.EntityFrameworkCore
{
    public class BloggingModelBuilderConfigurationOptions : AbpModelBuilderConfigurationOptions
    {
        public BloggingModelBuilderConfigurationOptions(
            [NotNull] string tablePrefix = "",
            [CanBeNull] string schema = null)
            : base(tablePrefix, schema)
        {
        }
    }
}