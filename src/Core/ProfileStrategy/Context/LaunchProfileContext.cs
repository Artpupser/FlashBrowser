using FlashBrowser.Services.Browser;

namespace FlashBrowser.Core.ProfileStrategy.Context;

public sealed class LaunchProfileContext(List<LaunchProfile> profiles)
   : ProfileContext<LaunchProfile>(profiles), IProfileContext<LaunchProfile>;