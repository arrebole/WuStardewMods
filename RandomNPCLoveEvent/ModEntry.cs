using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using StardewValley.GameData.Characters;
using StardewValley.SpecialOrders.Objectives;

namespace RandomNPCLoveEvent
{
    public class ModEntry : Mod
    {
        public override void Entry(IModHelper helper)
        {
            helper.Events.GameLoop.SaveLoaded += OnSaveLoaded;
            helper.Events.GameLoop.DayStarted += OnDayStarted;
        }

        private void OnSaveLoaded(object sender, SaveLoadedEventArgs e)
        {
            // 初始化或加载随机事件配置
            InitializeRandomEvents();
        }

        private void OnDayStarted(object sender, DayStartedEventArgs e)
        {
            // 每天检查是否可以触发随机事件
            // CheckForRandomEvents();
        }

        void InitializeRandomEvents()
        {
            
        }
    }
}