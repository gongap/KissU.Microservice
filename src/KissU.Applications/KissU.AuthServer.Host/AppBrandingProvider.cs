﻿using Volo.Abp.AspNetCore.Mvc.UI.Theme.Shared.Components;
using Volo.Abp.DependencyInjection;

namespace KissU.AuthServer.Host
{
    public class AppBrandingProvider : DefaultBrandingProvider, ISingletonDependency
    {
        public override string AppName => "Identity Server";
    }
}