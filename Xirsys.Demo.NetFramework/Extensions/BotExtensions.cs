using System;
using AIMLbot;

namespace Xirsys.Demo.NetFramework.Extensions
{
    public static class BotExtensions
    {
        public static String BotName(this Bot bot)
        {
            return bot.GlobalSettings.grabSetting("name");
        }
    }
}
