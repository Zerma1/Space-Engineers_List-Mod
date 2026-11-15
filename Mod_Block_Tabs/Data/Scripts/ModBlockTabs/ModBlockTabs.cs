using System.Collections.Generic;
using Sandbox.Definitions;
using Sandbox.ModAPI;
using VRage.Game;
using VRage.Game.Components;
using VRage.Utils;

namespace Zerma.ModBlockTabs
{
    // Adds one tab per loaded mod to the G menu (toolbar config screen), listing every block of that mod.
    // The G screen reads MyDefinitionManager.GetCategories() each time it opens, so adding entries
    // to that dictionary once the session has loaded is enough. Blocks keep their original tabs too.
    [MySessionComponentDescriptor(MyUpdateOrder.NoUpdate)]
    public class ModBlockTabsSession : MySessionComponentBase
    {
        const string CategoryPrefix = "ZermaModBlockTabs_";
        const string DisplayPrefix = "[MOD] ";

        readonly List<string> addedCategories = new List<string>();

        public override void BeforeStart()
        {
            // the dedicated server has no G menu
            if (MyAPIGateway.Utilities.IsDedicated)
                return;

            var categories = MyDefinitionManager.Static.GetCategories();
            var blocksByMod = new Dictionary<string, HashSet<string>>();

            foreach (var def in MyDefinitionManager.Static.GetAllDefinitions())
            {
                var block = def as MyCubeBlockDefinition;
                if (block == null || !block.Public || block.Context == null || block.Context.IsBaseGame)
                    continue;

                var modName = GetModName(block.Context);
                HashSet<string> items;
                if (!blocksByMod.TryGetValue(modName, out items))
                {
                    items = new HashSet<string>();
                    blocksByMod.Add(modName, items);
                }
                // same "TypeId/SubtypeId" form as BlockCategories.sbc (HasItem strips "MyObjectBuilder_")
                items.Add(block.Id.ToString().Replace("MyObjectBuilder_", ""));
            }

            foreach (var mod in blocksByMod)
            {
                var name = CategoryPrefix + mod.Key;
                var category = new MyGuiBlockCategoryDefinition();
                category.Id = new MyDefinitionId(typeof(MyObjectBuilder_GuiBlockCategoryDefinition), name);
                category.Name = name;
                category.DisplayNameString = DisplayPrefix + mod.Key;
                category.ItemIds = mod.Value;
                category.IsBlockCategory = true;
                category.SearchBlocks = true;
                category.ShowInCreative = true;
                category.Public = true;
                category.Enabled = true;
                category.AvailableInSurvival = true;

                categories[name] = category;
                addedCategories.Add(name);
            }

            MyLog.Default.WriteLineAndConsole(string.Format("Mod Block Tabs: {0} mod tab(s) added to the G menu", addedCategories.Count));
        }

        protected override void UnloadData()
        {
            // definitions can be kept between sessions: do not leave tabs of mods the next world does not use
            var categories = MyDefinitionManager.Static.GetCategories();
            foreach (var name in addedCategories)
                categories.Remove(name);
            addedCategories.Clear();
        }

        static string GetModName(MyModContext context)
        {
            var name = context.ModItem.FriendlyName;
            if (string.IsNullOrWhiteSpace(name))
                name = context.ModName;
            if (string.IsNullOrWhiteSpace(name))
                name = context.ModId;
            return string.IsNullOrWhiteSpace(name) ? "?" : name.Trim();
        }
    }
}
