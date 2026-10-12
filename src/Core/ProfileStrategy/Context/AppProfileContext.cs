using FlashBrowser.Services.Browser;

namespace FlashBrowser.Core.ProfileStrategy.Context;

public sealed class AppProfileContext(List<AppProfile> profiles)
   : ProfileContext<AppProfile>(profiles), IProfileContext<AppProfile>;