using System;
using System.Collections.Generic;
using System.Linq;
using EloBuddy;
using EloBuddy.SDK;
using EloBuddy.SDK.Constants;
using EloBuddy.SDK.Events;
using EloBuddy.SDK.Menu;
using EloBuddy.SDK.Menu.Values;

namespace SpellNameGetter
{
    class Program
    {
        private static Menu menu;
        static void Main(string[] args)
        {
            Loading.OnLoadingComplete += eventArgs =>
            {
                menu = MainMenu.AddMenu("SpellNameGetter", "spelaepsfdg");
                menu.AddGroupLabel("Moon Walk Evade Extension");

                List<string> heroCache = new List<string>(10);
                foreach (var hero in EntityManager.Heroes.AllHeroes)
                {
                    if (heroCache.Contains(hero.ChampionName))
                        continue;

                    heroCache.Add(hero.ChampionName);
                    menu.Add(hero.ChampionName, new CheckBox("Track the Spells of " + hero.ChampionName, false));
                }
                menu.AddSeparator();
            };
            GameObject.OnCreate += GameObjectOnOnCreate;
            AIHeroClient.OnProcessSpellCast += AiHeroClientOnOnProcessSpellCast;
        }

        private static void AiHeroClientOnOnProcessSpellCast(Obj_AI_Base sender, GameObjectProcessSpellCastEventArgs args)
        {
            // ReSharper disable once UseNullPropagation
            if (!(sender is AIHeroClient))
                return;

            var caster = (AIHeroClient)sender;
            if (!GetHeroNames().Contains(caster.ChampionName))
                return;

            if (!args.IsAutoAttack())
                Chat.Print("Skillshot Name Detected: " + args.SData.Name);
        }

        static IEnumerable<string> GetHeroNames()
        {
            return (from h in EntityManager.Heroes.AllHeroes where menu[h.ChampionName].Cast<CheckBox>().CurrentValue select h.ChampionName).ToList();
        }

        private static void GameObjectOnOnCreate(GameObject sender, EventArgs args)
        {
            // ReSharper disable once UseNullPropagation
            if (!(sender is MissileClient))
                return;

            var mis = (MissileClient) sender;
            if (!(mis.SpellCaster is AIHeroClient))
                return;

            var caster = (AIHeroClient)mis.SpellCaster;
            if (!GetHeroNames().Contains(caster.ChampionName))
                return;

            if (!mis.SData.IsAutoAttack())
                Chat.Print("Game Object Name Detected: " + mis.SData.Name);
        }
    }
}
