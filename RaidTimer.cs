// Plugin: RaidTimer 3.1 - Raideo con detección automática de base y GUI simplificada
// Autor: ChatGPT para usuario Rust  (versión con detección automática de radio)

using System;
using System.Collections.Generic;
using System.Linq;
using Oxide.Core;
using Oxide.Game.Rust.Cui;
using UnityEngine;

namespace Oxide.Plugins
{
    [Info("RaidTimer", "TuNombre", "3.1.0")]
    [Description("Temporizador de raideo con detección automática de base, zona protegida y GUI de selección")]

    public class RaidTimer : RustPlugin
    {
        private List<ulong> raidPlayers = new();
        private Vector3 raidCenter;
        private float raidRadius = 20f;
        private int remainingTime;
        private Timer raidTimer;
        private bool raidActive = false;

        private const string HUD_NAME = "RaidHUD";

        [ChatCommand("raidstart")]
        private void StartRaidSelection(BasePlayer player)
        {
            raidPlayers.Clear();
            ShowPlayerSelectionGUI(player);
        }

        [ChatCommand("raidmark")]
        private void CmdRaidMark(BasePlayer player)
        {
            if (!raidPlayers.Contains(player.userID))
            {
                SendReply(player, "❌ No estás autorizado a marcar la zona.");
                return;
            }

            // Detección automática de base mediante Tool Cupboard
            var tc = GetNearestTC(player.transform.position);
            if (tc == null)
            {
                SendReply(player, "⚠️ No se encontró ningún Tool Cupboard cerca.");
                return;
            }

            var building = tc.GetBuilding();
            if (building?.buildingBlocks == null || building.buildingBlocks.Count == 0)
            {
                SendReply(player, "⚠️ El TC no tiene bloques asociados.");
                return;
            }

            Bounds bounds = new Bounds(tc.transform.position, Vector3.zero);
            foreach (var block in building.buildingBlocks)
            {
                if (block != null)
                    bounds.Encapsulate(block.transform.position);
            }

            raidCenter = bounds.center;
            raidRadius = Mathf.Max(bounds.extents.magnitude, 5f); // mínimo 5 m
            SendReply(player, $"✅ Base detectada. Centro: {raidCenter}. Radio: {raidRadius:F1} m.");
        }

        /// <summary>Obtiene el Tool Cupboard más cercano en 30 m.</summary>
        private BuildingPrivlidge GetNearestTC(Vector3 pos)
        {
            return BaseNetworkable.serverEntities
                .OfType<BuildingPrivlidge>()
                .Where(tc => Vector3.Distance(pos, tc.transform.position) < 30f)
                .OrderBy(tc => Vector3.Distance(pos, tc.transform.position))
                .FirstOrDefault();
        }

        private void ShowPlayerSelectionGUI(BasePlayer player)
        {
            var container = new CuiElementContainer();
            string panel = container.Add(new CuiPanel { Image = { Color = "0.1 0.1 0.1 0.9" }, RectTransform = { AnchorMin = "0.2 0.2", AnchorMax = "0.8 0.8" }, CursorEnabled = true }, "Overlay", "RaidPlayerMenu");

            container.Add(new CuiLabel { Text = { Text = "Selecciona jugadores para el raideo:", FontSize = 18, Align = TextAnchor.MiddleCenter, Color = "1 1 1 1" }, RectTransform = { AnchorMin = "0.1 0.9", AnchorMax = "0.9 1" } }, panel);

            int index = 0;
            foreach (var target in BasePlayer.activePlayerList)
            {
                float yMin = 0.8f - index * 0.07f;
                float yMax = yMin + 0.06f;
                container.Add(new CuiButton
                {
                    Button = { Command = $"raid.addplayer {target.userID}", Color = "0.2 0.6 0.2 0.8" },
                    Text = { Text = target.displayName, FontSize = 14, Align = TextAnchor.MiddleCenter, Color = "1 1 1 1" },
                    RectTransform = { AnchorMin = $"0.1 {yMin}", AnchorMax = $"0.9 {yMax}" }
                }, panel);
                index++; if (index > 8) break;
            }

            container.Add(new CuiButton { Button = { Command = "raid.asktime", Color = "0.3 0.3 0.8 0.8" }, Text = { Text = "Continuar", FontSize = 16, Align = TextAnchor.MiddleCenter, Color = "1 1 1 1" }, RectTransform = { AnchorMin = "0.1 0.05", AnchorMax = "0.45 0.12" } }, panel);
            container.Add(new CuiButton { Button = { Command = "raid.close RaidPlayerMenu", Color = "0.8 0.3 0.3 0.8" }, Text = { Text = "Cerrar", FontSize = 16, Align = TextAnchor.MiddleCenter, Color = "1 1 1 1" }, RectTransform = { AnchorMin = "0.55 0.05", AnchorMax = "0.9 0.12" } }, panel);
            CuiHelper.AddUi(player, container);
        }

        [ConsoleCommand("raid.addplayer")]
        private void CmdAddPlayer(ConsoleSystem.Arg arg)
        {
            BasePlayer caller = arg.Player();
            if (caller == null || arg.Args.Length == 0) return;
            if (ulong.TryParse(arg.Args[0], out ulong id) && !raidPlayers.Contains(id))
            {
                raidPlayers.Add(id);
                SendReply(caller, $"✅ {BasePlayer.FindByID(id)?.displayName} agregado.");
            }
        }

        [ConsoleCommand("raid.asktime")]
        private void CmdAskTime(ConsoleSystem.Arg arg)
        {
            BasePlayer player = arg.Player();
            if (player == null) return;

            if (raidCenter == Vector3.zero)
            {
                SendReply(player, "⚠️ Debes marcar primero la zona con /raidmark.");
                return;
            }

            CuiHelper.DestroyUi(player, "RaidPlayerMenu");
            CuiHelper.DestroyUi(player, "RaidTimeInput");

            var container = new CuiElementContainer();
            string panel = container.Add(new CuiPanel { Image = { Color = "0.1 0.1 0.1 0.95" }, RectTransform = { AnchorMin = "0.4 0.4", AnchorMax = "0.6 0.6" }, CursorEnabled = true }, "Overlay", "RaidTimeInput");

            container.Add(new CuiLabel { Text = { Text = "Ingresa el tiempo del raideo (segundos):", FontSize = 14, Align = TextAnchor.MiddleCenter, Color = "1 1 1 1" }, RectTransform = { AnchorMin = "0.1 0.7", AnchorMax = "0.9 0.9" } }, panel);
            container.Add(new CuiElement
            {
                Parent = panel,
                Components =
                {
                    new CuiInputFieldComponent { Text = "", FontSize = 14, Align = TextAnchor.MiddleCenter, Color = "1 1 1 1", Command = "raid.startinput" },
                    new CuiRectTransformComponent { AnchorMin = "0.1 0.3", AnchorMax = "0.9 0.5" }
                }
            });
            container.Add(new CuiButton { Button = { Command = "raid.close RaidTimeInput", Color = "0.8 0.3 0.3 0.8" }, Text = { Text = "Cerrar", FontSize = 16, Align = TextAnchor.MiddleCenter, Color = "1 1 1 1" }, RectTransform = { AnchorMin = "0.1 0.1", AnchorMax = "0.9 0.2" } }, panel);
            CuiHelper.AddUi(player, container);
        }

        [ConsoleCommand("raid.startinput")]
        private void CmdStartRaid(ConsoleSystem.Arg arg)
        {
            BasePlayer player = arg.Player();
            if (arg.Args.Length == 0 || !int.TryParse(arg.Args[0], out int seconds)) return;
            if (raidCenter == Vector3.zero)
            {
                SendReply(player, "⚠️ Zona no marcada.");
                return;
            }
            remainingTime = seconds;
            raidActive = true;
            StartTimer();
            PrintToChat($"🚨 Raideo iniciado por {player.displayName} durante {seconds} segundos.");
        }

        private void StartTimer()
        {
            raidTimer = timer.Repeat(1f, remainingTime, () =>
            {
                remainingTime--;
                if (remainingTime <= 0)
                {
                    raidActive = false;
                    raidTimer?.Destroy();
                    PrintToChat("✅ El raideo ha finalizado. La zona está protegida.");
                }
                UpdateHUD();
            });
        }

        private void UpdateHUD()
        {
            foreach (ulong id in raidPlayers)
            {
                BasePlayer p = BasePlayer.FindByID(id);
                if (p == null) continue;
                CuiHelper.DestroyUi(p, HUD_NAME);
                var container = new CuiElementContainer();
                container.Add(new CuiPanel { Image = { Color = "0 0 0 0.5" }, RectTransform = { AnchorMin = "0.80 0.25", AnchorMax = "1.00 0.30" }, CursorEnabled = false }, "Overlay", HUD_NAME);
                container.Add(new CuiLabel { Text = { Text = $"⏱️ Raideo: {FormatTime(remainingTime)}", FontSize = 16, Align = TextAnchor.MiddleCenter, Color = "1 1 1 1" }, RectTransform = { AnchorMin = "0 0", AnchorMax = "1 1" } }, HUD_NAME);
                CuiHelper.AddUi(p, container);
            }
        }

        private string FormatTime(int seconds) => TimeSpan.FromSeconds(seconds).ToString(@"mm\:ss");

        object OnEntityTakeDamage(BaseCombatEntity entity, HitInfo info)
        {
            if (!raidActive && raidCenter != Vector3.zero && Vector3.Distance(entity.transform.position, raidCenter) <= raidRadius)
            {
                info?.InitiatorPlayer?.SendConsoleCommand("chat.add", 0, "⚠️ Zona protegida. El raideo ha terminado.");
                return true;
            }
            return null;
        }

        [ConsoleCommand("raid.close")]
        private void CmdClose(ConsoleSystem.Arg arg)
        {
            BasePlayer player = arg.Player();
            if (player == null || arg.Args == null || arg.Args.Length == 0) return;
            string panelName = arg.Args[0];
            CuiHelper.DestroyUi(player, panelName);
        }

        private void Unload()
        {
            raidTimer?.Destroy();
            foreach (var p in BasePlayer.activePlayerList)
                CuiHelper.DestroyUi(p, HUD_NAME);
        }

        private void Loaded()
        {
            permission.RegisterPermission("raidtimer.admin", this);
        }

        [ChatCommand("raidstop")]
        private void CmdRaidStop(BasePlayer player)
        {
            if (!permission.UserHasPermission(player.UserIDString, "raidtimer.admin"))
            {
                SendReply(player, "❌ No tienes permiso para usar este comando.");
                return;
            }
            if (!raidActive)
            {
                SendReply(player, "❌ No hay raideo activo.");
                return;
            }
            raidActive = false;
            raidTimer?.Destroy();
            remainingTime = 0;
            UpdateHUD(); // Limpia HUD
            PrintToChat("❌ Raideo cancelado por admin.");
        }
    }
}